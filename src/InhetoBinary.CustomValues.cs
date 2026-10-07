namespace Inheto;

public partial class InhetoBinary
{
    /// <summary>
    /// Reads a custom payload once into the existing operation cache without cloning its decoded result.<br/>
    /// The physical payload's property path selects the decoder for persisted references, regardless of visit order.<br/>
    /// Unshared property-name projections retain requested-path decoder precedence; shared projections use physical-path authority.<br/>
    /// Callers retain the prior DuplicateValue route; this helper is not an identity policy for deduplicated values.<br/>
    /// Successful null results are cached; failed callbacks are never published. Nested public operations retain their existing cache/cursor restoration.<br/>
    /// </summary>
    /// <param name="entry">Resolved custom header; its index is the physical payload identity even if its display name was substituted.<br/></param>
    /// <param name="requestedPath">Original requested property path, retained for non-reference name projections.<br/></param>
    /// <param name="resolvedPath">Path selected by existing name/projection navigation.<br/></param>
    /// <param name="reference">Whether the selected terminal was a persisted DuplicateRef.<br/></param>
    /// <param name="memberType">Optional member target enforcing the existing type-hook source-assignability check.<br/></param>
    /// <param name="options">Current operation's decoder registrations.<br/></param>
    /// <param name="value">Decoded result, reused by identity rather than cloned.<br/></param>
    /// <returns>True when custom decoding was handled; false preserves existing missing/incompatible member fallback.<br/></returns>
    private bool TryReadCustomValue(in NameHeaderEntry entry, string requestedPath, string resolvedPath,
        bool reference, Type? memberType, DeserializationOptions options, out object? value)
    {
        if (RefCache.TryGetValue(entry.Index, out value)) return true;

        Func<object, object?>? decoder;
        if (entry.Type.BaseType == InhetoBaseTypes.CustomProp)
        {
            // A borrowed duplicate may carry the alias name: recover the original physical path only on a cache miss.
            var source = NameHeaderEntry.CreateBorrowed(stream, entry.Index);
            string sourcePath = source.PropPath;
            options.MemberDeserializers.TryGetValue(sourcePath, out decoder);
            // Preserve ordinary name-remapping hooks, but not at the expense of persisted shared identity.
            // The scan is confined to this uncommon competing name-projection registration on a cache miss.
            if (!reference && StringComparer.Ordinal.Equals(resolvedPath, sourcePath) &&
                !StringComparer.Ordinal.Equals(requestedPath, sourcePath) &&
                options.MemberDeserializers.TryGetValue(requestedPath, out var projectedDecoder) &&
                !HasCustomReference(source))
                decoder = projectedDecoder;
            if (decoder is null)
            {
                value = null;
                return false;
            }
            stream.Position = entry.ValueOffset;
        }
        else
        {
            if (options.TypeDeserializers.Count == 0)
            {
                value = null;
                return false;
            }
            stream.Position = entry.ValueOffset;
            var sourceType = Type.GetType(stream.ReadString() ?? throw new InvalidDataException("Missing custom payload type name."));
            if (sourceType is null ||
                (memberType is not null && !memberType.IsAssignableFrom(sourceType) && Nullable.GetUnderlyingType(memberType) != sourceType) ||
                !options.TypeDeserializers.TryGetValue(sourceType, out decoder))
            {
                value = null;
                return false;
            }
        }

        value = decoder(stream.ReadBytesWithByteLength());
        RefCache[entry.Index] = value!;
        return true;
    }

    /// <summary>
    /// Detects persisted reference aliases to a custom payload or its physical ancestors without building an index.<br/>
    /// Used only when a different property-name projection explicitly registers its own decoder on a cold custom read.<br/>
    /// Ordinary custom paths and all cache hits skip this scan. Borrowed compiled headers create no managed/native descriptor allocation.<br/>
    /// </summary>
    /// <param name="source">Original physical custom header whose projection could otherwise override shared decoder authority.<br/></param>
    /// <returns>True when any persisted DuplicateRef reaches the source or an ancestor.<br/></returns>
    private bool HasCustomReference(in NameHeaderEntry source)
    {
        var bytes = stream.AsReadOnlySpan;
        int cursor = 1;
        while (cursor < bytes.Length)
        {
            if (!TryReadCompiledNameEntry(bytes, ref cursor, out var candidate))
                throw new InvalidDataException("Invalid custom-reference name header.");
            if (candidate.BaseType == InhetoBaseTypes.EndOfNamesHeader) break;
            if (candidate.BaseType != InhetoBaseTypes.DuplicateRef) continue;
            uint ancestor = source.Index;
            while (ancestor >= candidate.ValueOffset && ancestor != 0)
            {
                if (ancestor == candidate.ValueOffset) return true;
                int parentCursor = checked((int)ancestor);
                if (!TryReadCompiledNameEntry(bytes, ref parentCursor, out var parent) || parent.ParentIndex >= ancestor)
                    throw new InvalidDataException("Invalid custom-reference parent chain.");
                ancestor = parent.ParentIndex;
            }
        }
        return false;
    }

    /// <summary>
    /// Projects an undecoded custom payload through core byte-value conversion rather than suppressing it.<br/>
    /// Removes only Inheto framing; byte-array/object destinations receive the serializer's exact payload.<br/>
    /// MemoryStream targets wrap the exact bytes as a read-only stream; other targets use existing core coercion, never an invented native encoding for opaque custom bytes.<br/>
    /// Raw arrays share the existing operation cache; no missing-decoder decision or registration survives a conversion.<br/>
    /// </summary>
    /// <param name="entry">Resolved custom header, including the physical payload offset.<br/></param>
    /// <param name="targetType">Requested destination type, not the historical source type.<br/></param>
    /// <param name="path">Requested path included in unsupported-conversion diagnostics.<br/></param>
    /// <returns>The supported core projection; empty bytes remain distinct from an encoded Null node.<br/></returns>
    /// <exception cref="InvalidCastException">Core cannot convert the custom bytes to the requested type.<br/></exception>
    private object? ReadCustomFallback(in NameHeaderEntry entry, Type targetType, string path)
    {
        stream.Position = entry.ValueOffset;
        if (entry.Type.BaseType == InhetoBaseTypes.CustomType)
        {
            int typeLength = stream.Read7BitEncodedInt();
            if (typeLength < 0 || stream.Position + typeLength > stream.Length)
                throw new InvalidDataException($"Invalid custom type metadata at '{path}'.");
            stream.Position += typeLength;
        }
        byte[] bytes = stream.ReadBytesWithByteLength();
        if (targetType == typeof(MemoryStream))
        {
            var result = new MemoryStream(bytes, writable: false);
            if (materializationOperationDepth != 0) RefCache[entry.Index] = result;
            return result;
        }
        if (targetType.IsInstanceOfType(bytes))
        {
            if (materializationOperationDepth != 0) RefCache[entry.Index] = bytes;
            return bytes;
        }
        if (FastCoerce.TryCoerce(bytes, targetType, out var value)) return value;
        throw new InvalidCastException($"Custom payload at '{path}' has no applicable deserializer and core cannot convert its bytes to '{targetType.FullName}'.");
    }
}
