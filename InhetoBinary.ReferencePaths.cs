using System.Buffers;
using System.Text;

namespace Inheto;

public partial class InhetoBinary
{
    private uint literalReadAnchorIndex;
    private string? literalReadAnchorPath;
    /// <summary>
    /// Resolves an exact logical path that crosses a persisted reference, without rewriting or tokenizing the path.<br/>
    /// Called only after physical lookup misses; ordinary scalar success retains its existing fast route.<br/>
    /// Terminal reference entries remain reference entries so each reader retains its own terminal duplicate policy.<br/>
    /// </summary>
    /// <param name="path">Requested logical path; names, including literal dictionary keys, are matched verbatim.<br/></param>
    /// <param name="index">Physical index of the reachable terminal header, or zero when no exact continuation exists.<br/></param>
    /// <returns>True only for a complete match reached through at least one reference.<br/></returns>
    private bool TryFindReferencedNameEntry(string path, out uint index)
    {
        if (TryFindReferencePrefix(path, out index, out int matched) && matched == path.Length)
            return true;
        index = 0;
        return false;
    }

    /// <summary>
    /// Walks physical name headers with a logical consumed-prefix cursor and follows references only while a suffix remains.<br/>
    /// Each successful continuation consumes characters, so valid object cycles permit arbitrarily long finite requests.<br/>
    /// A separate bounded hop count rejects malformed reference-only cycles that consume no path characters.<br/>
    /// Uses borrowed spans and scalar locals only: no native descriptors, path strings, token arrays, delegates, or runtime index.<br/>
    /// </summary>
    /// <param name="path">Complete unchanged request.<br/></param>
    /// <param name="index">Deepest reachable physical prefix after a reference was followed.<br/></param>
    /// <param name="matched">Number of request characters consumed by that prefix.<br/></param>
    /// <returns>True when a reference-aware prefix was reached; false leaves the existing physical compatibility route authoritative.<br/></returns>
    private bool TryFindReferencePrefix(string path, out uint index, out int matched)
    {
        index = 0;
        matched = 0;
        if (!TryResolveLiteralNameEntry(path, out var entry, out matched, out bool followed) || !followed)
            return false;
        index = entry.Index;
        return true;
    }

    /// <summary>
    /// Compares the complete physical path between an anchor and a descendant against the logical remainder.<br/>
    /// Two bounded parent walks avoid constructing a path; names are compared intact rather than split at marker characters.<br/>
    /// Strictly decreasing parent offsets are the existing names-header tree invariant and prevent malformed parent loops.<br/>
    /// </summary>
    /// <param name="payload">Borrowed serialized bytes.<br/></param>
    /// <param name="entry">Candidate descendant.<br/></param>
    /// <param name="anchor">Physical root of the current reference target subtree.<br/></param>
    /// <param name="path">Unconsumed logical path.<br/></param>
    /// <param name="length">Matched UTF-16 character count, including stored structural markers.<br/></param>
    /// <returns>True for a complete stored-name prefix ending at the request end or a structural continuation boundary.<br/></returns>
    private static bool TryMatchReferencePrefix(ReadOnlySpan<byte> payload, CompiledNameEntry entry,
        uint anchor, ReadOnlySpan<char> path, out int length)
    {
        length = 0;
        int cursor = 0;
        var current = entry;
        while (true)
        {
            int count = Encoding.UTF8.GetCharCount(current.Name);
            if (count > path.Length - length) return false;
            length += count;
            if (current.ParentIndex == anchor) break;
            if (current.ParentIndex < anchor || current.ParentIndex >= current.Index) return false;
            cursor = checked((int)current.ParentIndex);
            if (!TryReadCompiledNameEntry(payload, ref cursor, out current)) return false;
        }
        if (length == 0 || (length < path.Length && path[length] is not ('.' or '!' or '#' or '@'))) return false;
        int end = length;
        current = entry;
        while (true)
        {
            int count = Encoding.UTF8.GetCharCount(current.Name);
            end -= count;
            if (!ReferenceNameEquals(current.Name, path.Slice(end, count))) return false;
            if (current.ParentIndex == anchor) return true;
            cursor = checked((int)current.ParentIndex);
            if (!TryReadCompiledNameEntry(payload, ref cursor, out current)) return false;
        }
    }

