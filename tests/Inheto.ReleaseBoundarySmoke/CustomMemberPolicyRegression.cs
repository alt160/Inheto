using Inheto;

/// <summary>Protects the 1.1 custom-member policy and collection comparer repair through package-only public calls.<br/></summary>
public static class CustomMemberPolicyRegression
{
    private static int checks;
    private static readonly byte[] Payload = { 1, 3, 5, 7 };

    /// <summary>Runs bounded in-memory scenarios; never opens stores, resolves private types or changes external state.<br/></summary>
    public static void Verify()
    {
        checks = 0;
        var options = new DeserializationOptions();
        Require(options.UnhandledCustomProperties == UnhandledCustomPropertyHandling.Ignore, "default Ignore");
        Expect<ArgumentOutOfRangeException>(() => options.UnhandledCustomProperties = (UnhandledCustomPropertyHandling)2);
        Require(options.UnhandledCustomProperties == UnhandledCustomPropertyHandling.Ignore, "invalid assignment leaves state intact");
        var write = new SerializationOptions();
        write.AddSerializer<Token>(".Value", _ => Payload);
        write.AddSerializer<Token>(".Field", _ => Array.Empty<byte>());
        var binary = InhetoBinary.FromObject(new Record { Name = "stored", Ordinary = Payload }, write);
        byte[] original = binary.RawBytes.ToArray();
        var hydrated = binary.ToObject<Record>()!;
        Require(hydrated.Value?.Text == "constructor" && hydrated.Field?.Text == "field constructor", "initialized members retained");
        Require(hydrated.Name == "stored" && hydrated.Ordinary.AsSpan().SequenceEqual(Payload), "ordinary members unchanged");
        var seed = new Record { Value = new Token { Text = "seed" } };
        var retained = seed.Value;
        binary.ReadPropOnto(seed, "");
        Require(ReferenceEquals(seed.Value, retained) && seed.Name == "stored", "fill retains existing reference");
        Require(binary.ReadPropAs<byte[]>(".Value")!.AsSpan().SequenceEqual(Payload), "explicit bytes unaffected by Ignore");
        var rawOptions = new DeserializationOptions { UnhandledCustomProperties = UnhandledCustomPropertyHandling.AsBinary };
        var raw = binary.ToObject<BinaryRecord>(rawOptions)!;
        Require(raw.Value!.AsSpan().SequenceEqual(Payload) && raw.Field!.Length == 0, "AsBinary exact and empty payloads");
        Expect<InvalidCastException>(() => binary.ToObject<Record>(rawOptions));
        foreach (var handling in Enum.GetValues<UnhandledCustomPropertyHandling>())
        {
            var read = new DeserializationOptions { UnhandledCustomProperties = handling };
            read.AddDeserializer<Token>(".Field", _ => new Token { Text = "field decoded" });
            int calls = 0;
            read.AddDeserializer<Token>(".Value", bytes => { calls++; return new Token { Text = Convert.ToHexString(bytes) }; });
            Require(binary.ToObject<Record>(read)!.Value?.Text == "01030507" && calls == 1, "registered reader precedence");
            read.AddDeserializer<Token>(".Value", _ => null!);
            Require(binary.ToObject<Record>(read)!.Value is null, "intentional null reader result assigned");
            read.AddDeserializer<Token>(".Value", _ => throw new ReaderFailure());
            Expect<ReaderFailure>(() => binary.ToObject<Record>(read));
        }
        var nestedWrite = new SerializationOptions();
        nestedWrite.AddSerializer<Token>(".Nested.Value", _ => Payload);
        var nested = InhetoBinary.FromObject(new Envelope(), nestedWrite);
        Require(nested.ReadPropAs<Record>(".Nested")!.Value?.Text == "constructor", "selected subtree Ignore");
        Require(nested.ReadPropAs<BinaryRecord>(".Nested", rawOptions)!.Value!.AsSpan().SequenceEqual(Payload), "selected subtree AsBinary");
        var alias = new DeserializationOptions();
        alias.Aliases.Add(".Value", ".Renamed");
        Require(binary.ToObject<RenamedRecord>(alias)!.Renamed?.Text == "renamed", "aliased missing reader ignored");
        alias.AddDeserializer<Token>(".Renamed", _ => new Token { Text = "decoded" });
        Require(binary.ToObject<RenamedRecord>(alias)!.Renamed?.Text == "decoded", "aliased reader takes precedence");
        Require(original.AsSpan().SequenceEqual(binary.RawBytes.AsReadOnlySpan), "reads leave stored bytes unchanged");
        var typeWrite = new SerializationOptions();
        typeWrite.AddTypeSerializer<Token>(_ => Payload);
        var typeBinary = InhetoBinary.FromObject(new Record(), typeWrite);
        Expect<InvalidCastException>(() => typeBinary.ToObject<Record>());
        var typeRead = new DeserializationOptions();
        typeRead.AddTypeDeserializer<Token>(_ => new Token { Text = "type decoded" });
        Require(typeBinary.ToObject<Record>(typeRead)!.Value?.Text == "type decoded", "type-wide reader unchanged");
        var comparer = StringComparer.OrdinalIgnoreCase;
        var comparerRead = new DeserializationOptions();
        comparerRead.AddTypeComparer<SortedSet<string>, string>(comparer);
        var setBinary = InhetoBinary.FromObject(new SetRecord { Values = new SortedSet<string>(StringComparer.Ordinal) { "A", "a" } });
        var set = setBinary.ToObject<SetRecord>(comparerRead)!.Values!;
        Require(set.Count == 1 && ReferenceEquals(set.Comparer, comparer), "type comparer belongs to collection, not owner");
        Console.WriteLine($"PASS custom-member policy/comparer: {checks} package-only checks on .NET {Environment.Version}.");
    }

