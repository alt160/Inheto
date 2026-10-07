namespace Inheto;

public partial class InhetoBinary
{
    private int materializationOperationDepth;
    private DeserializationOptions? materializationOptions;
    private Stack<Dictionary<uint, object>>? spareMaterializationCaches;

    /// <summary>
    /// Starts one public typed-read, create or fill operation, discarding values from earlier independent calls.<br/>
    /// Ordinary calls reuse the existing dictionary and allocate no operation frame on the heap.<br/>
    /// Reentrant public operations detach the parent cache and rent an empty dictionary when one is available.<br/>
        /// Internal materialization and low-level readers share their caller's cache; public typed reentry is independent.<br/>
        /// Public typed reentry detaches an enclosing physical dictionary-fill anchor so its path starts at the root.<br/>
    /// This is not a thread-safety boundary and does not isolate mutations to shared registration objects.<br/>
    /// </summary>
    /// <param name="options">Per-call options made available to specialized element readers, restored on every exit.<br/></param>
    /// <param name="suppressNameCache">True for bulk create/fill work; false for selected typed reads that retain stable path-to-offset memoization.<br/></param>
    /// <returns>Value-only restoration state for the matching finally block; parent cache and cursor are meaningful only for reentrancy.<br/></returns>
        private (Dictionary<uint, object>? Parent, long Position, bool Nested, object? Root, DeserializationOptions? Options, bool SuppressNameCache, uint LiteralIndex, string? LiteralPath) BeginMaterializationOperation(DeserializationOptions? options = null, bool suppressNameCache = true)
    {
        bool nested = materializationOperationDepth != 0;
        uint priorLiteralIndex = literalReadAnchorIndex;
        string? priorLiteralPath = literalReadAnchorPath;
        if (nested) { literalReadAnchorIndex = 0; literalReadAnchorPath = null; }
        var priorOptions = materializationOptions;
        materializationOptions = options ?? deserializationOptions;
            var parent = nested ? refCache : null;
            object? root = activeComplexRoot;
            activeComplexRoot = null;
        long position = nested ? stream.Position : 0;
        if (nested)
            refCache = spareMaterializationCaches is { Count: > 0 } ? spareMaterializationCaches.Pop() : null;
        else
            refCache?.Clear();
        materializationOperationDepth++;
        if (suppressNameCache) _nameEntryCacheSuppressionDepth++;
        return (parent, position, nested, root, priorOptions, suppressNameCache, priorLiteralIndex, priorLiteralPath);
    }

    /// <summary>
    /// Releases operation values on success or exception while retaining reusable empty dictionary capacity.<br/>
        /// Reentrant operations restore the parent's exact cache and cursor before returning their empty cache to the reader-local pool.<br/>
        /// The previous physical dictionary-fill anchor is restored on success and failure without a heap-allocated frame.<br/>
    /// Pool storage is created only after a reentrant operation actually used a value cache; native descriptor ownership is unchanged.<br/>
    /// </summary>
    /// <param name="state">Restoration state returned by the matching BeginMaterializationOperation call.<br/></param>
        private void EndMaterializationOperation((Dictionary<uint, object>? Parent, long Position, bool Nested, object? Root, DeserializationOptions? Options, bool SuppressNameCache, uint LiteralIndex, string? LiteralPath) state)
    {
        var completed = refCache;
        materializationOptions = state.Options;
        literalReadAnchorIndex = state.LiteralIndex;
        literalReadAnchorPath = state.LiteralPath;
        completed?.Clear();
            if (state.Nested) refCache = state.Parent;
            activeComplexRoot = state.Root;
        materializationOperationDepth--;
        if (state.SuppressNameCache) _nameEntryCacheSuppressionDepth--;
        if (state.Nested)
        {
            stream.Position = state.Position;
            if (completed is not null)
                (spareMaterializationCaches ??= new Stack<Dictionary<uint, object>>()).Push(completed);
        }
    }

    /// <summary>
    /// Initializes shared fill dispatch only when the runtime-Type overload is used.<br/>
    /// Weak keys prevent this cache alone from pinning collectible destination types through their delegates.<br/>
    /// The immutable delegates receive all operation state explicitly; this is not a materialized-value cache.<br/>
    /// </summary>
    private static class FillDispatch
    {
        internal static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Type, Action<InhetoBinary, object, string, DeserializationOptions>> Cache = new();
        private static readonly System.Reflection.MethodInfo OpenFill =
            ((Action<InhetoBinary, object, string, DeserializationOptions>)Fill<object>).Method.GetGenericMethodDefinition();
        private static readonly System.Reflection.MethodInfo OpenBoxedFill =
            ((Action<InhetoBinary, object, string, DeserializationOptions>)FillBoxed<object>).Method.GetGenericMethodDefinition();

