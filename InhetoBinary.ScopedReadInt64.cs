namespace Inheto;

public partial class InhetoBinary
{
    /// <summary>
    /// Reads an independent Int64 while releasing temporary recursive name-header descriptors at the end of this call.<br/>
    /// Preserves exact-path/cache resolution, DuplicateValue chains, missing/null checks and BufferStream value/cursor behavior.<br/>
    /// No native descriptor escapes in the nullable scalar; native layout, wire format and other reader contracts are unchanged.<br/>
    /// </summary>
    /// <param name="propPath">Exact physical path, including the existing empty/null root alias.<br/></param>
    /// <returns>The decoded Int64, or the existing null result for an absent or encoded-null entry.<br/></returns>
    private long? ReadInt64Core(string propPath)
    {
        NameHeaderAllocationGroup group = default;
        return TryReadInt64Scoped(propPath, ref group, out long? result) ? result : ReadInt64Legacy(propPath);
    }

    /// <summary>
    /// Resolves one Int64 under sole local native ownership and releases all linked chunks on return or exception.<br/>
    /// Scalar navigation skips native construction in exact-path fallback; retained rich root/cached/compiled routes still use this group.<br/>
    /// Each DuplicateValue target is parsed in the same group, without a name override, then rechecked for missing/null.<br/>
    /// DuplicateRef treatment, requested-type permissiveness and the existing duplicate-chain termination policy are unchanged.<br/>
    /// </summary>
    /// <param name="propPath">Existing exact-path input normalized by the shared prepared-path resolver.<br/></param>
    /// <param name="group">Fresh sole owner passed by ref; optional local counters and failure injection support acceptance tests.<br/></param>
    /// <param name="result">Independent nullable scalar, never a borrowed native description.<br/></param>
    /// <returns>True when handled; any retained false-result compatibility fallback occurs after releasing the group.<br/></returns>
    internal bool TryReadInt64Scoped(string propPath, ref NameHeaderAllocationGroup group, out long? result)
    {
        result = null;
        try
        {
            if (!TryGetScalarNavigation(GetPreparedPath(propPath), ref group, out var entry)) return false;
            while (true)
            {
                if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType is InhetoBaseTypes.Unknown or InhetoBaseTypes.Null) return true;
                if (entry.Kind != InhetoKindMasks.InhetoBaseType || entry.Type.BaseType != InhetoBaseTypes.DuplicateValue) break;
                if (!NameHeaderEntry.TryCreateScoped(stream, entry.ValueOffset, null, ref group, out entry)) return false;
            }
            stream.Position = entry.ValueOffset;
            result = stream.ReadInt64();
            return true;
        }
        finally { group.Release(); }
    }
}
