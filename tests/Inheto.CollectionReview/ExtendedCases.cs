using System.Collections.Concurrent;
using System.Numerics;
using Inheto;

/// <summary>Extends the original reproductions with destination-order, identity, alias, and direct-navigation acceptance boundaries.<br/></summary>
internal static class ExtendedCases
{
    /// <summary>Registers current-fix boundaries without changing the original diagnostic assertions.<br/></summary>
    /// <param name="cases">Named independent-process acceptance scenarios.<br/></param>
    internal static void Add(Dictionary<string, Action> cases)
    {
        AddStacks(cases, "bool", new[] { true, false, true, false });
        AddStacks(cases, "byte", new byte[] { 1, 2, 3 });
        AddStacks(cases, "sbyte", new sbyte[] { -1, 2, 3 });
        AddStacks(cases, "short", new short[] { -1, 2, 3 });
        AddStacks(cases, "ushort", new ushort[] { 1, 2, 3 });
        AddStacks(cases, "int", new[] { 1, 2, 3 });
        AddStacks(cases, "uint", new uint[] { 1, 2, 3 });
        AddStacks(cases, "long", new long[] { -1, 2, 3 });
        AddStacks(cases, "ulong", new ulong[] { 1, 2, 3 });
        AddStacks(cases, "int128", new Int128[] { -1, 2, 3 });
        AddStacks(cases, "uint128", new UInt128[] { 1, 2, 3 });
        AddStacks(cases, "half", new Half[] { (Half)1, (Half)2, (Half)3 });
        AddStacks(cases, "float", new[] { 1f, 2f, 3f });
        AddStacks(cases, "double", new[] { 1d, 2d, 3d });
        AddStacks(cases, "decimal", new[] { 1m, 2m, 3m });
        AddStacks(cases, "char", new[] { 'a', 'b', 'c' });
        AddStacks(cases, "string", new string?[] { "first", null, "last" });
        AddStacks(cases, "dateonly", new[] { new DateOnly(2026, 1, 1), new DateOnly(2026, 2, 2), new DateOnly(2026, 3, 3) });
        AddStacks(cases, "datetime", new[] { new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 2), new DateTime(2026, 3, 3) });
        AddStacks(cases, "datetimeoffset", new[] { DateTimeOffset.UnixEpoch, DateTimeOffset.UnixEpoch.AddDays(1), DateTimeOffset.UnixEpoch.AddDays(2) });
        AddStacks(cases, "timeonly", new[] { new TimeOnly(1, 2), new TimeOnly(2, 3), new TimeOnly(3, 4) });
        AddStacks(cases, "timespan", new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3) });
        AddStacks(cases, "guid", new[] { Guid.Parse("00000001-0000-0000-0000-000000000000"), Guid.Parse("00000002-0000-0000-0000-000000000000"), Guid.Parse("00000003-0000-0000-0000-000000000000") });
        AddStacks(cases, "biginteger", new BigInteger[] { 1, 2, 3 });
        AddStacks(cases, "vector2", new[] { new Vector2(1), new Vector2(2), new Vector2(3) });
        AddStacks(cases, "vector3", new[] { new Vector3(1), new Vector3(2), new Vector3(3) });
        AddStacks(cases, "vector4", new[] { new Vector4(1), new Vector4(2), new Vector4(3) });
        AddStacks(cases, "quaternion", new[] { new Quaternion(1, 2, 3, 4), new Quaternion(2, 3, 4, 5), new Quaternion(3, 4, 5, 6) });
        AddStacks(cases, "complex", new[] { new Complex(1, 2), new Complex(2, 3), new Complex(3, 4) });
        AddStacks(cases, "nullable", new int?[] { 1, null, 3 });
        AddStacks(cases, "enum", new[] { ReviewEnum.First, ReviewEnum.Second, ReviewEnum.Third });
        AddStacks(cases, "empty", Array.Empty<int>());
        AddStacks(cases, "single", new[] { 42 });
        foreach (bool concurrent in new[] { false, true })
        foreach (string route in new[] { "root", "selected", "generic-fill", "runtime-fill", "parent" })
        {
            cases[$"extended/stack-codec/{concurrent}/{route}"] = () => CodecStack(concurrent, route);
            cases[$"extended/stack-coercion/{concurrent}/{route}"] = () => CoercionStack(concurrent, route);
            cases[$"extended/stack-array/{concurrent}/{route}"] = () => ArrayStack(concurrent, route);
        }
        foreach (bool selected in new[] { false, true })
        {
            cases[$"extended/direct-navigation/{selected}"] = () =>
            {
                var source = new TaggedList { Tag = 7, Hero = new() { Value = 900 } }; source.Add(new() { Value = 100 });
                var binary = InhetoBinary.FromObject(selected ? (object)new ReviewHolder<TaggedList> { Value = source } : source);
                var options = new DeserializationOptions();
                options.AddActivator<TaggedList>(_ => throw new InvalidOperationException("container activation during navigation"));
                Require(binary.ReadPropExact<int>((selected ? ".Value" : "") + "#0.Value", options) == 100, "numbered element lookup required container reconstruction");
            };
            cases[$"extended/derived-codec/{selected}"] = () =>
            {
                string path = selected ? ".Value" : "";
                var options = new SerializationOptions(); options.AddSerializer<int>(path + ".Tag", value => BitConverter.GetBytes(value));
                var serializer = new InhetoSerializer(options);
                var source = new TaggedList { Tag = 7 }; source.Add(new() { Value = 1 });
                var binary = InhetoBinary.FromStream(serializer.Serialize(selected ? (object)new ReviewHolder<TaggedList> { Value = source } : source));
                var readOptions = new DeserializationOptions(); readOptions.AddDeserializer<int>(path + ".Tag", bytes => BitConverter.ToInt32(bytes));
                var actual = selected ? binary.ReadPropExact<TaggedList>(path, readOptions)! : binary.ToObject<TaggedList>(readOptions)!;
                Require(actual.Tag == 7 && actual.Count == 1, "direct derived member codec omitted");
            };
        }
        cases["extended/derived-alias"] = () =>
        {
            var source = new TaggedList { Tag = 7, Hero = new() { Value = 900 } }; source.Add(new() { Value = 100 });
            var options = new DeserializationOptions(); options.Aliases.Add(".Value", ".Renamed");
            var actual = InhetoBinary.FromObject(new ReviewHolder<TaggedList> { Value = source }).ReadPropExact<TaggedList>(".Renamed", options)!;
            Require(actual.Tag == 7 && actual.Hero?.Value == 900 && actual.Count == 1, "aliased derived collection members lost");
        };
        cases["extended/object-member-alias"] = () =>
        {
            var options = new DeserializationOptions(); options.Aliases.Add(".Value.Payload", ".Value.Renamed");
            var actual = InhetoBinary.FromObject(new ReviewHolder<ObjectPayloadNode> { Value = new() { Payload = 123 } }).ToObject<ReviewHolder<RenamedPayload>>(options)!;
            Require(actual.Value!.Renamed is int value && value == 123, "resolved full object member path lost alias");
        };
        cases["extended/supplied-container-topology"] = () =>
        {
            var source = new SelfDictionary { ["one"] = 1 }; source.Self = source;
            var first = new SelfDictionary(); var second = new SelfDictionary();
            var destination = new DictionaryAliases { B = first, A = second };
            var actual = InhetoBinary.FromObject(new DictionaryAliases { B = source, A = source });
            actual.ReadPropOnto(destination, "");
            Require(ReferenceEquals(first, destination.B) && ReferenceEquals(second, destination.A) && !ReferenceEquals(first, second), "supplied collections replaced");
            Require(ReferenceEquals(first.Self, first) && ReferenceEquals(second.Self, second) && first.Count == 1 && second.Count == 1, "temporary supplied collection publication leaked");
        };
        cases["extended/dictionary-owner"] = () =>
        {
            var source = new Dictionary<string, ObjectOwnerNode>(); source["first"] = new() { Owner = source };
            var actual = InhetoBinary.FromObject(source).ToObject<Dictionary<string, ObjectOwnerNode>>()!;
            Require(ReferenceEquals(actual, actual["first"].Owner), "dictionary owner reference copied");
        };
        cases["extended/stack-supplied-seed"] = () =>
        {
            var source = new Stack<int>(); source.Push(1); source.Push(2); source.Push(3);
            var supplied = new Stack<int>(); supplied.Push(99);
            InhetoBinary.FromObject(source).ReadPropOnto(supplied, "");
            Require(supplied.SequenceEqual(new[] { 3, 2, 1, 99 }), "supplied stack contents cleared or reordered");
        };
    }

    /// <summary>Registers ordinary and concurrent stack coverage across generic/runtime create and fill plus parent reconstruction.<br/></summary>
    /// <typeparam name="T">Declared scalar, promoted, nullable, or enum element type.<br/></typeparam>
    /// <param name="cases">Acceptance scenario registry.<br/></param>
    /// <param name="label">Stable shape label.<br/></param>
    /// <param name="values">Input push order; the expected enumeration is derived from the source stack.<br/></param>
    private static void AddStacks<T>(Dictionary<string, Action> cases, string label, T[] values)
    {
        AddRoutes<Stack<T>, T>(cases, label, () => new(), (stack, value) => stack.Push(value), values);
        AddRoutes<ConcurrentStack<T>, T>(cases, label, () => new(), (stack, value) => stack.Push(value), values);
    }

    /// <summary>Exercises one closed destination type using the exact public overload selected by each route.<br/></summary>
    /// <typeparam name="TCollection">Stack family destination type.<br/></typeparam>
    /// <typeparam name="T">Element type.<br/></typeparam>
    /// <param name="cases">Acceptance registry.<br/></param>
    /// <param name="label">Element-shape label.<br/></param>
    /// <param name="create">Constructs the source or supplied destination.<br/></param>
    /// <param name="push">Adds source values in declared push order.<br/></param>
    /// <param name="values">Source push sequence.<br/></param>
    private static void AddRoutes<TCollection, T>(Dictionary<string, Action> cases, string label, Func<TCollection> create, Action<TCollection, T> push, T[] values) where TCollection : class, IEnumerable<T>
    {
        foreach (string route in new[] { "root", "selected", "runtime-create", "generic-fill", "runtime-fill", "parent" })
            cases[$"extended/order/{typeof(TCollection).Name}/{label}/{route}"] = () =>
            {
                TCollection source = create(); foreach (T value in values) push(source, value);
                TCollection actual = RoundTrip(source, create, route);
                Require(actual.SequenceEqual(source), "destination stack order differs from encoded source");
            };
    }

    /// <summary>Reconstructs or fills a collection through one specified public route without sharing result instances across operations.<br/></summary>
    /// <typeparam name="T">Closed collection type.<br/></typeparam>
    /// <param name="source">Source collection.<br/></param>
    /// <param name="create">Supplied destination factory.<br/></param>
    /// <param name="route">Public create/fill route label.<br/></param>
    /// <param name="options">Optional read policy.<br/></param>
    /// <param name="serializer">Optional custom writer.<br/></param>
    /// <returns>Returned or caller-owned destination.<br/></returns>
    private static T RoundTrip<T>(T source, Func<T> create, string route, DeserializationOptions? options = null, InhetoSerializer? serializer = null) where T : class
    {
        bool selected = route is "selected" or "parent";
        object root = selected ? new ReviewHolder<T> { Value = source } : source;
        var binary = serializer is null ? InhetoBinary.FromObject(root) : InhetoBinary.FromStream(serializer.Serialize(root));
        string path = selected ? ".Value" : "";
        if (route == "parent") return binary.ToObject<ReviewHolder<T>>(options)!.Value!;
        if (route == "selected") return binary.ReadPropExact<T>(path, options)!;
        if (route == "runtime-create") return (T)binary.ToObject(typeof(T), options)!;
        if (route is "generic-fill" or "runtime-fill")
        {
            T supplied = create();
            if (route == "generic-fill") binary.ReadPropOnto(supplied, path, options);
            else binary.ReadPropOnto(typeof(T), supplied, path, options);
            return supplied;
        }
        return binary.ToObject<T>(options)!;
    }

    /// <summary>Checks custom child decoding order without coercing replacement values through historical source markers.<br/></summary>
    /// <param name="concurrent">Whether to use ConcurrentStack rather than Stack.<br/></param>
    /// <param name="route">Public read/fill route.<br/></param>
    private static void CodecStack(bool concurrent, string route)
    {
        string path = route is "selected" or "parent" ? ".Value" : "";
        var write = new SerializationOptions(); var read = new DeserializationOptions();
        for (int i = 0; i < 3; i++)
        {
            write.AddSerializer<int>(path + "#" + i, value => BitConverter.GetBytes(value));
            read.AddDeserializer<int>(path + "#" + i, bytes => BitConverter.ToInt32(bytes));
        }
        var serializer = new InhetoSerializer(write);
        if (concurrent)
        {
            var source = new ConcurrentStack<int>(new[] { 1, 2, 3 });
            Require(RoundTrip(source, () => new ConcurrentStack<int>(), route, read, serializer).SequenceEqual(source), "concurrent custom stack order");
        }
        else
        {
            var source = new Stack<int>(new[] { 1, 2, 3 });
            Require(RoundTrip(source, () => new Stack<int>(), route, read, serializer).SequenceEqual(source), "custom stack order");
        }
    }

    /// <summary>Checks destination-driven Int32-to-string projection while preserving the source's encoded enumeration.<br/></summary>
    /// <param name="concurrent">Whether the source and destination are concurrent stack families.<br/></param>
    /// <param name="route">Public typed projection/fill route.<br/></param>
    private static void CoercionStack(bool concurrent, string route)
    {
        bool selected = route is "selected" or "parent";
        object source = concurrent ? new ConcurrentStack<int>(new[] { 1, 2, 3 }) : new Stack<int>(new[] { 1, 2, 3 });
        object root = selected ? new ReviewHolder<object> { Value = source } : source;
        var binary = InhetoBinary.FromObject(root);
        IEnumerable<string> result;
        if (concurrent)
        {
            if (route == "parent") result = binary.ToObject<ReviewHolder<ConcurrentStack<string>>>()!.Value!;
            else if (route == "selected") result = binary.ReadPropAs<ConcurrentStack<string>>(".Value")!;
            else if (route is "generic-fill" or "runtime-fill") { var target = new ConcurrentStack<string>(); if (route == "generic-fill") binary.ReadPropOnto(target, ""); else binary.ReadPropOnto(typeof(ConcurrentStack<string>), target, ""); result = target; }
            else result = binary.ToObject<ConcurrentStack<string>>()!;
        }
        else
        {
            if (route == "parent") result = binary.ToObject<ReviewHolder<Stack<string>>>()!.Value!;
            else if (route == "selected") result = binary.ReadPropAs<Stack<string>>(".Value")!;
            else if (route is "generic-fill" or "runtime-fill") { var target = new Stack<string>(); if (route == "generic-fill") binary.ReadPropOnto(target, ""); else binary.ReadPropOnto(typeof(Stack<string>), target, ""); result = target; }
            else result = binary.ToObject<Stack<string>>()!;
        }
        Require(result.SequenceEqual(new[] { "3", "2", "1" }), "coerced stack order");
    }

    /// <summary>Checks the separate array-element loop rather than scalar or nullable/complex typed helper ordering.<br/></summary>
    /// <param name="concurrent">Stack family selection.<br/></param>
    /// <param name="route">Public reconstruction/fill route.<br/></param>
    private static void ArrayStack(bool concurrent, string route)
    {
        int[][] values = { new[] { 1 }, new[] { 2 }, new[] { 3 } };
        IEnumerable<int[]> actual = concurrent
            ? RoundTrip(new ConcurrentStack<int[]>(values), () => new ConcurrentStack<int[]>(), route)
            : RoundTrip(new Stack<int[]>(values), () => new Stack<int[]>(), route);
        Require(actual.Select(array => array[0]).SequenceEqual(new[] { 3, 2, 1 }), "array stack order");
    }

    /// <summary>Fails an acceptance invariant with a bounded scenario-specific explanation.<br/></summary>
    /// <param name="condition">Required result.<br/></param>
    /// <param name="message">Failure explanation.<br/></param>
    private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
}

/// <summary>Projects a nested object-valued member through a configured full-path alias.<br/></summary>
public sealed class RenamedPayload
{
    /// <summary>Gets or sets the aliased scalar payload.<br/></summary>
    public object? Renamed { get; set; }
}

/// <summary>Retains two independently supplied dictionary projections of one encoded referent.<br/></summary>
public sealed class DictionaryAliases
{
    /// <summary>Gets or sets the canonical destination collection.<br/></summary>
    public SelfDictionary? B { get; set; }
    /// <summary>Gets or sets an independent supplied alias destination.<br/></summary>
    public SelfDictionary? A { get; set; }
}