        /// <summary>
        /// Binds the existing generic fill entry point for one CLR destination type.<br/>
        /// Only type metadata is closed over; reader, destination, path and options remain invocation arguments.<br/>
        /// Non-nullable value types use the original box for ordinary complex shapes; other shapes retain typed reading.<br/>
        /// Reference and nullable types retain the original cast and direct-call path without reflective invocation per fill.<br/>
        /// </summary>
        /// <param name="type">Developer-selected destination type used to close the generic fill method.<br/></param>
        /// <returns>An immutable dispatch delegate suitable for weak-keyed sharing across independent readers.<br/></returns>
        internal static Action<InhetoBinary, object, string, DeserializationOptions> Create(Type type)
        {
            var method = type.IsValueType && Nullable.GetUnderlyingType(type) is null ? OpenBoxedFill : OpenFill;
            return method.MakeGenericMethod(type).CreateDelegate<Action<InhetoBinary, object, string, DeserializationOptions>>();
        }

        /// <summary>
        /// Adapts an object-typed supplied destination to the existing strongly typed fill operation.<br/>
        /// The ordinary CLR cast preserves reference/value-type behavior and exceptions without expression compilation.<br/>
        /// No operation state is retained in static storage or captured by this open static delegate.<br/>
        /// </summary>
        /// <typeparam name="T">Developer-selected CLR destination type.<br/></typeparam>
        /// <param name="reader">Reader owning the current payload and operation state.<br/></param>
        /// <param name="instance">Caller-provided destination, cast exactly as in the previous compiled expression.<br/></param>
        /// <param name="path">Encoded property path for this invocation.<br/></param>
        /// <param name="options">Effective options for this invocation, never stored in the dispatch cache.<br/></param>
        private static void Fill<T>(InhetoBinary reader, object instance, string path, DeserializationOptions options)
            => reader.ReadPropOnto((T)instance, path, options);

        /// <summary>
        /// Fills an ordinary complex value shape through the existing object-typed reader so compiled setters mutate the supplied box.<br/>
        /// Validates the requested CLR value type before starting the operation, without unboxing or allocating a replacement box.<br/>
        /// Header lookup occurs once. Scalar, custom, missing and null routes retain the previous typed-reader behavior.<br/>
        /// Existing operation cleanup and nested cache/cursor restoration run even when a member callback throws.<br/>
        /// </summary>
        /// <typeparam name="T">Non-nullable value type selected once by the shared dispatch factory.<br/></typeparam>
        /// <param name="reader">Reader owning the payload and operation state.<br/></param>
        /// <param name="instance">Original caller-owned box, never replaced by this adapter.<br/></param>
        /// <param name="path">Requested encoded path, also retained for property-path hooks.<br/></param>
        /// <param name="options">Current discovery, activation and decoding policy.<br/></param>
        private static void FillBoxed<T>(InhetoBinary reader, object instance, string path, DeserializationOptions options)
        {
            if (instance is not T) throw new InvalidCastException($"Supplied box does not contain '{typeof(T)}'.");
            var operation = reader.BeginMaterializationOperation(options);
            try
            {
                var entry = reader.GetNameEntryFromPropPath(path, options, out string resolved);
                var terminal = entry;
                if (!terminal.IsEmpty && terminal.Kind == InhetoKindMasks.InhetoBaseType &&
                    terminal.Type.BaseType is InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
                    terminal = NameHeaderEntry.CreateBorrowed(reader.stream, terminal.ValueOffset, terminal.Name);
                if (!terminal.IsEmpty && terminal.Kind == InhetoKindMasks.InhetoBaseType && terminal.Type.BaseType == InhetoBaseTypes.ComplexType)
                    reader.ReadMaterializedAs<object>(instance, entry, resolved, options, true, path);
                else
                    reader.ReadMaterializedAs<T>((T)instance, entry, resolved, options, true, path);
            }
            finally { reader.EndMaterializationOperation(operation); }
        }
    }

