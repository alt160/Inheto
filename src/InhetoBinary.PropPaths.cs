using System.Collections.Concurrent;

namespace Inheto
{
    /// <summary>
    /// Identifies the outcome of checking a property path against a declared CLR root type.<br/>
    /// Type checks never invoke a property getter or method; they inspect metadata and prepare canonical path information only.<br/>
    /// </summary>
    public enum PropPathCheckKind
    {
        /// <summary>
        /// The requested path/check is not valid for this operation.<br/>
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// The declared CLR path and requested result type can be resolved without executing developer code.<br/>
        /// </summary>
        Resolvable = 1
    }

    /// <summary>Identifies why a CLR property path could not be prepared.<br/></summary>
    public enum PropPathFailureKind
    {
        /// <summary>
        /// No structural path-resolution failure.<br/>
        /// </summary>
        None = 0,
        /// <summary>
        /// The path contains no admitted member segment.<br/>
        /// </summary>
        EmptyPath = 1,
        /// <summary>
        /// A required CLR member could not be resolved.<br/>
        /// </summary>
        MissingMember = 2,
        /// <summary>
        /// A method segment is outside the admitted parameterless-method policy.<br/>
        /// </summary>
        BlockedMethod = 3,
        /// <summary>
        /// The resolved member result is incompatible with the requested result type.<br/>
        /// </summary>
        ResultTypeMismatch = 4
    }

    /// <summary>Identifies how one serialized Inheto payload satisfies a prepared property path.<br/></summary>
    public enum PropPathBlobMatchKind
    {
        /// <summary>
        /// The requested path/check is not valid for this operation.<br/>
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// The complete path resolves directly to an encoded value.<br/>
        /// </summary>
        ExactSerializedValue = 1,
        /// <summary>
        /// An encoded prefix exists and the remaining path must execute against its materialized CLR value.<br/>
        /// </summary>
        SerializedPrefixWithClrContinuation = 2,
        /// <summary>
        /// The encoded source required for the path is absent.<br/>
        /// </summary>
        SerializedSourceMissing = 3,
        /// <summary>
        /// The encoded source required for continuation is explicitly null.<br/>
        /// </summary>
        SerializedSourceNull = 4
    }

    /// <summary>Identifies the outcome of executing a prepared property path against one serialized payload.<br/></summary>
    public enum PropPathReadState
    {
        /// <summary>
        /// The requested path/check is not valid for this operation.<br/>
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// Execution produced a non-null result value.<br/>
        /// </summary>
        Value = 1,
        /// <summary>
        /// Execution produced a null result value, distinct from a missing source or blocked continuation.<br/>
        /// </summary>
        Null = 2,
        /// <summary>
        /// The encoded source required for the read is absent.<br/>
        /// </summary>
        SourceMissing = 3,
        /// <summary>
        /// A null source prevents execution of the remaining CLR continuation.<br/>
        /// </summary>
        SourceNull = 4,
        /// <summary>
        /// Executing a CLR continuation failed; inspect the associated read outcome for failure information.<br/>
        /// </summary>
        InvocationFailed = 5
    }

    /// <summary>
    /// Describes a nonexecuting CLR property-path check and acts as the immutable plan reused by payload reads.<br/>
    /// The plan contains no record-specific state and can therefore be cached and shared safely across serialized objects.<br/>
    /// </summary>
    public sealed class PropPathCheck
    {
        internal PropPathCheck(Type rootType, Type? expectedResultType, string requestedPath, string canonicalPath, string serializedPrefixCandidate, string clrRemainderCandidate, Type? resultType, PropPathCheckKind kind, PropPathFailureKind failureKind, string? failedSegment, bool containsMethodInvocation, PreparedInhetoPath? serializedPath)
        {
            RootType = rootType;
            ExpectedResultType = expectedResultType;
            RequestedPath = requestedPath;
            CanonicalPath = canonicalPath;
            SerializedPrefixCandidate = serializedPrefixCandidate;
            ClrRemainderCandidate = clrRemainderCandidate;
            ResultType = resultType;
            Kind = kind;
            FailureKind = failureKind;
            FailedSegment = failedSegment;
            ContainsMethodInvocation = containsMethodInvocation;
            SerializedPath = serializedPath;
        }

