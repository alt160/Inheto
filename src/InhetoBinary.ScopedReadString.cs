namespace Inheto;

public partial class InhetoBinary
{
    /// <summary>
    /// Reads an independent string while releasing temporary recursive name-header storage at the end of this call.<br/>
    /// Preserves exact-path/cache resolution, one-hop duplicate handling, stream encoding/cursor behavior and missing/null/empty distinctions.<br/>
    /// No decoded descriptor escapes with the string; native layout, wire format and other materialization routes are unchanged.<br/>
    /// </summary>
    /// <param name="propPath">Exact physical property path, or the empty/null root path accepted by the existing resolver.<br/></param>
    /// <returns>The decoded string, including empty, or the existing null result for absent/encoded-null values.<br/></returns>
    private string? ReadStringCore(string propPath)
    {
        NameHeaderAllocationGroup group = default;
        return TryReadStringScoped(propPath, ref group, out string? result) ? result : ReadStringLegacy(propPath);
    }

    /// <summary>
    /// Resolves and reads one string under sole local native ownership, releasing on return and all exceptions.<br/>
    /// Scalar navigation skips native construction in exact-path fallback; retained rich root/cached/compiled routes still use this group.<br/>
    /// Duplicate targets are reparsed without a name override and followed once, matching the original ReadString contract.<br/>
    /// String decoding continues through BufferStream so encoding callbacks, bounds and final shared-cursor semantics are preserved.<br/>
    /// </summary>
    /// <param name="propPath">Existing exact-path input, normalized by the shared prepared-path resolver.<br/></param>
    /// <param name="group">Fresh sole owner passed by ref; internal counters/failure injection support cleanup verification.<br/></param>
    /// <param name="result">Independent decoded string, or null for the existing missing/null outcomes.<br/></param>
    /// <returns>True when handled; a retained false-result compatibility route releases all grouped storage before legacy handling.<br/></returns>
    internal bool TryReadStringScoped(string propPath, ref NameHeaderAllocationGroup group, out string? result)
    {
        result = null;
        try
        {
            if (!TryGetScalarNavigation(GetPreparedPath(propPath), ref group, out var entry)) return false;
            if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType is InhetoBaseTypes.Unknown or InhetoBaseTypes.Null) return true;
            if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType is InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
                if (!NameHeaderEntry.TryCreateScoped(stream, entry.ValueOffset, null, ref group, out entry)) return false;
            stream.Position = entry.ValueOffset;
            result = stream.ReadString();
            return true;
        }
        finally { group.Release(); }
    }

    /// <summary>
    /// Materializes a collection string with one ordinary navigation and a cold custom-value dispatch.<br/>
    /// Native strings retain direct BufferStream decoding without a generic reader, callback or value-cache lookup.<br/>
    /// Only custom terminals enter the existing typed codec pipeline, retaining duplicates, null results and core fallback.<br/>
    /// Public ReadString remains a low-level reader with its unchanged contract.<br/>
    /// </summary>
    /// <param name="path">Progressive-prefix element path, including arbitrary indexed/reference ancestry.<br/></param>
    /// <returns>The materialized string or intentional null; unsupported custom conversion throws with its path.<br/></returns>
    private string? ReadMaterializedString(string path)
    {
        NameHeaderAllocationGroup group = default;
        try
        {
            if (!TryGetScalarNavigation(GetPreparedPath(path), ref group, out var entry))
                return ReadMaterializedAs<string>(path, materializationOptions ?? deserializationOptions);
            var original = entry;
            if (entry.Kind == InhetoKindMasks.InhetoBaseType &&
                entry.Type.BaseType is InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
                if (!NameHeaderEntry.TryCreateScoped(stream, entry.ValueOffset, null, ref group, out entry))
                    return ReadMaterializedAs<string>(path, materializationOptions ?? deserializationOptions);
            if (entry.Kind == InhetoKindMasks.InhetoBaseType)
            {
                if (entry.Type.BaseType is InhetoBaseTypes.Unknown or InhetoBaseTypes.Null) return null;
                if (entry.Type.BaseType is InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType)
                    return ReadMaterializedAs<string>(default, original, path, materializationOptions ?? deserializationOptions, true);
            }
            stream.Position = entry.ValueOffset;
            return stream.ReadString();
        }
        finally { group.Release(); }
    }
}
