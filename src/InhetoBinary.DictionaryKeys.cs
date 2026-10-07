using System.Collections;

namespace Inheto;

public partial class InhetoBinary
{
    /// <summary>
    /// Parses a formatted dictionary key using its active encoded family, never speculative parser fallbacks.<br/>
    /// A legacy ComplexType header may have lost the key classification while retaining formatted text.<br/>
    /// Only an explicit supported destination key type can supply that missing information; agnostic reads do not guess.<br/>
    /// Empty string keys remain valid; unsupported, unresolved and null keys fail before insertion.<br/>
    /// </summary>
    /// <param name="text">Exact key text after removing its single entry prefix.<br/></param>
    /// <param name="shape">Encoded dictionary key descriptor.<br/></param>
    /// <param name="targetType">Optional developer-selected key type for coercion and legacy recovery.<br/></param>
    /// <returns>A non-null key; no new cache, delegate or intermediate path is created.<br/></returns>
    internal static object ParseDictionaryKey(string text, in NameHeaderEntryType shape, Type? targetType = null)
    {
        object? key;
        switch (shape.Kind)
        {
            case InhetoKindMasks.InhetoBaseType when shape.BaseType == InhetoBaseTypes.ComplexType:
                var legacyType = targetType is null ? null : Nullable.GetUnderlyingType(targetType) ?? targetType;
                if (legacyType is null || !InhetoSerializer.IsSafeKeyType(legacyType))
                    throw new InvalidDataException("A formatted dictionary key has no encoded scalar type; a supported destination key type is required.");
                key = InhetoSerializer.InhetoBaseTypesMap.TryGetValue(legacyType.TypeHandle, out var baseType)
                    ? InhetoSerializer.StringToSafeKeyType(text, baseType)
                    : InhetoSerializer.StringToSafeKeyType(text, InhetoSerializer.InhetoPromotedTypesMap[legacyType.TypeHandle]);
                break;
            case InhetoKindMasks.InhetoBaseType:
                key = InhetoSerializer.StringToSafeKeyType(text, shape.BaseType);
                break;
            case InhetoKindMasks.InhetoNullableBaseType:
                key = InhetoSerializer.StringToSafeKeyType(text, shape.NullableBaseType);
                break;
            case InhetoKindMasks.InhetoPromotedType:
                key = InhetoSerializer.StringToSafeKeyType(text, shape.PromotedType);
                break;
            default:
                throw new InvalidDataException("Unsupported formatted dictionary key shape.");
        }
        if (key is null) throw new InvalidDataException("The dictionary key could not be resolved.");
        if (targetType is not null) key = FastCoerce.Coerce(key, targetType);
        return key ?? throw new InvalidDataException("Dictionary key conversion returned null.");
    }

    /// <summary>
    /// Selects the next direct dictionary entry, accepting existing formatted and numbered entry layouts.<br/>
    /// Ordinary dictionary properties are skipped, and child traversal remains with the existing header machinery.<br/>
    /// The result is borrowed metadata and must not outlive the current payload.<br/>
    /// </summary>
    /// <param name="entry">Parent on the first call, or the preceding entry on later calls.<br/></param>
    /// <param name="first">Whether to select a child rather than a sibling.<br/></param>
    /// <returns>The next entry or an empty header at the end.<br/></returns>
    internal static NameHeaderEntry NextDictionaryEntry(NameHeaderEntry entry, bool first)
    {
        var node = first ? entry.FirstChild(NameHeaderEntry.EntryTypes.Any) : entry.NextSibling(NameHeaderEntry.EntryTypes.Any);
        while (!node.IsEmpty && (node.Name.Length == 0 || node.Name[0] is not ('!' or '#')))
            node = node.NextSibling(NameHeaderEntry.EntryTypes.Any);
        return node;
    }

    /// <summary>
    /// Fills an already selected dictionary without replacing its identity, comparer or existing entries.<br/>
    /// Formatted entries use their encoded key family; numbered entries materialize their explicit Key/Value children and honor codecs.<br/>
    /// Legacy formatted entries with missing type metadata require the developer-selected destination key type.<br/>
    /// Missing entries and null keys throw rather than shortening the dictionary or fabricating keys; omitted numbered Value members retain existing null-value semantics.<br/>
    /// Existing scalar member loops remain separate; this shared non-generic path handles previously unsupported key shapes and complex values.<br/>
    /// </summary>
    /// <param name="destination">Existing destination dictionary, including caller-owned instances.<br/></param>
    /// <param name="keyType">Declared destination key type.<br/></param>
    /// <param name="valueType">Declared destination value type.<br/></param>
    /// <param name="entry">Resolved dictionary header.<br/></param>
    /// <param name="options">Current activation and codec policy.<br/></param>
    private void FillDictionaryEntries(IDictionary destination, Type keyType, Type valueType, NameHeaderEntry entry, DeserializationOptions options)
    {
        var node = NextDictionaryEntry(entry, true);
        var shape = entry.Type.CollectionInfo.KeyType;
        for (uint i = 0; i < entry.ValueOffset; i++)
        {
            if (node.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
            using var physicalValue = new LiteralValueReadScope(this, node.Index, node.PropPath);
            bool formatted = node.Name[0] == '!';
            object key;
            string valuePath;
            if (formatted)
            {
                key = ParseDictionaryKey(node.CleanName, in shape, keyType);
                valuePath = node.PropPath;
            }
            else
            {
                string keyPath = $"{node.PropPath}.Key";
                var keyEntry = GetNameEntryFromPropPath(keyPath);
                if (keyEntry.Kind == InhetoKindMasks.InhetoBaseType && keyEntry.Type.BaseType is InhetoBaseTypes.Unknown or InhetoBaseTypes.Null)
                    throw new InvalidDataException("Dictionary entry has a missing or null key.");
                key = ReadMaterializedAs(keyType, keyPath, options)
                    ?? throw new InvalidDataException("Dictionary entry has a missing or null key.");
                valuePath = $"{node.PropPath}.Value";
                // Ordinary null members are omitted, including DictionaryEntry.Value; absence means a null dictionary value here.
            }
            destination.Add(key, ReadDictionaryValue(valueType, valuePath, options));
            node = NextDictionaryEntry(node, false);
        }
        if (!node.IsEmpty) throw new InvalidDataException("Dictionary has more encoded entries than its declared count.");
    }
}
