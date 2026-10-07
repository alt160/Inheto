using Fasterflect;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;







namespace Inheto
{
    //====== TYPES ======
    /// <summary>
    /// Writes an object graph into a reusable pooled BufferStream, with addressable member and collection-element headers. One instance and its returned stream must not be used concurrently.<br/>
    /// </summary>
    public partial class InhetoSerializer
    {
        //Shared/Static Members
        private static readonly ConcurrentDictionary<Type, Func<object, object>> _cacheGetLinkedListValue =
            new ConcurrentDictionary<Type, Func<object, object>>();
        static int GetEnumerableCount(IEnumerable? enumerable)
        {
            if (enumerable == null) return 0;
            var count = enumerable.GetPropertyValue("Count") as int? ?? 0;
            if (count == 0)
                foreach (var _ in enumerable)
                    count++;
            return count;
        }
        // Fixed implementation tables, not developer registrations or resolved shape-metadata caches.
        internal static readonly System.Collections.Concurrent.ConcurrentDictionary<RuntimeTypeHandle, InhetoBaseTypes> InhetoBaseTypesMap = new ConcurrentDictionary<RuntimeTypeHandle, InhetoBaseTypes>
        {
            // numerics
            [typeof(bool).TypeHandle] = InhetoBaseTypes.Boolean,
            [typeof(byte).TypeHandle] = InhetoBaseTypes.Byte,
            [typeof(sbyte).TypeHandle] = InhetoBaseTypes.SByte,
            [typeof(short).TypeHandle] = InhetoBaseTypes.Int16,
            [typeof(ushort).TypeHandle] = InhetoBaseTypes.UInt16,
            [typeof(Half).TypeHandle] = InhetoBaseTypes.Half,
            [typeof(int).TypeHandle] = InhetoBaseTypes.Int32,
            [typeof(uint).TypeHandle] = InhetoBaseTypes.UInt32,
            [typeof(float).TypeHandle] = InhetoBaseTypes.Single,
            [typeof(long).TypeHandle] = InhetoBaseTypes.Int64,
            [typeof(ulong).TypeHandle] = InhetoBaseTypes.UInt64,
            [typeof(double).TypeHandle] = InhetoBaseTypes.Double,
            [typeof(Int128).TypeHandle] = InhetoBaseTypes.Int128,
            [typeof(UInt128).TypeHandle] = InhetoBaseTypes.UInt128,
            [typeof(decimal).TypeHandle] = InhetoBaseTypes.Decimal,

            // string based
            [typeof(char).TypeHandle] = InhetoBaseTypes.Char,
            [typeof(Rune).TypeHandle] = InhetoBaseTypes.Rune,
            [typeof(string).TypeHandle] = InhetoBaseTypes.String,

            // valuetypes
            [typeof(DateOnly).TypeHandle] = InhetoBaseTypes.DateOnly,
            [typeof(DateTime).TypeHandle] = InhetoBaseTypes.DateTime,
            [typeof(TimeSpan).TypeHandle] = InhetoBaseTypes.TimeSpan,
            [typeof(TimeOnly).TypeHandle] = InhetoBaseTypes.TimeOnly,
            [typeof(Guid).TypeHandle] = InhetoBaseTypes.Guid,
            [typeof(DateTimeOffset).TypeHandle] = InhetoBaseTypes.DateTimeOffset,
            [typeof(Version).TypeHandle] = InhetoBaseTypes.Version,
            [typeof(Uri).TypeHandle] = InhetoBaseTypes.Uri,

            // system.numerics types
            [typeof(Vector2).TypeHandle] = InhetoBaseTypes.Vector2,
            [typeof(Vector3).TypeHandle] = InhetoBaseTypes.Vector3,
            [typeof(Vector4).TypeHandle] = InhetoBaseTypes.Vector4,
            [typeof(Complex).TypeHandle] = InhetoBaseTypes.ComplexNumeric,
            [typeof(Quaternion).TypeHandle] = InhetoBaseTypes.Quaternion,
            [typeof(Plane).TypeHandle] = InhetoBaseTypes.Plane,
            [typeof(Matrix3x2).TypeHandle] = InhetoBaseTypes.Matrix3x2,
            [typeof(Matrix4x4).TypeHandle] = InhetoBaseTypes.Matrix4x4,
            [typeof(BigInteger).TypeHandle] = InhetoBaseTypes.BigInteger,

            // promoted cases
            [typeof(byte[]).TypeHandle] = InhetoBaseTypes.ByteArray,
            [typeof(List<string>).TypeHandle] = InhetoBaseTypes.ListString,
        };
        //Shared/Static Members
        /// <summary>
        /// Promoted types checked are:<br/>
        /// - NameValueCollection<br/>
        /// - StringCollection<br/>
        /// - MemoryStream<br/>
        /// - MemoryByte<br/>
        /// - ReadOnlyMemoryByte<br/>
        /// - StringBuilder<br/>
        /// - SecureString<br/>
        /// - IPAddress<br/>
        /// - IPEndPoint<br/>
        /// - MailAddress<br/>
        /// - Regex<br/>
        /// - CultureInfo<br/>
        /// - TimeZoneInfo<br/>
        /// - BitArray<br/>
        /// - Type<br/>
        /// 
        /// </summary>
        internal static readonly System.Collections.Concurrent.ConcurrentDictionary<RuntimeTypeHandle, InhetoPromotedTypes> InhetoPromotedTypesMap = new ConcurrentDictionary<RuntimeTypeHandle, InhetoPromotedTypes>
        {
            [typeof(NameValueCollection).TypeHandle] = InhetoPromotedTypes.NameValueCollection,
            [typeof(StringCollection).TypeHandle] = InhetoPromotedTypes.StringCollection,
            [typeof(MemoryStream).TypeHandle] = InhetoPromotedTypes.MemoryStream,
            [typeof(Memory<byte>).TypeHandle] = InhetoPromotedTypes.MemoryByte,
            [typeof(ReadOnlyMemory<byte>).TypeHandle] = InhetoPromotedTypes.ReadOnlyMemoryByte,
            [typeof(StringBuilder).TypeHandle] = InhetoPromotedTypes.StringBuilder,
            [typeof(SecureString).TypeHandle] = InhetoPromotedTypes.SecureString,
            [typeof(IPAddress).TypeHandle] = InhetoPromotedTypes.IPAddress,
            [IPAddress.Loopback.GetType().TypeHandle] = InhetoPromotedTypes.IPAddress,
            [typeof(IPEndPoint).TypeHandle] = InhetoPromotedTypes.IPEndPoint,
            [typeof(MailAddress).TypeHandle] = InhetoPromotedTypes.MailAddress,
            [typeof(Regex).TypeHandle] = InhetoPromotedTypes.Regex,
            [typeof(CultureInfo).TypeHandle] = InhetoPromotedTypes.CultureInfo,
            [CultureInfo.InvariantCulture.GetType().TypeHandle] = InhetoPromotedTypes.CultureInfo,
            [typeof(TimeZoneInfo).TypeHandle] = InhetoPromotedTypes.TimeZoneInfo,
            [typeof(Type).TypeHandle] = InhetoPromotedTypes.Type,
            [typeof(Type).GetType().TypeHandle] = InhetoPromotedTypes.Type,
        };
        private static HashSet<InhetoBaseTypes> inlinedValueTypes = new()
        {
            InhetoBaseTypes.Boolean,
            InhetoBaseTypes.SByte,
            InhetoBaseTypes.Byte,
            InhetoBaseTypes.Int16,
            InhetoBaseTypes.UInt16,
            InhetoBaseTypes.Int32,
            InhetoBaseTypes.UInt32,
            InhetoBaseTypes.Char,
            InhetoBaseTypes.Rune,
            InhetoBaseTypes.Half,
            InhetoBaseTypes.Single,
            InhetoBaseTypes.DateOnly,
        };
        /// <summary>
        /// Heuristically tests whether the type, or the leaf type of an array, requires complex-object handling rather than a registered scalar/promoted representation. Uses public instance member metadata and does not inspect an instance.<br/>
        /// </summary>
        /// <param name="oType">CLR type whose scalar/promoted or complex classification is inspected.<br/></param>
        /// <returns>True when the documented predicate is satisfied; otherwise false.<br/></returns>
        public static bool IsInhetoComplexType(Type oType)
        {
            if (InhetoBaseTypesMap.TryGetValue(oType.TypeHandle, out var baseType) && baseType != InhetoBaseTypes.ListString) return false;
            if (InhetoPromotedTypesMap.ContainsKey(oType.TypeHandle)) return false;
            if (oType.IsArray)
            {
                var eType = oType.GetElementType()!;
                while (eType.IsArray)
                {
                    eType = eType.GetElementType()!;
                }
                return IsInhetoComplexType(eType);
            }
            if (oType.IsEnum) return false;
            if (oType.Properties(Flags.InstancePublic).Count + oType.Fields(Flags.InstancePublic).Count == 0) return false;
            return true;
        }
        internal static bool IsSafeKeyType(Type valueType)
        {
            if (valueType == null) return false;
            if (InhetoBaseTypesMap.TryGetValue(valueType.TypeHandle, out var iType))
                switch (iType)
                {
                    case InhetoBaseTypes.Unknown: return false;
                    case InhetoBaseTypes.Char:
                    case InhetoBaseTypes.Rune:
                    case InhetoBaseTypes.String:
                    case InhetoBaseTypes.Boolean:
                    case InhetoBaseTypes.Byte:
                    case InhetoBaseTypes.SByte:
                    case InhetoBaseTypes.Int16:
                    case InhetoBaseTypes.UInt16:
                    case InhetoBaseTypes.Int32:
                    case InhetoBaseTypes.UInt32:
                    case InhetoBaseTypes.Int64:
                    case InhetoBaseTypes.UInt64:
                    case InhetoBaseTypes.Int128:
                    case InhetoBaseTypes.UInt128:
                    case InhetoBaseTypes.Half:
                    case InhetoBaseTypes.Single:
                    case InhetoBaseTypes.Double:
                    case InhetoBaseTypes.Decimal:
                    case InhetoBaseTypes.BigInteger:
                    case InhetoBaseTypes.DateOnly:
                    case InhetoBaseTypes.DateTime:
                    case InhetoBaseTypes.DateTimeOffset:
                    case InhetoBaseTypes.Guid:
                    case InhetoBaseTypes.TimeOnly:
                    case InhetoBaseTypes.TimeSpan:
                    case InhetoBaseTypes.Uri:
                    case InhetoBaseTypes.Version: return true;
                }
            else if (InhetoPromotedTypesMap.TryGetValue(valueType.TypeHandle, out var pType))
                switch (pType)
                {
                    case InhetoPromotedTypes.IPAddress: return true;
                    case InhetoPromotedTypes.CurrentCulture: return true;
                    case InhetoPromotedTypes.TimeZoneInfo: return true;
                    case InhetoPromotedTypes.IPEndPoint: return true;
                    case InhetoPromotedTypes.Type: return true;
                }
            return false;
        }
        /// <summary>
        /// Tests whether a CLR type implements a closed ISet interface; this is a shape test, not a probe of collection contents.<br/>
        /// </summary>
        /// <param name="type">Exact CLR type used for registration, lookup or runtime shape analysis.<br/></param>
        /// <returns>True when the documented predicate is satisfied; otherwise false.<br/></returns>
        public static bool IsSetType(Type type)
        {
            return type.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISet<>));
        }
        /// <summary>
        /// Formats an admitted dictionary key using the existing wire-text representation.<br/>
        /// Returns null when no supported representation is available; callers must not turn that into an empty key.<br/>
        /// An actual empty string key remains valid and is returned unchanged.<br/>
        /// </summary>
        /// <param name="value">The dictionary key to format, including a possible invalid null key.<br/></param>
        /// <returns>Existing key text, or null for an absent, unsupported or unrepresentable key.<br/></returns>
        internal static string? SafeKeyTypeToString(object? value)
        {
            if (value == null) return null;
            if (value is string s) return s;
            if (!IsSafeKeyType(value.GetType())) return null;
            if (InhetoBaseTypesMap.TryGetValue(value.GetType().TypeHandle, out var iType))
                switch (iType)
                {
                    case InhetoBaseTypes.Unknown: return null;
                    case InhetoBaseTypes.Char: return ((char)value).ToString();
                    case InhetoBaseTypes.Rune: return ((Rune)value).ToString();
                    case InhetoBaseTypes.String: return (string)value;
                    case InhetoBaseTypes.Boolean: return ((bool)value).ToString();
                    case InhetoBaseTypes.Byte: return ((byte)value).ToString("D", CultureInfo.InvariantCulture);
                    case InhetoBaseTypes.SByte: return ((sbyte)value).ToString("D", CultureInfo.InvariantCulture);
                    case InhetoBaseTypes.Int16: return ((short)value).ToString("D", CultureInfo.InvariantCulture);
                    case InhetoBaseTypes.UInt16: return ((ushort)value).ToString("D", CultureInfo.InvariantCulture);
                    case InhetoBaseTypes.Int32: return ((int)value).ToString("D", CultureInfo.InvariantCulture);
                    case InhetoBaseTypes.UInt32: return ((uint)value).ToString("D", CultureInfo.InvariantCulture);
                    case InhetoBaseTypes.Int64: return ((long)value).ToString("D", CultureInfo.InvariantCulture);
                    case InhetoBaseTypes.UInt64: return ((ulong)value).ToString("D", CultureInfo.InvariantCulture);
                    case InhetoBaseTypes.Int128: return ((Int128)value).ToString("D", CultureInfo.InvariantCulture);
                    case InhetoBaseTypes.UInt128: return ((UInt128)value).ToString("D", CultureInfo.InvariantCulture);
                    case InhetoBaseTypes.BigInteger: return ((BigInteger)value).ToString("D", CultureInfo.InvariantCulture);
                    case InhetoBaseTypes.Half: return ((Half)value).ToString("R", CultureInfo.InvariantCulture);
                    case InhetoBaseTypes.Single: return ((float)value).ToString("R", CultureInfo.InvariantCulture);
                    case InhetoBaseTypes.Double: return ((double)value).ToString("R", CultureInfo.InvariantCulture);
                    case InhetoBaseTypes.Decimal: return ((decimal)value).ToString("G", CultureInfo.InvariantCulture);
                    case InhetoBaseTypes.DateOnly: return ((DateOnly)value).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    case InhetoBaseTypes.DateTime: return ((DateTime)value).ToString("o", CultureInfo.InvariantCulture);
                    case InhetoBaseTypes.DateTimeOffset: return ((DateTimeOffset)value).ToString("O", CultureInfo.InvariantCulture);
                    case InhetoBaseTypes.TimeOnly: return ((TimeOnly)value).ToString("HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
                    case InhetoBaseTypes.TimeSpan: return ((TimeSpan)value).ToString("c", CultureInfo.InvariantCulture);
                    case InhetoBaseTypes.Guid: return ((Guid)value).ToString();
                    case InhetoBaseTypes.Uri: return ((Uri)value).OriginalString;
                    case InhetoBaseTypes.Version: return ((Version)value).ToString();

                }
            else if (InhetoPromotedTypesMap.TryGetValue(value.GetType().TypeHandle, out var pType))
                switch (pType)
                {
                    case InhetoPromotedTypes.CurrentCulture:
                    case InhetoPromotedTypes.IPEndPoint:
                    case InhetoPromotedTypes.IPAddress: return value.ToString();
                    case InhetoPromotedTypes.TimeZoneInfo: return ((TimeZoneInfo)value).ToSerializedString();
                    case InhetoPromotedTypes.Type:
                        var name = ((Type)value).AssemblyQualifiedName;
                        return string.IsNullOrEmpty(name) ? null : name;
                }
            return null;
        }
        internal static object StringToSafeKeyType(string str, InhetoBaseTypes baseType)
        {
            return baseType switch
            {
                InhetoBaseTypes.Char => char.Parse(str),
                InhetoBaseTypes.Rune => Rune.TryGetRuneAt(str, 0, out var rune) ? rune : throw new FormatException($"Invalid Rune: {str}"), // Assuming Rune can be constructed this way.
                InhetoBaseTypes.String => str,
                InhetoBaseTypes.Boolean => bool.Parse(str),
                InhetoBaseTypes.Byte => byte.Parse(str, CultureInfo.InvariantCulture),
                InhetoBaseTypes.SByte => sbyte.Parse(str, CultureInfo.InvariantCulture),
                InhetoBaseTypes.Int16 => short.Parse(str, CultureInfo.InvariantCulture),
                InhetoBaseTypes.UInt16 => ushort.Parse(str, CultureInfo.InvariantCulture),
                InhetoBaseTypes.Int32 => int.Parse(str, CultureInfo.InvariantCulture),
                InhetoBaseTypes.UInt32 => uint.Parse(str, CultureInfo.InvariantCulture),
                InhetoBaseTypes.Int64 => long.Parse(str, CultureInfo.InvariantCulture),
                InhetoBaseTypes.UInt64 => ulong.Parse(str, CultureInfo.InvariantCulture),
                InhetoBaseTypes.Int128 => Int128.Parse(str, CultureInfo.InvariantCulture),
                InhetoBaseTypes.UInt128 => UInt128.Parse(str, CultureInfo.InvariantCulture),
                InhetoBaseTypes.Half => Half.Parse(str, CultureInfo.InvariantCulture),
                InhetoBaseTypes.Single => float.Parse(str, CultureInfo.InvariantCulture),
                InhetoBaseTypes.Double => double.Parse(str, CultureInfo.InvariantCulture),
                InhetoBaseTypes.Decimal => decimal.Parse(str, CultureInfo.InvariantCulture),
                InhetoBaseTypes.BigInteger => BigInteger.Parse(str, CultureInfo.InvariantCulture),
                InhetoBaseTypes.DateOnly => DateOnly.Parse(str, CultureInfo.InvariantCulture),
                InhetoBaseTypes.DateTime => DateTime.Parse(str, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                InhetoBaseTypes.DateTimeOffset => DateTimeOffset.Parse(str, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                InhetoBaseTypes.TimeOnly => TimeOnly.Parse(str, CultureInfo.InvariantCulture),
                InhetoBaseTypes.TimeSpan => TimeSpan.Parse(str, CultureInfo.InvariantCulture),
                InhetoBaseTypes.Guid => Guid.Parse(str),
                InhetoBaseTypes.Uri => new Uri(str),
                InhetoBaseTypes.Version => Version.Parse(str),
                _ => throw new ArgumentException($"Unsupported base type: {baseType}")
            };
        }
        internal static object StringToSafeKeyType(string str, InhetoPromotedTypes promotedType)
        {
            return promotedType switch
            {
                InhetoPromotedTypes.CurrentCulture => CultureInfo.GetCultureInfo(str)!,
                InhetoPromotedTypes.TimeZoneInfo => str.Contains(';') ? TimeZoneInfo.FromSerializedString(str) : TimeZoneInfo.FindSystemTimeZoneById(str),
                InhetoPromotedTypes.IPEndPoint => IPEndPoint.Parse(str)!,
                InhetoPromotedTypes.IPAddress => IPAddress.Parse(str)!,
                InhetoPromotedTypes.Type => Type.GetType(str)!,
                _ => throw new ArgumentException($"Unsupported promoted type: {promotedType}")
            };
        }
        internal static System.Collections.Concurrent.ConcurrentDictionary<Type, ConcurrentDictionary<MemberInfo, IAccessors>> TypeAccessors = new System.Collections.Concurrent.ConcurrentDictionary<Type, ConcurrentDictionary<MemberInfo, IAccessors>>();








        //======  FIELDS  ======
        private SerializationOptions options = new SerializationOptions();
        private Dictionary<uint, string> propPathIndexCache = new Dictionary<uint, string>();
        private const int MaxRetainedPathCapacity = 4096;
        [ThreadStatic]
        private static StringBuilder? retPath;
        private BufferStream stream = new BufferStream();
        private Dictionary<object, valueOffset> valueOffsets = new Dictionary<object, valueOffset>(RefEqualityComparer.Instance);
        private Dictionary<(RuntimeTypeHandle type, string memberName), valueOffset> circularOffsets = new Dictionary<(RuntimeTypeHandle type, string memberName), valueOffset>();


        private void TryUpdateCircularOffsets(Type type, string memberName,NodeInfo circNode)
        { 
            if(options.CircularProps.Contains((type.TypeHandle, memberName)))
                circularOffsets[(type.TypeHandle, memberName)] = new valueOffset { node = circNode, valueType = type };
        }




        //======  CONSTRUCTORS  ======
        /// <summary>
        /// Creates a reusable writer retaining the supplied serialization policy, or a new default policy when omitted.<br/>
        /// </summary>
        /// <param name="options">Optional operation policy; omission uses the implementation default policy.<br/></param>
        public InhetoSerializer(SerializationOptions? options = null)
        {
            if (options != null)
                this.options = options;
        }








        //======  METHODS  ======
        /// <summary>Combines the parent's encoded property path and optional member suffix into a non-null path.<br/></summary>
        /// <param name="parentNodeIndex">Parent header index; zero represents the root.<br/></param>
        /// <param name="memberPath">Optional suffix, treated as empty when absent.<br/></param>
        /// <returns>The combined path, including an empty string for the root without a suffix.<br/></returns>
        private string GetMemberPath(uint parentNodeIndex, string? memberPath)
        {
            return $"{GetPropPath(parentNodeIndex) ?? ""}{memberPath}";
        }

        private string GetPropPath(uint nodeIndex)
        {
            if (nodeIndex == 0) return "";

            if (propPathIndexCache.TryGetValue(nodeIndex, out var retStr)) return retStr;

            var path = retPath ??= new StringBuilder();
            path.Clear();
            try
            {
                var nvn = nameValueNode.CreateFromStream(stream, nodeIndex);
                var nvnName = nvn.name;
                if (nvn.valueType == InhetoBaseTypes.DuplicateValue || nvn.valueType == InhetoBaseTypes.DuplicateRef)
                    nvn = nameValueNode.CreateFromStream(stream, nvn.valueOffset) with { name = nvnName };
                while (nvn.parentNodeIndex != 0 && nvn.parentNodeIndex != nodeIndex)
                {
                    path.Insert(0, nvn.name);
                    nvn = nameValueNode.CreateFromStream(stream, nvn.parentNodeIndex);
                }

                retStr = path.ToString();
                propPathIndexCache[nodeIndex] = retStr;
                return retStr;
            }
            finally
            {
                if (path.Capacity > MaxRetainedPathCapacity)
                    retPath = null;
                else
                    path.Clear();
            }
        }

        private void GraphFields(object obj, Type oType, uint thisNodeIndex, IList<FieldInfo> fields)
        {
            foreach (var ___ in fields)
            {
                var memberPath = GetMemberPath(thisNodeIndex, string.Intern($".{___.Name}"));
                if (TryWriteSerializedMemberOverride(thisNodeIndex, string.Intern($".{___.Name}"), memberPath) ||
                    options.ExcludedMembers.Contains(memberPath) ||
                    options.IncludedMembers.Count > 0 && !options.IncludedMembers.Contains(memberPath))
                {
                    continue;
                }
                object? mVal = null;

                Type mValType;

                mValType = ___.FieldType;

                if (options.FailOnUnsupportedType && (mValType.IsByRefLike || mValType.IsByRef || mValType.IsPointer || mValType.IsFunctionPointer))
                {
                    if (options.ExcludedTypes.Contains(mValType)) continue;
                    throw CreateUnsupportedTypeException(mValType, memberPath, "the selected field cannot be boxed for the object-based writer");
                }

                if (!TypeAccessors.TryGetValue(oType, out var accs))
                {
                    TypeAccessors[oType] = new ConcurrentDictionary<MemberInfo, IAccessors>();
                    accs = TypeAccessors[oType];
                }
                if (!accs.TryGetValue(___, out var acc))
                {
                    var factory = typeof(AccessorUtils).GetMethod("CreateAccessors", BindingFlags.Static | BindingFlags.NonPublic)
                        ?? throw new MissingMethodException(typeof(AccessorUtils).FullName, "CreateAccessors");
                    var method = factory.MakeGenericMethod(oType, mValType);
                    acc = method.Invoke(null, new object[] { ___ }) as IAccessors
                        ?? throw new InvalidOperationException("The field accessor factory returned no compatible accessor.");
                    TypeAccessors[oType][___] = acc;
                }
                mVal = acc.GetValue(obj);

                if (mVal == null) continue; // skip null values, they don't have any data to serialize.
                var fName = ___.Name;
                GraphObject(mVal, thisNodeIndex, string.Intern($".{___.Name}"));

            }
        }

        private bool GraphObject(object? obj, uint parentNodeIndex, string propName = "", bool preserveNull = false)
        {
            if (obj is null)
            {
                if (preserveNull)
                {
                    WriteNullNode(parentNodeIndex, propName);
                    return true;
                }
                return false;
            }
            var oType = obj.GetType();
            var nodeInfo = WriteNodeForType(obj, oType, parentNodeIndex, propName);

            var doComplex = false;
            if ((nodeInfo.kind == InhetoKindMasks.InhetoBaseType && nodeInfo.kindType == (byte)InhetoBaseTypes.ComplexType) || nodeInfo.kind == InhetoKindMasks.InhetoCollectionType || (nodeInfo.kind == InhetoKindMasks.InhetoPromotedType && nodeInfo.kindType == (byte)InhetoPromotedTypes.Enumerable))
                doComplex = true;

            if (doComplex)
                GraphPropsAndFields(obj, oType, nodeInfo);


            return true;
        }

        private void GraphProperties(object obj, Type oType, uint thisNodeIndex, IList<PropertyInfo> props)
        {
            foreach (var ___ in props)
            {
                if (oType.IsAssignableTo(typeof(IDictionary)) || oType.IsAssignableTo(typeof(ICollection)) || IsSetType(oType))
                {
                    switch (___.Name)
                    {
                        case "Keys":
                        case "IsEmpty":
                        case "AllKeys":
                        case "Values":
                        case "Comparer":
                        case "Item":
                        case "SyncRoot":
                        case "IsReadOnly":
                        case "IsFixedSize":
                        case "IsSynchronized":
                        case "Capacity":
                            continue;
                    }
                }
                else if (oType.IsGenericType && oType.GetGenericTypeDefinition() == typeof(LinkedListNode<>))
                {
                    switch (___.Name)
                    {
                        case "List":
                        case "Next":
                        case "Previous":
                        case "Value":
                        case "ValueRef":
                            continue;
                    }
                }
                if (obj is Array)
                {
                    switch (___.Name)
                    {
                        case "Rank":
                        case "LongLength":
                        case "Length":
                            continue;
                    }
                }
                var memberName = string.Intern($".{___.Name}");
                var memberPath = GetMemberPath(thisNodeIndex, memberName);
                if (TryWriteSerializedMemberOverride(thisNodeIndex, memberName, memberPath) ||
                    options.ExcludedMembers.Contains(memberPath) ||
                    options.IncludedMembers.Count > 0 && !options.IncludedMembers.Contains(memberPath))
                {
                    continue;
                }
                object? mVal = null;

                Type mValType;

                mValType = ___.PropertyType;
                if (!___.CanRead) continue; // well, can't read so nothing to do.

                if (___.GetIndexParameters().Length > 0) continue; // ignore props with index params - wouldn't know what to set for param.
                if (options.FailOnUnsupportedType && (mValType.IsByRefLike || mValType.IsByRef || mValType.IsPointer || mValType.IsFunctionPointer))
                {
                    if (options.ExcludedTypes.Contains(mValType)) continue;
                    throw CreateUnsupportedTypeException(mValType, memberPath, "the selected property cannot be boxed for the object-based writer");
                }
                var oTypeName = oType.Name;
                if (!TypeAccessors.TryGetValue(oType, out var accs))
                {
                    TypeAccessors[oType] = new ConcurrentDictionary<MemberInfo, IAccessors>();
                    accs = TypeAccessors[oType];
                }
                if (!accs.TryGetValue(___, out var acc))
                {
                    var factory = typeof(AccessorUtils).GetMethod("CreateAccessors", BindingFlags.Static | BindingFlags.NonPublic)
                        ?? throw new MissingMethodException(typeof(AccessorUtils).FullName, "CreateAccessors");
                    var method = factory.MakeGenericMethod(oType, mValType);
                    acc = method.Invoke(null, new object[] { ___ }) as IAccessors
                        ?? throw new InvalidOperationException("The property accessor factory returned no compatible accessor.");
                    TypeAccessors[oType][___] = acc;
                }
                mVal = acc.GetValue(obj);

                if (mVal == null) continue; // skip null values, they don't have any data to serialize.
#if DEBUG
                var pName = ___.Name;
#endif

                GraphObject(mVal, thisNodeIndex, memberName);

            }
        }


        /// <summary>
        /// Discovers and graphs the fields/properties selected by the current member policy.<br/>
        /// Each branch consumes its non-null discovery result directly; mixed mode discovers both lists before invoking getters.<br/>
        /// Preserves property-before-field order, unknown-policy no-op behavior, and null-member omission in the existing graph methods.<br/>
        /// </summary>
        /// <param name="obj">Object to graph; null remains a no-op.<br/></param>
        /// <param name="oType">Runtime type obtained from the non-null object by GraphObject.<br/></param>
        /// <param name="nodeInfo">Already emitted parent header.<br/></param>
        private void GraphPropsAndFields(object? obj, Type oType, NodeInfo nodeInfo)
        {
            if (obj == null) return;
            var thisNodeIndex = (uint)stream.Position;
            var parentNodeIndex = nodeInfo.nodeIndex;
            switch (options.MemberTypes)
            {
                case MemberTypesEnum.PublicPropertiesOnly:
                    GraphProperties(obj, oType, parentNodeIndex, oType.Properties(Flags.InstancePublic));
                    break;
                case MemberTypesEnum.PublicPropertiesAndFields:
                    var props = oType.Properties(Flags.InstancePublic);
                    var fields = oType.Fields(Flags.InstancePublic);
                    GraphProperties(obj, oType, parentNodeIndex, props);
                    GraphFields(obj, oType, parentNodeIndex, fields);
                    break;
                case MemberTypesEnum.PublicAndPrivateFields:
                    GraphFields(obj, oType, parentNodeIndex, oType.Fields(Flags.InstancePublic | Flags.InstancePrivate));
                    break;
                case MemberTypesEnum.PublicFieldsOnly:
                    GraphFields(obj, oType, parentNodeIndex, oType.Fields(Flags.InstancePublic));
                    break;
                case MemberTypesEnum.PrivateFieldsOnly:
                    GraphFields(obj, oType, parentNodeIndex, oType.Fields(Flags.InstancePrivate));
                    break;
                default:
                    break;
            }

        }

        private (InhetoKindMasks kind, UtilsAndExtensions.GenericFamily family, byte nodeType, byte keyType) WriteGenericType(object? obj, Type oType, UtilsAndExtensions.GenericMatch genericMatch)
        {
            var baseType = InhetoBaseTypes.Unknown;
            switch (genericMatch.Family)
            {
                case UtilsAndExtensions.GenericFamily.Nullable:
                    if (InhetoBaseTypesMap.TryGetValue(genericMatch.T0!.TypeHandle, out baseType))
                        stream.Write((byte)((byte)baseType | (byte)InhetoKindMasks.InhetoNullableBaseType));
                    else
                        stream.Write((byte)((byte)InhetoBaseTypes.ComplexType | (byte)InhetoKindMasks.InhetoNullableBaseType));

                    return (InhetoKindMasks.InhetoNullableBaseType, genericMatch.Family, (byte)baseType, 0);

                case UtilsAndExtensions.GenericFamily.List:
                case UtilsAndExtensions.GenericFamily.Queue:
                case UtilsAndExtensions.GenericFamily.ConcurrentQueue:
                case UtilsAndExtensions.GenericFamily.Stack:
                case UtilsAndExtensions.GenericFamily.ConcurrentStack:
                case UtilsAndExtensions.GenericFamily.ObservableCollection:
                case UtilsAndExtensions.GenericFamily.ConcurrentBag:

                    if (InhetoBaseTypesMap.TryGetValue(genericMatch.T0!.TypeHandle, out baseType))
                    {
                        stream.Write((byte)((byte)genericMatch.Family | (byte)InhetoKindMasks.InhetoCollectionType));
                        stream.Write((byte)baseType);
                    }
                    else if (InhetoPromotedTypesMap.TryGetValue(genericMatch.T0!.TypeHandle, out var promotedType))
                    {
                        stream.Write((byte)((byte)genericMatch.Family | (byte)InhetoKindMasks.InhetoCollectionType));
                        stream.Write((byte)((byte)promotedType | (byte)InhetoKindMasks.InhetoPromotedType));
                    }
                    else
                    {
                        // treat element type as a complex type and serialize it as such
                        stream.Write((byte)((byte)genericMatch.Family | (byte)InhetoKindMasks.InhetoCollectionType));
                        stream.Write((byte)InhetoBaseTypes.ComplexType);
                    }

                    return (InhetoKindMasks.InhetoCollectionType, genericMatch.Family, (byte)genericMatch.Family, 0);

                case UtilsAndExtensions.GenericFamily.HashSet:
                case UtilsAndExtensions.GenericFamily.SortedSet:
                    if (InhetoBaseTypesMap.TryGetValue(genericMatch.T0!.TypeHandle, out baseType))
                    {
                        var hasAux = (byte)0;
                        var compType = InhetoComparerTypes.Ordinal;
                        // try to get to a comparer to capture it if it is a BCL type.
                        var comparerProp = oType.Properties(Flags.InstancePublic, "Comparer", "KeyComparer").FirstOrDefault();
                        if (comparerProp is not null)
                        {
                            var comparer = comparerProp.GetValue(obj);
                            if (ReferenceEquals(comparer, StringComparer.OrdinalIgnoreCase))
                            {
                                hasAux = (byte)InhetoKindMasks.InhetoAuxiliaryData;
                                compType = InhetoComparerTypes.OrdinalIgnoreCase;
                            }
                            else if (ReferenceEquals(comparer, StringComparer.InvariantCulture))
                            {
                                hasAux = (byte)InhetoKindMasks.InhetoAuxiliaryData;
                                compType = InhetoComparerTypes.InvariantCulture;
                            }
                            else if (ReferenceEquals(comparer, StringComparer.InvariantCultureIgnoreCase))
                            {
                                hasAux = (byte)InhetoKindMasks.InhetoAuxiliaryData;
                                compType = InhetoComparerTypes.InvariantCultureIgnoreCase;
                            }
                        }

                        stream.Write((byte)((byte)genericMatch.Family | (byte)InhetoKindMasks.InhetoCollectionType | hasAux));
                        if (hasAux != 0) stream.Write((byte)compType);
                        stream.Write((byte)baseType);
                    }
                    else if (InhetoPromotedTypesMap.TryGetValue(genericMatch.T0!.TypeHandle, out var promotedType))
                    {
                        var hasAux = (byte)0;
                        var compType = InhetoComparerTypes.Ordinal;
                        // try to get to a comparer to capture it if it is a BCL type.
                        var comparerProp = oType.Properties(Flags.InstancePublic, "Comparer", "KeyComparer").FirstOrDefault();
                        if (comparerProp is not null)
                        {
                            var comparer = comparerProp.GetValue(obj);
                            if (ReferenceEquals(comparer, StringComparer.OrdinalIgnoreCase))
                            {
                                hasAux = (byte)InhetoKindMasks.InhetoAuxiliaryData;
                                compType = InhetoComparerTypes.OrdinalIgnoreCase;
                            }
                            else if (ReferenceEquals(comparer, StringComparer.InvariantCulture))
                            {
                                hasAux = (byte)InhetoKindMasks.InhetoAuxiliaryData;
                                compType = InhetoComparerTypes.InvariantCulture;
                            }
                            else if (ReferenceEquals(comparer, StringComparer.InvariantCultureIgnoreCase))
                            {
                                hasAux = (byte)InhetoKindMasks.InhetoAuxiliaryData;
                                compType = InhetoComparerTypes.InvariantCultureIgnoreCase;
                            }
                        }

                        stream.Write((byte)((byte)genericMatch.Family | (byte)InhetoKindMasks.InhetoCollectionType | hasAux));
                        if (hasAux != 0) stream.Write((byte)compType);
                        stream.Write((byte)((byte)promotedType | (byte)InhetoKindMasks.InhetoPromotedType));
                    }
                    else
                    {
                        // treat element type as a complex type and serialize it as such
                        stream.Write((byte)((byte)genericMatch.Family | (byte)InhetoKindMasks.InhetoCollectionType));
                        baseType = InhetoBaseTypes.ComplexType;
                        stream.Write((byte)baseType);
                    }


                    return (InhetoKindMasks.InhetoCollectionType, genericMatch.Family, (byte)genericMatch.Family, (byte)baseType);


                case UtilsAndExtensions.GenericFamily.Dictionary:
                case UtilsAndExtensions.GenericFamily.ConcurrentDictionary:
                case UtilsAndExtensions.GenericFamily.SortedDictionary:
                case UtilsAndExtensions.GenericFamily.SortedList:
                    var family = genericMatch.Family;
                    if (InhetoBaseTypesMap.TryGetValue(genericMatch.T0!.TypeHandle, out var keyType))
                    {
                        Flags typeCheckFlags = Flags.InstancePublic;
                        switch (options.MemberTypes)
                        {
                            case MemberTypesEnum.PublicPropertiesAndFields: typeCheckFlags = Flags.InstancePublic; break;
                            case MemberTypesEnum.PublicAndPrivateFields: typeCheckFlags = Flags.InstanceAnyVisibility; break;
                            case MemberTypesEnum.PublicFieldsOnly: typeCheckFlags = Flags.InstancePublic; break;
                            case MemberTypesEnum.PublicPropertiesOnly: typeCheckFlags = Flags.InstancePublic; break;
                            case MemberTypesEnum.PrivateFieldsOnly: typeCheckFlags = Flags.InstancePrivate; break;
                        }

                        InhetoBaseTypes valueType;
                        // Every analyzer-produced dictionary family supplies T1 as its declared value Type,
                        // including nullable values. Only those matches reach this private writer branch.
                        if (!InhetoBaseTypesMap.TryGetValue(genericMatch.T1!.TypeHandle, out valueType))
                            if (!InhetoPromotedTypesMap.TryGetValue(genericMatch.T1.TypeHandle, out var promType))
                                if (!UtilsAndExtensions.GenericAnalyzer.TryAnalyzeForGenericType(genericMatch.T1, out var genType))
                                {
                                    if ((genericMatch.T1.Properties(typeCheckFlags).Count) + (genericMatch.T1.Fields(typeCheckFlags).Count) is int typeMemberCount && typeMemberCount == 0 && obj is not IEnumerable)
                                        valueType = InhetoBaseTypes.ComplexType;
                                }
                                else
                                    // Each dictionary value has its own fully described child node.
                                    // Do not emit its generic descriptor here, before the outer dictionary
                                    // marker, or inspect the outer dictionary as though it were that value.
                                    valueType = InhetoBaseTypes.ComplexType;
                            else
                                valueType = InhetoBaseTypes.PromotedType;

                        if (keyType == InhetoBaseTypes.String)
                        {
                            family = family + (byte)UtilsAndExtensions.GenericFamily.StringKeyEnumOffset;
                            var hasAux = (byte)0;
                            var compType = InhetoComparerTypes.Ordinal;
                            // try to get to a comparer to capture it if it is a BCL type.
                            var comparerProp = oType.Properties(Flags.InstancePublic, "Comparer", "KeyComparer").FirstOrDefault();
                            if (comparerProp is not null)
                            {
                                var comparer = comparerProp.GetValue(obj);
                                if (ReferenceEquals(comparer, StringComparer.OrdinalIgnoreCase))
                                {
                                    hasAux = (byte)InhetoKindMasks.InhetoAuxiliaryData;
                                    compType = InhetoComparerTypes.OrdinalIgnoreCase;
                                }
                                else if (ReferenceEquals(comparer, StringComparer.InvariantCulture))
                                {
                                    hasAux = (byte)InhetoKindMasks.InhetoAuxiliaryData;
                                    compType = InhetoComparerTypes.InvariantCulture;
                                }
                                else if (ReferenceEquals(comparer, StringComparer.InvariantCultureIgnoreCase))
                                {
                                    hasAux = (byte)InhetoKindMasks.InhetoAuxiliaryData;
                                    compType = InhetoComparerTypes.InvariantCultureIgnoreCase;
                                }
                            }

                            stream.Write((byte)((byte)family | (byte)InhetoKindMasks.InhetoCollectionType | hasAux));
                            if (hasAux != 0) stream.Write((byte)compType);
                            stream.Write((byte)valueType);
                        }
                        else
                        {
                            stream.Write((byte)((byte)genericMatch.Family | (byte)InhetoKindMasks.InhetoCollectionType));
                            stream.Write((byte)keyType);
                            stream.Write((byte)valueType);
                        }
                    }
                    else
                    {
                        // treat element type as a complex type and serialize it as such
                        stream.Write((byte)((byte)genericMatch.Family | (byte)InhetoKindMasks.InhetoCollectionType));
                        baseType = InhetoBaseTypes.ComplexType;
                        var declaredKey = genericMatch.T0;
                        var underlyingKey = Nullable.GetUnderlyingType(declaredKey);
                        if (underlyingKey is not null && InhetoBaseTypesMap.TryGetValue(underlyingKey.TypeHandle, out var nullableKey))
                            stream.Write((byte)((byte)InhetoKindMasks.InhetoNullableBaseType | (byte)nullableKey));
                        else if (IsSafeKeyType(declaredKey) && InhetoPromotedTypesMap.TryGetValue(declaredKey.TypeHandle, out var promotedKey))
                            stream.Write((byte)((byte)InhetoKindMasks.InhetoPromotedType | (byte)promotedKey));
                        else
                            stream.Write((byte)InhetoBaseTypes.ComplexType);
                        // The same analyzer-produced dictionary invariant holds for non-base key types.
                        if (!InhetoBaseTypesMap.TryGetValue(genericMatch.T1!.TypeHandle, out var valueType))
                            valueType = InhetoBaseTypes.ComplexType;
                        stream.Write((byte)valueType);
                    }

                    return (InhetoKindMasks.InhetoCollectionType, family, (byte)genericMatch.Family, (byte)baseType);

            }

            return default;
        }






        //------ Public Methods -----
        /// <summary>
        /// Reads Value from a LinkedListNode of the specified closed generic node type using a cached compiled getter. Rejects null inputs and non-LinkedListNode types.<br/>
        /// </summary>
        /// <param name="nodeType">Closed LinkedListNode CLR type whose Value getter is requested.<br/></param>
        /// <param name="nodeInstance">Non-null node instance matching nodeType.<br/></param>
        /// <returns>Converted or retrieved value, according to the documented operation; no blanket exception suppression is performed.<br/></returns>
        public object ReadLinkedListNodeValue(Type nodeType, object nodeInstance)
        {
            if (nodeType == null)
                throw new ArgumentNullException(nameof(nodeType));
            if (nodeInstance == null)
                throw new ArgumentNullException(nameof(nodeInstance));

            var fn = _cacheGetLinkedListValue.GetOrAdd(nodeType, static t =>
            {
                // Expecting LinkedListNode<T>
                if (!t.IsGenericType || t.GetGenericTypeDefinition() != typeof(LinkedListNode<>))
                    throw new ArgumentException($"Type {t.FullName} is not a LinkedListNode<>.");

                var valueProp = t.Property("Value", Flags.InstancePublic);
                if (valueProp == null)
                    throw new MissingMemberException($"No Value property found on {t.FullName}.");

                // Parameters
                var inst = Expression.Parameter(typeof(object), "node");
                var castInst = Expression.Convert(inst, t);
                var propAccess = Expression.Property(castInst, valueProp);
                var boxed = Expression.Convert(propAccess, typeof(object));

                var lambda = Expression.Lambda<Func<object, object>>(boxed, inst);
                return lambda.Compile();
            });

            return fn(nodeInstance);
        }

        //------ Public Methods -----
        /// <summary>
        /// Serializes a non-null object graph into this writer&apos;s reusable BufferStream and rewinds it to zero. The next Serialize call resets that same stream; detach bytes before retaining a previous result and do not dispose the stream while reusing the writer.<br/>
        /// The result is plain Inheto binary data. Compression/encryption for storage or transport is caller-owned and operates on that binary payload after serialization.<br/>
        /// </summary>
        /// <param name="obj">Non-null source object graph to serialize into the reusable writer stream.<br/></param>
        /// <returns>Writer-owned reusable BufferStream at position zero; the next serialization replaces its contents.<br/></returns>
        /// <remarks>With SerializationOptions.FailOnUnsupportedType enabled, representation checks occur during this same traversal after selection/custom handling. A failed operation may leave a partial writer buffer: do not consume it; the next call resets all payload locators before reuse.<br/></remarks>
        /// <exception cref="NotSupportedException">Strict policy rejects an unsupported value/member representation, or an ordinary dictionary key cannot be represented safely. Strict failures include Inheto.Path and Inheto.Type in exception Data.<br/></exception>
        public BufferStream Serialize(object obj)
        {
            ArgumentNullException.ThrowIfNull(obj);
            valueOffsets.Clear();
            // Circular locators are payload offsets, never reusable across serialization operations.
            circularOffsets.Clear();
            stream.Reset(true);
            // write version
            stream.WriteByte(1);

            GraphObject(obj, 0);

            // write end of name header marker
            var endOfNameHeader = WriteEndOfNameHeader();
            //Debug.WriteLine($"{int.MaxValue} | {stream.Position} | {InhetoBaseTypes.EndOfNamesHeader} | 'End of Name Header'", "WriteNameBlock");



            // capture current stream position to update the value offsets in the names block
            var dataPosition = stream.Position;
            bool hasNullCustomResults = false;


            foreach (var ___ in valueOffsets)
            {
                var propName = ___.Value.node.propName;

                if (___.Value.node.kind == InhetoKindMasks.InhetoBaseType && ___.Value.node.kindType == (byte)InhetoBaseTypes.Null) continue;

                var kind = ___.Value.node.kind;
                var kindType = ___.Value.node.kindType;
                uint offsetValue;
                using (BufferStream offsetReader = stream.Segment(___.Value.node.valueOffsetPos))
                    offsetValue = offsetReader.ReadUInt32();
                //if (offsetValue == 0 && !ReferenceEquals(___.Key, obj) && !(___.Value.node.kind == InhetoKindMasks.InhetoPromotedType && ___.Value.node.kindType == (byte)InhetoPromotedTypes.Enumerable) && !(___.Value.node.kind == InhetoKindMasks.InhetoBaseType && inlinedValueTypes.Contains((InhetoBaseTypes)___.Value.node.kindType)))
                //    stream.WriteAtOffset(___.Value.node.valueOffsetPos, (uint)stream.Position);
                //else if (offsetValue == 0 && (kind == InhetoKindMasks.InhetoBaseType && kindType >= (byte)InhetoBaseTypes.Boolean && !inlinedValueTypes.Contains((InhetoBaseTypes)kindType) || (kind == InhetoKindMasks.InhetoPromotedType)))
                //    stream.WriteAtOffset(___.Value.node.valueOffsetPos, (uint)stream.Position);
                //else
                //    Debug.WriteLine("Value Offset is already set, likely a count value from an enumerable type");

                var isCountSlot =
                    kind == InhetoKindMasks.InhetoCollectionType ||
                    (kind == InhetoKindMasks.InhetoBaseType && (InhetoBaseTypes)kindType == InhetoBaseTypes.ListString) ||
                    (kind == InhetoKindMasks.InhetoPromotedType && (
                        (InhetoPromotedTypes)kindType == InhetoPromotedTypes.Array ||
                        (InhetoPromotedTypes)kindType == InhetoPromotedTypes.ArrayMD ||
                        (InhetoPromotedTypes)kindType == InhetoPromotedTypes.Enumerable ||
                        (InhetoPromotedTypes)kindType == InhetoPromotedTypes.NameValueCollection ||
                        (InhetoPromotedTypes)kindType == InhetoPromotedTypes.StringCollection));

                if (!isCountSlot)
                {
                    var isInlineEnum =
                        kind == InhetoKindMasks.InhetoPromotedType &&
                        kindType == (byte)InhetoPromotedTypes.Enum &&
                        ___.Value.node.extendedType is
                            (byte)InhetoBaseTypes.SByte or
                            (byte)InhetoBaseTypes.Byte or
                            (byte)InhetoBaseTypes.Int16 or
                            (byte)InhetoBaseTypes.UInt16 or
                            (byte)InhetoBaseTypes.Int32 or
                            (byte)InhetoBaseTypes.UInt32;
                    if (offsetValue == 0 && !ReferenceEquals(___.Key, obj) && !(kind == InhetoKindMasks.InhetoPromotedType && kindType == (byte)InhetoPromotedTypes.Enumerable) && !(kind == InhetoKindMasks.InhetoBaseType && inlinedValueTypes.Contains((InhetoBaseTypes)kindType)) && !isInlineEnum)
                        stream.WriteAtOffset(___.Value.node.valueOffsetPos, (uint)stream.Position);
                    else if (offsetValue == 0 && (kind == InhetoKindMasks.InhetoBaseType && kindType >= (byte)InhetoBaseTypes.Boolean && !inlinedValueTypes.Contains((InhetoBaseTypes)kindType) || (kind == InhetoKindMasks.InhetoPromotedType && !isInlineEnum)))
                        stream.WriteAtOffset(___.Value.node.valueOffsetPos, (uint)stream.Position);
                }

                switch (___.Value.node.kind)
                {
                    case InhetoKindMasks.InhetoBaseType:
                    case InhetoKindMasks.InhetoNullableBaseType:
                        switch ((InhetoBaseTypes)___.Value.node.kindType)
                        {
                            // these commented types are inlined in the name header.  no need to write here.
                            //case InhetoBaseTypes.Boolean: stream.Write((bool)___.Key); break;
                            //case InhetoBaseTypes.Byte: stream.Write((byte)___.Key); break;
                            //case InhetoBaseTypes.SByte: stream.Write((sbyte)___.Key); break;
                            //case InhetoBaseTypes.Char: stream.Write((char)___.Key); break;
                            //case InhetoBaseTypes.Int16: stream.Write((short)___.Key); break;
                            //case InhetoBaseTypes.UInt16: stream.Write((ushort)___.Key); break;
                            //case InhetoBaseTypes.Half: stream.Write((Half)___.Key); break;
                            //case InhetoBaseTypes.Int32: stream.Write((int)___.Key); break;
                            //case InhetoBaseTypes.UInt32: stream.Write((uint)___.Key); break;
                            //case InhetoBaseTypes.Rune: stream.Write((Rune)___.Key); break;
                            //case InhetoBaseTypes.Single: stream.Write((float)___.Key); break;

                            case InhetoBaseTypes.Int64: stream.Write((long)___.Key); break;
                            case InhetoBaseTypes.UInt64: stream.Write((ulong)___.Key); break;
                            case InhetoBaseTypes.Int128: stream.Write((Int128)___.Key); break;
                            case InhetoBaseTypes.UInt128: stream.Write((UInt128)___.Key); break;
                            case InhetoBaseTypes.Double: stream.Write((double)___.Key); break;
                            case InhetoBaseTypes.Decimal: stream.Write((decimal)___.Key); break;
                            case InhetoBaseTypes.String: stream.Write((string)___.Key); break;

                            case InhetoBaseTypes.DateOnly: if (___.Key is DateOnly don) stream.Write(don.DayNumber); break;
                            case InhetoBaseTypes.DateTime: if (___.Key is DateTime dt) stream.Write(dt); break;
                            case InhetoBaseTypes.DateTimeOffset: if (___.Key is DateTimeOffset dto) { stream.Write(dto.Ticks); stream.Write(dto.Offset); } break;
                            case InhetoBaseTypes.TimeOnly: if (___.Key is TimeOnly ton) stream.Write(ton.Ticks); break;
                            case InhetoBaseTypes.TimeSpan: if (___.Key is TimeSpan ts) stream.Write(ts); break;
                            case InhetoBaseTypes.Guid: if (___.Key is Guid guid) stream.Write(guid); break;
                            case InhetoBaseTypes.Uri: if (___.Key is Uri uri) stream.Write(uri.ToString()); break;
                            case InhetoBaseTypes.Version: if (___.Key is Version ver) { stream.Write7BitEncodedInt(ver.Major); stream.Write7BitEncodedInt(ver.Minor); stream.Write7BitEncodedInt(ver.Build); stream.Write7BitEncodedInt(ver.Revision); }; break;

                            case InhetoBaseTypes.Vector2: if (___.Key is System.Numerics.Vector2 v2) { stream.Write(v2); }; break;
                            case InhetoBaseTypes.Vector3: if (___.Key is System.Numerics.Vector3 v3) { stream.Write(v3); }; break;
                            case InhetoBaseTypes.Vector4: if (___.Key is System.Numerics.Vector4 v4) { stream.Write(v4); }; break;
                            case InhetoBaseTypes.ComplexNumeric: if (___.Key is System.Numerics.Complex c) { stream.Write(c); }; break;
                            case InhetoBaseTypes.Quaternion: if (___.Key is System.Numerics.Quaternion q) { stream.Write(q); }; break;
                            case InhetoBaseTypes.Plane: if (___.Key is System.Numerics.Plane p) { stream.Write(p); }; break;
                            case InhetoBaseTypes.Matrix3x2: if (___.Key is System.Numerics.Matrix3x2 m3x2) { stream.Write(m3x2); }; break;
                            case InhetoBaseTypes.Matrix4x4: if (___.Key is System.Numerics.Matrix4x4 m4x4) { stream.Write(m4x4); }; break;
                            case InhetoBaseTypes.BigInteger: if (___.Key is System.Numerics.BigInteger bi) stream.WriteBytesWithByteLength(bi.ToByteArray()); break;

                            case InhetoBaseTypes.ByteArray: if (___.Key is byte[] ba) stream.WriteBytesWithByteLength(ba); break;
                            //case InhetoBaseTypes.ListString: if (___.Key is List<string> ls) { stream.Write7BitEncodedInt(ls.Count); foreach (var s in ls) stream.Write(s); }; break;

                            case InhetoBaseTypes.CustomProp:
                            case InhetoBaseTypes.CustomType:
                                {
                                    var serialize = ___.Value.sz ?? throw new InvalidOperationException("A custom header requires a serializer callback.");
                                    object? payload = serialize(___.Key);
                                    if (payload is null)
                                    {
                                        MarkCustomNull(___.Value.node);
                                        hasNullCustomResults = true;
                                        break;
                                    }
                                    // Root custom control codes precede Boolean and bypass the general
                                    // root offset update. Their non-null payload still needs a locator.
                                    if (___.Value.node.parentIndex == 0)
                                        stream.WriteAtOffset(___.Value.node.valueOffsetPos, (uint)stream.Position);
                                    if (kindType == (byte)InhetoBaseTypes.CustomType)
                                        stream.Write(___.Key.GetType().AssemblyQualifiedName);
                                    if (payload is byte[] bytes)
                                        stream.WriteBytesWithByteLength(bytes);
                                    else if (kindType == (byte)InhetoBaseTypes.CustomProp && payload is string text)
                                        WriteCustomText(text);
                                    else
                                        throw new InvalidOperationException("A custom serializer must return bytes, or text for a property-path hook.");
                                    break;
                                }

                            default:
                                break;
                        }
                        break;
                    case InhetoKindMasks.InhetoPromotedType:
                        switch ((InhetoPromotedTypes)___.Value.node.kindType)
                        {

                            case InhetoPromotedTypes.Enum:
                                // The enum header writer always supplies the underlying numeric marker.
                                // Other node kinds may omit it, but cannot enter this enum payload branch.
                                switch ((InhetoBaseTypes)(byte)___.Value.node.extendedType!)
                                {
                                    case InhetoBaseTypes.Int64:
                                        stream.Write(Convert.ToInt64(___.Key, CultureInfo.InvariantCulture));
                                        break;
                                    case InhetoBaseTypes.UInt64:
                                        stream.Write(Convert.ToUInt64(___.Key, CultureInfo.InvariantCulture));
                                        break;
                                }
                                break;
                            case InhetoPromotedTypes.CultureInfo: if (___.Key is CultureInfo ci) stream.Write(ci.Name); break;
                            case InhetoPromotedTypes.IPAddress: if (___.Key is IPAddress ip) { stream.WriteBytesWithByteLength(ip.GetAddressBytes()); stream.Write7BitEncodedInt64(ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 ? ip.ScopeId : 0L); }; break;
                            case InhetoPromotedTypes.IPEndPoint: if (___.Key is IPEndPoint ie) { stream.WriteBytesWithByteLength(ie.Address.GetAddressBytes()); stream.Write7BitEncodedInt64(ie.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 ? ie.Address.ScopeId : 0L); stream.Write(ie.Port); }; break;
                            case InhetoPromotedTypes.MailAddress: if (___.Key is System.Net.Mail.MailAddress ma) { stream.Write(ma.Address); stream.Write(ma.DisplayName); }; break;
                            case InhetoPromotedTypes.MemoryByte: if (___.Key is Memory<byte> mb) { stream.Write(mb); }; break;
                            case InhetoPromotedTypes.MemoryStream: if (___.Key is MemoryStream ms) stream.WriteBytesWithByteLength(ms.ToArray()); break;
                            case InhetoPromotedTypes.ReadOnlyMemoryByte: if (___.Key is ReadOnlyMemory<byte> romb) { stream.WriteBytesWithByteLength(romb.ToArray()); }; break;
                            case InhetoPromotedTypes.Regex: if (___.Key is Regex re) { stream.Write(re.ToString()); stream.Write7BitEncodedInt((int)re.Options); stream.Write(re.MatchTimeout); }; break;
                            case InhetoPromotedTypes.SecureString: if (___.Key is SecureString ss) stream.Write(new NetworkCredential("", (SecureString)___.Key).Password); break;
                            case InhetoPromotedTypes.StringBuilder: if (___.Key is StringBuilder sb) stream.Write(sb.ToString()); break;
                            case InhetoPromotedTypes.TimeZoneInfo: if (___.Key is TimeZoneInfo tzi) stream.Write(tzi.ToSerializedString()); break;
                            case InhetoPromotedTypes.Type: if (___.Key is Type t) stream.Write(t.AssemblyQualifiedName); break;
                            case InhetoPromotedTypes.StaticClass: stream.Write(___.Key.GetType().AssemblyQualifiedName); break;
                            case InhetoPromotedTypes.Array:
                            case InhetoPromotedTypes.ArrayMD:
                            case InhetoPromotedTypes.Enumerable:
                                // arrays are serialized by individual element.  write count/length instead.

                                break;
                        }
                        break;
                    default:
                        break;
                }
            }

            if (hasNullCustomResults)
                MarkNullAliases(endOfNameHeader.nodeIndex);

            stream.Position = 0;

            return stream;

        }

        /// <summary>
        /// Appends the end-of-name-header marker to the current writer stream and returns its entry metadata. This is a low-level format operation, not a standalone serialization call.<br/>
        /// </summary>
        /// <returns>Metadata for the terminator just written to the current stream.<br/></returns>
        public NodeInfo WriteEndOfNameHeader()
        {
            var thisNodeIndex = (uint)stream.Position;
            stream.WriteByte((byte)0);
            stream.WriteByte((byte)InhetoBaseTypes.EndOfNamesHeader);
            stream.Write("\0");
            stream.Write(0u);
            return new NodeInfo(0, thisNodeIndex, 0, InhetoKindMasks.InhetoBaseType, (byte)InhetoBaseTypes.EndOfNamesHeader);
        }

        private bool TryWriteSerializedMemberOverride(uint parentNodeIndex, string propName, string propPath)
        {
            if (!options.SerializedMemberOverrides.TryGetValue(propPath, out var payload))
                return false;

            uint nodeIndex = (uint)stream.Position;
            stream.Write7BitEncodedUInt(parentNodeIndex);
            stream.WriteByte((byte)InhetoBaseTypes.CustomProp);
            stream.Write(propName);
            uint valueOffsetPosition = (uint)stream.Position;
            stream.Write(0u);
            var node = new NodeInfo(
                parentNodeIndex,
                nodeIndex,
                valueOffsetPosition,
                InhetoKindMasks.InhetoBaseType,
                (byte)InhetoBaseTypes.CustomProp,
                null,
                propName);
            var preserved = new PreservedCustomMember(payload);
            valueOffsets.Add(preserved, new valueOffset(typeof(byte[]), node, static value => ((PreservedCustomMember)value).Payload));
            return true;
        }

        /// <summary>
        /// Writes the name header and schedules the value of a non-null object in this serialization operation.<br/>
        /// GraphObject handles null separately and supplies the object's runtime Type once for this path.<br/>
        /// Retains member filtering, reference tracking and custom-hook precedence before ordinary type dispatch.<br/>
        /// </summary>
        /// <param name="obj">The non-null source value being graphed.<br/></param>
        /// <param name="oType">The runtime Type obtained from this source value by GraphObject.<br/></param>
        /// <param name="parentNodeIndex">The owning name-header index, or zero for the root.<br/></param>
        /// <param name="propName">The local member name, or an empty string for the root.<br/></param>
        /// <returns>The emitted node descriptor, or default when the configured filters exclude this node.<br/></returns>
        private NodeInfo WriteNodeForType(object obj, Type oType, uint parentNodeIndex, string propName)
        {

            var thisNodeIndex = (uint)stream.Position;
            var valueOffsetPos = 0u;
            NodeInfo ret = default;

            var memberPath = propName;
            memberPath = GetMemberPath(parentNodeIndex, memberPath);
            // only exclude if the type is a non-root object, that is: the type is seen in the graph of the object, not the object itself.
            if (!string.IsNullOrEmpty(propName) && options.ExcludedTypes.Contains(oType)) return default;
            if (!string.IsNullOrEmpty(propName) && options.ExcludedMembers.Contains(memberPath)) return default;
            if (options.IncludedMembers.Count > 0 && !string.IsNullOrEmpty(propName) && !options.IncludedMembers.Contains(memberPath)) return default;


#if DEBUG
            var typeXName = oType.Name;
#endif

            if (valueOffsets.ContainsKey(obj))
            {
                var iType = InhetoBaseTypes.Unknown;

                if (oType.IsValueType || RefEqualityComparer.IsImmutableRefType(oType))
                    iType = InhetoBaseTypes.DuplicateValue;
                else
                    iType = InhetoBaseTypes.DuplicateRef;

                stream.Write7BitEncodedUInt(parentNodeIndex);
                stream.WriteByte((byte)iType);
                stream.Write(propName!);
                valueOffsetPos = (uint)stream.Position;
                // write the index of the first ref node
                stream.Write(valueOffsets[obj].node.nodeIndex);
                //Debug.WriteLine($"{parentNodeIndex.ToString().PadLeft(4, ' ')} | {valueOffsets[obj].node.nodeIndex.ToString().PadLeft(4, ' ')} | {iType} | {GetPropPath(thisNodeIndex)}", "WriteNameBlock");
                return new NodeInfo(parentNodeIndex, thisNodeIndex, valueOffsetPos, InhetoKindMasks.InhetoBaseType, (byte)iType, null, propName);
            }

            if (this.options.CircularProps.Contains((oType.TypeHandle, propName)))
                if (circularOffsets.TryGetValue((oType.TypeHandle, propName), out var cref))
                {
                    var iType = InhetoBaseTypes.DuplicateValue;

                    stream.Write7BitEncodedUInt(parentNodeIndex);
                    stream.WriteByte((byte)iType);
                    stream.Write(propName!);
                    valueOffsetPos = (uint)stream.Position;
                    // write the index of the first ref node
                    stream.Write(cref.node.nodeIndex);
                    //Debug.WriteLine($"{parentNodeIndex.ToString().PadLeft(4, ' ')} | {valueOffsets[obj].node.nodeIndex.ToString().PadLeft(4, ' ')} | {iType} | {GetPropPath(thisNodeIndex)}", "WriteNameBlock");
                    return new NodeInfo(parentNodeIndex, thisNodeIndex, valueOffsetPos, InhetoKindMasks.InhetoBaseType, (byte)iType, null, propName);
                }

            //Debug.WriteLine(memberPath, "memberPath");

            // check for custom serializers first

            propName = propName ?? "";

            if (options.MemberSerializers.TryGetValue(memberPath, out var sz))
            {
                stream.Write7BitEncodedUInt(parentNodeIndex);
                stream.WriteByte((byte)InhetoBaseTypes.CustomProp);
                stream.Write(propName);
                valueOffsetPos = (uint)stream.Position;
                stream.Write(0u);
                ret = new NodeInfo(parentNodeIndex, thisNodeIndex, valueOffsetPos, InhetoKindMasks.InhetoBaseType, (byte)InhetoBaseTypes.CustomProp, null, propName);
                TryUpdateCircularOffsets(oType, propName, ret);
                valueOffsets[obj] = new valueOffset(oType, ret, sz);

                return ret;
            }
            else if (options.TypeSerializers.TryGetValue(oType.TypeHandle, out var sz2))
            {
                stream.Write7BitEncodedUInt(parentNodeIndex);
                stream.WriteByte((byte)InhetoBaseTypes.CustomType);
                stream.Write(propName);
                valueOffsetPos = (uint)stream.Position;
                stream.Write(0u);
                ret = new NodeInfo(parentNodeIndex, thisNodeIndex, valueOffsetPos, InhetoKindMasks.InhetoBaseType, (byte)InhetoBaseTypes.CustomType, null, propName);
                TryUpdateCircularOffsets(oType, propName, ret);
                valueOffsets[obj] = new valueOffset(oType, ret, sz2);

                return ret;
            }

            if (options.FailOnUnsupportedType)
                EnsureSupportedValue(obj, oType, memberPath);

            stream.Write7BitEncodedUInt(parentNodeIndex);
            var kind = WriteNodeTypeMarkers(obj, oType, memberPath);

            switch (kind.kind)
            {
                case InhetoKindMasks.InhetoBaseType:
                    {
                        stream.Write(propName!);
                        valueOffsetPos = (uint)stream.Position;

                        // inline the value for those that are <= 4 bytes
                        switch ((InhetoBaseTypes)kind.kindType)
                        {
                            case InhetoBaseTypes.Boolean:
                                stream.Write((uint)((bool)obj ? 1 : 0));
                                break;
                            case InhetoBaseTypes.SByte:
                                stream.Write((int)(sbyte)obj!);
                                break;
                            case InhetoBaseTypes.Byte:
                                stream.Write((uint)(byte)obj!);
                                break;
                            case InhetoBaseTypes.Int16:
                                stream.Write((int)(short)obj!);
                                break;
                            case InhetoBaseTypes.UInt16:
                                stream.Write((uint)(ushort)obj!);
                                break;
                            case InhetoBaseTypes.Int32:
                                stream.Write((int)obj!);
                                break;
                            case InhetoBaseTypes.UInt32:
                                stream.Write((uint)obj!);
                                break;
                            case InhetoBaseTypes.Char:
                                stream.Write((uint)(char)obj!);
                                break;
                            case InhetoBaseTypes.Rune:
                                stream.Write(((Rune)obj).Value!);
                                break;
                            case InhetoBaseTypes.Half:
                                Span<Half> hs = stackalloc Half[1] { (Half)obj! };
                                var hb = MemoryMarshal.AsBytes(hs);
                                stream.Write(hb);
                                // write 2 more bytes to keep alignment of name header and 32bit valueOffset.
                                stream.WriteByte(0);
                                stream.WriteByte(0);
                                break;
                            case InhetoBaseTypes.Single:
                                Span<float> fs = stackalloc float[1] { (float)obj! };
                                var fb = MemoryMarshal.AsBytes(fs);
                                stream.Write(fb);
                                break;
                            case InhetoBaseTypes.DateOnly:
                                stream.Write(((DateOnly)obj).DayNumber);
                                break;
                            case InhetoBaseTypes.ListString:
                                stream.Write(((List<string>)obj).Count);
                                ret = new NodeInfo(parentNodeIndex, thisNodeIndex, valueOffsetPos, InhetoKindMasks.InhetoBaseType, (byte)kind.kindType, null, propName);
                                TryUpdateCircularOffsets(oType, propName, ret);
                                valueOffsets[obj] = new valueOffset(oType, ret);
                                var lsCounter = 0u;
                                foreach (var item in (List<string>)obj)
                                {
                                    GraphObject(item, thisNodeIndex, string.Intern($"#{lsCounter++}"), true);
                                }
                                return ret;
                            default:
                                stream.Write(0u);
                                break;
                        }

                        ret = new NodeInfo(parentNodeIndex, thisNodeIndex, valueOffsetPos, InhetoKindMasks.InhetoBaseType, (byte)kind.kindType, null, propName);
                        TryUpdateCircularOffsets(oType, propName, ret);
                        valueOffsets[obj] = new valueOffset(oType, ret);

                        return ret;
                    }

                case InhetoKindMasks.InhetoPromotedType:
                    {
                        switch ((InhetoPromotedTypes)kind.kindType)
                        {
                            case InhetoPromotedTypes.Enum:
                                stream.Write(propName!);
                                valueOffsetPos = (uint)stream.Position;
                                var eType = (InhetoBaseTypes)((byte)kind.extendedType);
                                switch (eType)
                                {
                                    case InhetoBaseTypes.SByte:
                                        stream.Write((int)(sbyte)obj!);
                                        break;
                                    case InhetoBaseTypes.Byte:
                                        stream.Write((uint)(byte)obj!);
                                        break;
                                    case InhetoBaseTypes.Int16:
                                        stream.Write((int)(short)obj!);
                                        break;
                                    case InhetoBaseTypes.UInt16:
                                        stream.Write((uint)(ushort)obj!);
                                        break;
                                    case InhetoBaseTypes.Int32:
                                        stream.Write((int)obj!);
                                        break;
                                    case InhetoBaseTypes.UInt32:
                                        stream.Write((uint)obj!);
                                        break;
                                    default:
                                        stream.Write(0u);
                                        break;
                                }

                                ret = new NodeInfo(parentNodeIndex, thisNodeIndex, valueOffsetPos, InhetoKindMasks.InhetoPromotedType, (byte)InhetoPromotedTypes.Enum, (byte)eType, propName);
                                TryUpdateCircularOffsets(oType, propName, ret);
                                valueOffsets[obj] = new valueOffset(oType, ret);

                                return ret;
                            case InhetoPromotedTypes.Array:
                                stream.Write(propName!);
                                var aType = kind.extendedType;
                                valueOffsetPos = (uint)stream.Position;
                                stream.Write((uint)((Array)obj).Length);

                                ret = new NodeInfo(parentNodeIndex, thisNodeIndex, valueOffsetPos, InhetoKindMasks.InhetoPromotedType, (byte)InhetoPromotedTypes.Array, aType, propName);
                                TryUpdateCircularOffsets(oType, propName, ret);
                                valueOffsets[obj] = new valueOffset(oType, ret);

                                var aCounter = 0u;
                                foreach (var item in (Array)obj)
                                {
                                    GraphObject(item, thisNodeIndex, string.Intern($"#{aCounter}"), true);
                                    aCounter++;
                                }

                                return ret;
                            case InhetoPromotedTypes.ArrayMD:
                                // write rank lengths
                                var arr = (Array)obj;
                                for (int i = 0; i < arr.Rank; i++)
                                {
                                    stream.Write7BitEncodedInt(arr.GetLength(i));
                                }
                                stream.Write(propName!);
                                var mdType = kind.extendedType;
                                valueOffsetPos = (uint)stream.Position;
                                stream.Write(0u);

                                ret = new NodeInfo(parentIndex: parentNodeIndex, nodeIndex: thisNodeIndex, valueOffsetPos, InhetoKindMasks.InhetoPromotedType, (byte)InhetoPromotedTypes.ArrayMD, mdType, propName);
                                TryUpdateCircularOffsets(oType, propName, ret);
                                valueOffsets[obj] = new valueOffset(oType, ret);

                                var mdCounter = 0u;
                                foreach (var item in MdPathEnumerable.Enumerate((Array)obj))
                                {
                                    GraphObject(item.Value, thisNodeIndex, string.Intern(item.Path), true);
                                    mdCounter++;
                                }
                                stream.WriteAtOffset(valueOffsetPos, mdCounter);
                                return ret;
                            case InhetoPromotedTypes.Enumerable:
                                stream.Write(propName!);
                                var enType = kind.extendedType;
                                valueOffsetPos = (uint)stream.Position;
                                stream.Write(0u);

                                ret = new NodeInfo(parentIndex: parentNodeIndex, nodeIndex: thisNodeIndex, valueOffsetPos, InhetoKindMasks.InhetoPromotedType, (byte)InhetoPromotedTypes.Enumerable, enType, propName);
                                TryUpdateCircularOffsets(oType, propName, ret);
                                valueOffsets[obj] = new valueOffset(oType, ret);

                                var anCounter = 0u;
                                foreach (var item in (IEnumerable)obj)
                                {
                                    GraphObject(item, thisNodeIndex, string.Intern($"#{anCounter}"), true);
                                    anCounter++;
                                }
                                stream.WriteAtOffset(valueOffsetPos, anCounter);
                                return ret;
                            case InhetoPromotedTypes.NameValueCollection:
                                stream.Write(propName!);
                                valueOffsetPos = (uint)stream.Position;
                                var nvc = (NameValueCollection)obj;
                                stream.Write((uint)(nvc.Count));


                                ret = new NodeInfo(parentNodeIndex, thisNodeIndex, valueOffsetPos, InhetoKindMasks.InhetoPromotedType, (byte)InhetoPromotedTypes.NameValueCollection, null, propName);
                                TryUpdateCircularOffsets(oType, propName, ret);
                                valueOffsets[obj] = new valueOffset(oType, ret);

                                foreach (string key in nvc)
                                {
                                    GraphObject(nvc.GetValues(key), thisNodeIndex, $"!{key}", true);
                                }

                                return ret;
                            case InhetoPromotedTypes.StringCollection:
                                stream.Write(propName!);
                                valueOffsetPos = (uint)stream.Position;
                                var sc = (StringCollection)obj;
                                stream.Write((uint)(sc).Count);

                                ret = new NodeInfo(parentIndex: parentNodeIndex, nodeIndex: thisNodeIndex, valueOffsetPos, InhetoKindMasks.InhetoPromotedType, (byte)InhetoPromotedTypes.StringCollection, null, propName);
                                TryUpdateCircularOffsets(oType, propName, ret);
                                valueOffsets[obj] = new valueOffset(oType, ret);

                                var scCounter = 0u;
                                foreach (var item in sc)
                                {
                                    GraphObject(item, thisNodeIndex, string.Intern($"#{scCounter}"), true);
                                    scCounter++;
                                }

                                return ret;
                            case InhetoPromotedTypes.StaticClass:
                                stream.Write(propName);
                                valueOffsetPos = (uint)stream.Position;
                                stream.Write(0u);

                                ret = new NodeInfo(parentIndex: parentNodeIndex, nodeIndex: thisNodeIndex, valueOffsetPos, InhetoKindMasks.InhetoPromotedType, (byte)InhetoPromotedTypes.StaticClass, null, propName);
                                TryUpdateCircularOffsets(oType, propName, ret);
                                valueOffsets[obj] = new valueOffset(oType, ret);
                                return ret;

                            default:
                                stream.Write(propName!);
                                valueOffsetPos = (uint)stream.Position;
                                stream.Write(0u);

                                ret = new NodeInfo(parentNodeIndex, thisNodeIndex, valueOffsetPos, InhetoKindMasks.InhetoPromotedType, kind.kindType, null, propName);
                                TryUpdateCircularOffsets(oType, propName, ret);
                                valueOffsets[obj] = new valueOffset(oType, ret);

                                return ret;

                        }
                    }

                case InhetoKindMasks.InhetoCollectionType:
                    stream.Write(propName!);
                    valueOffsetPos = (uint)stream.Position;
                    stream.Write(0u);

                    ret = new NodeInfo(parentNodeIndex, thisNodeIndex, valueOffsetPos, InhetoKindMasks.InhetoCollectionType, (byte)kind.kindType, (byte)kind.extendedType, propName);
                    TryUpdateCircularOffsets(oType, propName, ret);
                    valueOffsets[obj] = new valueOffset(oType, ret);

                    switch (kind.collectionFamily)
                    {
                        case UtilsAndExtensions.GenericFamily.List:
                        case UtilsAndExtensions.GenericFamily.Queue:
                        case UtilsAndExtensions.GenericFamily.ConcurrentQueue:
                        case UtilsAndExtensions.GenericFamily.Stack:
                        case UtilsAndExtensions.GenericFamily.ConcurrentStack:
                        case UtilsAndExtensions.GenericFamily.ObservableCollection:
                        case UtilsAndExtensions.GenericFamily.ConcurrentBag:
                        case UtilsAndExtensions.GenericFamily.ReadOnlyCollection:
                        case UtilsAndExtensions.GenericFamily.SortedSet:
                        case UtilsAndExtensions.GenericFamily.HashSet:
                            var counter = 0u;
                            foreach (var item in (IEnumerable)obj)
                            {
                                GraphObject(item, thisNodeIndex, string.Intern($"#{counter}"), true);
                                counter++;
                            }
                            stream.WriteAtOffset(valueOffsetPos, counter);
                            break;

                        case UtilsAndExtensions.GenericFamily.Dictionary:
                        case UtilsAndExtensions.GenericFamily.ConcurrentDictionary:
                        case UtilsAndExtensions.GenericFamily.SortedDictionary:
                        case UtilsAndExtensions.GenericFamily.SortedList:
                        case UtilsAndExtensions.GenericFamily.StringKeyDictionary:
                        case UtilsAndExtensions.GenericFamily.StringKeyConcurrentDictionary:
                        case UtilsAndExtensions.GenericFamily.StringKeySortedDictionary:
                        case UtilsAndExtensions.GenericFamily.StringKeySortedList:
                            var dictCounter = 0u;
                            var declaredKeyType = UtilsAndExtensions.GetDictTypes(oType).KeyType;
                            bool formattedKeys = IsSafeKeyType(Nullable.GetUnderlyingType(declaredKeyType) ?? declaredKeyType);
                            foreach (DictionaryEntry kvp in (IDictionary)obj)
                            {
                                if (formattedKeys)
                                {
                                    var keyText = SafeKeyTypeToString(kvp.Key);
                                    if (keyText is null)
                                        throw CreateDictionaryKeyException(kvp.Key, thisNodeIndex);
                                    GraphObject(kvp.Value, thisNodeIndex, $"!{keyText}", true);
                                }
                                else
                                {
                                    GraphObject(kvp, thisNodeIndex, string.Intern($"#{dictCounter}"), true);
                                }
                                dictCounter++;
                            }
                            stream.WriteAtOffset(valueOffsetPos, dictCounter);
                            break;

                    }

                    return ret;

            }

            stream.Write((byte)InhetoBaseTypes.ComplexType);
            stream.Write(propName);
            valueOffsetPos = (uint)stream.Position;

            ret = new NodeInfo(parentNodeIndex, thisNodeIndex, valueOffsetPos, InhetoKindMasks.InhetoBaseType, (byte)InhetoBaseTypes.ComplexType, null, propName);
            TryUpdateCircularOffsets(oType, propName, ret);
            valueOffsets[obj] = new valueOffset(oType, ret);

            return ret;

        }

        /// <summary>
        /// Emits the existing type-marker grammar for a non-null value during name-header construction.<br/>
        /// Called only by WriteNodeForType after its filtering, duplicate and custom-hook handling.<br/>
        /// Uses the caller's runtime Type and retains value inspection where collection classification requires it.<br/>
        /// </summary>
        /// <param name="obj">The non-null source value associated with the header.<br/></param>
        /// <param name="oType">The runtime Type already obtained by GraphObject.<br/></param>
        /// <param name="memberPath">Existing full path used only by strict singleton-member validation.<br/></param>
        /// <returns>The kind, collection family and type markers used by the remaining header writer.<br/></returns>
        private (InhetoKindMasks kind, UtilsAndExtensions.GenericFamily collectionFamily, byte kindType, byte extendedType) WriteNodeTypeMarkers(object obj, Type oType, string memberPath)
        {
            InhetoBaseTypes baseType;
            Type? extendedType = null;
            // the WriteNodeMarkersFromType method checks if the type is a known base, promoted, or array type using only the oType parameter and provides recursion when needed. like arrays of arrays.
            var res = WriteNodeMarkersFromType(oType, out extendedType);
            if (res.kind == InhetoKindMasks.InhetoBaseType && res.nodeType != (byte)InhetoBaseTypes.Unknown)
            {
                return res;
            }
            else if (res.kind == InhetoKindMasks.InhetoPromotedType)
            {
                return res;
            }

            // nope. check if the type is a generic type, including nullable<>
            if (UtilsAndExtensions.GenericAnalyzer.TryAnalyzeForGenericType(extendedType, out var match))
            {
                var genMatch = WriteGenericType(obj, oType, match);
                return (genMatch.kind, genMatch.family, genMatch.nodeType, genMatch.keyType);
            }

            // nope.  check if the type is or derives from an enumerable type
            if (!oType.IsArray && obj is IEnumerable)
            {
                // nope.  check if the type is or derives from IEnumerable<> type
                if (oType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)) is var gType && gType != null)
                {
                    var gParams = gType.GetGenericArguments();
                    if (gParams.Length == 1 && InhetoBaseTypesMap.TryGetValue(gParams[0].TypeHandle, out baseType))
                    {
                        stream.Write((byte)((byte)InhetoPromotedTypes.Enumerable | (byte)InhetoKindMasks.InhetoPromotedType));
                        stream.Write((byte)baseType);
                        return (InhetoKindMasks.InhetoPromotedType, UtilsAndExtensions.GenericFamily.None, (byte)InhetoPromotedTypes.Enumerable, (byte)baseType);
                    }
                    else
                    {
                        stream.Write((byte)((byte)InhetoPromotedTypes.Enumerable | (byte)InhetoKindMasks.InhetoPromotedType));
                        stream.Write((byte)InhetoBaseTypes.ComplexType);
                        return (InhetoKindMasks.InhetoPromotedType, UtilsAndExtensions.GenericFamily.None, (byte)InhetoPromotedTypes.Enumerable, (byte)InhetoBaseTypes.ComplexType);

                    }
                }
                else // non-generic IEnumerable
                {
                    stream.Write((byte)((byte)InhetoPromotedTypes.Enumerable | (byte)InhetoKindMasks.InhetoPromotedType));
                    stream.Write((byte)InhetoBaseTypes.ComplexType);
                    return (InhetoKindMasks.InhetoPromotedType, UtilsAndExtensions.GenericFamily.None, (byte)InhetoPromotedTypes.Enumerable, 0);
                }
            }

            if (ZeroInputSingletonDetector.IsZeroInputSingleton(obj))
            {
                if (options.FailOnUnsupportedType)
                    EnsureSingletonMembersBoxable(oType, memberPath);
                stream.Write((byte)((byte)InhetoKindMasks.InhetoPromotedType | (byte)InhetoPromotedTypes.StaticClass));
                return (InhetoKindMasks.InhetoPromotedType, UtilsAndExtensions.GenericFamily.None, (byte)InhetoPromotedTypes.StaticClass, 0);
            }


            // nope.  treat as a complex type
            stream.Write((byte)InhetoBaseTypes.ComplexType);
            return (InhetoKindMasks.InhetoBaseType, UtilsAndExtensions.GenericFamily.None, (byte)InhetoBaseTypes.ComplexType, 0);

            (InhetoKindMasks kind, UtilsAndExtensions.GenericFamily collectionFamily, byte nodeType, byte extendedType) WriteNodeMarkersFromType(Type oType, out Type? extendedType)
            {
#if DEBUG
                var oTypeName = oType.Name;
#endif
                // check if type is a base inheto type
                if (InhetoBaseTypesMap.TryGetValue(oType.TypeHandle, out var baseType))
                {
                    stream.Write((byte)baseType);
                    extendedType = null;
                    return (InhetoKindMasks.InhetoBaseType, UtilsAndExtensions.GenericFamily.None, (byte)baseType, 0);
                }

                if (InhetoPromotedTypesMap.TryGetValue(oType.TypeHandle, out var promotedType))
                {

                    stream.Write((byte)((byte)InhetoKindMasks.InhetoPromotedType | (byte)promotedType));
                    extendedType = null;
                    return (InhetoKindMasks.InhetoPromotedType, UtilsAndExtensions.GenericFamily.None, (byte)promotedType, 0);
                }
                if (InhetoPromotedTypesMap.TryGetValue(oType.IsGenericType ? oType.GetGenericTypeDefinition().TypeHandle : oType.TypeHandle, out var genericPromotedType))
                {
                    stream.Write((byte)((byte)InhetoKindMasks.InhetoPromotedType | (byte)genericPromotedType));
                    // write generic argument count
                    var gArgs = oType.GetGenericArguments();
                    stream.Write((byte)gArgs.Length);
                    foreach (var arg in gArgs)
                    {
                        WriteNodeMarkersFromType(arg, out var _);
                    }
                    extendedType = null;
                    return (InhetoKindMasks.InhetoPromotedType, UtilsAndExtensions.GenericFamily.None, (byte)promotedType, 0);
                }

                // nope. check if the type is an enum type
                if (oType.IsEnum)
                {
                    stream.Write((byte)((byte)InhetoPromotedTypes.Enum | (byte)InhetoKindMasks.InhetoPromotedType));
                    extendedType = oType.GetEnumUnderlyingType();
                    var enumType = InhetoBaseTypesMap[extendedType.TypeHandle];
                    stream.Write((byte)enumType);
                    return (InhetoKindMasks.InhetoPromotedType, UtilsAndExtensions.GenericFamily.None, (byte)InhetoPromotedTypes.Enum, (byte)enumType);
                }

                // nope. check if type is an array type
                if (oType.IsArray)
                {
                    var rank = oType.GetArrayRank();
                    var depth = 1;
                    var elementType = oType.GetElementType()!;
                    var nullableLeaf = elementType;
                    var nullableDepth = 1;
                    while (nullableLeaf.IsArray)
                    {
                        nullableDepth++;
                        nullableLeaf = nullableLeaf.GetElementType()!;
                    }
                    if (Nullable.GetUnderlyingType(nullableLeaf) is Type nullableUnderlying)
                    {
                        // Nullable base markers already exist in the grammar. Preserve the declared
                        // leaf even though non-null Nullable<T> values box as T when children are written.
                        var nullableStorage = nullableUnderlying.IsEnum ? nullableUnderlying.GetEnumUnderlyingType() : nullableUnderlying;
                        var nullableBase = InhetoBaseTypesMap.TryGetValue(nullableStorage.TypeHandle, out var mappedNullableBase)
                            ? mappedNullableBase : InhetoBaseTypes.ComplexType;
                        var arrayMarker = rank == 1 ? InhetoPromotedTypes.Array : InhetoPromotedTypes.ArrayMD;
                        stream.Write((byte)((byte)InhetoKindMasks.InhetoPromotedType | (byte)arrayMarker));
                        stream.Write(checked((byte)(rank == 1 ? nullableDepth : rank)));
                        stream.Write((byte)((byte)InhetoKindMasks.InhetoNullableBaseType | (byte)nullableBase));
                        extendedType = elementType;
                        return (InhetoKindMasks.InhetoPromotedType, UtilsAndExtensions.GenericFamily.None, (byte)arrayMarker, (byte)nullableBase);
                    }
                    if (rank == 1)
                    {
                        if (!elementType.IsArray && InhetoBaseTypesMap.TryGetValue(elementType.TypeHandle, out baseType))
                        {
                            stream.Write((byte)((byte)InhetoPromotedTypes.Array | (byte)InhetoKindMasks.InhetoPromotedType));
                            stream.Write((byte)depth);
                            stream.Write((byte)baseType);
                            extendedType = elementType;
                            return (InhetoKindMasks.InhetoPromotedType, UtilsAndExtensions.GenericFamily.None, (byte)InhetoPromotedTypes.Array, (byte)baseType);
                        }
                        else if (elementType.IsEnum)
                        {
                            stream.Write((byte)((byte)InhetoPromotedTypes.Array | (byte)InhetoKindMasks.InhetoPromotedType));
                            stream.Write((byte)depth);
                            extendedType = elementType.GetEnumUnderlyingType();
                            var enumType = InhetoBaseTypesMap[extendedType.TypeHandle];
                            stream.Write((byte)enumType);
                            return (InhetoKindMasks.InhetoPromotedType, UtilsAndExtensions.GenericFamily.None, (byte)InhetoPromotedTypes.Array, (byte)enumType);
                        }
                        else if (elementType.IsArray)
                        {
                            stream.Write((byte)((byte)InhetoPromotedTypes.Array | (byte)InhetoKindMasks.InhetoPromotedType));
                            var eType = elementType;
                            while (eType.IsArray)
                            { depth++; eType = eType.GetElementType()!; }
                            stream.Write((byte)depth);
                            var aNode = WriteNodeMarkersFromType(eType, out extendedType);
                            if (aNode.kind == InhetoKindMasks.InhetoBaseType && aNode.nodeType == (byte)InhetoBaseTypes.Unknown)
                            { stream.Write((byte)InhetoBaseTypes.ComplexType); baseType = InhetoBaseTypes.ComplexType; }
                            return (InhetoKindMasks.InhetoPromotedType, UtilsAndExtensions.GenericFamily.None, (byte)InhetoPromotedTypes.Array, (byte)baseType);
                        }
                        else if (InhetoPromotedTypesMap.TryGetValue(elementType.TypeHandle, out var pType))
                        {
                            stream.Write((byte)((byte)InhetoPromotedTypes.Array | (byte)InhetoKindMasks.InhetoPromotedType));
                            stream.Write((byte)depth);
                            stream.Write((byte)((byte)pType | (byte)InhetoKindMasks.InhetoPromotedType));
                            extendedType = elementType;
                            return (InhetoKindMasks.InhetoPromotedType, UtilsAndExtensions.GenericFamily.None, (byte)InhetoPromotedTypes.Array, (byte)pType);
                        }
                        else
                        {
                            // treat element type as a complex type and serialize it as such
                            stream.Write((byte)((byte)InhetoPromotedTypes.Array | (byte)InhetoKindMasks.InhetoPromotedType));
                            stream.Write((byte)depth);
                            stream.Write((byte)InhetoBaseTypes.ComplexType);
                            extendedType = elementType;
                            return (InhetoKindMasks.InhetoPromotedType, UtilsAndExtensions.GenericFamily.None, (byte)InhetoPromotedTypes.Array, (byte)InhetoBaseTypes.ComplexType);
                        }
                    }
                    else // multi-dimensional array
                    {
                        if (InhetoBaseTypesMap.TryGetValue(elementType.TypeHandle, out baseType))
                        {
                            stream.Write((byte)((byte)InhetoPromotedTypes.ArrayMD | (byte)InhetoKindMasks.InhetoPromotedType));
                            stream.Write((byte)rank);
                            stream.Write((byte)baseType);
                            extendedType = elementType;
                            return (InhetoKindMasks.InhetoPromotedType, UtilsAndExtensions.GenericFamily.None, (byte)InhetoPromotedTypes.ArrayMD, (byte)baseType);
                        }
                        // commented out because not supported by clr.  cannot do multi-dimensional arrays of arrays
                        //else if (elementType.IsArray)
                        else if (elementType.IsEnum)
                        {
                            extendedType = elementType.GetEnumUnderlyingType();
                            var enumType = InhetoBaseTypesMap[extendedType.TypeHandle];
                            stream.Write((byte)((byte)InhetoPromotedTypes.ArrayMD | (byte)InhetoKindMasks.InhetoPromotedType));
                            stream.Write((byte)rank);
                            stream.Write((byte)((byte)InhetoPromotedTypes.Enum | (byte)InhetoKindMasks.InhetoPromotedType));
                            stream.Write((byte)enumType);
                            return (InhetoKindMasks.InhetoPromotedType, UtilsAndExtensions.GenericFamily.None, (byte)InhetoPromotedTypes.ArrayMD, (byte)InhetoPromotedTypes.Enum | (byte)InhetoKindMasks.InhetoPromotedType);
                        }
                        else if (InhetoPromotedTypesMap.TryGetValue(elementType.TypeHandle, out var promotedElementType))
                        {
                            stream.Write((byte)((byte)InhetoPromotedTypes.ArrayMD | (byte)InhetoKindMasks.InhetoPromotedType));
                            stream.Write((byte)rank);
                            stream.Write((byte)((byte)promotedElementType | (byte)InhetoKindMasks.InhetoPromotedType));
                            extendedType = elementType;
                            return (InhetoKindMasks.InhetoPromotedType, UtilsAndExtensions.GenericFamily.None, (byte)InhetoPromotedTypes.ArrayMD, (byte)promotedElementType);
                        }

                        else
                        {
                            // treat element type as a complex type and serialize it as such
                            stream.Write((byte)((byte)InhetoPromotedTypes.ArrayMD | (byte)InhetoKindMasks.InhetoPromotedType));
                            stream.Write((byte)rank);
                            stream.Write((byte)InhetoBaseTypes.ComplexType);
                            extendedType = elementType;
                            return (InhetoKindMasks.InhetoPromotedType, UtilsAndExtensions.GenericFamily.None, (byte)InhetoPromotedTypes.ArrayMD, (byte)InhetoBaseTypes.ComplexType);
                        }
                    }
                }

                extendedType = oType;
                return default;
            }
        }






        //====== TYPES ======








        /// <summary>
        /// Describes one writer name-header entry. Its offsets refer to the current serialized stream, not to a durable CLR object identity.<br/>
        /// </summary>
        public record struct NodeInfo()
        {
            //======  FIELDS  ======
            /// <summary>
            /// Stores the optional extended type byte required by the applicable encoded node kind.<br/>
            /// </summary>
            public readonly byte? extendedType;
            /// <summary>
            /// Stores the representation-family selector for this name-header entry.<br/>
            /// </summary>
            public readonly InhetoKindMasks kind;
            /// <summary>
            /// Stores the base, promoted or collection identifier interpreted within kind.<br/>
            /// </summary>
            public readonly byte kindType;
            /// <summary>
            /// Stores the byte offset at which this name-header entry begins.<br/>
            /// </summary>
            public readonly uint nodeIndex;
            /// <summary>
            /// Stores the parent name-header entry byte offset; zero denotes no encoded parent.<br/>
            /// </summary>
            public readonly uint parentIndex;
            /// <summary>
            /// Stores the optional encoded member token associated with this writer entry.<br/>
            /// </summary>
            public string? propName;
            /// <summary>
            /// Stores the byte position of the header value/offset slot to backpatch, not the final payload location.<br/>
            /// </summary>
            public readonly uint valueOffsetPos;








            //======  CONSTRUCTORS  ======
            /// <summary>
            /// Creates header metadata from the supplied byte offsets and representation identifiers; does not write a header to a stream.<br/>
            /// </summary>
            /// <param name="parentIndex">Parent header byte offset, or zero for no encoded parent.<br/></param>
            /// <param name="nodeIndex">Byte offset of the new header entry.<br/></param>
            /// <param name="valueOffsetPos">Byte position of the header value/offset slot to backpatch.<br/></param>
            /// <param name="kind">Representation-family selector for the encoded node.<br/></param>
            /// <param name="kindType">Type/family identifier interpreted within kind.<br/></param>
            /// <param name="extendedType">Optional extended type byte for the encoded representation.<br/></param>
            /// <param name="propName">Encoded member token associated with this header entry, or null when none is required.<br/></param>
            public NodeInfo(uint parentIndex, uint nodeIndex, uint valueOffsetPos, InhetoKindMasks kind, byte kindType, byte? extendedType = null, string? propName = null) : this()
            {
                this.extendedType = extendedType;
                this.kindType = (byte)kindType;
                this.kind = kind;
                this.valueOffsetPos = valueOffsetPos;
                this.nodeIndex = nodeIndex;
                this.parentIndex = parentIndex;
                this.propName = propName;
                // write values to debug output
                //Debug.WriteLine(
                //    string.Format(
                //        "NodInfo: {0,-16} {1,-16} {2,-16} {3,-28} {4,-16} {5}",
                //        $"parentIdx: {parentIndex}",
                //        $"nodeIdx: {nodeIndex}",
                //        $"offsetPos: {valueOffsetPos}",
                //        $"kind: {kind}",
                //        $"type: {kindType}",
                //        $"propName: {propName}"
                //    )
                //);
            }
        };
        internal record struct nameValueNode()
        {
            //======  CONSTRUCTORS  ======
            public nameValueNode(InhetoKindMasks kind, InhetoBaseTypes? valueType, InhetoBaseTypes? keyType, InhetoPromotedTypes? pType, InhetoCollectionTypes? cType, InhetoCollectionAuxiliaryTypes? xType, string? name, uint nodeIndex, uint parentNodeIndex, uint nextNodeIndex, uint valueOffset) : this()
            {
                this.kind = kind;
                this.keyType = keyType;
                this.valueType = valueType;
                this.pType = pType;
                this.cType = cType;
                this.xType = xType;
                this.name = name;
                this.nodeIndex = nodeIndex;
                this.parentNodeIndex = parentNodeIndex;
                this.nextNodeIndex = nextNodeIndex;
                this.valueOffset = valueOffset;
            }








            //======  PROPERTIES  ======
            public InhetoKindMasks kind { get; set; }
            public InhetoBaseTypes? keyType { get; set; }
            public InhetoArrayTypes? aType { get; set; }
            public InhetoCollectionTypes? cType { get; set; }
            public bool? isNullableValueType { get; set; }
            public int jDepth { get; set; }
            public int mdRank { get; set; }
            public string? name { get; set; }
            public object? nextNode { get; set; }
            public uint nextNodeIndex { get; set; }
            public uint nodeIndex { get; set; }
            public uint parentNodeIndex { get; set; }
            public InhetoPromotedTypes? pType { get; set; }
            public uint valueOffset { get; set; }
            public InhetoBaseTypes? valueType { get; set; }
            public InhetoCollectionAuxiliaryTypes? xType { get; set; }








            //======  CONSTRUCTORS  ======
            //======  METHODS  ======
            public static nameValueNode CreateFromStream(BufferStream stream, uint nodeIndex)
            {
                using var nodeStream = stream.Segment(nodeIndex);
                var parentNodeIndex = nodeStream.Read7BitEncodedUInt();
                var typeByte = nodeStream.ReadByte();
                var kind = (InhetoKindMasks)(typeByte & 0b1100_0000);
                InhetoBaseTypes? valueType = null;
                InhetoPromotedTypes? pType = null;
                InhetoCollectionTypes? cType = null;
                InhetoCollectionAuxiliaryTypes? xType = null;
                InhetoBaseTypes? keyType = null;
                InhetoArrayTypes? aType = null;
                int jDepth = 0;
                int mdRank = 0;
                var propName = "";
                var dataOffset = 0u;
                bool? isNullableValueType = null;

                ParseNodeType(nodeStream, typeByte, ref aType, ref jDepth, ref mdRank, ref valueType, ref isNullableValueType, ref pType, ref cType, ref xType, ref keyType);
                if (mdRank > 1)
                {
                    for (int i = 0; i < mdRank; i++)
                    {
                        var len = nodeStream.Read7BitEncodedUInt();
                    }
                }
                propName = nodeStream.ReadString();
                dataOffset = nodeStream.ReadUInt32();

                return new nameValueNode(kind, valueType, keyType, pType, cType, xType, propName, nodeIndex, parentNodeIndex, (uint)nodeStream.Position + nodeIndex, dataOffset) { aType = aType, jDepth = jDepth, mdRank = mdRank };

                static void ParseNodeType(BufferStream nodeStream, byte typeByte, ref InhetoArrayTypes? aType, ref int jDepth, ref int mdRank, ref InhetoBaseTypes? valueType, ref bool? isNullableValueType, ref InhetoPromotedTypes? pType, ref InhetoCollectionTypes? cType, ref InhetoCollectionAuxiliaryTypes? xType, ref InhetoBaseTypes? keyType)
                {
                    var kind = (InhetoKindMasks)(typeByte & 0b1100_0000);
                    switch (kind)
                    {
                        case InhetoKindMasks.InhetoBaseType:
                            valueType = (InhetoBaseTypes)((byte)typeByte & InhetoTypeFlags.BASE_TYPE_MASK);
                            break;
                        case InhetoKindMasks.InhetoNullableBaseType:
                            valueType = (InhetoBaseTypes)((byte)typeByte & InhetoTypeFlags.BASE_TYPE_MASK);
                            isNullableValueType = true;
                            break;
                        case InhetoKindMasks.InhetoPromotedType:
                            pType = (InhetoPromotedTypes)((byte)typeByte & InhetoTypeFlags.BASE_TYPE_MASK);
                            switch (pType)
                            {
                                case InhetoPromotedTypes.Enum:
                                    var enumValueType = (InhetoBaseTypes)nodeStream.ReadByte();
                                    ParseNodeType(nodeStream, (byte)enumValueType, ref aType, ref jDepth, ref mdRank, ref valueType, ref isNullableValueType, ref pType, ref cType, ref xType, ref keyType);
                                    break;
                                case InhetoPromotedTypes.Array:
                                    jDepth = nodeStream.Read7BitEncodedInt();
                                    var arrayValueType = (InhetoBaseTypes)nodeStream.ReadByte();
                                    aType = InhetoArrayTypes.Array;
                                    ParseNodeType(nodeStream, (byte)arrayValueType, ref aType, ref jDepth, ref mdRank, ref valueType, ref isNullableValueType, ref pType, ref cType, ref xType, ref keyType);
                                    break;
                                case InhetoPromotedTypes.ArrayMD:
                                    mdRank = nodeStream.ReadByte();
                                    var arrayMDValueType = (InhetoBaseTypes)nodeStream.ReadByte();
                                    aType = InhetoArrayTypes.ArrayMD;
                                    ParseNodeType(nodeStream, (byte)arrayMDValueType, ref aType, ref jDepth, ref mdRank, ref valueType, ref isNullableValueType, ref pType, ref cType, ref xType, ref keyType);
                                    break;
                                case InhetoPromotedTypes.Enumerable:
                                    valueType = (InhetoBaseTypes)nodeStream.ReadByte();
                                    break;
                            }
                            break;
                        case InhetoKindMasks.InhetoNullablePromotedType:
                            pType = (InhetoPromotedTypes)((byte)typeByte & InhetoTypeFlags.BASE_TYPE_MASK);
                            break;
                        case InhetoKindMasks.InhetoCollectionType:

                            cType = (InhetoCollectionTypes)((byte)typeByte & InhetoTypeFlags.COLL_TYPE_MASK);
                            if ((typeByte & (byte)InhetoKindMasks.InhetoAuxiliaryData) == (byte)InhetoKindMasks.InhetoAuxiliaryData)
                                xType = (InhetoCollectionAuxiliaryTypes)(nodeStream.ReadByte());
                            if (cType >= InhetoCollectionTypes.Dictionary && cType <= InhetoCollectionTypes.SortedList)
                            {
                                keyType = (InhetoBaseTypes)nodeStream.ReadByte();
                                valueType = (InhetoBaseTypes)nodeStream.ReadByte();
                            }
                            else if (cType >= InhetoCollectionTypes.StringKeyDictionary && cType <= InhetoCollectionTypes.StringKeySortedList)
                            {
                                keyType = InhetoBaseTypes.String;
                                valueType = (InhetoBaseTypes)(nodeStream.ReadByte());
                            }
                            else if (cType >= InhetoCollectionTypes.List && cType <= InhetoCollectionTypes.HashSet)
                            {
                                valueType = (InhetoBaseTypes)nodeStream.ReadByte();
                            }
                            break;
                        default:
                            throw new Exception("Unknown kind marker");
                    }
                }
            }
        }
        private sealed class PreservedCustomMember
        {
            internal PreservedCustomMember(byte[] payload) => Payload = payload;
            internal byte[] Payload { get; }
        }

        private record struct valueOffset(Type? valueType, NodeInfo node, Func<object, object>? sz = null);

        /// <summary>
        /// Converts an already emitted custom header to an explicit Null without moving its name or value slot.<br/>
        /// The callback has already run once; no custom payload or type-name bytes are written for its null result.<br/>
        /// This preserves deferred execution, collection positions and the payload writer's current position.<br/>
        /// </summary>
        /// <param name="node">The custom header whose one-byte type marker and offset slot are replaced.<br/></param>
        private void MarkCustomNull(NodeInfo node)
        {
            var reader = new NameHeaderSpanReader(stream.AsReadOnlySpan, checked((int)node.nodeIndex), stream.StringEncoding);
            _ = reader.Read7BitEncodedUInt();
            stream.WriteAtOffset((uint)reader.Position, (byte)InhetoBaseTypes.Null);
            stream.WriteAtOffset(node.valueOffsetPos, 0u);
        }

        /// <summary>
        /// Normalizes duplicate headers that refer to a custom value whose callback returned null.<br/>
        /// Runs only after such a result, borrowing the completed stream without allocating metadata or decoding names.<br/>
        /// Writer-created aliases reference earlier headers, so one forward pass also propagates null through alias chains.<br/>
        /// Only fixed-width marker/offset slots change; all parent indices, names, counts and payload positions stay intact.<br/>
        /// </summary>
        /// <param name="endIndex">Index of the end-of-names sentinel, excluded from the scan.<br/></param>
        private void MarkNullAliases(uint endIndex)
        {
            ReadOnlySpan<byte> bytes = stream.AsReadOnlySpan;
            var reader = new NameHeaderSpanReader(bytes, 1, stream.StringEncoding);
            while ((uint)reader.Position < endIndex)
            {
                _ = reader.Read7BitEncodedUInt();
                uint markerOffset = (uint)reader.Position;
                byte marker = reader.PeekByte();
                reader.SkipType();
                int nameLength = reader.Read7BitEncodedInt();
                if (nameLength > 0) _ = reader.ReadSpan(nameLength);
                uint valueOffsetPosition = (uint)reader.Position;
                uint value = reader.ReadUInt32();
                if (marker is not ((byte)InhetoBaseTypes.DuplicateRef) and not ((byte)InhetoBaseTypes.DuplicateValue))
                    continue;
                var target = new NameHeaderSpanReader(bytes, checked((int)value), stream.StringEncoding);
                _ = target.Read7BitEncodedUInt();
                if (target.ReadByte() != (byte)InhetoBaseTypes.Null)
                    continue;
                stream.WriteAtOffset(markerOffset, (byte)InhetoBaseTypes.Null);
                stream.WriteAtOffset(valueOffsetPosition, 0u);
            }
        }
        /// <summary>
        /// Writes non-null custom text as UTF-8 inside the ordinary length-prefixed byte payload.<br/>
        /// Encodes directly into this serializer's owned stream without an intermediate byte array or a BOM.<br/>
        /// Uses the standard UTF-8 replacement fallback, independently of the stream's name-string encoding.<br/>
        /// The caller handles null before entering; an empty string emits a zero byte count.<br/>
        /// </summary>
        /// <param name="text">The immutable string returned by the developer's property-path hook.<br/></param>
        private void WriteCustomText(string text)
        {
            int count = Encoding.UTF8.GetByteCount(text);
            stream.Write7BitEncodedInt(count);
            if (count == 0) return;
            int position = checked((int)stream.Position);
            int end = checked(position + count);
            stream.SetLength(end);
            Encoding.UTF8.GetBytes(text.AsSpan(), stream.Span(position, count));
            stream.Position = end;
        }

        /// <summary>
        /// Writes an explicit Null name header when GraphObject must preserve a null slot.<br/>
        /// Applies the existing non-root filters before writing; root nodes bypass those filters.<br/>
        /// Ordinary null members are omitted by the caller and never enter this method.<br/>
        /// No payload, value-cache entry or temporary node descriptor is needed for an emitted Null.<br/>
        /// </summary>
        /// <param name="parentNodeIndex">The owning name-header index, or zero for the root.<br/></param>
        /// <param name="propName">The local slot name, or an empty string for the root.<br/></param>
        private void WriteNullNode(uint parentNodeIndex, string propName)
        {
            var memberPath = GetMemberPath(parentNodeIndex, propName);
            // The publicly mutable HashSet can contain null at runtime. Preserve that existing filter.
            // This literal intentionally represents an absent Type; it is never dereferenced.
            if (!string.IsNullOrEmpty(propName) && options.ExcludedTypes.Contains(null!)) return;
            if (!string.IsNullOrEmpty(propName) && options.ExcludedMembers.Contains(memberPath)) return;
            if (options.IncludedMembers.Count > 0 && !string.IsNullOrEmpty(propName) && !options.IncludedMembers.Contains(memberPath)) return;

            stream.Write7BitEncodedUInt(parentNodeIndex);
            stream.WriteByte((byte)InhetoBaseTypes.Null);
            stream.Write(propName);
            stream.Write(0u);
        }

        /// <summary>
        /// Creates a contextual failure for a dictionary key without a supported wire-text representation.<br/>
        /// Called only after formatting fails, so path reconstruction and diagnostic strings stay off successful writes.<br/>
        /// The partially built operation is not returned as a completed serialization.<br/>
        /// </summary>
        /// <param name="key">The rejected source key, which may itself be null.<br/></param>
        /// <param name="dictionaryNodeIndex">The dictionary header used to recover its source property path.<br/></param>
        /// <returns>A developer-facing exception identifying the dictionary and the key representation problem.<br/></returns>
        private NotSupportedException CreateDictionaryKeyException(object? key, uint dictionaryNodeIndex)
        {
            var path = GetPropPath(dictionaryNodeIndex);
            if (path.Length == 0) path = "<root>";
            var reason = key is Type type
                ? $"Type key '{type}' has no usable assembly-qualified name"
                : key is null ? "the key is null" : $"key of type '{key.GetType()}' has no supported textual representation";
            return new NotSupportedException($"Cannot serialize dictionary at '{path}': {reason}. Use a supported key or a custom serializer for the dictionary.");
        }

        /// <summary>
        /// Rejects representations that the ordinary object-based writer cannot preserve when strict mode is enabled.<br/>
        /// Called only after member/type filters, prior references and explicit custom serializers have taken precedence.<br/>
        /// Known scalar/promoted values and enumerable shapes retain their existing codecs; ordinary classes remain member-based graphs.<br/>
        /// An unrecognized struct with instance state must expose at least one member under the selected visibility policy; intentional member exclusions are not treated as type failures.<br/>
        /// Empty marker structs have no instance state to lose and remain valid. This is not a full graph preflight or a guarantee of CLR round-trip reconstruction.<br/>
        /// </summary>
        /// <param name="value">Non-null value already selected for ordinary serialization.<br/></param>
        /// <param name="runtimeType">Actual runtime type already obtained by the caller.<br/></param>
        /// <param name="memberPath">Existing full Inheto path, empty at the root; never constructed here on successful writes.<br/></param>
        /// <exception cref="NotSupportedException">A native address/handle, executable delegate or opaque stateful struct lacks an explicit representation.<br/></exception>
        private void EnsureSupportedValue(object value, Type runtimeType, string memberPath)
        {
            if (runtimeType == typeof(IntPtr) || runtimeType == typeof(UIntPtr) ||
                value is Delegate || value is SafeHandle || value is System.Runtime.InteropServices.CriticalHandle)
                throw CreateUnsupportedTypeException(runtimeType, memberPath, "native addresses, live handles and executable delegates require an explicit data representation");

            if (!runtimeType.IsValueType || runtimeType.IsEnum ||
                InhetoBaseTypesMap.ContainsKey(runtimeType.TypeHandle) ||
                InhetoPromotedTypesMap.ContainsKey(runtimeType.TypeHandle) || value is IEnumerable)
                return;

            if (HasSelectedSerializationMember(runtimeType))
                return;

            // A truly empty marker has only one possible state; a private-state struct can silently lose information.
            if (StrictRepresentationCache.Get(runtimeType).HasInstanceState)
                throw CreateUnsupportedTypeException(runtimeType, memberPath, "the struct has instance state but no readable members under the selected MemberTypes policy");
        }

        /// <summary>
        /// Tests metadata for a stateful struct using the same visibility families as ordinary member traversal.<br/>
        /// Does not invoke getters, enumerate a value, consult per-path exclusions or create accessor delegates.<br/>
        /// Uses lazy weakly keyed type-only metadata, independent of mutable options, paths and source objects; warmed checks allocate no member lists.<br/>
        /// </summary>
        /// <param name="runtimeType">Unrecognized non-enum struct whose ordinary member representation is being considered.<br/></param>
        /// <returns>True when the selected policy exposes a field or a readable non-indexed property; otherwise false.<br/></returns>
        private bool HasSelectedSerializationMember(Type runtimeType)
        {
            int policy = (int)options.MemberTypes;
            return (uint)policy <= (uint)MemberTypesEnum.PrivateFieldsOnly &&
                (StrictRepresentationCache.Get(runtimeType).MemberPolicyMask & (1 << policy)) != 0;
        }

        /// <summary>
        /// Builds an unsupported-representation failure only on an actual strict-mode rejection.<br/>
        /// Retains the full path and Type in exception Data for callers that should not parse diagnostic text.<br/>
        /// No exception, message, path rebuilding or dictionary allocation occurs through this method on successful writes.<br/>
        /// </summary>
        /// <param name="runtimeType">Rejected actual value type or declared unboxable member type.<br/></param>
        /// <param name="memberPath">Full Inheto path; the empty root is displayed as root but retained unchanged in Data.<br/></param>
        /// <param name="reason">Specific unsupported representation encountered by the writer.<br/></param>
        /// <returns>NotSupportedException with Inheto.Path and Inheto.Type context.<br/></returns>
        private static NotSupportedException CreateUnsupportedTypeException(Type runtimeType, string memberPath, string reason)
        {
            var exception = new NotSupportedException($"Cannot serialize type '{runtimeType.FullName ?? runtimeType.Name}' at '{(memberPath.Length == 0 ? "<root>" : memberPath)}': {reason}. Register a custom serializer for the value or its owner, select a valid member representation, or exclude the non-root member.");
            exception.Data["Inheto.Path"] = memberPath;
            exception.Data["Inheto.Type"] = runtimeType;
            return exception;
        }

        /// <summary>
        /// Prevents the existing zero-input heuristic from hiding an unboxable selected member in strict mode.<br/>
        /// Performs metadata-only checks in this otherwise non-traversing branch, without invoking getters, changing singleton eligibility or allocating ordinary member paths.<br/>
        /// Builds a child path only after an unboxable declaration is found; explicit member/type exclusions and inclusion policy still take precedence.<br/>
        /// </summary>
        /// <param name="runtimeType">Type already classified by the existing zero-input heuristic.<br/></param>
        /// <param name="memberPath">Existing path of this value, empty for the root.<br/></param>
        /// <exception cref="NotSupportedException">A selected member declaration cannot be boxed and has not been excluded.<br/></exception>
        private void EnsureSingletonMembersBoxable(Type runtimeType, string memberPath)
        {
            int policy = (int)options.MemberTypes;
            if ((uint)policy > (uint)MemberTypesEnum.PrivateFieldsOnly) return;
            int mask = 1 << policy;
            var members = StrictRepresentationCache.Get(runtimeType).UnboxableMembers;
            for (int i = 0; i < members.Length; i++)
            {
                var member = members[i];
                if ((member.PolicyMask & mask) == 0 || options.ExcludedTypes.Contains(member.Type)) continue;
                var path = memberPath + "." + member.Name;
                if (options.ExcludedMembers.Contains(path) || options.IncludedMembers.Count > 0 && !options.IncludedMembers.Contains(path)) continue;
                throw CreateUnsupportedTypeException(member.Type, path, "the selected zero-input member cannot be boxed for the object-based writer");
            }
        }

        /// <summary>Owns metadata allocated only when strict mode first needs an unrecognized type shape; weak keys do not root collectible CLR types.<br/></summary>
        private static class StrictRepresentationCache
        {
            private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Type, StrictRepresentationShape> Shapes;

            /// <summary>Disables beforefieldinit so the weak metadata store is initialized only by an actual strict-shape request, never merely by compiling the ordinary writer.<br/></summary>
            static StrictRepresentationCache() => Shapes = new();

            /// <summary>Returns immutable type-only metadata; the static factory delegate is reused and never closes over a writer, path, source value or options.<br/></summary>
            /// <param name="type">Unrecognized CLR type whose metadata is needed by an opt-in check.<br/></param>
            /// <returns>Shared immutable shape, built once for a retained type; concurrent first-use factories may harmlessly run more than once.<br/></returns>
            internal static StrictRepresentationShape Get(Type type) => Shapes.GetValue(type, static key => new StrictRepresentationShape(key));
        }

        /// <summary>Stores visibility masks and unboxable declarations only; contains no graph objects, option registrations, member paths or accessor delegates.<br/></summary>
        private sealed class StrictRepresentationShape
        {
            /// <summary>Records whether this CLR type has any instance fields, including non-public backing state.<br/></summary>
            internal readonly bool HasInstanceState;
            /// <summary>Contains one bit per MemberTypesEnum visibility family that exposes a readable property or field before intentional per-path filtering.<br/></summary>
            internal readonly int MemberPolicyMask;
            /// <summary>Contains only unboxable selected-member candidates and their applicable visibility masks; an empty array is shared.<br/></summary>
            internal readonly (int PolicyMask, string Name, Type Type)[] UnboxableMembers;

            /// <summary>Builds metadata without invoking instance/static getters, enumerating source values or constructing accessor delegates; all allocations are cold type-shape setup.<br/></summary>
            /// <param name="type">Type associated with the weak key; retained member Type references are safe under ConditionalWeakTable ephemeron semantics.<br/></param>
            internal StrictRepresentationShape(Type type)
            {
                List<(int PolicyMask, string Name, Type Type)>? unboxable = null;
                var fields = type.Fields(Flags.InstancePublic | Flags.InstancePrivate);
                HasInstanceState = fields.Count != 0;
                for (int i = 0; i < fields.Count; i++)
                {
                    var field = fields[i];
                    int mask = field.IsPublic
                        ? (1 << (int)MemberTypesEnum.PublicPropertiesAndFields) | (1 << (int)MemberTypesEnum.PublicFieldsOnly) | (1 << (int)MemberTypesEnum.PublicAndPrivateFields)
                        : (1 << (int)MemberTypesEnum.PublicAndPrivateFields) | (1 << (int)MemberTypesEnum.PrivateFieldsOnly);
                    MemberPolicyMask |= mask;
                    var memberType = field.FieldType;
                    if (memberType.IsByRefLike || memberType.IsByRef || memberType.IsPointer || memberType.IsFunctionPointer)
                        (unboxable ??= new()).Add((mask, field.Name, memberType));
                }
                var properties = type.Properties(Flags.InstancePublic);
                for (int i = 0; i < properties.Count; i++)
                {
                    var property = properties[i];
                    if (!property.CanRead || property.GetIndexParameters().Length != 0) continue;
                    int mask = (1 << (int)MemberTypesEnum.PublicPropertiesAndFields) | (1 << (int)MemberTypesEnum.PublicPropertiesOnly);
                    MemberPolicyMask |= mask;
                    var memberType = property.PropertyType;
                    if (memberType.IsByRefLike || memberType.IsByRef || memberType.IsPointer || memberType.IsFunctionPointer)
                        (unboxable ??= new()).Add((mask, property.Name, memberType));
                }
                UnboxableMembers = unboxable?.ToArray() ?? Array.Empty<(int, string, Type)>();
            }
        }
    };
}
