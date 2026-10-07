using System.Collections.Concurrent;
using System.Reflection;
using static Inheto.InhetoSerializer;

namespace Inheto;

public partial class InhetoBinary
{
    private static readonly ConcurrentDictionary<Type, Func<InhetoBinary, NameHeaderEntry, string, DeserializationOptions, Array>> nullableArrayReaders = new();
    private static readonly ConcurrentDictionary<Type, Func<InhetoBinary, NameHeaderEntry, IInhetoObject>> nullableArrayWrappers = new();

    /// <summary>
    /// Detects nullable value-type leaves without changing ordinary array dispatch.<br/>
    /// Traverses CLR array shape only; no instances or coordinate arrays are created.<br/>
    /// </summary>
    private static bool HasNullableArrayLeaf(Type type)
    {
        if (!type.IsArray) return false;
        do { type = type.GetElementType()!; } while (type.IsArray);
        return Nullable.GetUnderlyingType(type) is not null;
    }

    /// <summary>
    /// Reconstructs an array CLR shape from a known leaf and the existing rank/depth metadata.<br/>
    /// Shape reconstruction is separate from output allocation and preserves empty dimensions.<br/>
    /// </summary>
    private static Type ArrayTypeFromLeaf(Type leaf, NameHeaderEntry entry)
    {
        if (entry.Type.ArrayInfo.Rank > 1) return leaf.MakeArrayType(entry.Type.ArrayInfo.Rank);
        for (int depth = 0; depth < entry.Type.ArrayInfo.Depth; depth++) leaf = leaf.MakeArrayType();
        return leaf;
    }

    /// <summary>
    /// Resolves an agnostic nullable base array without inventing CLR identity for custom structs.<br/>
    /// Custom/complex nullable leaves require a typed destination and its applicable codec hooks.<br/>
    /// </summary>
    private static Type NullableArrayType(NameHeaderEntry entry)
    {
        var marker = entry.Type.ArrayInfo.ElementType.NullableBaseType;
        if (!inhetoBaseToTypeMap.TryGetValue(marker, out var underlying) || !underlying.IsValueType)
            throw new InvalidDataException("This nullable array requires a typed destination for its element shape.");
        return ArrayTypeFromLeaf(typeof(Nullable<>).MakeGenericType(underlying), entry);
    }

