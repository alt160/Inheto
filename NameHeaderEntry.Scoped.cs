namespace Inheto;

internal partial struct NameHeaderEntry
{
    /// <summary>
    /// Parses one complete borrowed entry with explicit nonescaping native storage, without changing the shared stream position.<br/>
    /// MD dimensions remain numeric payload locators; all recursive children use the same allocation group.<br/>
    /// Other malformed input retains the established exceptions. The result must not outlive the allocation group.<br/>
    /// </summary>
    /// <param name="buffer">Borrowed payload stream.<br/></param>
    /// <param name="index">Original entry byte offset.<br/></param>
    /// <param name="name">Optional duplicate-entry name override, preserving the ordinary borrowed parser contract.<br/></param>
    /// <param name="group">Sole stack-local owner of all recursive slots produced by this operation.<br/></param>
    /// <param name="entry">Scoped entry on success; no independently owned native pointers are returned.<br/></param>
    /// <returns>True after a complete parse; malformed input throws and the owning operation releases partial storage.<br/></returns>
    internal static bool TryCreateScoped(BufferStream buffer, uint index, string? name, ref NameHeaderAllocationGroup group, out NameHeaderEntry entry)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        entry = default;
        var reader = new NameHeaderSpanReader(buffer.AsReadOnlySpan, checked((int)index), buffer.StringEncoding);
        entry.buffer = buffer; entry.bufferBorrowed = true; entry.Index = index;
        entry.ParentIndex = reader.Read7BitEncodedUInt();
        entry.Kind = (InhetoKindMasks)(reader.PeekByte() & 0xc0);
        if (!NameHeaderEntryType.TryReadScoped(ref reader, ref group, out entry.Type)) return false;
        entry.Name = string.Intern(reader.ReadString()!);
        entry.ValueOffset = reader.ReadUInt32();
        entry.NextEntryIndex = entry.Type.BaseType == InhetoBaseTypes.EndOfNamesHeader ? 0 : checked((uint)reader.Position);
        if (name is not null) entry.Name = string.Intern(name);
        return true;
    }

    /// <summary>
    /// Creates an ephemeral navigation projection exclusively for the private ReadInt64/ReadString/ReadType exact-path fallback.<br/>
    /// Validates and consumes the complete type grammar through SkipType without constructing native recursive children.<br/>
    /// Only navigation fields, Kind and base-type flags are valid; this is not a complete type descriptor and must not escape to shape or materialization consumers.<br/>
    /// Preserves name decoding/interning, parent reconstruction, value offset, end markers and shared stream cursor behavior.<br/>
    /// The selected header is not reparsed, so no additional encoding callback, retained owner or arbitrary-key cache is introduced.<br/>
    /// </summary>
    /// <param name="buffer">Stable borrowed payload for the synchronous scalar operation.<br/></param>
    /// <param name="index">Exact header byte offset.<br/></param>
    /// <param name="entry">Incomplete scalar navigation projection; recursive type getters must never be used on this result.<br/></param>
    /// <returns>True after a complete parse; existing primitive and array-validation exceptions propagate.<br/></returns>
    internal static bool TryCreateScalarNavigation(BufferStream buffer, uint index, out NameHeaderEntry entry)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        entry = default;
        var reader = new NameHeaderSpanReader(buffer.AsReadOnlySpan, checked((int)index), buffer.StringEncoding);
        entry.buffer = buffer; entry.bufferBorrowed = true; entry.Index = index;
        entry.ParentIndex = reader.Read7BitEncodedUInt();
        byte marker = reader.PeekByte();
        entry.Kind = (InhetoKindMasks)(marker & 0xc0);
        entry.Type.Kind = entry.Kind;
        if (entry.Kind == InhetoKindMasks.InhetoBaseType)
            entry.Type.BaseType = (InhetoBaseTypes)(marker & 0x3f);
        reader.SkipType();
        entry.Name = string.Intern(reader.ReadString()!);
        entry.ValueOffset = reader.ReadUInt32();
        entry.NextEntryIndex = entry.Type.BaseType == InhetoBaseTypes.EndOfNamesHeader ? 0 : checked((uint)reader.Position);
        return true;
    }
}

