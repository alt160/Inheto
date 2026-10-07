using Inheto;
using System.Reflection;
using System.Text.RegularExpressions;
using MySingleton = SingletonPropertyProbe;

bool verify = args.Contains("--verify", StringComparer.Ordinal);
Console.WriteLine($"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss zzz} Activation {(verify ? "verification" : "characterization")} .NET {Environment.Version}; harmless counters only; library {typeof(InhetoBinary).Assembly.Location}");
int checks = 0;

byte[] emptyBytes = InhetoBinary.FromObject(new EmptyConstructorProbe()).RawMemory.ToArray();
EmptyConstructorProbe.Calls = 0;
using (var payload = new System.IO.BufferStream(emptyBytes))
{
    var reader = InhetoBinary.FromStream(payload);
    Require(EmptyConstructorProbe.Calls == 0, "opening valid binary does not construct its stored empty type");
    Require(reader.ToObject<EmptyConstructorProbe>() is not null && EmptyConstructorProbe.Calls == 1,
        "typed empty-object hydration invokes its constructor once");
}

EmptyConstructorProbe.Calls = 0;
using (var payload = new System.IO.BufferStream(emptyBytes))
{
    var reader = InhetoBinary.FromStream(payload);
    Exception? error = Capture(() => reader.ToObject<UnrelatedEmptyDestination>());
    Require(error is InvalidCastException && EmptyConstructorProbe.Calls == 1,
        "a mismatched requested type rejects only after constructing the stored type");
}

EmptyConstructorProbe.Calls = 0;
using (var payload = new System.IO.BufferStream(emptyBytes))
{
    var reader = InhetoBinary.FromStream(payload);
    Require(reader.ToObject(typeof(EmptyConstructorProbe)) is EmptyConstructorProbe && EmptyConstructorProbe.Calls == 1,
        "runtime-Type hydration follows the same stored-type constructor path");
}

EmptyConstructorProbe.Calls = 0;
using (var payload = new System.IO.BufferStream(emptyBytes))
{
    var reader = InhetoBinary.FromStream(payload);
    Require(reader.ToObject<object>() is EmptyConstructorProbe && EmptyConstructorProbe.Calls == 1,
        "object-valued hydration activates the stored type without a concrete destination fence");
}

EmptyConstructorProbe.Calls = 0;
using (var payload = new System.IO.BufferStream(emptyBytes))
{
    var reader = InhetoBinary.FromStream(payload);
    Require(reader.ReadStaticClass("") is EmptyConstructorProbe && EmptyConstructorProbe.Calls == 1,
        "native empty-object reading activates the stored type");
}

var rootExclusion = new SerializationOptions();
rootExclusion.ExcludedTypes.Add(typeof(EmptyConstructorProbe));
rootExclusion.ExcludedMembers.Add("");
var excludedRoot = InhetoBinary.FromObject(new EmptyConstructorProbe(), rootExclusion);
EmptyConstructorProbe.Calls = 0;
Require(excludedRoot.ToObject<EmptyConstructorProbe>() is not null && EmptyConstructorProbe.Calls == 1,
    "writer type/path exclusions do not suppress the root");

var nested = new ActivationEnvelope { Safe = 42, Special = new EmptyConstructorProbe() };
var memberExclusion = new SerializationOptions();
memberExclusion.ExcludedMembers.Add(".Special");
var excludedMember = InhetoBinary.FromObject(nested, memberExclusion);
EmptyConstructorProbe.Calls = 0;
Require(!excludedMember.PropExists(".Special") && excludedMember.ToObject<ActivationEnvelope>()!.Safe == 42 && EmptyConstructorProbe.Calls == 0,
    "exact writer member exclusion removes the activation-bearing node");
var typeExclusion = new SerializationOptions();
typeExclusion.ExcludedTypes.Add(typeof(EmptyConstructorProbe));
var excludedType = InhetoBinary.FromObject(nested, typeExclusion);
EmptyConstructorProbe.Calls = 0;
Require(!excludedType.PropExists(".Special") && excludedType.ToObject<ActivationEnvelope>()!.Safe == 42 && EmptyConstructorProbe.Calls == 0,
    "exact writer type exclusion removes a selected nested node");

