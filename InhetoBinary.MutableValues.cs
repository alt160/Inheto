namespace Inheto;

public partial class InhetoBinary
{
    /// <summary>
    /// Reads string-list contents into their operation-local canonical instance or a distinct supplied destination.<br/>
    /// Missing destinations reuse the first completed list. The same canonical destination is not appended twice.<br/>
    /// A different supplied list is filled without replacing the canonical cache entry or the developer's object.<br/>
    /// String elements cannot recursively reference this list, so publication is deferred until filling succeeds.<br/>
    /// Failure may leave a supplied list partially filled but never publishes an incomplete new canonical value.<br/>
    /// </summary>
    /// <param name="entry">Borrowed physical ListString header; its Index identifies the referent.<br/></param>
    /// <param name="path">Requested path used by existing progressive-prefix element reads.<br/></param>
    /// <param name="destination">Optional developer-owned list; its existing contents are retained.<br/></param>
    /// <returns>The exact filled or previously completed list, without a mutable clone.<br/></returns>
    private List<string> ReadOperationStringList(NameHeaderEntry entry, string path, List<string>? destination)
    {
        List<string>? canonical = null;
        if (refCache is not null && refCache.TryGetValue(entry.Index, out var cached))
            canonical = cached as List<string>;
        if (canonical is not null && (destination is null || ReferenceEquals(destination, canonical)))
            return canonical;

        var result = destination ?? new List<string>((int)entry.ValueOffset);
        for (int i = 0; i < entry.ValueOffset; i++)
            result.Add(ReadMaterializedString($"{path}#{i}")!);
        if (canonical is null) RefCache[entry.Index] = result;
        return result;
    }

    /// <summary>
    /// Retains the existing standalone cached-read and mutable-copy behavior outside explicit create/fill operations.<br/>
    /// Its captured factory is kept off the active operation path; this is not a new cache or dispatch abstraction.<br/>
    /// Standalone supplied-instance behavior is unchanged and remains distinct from explicit ReadPropOnto filling.<br/>
    /// </summary>
    /// <param name="entry">Borrowed resolved header, including existing duplicate handling.<br/></param>
    /// <param name="path">Requested path for string element lookup.<br/></param>
    /// <param name="destination">Existing standalone optional destination argument.<br/></param>
    /// <returns>The existing isolated mutable-copy result.<br/></returns>
    private List<string> ReadStandaloneStringList(NameHeaderEntry entry, string path, List<string>? destination)
        => GetOrAddCached(entry.Index, k =>
        {
            var result = destination ?? new List<string>((int)entry.ValueOffset);
            for (int i = 0; i < entry.ValueOffset; i++) result.Add(ReadMaterializedString($"{path}#{i}")!);
            return result;
        });

    /// <summary>
    /// Appends a NameValueCollection's encoded keys and values without treating reference headers as string payloads.<br/>
    /// A Null key-child represents an existing key whose value set is null; it is not an omitted POCO member.<br/>
    /// Element reads retain the existing progressive-prefix path and duplicate-value interpretation.<br/>
    /// Existing destination keys, comparer and values are retained through its normal Add semantics.<br/>
    /// </summary>
    /// <param name="entry">Borrowed resolved NameValueCollection header; ValueOffset is its key count.<br/></param>
    /// <param name="destination">Collection receiving the persisted keys and values in encoded order.<br/></param>
    /// <param name="options">Optional explicit decoder policy for cold whole-value-set reads.<br/></param>
    private void FillNameValues(NameHeaderEntry entry, System.Collections.Specialized.NameValueCollection destination, DeserializationOptions? options = null)
    {
        if (entry.ValueOffset == 0) return;
        var keyEntry = entry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
        for (int i = 0; i < entry.ValueOffset; i++)
        {
            var key = keyEntry.CleanName;
            if (keyEntry.Kind == InhetoKindMasks.InhetoBaseType && keyEntry.Type.BaseType == InhetoBaseTypes.Null)
                destination.Add(key, null);
            else if (keyEntry.Kind == InhetoKindMasks.InhetoPromotedType && keyEntry.Type.PromotedType == InhetoPromotedTypes.Array)
                for (int j = 0; j < keyEntry.ValueOffset; j++)
                    destination.Add(key, ReadMaterializedString($"{keyEntry.PropPath}#{j}"));
            else
            {
                // A custom/reference payload offset is not an element count. Preserve the native array loop above.
                var decoded = ReadNameValueSet(keyEntry, options);
                if (decoded is string[] values)
                {
                    if (values.Length == 0) destination.Add(key, null);
                    else for (int j = 0; j < values.Length; j++) destination.Add(key, values[j]);
                }
                else if (decoded is List<string> list)
                {
                    if (list.Count == 0) destination.Add(key, null);
                    else for (int j = 0; j < list.Count; j++) destination.Add(key, list[j]);
                }
                else destination.Add(key, null);
            }
            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
        }
    }

