using Inheto;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

/// <summary>Checks the opt-in unsupported-value policy only in the independent package consumer; never ships in Inheto.<br/></summary>
public static class UnsupportedPolicyRegression
{
    /// <summary>Characterizes silent opaque-struct loss and cross-payload circular-offset retention before the writer changes.<br/></summary>
    public static void Characterize()
    {
        var opaque = new OpaqueState(37);
        var old = InhetoBinary.FromObject(opaque, new SerializationOptions { FailOnUnsupportedType = true });
        Require(old.ToObject<OpaqueState>().Value == 0, "pre-fix opaque-state characterization changed");
        var policy = CircularPolicy();
        var writer = new InhetoSerializer(policy);
        _ = writer.Serialize(Node(1));
        byte[] reused = writer.Serialize(Node(10)).AsReadOnlyMemory.ToArray();
        byte[] fresh = new InhetoSerializer(policy).Serialize(Node(10)).AsReadOnlyMemory.ToArray();
        Require(!reused.AsSpan().SequenceEqual(fresh), "pre-fix circular-offset characterization changed");
        Console.WriteLine("CONFIRMED pre-fix: strict flag permits opaque struct state loss; reused circular-member writer differs from a fresh writer.");
    }

    /// <summary>Validates rejection paths, explicit representations, selection rules, ordinary graphs and writer recovery through public APIs.<br/></summary>
    public static void Verify()
    {
        var strict = new SerializationOptions { FailOnUnsupportedType = true };
        var cacheType = typeof(InhetoSerializer).GetNestedType("StrictRepresentationCache", BindingFlags.NonPublic)!;
        Require((cacheType.Attributes & TypeAttributes.BeforeFieldInit) == 0, "strict metadata cache can initialize eagerly");
        object[] rejected = { new IntPtr(123), new UIntPtr(123), new OpaqueState(37), (Action)(() => { }) };
        int checks = 0;
        foreach (object value in rejected)
        {
            Reject(value, strict, "", value.GetType()); checks++;
            Reject(new Holder { Value = value }, strict, ".Value", value.GetType()); checks++;
            Reject(new object[] { 1, value }, strict, "#1", value.GetType()); checks++;
            Reject(new List<object> { 1, value }, strict, "#1", value.GetType()); checks++;
            Reject(new Dictionary<string, object> { ["bad"] = value }, strict, "!bad", value.GetType()); checks++;
            Reject(new Holder { Value = new object[] { value } }, strict, ".Value#0", value.GetType()); checks++;
        }
        using (var handle = new TestHandle()) { Reject(handle, strict, "", typeof(TestHandle)); checks++; }
        Reject(new SpanOwner(), strict, ".Value", typeof(ReadOnlySpan<int>)); checks++;
        Reject(new RefOwner(), strict, ".Value", typeof(int).MakeByRefType()); checks++;
        Reject(new PointerOwner(), strict, ".Value", typeof(int).MakePointerType()); checks++;
        Reject(new FunctionPointerOwner(), strict, ".Value", typeof(FunctionPointerOwner).GetField("Value")!.FieldType); checks++;

        var excluded = new SerializationOptions { FailOnUnsupportedType = true };
        excluded.ExcludedMembers.Add(".Value");
        Require(!InhetoBinary.FromObject(new Holder { Value = new IntPtr(1) }, excluded).PropExists(".Value"), "member exclusion lost precedence"); checks++;
        excluded = new SerializationOptions { FailOnUnsupportedType = true };
        excluded.ExcludedTypes.Add(typeof(IntPtr));
        Require(!InhetoBinary.FromObject(new Holder { Value = new IntPtr(1) }, excluded).PropExists(".Value"), "type exclusion lost precedence"); checks++;
        Reject(new IntPtr(1), excluded, "", typeof(IntPtr)); checks++;
        var selected = new SerializationOptions { FailOnUnsupportedType = true };
        selected.IncludedMembers.Add(".Good");
        Require(InhetoBinary.FromObject(new SelectiveOwner(), selected).ReadInt32(".Good") == 5, "unselected unboxable getter was inspected"); checks++;
        excluded = new SerializationOptions { FailOnUnsupportedType = true };
        excluded.ExcludedMembers.Add(".Value");
        _ = InhetoBinary.FromObject(new SpanOwner(), excluded); checks++;
        excluded = new SerializationOptions { FailOnUnsupportedType = true };
        excluded.ExcludedTypes.Add(typeof(ReadOnlySpan<int>));
        _ = InhetoBinary.FromObject(new SpanOwner(), excluded); checks++;
        _ = InhetoBinary.FromObject(new OrdinarySpanOwner(), excluded); checks++;

        int hooks = 0;
        var custom = new SerializationOptions { FailOnUnsupportedType = true };
        custom.AddTypeSerializer<IntPtr>(value => { hooks++; return BitConverter.GetBytes(value.ToInt64()); });
        _ = InhetoBinary.FromObject(new IntPtr(123), custom);
        Require(hooks == 1, "custom type serializer was blocked or invoked twice"); checks++;
        custom = new SerializationOptions { FailOnUnsupportedType = true };
        custom.AddSerializer<IntPtr>(".Value", value => { hooks++; return value.ToInt64().ToString(); });
        _ = InhetoBinary.FromObject(new Holder { Value = new IntPtr(123) }, custom);
        Require(hooks == 2, "custom member serializer was blocked or invoked twice"); checks++;
        custom = new SerializationOptions { FailOnUnsupportedType = true };
        custom.AddTypeSerializer<SpanOwner>(_ => { hooks++; return new byte[] { 1 }; });
        _ = InhetoBinary.FromObject(new SpanOwner(), custom);
        Require(hooks == 3, "owning-object custom serializer did not bypass unboxable members"); checks++;
        custom = new SerializationOptions { FailOnUnsupportedType = true };
        custom.AddTypeSerializer<OpaqueState>(_ => null!);
        Require(InhetoBinary.FromObject(new OpaqueState(1), custom).ToObject<OpaqueState?>() is null, "intentional null hook was rejected"); checks++;

        object[] accepted = { 7, "text", DateTime.UtcNow, UInt128.MaxValue, new VisibleState { Value = 37 },
            new EmptyState(), new object(), new EmptyObject(), new[] { 1, 2 }, new object?[] { null, 3 },
            new List<int>(), new KeyValuePair<int, string>(1, "one"), new Memory<byte>(new byte[] { 1, 2 }) };
        foreach (object value in accepted)
        {
            byte[] ordinary = InhetoBinary.FromObject(value).RawMemory.ToArray();
            Require(InhetoBinary.FromObject(value, strict).RawSpan.SequenceEqual(ordinary), "strict mode changed an accepted payload: " + value.GetType()); checks++;
        }
        foreach (MemberTypesEnum mode in Enum.GetValues<MemberTypesEnum>())
        {
            var policy = new SerializationOptions { FailOnUnsupportedType = true, MemberTypes = mode };
            if (mode is MemberTypesEnum.PublicAndPrivateFields or MemberTypesEnum.PrivateFieldsOnly)
                Require(InhetoBinary.FromObject(new OpaqueState(37), policy).ReadInt32(".state") == 37, "selected private state was rejected/lost");
            else Reject(new OpaqueState(37), policy, "", typeof(OpaqueState));
            checks++;
        }
        var filtered = new SerializationOptions { FailOnUnsupportedType = true };
        filtered.ExcludedMembers.Add(".Value");
        _ = InhetoBinary.FromObject(new VisibleState { Value = 37 }, filtered); checks++;

        VerifyRecovery(); checks++;
        VerifyDeveloperExceptions(); checks++;
        VerifyAllocations(); checks++;
        Console.WriteLine($"PASS unsupported policy .NET {Environment.Version}: {checks} checks; root/member/collection paths, filters/hooks, selected state, empty markers, recovery and allocation guardrails.");
    }

