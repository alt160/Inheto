namespace Inheto;

public partial class InhetoBinary
{
    /// <summary>
    /// Compact lookup for the audited Half and DateOnly inline readers.<br/>
    /// Keeps resolution order, numeric cache publication and literal-key behavior; known offsets use the same validating span primitives without an NHE.<br/>
    /// Forward fallback keeps the existing navigation representation local; only a nullable 32-bit slot crosses this return boundary.<br/>
    /// Unknown/null base-kind headers and missing entries produce absence; zero and every other slot bit pattern remain valid values.<br/>
    /// Does not dereference duplicate markers, coerce requested types, construct native metadata, or materialize containers.<br/>
    /// </summary>
    /// <param name="propPath">Logical path, retaining the existing null/empty root alias and borrowed-payload lifetime.<br/></param>
    /// <returns>The original four-byte slot, or null under precisely the two readers' original missing/unknown/null rules.<br/></returns>
    private uint? GetInlineValue(string propPath)
    {
        var preparedPath = GetPreparedPath(propPath);
        propPath = preparedPath.Path;
        NameHeaderEntry entry;
        if (TryGetResolvedNameEntry(preparedPath, out uint index))
        {
            return index == 0 ? null : ReadInlineValue(stream, index);
        }
        if (TryFindCompiledNameEntry(preparedPath, out index))
        {
            uint? value = index == 0 ? null : ReadInlineValue(stream, index);
            CacheResolvedNameEntry(preparedPath, index);
            return value;
        }
        if (propPath.Length == 0)
        {
            uint? value = ReadInlineValue(stream, 1);
            CacheResolvedNameEntry(preparedPath, 1);
            return value;
        }
        entry = NameHeaderEntry.CreateInlineNavigation(stream, 1);
        while (entry.Type.BaseType != InhetoBaseTypes.EndOfNamesHeader)
        {
            if (entry.IsEmpty || entry.NextEntryIndex == 0) break;
            var next = NameHeaderEntry.CreateInlineNavigation(stream, entry.NextEntryIndex);
            if (next.IsEmpty) break;
            string path = next.PropPath;
            CacheResolvedNameEntry(path, next);
            if (path == propPath) { entry = next; goto Found; }
            entry = next;
        }
        // The physical scan missed; continue through references without rewriting the request.
        if (TryFindReferencedNameEntry(propPath, out uint referenceIndex))
        {
            CacheResolvedNameEntry(preparedPath, referenceIndex);
            return ReadInlineValue(stream, referenceIndex);
        }
        CacheResolvedNameEntry(preparedPath, 0);
        return null;

    Found:
        if (entry.Kind == InhetoKindMasks.InhetoBaseType &&
            (entry.Type.BaseType == InhetoBaseTypes.Unknown || entry.Type.BaseType == InhetoBaseTypes.Null)) return null;
        return entry.ValueOffset;
    }

    /// <summary>
    /// Reads a known header's inline slot without constructing or transporting a NameHeaderEntry.<br/>
    /// Consumes the parent, complete type grammar, decoded/interned name and four-byte slot in the original order.<br/>
    /// Name decoding and interning are deliberately retained for encoding callbacks and exception parity; no shared cursor is moved.<br/>
    /// SkipType retains enum-byte consumption and recursive/MD/comparer validation without native child storage.<br/>
    /// Only exact base-kind Unknown/Null markers produce absence, and only after the complete header has been validated.<br/>
    /// Other markers retain the existing readers' raw inline interpretation, including duplicates and unrelated shapes.<br/>
    /// </summary>
    /// <param name="buffer">Stable borrowed payload and encoding for the synchronous read.<br/></param>
    /// <param name="index">Exact logical header offset, validated by the existing span reader.<br/></param>
    /// <returns>The original 32 bits or absence under the Half/DateOnly contract; no managed references cross this boundary.<br/></returns>
    private static uint? ReadInlineValue(BufferStream buffer, uint index)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        var reader = new NameHeaderSpanReader(buffer.AsReadOnlySpan, checked((int)index), buffer.StringEncoding);
        _ = reader.Read7BitEncodedUInt();
        byte marker = reader.PeekByte();
        reader.SkipType();
        _ = string.Intern(reader.ReadString()!);
        uint value = reader.ReadUInt32();
        return marker is (byte)InhetoBaseTypes.Unknown or (byte)InhetoBaseTypes.Null ? null : value;
    }
}