    /// <summary>
    /// Materializes one requested shape in an independent operation using the current deserialization options.<br/>
    /// Recursive internal reads share references within this result; earlier public reads cannot supply cached outputs.<br/>
    /// A public read made by a developer callback gets its own value cache and restores the caller's cache, options and cursor.<br/>
    /// Finally cleanup releases retained result references, including intentional null results, without allocating a heap frame.<br/>
    /// Stable path-to-offset memoization remains reusable; it contains no decoded value or developer option instance.<br/>
    /// </summary>
    /// <typeparam name="T">Developer-selected destination shape, handled by current hooks or Inheto core conversion.<br/></typeparam>
    /// <param name="propPath">Progressive property path to materialize; an empty path selects the root.<br/></param>
    /// <param name="dzOptions">Per-call registrations, or this reader's configured options when omitted.<br/></param>
    /// <returns>The requested value, including an intentional null; unsupported conversion retains its contextual exception.<br/></returns>
    public T? ReadPropAs<T>(string propPath, DeserializationOptions? dzOptions = null)
    {
        var operation = BeginMaterializationOperation(dzOptions, suppressNameCache: false);
        try { return ReadMaterializedAs<T>(propPath, dzOptions); }
        finally { EndMaterializationOperation(operation); }
    }

    /// <summary>
    /// Materializes a runtime-selected shape in one independent operation, equivalent to the generic typed-read entry point.<br/>
    /// Reuses the existing compiled type dispatch, which stores no reader, option instance or materialized result.<br/>
    /// Internal dispatch does not open another operation; callback-initiated public reads are isolated and restored on every exit.<br/>
    /// </summary>
    /// <param name="returnType">Developer-selected CLR destination type used by the cached typed dispatch.<br/></param>
    /// <param name="propPath">Progressive property path to materialize; an empty path selects the root.<br/></param>
    /// <param name="dzOptions">Per-call registrations, or this reader's configured options when omitted.<br/></param>
    /// <returns>The requested value or intentional null, preserving existing conversion and exception semantics.<br/></returns>
    public object? ReadPropAs(Type returnType, string propPath, DeserializationOptions? dzOptions = null)
    {
        var operation = BeginMaterializationOperation(dzOptions, suppressNameCache: false);
        try { return ReadMaterializedAs(returnType, propPath, dzOptions); }
        finally { EndMaterializationOperation(operation); }
    }

    /// <summary>
    /// Eagerly reads typed enumerable elements in one independent operation, preserving shared element references.<br/>
    /// Separate calls use current options and independent decoded values; public callback reentry restores its parent state.<br/>
    /// A supplied destination retains its identity and existing contents; missing/null nodes still return null.<br/>
    /// Cleanup runs on every exit without a heap-allocated operation frame.<br/>
    /// </summary>
    /// <typeparam name="T">Developer-selected destination element type.<br/></typeparam>
    /// <param name="propPath">Progressive property path; empty selects the root collection.<br/></param>
    /// <param name="ontoThisInstance">Optional enumerable to fill through its existing mutator.<br/></param>
    /// <param name="dzOptions">Per-call registrations, or reader defaults when omitted.<br/></param>
    /// <returns>The completed or supplied enumerable, or null for a missing/null node.<br/></returns>
    public IEnumerable<T>? ReadEnumerable<T>(string propPath, IEnumerable<T>? ontoThisInstance = null, DeserializationOptions? dzOptions = null)
    {
        var operation = BeginMaterializationOperation(dzOptions, suppressNameCache: false);
        try
        {
            var result = ReadMaterializedEnumerable(propPath, ontoThisInstance, dzOptions);
            return result is null ? null : ontoThisInstance ?? result;
        }
        finally { EndMaterializationOperation(operation); }
    }

    /// <summary>
    /// Eagerly reads binary-shape-selected enumerable elements in one independent materialization operation.<br/>
    /// Shared elements reuse one referent during the call, while later calls and callback reentry remain independent.<br/>
    /// Preserves a supplied enumerable's identity and existing contents; success, null and failure all release operation state.<br/>
    /// </summary>
    /// <param name="propPath">Progressive property path; empty selects the root collection.<br/></param>
    /// <param name="ontoThisInstance">Optional destination filled through its existing mutator.<br/></param>
    /// <param name="dzOptions">Per-call registrations, or reader defaults when omitted.<br/></param>
    /// <returns>The completed or supplied enumerable, or null for a missing/null node.<br/></returns>
    public System.Collections.IEnumerable? ReadEnumerable(string propPath, System.Collections.IEnumerable? ontoThisInstance = null, DeserializationOptions? dzOptions = null)
    {
        var operation = BeginMaterializationOperation(dzOptions, suppressNameCache: false);
        try
        {
            var result = ReadMaterializedEnumerable(propPath, ontoThisInstance, dzOptions);
            return result is null ? null : ontoThisInstance ?? result;
        }
        finally { EndMaterializationOperation(operation); }
    }
}