    /// <summary>
    /// Reuses the first completed NameValueCollection within the current materialization operation.<br/>
    /// Distinct supplied destinations are filled in place without replacing the canonical referent.<br/>
    /// Reusing that canonical destination does not append twice; noncanonical fills remain additive per call.<br/>
    /// String-only contents cannot cycle back to this collection, so publication waits for successful filling.<br/>
    /// </summary>
    /// <param name="entry">Resolved physical header identifying the canonical referent.<br/></param>
    /// <param name="destination">Optional initialized destination, including a readonly member's referent.<br/></param>
    /// <returns>The exact completed canonical or filled supplied collection, without cloning.<br/></returns>
    private System.Collections.Specialized.NameValueCollection ReadOperationNameValues(NameHeaderEntry entry, System.Collections.Specialized.NameValueCollection? destination)
    {
        System.Collections.Specialized.NameValueCollection? canonical = null;
        if (refCache is not null && refCache.TryGetValue(entry.Index, out var cached))
            canonical = cached as System.Collections.Specialized.NameValueCollection;
        if (canonical is not null && (destination is null || ReferenceEquals(destination, canonical)))
            return canonical;
        var result = destination ?? new System.Collections.Specialized.NameValueCollection();
        FillNameValues(entry, result);
        if (canonical is null) RefCache[entry.Index] = result;
        return result;
    }

    /// <summary>
    /// Keeps standalone NameValueCollection copy isolation and optional-destination cache behavior unchanged.<br/>
    /// The captured factory remains outside the active operation reader; only content decoding is corrected.<br/>
    /// </summary>
    /// <param name="entry">Resolved physical collection header.<br/></param>
    /// <param name="destination">Existing standalone destination argument; used only on a cache miss.<br/></param>
    /// <returns>The existing helper's isolated copy of the decoded collection.<br/></returns>
    private System.Collections.Specialized.NameValueCollection ReadStandaloneNameValues(NameHeaderEntry entry, System.Collections.Specialized.NameValueCollection? destination)
        => GetOrAddCached(entry.Index, k =>
        {
            var result = destination ?? new System.Collections.Specialized.NameValueCollection();
            FillNameValues(entry, result);
            return result;
        });

    /// <summary>
    /// Reads StringCollection contents into an operation-local canonical or distinct supplied instance.<br/>
    /// Null positions and empty strings are appended unchanged through the existing marker-aware reader.<br/>
    /// Publication follows successful filling; repeat use of the canonical object does not append again.<br/>
    /// Distinct supplied destinations retain their identity and do not displace the canonical cache entry.<br/>
    /// </summary>
    /// <param name="entry">Resolved physical header; ValueOffset is the element count.<br/></param>
    /// <param name="path">Requested progressive-prefix path used for indexed element reads.<br/></param>
    /// <param name="destination">Optional developer-owned collection with existing contents retained.<br/></param>
    /// <returns>The exact completed or filled instance; independent operation cleanup remains external.<br/></returns>
    private System.Collections.Specialized.StringCollection ReadOperationStrings(NameHeaderEntry entry, string path, System.Collections.Specialized.StringCollection? destination)
    {
        System.Collections.Specialized.StringCollection? canonical = null;
        if (refCache is not null && refCache.TryGetValue(entry.Index, out var cached))
            canonical = cached as System.Collections.Specialized.StringCollection;
        if (canonical is not null && (destination is null || ReferenceEquals(destination, canonical)))
            return canonical;
        var result = destination ?? new System.Collections.Specialized.StringCollection();
        for (int i = 0; i < entry.ValueOffset; i++) result.Add(ReadMaterializedString($"{path}#{i}"));
        if (canonical is null) RefCache[entry.Index] = result;
        return result;
    }