internal partial record struct NameHeaderEntryType
{
    /// <summary>
    /// Reads normalized descriptors using explicit group storage and original field/getter layout.<br/>
    /// MD retains rank and numeric dimension locators without embedding managed arrays in native storage.<br/>
    /// Terminal decoding uses the already classified marker directly; recursive construction retains grouped storage.<br/>
    /// Base, nullable, enum and unrecognized promoted markers retain the original parser's normalization and byte consumption.<br/>
    /// </summary>
    /// <param name="reader">Local encoded-type cursor.<br/></param>
    /// <param name="group">Owner that remains live through every consumer of the returned value.<br/></param>
    /// <param name="value">Borrowed scoped descriptor; cleared/default fields retain original normalization.<br/></param>
    /// <returns>True for a complete descriptor; malformed input throws through the established primitive readers.<br/></returns>
    internal static bool TryReadScoped(ref NameHeaderSpanReader reader, ref NameHeaderAllocationGroup group, out NameHeaderEntryType value)
    {
        value = default;
        byte marker = reader.PeekByte();
        value.Kind = (InhetoKindMasks)(marker & 0xc0);
        if (value.Kind == InhetoKindMasks.InhetoCollectionType)
            return NameHeaderCollectionEntry.TryReadScoped(ref reader, ref group, out value.CollectionInfo);
        if (value.Kind == InhetoKindMasks.InhetoPromotedType)
        {
            value.PromotedType = (InhetoPromotedTypes)(marker & 0x3f);
            switch (value.PromotedType)
            {
                case InhetoPromotedTypes.ArrayMD:
                    _ = reader.ReadByte();
                    return NameHeaderArrayEntry.TryReadMdScoped(ref reader, ref group, out value.ArrayInfo);
                case InhetoPromotedTypes.Array:
                    _ = reader.ReadByte();
                    return NameHeaderArrayEntry.TryReadScoped(ref reader, ref group, out value.ArrayInfo);
                case InhetoPromotedTypes.Enumerable:
                    _ = reader.ReadByte();
                    return NameHeaderEnumerableEntry.TryReadScoped(ref reader, ref group, out value.EnumerableInfo);
                case InhetoPromotedTypes.Enum:
                    _ = reader.ReadByte();
                    value.EnumType = (InhetoBaseTypes)(reader.ReadByte() & 0x3f);
                    return true;
            }
        }
        if (value.Kind == InhetoKindMasks.InhetoBaseType)
        {
            value.BaseType = (InhetoBaseTypes)(reader.ReadByte() & 0x3f);
            return true;
        }
        if (value.Kind == InhetoKindMasks.InhetoNullableBaseType)
        {
            value.IsNullable = true;
            value.NullableBaseType = (InhetoBaseTypes)(reader.ReadByte() & 0x3f);
            return true;
        }
        // Unrecognized promoted values consume only their marker in the unchanged borrowed grammar.
        _ = reader.ReadByte();
        return true;
    }
}

internal unsafe partial struct NameHeaderArrayEntry
{
    /// <summary>Constructs only an ordinary/jagged array descriptor; validates its child before blitting into group storage.<br/></summary>
    /// <param name="reader">Cursor immediately after the promoted Array marker.<br/></param>
    /// <param name="group">Explicit operation owner.<br/></param>
    /// <param name="value">Original-layout descriptor with no managed dimension array.<br/></param>
    /// <returns>True after constructing the complete array descriptor.<br/></returns>
    internal static bool TryReadScoped(ref NameHeaderSpanReader reader, ref NameHeaderAllocationGroup group, out NameHeaderArrayEntry value)
    {
        value = default; value.ArrayType = InhetoArrayTypes.Array; value.Rank = 1; value.Depth = reader.ReadByte();
        if (!NameHeaderEntryType.TryReadScoped(ref reader, ref group, out var child)) return false;
        ValidateElementType(in child);
        value.ElementTypePtr = (NameHeaderEntryType*)group.Store(in child);
        return true;
    }
}