byte[] nestedBytes = InhetoBinary.FromObject(nested).RawMemory.ToArray();
EmptyConstructorProbe.Calls = 0;
using (var payload = new System.IO.BufferStream(nestedBytes))
{
    var reader = InhetoBinary.FromStream(payload);
    Require(reader.ReadInt32(".Safe") == 42 && EmptyConstructorProbe.Calls == 0,
        "selective scalar reading does not traverse the unrelated activation-bearing node");
    Require(reader.ToObject<SafeProjection>()!.Safe == 42 && EmptyConstructorProbe.Calls == 0,
        "a projection without the special destination member does not hydrate it");
    Require(reader.ReadPropExact<EmptyConstructorProbe>(".Special") is not null && EmptyConstructorProbe.Calls == 1,
        "selected special-member hydration activates the stored type");
}

EmptyConstructorProbe.Calls = 0;
using (var payload = new System.IO.BufferStream(nestedBytes))
{
    var reader = InhetoBinary.FromStream(payload);
    Require(reader.ToObject<ActivationEnvelope>()!.Special is EmptyConstructorProbe && EmptyConstructorProbe.Calls == 1,
        "whole-object hydration reaches its object-valued special member");
}

var suppression = new DeserializationOptions();
suppression.AddActivator<EmptyConstructorProbe>(_ => null);
EmptyConstructorProbe.Calls = 0;
using (var payload = new System.IO.BufferStream(emptyBytes))
{
    var reader = InhetoBinary.FromStream(payload, suppression);
    Require(reader.ToObject<EmptyConstructorProbe>() is null && EmptyConstructorProbe.Calls == 0,
        "an exact stored-type null activator prevents typed construction");
    Require(reader.ReadStaticClass("") is null && EmptyConstructorProbe.Calls == 0,
        "an exact stored-type null activator prevents native construction");
}

var unrelatedSuppression = new DeserializationOptions();
unrelatedSuppression.AddActivator<UnrelatedEmptyDestination>(_ => null);
EmptyConstructorProbe.Calls = 0;
using (var payload = new System.IO.BufferStream(emptyBytes))
{
    var reader = InhetoBinary.FromStream(payload, unrelatedSuppression);
    Exception? error = Capture(() => reader.ToObject<UnrelatedEmptyDestination>());
    Require(error is InvalidCastException && EmptyConstructorProbe.Calls == 1,
        "a requested-type null activator does not suppress a different payload-selected type");
}

byte[] typeBytes = InhetoBinary.FromObject(typeof(EmptyConstructorProbe)).RawMemory.ToArray();
EmptyConstructorProbe.Calls = 0;
using (var payload = new System.IO.BufferStream(typeBytes))
{
    var reader = InhetoBinary.FromStream(payload);
    Require(reader.ReadType("") == typeof(EmptyConstructorProbe) && EmptyConstructorProbe.Calls == 0,
        "reading a System.Type value resolves metadata without invoking the named constructor");
    Require(verify
        ? Capture(() => reader.ReadStaticClass("")) is InvalidCastException && EmptyConstructorProbe.Calls == 0
        : reader.ReadStaticClass("") is Type && EmptyConstructorProbe.Calls == 0,
        "warm native StaticClass header check precedes the shared Type value cache");
    reader = InhetoBinary.FromStream(payload);
    Require(verify
        ? Capture(() => reader.ReadStaticClass("")) is InvalidCastException && EmptyConstructorProbe.Calls == 0
        : reader.ReadStaticClass("") is EmptyConstructorProbe && EmptyConstructorProbe.Calls == 1,
        "cold native StaticClass reading checks the Type encoding before construction");
}

byte[] textBytes = InhetoBinary.FromObject(typeof(EmptyConstructorProbe).AssemblyQualifiedName!).RawMemory.ToArray();
EmptyConstructorProbe.Calls = 0;
using (var payload = new System.IO.BufferStream(textBytes))
{
    var reader = InhetoBinary.FromStream(payload);
    Require(reader.ReadString("") == typeof(EmptyConstructorProbe).AssemblyQualifiedName && EmptyConstructorProbe.Calls == 0,
        "reading ordinary type-name text is data-only");
    reader = InhetoBinary.FromStream(payload);
    Require(verify
        ? Capture(() => reader.ReadStaticClass("")) is InvalidCastException && EmptyConstructorProbe.Calls == 0
        : reader.ReadStaticClass("") is EmptyConstructorProbe && EmptyConstructorProbe.Calls == 1,
        "cold native StaticClass reading checks ordinary string encoding before construction");
}