    /// <summary>
    /// Retains standalone StringCollection cloning and factory-only optional-destination behavior.<br/>
    /// Isolates the captured factory from active operation reads without adding a cache or delegate layer there.<br/>
    /// </summary>
    /// <param name="entry">Borrowed resolved collection header.<br/></param>
    /// <param name="path">Existing indexed string lookup path.<br/></param>
    /// <param name="destination">Optional standalone supplied collection, used only on a cache miss.<br/></param>
    /// <returns>The existing isolated mutable-copy result.<br/></returns>
    private System.Collections.Specialized.StringCollection ReadStandaloneStrings(NameHeaderEntry entry, string path, System.Collections.Specialized.StringCollection? destination)
        => GetOrAddCached(entry.Index, k =>
        {
            var result = destination ?? new System.Collections.Specialized.StringCollection();
            for (int i = 0; i < entry.ValueOffset; i++) result.Add(ReadMaterializedString($"{path}#{i}"));
            return result;
        });

    /// <summary>
    /// Fills an admitted string-array dictionary from a promoted NameValueCollection source.<br/>
    /// NVC keys are implicitly strings, so no ordinary dictionary key descriptor is read from this header.<br/>
    /// Null value sets stay null; non-null sets allocate only their output array and use marker-aware string reads.<br/>
    /// Existing destination identity, comparer and Add collision semantics are preserved; insertion occurs after each value completes.<br/>
    /// </summary>
    /// <param name="entry">Borrowed resolved NVC header; ValueOffset is the encoded key count.<br/></param>
    /// <param name="destination">Already created or developer-supplied generic dictionary, never replaced here.<br/></param>
    /// <param name="options">Optional explicit decoder policy; native arrays retain their counted loop.<br/></param>
    private void FillNameValueArrays(NameHeaderEntry entry, IDictionary<string, string[]?> destination, DeserializationOptions? options = null)
    {
        if (entry.ValueOffset == 0) return;
        var child = entry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
        for (int i = 0; i < entry.ValueOffset; i++)
        {
            string[]? values = null;
            if (child.Kind == InhetoKindMasks.InhetoPromotedType && child.Type.PromotedType == InhetoPromotedTypes.Array)
            {
                values = new string[child.ValueOffset];
                string path = child.PropPath;
                for (int j = 0; j < values.Length; j++) values[j] = ReadMaterializedString($"{path}#{j}")!;
            }
            else if (child.Kind != InhetoKindMasks.InhetoBaseType || child.Type.BaseType != InhetoBaseTypes.Null)
            {
                var decoded = ReadNameValueSet(child, options);
                values = decoded as string[] ?? (decoded as List<string>)?.ToArray();
            }
            destination.Add(child.CleanName, values);
            child = child.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
        }
    }