    /// <summary>Rejects one unsupported input with contextual, machine-readable exception data and no successful serialization result.<br/></summary>
    /// <param name="value">Root graph containing the value to reject.<br/></param>
    /// <param name="policy">Opt-in policy, including any intentional selection rules.<br/></param>
    /// <param name="path">Expected root-empty or full Inheto path.<br/></param>
    /// <param name="type">Expected actual value type or unboxable declared member type.<br/></param>
    private static void Reject(object value, SerializationOptions policy, string path, Type type)
    {
        try { _ = InhetoBinary.FromObject(value, policy); }
        catch (NotSupportedException exception)
        {
            Require(Equals(exception.Data["Inheto.Path"], path) && Equals(exception.Data["Inheto.Type"], type), "unsupported exception lost type/path context");
            return;
        }
        throw new InvalidOperationException("unsupported value accepted: " + type + " at " + path);
    }

    /// <summary>Checks repeated circular-member writes and reuse after a strict failure, including switching the flag off.<br/></summary>
    private static void VerifyRecovery()
    {
        var policy = CircularPolicy(); policy.FailOnUnsupportedType = true;
        var writer = new InhetoSerializer(policy);
        for (int i = 0; i < 3; i++)
        {
            byte[] reused = writer.Serialize(Node(i * 10)).AsReadOnlyMemory.ToArray();
            byte[] fresh = new InhetoSerializer(policy).Serialize(Node(i * 10)).AsReadOnlyMemory.ToArray();
            Require(reused.AsSpan().SequenceEqual(fresh), "circular offsets escaped their serialization operation");
        }
        var failure = new FailureOwner { First = Node(1), Bad = new IntPtr(9) };
        bool failed = false;
        try { _ = writer.Serialize(failure); } catch (NotSupportedException) { failed = true; }
        Require(failed, "strict failure fixture was not rejected");
        policy.FailOnUnsupportedType = false;
        byte[] recovered = writer.Serialize(Node(40)).AsReadOnlyMemory.ToArray();
        Require(recovered.AsSpan().SequenceEqual(new InhetoSerializer(policy).Serialize(Node(40)).AsReadOnlyMemory.Span), "failed writer poisoned its next payload");
    }