var singletonSeed = (SingletonPropertyProbe)Activator.CreateInstance(typeof(SingletonPropertyProbe), nonPublic: true)!;
byte[] singletonBytes = InhetoBinary.FromObject(singletonSeed).RawMemory.ToArray();
SingletonPropertyProbe.Calls = 0;
using (var payload = new System.IO.BufferStream(singletonBytes))
{
    var reader = InhetoBinary.FromStream(payload);
    Require(reader.ToObject<SingletonPropertyProbe>() is not null && SingletonPropertyProbe.Calls == (verify ? 1 : 2),
        "cold singleton discovery returns the first successful getter result");
    SingletonPropertyProbe.Calls = 0;
    Require(reader.ToObject<SingletonPropertyProbe>() is not null && SingletonPropertyProbe.Calls == 1,
        "warm singleton construction invokes the cached getter once");
}
SingletonPropertyProbe.Calls = 0;
using (var payload = new System.IO.BufferStream(singletonBytes))
{
    var reader = InhetoBinary.FromStream(payload);
    Require(verify
        ? reader.ReadStaticClass("") is SingletonPropertyProbe && SingletonPropertyProbe.Calls == 1
        : Capture(() => reader.ReadStaticClass("")) is MissingMethodException && SingletonPropertyProbe.Calls == 0,
        "native empty-object reader shares the generic singleton fallback");
}

var factorySeed = (PrivateFactoryProbe)Activator.CreateInstance(typeof(PrivateFactoryProbe), nonPublic: true)!;
byte[] factoryBytes = InhetoBinary.FromObject(factorySeed).RawMemory.ToArray();
PrivateFactoryProbe.Calls = 0;
using (var payload = new System.IO.BufferStream(factoryBytes))
{
    var reader = InhetoBinary.FromStream(payload);
    Require(reader.ToObject<PrivateFactoryProbe>() is not null && PrivateFactoryProbe.Calls == (verify ? 1 : 2),
        "cold reconstruction returns its first nonpublic parameterless factory result");
    PrivateFactoryProbe.Calls = 0;
    Require(reader.ToObject<PrivateFactoryProbe>() is not null && PrivateFactoryProbe.Calls == 1,
        "warm reconstruction invokes the cached nonpublic factory once");
}

