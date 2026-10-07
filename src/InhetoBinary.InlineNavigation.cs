namespace Inheto;
public partial class InhetoBinary
{
    /// <summary>
    /// Restricted navigation for ten audited inline scalar readers, never a full descriptor or materializer resolver.<br/>
    /// Preserves the original prepared/resolved/compiled/fallback order and numeric-only cache publication.<br/>
    /// Root, cache and fallback entries all validate grammar without native construction; no group/finally or winning-header reparse is required.<br/>
    /// ReadInt32 keeps its existing compiled scalar fast path before this resolver is called.<br/>
    /// </summary>
    /// <param name="propPath">Exact serialized path, including the existing null/empty root alias and literal dictionary keys.<br/></param>
    /// <returns>Incomplete navigation/base/enum projection or default when missing; never use recursive getters on this result.<br/></returns>
    private NameHeaderEntry GetInlineNavigation(string propPath)
    {
        var preparedPath = GetPreparedPath(propPath);
        propPath = preparedPath.Path;
        if (TryGetResolvedNameEntry(preparedPath, out uint index))
            return index == 0 ? default : NameHeaderEntry.CreateInlineNavigation(stream, index);
        if (TryFindCompiledNameEntry(preparedPath, out index))
        {
            var compiled = index == 0 ? default : NameHeaderEntry.CreateInlineNavigation(stream, index);
            CacheResolvedNameEntry(preparedPath, index);
            return compiled;
        }
        var entry = NameHeaderEntry.CreateInlineNavigation(stream, 1);
        if (propPath.Length == 0) { CacheResolvedNameEntry(preparedPath, entry.Index); return entry; }
        while (entry.Type.BaseType != InhetoBaseTypes.EndOfNamesHeader)
        {
            if (entry.IsEmpty || entry.NextEntryIndex == 0) break;
            var next = NameHeaderEntry.CreateInlineNavigation(stream, entry.NextEntryIndex);
            if (next.IsEmpty) break;
            string path = next.PropPath;
            CacheResolvedNameEntry(path, next);
            if (path == propPath) return next;
            entry = next;
        }
        if (TryFindReferencedNameEntry(propPath, out uint referenceIndex))
        {
            CacheResolvedNameEntry(preparedPath, referenceIndex);
            return NameHeaderEntry.CreateInlineNavigation(stream, referenceIndex);
        }
        CacheResolvedNameEntry(preparedPath, 0);
        return default;
    }
}
