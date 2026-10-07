using System.Diagnostics;
using System.IO;
using Inheto;

internal static class LiteralPathReview
{
private static int passed, failed;
private static bool baseline;

/// <summary>Runs public correctness or saved-binary comparison workloads without requiring friend-assembly access.<br/></summary>
/// <param name="args">Selects benchmark mode and the comparison lane label.<br/></param>
public static void Main(string[] args)
{
if (args.Length == 2 && args[0] == "--compare-public-api")
{
    ComparePublicApi(args[1]);
    return;
}
if (args.Length == 1 && args[0] == "--payload-hashes")
{
    PayloadHashes();
    return;
}
bool benchmark = args.Contains("--benchmark");
baseline = args.Contains("--baseline");
Console.WriteLine($"{DateTimeOffset.Now:O} runtime={Environment.Version} lane={(baseline ? "baseline" : "candidate")}");
if (!benchmark)
{
    foreach (bool literalFirst in new[] { false, true })
    {
        var source = MakeCollision(literalFirst);
        var binary = InhetoBinary.FromObject(source);
        using var lifetime = binary.RawBytes;
        Check($"literal-int-{literalFirst}", () => binary.ReadInt32(".Dict!a!b") == 7);
        Check($"generic-int-{literalFirst}", () => binary.ReadPropExact<int>(".Dict!a!b") == 7);
        Check($"runtime-int-{literalFirst}", () => Equals(binary.ReadPropAs(typeof(int), ".Dict!a!b"), 7));
        Check($"names-{literalFirst}", () => binary.AllNames.Count(name => name == ".Dict!a!b") == 1);
        Check($"roundtrip-collision-{literalFirst}", () =>
        {
            var restored = binary.ToObject<PathRecord>()!;
            object? nestedValue = restored.Dict["a"];
            object? storedChild = nestedValue is System.Collections.IDictionary nested ? nested["b"] : null;
            if (!Equals(restored.Dict["a!b"], 7) || !Equals(storedChild, 8))
                Console.WriteLine($"ROUNDTRIP literal={restored.Dict["a!b"]} nestedType={nestedValue?.GetType()} nestedChild={storedChild}");
            return Equals(restored.Dict["a!b"], 7) && Equals(storedChild, 8);
        });
        source.Dict.Remove("a!b");
        var withoutLiteral = InhetoBinary.FromObject(source);
        using var noLiteralLifetime = withoutLiteral.RawBytes;
        Check($"nested-fallback-{literalFirst}", () => withoutLiteral.ReadInt32(".Dict!a!b") == 8);
    }
    foreach (bool literalFirst in new[] { false, true })
    {
        var nested = new Dictionary<string, object?> { ["b"] = new PathNode { Value = 8 } };
        var dictionary = new Dictionary<string, object?>();
        if (literalFirst) dictionary.Add("a!b", new PathNode { Value = 7 });
        dictionary.Add("a", nested);
        if (!literalFirst) dictionary.Add("a!b", new PathNode { Value = 7 });
        var binary = InhetoBinary.FromObject(new PathRecord { Dict = dictionary });
        using var lifetime = binary.RawBytes;
        Check($"literal-child-{literalFirst}", () => binary.ReadInt32(".Dict!a!b.Value") == 7);
        dictionary["a!b"] = null;
        var nullBinary = InhetoBinary.FromObject(new PathRecord { Dict = dictionary });
        using var nullLifetime = nullBinary.RawBytes;
        Check($"literal-null-{literalFirst}", () => nullBinary.ReadInt32(".Dict!a!b.Value") is null);
        dictionary["a!b"] = 99;
        var deadEnd = InhetoBinary.FromObject(new PathRecord { Dict = dictionary });
        using var deadLifetime = deadEnd.RawBytes;
        Check($"literal-dead-end-{literalFirst}", () => deadEnd.ReadInt32(".Dict!a!b.Value") is null);
    }
    string[] keys = ["", "a.b", "a#1", "a!b", "a@b", "a\\b", "a\"b", "a!b.c#1@d", "part.one#2!inside@tail😀", "é中😀", "a!", "!", ".", "#", "@"];
    foreach (string key in keys)
    {
        var source = new PathRecord { Dict = new() { [key] = 39 } };
        var binary = InhetoBinary.FromObject(source);
        using var lifetime = binary.RawBytes;
        Check("literal-key-" + key, () => binary.ReadInt32(".Dict!" + key) == 39);
    }
    var prefixDictionary = new Dictionary<string, object?>
    {
        ["a"] = new Dictionary<string, object?> { ["b"] = new PathNode { Value = 3 } },
        ["a!b"] = new PathNode { Value = 19 }
    };
    var prefixBinary = InhetoBinary.FromObject(new PathRecord { Dict = prefixDictionary });
    using var prefixLifetime = prefixBinary.RawBytes;
    Check("longest-prefix", () => prefixBinary.ReadInt32(".Dict!a!b.Value") == 19);
    var stringBinary = InhetoBinary.FromObject(new Dictionary<string, string> { ["a.b#2!c@d😀"] = "literal" });
    using var stringLifetime = stringBinary.RawBytes;
    Check("native-string", () => stringBinary.ReadString("!a.b#2!c@d😀") == "literal");
    var longBinary = InhetoBinary.FromObject(new Dictionary<string, long> { ["a.b#2!c@d😀"] = 1L << 40 });
    using var longLifetime = longBinary.RawBytes;
    Check("native-int64", () => longBinary.ReadInt64("!a.b#2!c@d😀") == 1L << 40);
    var node = new PathNode { Value = 17 };
    node.Next = node;
    var cycleBinary = InhetoBinary.FromObject(node);
    using var cycleLifetime = cycleBinary.RawBytes;
    foreach (int hops in new[] { 1, 10, 65, 500 })
    {
        string path = string.Concat(Enumerable.Repeat(".Next", hops)) + ".Value";
        Check("cycle-native-" + hops, () => cycleBinary.ReadInt32(path) == 17);
        Check("cycle-generic-" + hops, () => cycleBinary.ReadPropExact<int>(path) == 17);
    }
    Check("cycle-missing", () => cycleBinary.ReadInt32(".Next.Next.Absent") is null);
    var physical = new PathNode { Value = 130 };
    for (int i = 0; i < 130; i++) physical = new PathNode { Next = physical };
    var deepBinary = InhetoBinary.FromObject(physical);
    using var deepLifetime = deepBinary.RawBytes;
    string deepPath = string.Concat(Enumerable.Repeat(".Next", 130)) + ".Value";
    Check("physical-depth-130", () => deepBinary.ReadInt32(deepPath) == 130);
    var shared = new PathRecord { Dict = new() { ["a!b"] = node, ["x"] = node } };
    var sharedBinary = InhetoBinary.FromObject(shared);
    using var sharedLifetime = sharedBinary.RawBytes;
    Check("dictionary-reference", () => sharedBinary.ReadInt32(".Dict!x.Next.Value") == 17);
    var arrayBinary = InhetoBinary.FromObject(Enumerable.Range(0, 12).ToArray());
    using var arrayLifetime = arrayBinary.RawBytes;
    Check("element-1", () => arrayBinary.ReadInt32("#1") == 1);
    Check("element-10", () => arrayBinary.ReadInt32("#10") == 10);
    Check("element-100-missing", () => arrayBinary.ReadInt32("#100") is null);
    var zeroBinary = InhetoBinary.FromObject(0);
    using var zeroLifetime = zeroBinary.RawBytes;
    Check("root-zero", () => zeroBinary.ReadInt32("") == 0);
    // Explicit preparation must not let global cached offsets override a known physical fill entry.
    var prepare = typeof(InhetoBinary).GetMethod("GetPreparedPath",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!;
    _ = prepare.Invoke(null, [".Dict!a!b", true]);
    foreach (bool literalFirst in new[] { false, true })
    {
        var cached = InhetoBinary.FromObject(MakeCollision(literalFirst));
        using var cachedLifetime = cached.RawBytes;
        Check("prepared-key-warm-" + literalFirst, () => cached.ReadInt32(".Dict!a!b") == 7);
        Check("prepared-key-fill-" + literalFirst, () =>
        {
            var result = cached.ToObject<PathRecord>()!;
            return Equals(result.Dict["a!b"], 7) && result.Dict["a"] is System.Collections.IDictionary child && Equals(child["b"], 8);
        });
        Check("prepared-key-after-fill-" + literalFirst, () => cached.ReadInt32(".Dict!a!b") == 7);
    }
    var hookSource = MakeCollision(false);
    ((Dictionary<string, object?>)hookSource.Dict["a"]!)["trigger"] = 23;
    hookSource.Dict["a!trigger!probe"] = 71;
    var hookWrite = new SerializationOptions();
    hookWrite.AddSerializer<int>(".Dict!a!trigger", value => BitConverter.GetBytes(value));
    var hookRead = new DeserializationOptions();
    InhetoBinary? hookBinary = null;
    bool throwFromHook = false;
    int hookCalls = 0;
    hookRead.AddDeserializer<int>(".Dict!a!trigger", bytes =>
    {
        hookCalls++;
        if (hookBinary!.ReadPropExact<int>(".Dict!a!b") != 7)
            throw new InvalidOperationException("Public decoder reentry inherited the physical fill anchor.");
        if (hookBinary.ReadInt32(".Dict!a!b") != 7)
            throw new InvalidOperationException("Direct scalar decoder reentry inherited the physical fill anchor.");
        if (hookBinary.ReadPropExact<int>(".Dict!a!trigger!probe") != 71 ||
            hookBinary.ReadInt32(".Dict!a!trigger!probe") != 71)
            throw new InvalidOperationException("Direct scalar decoder reentry selected the custom anchor instead of the literal key.");
        if (throwFromHook) throw new InvalidOperationException("intentional decoder failure");
        return BitConverter.ToInt32(bytes);
    });
    hookBinary = InhetoBinary.FromObject(hookSource, hookWrite, hookRead);
    using var hookLifetime = hookBinary.RawBytes;
    Check("decoder-reentry", () =>
    {
        var restored = hookBinary.ToObject<PathRecord>()!;
        return hookCalls > 0 && restored.Dict["a"] is System.Collections.IDictionary child &&
            Equals(child["b"], 8) && Equals(child["trigger"], 23) && Equals(restored.Dict["a!b"], 7);
    });
    Check("decoder-exception", () =>
    {
        throwFromHook = true;
        try { hookBinary.ToObject<PathRecord>(); return false; }
        catch (Exception exception) { return exception.ToString().Contains("intentional decoder failure", StringComparison.Ordinal); }
        finally { throwFromHook = false; }
    });
    Check("decoder-after-exception", () => hookBinary.ReadInt32(".Id") == 41 &&
        hookBinary.ReadPropExact<int>(".Dict!a!b") == 7 && hookBinary.ToObject<PathRecord>()!.Dict.Count == 3);
    var callbackSource = new CallbackRoot
    {
        Dict = new() { ["a"] = new CallbackNode { Value = 8 }, ["a.Value"] = new CallbackNode { Value = 7 } }
    };
    var callbackOptions = new DeserializationOptions();
    InhetoBinary? callbackBinary = null;
    int activations = 0, accessors = 0;
    callbackOptions.AddActivator<CallbackNode>(_ =>
    {
        activations++;
        if (callbackBinary!.ReadInt32(".Dict!a.Value.Value") != 7)
            throw new InvalidOperationException("Native activator read inherited a physical fill anchor.");
        return new CallbackNode();
    });
    callbackBinary = InhetoBinary.FromObject(callbackSource, null, callbackOptions);
    using var callbackLifetime = callbackBinary.RawBytes;
    CallbackNode.Probe = () =>
    {
        accessors++;
        return callbackBinary.ReadInt32(".Dict!a.Value.Value");
    };
    try
    {
        Check("activator-accessor-reentry", () =>
        {
            var restored = callbackBinary.ToObject<CallbackRoot>()!;
            return activations >= 2 && accessors > 0 && restored.Dict["a"].Value == 8 && restored.Dict["a.Value"].Value == 7;
        });
    }
    finally { CallbackNode.Probe = null; }
    var serializer = new InhetoSerializer();
    BufferStream payload = serializer.Serialize(MakeCollision(false));
    try
    {
        using var buffer = new BufferStream(payload.AsReadOnlyMemory);
        var reader = InhetoBinary.FromStream(buffer);
        var plan = InhetoBinary.CheckPropPath<PathRecord, int>(".Id");
        for (int iteration = 0; iteration < 40; iteration++)
        {
            var next = MakeCollision(iteration % 2 == 0);
            next.Id = iteration;
            serializer.Serialize(next);
            reader.RebindBorrowedPayload(payload.AsReadOnlyMemory);
            Check("rebind-id-" + iteration, () => reader.ReadInt32(".Id") == iteration);
            Check("rebind-literal-" + iteration, () => reader.ReadInt32(".Dict!a!b") == 7);
            Check("rebind-prepared-" + iteration, () => reader.TryReadPropPath<int>(plan, out int value) == PropPathReadState.Value && value == iteration);
            Check("rebind-missing-" + iteration, () => reader.ReadInt32(".Absent") is null);
            var clone = reader.ShallowClone();
            Check("clone-" + iteration, () => clone.ReadInt32(".Id") == iteration && clone.ReadInt32(".Dict!a!b") == 7);
        }
    }
    finally { payload.Dispose(); }
    Console.WriteLine($"RESULT passed={passed} failed={failed}");
    Environment.ExitCode = failed == 0 ? 0 : 1;
}
else
{
    var source = new PathRecord { Id = 41, Status = "ready", Values = Enumerable.Range(0, 16).ToList(), Dict = new() { ["region"] = 19 } };
    var binary = InhetoBinary.FromObject(source);
    using var lifetime = binary.RawBytes;
    Measure("native-root-member", 100000, () => binary.ReadInt32(".Id") == 41);
    Measure("native-element", 100000, () => binary.ReadInt32(".Values#7") == 7);
    Measure("native-key", 100000, () => binary.ReadInt32(".Dict!region") == 19);
    Measure("selected-list", 4000, () => binary.ReadPropExact<List<int>>(".Values") is { Count: 16 } values && values[7] == 7);
    Measure("whole-parent", 4000, () => binary.ToObject<PathRecord>() is { Id: 41, Values.Count: 16 });
    using var buffer = new BufferStream(binary.RawMemory);
    var reader = InhetoBinary.FromStream(buffer);
    Measure("rebind-native-root", 100000, () => { reader.RebindBorrowedPayload(binary.RawMemory); return reader.ReadInt32(".Id") == 41; });
    Measure("rebind-native-element", 100000, () => { reader.RebindBorrowedPayload(binary.RawMemory); return reader.ReadInt32(".Values#7") == 7; });
}
}

/// <summary>Builds both wire orders of the same literal/nested key collision without sharing destination objects.<br/></summary>
/// <param name="literalFirst">Whether the literal long key precedes the shorter nested key.<br/></param>
/// <returns>A graph whose literal value is 7 and nested value is 8.<br/></returns>
static PathRecord MakeCollision(bool literalFirst)
{
    var values = new Dictionary<string, object?>();
    if (literalFirst) values.Add("a!b", 7);
    values.Add("a", new Dictionary<string, object?> { ["b"] = 8 });
    if (!literalFirst) values.Add("a!b", 7);
    return new PathRecord { Id = 41, Dict = values };
}

/// <summary>Compares exported CLR types and declared public member signatures with a saved pre-refactor assembly.<br/>
/// Includes generic arity, parameter types/names/defaults, return types, fields, properties and constructors.<br/>
/// Ignores method bodies, assembly version, private implementation and JIT hints; this is API-shape evidence, not behavioral proof.<br/></summary>
/// <param name="savedAssembly">Absolute path to the preserved library; an isolated load context avoids replacing the consumer's candidate.<br/></param>
private static void ComparePublicApi(string savedAssembly)
{
    var context = new System.Runtime.Loader.AssemblyLoadContext("saved-public-contract", isCollectible: true);
    try
    {
        var before = context.LoadFromAssemblyPath(Path.GetFullPath(savedAssembly));
        var expected = PublicSurface(before);
        var actual = PublicSurface(typeof(InhetoBinary).Assembly);
        string[] changes = expected.Except(actual).Select(value => "REMOVED " + value)
            .Concat(actual.Except(expected).Select(value => "ADDED " + value)).ToArray();
        if (changes.Length != 0) throw new InvalidOperationException(string.Join(Environment.NewLine, changes));
        Console.WriteLine($"PASS unchanged public CLR surface: {actual.Count} exported type/member signatures; saved={before.GetName().Version}, candidate={typeof(InhetoBinary).Assembly.GetName().Version}");
    }
    finally { context.Unload(); }
}

/// <summary>Produces ordinal signature records independent of assembly version and method implementation details.<br/></summary>
/// <param name="assembly">Saved or candidate assembly to inspect; no developer constructor or property getter is executed.<br/></param>
/// <returns>Distinct public type/member signature records used only by the external validation harness.<br/></returns>
private static HashSet<string> PublicSurface(System.Reflection.Assembly assembly)
{
    var surface = new HashSet<string>(StringComparer.Ordinal);
    var flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance |
        System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly;
    foreach (Type type in assembly.GetExportedTypes())
    {
        string prefix = type.FullName!;
        surface.Add($"{prefix}|type|{type.IsValueType}|{type.IsInterface}|{type.IsAbstract}|{type.IsSealed}|{type.BaseType}");
        foreach (var member in type.GetMembers(flags))
        {
            if (member is System.Reflection.MethodBase method)
            {
                string parameters = string.Join(",", method.GetParameters().Select(parameter =>
                    $"{parameter.ParameterType}:{parameter.Name}:{parameter.IsOut}:{parameter.IsOptional}:{parameter.DefaultValue}"));
                string returns = method is System.Reflection.MethodInfo info ? info.ReturnType.ToString() : "ctor";
                int arity = method is System.Reflection.MethodInfo generic ? generic.GetGenericArguments().Length : 0;
                surface.Add($"{prefix}|{method.Name}|{method.IsStatic}|{arity}|{returns}|{parameters}");
            }
            else if (member is System.Reflection.PropertyInfo property)
                surface.Add($"{prefix}|property|{property.Name}|{property.PropertyType}|{property.CanRead}|{property.CanWrite}");
            else if (member is System.Reflection.FieldInfo field)
                surface.Add($"{prefix}|field|{field.Name}|{field.FieldType}|{field.IsStatic}|{field.IsInitOnly}|{field.IsLiteral}");
            else if (member is System.Reflection.EventInfo notification)
                surface.Add($"{prefix}|event|{notification.Name}|{notification.EventHandlerType}");
        }
    }
    return surface;
}

/// <summary>Fingerprints deterministic writer fixtures under saved and candidate libraries without changing their bytes.<br/>
/// Includes colliding key orders, literal Unicode names, arrays and a cycle; equality is bounded wire-output evidence.<br/></summary>
private static void PayloadHashes()
{
    var cycle = new PathNode { Value = 17 };
    cycle.Next = cycle;
    object[] fixtures = [MakeCollision(false), MakeCollision(true), cycle,
        new Dictionary<string, int> { ["a.b#2!c@d😀"] = 39, [""] = 0 },
        new int?[] { 1, null, 3 }, new int[2, 3], new PathRecord { Values = [1, 2, 3] }];
    for (int i = 0; i < fixtures.Length; i++)
    {
        var binary = InhetoBinary.FromObject(fixtures[i]);
        using var lifetime = binary.RawBytes;
        Console.WriteLine($"WIRE {i} length={binary.RawSpan.Length} sha256={Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(binary.RawSpan))}");
    }
}

/// <summary>Runs a bounded public-API assertion and reports exceptions without preventing the remaining cases.<br/></summary>
/// <param name="name">Unique case label.<br/></param>
/// <param name="assertion">Unmeasured test assertion, never part of production navigation.<br/></param>
private static void Check(string name, Func<bool> assertion)
{
    try { if (assertion()) { passed++; return; } Console.WriteLine("FAIL " + name + " result"); }
    catch (Exception exception) { Console.WriteLine("FAIL " + name + " " + exception.GetType().Name + ": " + exception.Message); }
    failed++;
}

/// <summary>Measures warmed public calls against either saved or candidate assemblies, excluding setup/serialization/output.<br/></summary>
/// <param name="name">Shared workload label.<br/></param>
/// <param name="iterations">Measured operations per sample.<br/></param>
/// <param name="operation">Retained test delegate with a result assertion; delegate creation is outside measurement.<br/></param>
private static void Measure(string name, int iterations, Func<bool> operation)
{
    for (int i = 0; i < 10000; i++) if (!operation()) throw new InvalidOperationException(name);
    for (int sample = 0; sample < 5; sample++)
    {
        GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
        long bytes = GC.GetAllocatedBytesForCurrentThread(), start = Stopwatch.GetTimestamp();
        for (int i = 0; i < iterations; i++) if (!operation()) throw new InvalidOperationException(name);
        double ns = (Stopwatch.GetTimestamp() - start) * 1e9 / Stopwatch.Frequency / iterations;
        double allocated = (double)(GC.GetAllocatedBytesForCurrentThread() - bytes) / iterations;
        Console.WriteLine($"PERF {(baseline ? "baseline" : "candidate")} {name} sample={sample} bytes/op={allocated:F2} ns/op={ns:F2}");
    }
}
}

/// <summary>Contains ordinary members, a list and literal-key dictionaries for public consumer validation.<br/></summary>
public sealed class PathRecord
{
    /// <summary>Gets or sets the stable prepared scalar.<br/></summary>
    public int Id { get; set; }
    /// <summary>Gets or sets the ordinary scalar string.<br/></summary>
    public string Status { get; set; } = "ready";
    /// <summary>Gets or sets the numbered collection.<br/></summary>
    public List<int> Values { get; set; } = [];
    /// <summary>Gets or sets mixed literal/nested values without serialization attributes.<br/></summary>
    public Dictionary<string, object?> Dict { get; set; } = new();
}

/// <summary>Supports finite physical depth and circular-reference continuation tests.<br/></summary>
public sealed class PathNode
{
    /// <summary>Gets or sets the selected scalar.<br/></summary>
    public int Value { get; set; }
    /// <summary>Gets or sets the next physical or shared node.<br/></summary>
    public PathNode? Next { get; set; }
}

/// <summary>Provides a typed dictionary whose literal dotted key shadows the shorter object's scalar path.<br/></summary>
public sealed class CallbackRoot
{
    /// <summary>Gets or sets independently reconstructed typed values under colliding literal/member paths.<br/></summary>
    public Dictionary<string, CallbackNode> Dict { get; set; } = new();
}

/// <summary>Checks public native-read semantics when application getter/setter code runs inside physical reconstruction.<br/></summary>
public sealed class CallbackNode
{
    private int stored;
    /// <summary>Gets or sets the test-only probe after serialization; default instance-member policy excludes this static member.<br/></summary>
    public static Func<int?>? Probe { get; set; }
    /// <summary>Gets or sets the scalar while checking that any reentrant public probe starts at the payload root.<br/></summary>
    public int Value
    {
        get { if (Probe is { } probe && probe() != 7) throw new InvalidOperationException("Native getter read inherited a physical anchor."); return stored; }
        set { if (Probe is { } probe && probe() != 7) throw new InvalidOperationException("Native setter read inherited a physical anchor."); stored = value; }
    }
}
