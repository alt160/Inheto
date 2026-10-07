namespace Inheto;
internal partial struct NameHeaderEntry
{
    /// <summary>
    /// Restricted projection for audited inline scalar readers; consumes the complete existing grammar without native children.<br/>
    /// Only navigation fields, Kind, BaseType and EnumType are valid; recursive getters must never consume this projection.<br/>
    /// Enum underlying markers retain the original low-six-bit normalization; nullable and unrelated shapes retain default base/enum flags.<br/>
    /// Name decoding/interning, borrowed parent paths, value slot and shared cursor semantics remain unchanged.<br/>
    /// </summary>
    /// <param name="buffer">Stable borrowed payload for the synchronous inline read.<br/></param>
    /// <param name="index">Exact logical header offset.<br/></param>
    /// <returns>An ephemeral incomplete NHE, with no native storage to release.<br/></returns>
    internal static NameHeaderEntry CreateInlineNavigation(BufferStream buffer, uint index)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        NameHeaderEntry entry = default;
        var reader = new NameHeaderSpanReader(buffer.AsReadOnlySpan, checked((int)index), buffer.StringEncoding);
        entry.buffer = buffer; entry.bufferBorrowed = true; entry.Index = index;
        entry.ParentIndex = reader.Read7BitEncodedUInt();
        byte marker = reader.PeekByte();
        entry.Kind = (InhetoKindMasks)(marker & 0xc0); entry.Type.Kind = entry.Kind;
        if (entry.Kind == InhetoKindMasks.InhetoBaseType)
            entry.Type.BaseType = (InhetoBaseTypes)(marker & 0x3f);
        if (marker == 0x83)
        {
            _ = reader.ReadByte();
            entry.Type.EnumType = (InhetoBaseTypes)(reader.ReadByte() & 0x3f);
        }
        else reader.SkipType();
        entry.Name = string.Intern(reader.ReadString()!);
        entry.ValueOffset = reader.ReadUInt32();
        entry.NextEntryIndex = entry.Type.BaseType == InhetoBaseTypes.EndOfNamesHeader ? 0 : checked((uint)reader.Position);
        return entry;
    }
}