internal unsafe partial struct NameHeaderEnumerableEntry
{
    /// <summary>Builds an enumerable child using the same explicit group, preserving the direct pointer getter.<br/></summary>
    /// <param name="reader">Cursor at the child descriptor.<br/></param>
    /// <param name="group">Explicit operation owner.<br/></param>
    /// <param name="value">Borrowed enumerable descriptor.<br/></param>
    /// <returns>True after constructing the complete enumerable descriptor.<br/></returns>
    internal static bool TryReadScoped(ref NameHeaderSpanReader reader, ref NameHeaderAllocationGroup group, out NameHeaderEnumerableEntry value)
    {
        value = default;
        if (!NameHeaderEntryType.TryReadScoped(ref reader, ref group, out var child)) return false;
        value.ElementTypePtr = (NameHeaderEntryType*)group.Store(in child);
        return true;
    }
}

internal unsafe partial struct NameHeaderCollectionEntry
{
    /// <summary>Preserves single-marker family/comparer/key/value grammar while grouping recursive children.<br/></summary>
    /// <param name="reader">Cursor at the collection marker.<br/></param>
    /// <param name="group">Explicit operation owner; includes both key and value construction.<br/></param>
    /// <param name="value">Borrowed original-layout collection descriptor.<br/></param>
    /// <returns>True after constructing both key and value subtrees.<br/></returns>
    internal static bool TryReadScoped(ref NameHeaderSpanReader reader, ref NameHeaderAllocationGroup group, out NameHeaderCollectionEntry value)
    {
        value = default;
        byte marker = reader.ReadByte(); value.CollectionType = (InhetoCollectionTypes)(marker & 0x1f);
        value.Comparer = (marker & 0x20) != 0 ? (InhetoComparerTypes)reader.ReadByte() : InhetoComparerTypes.None;
        if (value.CollectionType is InhetoCollectionTypes.Dictionary or InhetoCollectionTypes.ConcurrentDictionary
            or InhetoCollectionTypes.SortedDictionary or InhetoCollectionTypes.SortedList)
        {
            if (!NameHeaderEntryType.TryReadScoped(ref reader, ref group, out var key)) return false;
            value.KeyTypePtr = (NameHeaderEntryType*)group.Store(in key);
        }
        else if (value.CollectionType is InhetoCollectionTypes.StringKeyDictionary or InhetoCollectionTypes.StringKeyConcurrentDictionary
            or InhetoCollectionTypes.StringKeySortedDictionary or InhetoCollectionTypes.StringKeySortedList)
        {
            var key = NameHeaderEntryType.StringType;
            value.KeyTypePtr = (NameHeaderEntryType*)group.Store(in key);
        }
        if (!NameHeaderEntryType.TryReadScoped(ref reader, ref group, out var child)) return false;
        value.ValueTypePtr = (NameHeaderEntryType*)group.Store(in child);
        return true;
    }
}

internal unsafe partial struct NameHeaderArrayEntry
{
    /// <summary>
    /// Parses an MD descriptor into the original native layout using operation-owned recursive children.<br/>
    /// Dimension bytes are consumed exactly as in ReadBorrowed; only their numeric offset and extent are retained.<br/>
    /// No dimension array, native payload copy, new rank/count policy or ordinary-array hot-path branch is introduced.<br/>
    /// The caller must retain the matching payload and allocation group through every use of the descriptor.<br/>
    /// </summary>
    /// <param name="reader">Borrowed cursor immediately after the promoted MD marker.<br/></param>
    /// <param name="group">Sole operation owner, including children stored before malformed dimensions throw.<br/></param>
    /// <param name="value">Original-layout MD metadata with group-backed children and payload-relative locators.<br/></param>
    /// <returns>True for a complete descriptor; existing primitive-reader exceptions propagate on malformed input.<br/></returns>
    internal static bool TryReadMdScoped(ref NameHeaderSpanReader reader, ref NameHeaderAllocationGroup group, out NameHeaderArrayEntry value)
    {
        value = default;
        value.ArrayType = InhetoArrayTypes.ArrayMD;
        value.Rank = reader.ReadByte();
        value.Depth = 1;
        if (!NameHeaderEntryType.TryReadScoped(ref reader, ref group, out var child)) return false;
        ValidateElementType(in child);
        value.ElementTypePtr = (NameHeaderEntryType*)group.Store(in child);
        value.LengthsOffset = reader.Position;
        for (int dimension = 0; dimension < value.Rank; dimension++)
            _ = reader.Read7BitEncodedInt();
        value.LengthsBytes = reader.Position - value.LengthsOffset;
        return true;
    }
}