if (verify)
{
    Require(new DeserializationOptions().AllowStoredTypeActivation, "compatible stored-type activation remains the default");
    var controlled = new DeserializationOptions { AllowStoredTypeActivation = false };
    EmptyConstructorProbe.Calls = 0;
    using (var payload = new System.IO.BufferStream(emptyBytes))
    {
        var reader = InhetoBinary.FromStream(payload, controlled);
        Require(reader.ToObject<EmptyConstructorProbe>() is not null && EmptyConstructorProbe.Calls == 1,
            "opt-in concrete destination uses its caller-selected constructor");
        EmptyConstructorProbe.Calls = 0;
        Require(reader.ToObject<UnrelatedEmptyDestination>() is not null && EmptyConstructorProbe.Calls == 0,
            "opt-in unrelated concrete destination does not activate the historical source type");
        Require(Capture(() => reader.ToObject<object>()) is NotSupportedException && EmptyConstructorProbe.Calls == 0,
            "opt-in object destination requires explicit stored-type approval before construction");
        Require(Capture(() => reader.ToObject(typeof(object))) is NotSupportedException && EmptyConstructorProbe.Calls == 0,
            "opt-in runtime-Type object destination uses the same explicit approval boundary");
        Require(Capture(() => reader.ToObject<IEmptyShape>()) is NotSupportedException && EmptyConstructorProbe.Calls == 0,
            "opt-in interface destination requires explicit stored-type approval");
        Require(Capture(() => reader.ToObject<EmptyShapeBase>()) is NotSupportedException && EmptyConstructorProbe.Calls == 0,
            "opt-in abstract destination requires explicit stored-type approval");
        Require(Capture(() => reader.ReadStaticClass("")) is NotSupportedException && EmptyConstructorProbe.Calls == 0,
            "opt-in native destination requires approval before construction");
        controlled.AllowStoredType<EmptyConstructorProbe>();
        Require(reader.ToObject<object>() is EmptyConstructorProbe && EmptyConstructorProbe.Calls == 1,
            "exact generic approval permits object-valued stored-type reconstruction");
        EmptyConstructorProbe.Calls = 0;
        Require(Capture(() => reader.ToObject<UnrelatedEmptyDestination>()) is NotSupportedException && EmptyConstructorProbe.Calls == 0,
            "approved stored type incompatible with destination is rejected before construction");
        Require(reader.ReadStaticClass("") is EmptyConstructorProbe && EmptyConstructorProbe.Calls == 1,
            "exact approval permits native reconstruction");
        EmptyConstructorProbe.Calls = 0;
        Require(controlled.RemoveAllowedStoredType<EmptyConstructorProbe>(), "existing approval can be removed");
        Require(Capture(() => reader.ReadStaticClass("")) is NotSupportedException && EmptyConstructorProbe.Calls == 0,
            "removing approval blocks even a warm native value cache");
        Require(!controlled.RemoveAllowedStoredType<EmptyConstructorProbe>(), "removing an absent approval is a no-op");
        controlled.AllowStoredType(typeof(EmptyConstructorProbe));
        Require(reader.ReadStaticClass("") is EmptyConstructorProbe && EmptyConstructorProbe.Calls == 0,
            "runtime-Type registration restores authorized access to the existing native cached instance");
        Require(reader.ToObject<IEmptyShape>() is EmptyConstructorProbe && reader.ToObject<EmptyShapeBase>() is EmptyConstructorProbe && EmptyConstructorProbe.Calls == 2,
            "approved stored type reconstructs through assignable interface and abstract destinations");
        EmptyConstructorProbe.Calls = 0;
        Require(reader.ToObject(typeof(EmptyConstructorProbe)) is EmptyConstructorProbe && EmptyConstructorProbe.Calls == 1,
            "opt-in runtime-Type concrete destination follows the caller-selected type");
    }

    var noApproval = new DeserializationOptions { AllowStoredTypeActivation = false };
    EmptyConstructorProbe.Calls = 0;
    using (var payload = new System.IO.BufferStream(nestedBytes))
    {
        var reader = InhetoBinary.FromStream(payload, noApproval);
        Require(reader.ReadInt32(".Safe") == 42 && reader.ToObject<SafeProjection>()!.Safe == 42 && EmptyConstructorProbe.Calls == 0,
            "opt-in control does not change ordinary scalar reads or a projection omitting the special member");
        Require(Capture(() => reader.ToObject<ActivationEnvelope>()) is NotSupportedException && EmptyConstructorProbe.Calls == 0,
            "nested object-valued zero-input member requires approval in whole-graph reconstruction");
        noApproval.AllowStoredType<EmptyConstructorProbe>();
        Require(reader.ToObject<ActivationEnvelope>()!.Special is EmptyConstructorProbe && EmptyConstructorProbe.Calls == 1,
            "approved nested zero-input member reconstructs in its ordinary parent graph");
    }

    var controlledSuppression = new DeserializationOptions { AllowStoredTypeActivation = false };
    controlledSuppression.AddActivator<EmptyConstructorProbe>(_ => null);
    EmptyConstructorProbe.Calls = 0;
    using (var payload = new System.IO.BufferStream(emptyBytes))
    {
        var reader = InhetoBinary.FromStream(payload, controlledSuppression);
        Require(reader.ToObject<EmptyConstructorProbe>() is null && EmptyConstructorProbe.Calls == 0,
            "opt-in concrete destination retains intentional null custom activators");
        Require(Capture(() => reader.ReadStaticClass("")) is NotSupportedException && EmptyConstructorProbe.Calls == 0,
            "registering an activator alone does not implicitly approve an untyped stored identity");
        controlledSuppression.AllowStoredType<EmptyConstructorProbe>();
        Require(reader.ReadStaticClass("") is null && EmptyConstructorProbe.Calls == 0,
            "opt-in approved native read retains intentional null custom activators");
    }

    using (var payload = new System.IO.BufferStream(typeBytes))
    {
        var reader = InhetoBinary.FromStream(payload, noApproval);
        Require(reader.ReadType("") == typeof(EmptyConstructorProbe) && EmptyConstructorProbe.Calls == 0,
            "stored-construction control does not reinterpret explicit System.Type metadata as construction");
    }
    MethodInfo forkMethod = typeof(DeserializationOptions).GetMethod("ForkExcluding", BindingFlags.Instance | BindingFlags.NonPublic)!;
    var fork = (DeserializationOptions)forkMethod.Invoke(controlled, [new HashSet<string> { ".Unused" }])!;
    Require(!fork.AllowStoredTypeActivation, "owner-scoped options fork retains the selected mode");
    controlled.RemoveAllowedStoredType<EmptyConstructorProbe>();
    using (var payload = new System.IO.BufferStream(emptyBytes))
    {
        Require(InhetoBinary.FromStream(payload, fork).ReadStaticClass("") is EmptyConstructorProbe,
            "owner-scoped options fork independently retains its exact approval snapshot");
    }
    Require(Capture(() => controlled.AllowStoredType(null!)) is ArgumentNullException, "null approval fails at registration");

    var nullSeed = (NullThenFactoryProbe)Activator.CreateInstance(typeof(NullThenFactoryProbe), nonPublic: true)!;
    byte[] nullFirstBytes = InhetoBinary.FromObject(nullSeed).RawMemory.ToArray();
    NullThenFactoryProbe.NullCalls = NullThenFactoryProbe.SuccessCalls = 0;
    using (var payload = new System.IO.BufferStream(nullFirstBytes))
    {
        var reader = InhetoBinary.FromStream(payload);
        Require(reader.ToObject<NullThenFactoryProbe>() is not null && NullThenFactoryProbe.NullCalls == 1 && NullThenFactoryProbe.SuccessCalls == 1,
            "cold null singleton candidate is tried once before returning the first successful getter result");
        NullThenFactoryProbe.NullCalls = NullThenFactoryProbe.SuccessCalls = 0;
        Require(reader.ToObject<NullThenFactoryProbe>() is not null && NullThenFactoryProbe.NullCalls == 0 && NullThenFactoryProbe.SuccessCalls == 1,
            "warm factory selection does not rescan earlier null candidates");
    }
    var changedOptions = new DeserializationOptions();
    changedOptions.AddActivator<PrivateFactoryProbe>(_ => null);
    PrivateFactoryProbe.Calls = 0;
    using (var payload = new System.IO.BufferStream(factoryBytes))
    {
        var reader = InhetoBinary.FromStream(payload, changedOptions);
        Require(reader.ToObject<PrivateFactoryProbe>() is null && PrivateFactoryProbe.Calls == 0,
            "warm intrinsic factory cache cannot override current-call developer activators");
    }
    var nullFactorySeed = (AllNullFactoryProbe)Activator.CreateInstance(typeof(AllNullFactoryProbe), nonPublic: true)!;
    byte[] nullFactoryBytes = InhetoBinary.FromObject(nullFactorySeed).RawMemory.ToArray();
    AllNullFactoryProbe.Calls = 0;
    using (var payload = new System.IO.BufferStream(nullFactoryBytes))
    {
        var reader = InhetoBinary.FromStream(payload);
        Require(reader.ToObject<AllNullFactoryProbe>() is null && AllNullFactoryProbe.Calls == 1,
            "all-null discovery returns null without repeating a factory");
        Require(reader.ToObject<AllNullFactoryProbe>() is null && AllNullFactoryProbe.Calls == 1,
            "all-null warm fallback retains the existing cached-null contract");
    }
    byte[] fieldBytes = InhetoBinary.FromObject(SingletonFieldProbe.Instance).RawMemory.ToArray();
    using (var payload = new System.IO.BufferStream(fieldBytes))
    {
        Require(ReferenceEquals(InhetoBinary.FromStream(payload).ToObject<SingletonFieldProbe>(), SingletonFieldProbe.Instance),
            "public static singleton field remains an intrinsic reconstruction candidate");
    }
    var concurrentSeed = (ConcurrentFactoryProbe)Activator.CreateInstance(typeof(ConcurrentFactoryProbe), nonPublic: true)!;
    byte[] concurrentBytes = InhetoBinary.FromObject(concurrentSeed).RawMemory.ToArray();
    ConcurrentFactoryProbe.Calls = 0;
    var results = new ConcurrentFactoryProbe?[32];
    Parallel.For(0, results.Length, index =>
    {
        using var payload = new System.IO.BufferStream((ReadOnlyMemory<byte>)concurrentBytes);
        results[index] = InhetoBinary.FromStream(payload).ToObject<ConcurrentFactoryProbe>();
    });
    Require(results.All(value => value is not null) && ConcurrentFactoryProbe.Calls == results.Length,
        "independent concurrent readers execute each successful factory once, including cold-cache races");
    Require(results.Distinct(ReferenceEqualityComparer.Instance).Count() == results.Length,
        "intrinsic cache retains the factory rather than sharing constructed instances across readers");
    if (args.Length == 3)
    {
        var guideBlocks = Regex.Matches(File.ReadAllText(args[1]), @"```csharp\r?\n(.*?)```", RegexOptions.Singleline);
        var compiled = Regex.Match(File.ReadAllText(args[2]), @"// RECONSTRUCTION-SAMPLE-1\r?\n(.*?)// END-RECONSTRUCTION-SAMPLE", RegexOptions.Singleline);
        Require(guideBlocks.Count == 3 && compiled.Success && Normalize(compiled.Groups[1].Value) == Normalize(guideBlocks[2].Groups[1].Value),
            "optional reconstruction guide example matches independently compiled source");
        VerifyGuideExample(singletonBytes);
    }
}

