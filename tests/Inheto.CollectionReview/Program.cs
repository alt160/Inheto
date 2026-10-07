using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Inheto;

var cases = ReviewCases.Build();
if (args.Length == 2 && args[0] == "--case")
{
    try { cases[args[1]](); Console.WriteLine("PASS " + args[1]); }
    catch (Exception exception) { Console.WriteLine("FAIL " + args[1] + " " + exception.GetType().Name + ": " + exception.Message); Console.WriteLine(string.Join(Environment.NewLine, (exception.StackTrace ?? "").Split(Environment.NewLine).Take(3))); Environment.ExitCode = 1; }
    return;
}

int passed = 0, failed = 0, timedOut = 0;
foreach (string name in cases.Keys.Where(name => args.Length != 2 || args[0] != "--prefix" || name.StartsWith(args[1], StringComparison.Ordinal)))
{
    var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
    start.ArgumentList.Add(typeof(ReviewCases).Assembly.Location);
    start.ArgumentList.Add("--case"); start.ArgumentList.Add(name);
    using var child = Process.Start(start)!;
    var output = child.StandardOutput.ReadToEndAsync();
    var diagnostics = child.StandardError.ReadToEndAsync();
    if (!child.WaitForExit(15000)) { child.Kill(entireProcessTree: true); child.WaitForExit(); timedOut++; Console.WriteLine("TIMEOUT " + name); }
    else if (child.ExitCode == 0) passed++;
    else failed++;
    string text = output.GetAwaiter().GetResult() + diagnostics.GetAwaiter().GetResult();
    Console.WriteLine(text.Length <= 1500 ? text.TrimEnd() : text[..1500]);
}
Console.WriteLine($"TOTAL cases={passed + failed + timedOut}; pass={passed}; fail={failed}; timeout={timedOut}");
Environment.ExitCode = failed == 0 && timedOut == 0 ? 0 : 1;

