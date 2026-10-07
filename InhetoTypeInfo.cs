using Fasterflect;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Numerics;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace Inheto
{
    // Replace 'Registry' with the actual static class that contains your two dictionaries.
    // public static class Registry { public static ConcurrentDictionary<RuntimeTypeHandle, InhetoPromotedTypes> InhetoPromotedTypesMap; public static ConcurrentDictionary<RuntimeTypeHandle, InhetoBaseTypes> InhetoBaseTypesMap; }

    /// <summary>
    /// Identifies the resolved CLR-to-wire representation family used by type metadata.<br/>
    /// </summary>
    public enum InhetoKindRoute : byte
    {
        /// <summary>
        /// Base representation route.<br/>
        /// </summary>
        Base = 0b0000_0000, 
        /// <summary>
        /// Nullable representation route.<br/>
        /// </summary>
        Nullable = 0b0100_0000, 
        /// <summary>
        /// Promoted representation route.<br/>
        /// </summary>
        Promoted = 0b1000_0000, 
        /// <summary>
        /// Recognized encoded collection route.<br/>
        /// </summary>
        Collection = 0b1100_0000
    }

    /// <summary>
    /// Describes a resolved CLR type representation, including nullable, array and collection information. Not every property applies to every representation family.<br/>
    /// </summary>
    public readonly struct InhetoTypeInfo
    {
        /// <summary>
        /// Gets the resolved representation family.<br/>
        /// </summary>
        public InhetoKindRoute Kind { get; init; }
        /// <summary>
        /// Gets whether the classified CLR type is a nullable value-type wrapper.<br/>
        /// </summary>
        public bool IsNullable { get; init; }
        /// <summary>
        /// Gets whether the collection representation carries auxiliary metadata, such as supported comparer information.<br/>
        /// </summary>
        public bool HasAuxiliary { get; init; } // set by collection detection when comparer info is known
        /// <summary>
        /// Includes Enum underlying type.
        /// </summary>
        public InhetoBaseTypes BaseType { get; init; }
        /// <summary>
        /// Gets the promoted type identifier when the resolved representation uses a promoted route.<br/>
        /// </summary>
        public InhetoPromotedTypes PromotedType { get; init; }
        /// <summary>
        /// Gets whether a candidate zero-input/static-class representation requires additional validation of the actual instance.<br/>
        /// </summary>
        public bool NeedsStaticClassInstanceValidation { get; init; }
        /// <summary>
        /// Gets the recognized encoded collection family, or its default None value when no family applies.<br/>
        /// </summary>
        public InhetoCollectionTypes CollectionType { get; init; }
        /// <summary>
        /// Gets the array representation category, or NonArray when this metadata is not for an array.<br/>
        /// </summary>
        public InhetoArrayTypes ArrayType { get; init; } // when PromotedType is Array/ArrayMD
        /// <summary>
        /// Gets the resolved nested array depth; interpret this together with the array representation rather than as a collection count.<br/>
        /// </summary>
        public int ArrayDepth { get; init; }
        /// <summary>
        /// Gets the resolved CLR array rank; not applicable to non-array representations.<br/>
        /// </summary>
        public int ArrayRank { get; init; }

        /// <summary>
        /// Gets the supported built-in comparer identifier when available; does not retain an arbitrary comparer instance.<br/>
        /// </summary>
        public InhetoComparerTypes ComparerType { get; init; } // optional, when detectable
        /// <summary>
        /// Used by collections and arrays to store the element type.
        /// </summary>
        public Type? ElementType { get; init; } // T or TValue
        /// <summary>
        /// Gets the CLR dictionary key type when the recognized collection family has keys; otherwise null.<br/>
        /// </summary>
        public Type? KeyType { get; init; }     // for dictionaries
        /// <summary>
        /// Gets the composed type-header byte for the resolved representation.<br/>
        /// </summary>
        public byte HeaderByte { get; init; }

        /// <summary>
        /// Gets whether the representation is the base-family ComplexType marker.<br/>
        /// </summary>
        public bool IsComplex => Kind == InhetoKindRoute.Base && BaseType == InhetoBaseTypes.ComplexType;

        /// <summary>
        /// Gets whether the resolved representation has an array category other than NonArray.<br/>
        /// </summary>
        public bool IsArray => ArrayType != InhetoArrayTypes.NonArray;
        /// <summary>
        /// Gets whether this metadata describes an encoded Collection route or the promoted Enumerable, NameValueCollection or StringCollection representation.<br/>
        /// </summary>
        public bool IsCollection
        {
            get
            {
                switch (Kind)
                {
                    case InhetoKindRoute.Base:
                        return false;
                    case InhetoKindRoute.Collection:
                        return true;
                    case InhetoKindRoute.Promoted when PromotedType == InhetoPromotedTypes.NameValueCollection:
                    case InhetoKindRoute.Promoted when PromotedType == InhetoPromotedTypes.Enumerable:
                    case InhetoKindRoute.Promoted when PromotedType == InhetoPromotedTypes.StringCollection:
                        return true;

                }
                return Kind == InhetoKindRoute.Collection || (Kind == InhetoKindRoute.Promoted && PromotedType == InhetoPromotedTypes.Enumerable);
            }
        }
        /// <summary>
        /// Gets whether the representation is the base-family Null marker.<br/>
        /// </summary>
        public bool IsNull => Kind == InhetoKindRoute.Base && BaseType == InhetoBaseTypes.Null;
    }

    /// <summary>
    /// Resolves and caches type-only Inheto representation metadata; it does not inspect instance values or prove that arbitrary user code can round-trip.<br/>
    /// </summary>
    public static class InhetoTypeResolver
    {
        private const byte KIND_MASK = 0b1100_0000;
        private const byte AUX_BIT = 0b0010_0000;

        private static readonly ConcurrentDictionary<(RuntimeTypeHandle TypeHandle, MemberTypesEnum MemberTypes), InhetoTypeInfo> Cache = new();

        /// <summary>
        /// Resolves and caches the Inheto representation for a CLR type and member-selection policy.<br/>
        /// The member-selection policy participates in the cache key because zero-input/static-class
        /// classification depends on which properties and fields are visible.<br/>
        /// The stateful <see cref="ConcurrentDictionary{TKey,TValue}.GetOrAdd{TArg}(TKey, Func{TKey,TArg,TValue}, TArg)"/>
        /// overload avoids allocating a capturing factory delegate on every lookup.<br/>
        /// </summary>
        /// <param name="t">The CLR type to classify.</param>
        /// <param name="staticClassTypesLevel">The member-selection policy used for shape classification.</param>
        /// <returns>The immutable Inheto type description for the requested type and policy.</returns>
        public static InhetoTypeInfo GetInhetoTypeInfo(Type t, MemberTypesEnum staticClassTypesLevel = MemberTypesEnum.PublicPropertiesAndFields)
        {
            var key = (t.TypeHandle, staticClassTypesLevel);
            return Cache.GetOrAdd(
                key,
                static (cacheKey, runtimeType) => Resolve(runtimeType, cacheKey.MemberTypes),
                t);
        }

        /// <summary>
        /// Tests whether type resolution returns the base-family ComplexType representation; does not serialize or inspect an instance.<br/>
        /// </summary>
        /// <param name="t">CLR type whose supported representation or dictionary arguments are inspected.<br/></param>
        /// <returns>True when the documented predicate is satisfied; otherwise false.<br/></returns>
        public static bool IsComplexType(Type t)
        {
            var ti = GetInhetoTypeInfo(t);
            return ti.Kind == InhetoKindRoute.Base && ti.BaseType == InhetoBaseTypes.ComplexType;
        }
        /// <summary>
        /// Tests whether type resolution uses the encoded Collection route. This helper is narrower than InhetoTypeInfo.IsCollection, which also admits some promoted collection representations.<br/>
        /// </summary>
        /// <param name="t">CLR type whose supported representation or dictionary arguments are inspected.<br/></param>
        /// <returns>True when the documented predicate is satisfied; otherwise false.<br/></returns>
        public static bool IsCollection(Type t) => GetInhetoTypeInfo(t).Kind == InhetoKindRoute.Collection;
        /// <summary>
        /// Gets whether the resolved representation has an array category other than NonArray.<br/>
        /// </summary>
        /// <param name="t">CLR type whose supported representation or dictionary arguments are inspected.<br/></param>
        /// <returns>True when the documented predicate is satisfied; otherwise false.<br/></returns>
        public static bool IsArray(Type t) => GetInhetoTypeInfo(t).ArrayType != InhetoArrayTypes.NonArray;

        private static InhetoTypeInfo Resolve(Type t, MemberTypesEnum staticClassTypesLevel = MemberTypesEnum.PublicPropertiesAndFields)
        {
            // Nullable<T>
            var tName = t.FullName;
            var underlying = Nullable.GetUnderlyingType(t);
            if (underlying != null)
            {
                var inner = GetInhetoTypeInfo(underlying);
                return inner with { IsNullable = true };
            }

            if (t.IsEnum)
            {
                var eType = t.GetEnumUnderlyingType();
                if (InhetoSerializer.InhetoBaseTypesMap.TryGetValue(eType.TypeHandle, out var et))
                {
                    var header = (byte)((byte)InhetoKindRoute.Promoted | (byte)InhetoPromotedTypes.Enum);
                    return new InhetoTypeInfo
                    {
                        Kind = InhetoKindRoute.Promoted,
                        PromotedType = InhetoPromotedTypes.Enum,
                        BaseType = et,
                        HeaderByte = header
                    };
                }
            }

            // Base types via your map
            if (InhetoSerializer.InhetoBaseTypesMap.TryGetValue(t.TypeHandle, out var bt))
            {
                var header = (byte)((byte)InhetoKindRoute.Base | (byte)bt);
                return new InhetoTypeInfo
                {
                    Kind = InhetoKindRoute.Base,
                    BaseType = bt,
                    HeaderByte = header
                };
            }

            // Promoted types via your map
            if (InhetoSerializer.InhetoPromotedTypesMap.TryGetValue(t.TypeHandle, out var pt))
            {
                var header = (byte)((byte)InhetoKindRoute.Promoted | (byte)pt);
                return new InhetoTypeInfo
                {
                    Kind = InhetoKindRoute.Promoted,
                    PromotedType = pt,
                    HeaderByte = header,
                };
            }

            // Arrays (promoted Array/ArrayMD; values are coupled to InhetoArrayTypes)
            if (t.IsArray)
            {
                var info = ParseArray(t);
                var at = info.Rank == 1 ? InhetoArrayTypes.Array : InhetoArrayTypes.ArrayMD;
                var header = (byte)((byte)InhetoKindRoute.Promoted | (byte)at);
                return new InhetoTypeInfo
                {
                    Kind = InhetoKindRoute.Promoted,
                    ArrayType = at,
                    ArrayDepth = info.Depth,
                    ArrayRank = info.Rank,
                    PromotedType = (InhetoPromotedTypes)(byte)at,
                    ElementType = info.ElementType,
                    HeaderByte = header
                };
            }

            // Generic collections, including concrete subclasses of a supported generic collection base.
            for (Type? genericCollectionType = t; genericCollectionType is not null; genericCollectionType = genericCollectionType.BaseType)
            {
                if (!genericCollectionType.IsGenericType) continue;
                var def = genericCollectionType.GetGenericTypeDefinition();

                // Lists, queues, stacks, readonly, observable, concurrent bag/queue/stack
                // the switch to text based comparisons is for performance and avoidance of type loading if not already loaded.
                if (def == typeof(List<>) || def == typeof(LinkedList<>) || def == typeof(Queue<>) ||
                    def == typeof(Stack<>) || def == typeof(ReadOnlyCollection<>) || def == typeof(SortedSet<>) || def == typeof(HashSet<>) ||
                    def.FullName == "System.Collections.ObjectModel.ObservableCollection`1" ||
                    def.FullName == "System.Collections.Concurrent.ConcurrentBag`1" ||
                    def.FullName == "System.Collections.Concurrent.ConcurrentQueue`1" ||
                    def.FullName == "System.Collections.Concurrent.ConcurrentStack`1")
                {
                    var ct =
                        def == typeof(List<>) ? InhetoCollectionTypes.List :
                        def == typeof(Queue<>) ? InhetoCollectionTypes.Queue :
                        def == typeof(Stack<>) ? InhetoCollectionTypes.Stack :
                        def == typeof(HashSet<>) ? InhetoCollectionTypes.HashSet :
                        def == typeof(SortedSet<>) ? InhetoCollectionTypes.SortedSet :
                        def == typeof(ConcurrentQueue<>) ? InhetoCollectionTypes.ConcurrentQueue :
                        def.FullName == "System.Collections.ObjectModel.ObservableCollection`1" ? InhetoCollectionTypes.ObservableCollection :
                        def.FullName == "System.Collections.Concurrent.ConcurrentBag`1" ? InhetoCollectionTypes.ConcurrentBag :
                        def.FullName == "System.Collections.Concurrent.ConcurrentQueue`1" ? InhetoCollectionTypes.ConcurrentQueue :
                        InhetoCollectionTypes.ConcurrentStack;

                    var header = (byte)((byte)InhetoKindRoute.Collection | (byte)ct);
                    return new InhetoTypeInfo
                    {
                        Kind = InhetoKindRoute.Collection,
                        CollectionType = ct,
                        ElementType = genericCollectionType.GetGenericArguments()[0],
                        HeaderByte = header
                    };
                }

                // Dictionaries and sorted variants
                if (def == typeof(Dictionary<,>) || def == typeof(SortedDictionary<,>) || def == typeof(ReadOnlyDictionary<,>) ||
                    def.FullName == "System.Collections.Concurrent.ConcurrentDictionary`2" ||
                    def == typeof(SortedList<,>))
                {

                    var ct =
                        def == typeof(Dictionary<,>) ? InhetoCollectionTypes.Dictionary :
                        def.FullName == "System.Collections.Concurrent.ConcurrentDictionary`2" ? InhetoCollectionTypes.ConcurrentDictionary :
                        def == typeof(SortedDictionary<,>) ? InhetoCollectionTypes.SortedDictionary :
                        InhetoCollectionTypes.SortedList;

                    var dTypes = UtilsAndExtensions.GetDictTypes(genericCollectionType);
                    if (dTypes.KeyType == typeof(string))
                    {
                        switch (ct)
                        {
                            case InhetoCollectionTypes.Dictionary: ct = InhetoCollectionTypes.StringKeyDictionary; break;
                            case InhetoCollectionTypes.ConcurrentDictionary: ct = InhetoCollectionTypes.StringKeyConcurrentDictionary; break;
                            case InhetoCollectionTypes.SortedDictionary: ct = InhetoCollectionTypes.StringKeySortedDictionary; break;
                            case InhetoCollectionTypes.SortedList: ct = InhetoCollectionTypes.StringKeySortedList; break;
                        }
                    }


                    // Instance-level comparer detection would set HasAuxiliary and ComparerType. Type-level leaves false.
                    var hasAux = false;
                    var header = (byte)((byte)InhetoKindRoute.Collection | (hasAux ? AUX_BIT : (byte)0) | (byte)ct);

                    return new InhetoTypeInfo
                    {
                        Kind = InhetoKindRoute.Collection,
                        CollectionType = ct,
                        HasAuxiliary = hasAux,
                        KeyType = dTypes.KeyType,
                        ElementType = dTypes.ValueType,
                        HeaderByte = header
                    };
                }
            }

            // check for zero-zero type
            Flags typeCheckFlags = Flags.InstancePublic;
            switch (staticClassTypesLevel)
            {
                case MemberTypesEnum.PublicPropertiesAndFields: typeCheckFlags = Flags.InstancePublic; break;
                case MemberTypesEnum.PublicAndPrivateFields: typeCheckFlags = Flags.InstanceAnyVisibility; break;
                case MemberTypesEnum.PublicFieldsOnly: typeCheckFlags = Flags.InstancePublic; break;
                case MemberTypesEnum.PublicPropertiesOnly: typeCheckFlags = Flags.InstancePublic; break;
                case MemberTypesEnum.PrivateFieldsOnly: typeCheckFlags = Flags.InstancePrivate; break;
            }

            // Declared interfaces and abstract bases describe a destination shape, not a
            // possible runtime singleton. Let normal complex materialization use its
            // supplied instance or activator. Abstract-sealed static types still qualify.
            if ((!t.IsAbstract || t.IsSealed) && ZeroInputSingletonDetector.IsMaybeZeroInputType(t))
                return new InhetoTypeInfo
                {
                    Kind = InhetoKindRoute.Promoted,
                    PromotedType = InhetoPromotedTypes.StaticClass,
                    NeedsStaticClassInstanceValidation = true,
                    HeaderByte = (byte)((byte)InhetoKindRoute.Promoted | (byte)InhetoPromotedTypes.StaticClass)
                };


            // Fallback: ComplexType (graph it)
            return new InhetoTypeInfo
            {
                Kind = InhetoKindRoute.Base,
                BaseType = InhetoBaseTypes.ComplexType,
                HeaderByte = (byte)((byte)InhetoKindRoute.Base | (byte)InhetoBaseTypes.ComplexType)
            };

            (int Depth, int Rank, Type ElementType) ParseArray(Type t)
            {
                if (!t.IsArray)
                    return (0, 0, t);

                int depth = 0;
                int rank = 1;           // default if all 1-D
                Type et = t;

                while (et.IsArray)
                {
                    int r = et.GetArrayRank();

                    if (r == 1)
                    {
                        depth++;        // count jagged layers
                        et = et.GetElementType()!;
                    }
                    else
                    {
                        // we've hit a multidimensional array, stop there
                        rank = r;
                        et = et.GetElementType()!;
                        depth++;        // this layer still counts as one array
                        break;
                    }
                }

                return (depth, rank, et);
            }

        }
    }


    /// <summary>
    /// Resolves a CLR Type from a NameHeaderEntry's encoded type.<br/>
    /// Returns null when not deterministically resolvable from enums (e.g., Enum kind, StaticClass, Complex/Custom, etc.).<br/>
    /// Supports Base, Nullable(Base), Promoted framework types, Arrays/MD-Arrays/Enumerable, and Collection families.
    /// </summary>
    internal static class InhetoToTypeResolver
    {
        public static Type? Resolve(in NameHeaderEntry entry) => Resolve(in entry.Type);

        public static Type? Resolve(in NameHeaderEntryType t) =>
            t.Kind switch
            {
                InhetoKindMasks.InhetoBaseType => MapBase(t.BaseType),
                InhetoKindMasks.InhetoNullableBaseType => MakeNullable(MapBase(t.NullableBaseType)),
                InhetoKindMasks.InhetoPromotedType => MapPromoted(in t),
                InhetoKindMasks.InhetoCollectionType => MapCollection(in t),
                _ => null
            };

        private static Type? MakeNullable(Type? inner) =>
            inner is null ? null : (inner.IsValueType ? typeof(Nullable<>).MakeGenericType(inner) : null);

        private static Type? MapBase(InhetoBaseTypes b) => b switch
        {
            InhetoBaseTypes.Boolean => typeof(bool),
            InhetoBaseTypes.Char => typeof(char),
            InhetoBaseTypes.Rune => typeof(Rune),
            InhetoBaseTypes.String => typeof(string),

            InhetoBaseTypes.SByte => typeof(sbyte),
            InhetoBaseTypes.Byte => typeof(byte),
            InhetoBaseTypes.Int16 => typeof(short),
            InhetoBaseTypes.UInt16 => typeof(ushort),
            InhetoBaseTypes.Int32 => typeof(int),
            InhetoBaseTypes.UInt32 => typeof(uint),
            InhetoBaseTypes.Int64 => typeof(long),
            InhetoBaseTypes.UInt64 => typeof(ulong),

            InhetoBaseTypes.Int128 => typeof(System.Int128),
            InhetoBaseTypes.UInt128 => typeof(System.UInt128),

            InhetoBaseTypes.Half => typeof(Half),
            InhetoBaseTypes.Single => typeof(float),
            InhetoBaseTypes.Double => typeof(double),
            InhetoBaseTypes.Decimal => typeof(decimal),

            InhetoBaseTypes.DateOnly => typeof(DateOnly),
            InhetoBaseTypes.DateTime => typeof(DateTime),
            InhetoBaseTypes.TimeSpan => typeof(TimeSpan),
            InhetoBaseTypes.TimeOnly => typeof(TimeOnly),
            InhetoBaseTypes.DateTimeOffset => typeof(DateTimeOffset),
            InhetoBaseTypes.Guid => typeof(Guid),
            InhetoBaseTypes.Version => typeof(Version),
            InhetoBaseTypes.Uri => typeof(Uri),

            InhetoBaseTypes.Vector2 => typeof(Vector2),
            InhetoBaseTypes.Vector3 => typeof(Vector3),
            InhetoBaseTypes.Vector4 => typeof(Vector4),
            InhetoBaseTypes.ComplexNumeric => typeof(Complex),
            InhetoBaseTypes.Quaternion => typeof(Quaternion),
            InhetoBaseTypes.Plane => typeof(Plane),
            InhetoBaseTypes.Matrix3x2 => typeof(Matrix3x2),
            InhetoBaseTypes.Matrix4x4 => typeof(Matrix4x4),
            InhetoBaseTypes.BigInteger => typeof(BigInteger),

            InhetoBaseTypes.ByteArray => typeof(byte[]),
            InhetoBaseTypes.ListString => typeof(List<string>),

            _ => null // Unknown/PromotedType/Custom*/Duplicate*/Inherited/ComplexType/Null/EndOfNamesHeader
        };

        private static Type? MapPromoted(in NameHeaderEntryType t)
        {
            switch (t.PromotedType)
            {
                case InhetoPromotedTypes.Array:
                case InhetoPromotedTypes.ArrayMD:
                    return MapArray(in t.ArrayInfo);

                case InhetoPromotedTypes.Enumerable:
                    {
                        var tet = t.EnumerableInfo.ElementType;
                        var et = Resolve(in tet);
                        return et is null ? null : typeof(IEnumerable<>).MakeGenericType(et);
                    }

                case InhetoPromotedTypes.Enum:
                    return MapBase(t.EnumType);

                case InhetoPromotedTypes.StaticClass:
                    return null; // identity not available here

                // Special collection-like non-generic BCL types
                case InhetoPromotedTypes.NameValueCollection: return typeof(NameValueCollection);
                case InhetoPromotedTypes.StringCollection: return typeof(StringCollection);

                // Framework object graph “promoted” types
                case InhetoPromotedTypes.CultureInfo: return typeof(CultureInfo);
                case InhetoPromotedTypes.IPAddress: return typeof(IPAddress);
                case InhetoPromotedTypes.IPEndPoint: return typeof(IPEndPoint);
                case InhetoPromotedTypes.MailAddress: return typeof(MailAddress);
                case InhetoPromotedTypes.MemoryByte: return typeof(Memory<byte>);
                case InhetoPromotedTypes.MemoryStream: return typeof(MemoryStream);
                case InhetoPromotedTypes.ReadOnlyMemoryByte: return typeof(ReadOnlyMemory<byte>);
                case InhetoPromotedTypes.Regex: return typeof(Regex);
                case InhetoPromotedTypes.SecureString: return typeof(SecureString);
                case InhetoPromotedTypes.StringBuilder: return typeof(StringBuilder);
                case InhetoPromotedTypes.TimeZoneInfo: return typeof(TimeZoneInfo);
                case InhetoPromotedTypes.Type: return typeof(Type);

                // Comparer markers → public base type only
                case InhetoPromotedTypes.CurrentCulture:
                case InhetoPromotedTypes.CurrentCultureIgnoreCase:
                case InhetoPromotedTypes.InvariantCulture:
                case InhetoPromotedTypes.InvariantCultureIgnoreCase:
                case InhetoPromotedTypes.Ordinal:
                case InhetoPromotedTypes.OrdinalIgnoreCase:
                    return typeof(StringComparer);

                default:
                    return null;
            }
        }

        private static Type? MapArray(in NameHeaderArrayEntry a)
        {
            var aet = a.ElementType;
            var et = Resolve(in aet);
            if (et is null) return null;

            if (a.ArrayType == InhetoArrayTypes.ArrayMD)
                return et.MakeArrayType(Math.Max((byte)1, a.Rank));

            // Jagged/unknown depth arrays
            var t = et;
            var depth = Math.Max(1, (int)a.Depth);
            for (int i = 0; i < depth; i++) t = t.MakeArrayType();
            return t;
        }

        private static Type? MapCollection(in NameHeaderEntryType t)
        {
            var info = t.CollectionInfo;
            var ivt = info.ValueType;
            var vt = Resolve(in ivt);
            if (vt is null) return null;

            switch (info.CollectionType)
            {
                // Lists/sets
                case InhetoCollectionTypes.List: return typeof(List<>).MakeGenericType(vt);
                case InhetoCollectionTypes.Queue: return typeof(Queue<>).MakeGenericType(vt);
                case InhetoCollectionTypes.ConcurrentQueue: return typeof(ConcurrentQueue<>).MakeGenericType(vt);
                case InhetoCollectionTypes.Stack: return typeof(Stack<>).MakeGenericType(vt);
                case InhetoCollectionTypes.ConcurrentStack: return typeof(ConcurrentStack<>).MakeGenericType(vt);
                case InhetoCollectionTypes.ObservableCollection: return typeof(ObservableCollection<>).MakeGenericType(vt);
                case InhetoCollectionTypes.ConcurrentBag: return typeof(ConcurrentBag<>).MakeGenericType(vt);
                case InhetoCollectionTypes.SortedSet: return typeof(SortedSet<>).MakeGenericType(vt);
                case InhetoCollectionTypes.HashSet: return typeof(HashSet<>).MakeGenericType(vt);

                // Dictionaries (incl. string-key shortcuts)
                case InhetoCollectionTypes.Dictionary:
                case InhetoCollectionTypes.ConcurrentDictionary:
                case InhetoCollectionTypes.SortedDictionary:
                case InhetoCollectionTypes.SortedList:
                case InhetoCollectionTypes.StringKeyDictionary:
                case InhetoCollectionTypes.StringKeyConcurrentDictionary:
                case InhetoCollectionTypes.StringKeySortedDictionary:
                case InhetoCollectionTypes.StringKeySortedList:
                    {
                        var kt = info.KeyType is NameHeaderEntryType k ? Resolve(in k) : null;
                        if (kt is null || vt is null) return null;

                        return info.CollectionType switch
                        {
                            InhetoCollectionTypes.Dictionary => typeof(Dictionary<,>).MakeGenericType(kt, vt),
                            InhetoCollectionTypes.ConcurrentDictionary => typeof(ConcurrentDictionary<,>).MakeGenericType(kt, vt),
                            InhetoCollectionTypes.SortedDictionary => typeof(SortedDictionary<,>).MakeGenericType(kt, vt),
                            InhetoCollectionTypes.SortedList => typeof(SortedList<,>).MakeGenericType(kt, vt),
                            InhetoCollectionTypes.StringKeyDictionary => typeof(Dictionary<,>).MakeGenericType(kt, vt),
                            InhetoCollectionTypes.StringKeyConcurrentDictionary => typeof(ConcurrentDictionary<,>).MakeGenericType(kt, vt),
                            InhetoCollectionTypes.StringKeySortedDictionary => typeof(SortedDictionary<,>).MakeGenericType(kt, vt),
                            InhetoCollectionTypes.StringKeySortedList => typeof(SortedList<,>).MakeGenericType(kt, vt),
                            _ => null
                        };
                    }

                default:
                    return null;
            }
        }
    }

}