Console.WriteLine($"PASS {checks} {(verify ? "regression" : "current-behavior")} checks. Valid locally generated binaries and harmless in-process counters only.");

/// <summary>Asserts one source-predicted baseline behavior and records its successful observation.<br/></summary>
/// <param name="condition">Predicate over the harmless counter/result or exception collected by this isolated consumer.<br/></param>
/// <param name="description">Bounded diagnostic name describing the behavior, not a claim that it is the desired policy.<br/></param>
void Require(bool condition, string description)
{
    if (!condition) throw new InvalidOperationException("Characterization mismatch: " + description);
    checks++;
    Console.WriteLine("CONFIRMED " + description);
}

/// <summary>Collects the exception from a bounded read without treating expected pre-fix failures as harness failures.<br/></summary>
/// <param name="read">One synchronous public reader call over valid locally generated binary; no remote input or executable payload is used.<br/></param>
/// <returns>The observed exception, or null when the read completed normally.<br/></returns>
static Exception? Capture(Action read)
{
    try { read(); return null; }
    catch (Exception error) { return error; }
}

/// <summary>Executes the exact optional-control guide fragment with a harmless singleton fixture and an owned input stream.<br/></summary>
/// <param name="bytes">Valid locally generated singleton binary; no modified or remote input is used.<br/></param>
void VerifyGuideExample(byte[] bytes)
{
    using var input = new System.IO.BufferStream(bytes);
    // RECONSTRUCTION-SAMPLE-1
var options = new DeserializationOptions { AllowStoredTypeActivation = false };
// Add only if this graph uses a zero-input/singleton type in an object/interface
// member, an untyped read, or ReadStaticClass:
options.AllowStoredType<MySingleton>();

var reader = InhetoBinary.FromStream(input, options);
    // END-RECONSTRUCTION-SAMPLE
    Require(reader.ReadStaticClass("") is MySingleton, "optional reconstruction guide fragment restores its explicitly approved singleton");
}

