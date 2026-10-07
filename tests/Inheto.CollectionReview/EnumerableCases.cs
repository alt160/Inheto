using System.Collections;
using System.Collections.Concurrent;
using Inheto;

/// <summary>Checks generic-enumerable wire sources independently from numbered native collection sources.<br/></summary>
internal static class EnumerableCases
{
    private static readonly string[] Routes = { "root", "selected", "runtime", "generic-fill", "runtime-fill", "parent", "parent-fill" };

    /// <summary>Registers typed destinations, derived members, ownership, alias, codec and failure boundaries.<br/></summary>
    /// <param name="cases">Independent-process scenario registry.<br/></param>
    internal static void Add(Dictionary<string, Action> cases)
    {
        Families(cases, "int", new[] { 1, 2, 3 }, new[] { 1, 2, 3 });
        Families(cases, "int-string", new[] { 1, 2, 3 }, new[] { "1", "2", "3" });
        Families(cases, "string-int", new[] { "1", "2", "3" }, new[] { 1, 2, 3 });
        Families(cases, "nullable", new int?[] { 1, null, 3 }, new int?[] { 1, null, 3 });
        Families(cases, "enum", new[] { ReviewEnum.First, ReviewEnum.Second, ReviewEnum.Third }, new[] { ReviewEnum.First, ReviewEnum.Second, ReviewEnum.Third });
        Families(cases, "empty", Array.Empty<int>(), Array.Empty<int>());
        foreach (string route in Routes)
        {
            cases[$"enumerable/derived/{route}"] = () =>
            {
                var source = new ConversionLinkedList<int> { Tag = 7 }; foreach (int value in new[] { 1, 2, 3 }) source.AddLast(value);
                var actual = Read<ConversionLinkedList<string>>(source, () => new(), route);
                Require(actual is not null && actual.Tag == 7 && actual.SequenceEqual(new[] { "1", "2", "3" }), "derived enumerable members/elements lost");
            };
            cases[$"enumerable/owner/{route}"] = () =>
            {
                var source = new LinkedList<ObjectOwnerNode>(); source.AddLast(new ObjectOwnerNode { Owner = source });
                var actual = Read<LinkedList<ObjectOwnerNode>>(source, () => new(), route)!;
                Require(actual.Count == 1 && ReferenceEquals(actual, actual.First!.Value.Owner), "generic-enumerable owner reference detached");
            };
            cases[$"enumerable/self/{route}"] = () =>
            {
                var source = new SelfEnumerable(); source.AddLast(1); source.Self = source;
                var actual = Read<SelfEnumerable>(source, () => new(), route)!;
                Require(actual.Count == 1 && ReferenceEquals(actual, actual.Self), "declared-member cycle duplicated or detached");
            };
            cases[$"enumerable/arrays/{route}"] = () =>
            {
                var source = new LinkedList<int[]>(); source.AddLast(new[] { 1, 2 }); source.AddLast(new[] { 3 });
                var actual = Read<Stack<int[]>>(source, () => new(), route)!;
                Require(actual.Select(value => value[0]).SequenceEqual(new[] { 1, 3 }), "array children incorrectly activated or reordered");
            };
            cases[$"enumerable/codec/{route}"] = () =>
            {
                string prefix = route is "selected" or "parent" or "parent-fill" ? ".Value" : "";
                var write = new SerializationOptions(); var read = new DeserializationOptions();
                for (int i = 0; i < 3; i++) { write.AddSerializer<int>(prefix + "#" + i, value => BitConverter.GetBytes(value)); read.AddDeserializer<string>(prefix + "#" + i, bytes => "c" + BitConverter.ToInt32(bytes)); }
                var actual = Read<Stack<string>>(new LinkedList<int>(new[] { 1, 2, 3 }), () => new(), route, read, new InhetoSerializer(write))!;
                Require(actual.SequenceEqual(new[] { "c1", "c2", "c3" }), "custom enumerable child decoder/order lost");
            };
        }
        cases["enumerable/alias-codec"] = () =>
        {
            var write = new SerializationOptions(); write.AddSerializer<int>(".Value#0", value => BitConverter.GetBytes(value));
            var read = new DeserializationOptions(); read.Aliases.Add(".Value", ".Renamed");
            read.AddDeserializer<string>(".Value#0", _ => "canonical"); read.AddDeserializer<string>(".Renamed#0", _ => "requested");
            var binary = InhetoBinary.FromObject(new ReviewHolder<LinkedList<int>> { Value = new(new[] { 1 }) }, write);
            Require(binary.ReadPropAs<LinkedList<string>>(".Renamed", read)!.Single() == "requested", "requested-path codec precedence lost");
        };
        cases["enumerable/supplied-comparer-seed"] = () =>
        {
            var comparer = StringComparer.OrdinalIgnoreCase; var target = new HashSet<string>(comparer) { "seed" };
            InhetoBinary.FromObject(new LinkedList<string>(new[] { "ONE", "one" })).ReadPropOnto(target, "");
            Require(ReferenceEquals(target.Comparer, comparer) && target.SetEquals(new[] { "seed", "ONE" }), "supplied comparer/seed replaced");
        };
        cases["enumerable/independent"] = () =>
        {
            var binary = InhetoBinary.FromObject(new LinkedList<ReviewNode>(new[] { new ReviewNode { Value = 1 } }));
            var first = binary.ToObject<LinkedList<ReviewNode>>()!; var second = binary.ToObject<LinkedList<ReviewNode>>()!;
            Require(!ReferenceEquals(first, second) && !ReferenceEquals(first.First!.Value, second.First!.Value), "independent operations shared results");
        };
        cases["enumerable/null-activation"] = () =>
        {
            int calls = 0; var options = new DeserializationOptions(); options.AddActivator<LinkedList<int>>(_ => { calls++; return null!; });
            Require(InhetoBinary.FromObject(new LinkedList<int>(new[] { 1 })).ToObject<LinkedList<int>>(options) is null && calls == 1, "intentional null was retried");
        };
        cases["enumerable/failure-retry"] = () =>
        {
            var binary = InhetoBinary.FromObject(new LinkedList<ReviewNode>(new[] { new ReviewNode { Value = 1 } }));
            var options = new DeserializationOptions(); options.AddActivator<ReviewNode>(_ => throw new InvalidOperationException("sentinel"));
            bool caught = false; try { binary.ToObject<LinkedList<ReviewNode>>(options); } catch (InvalidOperationException error) when (error.Message == "sentinel") { caught = true; }
            Require(caught && binary.ToObject<LinkedList<ReviewNode>>()!.First!.Value.Value == 1, "failure poisoned next operation");
        };
        cases["enumerable/reentry"] = () =>
        {
            var binary = InhetoBinary.FromObject(new LinkedList<ReviewNode>(new[] { new ReviewNode { Value = 1 } }));
            var options = new DeserializationOptions(); options.AddActivator<ReviewNode>(_ => { Require(binary.ReadPropExact<int>("#0.Value") == 1, "nested read failed"); return new ReviewNode(); });
            Require(binary.ToObject<LinkedList<ReviewNode>>(options)!.First!.Value.Value == 1, "nested read corrupted outer operation");
        };
        cases["enumerable/direct-navigation"] = () =>
        {
            var options = new DeserializationOptions(); options.AddActivator<LinkedList<int>>(_ => throw new InvalidOperationException("activated"));
            Require(InhetoBinary.FromObject(new LinkedList<int>(new[] { 42 })).ReadPropExact<int>("#0", options) == 42, "element navigation reconstructed container");
        };
        cases["enumerable/legacy-object-array"] = () =>
        {
            var binary = InhetoBinary.FromObject(new LinkedList<int>(new[] { 1, 2, 3 }));
            Require(binary.ToObject<object[]>()!.SequenceEqual(new object[] { 1, 2, 3 }), "existing object-array projection changed");
        };
        cases["enumerable/legacy-object-list"] = () =>
        {
            var binary = InhetoBinary.FromObject(new LinkedList<ReviewNode>(new[] { new ReviewNode { Value = 42 } }));
            var actual = binary.ToObject<List<object>>()!;
            Require(actual.Count == 1 && actual[0] is IInhetoObject, "legacy object projection ceased returning a navigable Inheto wrapper");
        };
        cases["enumerable/declined-shared"] = () =>
        {
            var source = new LinkedList<int>(new[] { 1 }); var options = new DeserializationOptions(); int calls = 0; options.AddActivator<LinkedList<int>>(_ => { calls++; return null!; });
            var actual = InhetoBinary.FromObject(new EnumerablePair<LinkedList<int>> { A = source, B = source }).ToObject<EnumerablePair<LinkedList<int>>>(options)!;
            Require(actual.A is null && actual.B is null && calls == 1, "declined shared container was reconstructed again");
        };
        cases["enumerable/supplied-aliases"] = () =>
        {
            var source = new SelfEnumerable(); source.AddLast(1); source.Self = source;
            var first = new SelfEnumerable(); var second = new SelfEnumerable();
            var target = new EnumerablePair<SelfEnumerable> { A = first, B = second };
            InhetoBinary.FromObject(new EnumerablePair<SelfEnumerable> { A = source, B = source }).ReadPropOnto(target, "");
            Require(ReferenceEquals(target.A, first) && ReferenceEquals(target.B, second) && first.Count == 1 && second.Count == 1 && ReferenceEquals(first.Self, first) && ReferenceEquals(second.Self, second), "independent supplied alias topology changed");
        };
    }