    /// <summary>
    /// Compares one intact UTF-8 header name to UTF-16 request text without temporary encoding buffers.<br/>
    /// ASCII uses direct comparisons; non-ASCII uses scalar decoding and rejects malformed sequences on this optional route.<br/>
    /// </summary>
    /// <param name="bytes">Borrowed complete name bytes.<br/></param>
    /// <param name="text">Request slice of the same decoded character length.<br/></param>
    /// <returns>True for ordinal equality, including literal markers inside keys and supplementary Unicode characters.<br/></returns>
    private static bool ReferenceNameEquals(ReadOnlySpan<byte> bytes, ReadOnlySpan<char> text)
    {
        while (!bytes.IsEmpty && !text.IsEmpty)
        {
            if (bytes[0] < 128)
            {
                if (bytes[0] != text[0]) return false;
                bytes = bytes[1..]; text = text[1..];
            }
            else
            {
                if (Rune.DecodeFromUtf8(bytes, out Rune left, out int readBytes) != OperationStatus.Done ||
                    Rune.DecodeFromUtf16(text, out Rune right, out int readChars) != OperationStatus.Done || left != right) return false;
                bytes = bytes[readBytes..]; text = text[readChars..];
            }
        }
        return bytes.IsEmpty && text.IsEmpty;
    }

    /// <summary>
    /// Resolves a reference continuation only after the rich physical compatibility traversal found no exact path.<br/>
    /// Keeps reference-only descriptor construction and locals outside the ordinary rich lookup method.<br/>
    /// A missing continuation remains empty; no destination assignment or missing-value exception is introduced.<br/>
    /// </summary>
    /// <param name="path">Unchanged request and existing preparation token, including transient unsupported physical plans.<br/></param>
    /// <param name="cacheResolution">Whether the resulting physical index or missing marker may be published.<br/></param>
    /// <returns>The rich borrowed terminal descriptor, or an empty descriptor for an absent logical value.<br/></returns>
    private NameHeaderEntry ResolveReferenceMiss(PreparedInhetoPath path, bool cacheResolution)
    {
        TryFindReferencedNameEntry(path.Path, out uint index);
        if (cacheResolution) CacheResolvedNameEntry(path, index);
        return index == 0 ? default : NameHeaderEntry.CreateBorrowed(stream, index);
    }