/// <summary>Removes only outer/line-ending formatting when comparing a published fragment with compiled fixture source.<br/></summary>
/// <param name="text">One C# example fragment, inspected as text and never evaluated as a script.<br/></param>
/// <returns>Normalized exact source text for drift detection.<br/></returns>
static string Normalize(string text) => string.Join('\n', text.Replace("\r", "").Split('\n').Select(line => line.TrimEnd())).Trim();

/// <summary>Benign zero-input shape with an observable constructor and no serializable instance state.<br/></summary>
public sealed class EmptyConstructorProbe : EmptyShapeBase, IEmptyShape
{
    /// <summary>Counts construction within the isolated test process only.<br/></summary>
    public static int Calls;
    /// <summary>Increments an in-process integer; performs no I/O, process creation or other external action.<br/></summary>
    public EmptyConstructorProbe() => Calls++;
}

/// <summary>Unrelated concrete zero-input destination used to observe the pre-cast activation boundary.<br/></summary>
public sealed class UnrelatedEmptyDestination { }

/// <summary>Ordinary stateful root separating one selective scalar from an optional special object.<br/></summary>
public sealed class ActivationEnvelope
{
    /// <summary>Ordinary serialized scalar that does not require special activation.<br/></summary>
    public int Safe { get; set; }
    /// <summary>Object-valued slot that permits the stored zero-input representation to be reached.<br/></summary>
    public object? Special { get; set; }
}

/// <summary>Destination shape intentionally omitting the special node while retaining the matching scalar.<br/></summary>
public sealed class SafeProjection
{
    /// <summary>Matching scalar selected by shape-based materialization.<br/></summary>
    public int Safe { get; set; }
}