    /// <summary>
    /// Fills the admitted string-list dictionary from NVC keys and their encoded value sets.<br/>
    /// Uses the known value count as list capacity and preserves Null children without creating empty replacements.<br/>
    /// String markers are interpreted by the existing reader; no per-element reflection, new cache or delegate is introduced.<br/>
    /// The typed Add call matches branch admission, including dictionaries without nongeneric IDictionary support.<br/>
    /// </summary>
    /// <param name="entry">Borrowed resolved NVC header, not an ordinary dictionary CollectionInfo header.<br/></param>
    /// <param name="destination">Existing destination retaining its comparer, contents and insertion policy.<br/></param>
    /// <param name="options">Optional explicit decoder policy for complete custom or referenced sets.<br/></param>
    private void FillNameValueLists(NameHeaderEntry entry, IDictionary<string, List<string>?> destination, DeserializationOptions? options = null)
    {
        if (entry.ValueOffset == 0) return;
        var child = entry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
        for (int i = 0; i < entry.ValueOffset; i++)
        {
            List<string>? values = null;
            if (child.Kind == InhetoKindMasks.InhetoPromotedType && child.Type.PromotedType == InhetoPromotedTypes.Array)
            {
                values = new List<string>(checked((int)child.ValueOffset));
                string path = child.PropPath;
                for (int j = 0; j < child.ValueOffset; j++) values.Add(ReadMaterializedString($"{path}#{j}")!);
            }
            else if (child.Kind != InhetoKindMasks.InhetoBaseType || child.Type.BaseType != InhetoBaseTypes.Null)
            {
                var decoded = ReadNameValueSet(child, options);
                values = decoded as List<string> ?? (decoded is string[] array ? new List<string>(array) : null);
            }
            destination.Add(child.CleanName, values);
            child = child.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
        }
    }

    /// <summary>
    /// Projects an admitted ordinary scalar collection into a ListString member through existing typed fill loops.<br/>
    /// Reuses the operation's first completed string list; distinct supplied lists retain their contents and identity.<br/>
    /// A registered activator may decline by returning null. Publication occurs only after successful filling.<br/>
    /// Physical entry paths govern element hooks and references; the requested path supplies activation context.<br/>
    /// No native ListString reader, wire layout, global cache or per-element dispatch layer is added.<br/>
    /// </summary>
    /// <param name="entry">Resolved ordinary collection with supported scalar metadata or individually described children.<br/></param>
    /// <param name="path">Requested member path for the developer's activation context.<br/></param>
    /// <param name="destination">Optional developer-owned list, filled additively and never replaced.<br/></param>
    /// <param name="options">Current operation's activation and custom decoding policy.<br/></param>
    /// <returns>The completed list, or null when its activator suppresses rehydration.<br/></returns>
    private List<string>? ReadProjectedStringList(NameHeaderEntry entry, string path, List<string>? destination, DeserializationOptions options)
    {
        List<string>? canonical = null;
        if (refCache is not null && refCache.TryGetValue(entry.Index, out var cached))
            canonical = cached as List<string>;
        if (canonical is not null && (destination is null || ReferenceEquals(destination, canonical)))
            return canonical;

        var result = destination;
        if (result is null)
        {
            result = options.TryGetActivator(typeof(List<string>), out var activate)
                ? (List<string>?)activate(CreateActivatorContext(path))
                : new List<string>(checked((int)entry.ValueOffset));
            if (result is null) return null;
        }
        if (entry.Type.CollectionInfo.ValueType.Kind == InhetoKindMasks.InhetoBaseType &&
            entry.Type.CollectionInfo.ValueType.BaseType == InhetoBaseTypes.ComplexType)
        {
            // Nullable ordinary collections use self-described children in the existing wire format.
            // Read each child's actual marker; never reinterpret the parent's ComplexType as a scalar.
            string physicalPath = entry.PropPath;
            for (int i = 0; i < entry.ValueOffset; i++)
                result.Add(ReadMaterializedAs<string>($"{physicalPath}#{i}", options)!);
        }
        else FillStringList(entry, result, true, true);
        if (canonical is null) RefCache[entry.Index] = result;
        return result;
    }

