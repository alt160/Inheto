using System.Reflection;
using System.Runtime.CompilerServices;

namespace Inheto;

public partial class InhetoBinary
{
    private static readonly ConditionalWeakTable<Type, Func<InhetoBinary, NameHeaderEntry, string, DeserializationOptions, Array?, Array>> complexArrayReaders = new();

    /// <summary>
    /// Selects only typed complex-element array projections; existing scalar, nullable and agnostic object-array routes remain unchanged.<br/>
    /// Wire classification rejects other arrays before inspecting the destination leaf. No objects or dimensions are materialized here.<br/>
    /// </summary>
    /// <param name="arrayType">Requested CLR array type, including supported jagged arrays.<br/></param>
    /// <param name="entry">Resolved original header rather than an unresolved duplicate marker.<br/></param>
    /// <param name="members">Current type-classification policy.<br/></param>
    /// <returns>True when indexed children must construct the requested complex CLR element shape.<br/></returns>
    private static bool IsComplexArrayShape(Type arrayType, in NameHeaderEntry entry, MemberTypesEnum members)
    {
        if (!arrayType.IsArray || entry.Kind != InhetoKindMasks.InhetoPromotedType ||
            entry.Type.PromotedType is not (InhetoPromotedTypes.Array or InhetoPromotedTypes.ArrayMD) ||
            entry.Type.ArrayInfo.ElementType.Kind != InhetoKindMasks.InhetoBaseType ||
            entry.Type.ArrayInfo.ElementType.BaseType != InhetoBaseTypes.ComplexType) return false;
        Type leaf = arrayType.GetElementType()!;
        while (leaf.IsArray) leaf = leaf.GetElementType()!;
        return leaf != typeof(object) && Nullable.GetUnderlyingType(leaf) is null &&
            InhetoTypeResolver.GetInhetoTypeInfo(leaf, members).IsComplex;
    }

    /// <summary>
    /// Reuses the existing operation reference map for typed complex arrays, without passing their output through array cloning.<br/>
    /// A supplied root destination is filled rather than replaced. Otherwise an existing compatible reference wins.<br/>
    /// Binds a typed loop once per CLR array type; the weak key does not independently retain collectible destination types.<br/>
    /// </summary>
    /// <param name="arrayType">Exact requested destination array shape.<br/></param>
    /// <param name="entry">Resolved array header whose index identifies the operation-local reference.<br/></param>
    /// <param name="path">Resolved array path used to address persisted children.<br/></param>
    /// <param name="options">Current activators and codecs; never captured by the static dispatch cache.<br/></param>
    /// <param name="destination">Caller-owned root array, or null when creating/replacing an array member.<br/></param>
    /// <returns>The exact constructed, supplied or already registered array, never a clone.<br/></returns>
    private Array ReadComplexArray(Type arrayType, NameHeaderEntry entry, string path, DeserializationOptions options, Array? destination = null)
    {
        if (destination is null && refCache is not null && refCache.TryGetValue(entry.Index, out var previous) && arrayType.IsInstanceOfType(previous))
            return (Array)previous;
        int depth = 0;
        for (Type type = arrayType; type.IsArray; type = type.GetElementType()!) depth++;
        if (arrayType.GetArrayRank() != entry.Type.ArrayInfo.Rank || depth != entry.Type.ArrayInfo.Depth ||
            (arrayType.GetArrayRank() == 1 && !arrayType.IsSZArray))
            throw new InvalidDataException($"Complex array rank/depth mismatch at '{path}'.");
        var read = complexArrayReaders.GetValue(arrayType, static type =>
            typeof(InhetoBinary).GetMethod(nameof(ReadComplexArrayElements), BindingFlags.Instance | BindingFlags.NonPublic)!
                .MakeGenericMethod(type.GetElementType()!)
                .CreateDelegate<Func<InhetoBinary, NameHeaderEntry, string, DeserializationOptions, Array?, Array>>());
        return read(this, entry, path, options, destination);
    }