/// <summary>Builds isolated acceptance scenarios against production public APIs, retaining the original pre-fix diagnostic cases.<br/></summary>
public static class ReviewCases
{
    /// <summary>Creates a bounded collection-route and lifecycle matrix, each executed in its own child process.<br/></summary>
    /// <returns>Named assertions that distinguish current failures from expected fill semantics.<br/></returns>
    public static Dictionary<string, Action> Build()
    {
        var cases = new Dictionary<string, Action>();
        AddFamily<TaggedList>(cases, (a, b) => new() { a, b });
        AddFamily<TaggedQueue>(cases, (a, b) => { var c = new TaggedQueue(); c.Enqueue(a); c.Enqueue(b); return c; });
        AddFamily<TaggedStack>(cases, (a, b) => { var c = new TaggedStack(); c.Push(a); c.Push(b); return c; });
        AddFamily<TaggedSet>(cases, (a, b) => new() { a, b });
        AddFamily<TaggedObservable>(cases, (a, b) => new() { a, b });
        AddFamily<TaggedDictionary>(cases, (a, b) => new() { ["first"] = a, ["second"] = b });
        foreach (bool selected in new[] { false, true })
        {
            AddNull<TaggedList>(cases, selected, new TaggedList { new() { Value = 1 } });
            AddNull<TaggedDictionary>(cases, selected, new TaggedDictionary { ["first"] = new() { Value = 1 } });
            AddNull<List<ReviewNode>>(cases, selected, new List<ReviewNode> { new() { Value = 1 } });
            AddNull<Dictionary<string, ReviewNode>>(cases, selected, new Dictionary<string, ReviewNode> { ["first"] = new() { Value = 1 } });
        }
        cases["control/list/supplied-append"] = () =>
        {
            var binary = InhetoBinary.FromObject(new List<int> { 1, 2 }); var supplied = new List<int> { 99 };
            binary.ReadPropOnto(supplied, ""); Require(supplied.SequenceEqual(new[] { 99, 1, 2 }), "supplied contents were not retained/appended");
        };
        cases["control/set/supplied-comparer"] = () =>
        {
            var binary = InhetoBinary.FromObject(new HashSet<string>(StringComparer.Ordinal) { "ABC", "abc" });
            var supplied = new HashSet<string>(StringComparer.OrdinalIgnoreCase); binary.ReadPropOnto(supplied, "");
            Require(supplied.Count == 1 && ReferenceEquals(supplied.Comparer, StringComparer.OrdinalIgnoreCase), "comparer or deduplication changed");
        };
        cases["control/stack/scalar-order"] = () =>
        {
            var source = new Stack<int>(); source.Push(1); source.Push(2); source.Push(3);
            var actual = InhetoBinary.FromObject(source).ToObject<Stack<int>>()!;
            Require(actual.SequenceEqual(source), "expected=" + string.Join(',', source) + "; actual=" + string.Join(',', actual));
        };
        cases["control/stack/complex-order"] = () =>
        {
            var source = new Stack<ReviewNode>(); source.Push(new() { Value = 1 }); source.Push(new() { Value = 2 });
            var actual = InhetoBinary.FromObject(source).ToObject<Stack<ReviewNode>>()!;
            Require(actual.Select(n => n.Value).SequenceEqual(source.Select(n => n.Value)), "complex stack enumeration reversed");
        };
        cases["control/list/independent-results"] = () =>
        {
            var binary = InhetoBinary.FromObject(new List<ReviewNode> { new() { Value = 1 } });
            var a = binary.ToObject<List<ReviewNode>>()!; var b = binary.ToObject<List<ReviewNode>>()!;
            Require(!ReferenceEquals(a, b) && !ReferenceEquals(a[0], b[0]), "public operations shared result instances");
        };
        cases["control/list/exception-then-retry"] = () =>
        {
            var binary = InhetoBinary.FromObject(new List<ReviewNode> { new() { Value = 1 } });
            var options = new DeserializationOptions(); options.AddActivator<ReviewNode>(_ => throw new InvalidOperationException("injected"));
            bool threw = false; try { binary.ToObject<List<ReviewNode>>(options); } catch (InvalidOperationException) { threw = true; }
            Require(threw && binary.ToObject<List<ReviewNode>>()![0].Value == 1, "failed operation poisoned retry");
        };
        cases["control/list/reentrant-read"] = () =>
        {
            var binary = InhetoBinary.FromObject(new ReviewHolder<List<ReviewNode>> { Value = new() { new() { Value = 1 } } });
            var options = new DeserializationOptions(); int calls = 0;
            options.AddActivator<ReviewNode>(context => { calls++; Require(context.Binary.ReadPropExact<int>(".Value#0.Value") == 1, "nested read failed"); return new ReviewNode(); });
            var result = binary.ReadPropExact<List<ReviewNode>>(".Value", options)!;
            Require(calls == 1 && result[0].Value == 1, "reentry changed outer activation or values");
        };
        cases["control/list/supplied-skips-activation"] = () =>
        {
            var binary = InhetoBinary.FromObject(new List<ReviewNode> { new() { Value = 1 } });
            var options = new DeserializationOptions(); options.AddActivator<List<ReviewNode>>(_ => throw new InvalidOperationException("should not activate supplied collection"));
            var supplied = new List<ReviewNode>(); binary.ReadPropOnto(supplied, "", options); Require(supplied.Count == 1, "supplied fill failed");
        };
        AddOrder(cases, "int", new[] { 1, 2, 3 });
        AddOrder(cases, "nullable-int", new int?[] { 1, null, 3 });
        AddOrder(cases, "enum", new[] { ReviewEnum.First, ReviewEnum.Second, ReviewEnum.Third });
        AddOrder(cases, "string", new[] { "one", "two", "three" });
        AddOrder(cases, "date", new[] { new DateOnly(2026, 1, 1), new DateOnly(2026, 2, 1), new DateOnly(2026, 3, 1) });
        foreach (bool selected in new[] { false, true })
        foreach (string route in new[] { "create", "parent-create" })
        {
            if (!selected && route == "parent-create") continue;
            cases["container-backref/object-owner/" + selected + "/" + route] = () =>
            {
                var source = new List<ObjectOwnerNode>(); source.Add(new() { Owner = source });
                var binary = InhetoBinary.FromObject(selected ? (object)new ReviewHolder<List<ObjectOwnerNode>> { Value = source } : source);
                var actual = route == "parent-create" ? binary.ToObject<ReviewHolder<List<ObjectOwnerNode>>>()!.Value! : selected
                    ? binary.ReadPropExact<List<ObjectOwnerNode>>(".Value")! : binary.ToObject<List<ObjectOwnerNode>>()!;
                Require(actual.Count == 1 && ReferenceEquals(actual, actual[0].Owner), "owner back-reference was not the returned list; owner=" + actual[0].Owner?.GetType().Name);
            };
            cases["container-backref/typed-owner/" + selected + "/" + route] = () =>
            {
                var source = new List<TypedOwnerNode>(); source.Add(new() { Owner = source });
                var binary = InhetoBinary.FromObject(selected ? (object)new ReviewHolder<List<TypedOwnerNode>> { Value = source } : source);
                var actual = route == "parent-create" ? binary.ToObject<ReviewHolder<List<TypedOwnerNode>>>()!.Value! : selected
                    ? binary.ReadPropExact<List<TypedOwnerNode>>(".Value")! : binary.ToObject<List<TypedOwnerNode>>()!;
                Require(actual.Count == 1 && ReferenceEquals(actual, actual[0].Owner), "typed owner was not the returned list; ownerCount=" + actual[0].Owner?.Count);
            };
            cases["container-backref/dictionary-self/" + selected + "/" + route] = () =>
            {
                var source = new SelfDictionary { ["first"] = 1 }; source.Self = source;
                var binary = InhetoBinary.FromObject(selected ? (object)new ReviewHolder<SelfDictionary> { Value = source } : source);
                var actual = route == "parent-create" ? binary.ToObject<ReviewHolder<SelfDictionary>>()!.Value! : selected
                    ? binary.ReadPropExact<SelfDictionary>(".Value")! : binary.ToObject<SelfDictionary>()!;
                Require(actual.Count == 1 && ReferenceEquals(actual, actual.Self), "dictionary self-reference failed; selfCount=" + actual.Self?.Count);
            };
        }
        cases["control/list/empty-derived-parent"] = () =>
        {
            var source = new TaggedList { Tag = 7, Hero = new() { Value = 900 } };
            var actual = InhetoBinary.FromObject(new ReviewHolder<TaggedList> { Value = source }).ToObject<ReviewHolder<TaggedList>>()!.Value!;
            Require(actual.Count == 0 && actual.Tag == 7 && actual.Hero?.Value == 900, "empty derived parent members failed");
        };
        cases["control/list/element-null-activation"] = () =>
        {
            var source = new List<ReviewNode?> { new() { Value = 1 }, null };
            var options = new DeserializationOptions(); int calls = 0;
            options.AddActivator<ReviewNode>(_ => { calls++; return null; });
            var result = InhetoBinary.FromObject(source).ToObject<List<ReviewNode?>>(options)!;
            Require(result.Count == 2 && result.All(n => n is null) && calls == 1, "null activation did not preserve element slots");
        };
        cases["control/list/omitted-member-parent"] = () =>
        {
            var source = new TaggedList { Tag = 7, Hero = new() { Value = 900 } };
            var serialization = new SerializationOptions(); serialization.ExcludedMembers.Add(".Value.Tag");
            var serializer = new InhetoSerializer(serialization);
            var binary = InhetoBinary.FromStream(serializer.Serialize(new ReviewHolder<TaggedList> { Value = source }));
            var destination = new ReviewHolder<TaggedList> { Value = new() { Tag = 99 } };
            binary.ReadPropOnto(destination, "");
            Require(destination.Value!.Tag == 99 && destination.Value.Hero?.Value == 900, "omitted source member overwrote supplied state");
        };
        cases["control/list/codec-parent"] = () =>
        {
            var source = new TaggedList { Tag = 7, Hero = new() { Value = 900 } };
            var serialization = new SerializationOptions(); serialization.AddSerializer<int>(".Value.Tag", value => BitConverter.GetBytes(value));
            var serializer = new InhetoSerializer(serialization);
            var binary = InhetoBinary.FromStream(serializer.Serialize(new ReviewHolder<TaggedList> { Value = source }));
            var options = new DeserializationOptions(); options.AddDeserializer<int>(".Value.Tag", bytes => BitConverter.ToInt32(bytes));
            var result = binary.ToObject<ReviewHolder<TaggedList>>(options)!;
            Require(result.Value!.Tag == 7 && result.Value.Hero?.Value == 900, "member codec was not applied");
        };
        cases["control/dictionary/alias-selected-path"] = () =>
        {
            var source = new TaggedDictionary { Tag = 7, Hero = new() { Value = 900 }, ["first"] = new() { Value = 100 } };
            var binary = InhetoBinary.FromObject(new ReviewHolder<TaggedDictionary> { Value = source });
            var options = new DeserializationOptions(); options.Aliases.Add(".Value", ".Renamed");
            var actual = binary.ReadPropExact<TaggedDictionary>(".Renamed", options)!;
            Require(actual.Count == 1 && actual.Tag == 7 && actual.Hero?.Value == 900, "aliased collection path did not restore members");
        };
        cases["control/object-member/nested-scalar"] = () =>
        {
            var source = new ReviewHolder<ObjectPayloadNode> { Value = new() { Payload = 123 } };
            var actual = InhetoBinary.FromObject(source).ToObject<ReviewHolder<ObjectPayloadNode>>()!.Value!;
            Require(actual.Payload is int value && value == 123, "nested object member=" + actual.Payload);
        };
        cases["control/object-member/root-scalar"] = () =>
        {
            var actual = InhetoBinary.FromObject(new ObjectPayloadNode { Payload = 123 }).ToObject<ObjectPayloadNode>()!;
            Require(actual.Payload is int value && value == 123, "root object member=" + actual.Payload);
        };
        cases["control/object-member/nested-reference-encoding"] = () =>
        {
            var source = new List<ObjectOwnerNode>(); source.Add(new() { Owner = source });
            var binary = InhetoBinary.FromObject(new ReviewHolder<List<ObjectOwnerNode>> { Value = source });
            Require(binary.AllNames.Contains(".Value#0.Owner"), "owner reference was not encoded");
        };
        cases["control/list/readonly-supplied-member-parent"] = () =>
        {
            var source = new ReadonlyMembersList { new() { Value = 100 } }; source.Hero.Value = 900;
            var actual = InhetoBinary.FromObject(new ReviewHolder<ReadonlyMembersList> { Value = source }).ToObject<ReviewHolder<ReadonlyMembersList>>()!.Value!;
            Require(actual.Count == 1 && actual.Hero.Value == 900, "readonly initialized member not filled");
        };
        foreach (bool selected in new[] { false, true })
        {
            cases["control/dictionary/null-activation-parent/" + selected] = () =>
            {
                var binary = InhetoBinary.FromObject(new ReviewHolder<Dictionary<string, ReviewNode>> { Value = new() { ["first"] = new() { Value = 1 } } });
                var options = new DeserializationOptions(); int calls = 0; options.AddActivator<Dictionary<string, ReviewNode>>(_ => { calls++; return null; });
                var actual = binary.ToObject<ReviewHolder<Dictionary<string, ReviewNode>>>(options)!;
                Require(actual.Value is null && calls == 1, "whole-parent null suppression failed");
            };
        }
        ExtendedCases.Add(cases);
        ConversionCases.Add(cases);
        EnumerableCases.Add(cases);
        return cases;
    }

