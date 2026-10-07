namespace Inheto;

/// <summary>
/// Navigates and materializes an Inheto binary payload without requiring a root-object reconstruction for each member read. One instance has mutable cursor and cache state; do not use it concurrently without external coordination.<br/>
/// </summary>
public partial class InhetoBinary
{
    /// <summary>
    /// Resolves an uncached ordinary-object reference through its serialized destination member before choosing an alias type.<br/>
    /// Reuses the existing root/publication cache and immutable compiled member plans; creates no global path index or operation frame.<br/>
    /// A local parent supports partial projections; a full operation root permits ordinary-member ancestor traversal across branches.<br/>
    /// Missing, excluded or readonly-null canonical destinations retain alias fallback. Explicit null activation remains suppression.<br/>
    /// </summary>
    /// <param name="targetIndex">Physical ordinary-object header referenced by the requesting alias.<br/></param>
    /// <param name="options">Current operation's discovery, projection and activation options.<br/></param>
    /// <param name="localParent">Optional object currently being filled, preserving partial-read scope.<br/></param>
    /// <param name="localPath">Logical path of that object, without tokenizing its member names.<br/></param>
    /// <param name="localIndex">Physical parent header, or zero for the fast root path.<br/></param>
    /// <returns>True when the canonical target has a published instance or intentional null; false permits existing fallback.<br/></returns>
    private bool TryHydrateCanonicalMember(uint targetIndex, DeserializationOptions options, object? localParent = null, string localPath = "", uint localIndex = 0)
    {
        if (refCache is not null && refCache.ContainsKey(targetIndex)) return true;
        var target = NameHeaderEntry.CreateBorrowed(stream, targetIndex);
        if (target.Kind != InhetoKindMasks.InhetoBaseType || target.Type.BaseType != InhetoBaseTypes.ComplexType) return false;
        if (localParent is not null && (localIndex == target.ParentIndex || localIndex == 0 && localPath.Length == 0 && target.ParentIndex == 1))
            return HydrateCanonicalBranch(localParent, localPath, target.ParentIndex, target, options);

        object? root = activeComplexRoot;
        if (root is null && refCache is not null) refCache.TryGetValue(1, out root);
        if (root is not null) return HydrateCanonicalBranch(root, "", 1, target, options);
        return localParent is not null && localIndex != 0 &&
            HydrateCanonicalBranch(localParent, localPath, localIndex, target, options);
    }

    /// <summary>
    /// Follows physical header ancestry through selected destination object members, hydrating only the needed branch.<br/>
    /// Sibling lookup without name aliases compares cached member segments directly; unrelated getters are never invoked.<br/>
    /// Ancestor tests use header parent links, not split proppath tokens, so literal separators retain their existing meaning.<br/>
    /// The ordinary filler assigns and publishes an activated member before descending, allowing existing cycle handling to apply.<br/>
    /// Array and collection ancestors use their existing typed fillers; selected collections publish before filling to prevent rescheduling.<br/>
    /// </summary>
    /// <param name="parent">Existing destination parent or box to inspect through its compiled member plan.<br/></param>
    /// <param name="parentPath">Current logical destination path used by existing name projection.<br/></param>
    /// <param name="parentIndex">Physical source index represented by this parent.<br/></param>
    /// <param name="target">Borrowed complete canonical target descriptor; ownership stays with the reader.<br/></param>
    /// <param name="options">Effective member policy, exclusions, aliases and activators.<br/></param>
    /// <returns>True for a published target including intentional null; false if the destination branch is unavailable.<br/></returns>
    private bool HydrateCanonicalBranch(object parent, string parentPath, uint parentIndex, NameHeaderEntry target, DeserializationOptions options)
    {
        bool sibling = parentIndex == target.ParentIndex;
        foreach (var member in GetMaterializationMembers(parent.GetType(), options.MemberTypes))
        {
            if (sibling && !options.Aliases.HasEntries && !StringComparer.Ordinal.Equals(member.PathSegment, target.Name)) continue;
            if (member.MemberType.IsValueType || member.Accessors is null) continue;
            string path = parentPath.Length == 0 ? member.PathSegment : string.Concat(parentPath, member.PathSegment);
            if (options.ExcludedMembers?.Contains(path) == true) continue;
            var entry = GetNameEntryFromPropPath(path, options, out _, cacheResolution: false);
            if (entry.IsEmpty) continue;
            bool container = entry.Kind == InhetoKindMasks.InhetoCollectionType && member.TypeInfo.IsCollection ||
                IsComplexArrayShape(member.MemberType, entry, options.MemberTypes);
            if (!container && (entry.Kind != InhetoKindMasks.InhetoBaseType || entry.Type.BaseType != InhetoBaseTypes.ComplexType)) continue;
            bool terminal = entry.Index == target.Index;
            if (!terminal && (sibling || !IsCanonicalAncestor(entry.Index, target.ParentIndex))) continue;
            // A published container may still be filling. Never recursively start that same fill again.
            if (container && refCache is not null && refCache.TryGetValue(entry.Index, out var published))
            {
                if (published is null) RefCache[target.Index] = null!;
                return refCache.ContainsKey(target.Index);
            }
            object? supplied = member.Accessors.GetValue(parent);
            if (supplied is null && member.Accessors.ReadOnly) return false;
            // Array members retain copy-on-write semantics; their ordinary filler does not mutate readonly arrays.
            if (container && member.MemberType.IsArray && member.Accessors.ReadOnly) return false;
            doPropsAndFields(options, parentPath, ref parent, true, true, parentIndex, member.PathSegment);
            if (refCache is not null && refCache.ContainsKey(target.Index)) return true;
            if (terminal) return false;
            object? child = member.Accessors.GetValue(parent);
            if (child is null)
            {
                if (refCache is not null && refCache.TryGetValue(entry.Index, out var declined) && declined is null)
                {
                    RefCache[target.Index] = null!;
                    return true;
                }
                return false;
            }
            if (container) return false;
            return HydrateCanonicalBranch(child, path, entry.Index, target, options);
        }
        return false;
    }

    /// <summary>
    /// Checks physical ancestry without allocating a path, array or visited-node set.<br/>
    /// Parent headers must precede their children; a non-decreasing malformed link cannot make this scan loop forever.<br/>
    /// </summary>
    /// <param name="candidate">Potential ancestor header index from a selected destination member.<br/></param>
    /// <param name="parent">First physical parent of the target node.<br/></param>
    /// <returns>True when the candidate occurs in the strictly decreasing physical parent chain.<br/></returns>
    private bool IsCanonicalAncestor(uint candidate, uint parent)
    {
        while (parent >= candidate && parent != 0)
        {
            if (parent == candidate) return true;
            var entry = NameHeaderEntry.CreateBorrowed(stream, parent);
            if (entry.ParentIndex >= parent) return false;
            parent = entry.ParentIndex;
        }
        return false;
    }
}