    /// <summary>
    /// Reads a non-native NVC value-set marker without confusing payload/reference offsets with element counts.<br/>
    /// Native referenced arrays use their physical path, including DuplicateValue, before array validation.<br/>
    /// Custom markers retain existing typed dispatch, physical reference authority and per-path DuplicateValue behavior.<br/>
    /// Returns decoded arrays/lists directly; callers allocate only when their output representation requires conversion.<br/>
    /// Null callback results remain handled values. Missing decoders first expose core bytes; unsupported set conversion throws contextually.<br/>
    /// This cold helper adds no native-array loop work, cache policy, delegate or descriptor ownership.<br/>
    /// </summary>
    /// <param name="child">Borrowed logical key header, possibly a custom marker or reference.<br/></param>
    /// <param name="options">Explicit per-call options, otherwise the current operation or stored options.<br/></param>
    /// <returns>A string array, string list or null; no intermediate collection is constructed here.<br/></returns>
    private object? ReadNameValueSet(NameHeaderEntry child, DeserializationOptions? options)
    {
        var effectiveOptions = options ?? materializationOptions ?? deserializationOptions;
        string path = child.PropPath;
        var resolved = child;
        while (resolved.Kind == InhetoKindMasks.InhetoBaseType &&
            resolved.Type.BaseType is InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
            resolved = NameHeaderEntry.CreateBorrowed(stream, resolved.ValueOffset);
        object? decoded = resolved.Kind == InhetoKindMasks.InhetoPromotedType && resolved.Type.PromotedType == InhetoPromotedTypes.Array
            ? ReadMaterializedAs<string[]>(resolved.PropPath, effectiveOptions)
            : ReadMaterializedAs<object>(path, effectiveOptions);
        if (decoded is null or string[] or List<string>) return decoded;
        if (FastCoerce.TryCoerce(decoded, typeof(string[]), out var converted)) return converted;
        throw new InvalidCastException($"Value set at '{path}' cannot be converted from '{decoded.GetType().FullName}' to a string array or list; supply an applicable deserializer.");
    }