        /// <summary>Gets the declared CLR type from which resolution begins.<br/></summary>
        public Type RootType { get; }
        /// <summary>Gets the optional result type requested by the caller.<br/></summary>
        public Type? ExpectedResultType { get; }
        /// <summary>Gets the caller-supplied property path.<br/></summary>
        public string RequestedPath { get; }
        /// <summary>Gets the canonical path, with method segments carrying <c>()</c> and property or field segments carrying only their member name.<br/></summary>
        public string CanonicalPath { get; }
        /// <summary>Gets the longest statically identifiable path expected to be stored directly before CLR continuation begins.<br/></summary>
        public string SerializedPrefixCandidate { get; }
        /// <summary>Gets the statically identifiable CLR continuation after <see cref="SerializedPrefixCandidate"/>.<br/></summary>
        public string ClrRemainderCandidate { get; }
        /// <summary>Gets the CLR type produced by the final resolved member, or <see langword="null"/> when resolution failed.<br/></summary>
        public Type? ResultType { get; }
        /// <summary>Gets the structural resolution outcome.<br/></summary>
        public PropPathCheckKind Kind { get; }
        /// <summary>Gets the structural failure classification.<br/></summary>
        public PropPathFailureKind FailureKind { get; }
        /// <summary>Gets the first member segment that could not be resolved or admitted.<br/></summary>
        public string? FailedSegment { get; }
        /// <summary>Gets whether at least one path segment invokes a parameterless CLR method.<br/></summary>
        public bool ContainsMethodInvocation { get; }
        /// <summary>Gets whether the declared path necessarily continues through CLR members after reaching a serialized value boundary.<br/></summary>
        public bool RequiresClrContinuation => ClrRemainderCandidate.Length != 0;
        /// <summary>Gets whether the complete path and requested result type are structurally valid.<br/></summary>
        public bool IsValid => Kind == PropPathCheckKind.Resolvable;

        /// <summary>Gets the shared physical serialized-path plan used by payload reader loops.<br/></summary>
        internal PreparedInhetoPath? SerializedPath { get; }
    }

    /// <summary>Describes how one Inheto payload maps onto a prepared CLR property path without invoking its CLR continuation.<br/></summary>
    public readonly struct PropPathBlobCheck
    {
        internal PropPathBlobCheck(PropPathCheck path, PropPathBlobMatchKind kind, string serializedPrefix, string clrRemainder, uint entryIndex = 0)
        {
            Path = path;
            Kind = kind;
            SerializedPrefix = serializedPrefix;
            ClrRemainder = clrRemainder;
            EntryIndex = entryIndex;
        }

        /// <summary>Gets the prepared type-level path used for this payload check.<br/></summary>
        public PropPathCheck Path { get; }
        /// <summary>Gets how this payload satisfies or fails to satisfy the prepared path.<br/></summary>
        public PropPathBlobMatchKind Kind { get; }
        /// <summary>Gets the deepest path stored directly in the Inheto name table.<br/></summary>
        public string SerializedPrefix { get; }
        /// <summary>Gets the canonical CLR continuation remaining after the serialized prefix.<br/></summary>
        public string ClrRemainder { get; }
        /// <summary>Gets whether this payload can attempt a value read without resolving additional metadata.<br/></summary>
        public bool CanRead => Kind is PropPathBlobMatchKind.ExactSerializedValue or PropPathBlobMatchKind.SerializedPrefixWithClrContinuation;

        /// <summary>Gets the resolved payload-local names-header byte offset, or zero when no serialized source exists.<br/></summary>
        internal uint EntryIndex { get; }
    }

    public partial class InhetoBinary
    {
        private static readonly ConcurrentDictionary<(RuntimeTypeHandle Root, RuntimeTypeHandle Expected, string Path), PropPathCheck> _propPathChecks = new();

        /// <summary>
        /// Checks and canonicalizes a property, field, or admitted parameterless-method path against a declared CLR type without invoking developer code.<br/>
        /// The returned immutable result is cached by root type, expected result type, and requested path so callers may reuse it as an execution plan.<br/>
        /// </summary>
        /// <param name="rootType">Declared CLR type from which the path begins.<br/></param>
        /// <param name="propPath">Dotted path whose leading dot is optional and whose method segments may include or omit <c>()</c>.<br/></param>
        /// <param name="expectedResultType">Optional result type that the final member must produce.<br/></param>
        /// <returns>A nonexecuting structural check and reusable path plan.<br/></returns>
        public static PropPathCheck CheckPropPath(Type rootType, string propPath, Type? expectedResultType = null)
        {
            ArgumentNullException.ThrowIfNull(rootType);
            propPath ??= string.Empty;
            RuntimeTypeHandle expectedHandle = expectedResultType?.TypeHandle ?? default;
            return _propPathChecks.GetOrAdd(
                (rootType.TypeHandle, expectedHandle, propPath),
                static key => BuildPropPathCheck(Type.GetTypeFromHandle(key.Root)!, key.Expected.Equals(default(RuntimeTypeHandle)) ? null : Type.GetTypeFromHandle(key.Expected), key.Path));
        }