    /// <summary>
    /// Dispatches a nullable array to a cached closed generic reader after checking rank and depth.<br/>
    /// Reflection/delegate construction occurs once per destination array type, not per element.<br/>
    /// The caller owns operation-local reference caching; this method does not cache mutable output.<br/>
    /// </summary>
    private Array ReadNullableArray(Type arrayType, NameHeaderEntry entry, string path, DeserializationOptions options)
    {
        int depth = 0;
        for (var current = arrayType; current.IsArray; current = current.GetElementType()!) depth++;
        if (arrayType.GetArrayRank() != entry.Type.ArrayInfo.Rank || depth != entry.Type.ArrayInfo.Depth)
            throw new InvalidDataException($"Nullable array rank/depth mismatch at '{path}'.");
        var reader = nullableArrayReaders.GetOrAdd(arrayType, static type =>
            typeof(InhetoBinary).GetMethod(nameof(ReadNullableArrayElements), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(type.GetElementType()!)
                .CreateDelegate<Func<InhetoBinary, NameHeaderEntry, string, DeserializationOptions, Array>>());
        return reader(this, entry, path, options);
    }

    /// <summary>
    /// Materializes nullable leaves or nested nullable-array children using their complete child nodes.<br/>
    /// Ranks one through four use typed writes without Array.SetValue or coordinate-array boxing.<br/>
    /// Higher ranks retain the general Array fallback with one reusable coordinate array per output.<br/>
    /// Explicit null child nodes become default nullable values; empty arrays remain actual arrays.<br/>
    /// </summary>
    private Array ReadNullableArrayElements<T>(NameHeaderEntry entry, string path, DeserializationOptions options)
    {
        int rank = entry.Type.ArrayInfo.Rank;
        int count = checked((int)entry.ValueOffset);
        if (rank == 1)
        {
            var values = new T[count];
            for (int i = 0; i < values.Length; i++) values[i] = ReadNullableElement<T>($"{path}#{i}", options);
            return values;
        }

        // Retain the existing typed/generic construction paths; only explicit materialization prepares dimensions.
        var lengths = new int[rank];
        if (entry.Type.ArrayInfo.ArrayType != InhetoArrayTypes.ArrayMD || entry.CopyLengths(lengths) != rank)
            throw new InvalidDataException($"Missing nullable array dimensions at '{path}'.");
        long product = lengths.AsSpan().Contains(0) ? 0 : 1;
        foreach (int length in lengths)
        {
            if (length < 0) throw new InvalidDataException($"Negative array length at '{path}'.");
            if (product != 0) product = checked(product * length);
        }
        if (lengths.Length != rank || product != count)
            throw new InvalidDataException($"Nullable array shape/count mismatch at '{path}'.");
        switch (rank)
        {
            case 2:
                var a2 = new T[lengths[0], lengths[1]];
                for (int i = 0; i < lengths[0]; i++)
                    for (int j = 0; j < lengths[1]; j++)
                        a2[i, j] = ReadNullableElement<T>($"{path}@{i},{j}", options);
                return a2;
            case 3:
                var a3 = new T[lengths[0], lengths[1], lengths[2]];
                for (int i = 0; i < lengths[0]; i++)
                    for (int j = 0; j < lengths[1]; j++)
                        for (int k = 0; k < lengths[2]; k++)
                            a3[i, j, k] = ReadNullableElement<T>($"{path}@{i},{j},{k}", options);
                return a3;
            case 4:
                var a4 = new T[lengths[0], lengths[1], lengths[2], lengths[3]];
                for (int i = 0; i < lengths[0]; i++)
                    for (int j = 0; j < lengths[1]; j++)
                        for (int k = 0; k < lengths[2]; k++)
                            for (int l = 0; l < lengths[3]; l++)
                                a4[i, j, k, l] = ReadNullableElement<T>($"{path}@{i},{j},{k},{l}", options);
                return a4;
            default:
                var array = Array.CreateInstance(typeof(T), lengths);
                var coordinates = new int[rank];
                for (int index = 0; index < count; index++)
                {
                    array.SetValue(ReadNullableElement<T>(path + "@" + string.Join(',', coordinates), options), coordinates);
                    for (int dimension = rank - 1; dimension >= 0; dimension--)
                    {
                        if (++coordinates[dimension] < lengths[dimension]) break;
                        coordinates[dimension] = 0;
                    }
                }
                return array;
        }
    }

    /// <summary>
    /// Selects the established rank-specific public wrapper while preserving the nullable CLR element.<br/>
    /// Wrapper constructors continue using the owning reader's configured options.<br/>
    /// </summary>
    private IInhetoObject ReadNullableArrayWrapper(NameHeaderEntry entry, DeserializationOptions options)
    {
        var arrayType = NullableArrayType(entry);
        var factory = nullableArrayWrappers.GetOrAdd(arrayType, static type =>
            typeof(InhetoBinary).GetMethod(nameof(CreateNullableArrayWrapper), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(type.GetElementType()!)
                .CreateDelegate<Func<InhetoBinary, NameHeaderEntry, IInhetoObject>>());
        return factory(this, entry);
    }

    /// <summary>
    /// Constructs the existing strongly typed rank-one through rank-four wrapper or general higher-rank wrapper.<br/>
    /// This does not add a replacement wrapper API or retain separate descriptor ownership.<br/>
    /// </summary>
    private IInhetoObject CreateNullableArrayWrapper<T>(NameHeaderEntry entry) => entry.Type.ArrayInfo.Rank switch
    {
        1 => new InhetoArray1<T>(this, entry),
        2 => new InhetoArray2<T>(this, entry),
        3 => new InhetoArray3<T>(this, entry),
        4 => new InhetoArray4<T>(this, entry),
        _ => new InhetoArray<T>(this, entry)
    };

    /// <summary>
    /// Reads an existing array or complex collection child, distinguishing a missing node from an encoded null.<br/>
    /// Matching primitive nodes use cached native nullable-return readers without per-value coercion boxing.<br/>
    /// Enum reconstruction and complex structs use the declared nullable underlying type; custom nodes
    /// retain the normal registered type/property-path deserializer dispatch.<br/>
    /// Collection-only numeric and Rune text parsing follows the existing non-nullable collection parsers; arrays keep their prior coercion policy.<br/>
    /// </summary>
    /// <param name="path">Requested child path, including aliases and reference ancestry.<br/></param>
    /// <param name="options">Active operation's deserialization options and hooks.<br/></param>
    /// <param name="collectionText">Enables numeric and Rune text parser parity only for collection callers.<br/></param>
    private T ReadNullableElement<T>(string path, DeserializationOptions options, bool collectionText = false)
    {
        var entry = GetNameEntryFromPropPath(path, options, out var resolved, cacheResolution: false);
        if (entry.IsEmpty) throw new InvalidDataException($"Missing collection or array element at '{path}'.");
        if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType == InhetoBaseTypes.Null)
            return default!;
        if (NullableElementReader<T>.Native is { } native &&
            entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType == NullableElementReader<T>.BaseType)
            return native(this, resolved);
        if (NullableElementReader<T>.Underlying is Type underlying)
        {
            if (collectionText && NullableElementReader<T>.BaseType is
                InhetoBaseTypes.Byte or InhetoBaseTypes.SByte or InhetoBaseTypes.Int16 or InhetoBaseTypes.UInt16 or
                InhetoBaseTypes.Int32 or InhetoBaseTypes.UInt32 or InhetoBaseTypes.Int64 or InhetoBaseTypes.UInt64 or
                InhetoBaseTypes.Int128 or InhetoBaseTypes.UInt128 or InhetoBaseTypes.Half or InhetoBaseTypes.Single or
                InhetoBaseTypes.Double or InhetoBaseTypes.Decimal or InhetoBaseTypes.Rune)
            {
                // Inspect referenced metadata without changing the entry/path used for custom materialization below.
                var textEntry = entry;
                while (textEntry.Kind == InhetoKindMasks.InhetoBaseType &&
                    textEntry.Type.BaseType is InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
                    textEntry = NameHeaderEntry.CreateBorrowed(stream, textEntry.ValueOffset);
                if (textEntry.Kind == InhetoKindMasks.InhetoBaseType && textEntry.Type.BaseType == InhetoBaseTypes.String)
                {
                    string text = ReadStringCore(resolved) ?? throw new InvalidDataException($"Missing {(NullableElementReader<T>.BaseType == InhetoBaseTypes.Rune ? "Rune" : "numeric")} collection text at '{path}'.");
                    // Parse at read time: do not cache culture in the closed generic metadata.
                    return NullableElementReader<T>.BaseType switch
                    {
                        InhetoBaseTypes.Byte => (T)(object)byte.Parse(text),
                        InhetoBaseTypes.SByte => (T)(object)sbyte.Parse(text),
                        InhetoBaseTypes.Int16 => (T)(object)short.Parse(text),
                        InhetoBaseTypes.UInt16 => (T)(object)ushort.Parse(text),
                        InhetoBaseTypes.Int32 => (T)(object)int.Parse(text),
                        InhetoBaseTypes.UInt32 => (T)(object)uint.Parse(text),
                        InhetoBaseTypes.Int64 => (T)(object)long.Parse(text),
                        InhetoBaseTypes.UInt64 => (T)(object)ulong.Parse(text),
                        InhetoBaseTypes.Int128 => (T)(object)Int128.Parse(text),
                        InhetoBaseTypes.UInt128 => (T)(object)UInt128.Parse(text),
                        InhetoBaseTypes.Half => (T)(object)Half.Parse(text),
                        InhetoBaseTypes.Single => (T)(object)float.Parse(text),
                        InhetoBaseTypes.Double => (T)(object)double.Parse(text),
                        InhetoBaseTypes.Decimal => (T)(object)decimal.Parse(text),
                        InhetoBaseTypes.Rune => (T)(object)System.Text.Rune.GetRuneAt(text, 0),
                        _ => throw new InvalidOperationException("Unsupported numeric collection parser marker.")
                    };
                }
            }
            if (underlying.IsEnum)
            {
                var value = ReadPropInternal(resolved, options);
                return value is T typed ? typed : value is null ? default! : (T)Enum.ToObject(underlying, value);
            }
            if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType == InhetoBaseTypes.ComplexType)
                return (T)ReadMaterializedAs(underlying, resolved, options)!;
        }
        return ReadMaterializedAs<T>(default, entry, resolved, options, true, path)!;
    }

    /// <summary>
    /// Holds per-closed-element metadata and an optional native nullable-return reader.<br/>
    /// Discovery is once per CLR element type; unsupported/custom types keep the normal dispatch path.<br/>
    /// </summary>
    private static class NullableElementReader<T>
    {
        internal static readonly Type? Underlying = Nullable.GetUnderlyingType(typeof(T));
        internal static readonly InhetoBaseTypes BaseType = Underlying is not null && InhetoBaseTypesMap.TryGetValue(Underlying.TypeHandle, out var marker)
            ? marker : InhetoBaseTypes.Unknown;
        internal static readonly Func<InhetoBinary, string, T>? Native = CreateNative();

        /// <summary>
        /// Binds only an exact existing nullable-return scalar reader; never compiles a per-element conversion.<br/>
        /// </summary>
        private static Func<InhetoBinary, string, T>? CreateNative()
        {
            if (BaseType == InhetoBaseTypes.Unknown) return null;
            var method = typeof(InhetoBinary).GetMethod("Read" + BaseType, [typeof(string)]);
            return method?.ReturnType == typeof(T) ? method.CreateDelegate<Func<InhetoBinary, string, T>>() : null;
        }
    }
}
