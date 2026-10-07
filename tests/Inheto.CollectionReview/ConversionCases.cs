using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Globalization;
using Inheto;

/// <summary>Exercises destination-selected collection projections without weakening earlier lifecycle and order assertions.<br/></summary>
internal static class ConversionCases
{
    private static readonly string[] Routes = { "root", "selected", "exact", "runtime-create", "generic-fill", "runtime-fill", "parent" };

    /// <summary>Registers scalar, nullable, enum, custom-codec, culture, supplied-instance and rejection boundaries.<br/></summary>
    /// <param name="cases">Independent-process scenario registry.<br/></param>
    internal static void Add(Dictionary<string, Action> cases)
    {
        AddFamilies(cases, "int-string", new[] { 1, 2, 3 }, new[] { "1", "2", "3" });
        AddFamilies(cases, "long-int", new long[] { 1, 2, 3 }, new[] { 1, 2, 3 });
        AddFamilies(cases, "string-int", new[] { "1", "2", "3" }, new[] { 1, 2, 3 });
        AddFamilies(cases, "int-nullable", new[] { 1, 2, 3 }, new int?[] { 1, 2, 3 });
        AddFamilies(cases, "nullable-int", new int?[] { 1, null, 3 }, new[] { 1, 0, 3 });
        AddFamilies(cases, "int-enum", new[] { 1, 2, 3 }, new[] { ReviewEnum.First, ReviewEnum.Second, ReviewEnum.Third });
        AddFamilies(cases, "int-object", new[] { 1, 2, 3 }, new object[] { 1, 2, 3 });
        foreach (string route in Routes)
        {
            cases[$"conversion/culture-double-string/{route}"] = () =>
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
                Check<Queue<string>, double, string>(new[] { 1.5, 2.5, 3.5 }, new[] { "1.5", "2.5", "3.5" }, () => new(), route);
            };
            cases[$"conversion/string-guid/{route}"] = () =>
            {
                Guid[] values = { Guid.Parse("00000001-0000-0000-0000-000000000000"), Guid.Parse("00000002-0000-0000-0000-000000000000") };
                Check<List<Guid>, string, Guid>(values.Select(value => value.ToString()).ToArray(), values, () => new(), route);
            };
            cases[$"conversion/codec-int-string/{route}"] = () =>
            {
                string prefix = route is "selected" or "exact" or "parent" ? ".Value" : "";
                var write = new SerializationOptions(); var read = new DeserializationOptions();
                for (int i = 0; i < 3; i++)
                {
                    write.AddSerializer<int>(prefix + "#" + i, value => BitConverter.GetBytes(value));
                    read.AddDeserializer<string>(prefix + "#" + i, bytes => "codec:" + BitConverter.ToInt32(bytes));
                }
                Check<Stack<string>, int, string>(new[] { 1, 2, 3 }, new[] { "codec:1", "codec:2", "codec:3" }, () => new(), route, read, new InhetoSerializer(write));
            };
            cases[$"conversion/overflow-byte/{route}"] = () => RejectLikeParent<byte, int>(new[] { 1, 256 }, route);
            cases[$"conversion/malformed-int/{route}"] = () => RejectLikeParent<int, string>(new[] { "1", "not-a-number" }, route);
            cases[$"conversion/empty/{route}"] = () => Check<Stack<string>, int, string>(Array.Empty<int>(), Array.Empty<string>(), () => new(), route);
            cases[$"conversion/single/{route}"] = () => Check<Stack<string>, int, string>(new[] { 42 }, new[] { "42" }, () => new(), route);
        }
        cases["conversion/alias"] = () =>
        {
            var binary = InhetoBinary.FromObject(new ReviewHolder<Queue<int>> { Value = new(new[] { 1, 2, 3 }) });
            var options = new DeserializationOptions(); options.Aliases.Add(".Value", ".Renamed");
            Require(binary.ReadPropAs<Stack<string>>(".Renamed", options)!.SequenceEqual(new[] { "1", "2", "3" }), "aliased projection changed values/order");
        };
        cases["conversion/supplied-stack"] = () =>
        {
            var binary = InhetoBinary.FromObject(new Queue<int>(new[] { 1, 2, 3 }));
            var target = new Stack<string>(); target.Push("seed");
            var options = new DeserializationOptions(); options.AddActivator<Stack<string>>(_ => throw new InvalidOperationException("supplied destination activated"));
            binary.ReadPropOnto(target, "", options);
            Require(target.SequenceEqual(new[] { "1", "2", "3", "seed" }), "supplied stack lost contents or order");
        };
        cases["conversion/supplied-set-comparer"] = () =>
        {
            var binary = InhetoBinary.FromObject(new Queue<int>(new[] { 1, 2, 3 }));
            var comparer = StringComparer.OrdinalIgnoreCase; var target = new HashSet<string>(comparer) { "seed" };
            binary.ReadPropOnto(target, "");
            Require(ReferenceEquals(target.Comparer, comparer) && target.SetEquals(new[] { "1", "2", "3", "seed" }), "supplied comparer/contents replaced");
        };
        cases["conversion/independent-results"] = () =>
        {
            var binary = InhetoBinary.FromObject(new Queue<int>(new[] { 1, 2, 3 }));
            var first = binary.ToObject<List<string>>()!; var second = binary.ToObject<List<string>>()!;
            Require(!ReferenceEquals(first, second) && first.SequenceEqual(second), "independent projections shared collection instances");
        };
        cases["conversion/null-activator"] = () =>
        {
            var options = new DeserializationOptions(); int calls = 0; options.AddActivator<Stack<string>>(_ => { calls++; return null!; });
            Require(InhetoBinary.FromObject(new Queue<int>(new[] { 1 })).ToObject<Stack<string>>(options) is null && calls == 1, "declined activation traversed/retried");
        };
        AddFamily<ConversionLinkedList<string>, int, string>(cases, "derived-int-string", new[] { 1, 2, 3 }, new[] { "1", "2", "3" }, () => new());
        foreach (string route in Routes)
            cases[$"conversion/linked-same-type/{route}"] = () => Check<LinkedList<int>, int, int>(new[] { 1, 2, 3 }, new[] { 1, 2, 3 }, () => new(), route);
        cases["conversion/linked-mutator-overload"] = () =>
        {
            var target = new LinkedList<int>(); target.AddLast(99);
            Require(EnumerableMutator.TryGetMutator(target, out var mutator), "linked element mutator unavailable");
            mutator!.Add(target, 1); mutator.Add(target, 2); mutator.Add(target, 3);
            Require(target.SequenceEqual(new[] { 99, 1, 2, 3 }), "cached mutator prepended/replaced existing values");
        };
        cases["conversion/linked-derived-members"] = () =>
        {
            var source = new ConversionLinkedList<int> { Tag = 7 }; source.AddLast(1); source.AddLast(2); source.AddLast(3);
            var actual = InhetoBinary.FromObject(source).ToObject<ConversionLinkedList<string>>()!;
            Require(actual.Tag == 7 && actual.SequenceEqual(new[] { "1", "2", "3" }), "derived linked members or scalar order lost");
        };
        cases["conversion/custom-addfirst-unchanged"] = () =>
        {
            var target = new ConversionPrependOnly();
            Require(EnumerableMutator.TryGetMutator(target, out var mutator), "custom AddFirst fallback removed");
            mutator!.Add(target, 1); mutator.Add(target, 2);
            Require(target.Values.SequenceEqual(new[] { 2, 1 }), "non-LinkedList mutation semantics changed");
        };
    }

    /// <summary>Checks the same encoded Queue slots against ten destination families across all public create/fill routes.<br/></summary>
    /// <typeparam name="TSource">Encoded element type.<br/></typeparam>
    /// <typeparam name="TTarget">Requested element type.<br/></typeparam>
    /// <param name="cases">Scenario registry.<br/></param>
    /// <param name="label">Conversion label.<br/></param>
    /// <param name="source">Encoded enumeration.<br/></param>
    /// <param name="expected">Expected projected enumeration, or set contents for unordered destinations.<br/></param>
    private static void AddFamilies<TSource, TTarget>(Dictionary<string, Action> cases, string label, TSource[] source, TTarget[] expected)
    {
        AddFamily<List<TTarget>, TSource, TTarget>(cases, label, source, expected, () => new());
        AddFamily<Queue<TTarget>, TSource, TTarget>(cases, label, source, expected, () => new());
        AddFamily<Stack<TTarget>, TSource, TTarget>(cases, label, source, expected, () => new());
        AddFamily<ConcurrentStack<TTarget>, TSource, TTarget>(cases, label, source, expected, () => new());
        AddFamily<HashSet<TTarget>, TSource, TTarget>(cases, label, source, expected, () => new(), unordered: true);
        AddFamily<ConcurrentQueue<TTarget>, TSource, TTarget>(cases, label, source, expected, () => new());
        AddFamily<LinkedList<TTarget>, TSource, TTarget>(cases, label, source, expected, () => new());
        AddFamily<ObservableCollection<TTarget>, TSource, TTarget>(cases, label, source, expected, () => new());
        AddFamily<ConcurrentBag<TTarget>, TSource, TTarget>(cases, label, source, expected, () => new(), unordered: true);
        // Object has no default ordering contract; the other declared destination types are comparable.
        if (typeof(TTarget) != typeof(object)) AddFamily<SortedSet<TTarget>, TSource, TTarget>(cases, label, source, expected, () => new(), unordered: true);
    }

    /// <summary>Registers one closed destination shape without reflecting inside the production read loop.<br/></summary>
    /// <typeparam name="TCollection">Mutable destination collection.<br/></typeparam>
    /// <typeparam name="TSource">Encoded element type.<br/></typeparam>
    /// <typeparam name="TTarget">Requested element type.<br/></typeparam>
    /// <param name="cases">Scenario registry.<br/></param>
    /// <param name="label">Conversion label.<br/></param>
    /// <param name="source">Encoded slots.<br/></param>
    /// <param name="expected">Required projected values.<br/></param>
    /// <param name="create">Caller-owned destination factory.<br/></param>
    /// <param name="unordered">Whether only count and set contents are contractual.<br/></param>
    private static void AddFamily<TCollection, TSource, TTarget>(Dictionary<string, Action> cases, string label, TSource[] source, TTarget[] expected, Func<TCollection> create, bool unordered = false) where TCollection : class, IEnumerable<TTarget>
    {
        foreach (string route in Routes)
            cases[$"conversion/{typeof(TCollection).Name}/{label}/{route}"] = () => Check<TCollection, TSource, TTarget>(source, expected, create, route, unordered: unordered);
    }

    /// <summary>Applies the requested public route and verifies contents/order without using a production private helper as the oracle.<br/></summary>
    /// <typeparam name="TCollection">Requested collection type.<br/></typeparam>
    /// <typeparam name="TSource">Source element type.<br/></typeparam>
    /// <typeparam name="TTarget">Projected element type.<br/></typeparam>
    /// <param name="source">Input enumeration.<br/></param>
    /// <param name="expected">Expected result.<br/></param>
    /// <param name="create">Supplied-instance factory.<br/></param>
    /// <param name="route">Public route label.<br/></param>
    /// <param name="options">Optional activation/codec policy.<br/></param>
    /// <param name="serializer">Optional custom encoder.<br/></param>
    /// <param name="unordered">Checks count and set membership rather than enumeration order.<br/></param>
    private static void Check<TCollection, TSource, TTarget>(TSource[] source, TTarget[] expected, Func<TCollection> create, string route, DeserializationOptions? options = null, InhetoSerializer? serializer = null, bool unordered = false) where TCollection : class, IEnumerable<TTarget>
    {
        var encoded = new Queue<TSource>(source);
        bool selected = route is "selected" or "exact" or "parent";
        object root = selected ? new ReviewHolder<Queue<TSource>> { Value = encoded } : encoded;
        var binary = serializer is null ? InhetoBinary.FromObject(root) : InhetoBinary.FromStream(serializer.Serialize(root));
        string path = selected ? ".Value" : "";
        TCollection actual;
        if (route == "parent") actual = binary.ToObject<ReviewHolder<TCollection>>(options)!.Value!;
        else if (route == "selected") actual = binary.ReadPropAs<TCollection>(path, options)!;
        else if (route == "exact") actual = binary.ReadPropExact<TCollection>(path, options)!;
        else if (route == "runtime-create") actual = (TCollection)binary.ToObject(typeof(TCollection), options)!;
        else if (route is "generic-fill" or "runtime-fill")
        {
            actual = create();
            if (route == "generic-fill") binary.ReadPropOnto(actual, path, options);
            else binary.ReadPropOnto(typeof(TCollection), actual, path, options);
        }
        else actual = binary.ToObject<TCollection>(options)!;
        Require(actual is not null && (unordered ? actual.Count() == expected.Length && new HashSet<TTarget>(actual).SetEquals(expected) : actual.SequenceEqual(expected)), "projected collection values/count/order differ; expected=" + string.Join(',', expected) + "; actual=" + (actual is null ? "null" : string.Join(',', actual)));
    }

    /// <summary>Requires each direct route to reject the same invalid conversion as the established parent route and leave subsequent operations usable.<br/></summary>
    /// <typeparam name="TTarget">Requested scalar type.<br/></typeparam>
    /// <typeparam name="TSource">Encoded scalar type.<br/></typeparam>
    /// <param name="source">At least one invalid numeric/text slot.<br/></param>
    /// <param name="route">Public create/fill route.<br/></param>
    private static void RejectLikeParent<TTarget, TSource>(TSource[] source, string route)
    {
        Type? expected = Failure(() => InhetoBinary.FromObject(new ReviewHolder<Queue<TSource>> { Value = new(source) }).ToObject<ReviewHolder<Queue<TTarget>>>());
        Require(expected is not null, "parent unexpectedly accepted invalid conversion");
        Type? actual = Failure(() => Check<Queue<TTarget>, TSource, TTarget>(source, Array.Empty<TTarget>(), () => new(), route));
        Require(actual == expected, $"rejection differs: parent={expected}, requested={actual}");
        var binary = InhetoBinary.FromObject(new Queue<TSource>(source));
        Failure(() => binary.ToObject<Queue<TTarget>>());
        Require(binary.ToObject<Queue<TSource>>()!.SequenceEqual(source), "failed conversion poisoned a later operation");
    }

    /// <summary>Captures the actual public conversion exception without substituting another acceptable failure.<br/></summary>
    /// <param name="read">Public operation to execute.<br/></param>
    /// <returns>Exception type, or null if no exception occurred.<br/></returns>
    private static Type? Failure(Action read) { try { read(); return null; } catch (Exception exception) { return exception.GetType(); } }

    /// <summary>Fails an acceptance case without silently skipping unsupported shapes.<br/></summary>
    /// <param name="condition">Required invariant.<br/></param>
    /// <param name="message">Diagnostic failure.<br/></param>
    private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
}

/// <summary>Confirms cold LinkedList base discovery and declared-member reconstruction for a concrete subtype.<br/></summary>
public sealed class ConversionLinkedList<T> : LinkedList<T>
{
    /// <summary>Gets or sets the independently encoded derived member.<br/></summary>
    public int Tag { get; set; }
}

/// <summary>Retains a custom AddFirst-only contract outside the LinkedList family.<br/></summary>
public sealed class ConversionPrependOnly
{
    /// <summary>Gets values in the custom prepend order.<br/></summary>
    public List<int> Values { get; } = new();
    /// <summary>Prepends one scalar using the pre-existing custom mutator fallback.<br/></summary>
    /// <param name="value">Value to insert first.<br/></param>
    public void AddFirst(int value) => Values.Insert(0, value);
}