        /// <summary>
        /// Checks and canonicalizes a property path against <typeparamref name="TRoot"/> and requires a result compatible with <typeparamref name="TValue"/>.<br/>
        /// No property getter or method is invoked while checking the path.<br/>
        /// </summary>
        /// <typeparam name="TRoot">Declared CLR root type from which member resolution begins.<br/></typeparam>
        /// <typeparam name="TValue">Required final result type, with nullable wrappers compared by their underlying value type.<br/></typeparam>
        /// <param name="propPath">Dotted path whose leading dot and parameterless-method parentheses are optional.<br/></param>
        /// <returns>A cached immutable structural check suitable for repeated blob reads.<br/></returns>
        public static PropPathCheck CheckPropPath<TRoot, TValue>(string propPath)
            => CheckPropPath(typeof(TRoot), propPath, typeof(TValue));

        /// <summary>
        /// Checks how this serialized payload satisfies a previously prepared property path without invoking its CLR continuation.<br/>
        /// Exact name-table matches are distinguished from deepest-prefix matches, missing sources, and explicit null sources.<br/>
        /// </summary>
        /// <param name="path">Previously validated type-level path plan.<br/></param>
        /// <returns>A payload-specific match that identifies the stored prefix and remaining CLR suffix without executing either.<br/></returns>
        public PropPathBlobCheck CheckPropPath(PropPathCheck path)
        {
            using var rooted = new LiteralValueReadScope(this, 0, null);
            ArgumentNullException.ThrowIfNull(path);
            if (!path.IsValid)
                return new PropPathBlobCheck(path, PropPathBlobMatchKind.Invalid, string.Empty, path.CanonicalPath);

            NameHeaderEntry exact = path.SerializedPath is not null && !path.RequiresClrContinuation
                ? GetNameEntryFromPropPath(path.SerializedPath.Value)
                : GetNameEntryFromPropPath(path.CanonicalPath);
            if (!exact.IsEmpty)
            {
                bool exactNull = exact.Kind == InhetoKindMasks.InhetoBaseType && exact.Type.BaseType == InhetoBaseTypes.Null;
                return new PropPathBlobCheck(path, exactNull ? PropPathBlobMatchKind.SerializedSourceNull : PropPathBlobMatchKind.ExactSerializedValue, path.CanonicalPath, string.Empty, exact.Index);
            }

            var partial = GetClosestNameEntry(path.CanonicalPath);
            if (partial.Entry is null || string.IsNullOrEmpty(partial.Entry.Value.PropPath))
                return new PropPathBlobCheck(path, PropPathBlobMatchKind.SerializedSourceMissing, string.Empty, path.CanonicalPath);

            NameHeaderEntry prefix = partial.Entry.Value;
            bool prefixNull = prefix.Kind == InhetoKindMasks.InhetoBaseType && prefix.Type.BaseType == InhetoBaseTypes.Null;
            return new PropPathBlobCheck(path, prefixNull ? PropPathBlobMatchKind.SerializedSourceNull : PropPathBlobMatchKind.SerializedPrefixWithClrContinuation, prefix.PropPath, partial.RemainingPath, prefix.Index);
        }

        /// <summary>
        /// Checks one prepared logical path against this payload while honoring read-time property aliases.<br/>
        /// The returned serialized prefix is always the physical path present in this payload.<br/>
        /// </summary>
        private PropPathBlobCheck CheckPropPath(PropPathCheck path, DeserializationOptions? dzOptions)
        {
            if (dzOptions is null ||
                !dzOptions.Aliases.TryGetGroup(path.CanonicalPath, out _, out _))
                return CheckPropPath(path);
            if (!path.IsValid)
                return new PropPathBlobCheck(path, PropPathBlobMatchKind.Invalid, string.Empty, path.CanonicalPath);

            var resolved = GetClosestAliasedNameEntry(path.CanonicalPath, dzOptions);
            if (resolved.Entry is null || string.IsNullOrEmpty(resolved.Entry.Value.PropPath))
                return new PropPathBlobCheck(path, PropPathBlobMatchKind.SerializedSourceMissing, string.Empty, path.CanonicalPath);

            NameHeaderEntry entry = resolved.Entry.Value;
            bool isNull = entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType == InhetoBaseTypes.Null;
            if (isNull)
                return new PropPathBlobCheck(path, PropPathBlobMatchKind.SerializedSourceNull, entry.PropPath, resolved.RemainingPath, entry.Index);
            return new PropPathBlobCheck(
                path,
                resolved.RemainingPath.Length == 0
                    ? PropPathBlobMatchKind.ExactSerializedValue
                    : PropPathBlobMatchKind.SerializedPrefixWithClrContinuation,
                entry.PropPath,
                resolved.RemainingPath,
                entry.Index);
        }