    /// <summary>Confirms developer getter and hook exceptions retain their original identity instead of being reclassified as type failures.<br/></summary>
    private static void VerifyDeveloperExceptions()
    {
        var sentinel = new InvalidOperationException("developer sentinel");
        var policy = new SerializationOptions { FailOnUnsupportedType = true };
        try { _ = InhetoBinary.FromObject(new ThrowingOwner(sentinel), policy); }
        catch (InvalidOperationException exception) when (ReferenceEquals(exception, sentinel)) { goto Hook; }
        throw new InvalidOperationException("getter exception changed");
    Hook:
        policy.AddTypeSerializer<OpaqueState>(_ => throw sentinel);
        try { _ = InhetoBinary.FromObject(new OpaqueState(1), policy); }
        catch (InvalidOperationException exception) when (ReferenceEquals(exception, sentinel)) { return; }
        throw new InvalidOperationException("custom hook exception changed");
    }

    /// <summary>
    /// Compares warmed steady-state managed allocation counts with strict mode off/on for identical repeated graphs.<br/>
    /// Alternates measurement order across eight paired batches and requires exactly equal minimum counts, not a byte tolerance.<br/>
    /// Runtime/pool bookkeeping can add a small transient allocation to an individual batch; recurring per-write allocations remain in every batch and fail the exact comparison.<br/>
    /// Setup, warmup, sample storage, logging and assertions remain outside each measured interval.<br/>
    /// </summary>
    private static void VerifyAllocations()
    {
        var value = new VisibleState { Value = 7 };
        var policy = new SerializationOptions();
        var writer = new InhetoSerializer(policy);
        long Measure(bool strict)
        {
            policy.FailOnUnsupportedType = strict;
            for (int i = 0; i < 1000; i++) _ = writer.Serialize(value);
            long before = GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < 10000; i++) _ = writer.Serialize(value);
            return GC.GetAllocatedBytesForCurrentThread() - before;
        }
        _ = Measure(false); _ = Measure(true);
        long off = long.MaxValue, on = long.MaxValue;
        for (int batch = 0; batch < 8; batch++)
        {
            long offSample, onSample;
            if ((batch & 1) == 0)
            {
                offSample = Measure(false); onSample = Measure(true);
            }
            else
            {
                onSample = Measure(true); offSample = Measure(false);
            }
            off = Math.Min(off, offSample); on = Math.Min(on, onSample);
            Console.WriteLine($"Allocation sample {batch + 1}: off={offSample}, on={onSample} managed bytes.");
        }
        Require(on == off, $"strict success added managed allocations: off={off}; on={on}");
        Console.WriteLine($"PASS strict allocation delta: minimum of eight 10,000-write warmed batches off={off}, on={on}, delta={on - off} managed bytes.");
    }

    /// <summary>Creates a policy with one repeated member token for the operation-lifetime regression.<br/></summary>
    /// <returns>Mutable policy shared by the reusable and fresh comparison writers.<br/></returns>
    private static SerializationOptions CircularPolicy()
    {
        var policy = new SerializationOptions(); policy.AddCircularProperty<CircleNode>(".Child"); return policy;
    }

    /// <summary>Creates two different values so a stale circular offset cannot be hidden by identical payload contents.<br/></summary>
    /// <param name="value">Root scalar; the child receives its successor.<br/></param>
    /// <returns>A fresh non-cyclic graph using a registered circular-member token.<br/></returns>
    private static CircleNode Node(int value) => new() { Id = value, Child = new() { Id = value + 1 } };

    /// <summary>Throws on a failed independent-consumer assertion.<br/></summary>
    /// <param name="condition">Required invariant.<br/></param>
    /// <param name="message">Failure context, built outside allocation measurements.<br/></param>
    private static void Require(bool condition, string message)
    { if (!condition) throw new InvalidOperationException(message); }

    private struct OpaqueState
    {
        private int state;
        /// <summary>Sets private state without exposing serializable public members.<br/></summary>
        /// <param name="value">Private value used to detect silent loss.<br/></param>
        public OpaqueState(int value) => state = value;
        internal int Value => state;
    }
    private struct VisibleState { public int Value; }
    private struct EmptyState { }
    private sealed class EmptyObject { }
    private sealed class Holder { public object? Value { get; set; } }
    private sealed class CircleNode { public int Id { get; set; } public CircleNode? Child { get; set; } }
    private sealed class FailureOwner { public CircleNode? First { get; set; } public IntPtr Bad { get; set; } }
    private sealed class SpanOwner { public ReadOnlySpan<int> Value => throw new InvalidOperationException("unboxable getter should not run"); }
    private sealed class OrdinarySpanOwner { public int Good { get; set; } public ReadOnlySpan<int> Value => throw new InvalidOperationException("excluded unboxable getter should not run"); }
    private sealed class SelectiveOwner { public int Good { get; set; } = 5; public ReadOnlySpan<int> Bad => throw new InvalidOperationException("unselected getter should not run"); }
    private sealed class RefOwner { private int state; public ref int Value => ref state; }
    private sealed class ThrowingOwner
    {
        private readonly Exception exception;
        /// <summary>Stores the exact exception instance that the getter must propagate.<br/></summary>
        /// <param name="exception">Caller-owned failure sentinel.<br/></param>
        public ThrowingOwner(Exception exception) => this.exception = exception;
        public int Value => throw exception;
    }
    private sealed class TestHandle : SafeHandle
    {
        /// <summary>Creates a fake invalid handle with no operating-system resource ownership.<br/></summary>
        public TestHandle() : base(IntPtr.Zero, false) { }
        public override bool IsInvalid => true;
        /// <summary>Performs no native operation; this fixture never owns a real handle.<br/></summary>
        /// <returns>True because there is no resource to release.<br/></returns>
        protected override bool ReleaseHandle() => true;
    }
    private unsafe sealed class PointerOwner
    {
        public int* Value;
        /// <summary>Sets a fake address that is inspected as metadata only; never dereferenced.<br/></summary>
        public PointerOwner() => Value = (int*)1;
    }
    private unsafe sealed class FunctionPointerOwner
    {
        public delegate*<int> Value;
        /// <summary>Sets a null function pointer that is never invoked or boxed.<br/></summary>
        public FunctionPointerOwner() => Value = null;
    }

    /// <summary>Checks the production change allowlist and compares default-policy payload bytes against the saved serialization-only baseline.<br/></summary>
    /// <param name="candidate">Actual independent-package runtime loaded by this consumer.<br/></param>
    /// <param name="baselinePath">Absolute path of the saved pre-policy runtime.<br/></param>
    public static void CompareScope(Assembly candidate, string baselinePath)
    {
        var context = new AssemblyLoadContext("unsupported-before", isCollectible: true);
        context.Resolving += (_, name) => AssemblyLoadContext.Default.Assemblies.FirstOrDefault(item => item.GetName().Name == name.Name);
        try
        {
            var baseline = context.LoadFromAssemblyPath(Path.GetFullPath(baselinePath));
            ProductionIlAudit.ValidateControls();
            var before = ProductionIlAudit.Instructions(baseline);
            var after = ProductionIlAudit.Instructions(candidate);
            string oldMarkers = before.Keys.Single(key => key.StartsWith("Inheto.InhetoSerializer::", StringComparison.Ordinal) && key.Contains(" WriteNodeTypeMarkers(", StringComparison.Ordinal));
            string newMarkers = after.Keys.Single(key => key.StartsWith("Inheto.InhetoSerializer::", StringComparison.Ordinal) && key.Contains(" WriteNodeTypeMarkers(", StringComparison.Ordinal));
            Require(before.Keys.Where(key => !after.ContainsKey(key)).SequenceEqual(new[] { oldMarkers }), "unexpected removed production methods");
            string[] helpers = { "EnsureSupportedValue", "HasSelectedSerializationMember", "CreateUnsupportedTypeException", "EnsureSingletonMembersBoxable" };
            var expectedAdded = helpers.Select(name => after.Keys.Single(key => key.StartsWith("Inheto.InhetoSerializer::", StringComparison.Ordinal) && key.Contains(" " + name + "(", StringComparison.Ordinal)))
                .Concat(new[] { newMarkers })
                .Concat(after.Keys.Where(key => key.StartsWith("Inheto.InhetoSerializer+StrictRepresentationCache", StringComparison.Ordinal) || key.StartsWith("Inheto.InhetoSerializer+StrictRepresentationShape", StringComparison.Ordinal)))
                .ToHashSet(StringComparer.Ordinal);
            Require(after.Keys.Where(key => !before.ContainsKey(key)).ToHashSet(StringComparer.Ordinal).SetEquals(expectedAdded), "unexpected added production methods");
            string[] changedNames = { "GraphFields", "GraphProperties", "WriteNodeForType", "Serialize" };
            var expectedChanged = changedNames.Select(name => before.Keys.Single(key => key.StartsWith("Inheto.InhetoSerializer::", StringComparison.Ordinal) && key.Contains(" " + name + "(", StringComparison.Ordinal))).ToHashSet(StringComparer.Ordinal);
            var changed = before.Keys.Where(key => after.TryGetValue(key, out var body) && body != before[key]).ToHashSet(StringComparer.Ordinal);
            Require(changed.SetEquals(expectedChanged), "unexpected changed production instructions: " + string.Join("; ", changed));
            Console.WriteLine($"PASS unsupported-policy IL scope: five existing writer methods changed/signature-adjusted; {after.Count - expectedAdded.Count - changed.Count} other methods/constructors unchanged; only strict guards/lazy metadata added.");

            var owner = baseline.GetType("Inheto.InhetoBinary", throwOnError: true)!;
            var factory = owner.GetMethods(BindingFlags.Public | BindingFlags.Static).Single(method => method.Name == "FromObject");
            var shared = new[] { 1, 2, 3 };
            var self = new Dictionary<string, object>(); self["self"] = self;
            object[] values = { 7, "Unicode: \u03a9 \U0001f642", new OpaqueState(37), new VisibleState { Value = 9 },
                new Holder { Value = new[] { 3, 5, 8 } }, new object?[] { null, shared, shared, self },
                new List<string?> { null, "", "text" }, new int[,] { { 1, 2 }, { 3, 4 } },
                new Dictionary<string, object> { ["date"] = new DateTime(638961234567890123, DateTimeKind.Utc), ["128"] = UInt128.MaxValue } };
            foreach (object value in values)
            {
                object earlier = factory.Invoke(null, new object?[] { value, null, null })!;
                var raw = (ReadOnlyMemory<byte>)owner.GetProperty("RawMemory")!.GetValue(earlier)!;
                Require(InhetoBinary.FromObject(value).RawSpan.SequenceEqual(raw.Span), "default payload changed: " + value.GetType());
            }
            Console.WriteLine($"PASS default-policy byte identity: {values.Length} scalar/object/opaque/collection/null/shared-cycle/128-bit/date fixtures.");
            var writerType = baseline.GetType("Inheto.InhetoSerializer", throwOnError: true)!;
            var oldOptionsType = baseline.GetType("Inheto.SerializationOptions", throwOnError: true)!;
            var oldWriter = writerType.GetConstructor(new[] { oldOptionsType })!.Invoke(new object?[] { null });
            var oldWrite = writerType.GetMethod("Serialize")!.CreateDelegate<Func<object, System.IO.BufferStream>>(oldWriter);
            var newWriter = new InhetoSerializer();
            object fixture = new VisibleState { Value = 7 };
            long Measure(Func<object, System.IO.BufferStream> write)
            {
                for (int i = 0; i < 1000; i++) _ = write(fixture);
                long start = GC.GetAllocatedBytesForCurrentThread();
                for (int i = 0; i < 10000; i++) _ = write(fixture);
                return GC.GetAllocatedBytesForCurrentThread() - start;
            }
            _ = Measure(oldWrite); _ = Measure(newWriter.Serialize);
            long oldBytes = Measure(oldWrite), newBytes = Measure(newWriter.Serialize);
            Require(oldBytes == newBytes, $"default policy added managed allocations: before={oldBytes}; after={newBytes}");
            Console.WriteLine($"PASS default allocation delta vs pre-policy runtime: before={oldBytes}, after={newBytes}, delta={newBytes - oldBytes} bytes over 10,000 warmed writes.");
        }
        finally { context.Unload(); }
    }
}
