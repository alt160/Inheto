namespace Inheto
{
    public partial class InhetoBinary
    {
        /// <summary>
        /// Tries to retrieve one payload-local prepared-path resolution without reading the serialized names header.<br/>
        /// The first cached path is packed into one 64-bit field; multiple paths use a token-to-entry-index dictionary after promotion.<br/>
        /// Entry index zero is a valid cached missing result, while token zero identifies a transient path that is never admitted.<br/>
        /// A physical dictionary-fill anchor bypasses global path resolutions so shadowed entries keep their own identity.<br/>
        /// </summary>
        /// <param name="path">Shared prepared physical-path plan.<br/></param>
        /// <param name="entryIndex">Resolved names-header byte offset, or zero for a cached missing path.<br/></param>
        /// <returns><see langword="true"/> when this payload already resolved the token; otherwise <see langword="false"/>.<br/></returns>
        private bool TryGetResolvedNameEntry(PreparedInhetoPath path, out uint entryIndex)
        {
            uint token = path.Token;
            if (token == 0 || literalReadAnchorIndex != 0)
            {
                entryIndex = 0;
                return false;
            }

            ulong single = _resolvedNameEntry;
            if ((uint)(single >> 32) == token)
            {
                entryIndex = (uint)single;
                return true;
            }

            if (_resolvedNameEntries is not null &&
                _resolvedNameEntries.TryGetValue(token, out entryIndex))
            {
                return true;
            }

            entryIndex = 0;
            return false;
        }

        /// <summary>
        /// Resolves a stable string path to its shared token before checking the payload-local entry-index cache.<br/>
        /// Dynamic collection paths remain transient and therefore always return a cache miss unless explicitly prepared elsewhere.<br/>
        /// </summary>
        /// <param name="propPath">Exact serialized property path.<br/></param>
        /// <param name="entry">Borrowed resolved entry, or the cached empty result for a missing path.<br/></param>
        /// <returns><see langword="true"/> when a stable token already has a payload-local result; otherwise <see langword="false"/>.<br/></returns>
        private bool TryGetResolvedNameEntry(string propPath, out NameHeaderEntry entry)
        {
            PreparedInhetoPath path = GetPreparedPath(propPath);
            if (TryGetResolvedNameEntry(path, out uint entryIndex))
            {
                entry = entryIndex == 0 ? default : NameHeaderEntry.CreateBorrowed(stream, entryIndex);
                return true;
            }

            entry = default;
            return false;
        }

        /// <summary>
        /// Resolves one exact path to its payload-local entry index without constructing a rich <see cref="NameHeaderEntry"/>.<br/>
        /// Cache hits are two integer comparisons in the packed lane; cache misses use the allocation-free compiled scanner and publish the resulting index or confirmed zero miss.<br/>
        /// </summary>
        /// <param name="propPath">Exact serialized property path.<br/></param>
        /// <param name="entryIndex">Resolved names-header byte offset, or zero for a confirmed missing path.<br/></param>
        /// <returns><see langword="true"/> when the prepared scanner handled the payload; otherwise <see langword="false"/> so malformed or unsupported bytes can use compatibility resolution.<br/></returns>
        private bool TryResolvePreparedEntryIndex(string propPath, out uint entryIndex)
        {
            PreparedInhetoPath path = GetPreparedPath(propPath);
            if (TryGetResolvedNameEntry(path, out entryIndex))
                return true;
            if (!TryFindCompiledNameEntry(path, out entryIndex))
                return false;
            CacheResolvedNameEntry(path, entryIndex);
            return true;
        }

        /// <summary>
        /// Caches one exact prepared-path resolution for the lifetime of this runtime payload wrapper.<br/>
        /// Only the numeric token and payload-specific entry index are retained; path text, CLR types, and full entry structs remain outside the per-payload cache.<br/>
        /// Anchored dictionary-fill resolutions are context-specific and must never enter the global-path cache.<br/>
        /// </summary>
        /// <param name="path">Stable prepared path; token-zero transient paths are ignored.<br/></param>
        /// <param name="entryIndex">Resolved names-header byte offset, or zero for a confirmed missing path.<br/></param>
        private void CacheResolvedNameEntry(PreparedInhetoPath path, uint entryIndex)
        {
            uint token = path.Token;
            if (_nameEntryCacheSuppressionDepth != 0 || token == 0 || literalReadAnchorIndex != 0)
                return;

            ulong packed = ((ulong)token << 32) | entryIndex;
            if (_resolvedNameEntry == 0)
            {
                _resolvedNameEntry = packed;
                return;
            }

            uint singleToken = (uint)(_resolvedNameEntry >> 32);
            if (singleToken == token)
                return;

            if (_resolvedNameEntries is null)
            {
                _resolvedNameEntries = new Dictionary<uint, uint>(2)
                {
                    [singleToken] = (uint)_resolvedNameEntry,
                    [token] = entryIndex
                };
                return;
            }

            _resolvedNameEntries.TryAdd(token, entryIndex);
        }

        /// <summary>
        /// Resolves a stable string path to its shared token before admitting its payload-specific entry index.<br/>
        /// Synthesized dynamic paths are intentionally ignored so record contents cannot grow the process registry or each payload cache without bound.<br/>
        /// </summary>
        /// <param name="propPath">Exact serialized property path.<br/></param>
        /// <param name="entry">Resolved entry or an empty entry representing a confirmed missing path.<br/></param>
        private void CacheResolvedNameEntry(string propPath, NameHeaderEntry entry)
        {
            PreparedInhetoPath path = GetPreparedPath(propPath);
            CacheResolvedNameEntry(path, entry.IsEmpty ? 0 : entry.Index);
        }

        /// <summary>
        /// Copies the payload-local token cache into a shallow clone without forcing unused cache state into existence.<br/>
        /// The packed first entry is copied by value, while a promoted dictionary is duplicated so clone mutations remain detached.<br/>
        /// </summary>
        /// <param name="clone">New runtime wrapper over the same immutable payload bytes.<br/></param>
        private void CloneResolvedNameEntryCacheTo(InhetoBinary clone)
        {
            clone._resolvedNameEntry = _resolvedNameEntry;
            if (_resolvedNameEntries is not null)
                clone._resolvedNameEntries = new Dictionary<uint, uint>(_resolvedNameEntries);
        }
    }
}