    /// <summary>Registers typed destinations while retaining the same generic-enumerable source representation.<br/></summary>
    /// <typeparam name="TSource">Encoded element type.<br/></typeparam>
    /// <typeparam name="TTarget">Destination element type.<br/></typeparam>
    /// <param name="cases">Scenario registry.<br/></param>
    /// <param name="label">Conversion label.<br/></param>
    /// <param name="source">Source values.<br/></param>
    /// <param name="expected">Expected values.<br/></param>
    private static void Families<TSource, TTarget>(Dictionary<string, Action> cases, string label, TSource[] source, TTarget[] expected)
    {
        Family<List<TTarget>, TSource, TTarget>(cases, label, source, expected, () => new());
        Family<LinkedList<TTarget>, TSource, TTarget>(cases, label, source, expected, () => new());
        Family<Queue<TTarget>, TSource, TTarget>(cases, label, source, expected, () => new());
        Family<Stack<TTarget>, TSource, TTarget>(cases, label, source, expected, () => new());
        Family<ConcurrentStack<TTarget>, TSource, TTarget>(cases, label, source, expected, () => new());
        Family<HashSet<TTarget>, TSource, TTarget>(cases, label, source, expected, () => new(), true);
    }

    /// <summary>Checks a closed destination across seven root/member/create/fill routes.<br/></summary>
    /// <typeparam name="TCollection">Destination collection type.<br/></typeparam>
    /// <typeparam name="TSource">Encoded element type.<br/></typeparam>
    /// <typeparam name="TTarget">Requested element type.<br/></typeparam>
    /// <param name="cases">Scenario registry.<br/></param>
    /// <param name="label">Conversion label.<br/></param>
    /// <param name="source">Source values.<br/></param>
    /// <param name="expected">Projected values.<br/></param>
    /// <param name="create">Supplied destination factory.<br/></param>
    /// <param name="set">Checks uniqueness/membership instead of ordered enumeration.<br/></param>
    private static void Family<TCollection, TSource, TTarget>(Dictionary<string, Action> cases, string label, TSource[] source, TTarget[] expected, Func<TCollection> create, bool set = false) where TCollection : class, IEnumerable<TTarget>
    {
        foreach (string route in Routes) cases[$"enumerable/{typeof(TCollection).Name}/{label}/{route}"] = () =>
        {
            var actual = Read<TCollection>(new LinkedList<TSource>(source), create, route);
            Require(actual is not null && (set ? actual.Count() == expected.Length && new HashSet<TTarget>(actual).SetEquals(expected) : actual.SequenceEqual(expected)), "generic enumerable values/order/count differ");
        };
    }