    /// <summary>Counts one condition and throws a descriptive test failure when the public contract is violated.<br/></summary>
    /// <param name="condition">Observed contract predicate.<br/></param><param name="name">Bounded scenario label.<br/></param>
    private static void Require(bool condition, string name)
    {
        if (!condition) throw new InvalidOperationException("Custom-member regression: " + name);
        checks++;
    }

    /// <summary>Requires a specific exception class without suppressing a different application failure.<br/></summary>
    /// <typeparam name="T">Expected exception class.<br/></typeparam><param name="action">Single bounded public API call.<br/></param>
    private static void Expect<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T) { checks++; return; }
        throw new InvalidOperationException("Expected " + typeof(T).Name);
    }

    /// <summary>Ordinary application-shaped custom value.<br/></summary>
    public sealed class Token { public string Text { get; set; } = ""; }
    /// <summary>Initialized property/field and ordinary byte controls.<br/></summary>
    public sealed class Record
    {
        public string Name { get; set; } = "";
        public Token? Value { get; set; } = new() { Text = "constructor" };
        public Token? Field = new() { Text = "field constructor" };
        public byte[] Ordinary { get; set; } = Array.Empty<byte>();
    }
    /// <summary>Compatible binary projection without a custom reader.<br/></summary>
    public sealed class BinaryRecord { public byte[]? Value { get; set; } public byte[]? Field; }
    /// <summary>Selected subtree fixture.<br/></summary>
    public sealed class Envelope { public Record Nested { get; set; } = new(); }
    /// <summary>Target-path alias fixture.<br/></summary>
    public sealed class RenamedRecord { public Token? Renamed { get; set; } = new() { Text = "renamed" }; }
    /// <summary>Collection whose configured type comparer differs from the source.<br/></summary>
    public sealed class SetRecord { public SortedSet<string>? Values { get; set; } }
    /// <summary>Identifiable application callback failure.<br/></summary>
    public sealed class ReaderFailure : Exception { }
}
