using Fasterflect;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Inheto
{
    public partial class InhetoSerializer
    {
        // ====== TYPE SAFETY PROBING ======
        #region Developer compatibility diagnostics

        /// <summary>Identifies a serialization compatibility hazard or a limit reached during a bounded runtime-object probe.<br/></summary>
        public enum TestTypeIssueKind : byte
        {
            /// <summary>The supplied root was null, so no object graph was traversed.<br/></summary>
            NullRoot,
            /// <summary>A branch exceeded the configured maximum traversal depth.<br/></summary>
            DepthLimitReached,
            /// <summary>The probe reached its configured maximum visited-node count.<br/></summary>
            NodeLimitReached,

            /// <summary>A member getter failed, or the probe identified a member whose accessor is unsafe or unsupported.<br/></summary>
            GetterThrows,
            /// <summary>Enumerable or dictionary traversal threw an exception.<br/></summary>
            EnumerationThrows,

            /// <summary>A registered custom serializer threw when the probe invoked it.<br/></summary>
            CustomSerializerThrows,
            /// <summary>A custom serializer returned an unsupported payload kind: type hooks require bytes or null; member hooks also permit text.<br/></summary>
            CustomSerializerReturnedNonBytes,

            /// <summary>An unrecognized value type has no public instance members through which its value can be preserved.<br/></summary>
            OpaqueValueType,
            /// <summary>A dictionary key cannot be represented safely by the ordinary dictionary-key path.<br/></summary>
            UnsafeDictionaryKey,

            /// <summary>A getter or recursive traversal produced fresh references in a pattern that may expand without bound.<br/></summary>
            PotentialInfiniteExpansion,
        }

        /// <summary>Describes one compatibility observation without changing serialization options or persisting a record.<br/></summary>
        /// <param name="Kind">Hazard or traversal-limit classification.<br/></param>
        /// <param name="MemberPath">Inheto path at which the observation occurred; empty denotes the root.<br/></param>
        /// <param name="DeclaringOrOwningType">Type declaring the member or owning the affected graph value.<br/></param>
        /// <param name="ValueType">Observed or declared value type when available.<br/></param>
        /// <param name="Message">Human-readable explanation of the observation.<br/></param>
        /// <param name="Exception">Captured exception when the observation resulted from a failed callback or traversal.<br/></param>
        public readonly record struct TestTypeIssue(
            TestTypeIssueKind Kind,
            string MemberPath,
            Type DeclaringOrOwningType,
            Type? ValueType,
            string Message,
            Exception? Exception = null
        );

        /// <summary>Identifies a configuration action that the developer may choose after inspecting a compatibility report.<br/></summary>
        public enum TestTypeSuggestionKind : byte
        {
            /// <summary>Consider excluding the indicated member from serialization.<br/></summary>
            ExcludeMember,
            /// <summary>Consider excluding the indicated non-root runtime type.<br/></summary>
            ExcludeType,
            /// <summary>Consider explicit circular-member handling for the indicated member token and type.<br/></summary>
            AddCircularProperty,
            /// <summary>Consider a member-specific custom serializer to control the affected value.<br/></summary>
            AddMemberSerializer,
            /// <summary>Consider a type-wide custom serializer to preserve an unsupported value representation.<br/></summary>
            AddTypeSerializer,
            /// <summary>Use the writer's exact member-token convention when registering member policies.<br/></summary>
            NormalizeMemberNameToken, // e.g. "Root" -> ".Root"
        }

        /// <summary>Provides optional guidance; the probe does not apply configuration changes automatically.<br/></summary>
        /// <param name="Kind">Suggested action classification.<br/></param>
        /// <param name="Target">Member path, type name, or configuration target described by the suggestion.<br/></param>
        /// <param name="Message">Human-readable guidance to review before applying any change.<br/></param>
        public readonly record struct TestTypeSuggestion(
            TestTypeSuggestionKind Kind,
            string Target,
            string Message
        );

        /// <summary>Returns the observations from one bounded compatibility probe; absence of issues is not proof that every possible runtime graph will serialize safely.<br/></summary>
        /// <param name="RootType">Declared generic root type used for this probe.<br/></param>
        /// <param name="NodesVisited">Number of values recorded as visited by the traversal.<br/></param>
        /// <param name="Issues">Collected compatibility hazards and traversal-limit observations.<br/></param>
        /// <param name="Suggestions">Deduplicated configuration guidance; changes remain the developer's decision.<br/></param>
        public sealed record TestTypeReport(
            Type RootType,
            int NodesVisited,
            IReadOnlyList<TestTypeIssue> Issues,
            IReadOnlyList<TestTypeSuggestion> Suggestions
        );

        /// <summary>
        /// Probes a runtime object graph for compatibility with this serializer's current member and custom-hook policies.<br/>
        /// Reports throwing or unsupported getters, enumeration failures, unsafe dictionary keys, opaque values and unstable recursive expansion.<br/>
        /// Does not serialize a payload, persist a record or change the configured serialization policy.<br/>
        /// Probing may invoke getters, enumerate collections and, by default, invoke registered custom serializers; their side effects are not rolled back.<br/>
        /// Depth, node and enumeration limits bound inspection; a clean report is guidance, not a guarantee for unvisited values or later mutations.<br/>
        /// This is an explicit developer diagnostic, not part of the normal serialization hot path or a concurrency boundary.<br/>
        /// </summary>
        /// <typeparam name="T">Declared root type reported alongside the inspected runtime graph.<br/></typeparam>
        /// <param name="instance">Runtime graph to inspect; a null root returns a NullRoot issue without traversal.<br/></param>
        /// <param name="probeOptions">Optional traversal bounds and callback choices; null uses the documented defaults.<br/></param>
        /// <returns>A report of inspected values, compatibility observations and optional configuration suggestions.<br/></returns>
        public TestTypeReport TestType<T>(T instance, TestTypeProbeOptions? probeOptions = null)
        {
            var opts = probeOptions ?? new TestTypeProbeOptions();

            var issues = new List<TestTypeIssue>(64);
            var suggestions = new List<TestTypeSuggestion>(64);

            if (instance is null)
            {
                issues.Add(new TestTypeIssue(
                    TestTypeIssueKind.NullRoot,
                    MemberPath: "",
                    DeclaringOrOwningType: typeof(T),
                    ValueType: typeof(T),
                    Message: "Root instance is null; probing stops here."
                ));

                return new TestTypeReport(typeof(T), 0, issues, suggestions);
            }

            var state = new ProbeState(this, opts, issues, suggestions);
            state.ProbeValue(instance, typeof(T), memberPath: "", propToken: "", depth: 0);

            state.FinalizeSuggestions();

            return new TestTypeReport(typeof(T), state.NodesVisited, issues, suggestions);
        }

        private sealed class ProbeState
        {
            private readonly InhetoSerializer _sz;
            private readonly SerializationOptions _options;
            private readonly TestTypeProbeOptions _probeOptions;
            private readonly List<TestTypeIssue> _issues;
            private readonly List<TestTypeSuggestion> _suggestions;

            // Reference-identity visited set (mirrors serializer's DuplicateRef behavior)
            private readonly HashSet<object> _visitedRefs = new(RefEqualityComparer.Instance);

            // Detect repeated (Type, propToken) patterns that return a NEW instance each time (defeats DuplicateRef)
            private readonly Stack<(RuntimeTypeHandle type, string propToken, object? value)> _chain = new();

            // Dedup suggestions
            private readonly HashSet<string> _suggested = new(StringComparer.Ordinal);

            public int NodesVisited { get; private set; }

            public ProbeState(
                InhetoSerializer serializer,
                TestTypeProbeOptions probeOptions,
                List<TestTypeIssue> issues,
                List<TestTypeSuggestion> suggestions)
            {
                _sz = serializer;
                _options = serializer.options;
                _probeOptions = probeOptions;
                _issues = issues;
                _suggestions = suggestions;
            }

            public void FinalizeSuggestions()
            {
                // Surface the "token formatting" reality explicitly: users tend to pass "Root" vs ".Root".
                // If we suggested AddCircularProperty on ".X", also suggest normalization guidance once.
                AddSuggestionOnce(
                    key: "NormalizeMemberNameToken",
                    kind: TestTypeSuggestionKind.NormalizeMemberNameToken,
                    target: "Member tokens",
                    message: "Member names used by IncludedMembers/ExcludedMembers/AddCircularProperty must match serializer tokens (typically \".Prop\", \"#0\", \"!key\"). Passing \"Root\" won’t match \".Root\"."
                );

            }

            public void ProbeValue(object value, Type declaredType, string memberPath, string propToken, int depth)
            {
                if (NodesVisited >= _probeOptions.MaxNodes)
                {
                    _issues.Add(new TestTypeIssue(
                        TestTypeIssueKind.NodeLimitReached,
                        memberPath,
                        declaredType,
                        value.GetType(),
                        $"Probe stopped: MaxNodes={_probeOptions.MaxNodes} reached."
                    ));
                    return;
                }

                if (depth > _probeOptions.MaxDepth)
                {
                    _issues.Add(new TestTypeIssue(
                        TestTypeIssueKind.DepthLimitReached,
                        memberPath,
                        declaredType,
                        value.GetType(),
                        $"Probe stopped: MaxDepth={_probeOptions.MaxDepth} reached."
                    ));
                    return;
                }

                var runtimeType = value.GetType();

                // Apply the same “root is special” rule: ExcludedTypes doesn’t block root because memberPath == "" in serializer.
                if (memberPath.Length != 0 && _options.ExcludedTypes.Contains(runtimeType))
                {
                    AddSuggestionOnce(
                        key: $"ExcludeType::{runtimeType.AssemblyQualifiedName}",
                        kind: TestTypeSuggestionKind.ExcludeType,
                        target: runtimeType.FullName ?? runtimeType.Name,
                        message: $"Type is excluded by options; serializer will skip it at {memberPath}."
                    );
                    return;
                }

                if (memberPath.Length != 0 && _options.IncludedMembers.Count != 0 && !_options.IncludedMembers.Contains(memberPath))
                    return;

                if (memberPath.Length != 0 && _options.ExcludedMembers.Contains(memberPath))
                    return;

                // Custom member serializer (path-based) takes precedence.
                if (_options.MemberSerializers.TryGetValue(memberPath, out var memberSz))
                {
                    if (_probeOptions.InvokeCustomSerializers)
                        ValidateCustomSerializer(memberSz, value, runtimeType, memberPath, isTypeSerializer: false);

                    NodesVisited++;
                    return;
                }

                // Custom type serializer next.
                if (_options.TypeSerializers.TryGetValue(runtimeType.TypeHandle, out var typeSz))
                {
                    if (_probeOptions.InvokeCustomSerializers)
                        ValidateCustomSerializer(typeSz, value, runtimeType, memberPath, isTypeSerializer: true);

                    NodesVisited++;
                    return;
                }

                // Base / promoted handling (mirrors the serializer’s classification intent)
                if (InhetoBaseTypesMap.TryGetValue(runtimeType.TypeHandle, out var baseType) && baseType != InhetoBaseTypes.Unknown)
                {
                    NodesVisited++;

                    // ListString is special: serializer enumerates it.
                    if (baseType == InhetoBaseTypes.ListString && value is IEnumerable enLs)
                        ProbeEnumerable(enLs, runtimeType, memberPath, depth);

                    return;
                }

                if (runtimeType.IsEnum)
                {
                    NodesVisited++;
                    return;
                }

                if (InhetoPromotedTypesMap.ContainsKey(runtimeType.TypeHandle))
                {
                    NodesVisited++;

                    // Some promoted types still enumerate internally (arrays / IEnumerable)
                    if (value is Array arr)
                    {
                        ProbeArray(arr, runtimeType, memberPath, depth);
                    }
                    else if (value is IEnumerable en && runtimeType != typeof(string))
                    {
                        // Serializer uses a promoted Enumerable path for certain cases; probing still worthwhile.
                        ProbeEnumerable(en, runtimeType, memberPath, depth);
                    }

                    return;
                }

                // Generic collection families
                if (UtilsAndExtensions.GenericAnalyzer.TryAnalyzeForGenericType(runtimeType, out var match))
                {
                    NodesVisited++;

                    if (value is IDictionary dict)
                    {
                        ProbeDictionary(dict, runtimeType, memberPath, depth);
                        return;
                    }

                    if (value is IEnumerable en)
                    {
                        ProbeEnumerable(en, runtimeType, memberPath, depth);
                        return;
                    }
                }

                // Reference tracking (mirrors DuplicateRef behavior): stop revisiting the same instance.
                // IMPORTANT: this does not protect against "new instance per getter call" patterns (DirectoryInfo.Root-like);
                // that is handled via _chain detection below.
                if (!runtimeType.IsValueType && !_visitedRefs.Add(value))
                {
                    NodesVisited++;
                    return;
                }

                // Opaque value types: serializes as ComplexType with no members => data loss.
                // This is a real-world footgun for IntPtr-like structs.
                if (runtimeType.IsValueType &&
                    !runtimeType.IsEnum &&
                    (runtimeType.Properties(Flags.InstancePublic).Count + runtimeType.Fields(Flags.InstancePublic).Count) == 0)
                {
                    _issues.Add(new TestTypeIssue(
                        TestTypeIssueKind.OpaqueValueType,
                        memberPath,
                        declaredType,
                        runtimeType,
                        "Value type has no public instance fields/properties and is not a known base/promoted type; serialization will not capture its value."
                    ));

                    AddSuggestionOnce(
                        key: $"AddTypeSerializer::{runtimeType.AssemblyQualifiedName}",
                        kind: TestTypeSuggestionKind.AddTypeSerializer,
                        target: runtimeType.FullName ?? runtimeType.Name,
                        message: $"Register a TypeSerializer to preserve the struct payload at '{memberPath}', or exclude the member."
                    );

                    NodesVisited++;
                    return;
                }

                // Complex type: probe members using the same MemberTypes selection as GraphPropsAndFields.
                ProbeMembers(value, runtimeType, memberPath, depth);
                NodesVisited++;
            }

            private void ProbeMembers(object obj, Type oType, string basePath, int depth)
            {
                IList<PropertyInfo>? props = null;
                IList<FieldInfo>? fields = null;

                switch (_options.MemberTypes)
                {
                    case MemberTypesEnum.PublicPropertiesOnly:
                        props = oType.Properties(Flags.InstancePublic);
                        break;

                    case MemberTypesEnum.PublicPropertiesAndFields:
                        props = oType.Properties(Flags.InstancePublic);
                        fields = oType.Fields(Flags.InstancePublic);
                        break;

                    case MemberTypesEnum.PublicAndPrivateFields:
                        fields = oType.Fields(Flags.InstancePublic | Flags.InstancePrivate);
                        break;

                    case MemberTypesEnum.PublicFieldsOnly:
                        fields = oType.Fields(Flags.InstancePublic);
                        break;

                    case MemberTypesEnum.PrivateFieldsOnly:
                        fields = oType.Fields(Flags.InstancePrivate);
                        break;

                    default:
                        props = oType.Properties(Flags.InstancePublic);
                        fields = oType.Fields(Flags.InstancePublic);
                        break;
                }

                if (props is not null)
                    foreach (var p in props)
                        ProbeProperty(obj, oType, p, basePath, depth);

                if (fields is not null)
                    foreach (var f in fields)
                        ProbeField(obj, oType, f, basePath, depth);
            }

            private void ProbeProperty(object obj, Type oType, PropertyInfo p, string basePath, int depth)
            {
                // Mirror GraphProperties skip rules.
                if (oType.IsAssignableTo(typeof(IDictionary)) || oType.IsAssignableTo(typeof(ICollection)) || IsSetType(oType))
                {
                    switch (p.Name)
                    {
                        case "Keys":
                        case "IsEmpty":
                        case "AllKeys":
                        case "Values":
                        case "Comparer":
                        case "Item":
                        case "SyncRoot":
                        case "IsReadOnly":
                        case "IsFixedSize":
                        case "IsSynchronized":
                        case "Capacity":
                            return;
                    }
                }
                else if (oType.IsGenericType && oType.GetGenericTypeDefinition() == typeof(LinkedListNode<>))
                {
                    switch (p.Name)
                    {
                        case "List":
                        case "Next":
                        case "Previous":
                        case "Value":
                        case "ValueRef":
                            return;
                    }
                }

                if (obj is Array)
                {
                    switch (p.Name)
                    {
                        case "Rank":
                        case "LongLength":
                        case "Length":
                            return;
                    }
                }

                if (!p.CanRead) return;
                if (p.GetIndexParameters().Length != 0) return;

                var propToken = string.Intern($".{p.Name}");
                var childPath = $"{basePath}{propToken}";
                var valueType = p.PropertyType;

                if (valueType.IsByRefLike)
                {
                    _issues.Add(new TestTypeIssue(
                        TestTypeIssueKind.GetterThrows,
                        childPath,
                        oType,
                        valueType,
                        $"Member {childPath} is byref-like ({valueType.FullName}); accessors cannot be generated. Exclude or custom-serialize this member."
                    ));

                    AddSuggestionOnce(
                        key: $"ByRefLike::{childPath}",
                        kind: TestTypeSuggestionKind.ExcludeMember,
                        target: childPath,
                        message: $"Byref-like member {childPath}; exclude via options.ExcludedMembers.Add(\"{childPath}\") or supply a custom serializer."
                    );

                    return;
                }

                // Known-problematic members: skip probing and emit actionable guidance to avoid first-chance exceptions spam.
                if (oType == typeof(AssemblyName) && string.Equals(p.Name, "KeyPair", StringComparison.Ordinal))
                {
                    _issues.Add(new TestTypeIssue(
                        TestTypeIssueKind.GetterThrows,
                        childPath,
                        oType,
                        valueType,
                        $"Getter skipped: AssemblyName.KeyPair ({childPath}) is not supported on this platform; exclude or custom-serialize this member.",
                        null
                    ));

                    AddSuggestionOnce(
                        key: $"PlatformNotSupported::{childPath}",
                        kind: TestTypeSuggestionKind.ExcludeMember,
                        target: childPath,
                        message: $"AssemblyName.KeyPair ({childPath}) is not supported on this platform; exclude via options.ExcludedMembers.Add(\"{childPath}\") or handle with a custom serializer."
                    );
                    return;
                }

                if (IsProcessHandleMember(p.Name) || IsHandleLike(valueType))
                {
                    _issues.Add(new TestTypeIssue(
                        TestTypeIssueKind.GetterThrows,
                        childPath,
                        oType,
                        valueType,
                        $"Getter skipped: handle-like member {childPath} requires a live OS handle; exclude it or supply a started Process and custom serializer.",
                        null
                    ));

                    AddSuggestionOnce(
                        key: $"HandleLike::{childPath}",
                        kind: TestTypeSuggestionKind.ExcludeMember,
                        target: childPath,
                        message: $"Handle-like member {childPath}; exclude via options.ExcludedMembers.Add(\"{childPath}\") or provide a custom serializer (for Process, supply a started process instance)."
                    );
                    return;
                }

                object? mVal;
                try
                {
                    var acc = GetOrCreateAccessor(oType, p, valueType);
                    mVal = acc.GetValue(obj);
                }
                catch (Exception ex)
                {
                    _issues.Add(new TestTypeIssue(
                        TestTypeIssueKind.GetterThrows,
                        childPath,
                        oType,
                        valueType,
                        $"Getter threw for {childPath}.",
                        ex
                    ));

                    AddSuggestionOnce(
                        key: $"ExcludeMember::{childPath}",
                        kind: TestTypeSuggestionKind.ExcludeMember,
                        target: childPath,
                        message: $"Exclude this member ({childPath}), or register a MemberSerializer to control/avoid getter execution."
                    );

                    if (ex is PlatformNotSupportedException)
                    {
                        AddSuggestionOnce(
                            key: $"PlatformNotSupported::{childPath}",
                            kind: TestTypeSuggestionKind.ExcludeMember,
                            target: childPath,
                            message: $"Member ({childPath}) is not supported on this platform; exclude it or handle with a custom serializer."
                        );
                    }
                    else if ((ex is InvalidOperationException || ex is ObjectDisposedException) && oType == typeof(Process))
                    {
                        AddSuggestionOnce(
                            key: $"ProcessGetter::{childPath}",
                            kind: TestTypeSuggestionKind.ExcludeMember,
                            target: childPath,
                            message: "Process members like Handle/SafeHandle/MainModule require a started process; exclude this member or supply a started Process instance."
                        );
                    }
                    else if (IsHandleLike(valueType) || IsHandleLike(oType))
                    {
                        AddSuggestionOnce(
                            key: $"HandleLike::{childPath}",
                            kind: TestTypeSuggestionKind.ExcludeMember,
                            target: childPath,
                            message: "Handle-like member detected; exclude this member or provide a custom serializer to avoid handle probing."
                        );
                    }

                    return;
                }

                if (mVal is null)
                    return;

                // Optional stability check: call getter twice and compare reference identity.
                if (_probeOptions.ProbeGetterStability && !valueType.IsValueType && !IsSimpleStableType(valueType))
                {
                    if (IsBenignWrapperMember(oType, propToken))
                    {
                        // Known to return a fresh wrapper but not cause unbounded expansion.
                        _chain.Push((oType.TypeHandle, propToken, mVal));
                        try
                        {
                            ProbeValue(mVal, valueType, childPath, propToken, depth + 1);
                        }
                        finally
                        {
                            _chain.Pop();
                        }
                        return;
                    }

                    try
                    {
                        var acc = GetOrCreateAccessor(oType, p, valueType);
                        var mVal2 = acc.GetValue(obj);

                        if (mVal2 is not null &&
                            !ReferenceEquals(mVal, mVal2) &&
                            mVal2.GetType() == mVal.GetType())
                        {
                            var targetType = oType;
                            var targetPropToken = propToken;
                            var targetPath = childPath;

                            if (TryMapFileSystemInfoRootCause(valueType, childPath, out var mappedType, out var mappedToken, out var mappedPath))
                            {
                                targetType = mappedType;
                                targetPropToken = mappedToken;
                                targetPath = mappedPath;
                            }

                            // This is exactly the pattern that defeats DuplicateRef detection if the object is recreated on each getter call.
                            _issues.Add(new TestTypeIssue(
                                TestTypeIssueKind.PotentialInfiniteExpansion,
                                targetPath,
                                targetType,
                                mVal.GetType(),
                                $"Getter appears to return a new instance per call for {targetPath} (same runtime type, different reference). This can cause unbounded expansion unless handled."
                            ));

                            AddSuggestionOnce(
                                key: $"AddCircularProperty::{targetType.AssemblyQualifiedName}::{targetPropToken}",
                                kind: TestTypeSuggestionKind.AddCircularProperty,
                                target: $"{targetType.FullName}{targetPropToken}",
                                message: $"Consider options.AddCircularProperty(typeof({targetType.Name}), \"{targetPropToken}\") at '{targetPath}' (note the leading dot token)."
                            );

                            // Don’t recurse; we’ve already found the dangerous pattern.
                            return;
                        }
                    }
                    catch
                    {
                        // Ignore secondary stability check failures; primary getter exception is handled above.
                    }
                }

                // Chain-based detection: repeated (Type, propToken) along traversal.
                if (TryDetectRepeatedExpansion(oType.TypeHandle, propToken, mVal, childPath, oType))
                {
                    AddSuggestionOnce(
                        key: $"AddCircularProperty::{oType.AssemblyQualifiedName}::{propToken}",
                        kind: TestTypeSuggestionKind.AddCircularProperty,
                        target: $"{oType.FullName}{propToken}",
                        message: $"Repeated member token detected along traversal; consider options.AddCircularProperty(typeof({oType.Name}), \"{propToken}\") or exclude {childPath}."
                    );
                    return;
                }

                _chain.Push((oType.TypeHandle, propToken, mVal));
                try
                {
                    ProbeValue(mVal, valueType, childPath, propToken, depth + 1);
                }
                finally
                {
                    _chain.Pop();
                }
            }

            private void ProbeField(object obj, Type oType, FieldInfo f, string basePath, int depth)
            {
                var fieldToken = string.Intern($".{f.Name}");
                var childPath = $"{basePath}{fieldToken}";
                var valueType = f.FieldType;

                if (oType == typeof(AssemblyName) && string.Equals(f.Name, "KeyPair", StringComparison.Ordinal))
                {
                    _issues.Add(new TestTypeIssue(
                        TestTypeIssueKind.GetterThrows,
                        childPath,
                        oType,
                        valueType,
                        $"Field skipped: AssemblyName.KeyPair ({childPath}) is not supported on this platform; exclude or custom-serialize this member.",
                        null
                    ));

                    AddSuggestionOnce(
                        key: $"PlatformNotSupported::{childPath}",
                        kind: TestTypeSuggestionKind.ExcludeMember,
                        target: childPath,
                        message: $"AssemblyName.KeyPair ({childPath}) is not supported on this platform; exclude via options.ExcludedMembers.Add(\"{childPath}\") or handle with a custom serializer."
                    );
                    return;
                }

                if (IsProcessHandleMember(f.Name) || IsHandleLike(valueType))
                {
                    _issues.Add(new TestTypeIssue(
                        TestTypeIssueKind.GetterThrows,
                        childPath,
                        oType,
                        valueType,
                        $"Field skipped: handle-like member {childPath} requires a live OS handle; exclude it or supply a started Process and custom serializer.",
                        null
                    ));

                    AddSuggestionOnce(
                        key: $"HandleLike::{childPath}",
                        kind: TestTypeSuggestionKind.ExcludeMember,
                        target: childPath,
                        message: $"Handle-like member {childPath}; exclude via options.ExcludedMembers.Add(\"{childPath}\") or provide a custom serializer (for Process, supply a started process instance)."
                    );
                    return;
                }

                object? mVal;
                try
                {
                    if (valueType.IsByRefLike)
                    {
                        _issues.Add(new TestTypeIssue(
                            TestTypeIssueKind.GetterThrows,
                            childPath,
                            oType,
                            valueType,
                            $"Member {childPath} is byref-like ({valueType.FullName}); accessors cannot be generated. Exclude or custom-serialize this member."
                        ));

                        AddSuggestionOnce(
                            key: $"ByRefLike::{childPath}",
                            kind: TestTypeSuggestionKind.ExcludeMember,
                            target: childPath,
                            message: $"Byref-like member {childPath}; exclude via options.ExcludedMembers.Add(\"{childPath}\") or supply a custom serializer."
                        );

                        return;
                    }

                    var acc = GetOrCreateAccessor(oType, f, valueType);
                    mVal = acc.GetValue(obj);
                }
                catch (Exception ex)
                {
                    _issues.Add(new TestTypeIssue(
                        TestTypeIssueKind.GetterThrows,
                        childPath,
                        oType,
                        valueType,
                        $"Field accessor threw for {childPath}.",
                        ex
                    ));

                    AddSuggestionOnce(
                        key: $"ExcludeMember::{childPath}",
                        kind: TestTypeSuggestionKind.ExcludeMember,
                        target: childPath,
                        message: $"Exclude this member ({childPath}), or register a MemberSerializer."
                    );

                    if (ex is PlatformNotSupportedException)
                    {
                        AddSuggestionOnce(
                            key: $"PlatformNotSupported::{childPath}",
                            kind: TestTypeSuggestionKind.ExcludeMember,
                            target: childPath,
                            message: $"Member ({childPath}) is not supported on this platform; exclude it or handle with a custom serializer."
                        );
                    }
                    else if ((ex is InvalidOperationException || ex is ObjectDisposedException) && oType == typeof(Process))
                    {
                        AddSuggestionOnce(
                            key: $"ProcessGetter::{childPath}",
                            kind: TestTypeSuggestionKind.ExcludeMember,
                            target: childPath,
                            message: "Process members like Handle/SafeHandle/MainModule require a started process; exclude this member or supply a started Process instance."
                        );
                    }
                    else if (IsHandleLike(valueType) || IsHandleLike(oType))
                    {
                        AddSuggestionOnce(
                            key: $"HandleLike::{childPath}",
                            kind: TestTypeSuggestionKind.ExcludeMember,
                            target: childPath,
                            message: "Handle-like member detected; exclude this member or provide a custom serializer to avoid handle probing."
                        );
                    }

                    return;
                }

                if (mVal is null)
                    return;

                if (TryDetectRepeatedExpansion(oType.TypeHandle, fieldToken, mVal, childPath, oType))
                {
                    AddSuggestionOnce(
                        key: $"AddCircularProperty::{oType.AssemblyQualifiedName}::{fieldToken}",
                        kind: TestTypeSuggestionKind.AddCircularProperty,
                        target: $"{oType.FullName}{fieldToken}",
                        message: $"Repeated member token detected along traversal; consider options.AddCircularProperty(typeof({oType.Name}), \"{fieldToken}\") or exclude {childPath}."
                    );
                    return;
                }

                _chain.Push((oType.TypeHandle, fieldToken, mVal));
                try
                {
                    ProbeValue(mVal, valueType, childPath, fieldToken, depth + 1);
                }
                finally
                {
                    _chain.Pop();
                }
            }

            private bool TryDetectRepeatedExpansion(RuntimeTypeHandle owningType, string propToken, object mVal, string childPath, Type owningRuntimeType)
            {
                if (IsBenignWrapperMember(owningRuntimeType, propToken))
                    return false;

                // If the same (Type, propToken) exists in the chain AND the value is not reference-equal, it’s a strong signal
                // of "re-creation node" patterns (DirectoryInfo.Root-like) which bypass DuplicateRef.
                foreach (var frame in _chain)
                {
                    if (frame.type.Equals(owningType) && ReferenceEquals(frame.propToken, propToken))
                    {
                        if (frame.value is not null && !ReferenceEquals(frame.value, mVal))
                        {
                            _issues.Add(new TestTypeIssue(
                                TestTypeIssueKind.PotentialInfiniteExpansion,
                                childPath,
                                owningRuntimeType,
                                mVal.GetType(),
                                $"Traversal revisited {owningRuntimeType.Name}{propToken} and observed a different reference instance. This can produce unbounded graphs unless circular handling is configured."
                            ));
                            return true;
                        }
                    }
                }
                return false;
            }

            private static bool IsHandleLike(Type type)
            {
                if (type == typeof(IntPtr) || type == typeof(UIntPtr) || type == typeof(HandleRef))
                    return true;

                if (typeof(SafeHandle).IsAssignableFrom(type))
                    return true;

                if (typeof(WaitHandle).IsAssignableFrom(type))
                    return true;

                return false;
            }

            private static bool IsProcessHandleMember(string memberName)
            {
                return memberName is "Handle" or "SafeHandle" or "MainModule" or "Modules" or "HandleCount";
            }

            private static bool TryMapFileSystemInfoRootCause(Type valueType, string currentPath, out Type targetType, out string targetPropToken, out string targetPath)
            {
                targetType = valueType;
                targetPropToken = string.Empty;
                targetPath = currentPath;

                if (!typeof(FileSystemInfo).IsAssignableFrom(valueType))
                    return false;

                targetType = typeof(DirectoryInfo);
                targetPropToken = ".Root";
                targetPath = string.Concat(currentPath, ".Root");
                return true;
            }

            private void ProbeArray(Array arr, Type oType, string basePath, int depth)
            {
                var count = 0;
                foreach (var item in arr)
                {
                    if (count >= _probeOptions.MaxEnumerableItems) break;
                    if (item is null) { count++; continue; }

                    var token = string.Intern($"#{count}");
                    ProbeValue(item, item.GetType(), $"{basePath}{token}", token, depth + 1);

                    count++;
                }
            }

            private void ProbeEnumerable(IEnumerable en, Type oType, string basePath, int depth)
            {
                try
                {
                    var i = 0;
                    foreach (var item in en)
                    {
                        if (i >= _probeOptions.MaxEnumerableItems) break;
                        if (item is null) { i++; continue; }

                        var token = string.Intern($"#{i}");
                        ProbeValue(item, item.GetType(), $"{basePath}{token}", token, depth + 1);

                        i++;
                    }
                }
                catch (Exception ex)
                {
                    _issues.Add(new TestTypeIssue(
                        TestTypeIssueKind.EnumerationThrows,
                        basePath,
                        oType,
                        oType,
                        $"Enumeration threw for {basePath}.",
                        ex
                    ));

                    AddSuggestionOnce(
                        key: $"ExcludeMember::{basePath}",
                        kind: TestTypeSuggestionKind.ExcludeMember,
                        target: basePath,
                        message: "Exclude this enumerable member, or register a MemberSerializer/TypeSerializer to control enumeration."
                    );
                }
            }

            private void ProbeDictionary(IDictionary dict, Type oType, string basePath, int depth)
            {
                try
                {
                    var i = 0;
                    foreach (DictionaryEntry kvp in dict)
                    {
                        if (i >= _probeOptions.MaxEnumerableItems) break;

                        if (kvp.Key is null)
                        {
                            i++;
                            continue;
                        }

                        if (IsSafeKeyType(kvp.Key.GetType()))
                        {
                            // Mirrors serializer: only a representable key may become "!<stringified>".
                            var keyText = SafeKeyTypeToString(kvp.Key);
                            if (keyText is null)
                            {
                                _issues.Add(new TestTypeIssue(
                                    TestTypeIssueKind.UnsafeDictionaryKey,
                                    basePath,
                                    oType,
                                    kvp.Key.GetType(),
                                    $"Dictionary at '{(basePath.Length == 0 ? "<root>" : basePath)}' contains an unrepresentable key; Type keys require a usable assembly-qualified name."
                                ));
                                AddSuggestionOnce(
                                    key: $"UnrepresentableKey::{basePath}",
                                    kind: TestTypeSuggestionKind.AddMemberSerializer,
                                    target: basePath,
                                    message: "Use a supported key or a custom serializer for the dictionary."
                                );
                                i++;
                                continue;
                            }
                            var token = $"!{keyText}";
                            if (kvp.Value is not null)
                                ProbeValue(kvp.Value, kvp.Value.GetType(), $"{basePath}{token}", token, depth + 1);
                        }
                        else
                        {
                            // Mirrors serializer: unsafe keys cause DictionaryEntry itself to be graphed (which drags Key into the graph).
                            _issues.Add(new TestTypeIssue(
                                TestTypeIssueKind.UnsafeDictionaryKey,
                                basePath,
                                oType,
                                kvp.Key.GetType(),
                                $"Dictionary contains unsafe key type {kvp.Key.GetType().FullName}; serializer will graph DictionaryEntry (Key+Value) which often breaks or explodes."
                            ));

                            AddSuggestionOnce(
                                key: $"AddMemberSerializer::{basePath}",
                                kind: TestTypeSuggestionKind.AddMemberSerializer,
                                target: basePath,
                                message: $"Consider a custom serializer for dictionary member '{basePath}' (e.g., normalize keys to string) or exclude it."
                            );

                            // Still probe the entry a little (bounded) to find more issues.
                            var token = string.Intern($"#{i}");
                            ProbeValue(kvp, kvp.GetType(), $"{basePath}{token}", token, depth + 1);
                        }

                        i++;
                    }

                    // Reality check: your AllowMixedKeyTypesInDictionaryTypes option is currently not consulted by the serializer’s dictionary path.
                    AddSuggestionOnce(
                        key: "AllowMixedKeyTypesInDictionaryTypesUnused",
                        kind: TestTypeSuggestionKind.AddMemberSerializer,
                        target: "SerializationOptions.AllowMixedKeyTypesInDictionaryTypes",
                        message: $"Dictionary member '{basePath}': serializer does not currently consult AllowMixedKeyTypesInDictionaryTypes, so enabling it won’t change behavior unless you wire it in."
                    );
                }
                catch (Exception ex)
                {
                    _issues.Add(new TestTypeIssue(
                        TestTypeIssueKind.EnumerationThrows,
                        basePath,
                        oType,
                        oType,
                        $"Dictionary enumeration threw for {basePath}.",
                        ex
                    ));
                }
            }

            private void ValidateCustomSerializer(Func<object, object> sz, object value, Type runtimeType, string memberPath, bool isTypeSerializer)
            {
                object result;
                try
                {
                    result = sz(value);
                }
                catch (Exception ex)
                {
                    _issues.Add(new TestTypeIssue(
                        TestTypeIssueKind.CustomSerializerThrows,
                        memberPath,
                        runtimeType,
                        runtimeType,
                        $"{(isTypeSerializer ? "TypeSerializer" : "MemberSerializer")} threw at {memberPath}.",
                        ex
                    ));
                    return;
                }

                if (result is not null && result is not byte[] && (isTypeSerializer || result is not string))
                {
                    _issues.Add(new TestTypeIssue(
                        TestTypeIssueKind.CustomSerializerReturnedNonBytes,
                        memberPath,
                        runtimeType,
                        runtimeType,
                        $"{(isTypeSerializer ? "TypeSerializer" : "MemberSerializer")} returned {result.GetType().FullName}; writer expects byte[] or null, or string from a member hook."
                    ));
                }
            }

            private IAccessors GetOrCreateAccessor(Type ownerType, MemberInfo member, Type memberValueType)
            {
                if (!TypeAccessors.TryGetValue(ownerType, out var accs))
                {
                    TypeAccessors[ownerType] = new ConcurrentDictionary<MemberInfo, IAccessors>();
                    accs = TypeAccessors[ownerType];
                }

                if (!accs.TryGetValue(member, out var acc))
                {
                    var method = typeof(AccessorUtils)
                        .GetMethod("CreateAccessors", BindingFlags.Static | BindingFlags.NonPublic)!
                        .MakeGenericMethod(ownerType, memberValueType);

                    acc = (IAccessors)method.Invoke(null, new object[] { member })!;
                    TypeAccessors[ownerType][member] = acc;
                }

                return acc;
            }

            private void AddSuggestionOnce(string key, TestTypeSuggestionKind kind, string target, string message)
            {
                if (_suggested.Add(key))
                    _suggestions.Add(new TestTypeSuggestion(kind, target, message));
            }
        }
        #endregion

        /// <summary>Controls the scope and developer-code invocation of one compatibility probe without changing serialization behavior.<br/></summary>
        public sealed record TestTypeProbeOptions
        {
            /// <summary>Gets the maximum accepted traversal depth, with the root at depth zero; defaults to 64.<br/></summary>
            public int MaxDepth { get; init; } = 64;
            /// <summary>Gets the maximum visited-node count checked before probing a value; defaults to 100,000.<br/></summary>
            public int MaxNodes { get; init; } = 100_000;
            /// <summary>Gets the maximum number of slots inspected per array, enumerable or dictionary, including null slots; defaults to 256.<br/></summary>
            public int MaxEnumerableItems { get; init; } = 256;

            /// <summary>
            /// If true, invokes registered hooks to verify they do not throw and return bytes, null, or text for a member hook.<br/>
            /// Defaults to true; disabling this check avoids invoking custom serializers during the probe.<br/>
            /// </summary>
            public bool InvokeCustomSerializers { get; init; } = true;

            /// <summary>
            /// If true, calls selected getters twice to detect "new instance each call" patterns that defeat DuplicateRef detection.<br/>
            /// Defaults to true; those getters may therefore have observable side effects during inspection.<br/>
            /// </summary>
            public bool ProbeGetterStability { get; init; } = true;
        }

        private static bool IsSimpleStableType(Type type)
        {
            if (type.IsEnum)
                return true;

            if (InhetoBaseTypesMap.TryGetValue(type.TypeHandle, out var baseType) && baseType != InhetoBaseTypes.Unknown)
                return true;

            if (InhetoPromotedTypesMap.ContainsKey(type.TypeHandle))
                return true;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return IsSimpleStableType(type.GetGenericArguments()[0]);

            if (type.IsArray)
                return IsSimpleStableType(type.GetElementType()!);

            if (type.IsGenericType)
            {
                var args = type.GetGenericArguments();
                if (args.Length == 1 && typeof(IEnumerable).IsAssignableFrom(type) && IsSimpleStableType(args[0]))
                    return true;
            }

            return false;
        }

        private static bool IsBenignWrapperMember(Type ownerType, string propToken)
        {
            if (ownerType == typeof(ProcessStartInfo) && propToken == ".EnvironmentVariables")
                return true;

            return false;
        }
    };
}