    /// <summary>Uses a single production public route without nesting an independent public read inside materialization.<br/></summary>
    /// <typeparam name="T">Destination type.<br/></typeparam>
    /// <param name="source">Real generic enumerable wire source.<br/></param>
    /// <param name="create">Supplied instance factory.<br/></param>
    /// <param name="route">Public operation label.<br/></param>
    /// <param name="options">Optional activation/codec policy.<br/></param>
    /// <param name="serializer">Optional custom encoder.<br/></param>
    /// <returns>The actual returned or developer-owned destination.<br/></returns>
    private static T? Read<T>(object source, Func<T> create, string route, DeserializationOptions? options = null, InhetoSerializer? serializer = null) where T : class
    {
        bool selected = route is "selected" or "parent" or "parent-fill";
        object root = selected ? new ReviewHolder<object> { Value = source } : source;
        var binary = serializer is null ? InhetoBinary.FromObject(root) : InhetoBinary.FromStream(serializer.Serialize(root));
        byte[] before = binary.RawBytes.ToArray();
        string path = selected ? ".Value" : "";
        T? result;
        if (route == "selected") result = binary.ReadPropExact<T>(path, options);
        else if (route == "runtime") result = (T?)binary.ToObject(typeof(T), options);
        else if (route == "parent") result = binary.ToObject<ReviewHolder<T>>(options)!.Value;
        else if (route == "parent-fill") { var target = new ReviewHolder<T> { Value = create() }; binary.ReadPropOnto(target, "", options); result = target.Value; }
        else if (route is "generic-fill" or "runtime-fill") { result = create(); if (route == "generic-fill") binary.ReadPropOnto(result, path, options); else binary.ReadPropOnto(typeof(T), result, path, options); }
        else result = binary.ToObject<T>(options);
        Require(binary.RawBytes.ToArray().AsSpan().SequenceEqual(before), "wire bytes mutated by reconstruction");
        return result;
    }

    /// <summary>Reports a required acceptance invariant without skips or expected-failure masks.<br/></summary>
    /// <param name="condition">Required invariant.<br/></param>
    /// <param name="message">Failure description.<br/></param>
    private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
}

/// <summary>Provides a generic-enumerable declared member that references the containing collection.<br/></summary>
public sealed class SelfEnumerable : LinkedList<int>
{
    /// <summary>Gets or sets the container self-reference.<br/></summary>
    public SelfEnumerable? Self { get; set; }
}

/// <summary>Provides two serialized aliases or independently supplied collection destinations.<br/></summary>
public sealed class EnumerablePair<T>
{
    /// <summary>Gets or sets the first destination.<br/></summary>
    public T? A { get; set; }
    /// <summary>Gets or sets the second destination.<br/></summary>
    public T? B { get; set; }
}