    /// <summary>
    /// Constructs or fills an indexed complex array using direct typed writes through rank four.<br/>
    /// Registers the array before reading any child so existing reference-marker resolution closes cycles and aliases.<br/>
    /// Ranks one through four need no managed dimension/index array; higher ranks retain one lengths array and one reusable coordinate array.<br/>
    /// Shape validation precedes allocation or destination mutation. Failure restores the previous array reference-map entry.<br/>
    /// </summary>
    /// <typeparam name="T">Declared immediate CLR element type, including a nested typed array.<br/></typeparam>
    /// <param name="entry">Resolved header carrying count and borrowed dimensions.<br/></param>
    /// <param name="path">Existing indexed property-path prefix.<br/></param>
    /// <param name="options">Current materialization policy and hooks.<br/></param>
    /// <param name="destination">Exact caller-owned array to fill, or null to construct one.<br/></param>
    /// <returns>The registered array with its persisted elements applied.<br/></returns>
    private Array ReadComplexArrayElements<T>(NameHeaderEntry entry, string path, DeserializationOptions options, Array? destination)
    {
        int rank = entry.Type.ArrayInfo.Rank;
        int count = checked((int)entry.ValueOffset);
        int[]? higherLengths = rank > 4 ? new int[rank] : null;
        Span<int> lengths = higherLengths is null ? stackalloc int[4] : higherLengths;
        lengths = lengths[..rank];
        if (rank == 1) lengths[0] = count;
        else if (entry.Type.ArrayInfo.ArrayType != InhetoArrayTypes.ArrayMD || entry.CopyLengths(lengths) != rank)
            throw new InvalidDataException($"Missing complex array dimensions at '{path}'.");
        long product = lengths.Contains(0) ? 0 : 1;
        foreach (int length in lengths)
        {
            if (length < 0) throw new InvalidDataException($"Negative complex array length at '{path}'.");
            if (product != 0) product = checked(product * length);
        }
        if (product != count) throw new InvalidDataException($"Complex array shape/count mismatch at '{path}'.");
        if (destination is not null)
        {
            if (destination.GetType().GetElementType() != typeof(T) || destination.Rank != rank)
                throw new InvalidDataException($"Supplied complex array type/rank mismatch at '{path}'.");
            for (int dimension = 0; dimension < rank; dimension++)
                if (destination.GetLength(dimension) != lengths[dimension] || destination.GetLowerBound(dimension) != 0)
                    throw new InvalidDataException($"Supplied complex array dimensions mismatch at '{path}'.");
        }
        Array values = destination ?? (rank switch
        {
            1 => new T[count],
            2 => new T[lengths[0], lengths[1]],
            3 => new T[lengths[0], lengths[1], lengths[2]],
            4 => new T[lengths[0], lengths[1], lengths[2], lengths[3]],
            _ => Array.CreateInstance(typeof(T), higherLengths!)
        });
        var references = RefCache;
        bool hadPrevious = references.TryGetValue(entry.Index, out var previous);
        references[entry.Index] = values;
        try
        {
            switch (rank)
            {
                case 1:
                    var a1 = (T[])values;
                    for (int i = 0; i < a1.Length; i++) a1[i] = ReadComplexArrayElement(a1[i], $"{path}#{i}", options);
                    break;
                case 2:
                    var a2 = (T[,])values;
                    for (int i = 0; i < lengths[0]; i++)
                        for (int j = 0; j < lengths[1]; j++) a2[i, j] = ReadComplexArrayElement(a2[i, j], $"{path}@{i},{j}", options);
                    break;
                case 3:
                    var a3 = (T[,,])values;
                    for (int i = 0; i < lengths[0]; i++)
                        for (int j = 0; j < lengths[1]; j++)
                            for (int k = 0; k < lengths[2]; k++) a3[i, j, k] = ReadComplexArrayElement(a3[i, j, k], $"{path}@{i},{j},{k}", options);
                    break;
                case 4:
                    var a4 = (T[,,,])values;
                    for (int i = 0; i < lengths[0]; i++)
                        for (int j = 0; j < lengths[1]; j++)
                            for (int k = 0; k < lengths[2]; k++)
                                for (int l = 0; l < lengths[3]; l++) a4[i, j, k, l] = ReadComplexArrayElement(a4[i, j, k, l], $"{path}@{i},{j},{k},{l}", options);
                    break;
                default:
                    var indices = new int[rank];
                    for (int index = 0; index < count; index++)
                    {
                        T existing = destination is null ? default! : (T)values.GetValue(indices)!;
                        values.SetValue(ReadComplexArrayElement(existing, path + "@" + string.Join(',', indices), options), indices);
                        for (int dimension = rank - 1; dimension >= 0; dimension--)
                        {
                            if (++indices[dimension] < lengths[dimension]) break;
                            indices[dimension] = 0;
                        }
                    }
                    break;
            }
            return values;
        }
        catch
        {
            if (hadPrevious) references[entry.Index] = previous!;
            else references.Remove(entry.Index);
            throw;
        }
    }

    /// <summary>
    /// Applies one complete persisted child through existing typed activation, custom-hook and reference resolution.<br/>
    /// Supplied ordinary elements may be filled in place; duplicate markers resolve through the registered original instance.<br/>
    /// Explicit null occupies its slot; a missing indexed child is invalid rather than silently omitted.<br/>
    /// </summary>
    /// <typeparam name="T">Exact requested element type.<br/></typeparam>
    /// <param name="existing">Caller-owned element when filling, otherwise default.<br/></param>
    /// <param name="path">Complete indexed child path; passed through to property-path hooks.<br/></param>
    /// <param name="options">Current materialization options.<br/></param>
    /// <returns>The constructed, filled, custom-decoded, shared or null element.<br/></returns>
    private T ReadComplexArrayElement<T>(T existing, string path, DeserializationOptions options)
    {
        var entry = GetNameEntryFromPropPath(path, options, out string resolved, cacheResolution: false);
        if (entry.IsEmpty) throw new InvalidDataException($"Missing complex array element at '{path}'.");
        if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType == InhetoBaseTypes.Null) return default!;
        return ReadMaterializedAs(existing, entry, resolved, options, true, path)!;
    }
}