/// <summary>Benign singleton-like shape exposing only a zero-input static property.<br/></summary>
public sealed class SingletonPropertyProbe
{
    private static readonly SingletonPropertyProbe instance = new();
    /// <summary>Counts getter calls within this test process only.<br/></summary>
    public static int Calls;
    /// <summary>Restricts ordinary public construction, allowing the static-property reconstruction branch to be observed.<br/></summary>
    private SingletonPropertyProbe() { }
    /// <summary>Returns a local singleton and increments an integer, without external side effects.<br/></summary>
    public static SingletonPropertyProbe Instance { get { Calls++; return instance; } }
}

/// <summary>Benign shape whose only self-returning static factory is nonpublic.<br/></summary>
public sealed class PrivateFactoryProbe
{
    /// <summary>Counts factory calls within the isolated test process only.<br/></summary>
    public static int Calls;
    /// <summary>Prevents the public-constructor branch from hiding factory-discovery behavior.<br/></summary>
    private PrivateFactoryProbe() { }
    /// <summary>Creates one harmless local instance and records the call; no external actions are performed.<br/></summary>
    /// <returns>A new instance of this test-only zero-input shape.<br/></returns>
    private static PrivateFactoryProbe Create() { Calls++; return new(); }
}

/// <summary>Benign zero-input shape with an unsuccessful singleton candidate and one valid private factory.<br/></summary>
public sealed class NullThenFactoryProbe
{
    /// <summary>Counts null property candidates without external effects.<br/></summary>
    public static int NullCalls;
    /// <summary>Counts valid private-factory calls without external effects.<br/></summary>
    public static int SuccessCalls;
    /// <summary>Excludes the direct public-constructor route in this isolated regression.<br/></summary>
    private NullThenFactoryProbe() { }
    /// <summary>Returns null to exercise the unsuccessful-candidate branch.<br/></summary>
    public static NullThenFactoryProbe? Missing { get { NullCalls++; return null; } }
    /// <summary>Returns a valid exact-type result after the preceding null property candidate.<br/></summary>
    public static NullThenFactoryProbe Available { get { SuccessCalls++; return new(); } }
    /// <summary>Produces the first valid exact-type result after unsuccessful property discovery.<br/></summary>
    /// <returns>One local instance whose construction performs no external work.<br/></returns>
    private static NullThenFactoryProbe Create() { SuccessCalls++; return new(); }
}

/// <summary>Empty interface used to verify explicit approval for an abstract destination shape.<br/></summary>
public interface IEmptyShape { }

/// <summary>Empty abstract base whose assignable concrete type is selected through an exact approval.<br/></summary>
public abstract class EmptyShapeBase { }

/// <summary>Harmless zero-input type whose intrinsic factory deliberately returns null.<br/></summary>
public sealed class AllNullFactoryProbe
{
    /// <summary>Counts the unsuccessful factory only inside this process.<br/></summary>
    public static int Calls;
    /// <summary>Allows a local seed without creating a public intrinsic construction route.<br/></summary>
    private AllNullFactoryProbe() { }
    /// <summary>Exercises intrinsic fallback when no candidate produces an exact-type instance.<br/></summary>
    /// <returns>Null by design.<br/></returns>
    private static AllNullFactoryProbe? Create() { Calls++; return null; }
}

/// <summary>Harmless zero-input shape reconstructed from a public static singleton field.<br/></summary>
public sealed class SingletonFieldProbe
{
    /// <summary>Local singleton whose identity is observed without an executing property getter.<br/></summary>
    public static readonly SingletonFieldProbe Instance = new();
    /// <summary>Excludes the public-constructor route for this field-specific fixture.<br/></summary>
    private SingletonFieldProbe() { }
}

/// <summary>Harmless factory shape counting independent construction under concurrent reader cache races.<br/></summary>
public sealed class ConcurrentFactoryProbe
{
    /// <summary>Interlocked count of factory executions across the isolated consumer's workers.<br/></summary>
    public static int Calls;
    /// <summary>Allows local seeding without a public parameterless construction route.<br/></summary>
    private ConcurrentFactoryProbe() { }
    /// <summary>Produces one independent instance and records exactly one factory execution.<br/></summary>
    /// <returns>A new local instance, never a shared result cache.<br/></returns>
    private static ConcurrentFactoryProbe Create() { Interlocked.Increment(ref Calls); return new(); }
}