    /// <summary>Checks stack order across scalar shapes and all public create/fill routes.<br/></summary>
    /// <typeparam name="T">Encoded element type.<br/></typeparam>
    /// <param name="cases">Scenario registry.<br/></param>
    /// <param name="label">Stable diagnostic shape label.<br/></param>
    /// <param name="values">Distinct or nullable source values in push order.<br/></param>
    private static void AddOrder<T>(Dictionary<string, Action> cases, string label, T[] values)
    {
        foreach (string route in new[] { "root", "selected", "generic-fill", "parent-create" })
            cases["stack-order/" + label + "/" + route] = () =>
            {
                var source = new Stack<T>(); foreach (var value in values) source.Push(value);
                bool nested = route is "selected" or "parent-create";
                var binary = InhetoBinary.FromObject(nested ? (object)new ReviewHolder<Stack<T>> { Value = source } : source);
                Stack<T> actual;
                if (route == "parent-create") actual = binary.ToObject<ReviewHolder<Stack<T>>>()!.Value!;
                else if (route == "selected") actual = binary.ReadPropExact<Stack<T>>(".Value")!;
                else if (route == "generic-fill") { actual = new(); binary.ReadPropOnto(actual, ""); }
                else actual = binary.ToObject<Stack<T>>()!;
                Require(actual.SequenceEqual(source), "expected=" + string.Join(',', source) + "; actual=" + string.Join(',', actual));
            };
    }