        /// <summary>
        /// Executes a prepared property path against this serialized payload while preserving missing, null, invalid, and invocation-failure states.<br/>
        /// The deepest serialized prefix is materialized directly and only the unresolved CLR suffix is executed through Inheto's cached accessor machinery.<br/>
        /// </summary>
        /// <typeparam name="TValue">Expected result type of the prepared path.<br/></typeparam>
        /// <param name="path">Cached type-level path plan to execute against this payload.<br/></param>
        /// <param name="value">Receives the resolved value when the returned state is <see cref="PropPathReadState.Value"/>.<br/></param>
        /// <param name="dzOptions">Optional deserialization registrations used while materializing the stored prefix.<br/></param>
        /// <param name="throwOnInvocationError">Whether getter, method, conversion, and materialization failures should propagate instead of returning <see cref="PropPathReadState.InvocationFailed"/>.<br/></param>
        /// <returns>The exact value, null, missing-source, invalid-plan, or invocation-failure outcome.<br/></returns>
        public PropPathReadState TryReadPropPath<TValue>(PropPathCheck path, out TValue? value, DeserializationOptions? dzOptions = null, bool throwOnInvocationError = false)
        {
            using var rooted = new LiteralValueReadScope(this, 0, null);
            value = default;
            if (TryReadCompiledInt32PropPath(path, dzOptions, out value, out PropPathReadState compiledState))
                return compiledState;

            PropPathBlobCheck blob = CheckPropPath(path, dzOptions);
            if (blob.Kind == PropPathBlobMatchKind.Invalid) return PropPathReadState.Invalid;
            if (blob.Kind == PropPathBlobMatchKind.SerializedSourceMissing) return PropPathReadState.SourceMissing;
            if (blob.Kind == PropPathBlobMatchKind.SerializedSourceNull) return PropPathReadState.SourceNull;

            try
            {
                if (blob.Kind == PropPathBlobMatchKind.ExactSerializedValue)
                {
                    NameHeaderEntry entry = NameHeaderEntry.CreateBorrowed(stream, blob.EntryIndex);
                    value = ReadMaterializedAs<TValue>(default, entry, blob.SerializedPrefix, dzOptions!, true, path.CanonicalPath);
                }
                else
                {
                    object? source = ReadPropInternal(blob.SerializedPrefix, dzOptions);
                    if (source is null) return PropPathReadState.SourceNull;
                    value = ValuePathRetriever.GetValue<TValue>(source, blob.ClrRemainder, throwOnError: true);
                }

                return value is null ? PropPathReadState.Null : PropPathReadState.Value;
            }
            catch when (!throwOnInvocationError)
            {
                value = default;
                return PropPathReadState.InvocationFailed;
            }
        }

        private static PropPathCheck BuildPropPathCheck(Type rootType, Type? expectedResultType, string requestedPath)
        {
            if (!ValuePathRetriever.TryCheckPath(rootType, requestedPath, out string canonicalPath, out string serializedPrefix, out string clrRemainder, out Type? resultType, out bool containsMethod, out PropPathFailureKind failure, out string? failedSegment))
                return new PropPathCheck(rootType, expectedResultType, requestedPath, canonicalPath, serializedPrefix, clrRemainder, resultType, PropPathCheckKind.Invalid, failure, failedSegment, containsMethod, null);

            Type normalizedResult = Nullable.GetUnderlyingType(resultType!) ?? resultType!;
            Type? normalizedExpected = expectedResultType is null ? null : Nullable.GetUnderlyingType(expectedResultType) ?? expectedResultType;
            if (normalizedExpected is not null && !normalizedExpected.IsAssignableFrom(normalizedResult) && normalizedExpected != normalizedResult)
                return new PropPathCheck(rootType, expectedResultType, requestedPath, canonicalPath, serializedPrefix, clrRemainder, resultType, PropPathCheckKind.Invalid, PropPathFailureKind.ResultTypeMismatch, null, containsMethod, null);

            PreparedInhetoPath preparedPath = GetPreparedPath(
                clrRemainder.Length == 0 ? canonicalPath : serializedPrefix,
                prepareDynamicPath: true);
            return new PropPathCheck(rootType, expectedResultType, requestedPath, canonicalPath, serializedPrefix, clrRemainder, resultType, PropPathCheckKind.Resolvable, PropPathFailureKind.None, null, containsMethod, preparedPath);
        }
    }
}
