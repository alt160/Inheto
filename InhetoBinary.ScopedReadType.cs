namespace Inheto;

public partial class InhetoBinary
{
    /// <summary>
    /// Reads a serialized CLR Type while reclaiming temporary native descriptors, including MD, at the end of this call.<br/>
    /// Preserves missing/null, duplicate, cache and exact-path behavior; missing required type-name payloads throw with their path.<br/>
    /// The returned Type has no dependency on native header storage. No developer disposal or reader-lifetime owner is introduced.<br/>
    /// </summary>
    /// <param name="propPath">Exact physical property path, including the empty root path.<br/></param>
    /// <returns>The resolved CLR Type or the existing null result when absent/unresolved.<br/></returns>
    private Type? ReadTypeCore(string propPath)
    {
        NameHeaderAllocationGroup group = default;
        return TryReadTypeScoped(propPath, ref group, out Type? result) ? result : ReadTypeLegacy(propPath);
    }

    /// <summary>
    /// Executes exactly one ReadType operation with explicit local native ownership, releasing on return or exception.<br/>
    /// Only a CLR Type escapes. Numeric path-cache entries and cached Type values contain no scoped descriptors.<br/>
    /// Type resolution may invoke reentrant assembly-resolution callbacks; each nested ReadType gets an independent group.<br/>
    /// </summary>
    /// <param name="propPath">Existing exact physical path, including literal dictionary keys.<br/></param>
    /// <param name="group">Fresh sole owner; optional counters/failure policy are exposed internally for acceptance tests.<br/></param>
    /// <param name="result">Independent CLR Type, or null for the existing missing/null behavior.<br/></param>
    /// <returns>True when handled; the retained false-result compatibility route releases grouped storage before legacy handling.<br/></returns>
    internal bool TryReadTypeScoped(string propPath, ref NameHeaderAllocationGroup group, out Type? result)
    {
        result = null;
        try
        {
            if (!TryGetScalarNavigation(GetPreparedPath(propPath), ref group, out var entry)) return false;
            if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType is InhetoBaseTypes.Unknown or InhetoBaseTypes.Null) return true;
            if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType is InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
                if (!NameHeaderEntry.TryCreateScoped(stream, entry.ValueOffset, entry.Name, ref group, out entry)) return false;
            uint valueOffset = entry.ValueOffset;
            result = (Type?)GetOrAddCached(entry.Index, k =>
            {
                stream.Position = valueOffset;
                return Type.GetType(stream.ReadString() ?? throw new InvalidDataException($"Missing Type payload at '{propPath}'."));
            });
            return true;
        }
        finally { group.Release(); }
    }

    /// <summary>
    /// Resolves navigation exclusively for ReadInt64, ReadString and ReadType; never returns a general-purpose shape descriptor.<br/>
    /// Cache/compiled/root routes retain scoped rich parsing; exact-path fallback validates types without constructing native children.<br/>
    /// Parent-path calculation already uses SkipType, so it creates no independent recursive descriptors.<br/>
    /// Cache publication retains numeric indices only; no entry, winning candidate or factory captures group ownership.<br/>
    /// </summary>
    /// <param name="path">Prepared or transient exact-path plan.<br/></param>
    /// <param name="group">Owner covering the retained rich root, cached/compiled entries and subsequent duplicate replacement.<br/></param>
    /// <param name="entry">Ephemeral scalar projection: only navigation fields, Kind and base-type flags are guaranteed; never use recursive type getters or expose this as a full descriptor.<br/></param>
    /// <returns>True after resolution, including a missing entry; parser exceptions retain the existing resolver behavior.<br/></returns>
    private bool TryGetScalarNavigation(PreparedInhetoPath path, ref NameHeaderAllocationGroup group, out NameHeaderEntry entry)
    {
        entry = default;
        if (TryGetResolvedNameEntry(path, out uint index))
            return index == 0 || NameHeaderEntry.TryCreateScoped(stream, index, null, ref group, out entry);
        if (TryFindCompiledNameEntry(path, out index))
        {
            if (index != 0 && !NameHeaderEntry.TryCreateScoped(stream, index, null, ref group, out entry)) return false;
            CacheResolvedNameEntry(path, index);
            return true;
        }
        if (!NameHeaderEntry.TryCreateScoped(stream, 1, null, ref group, out entry)) return false;
        if (path.Path.Length == 0) { CacheResolvedNameEntry(path, entry.Index); return true; }
        while (entry.Type.BaseType != InhetoBaseTypes.EndOfNamesHeader)
        {
            if (entry.IsEmpty || entry.NextEntryIndex == 0) break;
            if (!NameHeaderEntry.TryCreateScalarNavigation(stream, entry.NextEntryIndex, out var next)) return false;
            if (next.IsEmpty) break;
            string candidate = next.PropPath;
            CacheResolvedNameEntry(candidate, next);
            if (candidate == path.Path) { entry = next; return true; }
            entry = next;
        }
        if (TryFindReferencedNameEntry(path.Path, out uint referenceIndex))
        {
            CacheResolvedNameEntry(path, referenceIndex);
            return NameHeaderEntry.TryCreateScoped(stream, referenceIndex, null, ref group, out entry);
        }
        CacheResolvedNameEntry(path, 0); entry = default; return true;
    }
}