    /// <summary>Registers create/fill routes for a derived family with independent and aliased declared members.<br/></summary>
    /// <typeparam name="T">Concrete supported collection subclass.<br/></typeparam>
    /// <param name="cases">Destination scenario registry.<br/></param>
    /// <param name="create">Creates the collection with two source nodes.<br/></param>
    private static void AddFamily<T>(Dictionary<string, Action> cases, Func<ReviewNode, ReviewNode, T> create) where T : class, ITagged, new()
    {
        foreach (bool selected in new[] { false, true })
        foreach (bool alias in new[] { false, true })
        foreach (string route in new[] { "create", "generic-fill", "runtime-fill", "parent-create" })
        {
            if (route == "parent-create" && !selected) continue;
            string name = typeof(T).Name + "/" + (selected ? "selected" : "root") + "/" + route + "/" + (alias ? "alias" : "independent");
            cases[name] = () =>
            {
                var first = new ReviewNode { Value = 100 }; first.Next = first;
                var source = create(first, new ReviewNode { Value = 200 }); source.Tag = 7; source.Hero = alias ? first : new() { Value = 900 };
                var binary = InhetoBinary.FromObject(selected ? (object)new ReviewHolder<T> { Value = source } : source);
                string path = selected ? ".Value" : ""; byte[] before = binary.RawBytes.ToArray();
                T? result;
                if (route == "parent-create") result = binary.ToObject<ReviewHolder<T>>()!.Value;
                else if (route == "create") result = selected ? binary.ReadPropExact<T>(path) : binary.ToObject<T>();
                else
                {
                    result = new T { Tag = -1, Hero = new() { Value = -1 } };
                    if (route == "generic-fill") binary.ReadPropOnto(result, path);
                    else binary.ReadPropOnto(typeof(T), result, path);
                }
                Require(result is not null, "collection unexpectedly null");
                var nodes = Nodes(result!).ToArray();
                Require(nodes.Length == 2 && nodes.Select(n => n.Value).Order().SequenceEqual(new[] { 100, 200 }), "entry values/count=" + nodes.Length);
                Require(nodes.First(n => n.Value == 100).Next == nodes.First(n => n.Value == 100), "element cycle lost");
                Require(result!.Tag == 7 && result.Hero?.Value == (alias ? 100 : 900), $"declared members omitted: Tag={result.Tag}; Hero={result.Hero?.Value}");
                if (alias) Require(ReferenceEquals(result.Hero, nodes.First(n => n.Value == 100)), "declared-member alias lost");
                Require(before.AsSpan().SequenceEqual(binary.RawBytes.AsReadOnlySpan), "read mutated encoded bytes");
            };
        }
    }