    /// <summary>
    /// Resolves a complete binary request progressively against intact stored names and parent offsets.<br/>
    /// Ordinary members/elements advance directly; literal dictionary keys select the longest matching name before descent.<br/>
    /// Exact key names win immediately, independent of enumeration order; a selected null or dead end never retries a shorter key.<br/>
    /// Reference continuations consume request characters before following their persisted target; terminal references remain terminal.<br/>
    /// Uses borrowed spans and scalar metadata only, with no split strings, encoded request arrays, delegates, or depth-sized stack.<br/>
    /// </summary>
    /// <param name="path">Unchanged binary property path; an empty string addresses the root.<br/></param>
    /// <param name="resolved">Deepest matched entry, including a terminal null/reference or root when no child matched.<br/></param>
    /// <param name="matched">Consumed UTF-16 characters; equality with path length identifies an exact match.<br/></param>
    /// <param name="followedReference">Whether a nonterminal persisted reference was traversed.<br/></param>
    /// <returns>True for a handled exact/prefix/missing result; false requests established compatibility parsing.<br/></returns>
    private bool TryResolveLiteralNameEntry(string path, out CompiledNameEntry resolved, out int matched, out bool followedReference)
    {
        resolved = default;
        matched = 0;
        followedReference = false;
        ReadOnlySpan<byte> payload = stream.AsReadOnlySpan;
        if (payload.Length < 2 || payload[0] != 1) return false;
        int cursor = 1;
        if (literalReadAnchorIndex != 0 && literalReadAnchorPath is { } anchorPath &&
            path.AsSpan().StartsWith(anchorPath, StringComparison.Ordinal) &&
            (path.Length == anchorPath.Length || path[anchorPath.Length] is '.' or '!' or '#' or '@'))
        {
            cursor = checked((int)literalReadAnchorIndex);
            matched = anchorPath.Length;
        }
        if (!TryReadCompiledNameEntry(payload, ref cursor, out resolved)) return false;
        while (matched < path.Length)
        {
            int hops = 0;
            while (resolved.BaseType is InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
            {
                if (++hops > payload.Length || resolved.ValueOffset == 0 || resolved.ValueOffset >= payload.Length)
                    throw new InvalidDataException("Invalid or cyclic name-header reference chain.");
                cursor = checked((int)resolved.ValueOffset);
                if (!TryReadCompiledNameEntry(payload, ref cursor, out resolved))
                    throw new InvalidDataException("Invalid referenced name header.");
                followedReference = true;
            }
            uint anchor = resolved.Index;
            ReadOnlySpan<char> remainder = path.AsSpan(matched);
            CompiledNameEntry best = default;
            int bestLength = 0;
            int bestCursor = 0;
            while (cursor < payload.Length)
            {
                if (!TryReadCompiledNameEntry(payload, ref cursor, out var child)) return false;
                if (child.BaseType == InhetoBaseTypes.EndOfNamesHeader || child.ParentIndex < anchor) break;
                if (child.ParentIndex != anchor || !TryMatchStoredNamePrefix(child.Name, remainder, out int length)) continue;
                if (length == remainder.Length)
                {
                    resolved = child;
                    matched = path.Length;
                    return true;
                }
                if (length > bestLength)
                {
                    best = child;
                    bestLength = length;
                    bestCursor = cursor;
                }
                // Dictionary keys are the literal, variable-name namespace. Structural member/element
                // names proceed directly; key prefixes must wait for any more-specific sibling key.
                if (child.Name[0] != (byte)'!') break;
            }
            if (bestLength == 0) return true;
            resolved = best;
            matched += bestLength;
            cursor = bestCursor;
        }
        return true;
    }

    /// <summary>
    /// Matches one complete UTF-8 stored name at the beginning of unchanged request text without allocating or parsing that text.<br/>
    /// ASCII uses direct comparisons; Unicode uses Rune decoding so supplementary characters count as two UTF-16 units.<br/>
    /// A partial match must end at a structural continuation; numbered element 1 therefore cannot match element 10.<br/>
    /// Marker characters inside the stored name remain literal; an empty dictionary key has the complete stored name !.<br/>
    /// </summary>
    /// <param name="name">Borrowed intact name, including its structural leading marker.<br/></param>
    /// <param name="request">Unconsumed request text.<br/></param>
    /// <param name="length">Matched UTF-16 character count when the complete name matches.<br/></param>
    /// <returns>True only for an intact ordinal name prefix ending at a valid continuation boundary.<br/></returns>
    private static bool TryMatchStoredNamePrefix(ReadOnlySpan<byte> name, ReadOnlySpan<char> request, out int length)
    {
        length = 0;
        if (name.IsEmpty || request.IsEmpty || name[0] != request[0]) return false;
        while (!name.IsEmpty)
        {
            if (length >= request.Length) return false;
            if (name[0] < 128)
            {
                if (name[0] != request[length]) return false;
                name = name[1..];
                length++;
            }
            else
            {
                if (Rune.DecodeFromUtf8(name, out Rune left, out int bytes) != OperationStatus.Done ||
                    Rune.DecodeFromUtf16(request[length..], out Rune right, out int characters) != OperationStatus.Done || left != right)
                    return false;
                name = name[bytes..];
                length += characters;
            }
        }
        return length == request.Length || request[length] is '.' or '!' or '#' or '@';
    }

    /// <summary>
    /// Retains a dictionary filler's already-selected physical child while its value and descendants are reconstructed.<br/>
    /// Global most-specific lookup cannot identify a shadowed shorter interpretation from the identical flattened string.<br/>
    /// This value-only scope borrows existing text and restores the prior anchor on every exit without heap allocation or delegates.<br/>
    /// Reentrant public typed operations detach this internal anchor; ordinary external navigation remains rooted and most-specific.<br/>
    /// </summary>
    private readonly struct LiteralValueReadScope : IDisposable
    {
        private readonly InhetoBinary owner;
        private readonly uint previousIndex;
        private readonly string? previousPath;

        /// <summary>Publishes a borrowed physical child and its existing logical name for one synchronous value reconstruction.<br/></summary>
        /// <param name="reader">Payload reader performing the dictionary fill.<br/></param>
        /// <param name="entryIndex">Known physical child offset, or the numbered entry parent for Key/Value children.<br/></param>
        /// <param name="path">Already-constructed exact path of that physical child; no new string is produced.<br/></param>
        internal LiteralValueReadScope(InhetoBinary reader, uint entryIndex, string? path)
        {
            owner = reader;
            previousIndex = reader.literalReadAnchorIndex;
            previousPath = reader.literalReadAnchorPath;
            reader.literalReadAnchorIndex = entryIndex;
            reader.literalReadAnchorPath = path;
        }

        /// <summary>Restores the enclosing known-entry context after success, null, callback failure, or conversion failure.<br/></summary>
        public void Dispose()
        {
            owner.literalReadAnchorIndex = previousIndex;
            owner.literalReadAnchorPath = previousPath;
        }
    }
}