    /// <summary>
    /// Materializes one MemoryStream slot without publishing an intermediate byte array as its referent.<br/>
    /// Native nulls retain their slot; missing children throw. Custom terminals keep current hook dispatch, intentional nulls and decoder-owned instances.<br/>
    /// DuplicateRef shares the operation's stream; DuplicateValue requests a separate stream. No new reference dictionary is created.<br/>
    /// </summary>
    /// <param name="path">Requested element/member path, including reference ancestry and aliases.<br/></param>
    /// <param name="options">Active operation's deserialization options, never captured or cached by this method.<br/></param>
    /// <param name="writable">Capability of a newly created native stream; an existing shared or decoder-owned stream is not replaced.<br/></param>
    /// <param name="missingIsNull">True only for the public low-level reader, which preserves its absent-path null contract even inside a callback.<br/></param>
    /// <returns>The exact shared/decoded stream, a newly reconstructed stream at position zero, or intentional null.<br/></returns>
    private MemoryStream? ReadMaterializedMemoryStream(string path, DeserializationOptions options, bool writable, bool missingIsNull = false)
    {
        var original = GetNameEntryFromPropPath(path, options, out string resolved, cacheResolution: false);
        if (original.IsEmpty)
        {
            if (missingIsNull) return null;
            throw new InvalidDataException($"Missing MemoryStream element at '{path}'.");
        }
        var entry = original;
        bool copy = false;
        while (entry.Kind == InhetoKindMasks.InhetoBaseType &&
            entry.Type.BaseType is InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
        {
            copy |= entry.Type.BaseType == InhetoBaseTypes.DuplicateValue;
            if (entry.ValueOffset == 0 || entry.ValueOffset >= entry.Index)
                throw new InvalidDataException($"Invalid MemoryStream reference at '{path}'.");
            entry = NameHeaderEntry.CreateBorrowed(stream, entry.ValueOffset);
        }
        if (entry.Kind == InhetoKindMasks.InhetoBaseType)
        {
            if (entry.Type.BaseType == InhetoBaseTypes.Null) return null;
            if (entry.Type.BaseType is InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType)
                return ReadMaterializedAs<MemoryStream>(default, original, resolved, options, true, path);
        }
        return ReadOperationMemoryStream(entry, resolved, writable, copy);
    }

    /// <summary>
    /// Reads compatible native byte-family payloads directly into the final stream, retaining one operation-local canonical instance.<br/>
    /// Writable results remain expandable; read-only results preserve typed/low-level reader capability. Both start at position zero.<br/>
    /// A cached stream is returned untouched, including its current position and capabilities. Independent incompatible cached projections are not overwritten.<br/>
    /// DuplicateValue copies bypass sharing. Standalone calls do not publish operation values; the standalone clone helper is unchanged.<br/>
    /// </summary>
    /// <param name="entry">Resolved native ByteArray, MemoryByte, ReadOnlyMemoryByte or MemoryStream header.<br/></param>
    /// <param name="path">Diagnostic path for invalid payloads or incompatible native shapes.<br/></param>
    /// <param name="writable">Whether a newly created stream supports writes and expansion.<br/></param>
    /// <param name="copy">True for a deduplicated value that must not adopt the canonical reference.<br/></param>
    /// <returns>A stream containing the persisted bytes, with no intermediate decoded byte-array cache entry.<br/></returns>
    private MemoryStream ReadOperationMemoryStream(NameHeaderEntry entry, string path, bool writable, bool copy = false)
    {
        if (!(entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType == InhetoBaseTypes.ByteArray) &&
            !(entry.Kind == InhetoKindMasks.InhetoPromotedType && entry.Type.PromotedType is
                InhetoPromotedTypes.MemoryByte or InhetoPromotedTypes.ReadOnlyMemoryByte or InhetoPromotedTypes.MemoryStream))
            throw new InvalidCastException($"Cannot convert '{entry.Type}' at '{path}' to MemoryStream.");
        object? cachedValue = null;
        bool cached = refCache is not null && refCache.TryGetValue(entry.Index, out cachedValue);
        if (!copy && cachedValue is MemoryStream canonical) return canonical;

        stream.Position = entry.ValueOffset;
        int length = stream.Read7BitEncodedInt();
        if (length < 0 || length > stream.Length - stream.Position)
            throw new InvalidDataException($"Invalid MemoryStream payload length at '{path}'.");
        var bytes = stream.Span(checked((int)stream.Position), length);
        MemoryStream result;
        if (writable)
        {
            result = new MemoryStream(length);
            result.Write(bytes);
            result.Position = 0;
        }
        else result = new MemoryStream(bytes.ToArray(), writable: false);
        if (materializationOperationDepth != 0 && !copy && !cached) RefCache[entry.Index] = result;
        return result;
    }

    /// <summary>
    /// Materializes a string-list array element without interpreting a custom payload offset as a native list count.<br/>
    /// Native elements retain one header lookup and the existing operation-local sharing or standalone cloning path.<br/>
    /// Only custom terminals re-enter typed materialization, preserving current codecs, core fallback and intentional null results.<br/>
    /// Re-reading the original path on that custom-only branch retains duplicate-marker and alias semantics.<br/>
    /// Public low-level ReadStringList behavior and native DuplicateValue cloning remain unchanged.<br/>
    /// </summary>
    /// <param name="propPath">Progressive-prefix path of the array element, including its numeric index.<br/></param>
    /// <param name="options">Current caller's decoding policy; never captured in a static dispatch cache.<br/></param>
    /// <returns>The native or developer-decoded list, including an intentional null slot.<br/></returns>
    /// <exception cref="InvalidCastException">Existing core fallback cannot project an undecoded custom payload to a string list.<br/></exception>
    private List<string>? ReadMaterializedStringList(string propPath, DeserializationOptions options)
    {
        var entry = GetNameEntryFromPropPath(propPath);
        if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType is InhetoBaseTypes.Unknown or InhetoBaseTypes.Null)
            return null;
        bool duplicateValue = entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType == InhetoBaseTypes.DuplicateValue;
        if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType is InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
            entry = NameHeaderEntry.CreateBorrowed(stream, entry.ValueOffset, entry.Name);
        if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType is InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType)
            return ReadMaterializedAs<List<string>>(null, propPath, options);
        if (materializationOperationDepth == 0)
            return ReadStandaloneStringList(entry, propPath, null);
        var result = ReadOperationStringList(entry, propPath, null);
        return duplicateValue ? CloneIfMutable(result) : result;
    }
}