    /// <summary>Checks null activation suppression without permitting a null destination to be dereferenced.<br/></summary>
    /// <typeparam name="T">Collection type under test.<br/></typeparam>
    /// <param name="cases">Destination registry.<br/></param>
    /// <param name="selected">Whether the collection is selected from a parent.<br/></param>
    /// <param name="source">Populated source collection.<br/></param>
    private static void AddNull<T>(Dictionary<string, Action> cases, bool selected, T source) where T : class
    {
        cases["null-activation/" + typeof(T).Name + "/" + (selected ? "selected" : "root")] = () =>
        {
            var binary = InhetoBinary.FromObject(selected ? (object)new ReviewHolder<T> { Value = source } : source);
            var options = new DeserializationOptions(); int calls = 0; options.AddActivator<T>(_ => { calls++; return null; });
            T? result = selected ? binary.ReadPropExact<T>(".Value", options) : binary.ToObject<T>(options);
            Require(result is null && calls == 1, "null suppression or activation count=" + calls);
        };
    }

    /// <summary>Enumerates actual value nodes without imposing a family-specific ordering contract.<br/></summary>
    /// <param name="collection">Constructed diagnostic collection.<br/></param>
    /// <returns>Dictionary values or enumerable entries.<br/></returns>
    private static IEnumerable<ReviewNode> Nodes(object collection) => collection is IDictionary dictionary
        ? dictionary.Values.Cast<ReviewNode>() : ((IEnumerable)collection).Cast<ReviewNode>();

    /// <summary>Fails one diagnostic case without treating partial state as acceptance.<br/></summary>
    /// <param name="condition">Required invariant.<br/></param>
    /// <param name="message">Bounded fixture diagnostic.<br/></param>
    private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
}

/// <summary>Provides scalar and referent members shared by diagnostic collection subclasses.<br/></summary>
public interface ITagged
{
    /// <summary>Gets or sets the independent scalar member.<br/></summary>
    int Tag { get; set; }
    /// <summary>Gets or sets the independent or aliased node member.<br/></summary>
    ReviewNode? Hero { get; set; }
}
/// <summary>Defines a cycle-capable diagnostic value.<br/></summary>
public sealed class ReviewNode
{
    /// <summary>Gets or sets the numeric identity.<br/></summary>
    public int Value { get; set; }
    /// <summary>Gets or sets the linked diagnostic value.<br/></summary>
    public ReviewNode? Next { get; set; }
}
/// <summary>Places a collection beneath an ordinary complex root.<br/></summary>
/// <typeparam name="T">Collection or projection type.<br/></typeparam>
public sealed class ReviewHolder<T>
{
    /// <summary>Gets or sets the selected collection.<br/></summary>
    public T? Value { get; set; }
}
/// <summary>Exercises inherited list entries and declared members.<br/></summary>
public sealed class TaggedList : List<ReviewNode>, ITagged
{
    /// <summary>Gets or sets the scalar member.<br/></summary>
    public int Tag { get; set; }
    /// <summary>Gets or sets the node member.<br/></summary>
    public ReviewNode? Hero { get; set; }
}
/// <summary>Exercises inherited queue entries and declared members.<br/></summary>
public sealed class TaggedQueue : Queue<ReviewNode>, ITagged
{
    /// <summary>Gets or sets the scalar member.<br/></summary>
    public int Tag { get; set; }
    /// <summary>Gets or sets the node member.<br/></summary>
    public ReviewNode? Hero { get; set; }
}
/// <summary>Exercises inherited stack entries and declared members.<br/></summary>
public sealed class TaggedStack : Stack<ReviewNode>, ITagged
{
    /// <summary>Gets or sets the scalar member.<br/></summary>
    public int Tag { get; set; }
    /// <summary>Gets or sets the node member.<br/></summary>
    public ReviewNode? Hero { get; set; }
}
/// <summary>Exercises inherited set entries and declared members.<br/></summary>
public sealed class TaggedSet : HashSet<ReviewNode>, ITagged
{
    /// <summary>Gets or sets the scalar member.<br/></summary>
    public int Tag { get; set; }
    /// <summary>Gets or sets the node member.<br/></summary>
    public ReviewNode? Hero { get; set; }
}
/// <summary>Exercises inherited observable entries and declared members.<br/></summary>
public sealed class TaggedObservable : ObservableCollection<ReviewNode>, ITagged
{
    /// <summary>Gets or sets the scalar member.<br/></summary>
    public int Tag { get; set; }
    /// <summary>Gets or sets the node member.<br/></summary>
    public ReviewNode? Hero { get; set; }
}
/// <summary>Exercises inherited dictionary entries and declared members.<br/></summary>
public sealed class TaggedDictionary : Dictionary<string, ReviewNode>, ITagged
{
    /// <summary>Gets or sets the scalar member.<br/></summary>
    public int Tag { get; set; }
    /// <summary>Gets or sets the node member.<br/></summary>
    public ReviewNode? Hero { get; set; }
}
/// <summary>Defines distinct scalar enum values for stack enumeration checks.<br/></summary>
public enum ReviewEnum { First = 1, Second = 2, Third = 3 }
/// <summary>Provides an object-typed back-reference to an enclosing collection.<br/></summary>
public sealed class ObjectOwnerNode
{
    /// <summary>Gets or sets the owning collection.<br/></summary>
    public object? Owner { get; set; }
}
/// <summary>Provides a strongly typed back-reference to an enclosing collection.<br/></summary>
public sealed class TypedOwnerNode
{
    /// <summary>Gets or sets the owning collection.<br/></summary>
    public List<TypedOwnerNode>? Owner { get; set; }
}
/// <summary>Provides an inherited dictionary member referencing the dictionary itself.<br/></summary>
public sealed class SelfDictionary : Dictionary<string, int>
{
    /// <summary>Gets or sets the self-reference.<br/></summary>
    public SelfDictionary? Self { get; set; }
}
/// <summary>Provides an initialized read-only member that retains supplied destination topology.<br/></summary>
public sealed class ReadonlyMembersList : List<ReviewNode>
{
    /// <summary>Gets the pre-existing node to fill without replacing it.<br/></summary>
    public ReviewNode Hero { get; } = new();
}
/// <summary>Provides an ordinary object-valued member unrelated to container references.<br/></summary>
public sealed class ObjectPayloadNode
{
    /// <summary>Gets or sets the scalar or reference payload.<br/></summary>
    public object? Payload { get; set; }
}
