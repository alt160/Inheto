using Inheto;
using System.Numerics;
using System.Reflection;
using System.Runtime.Loader;

/// <summary>
/// Provides independent packed-consumer regression checks for public Stream helpers and narrowly scoped factory repairs.<br/>
/// Never enters the shipping runtime assembly or its dependency graph.<br/>
/// </summary>
public static class HelperRegression
{
    /// <summary>
    /// Exercises all six codecs at bit boundaries and deterministic random values against an independent BigInteger encoding model.<br/>
    /// Also checks every invalid terminal byte, every truncated maximum-width prefix, excessive continuation, compatible redundant groups and warmed managed allocations.<br/>
    /// </summary>
    public static void VerifyVarints()
    {
        VerifyCodec<int>(32, true, value => (int)value, UtilsAndExtensions.Write7BitEncodedInt, UtilsAndExtensions.Read7BitEncodedInt);
        VerifyCodec<uint>(32, false, value => (uint)value, UtilsAndExtensions.Write7BitEncodedUInt, UtilsAndExtensions.Read7BitEncodedUInt);
        VerifyCodec<long>(64, true, value => (long)value, UtilsAndExtensions.Write7BitEncodedInt64, UtilsAndExtensions.Read7BitEncodedInt64);
        VerifyCodec<ulong>(64, false, value => (ulong)value, UtilsAndExtensions.Write7BitEncodedUInt64, UtilsAndExtensions.Read7BitEncodedUInt64);
        VerifyCodec<Int128>(128, true, value => (Int128)value, UtilsAndExtensions.Write7BitEncodedInt128, UtilsAndExtensions.Read7BitEncodedInt128);
        VerifyCodec<UInt128>(128, false, value => (UInt128)value, UtilsAndExtensions.Write7BitEncodedUInt128, UtilsAndExtensions.Read7BitEncodedUInt128);
    }

    /// <summary>
    /// Verifies one typed codec without treating its own writer output as the sole source of truth.<br/>
    /// Keeps allocation-heavy test-data generation outside the warmed reader measurement.<br/>
    /// </summary>
    /// <typeparam name="T">Exact signed or unsigned integer type under test.<br/></typeparam>
    /// <param name="bits">Declared integer width: 32, 64 or 128.<br/></param>
    /// <param name="signed">Whether the wire representation applies the ZigZag mapping.<br/></param>
    /// <param name="convert">Checked test-data conversion from the independent integer model.<br/></param>
    /// <param name="write">Actual production Stream writer, not a BufferStream instance method.<br/></param>
    /// <param name="read">Actual production Stream reader, not a BufferStream instance method.<br/></param>
    private static void VerifyCodec<T>(int bits, bool signed, Func<BigInteger, T> convert, Action<Stream, T> write, Func<Stream, T> read)
    {
        int width = (bits + 6) / 7;
        int maximumFinal = (1 << (bits - 7 * (width - 1))) - 1;
        var minimum = signed ? -(BigInteger.One << (bits - 1)) : BigInteger.Zero;
        var maximum = (BigInteger.One << (signed ? bits - 1 : bits)) - 1;
        var values = new HashSet<BigInteger> { minimum, maximum, BigInteger.Zero, BigInteger.One };
        for (int bit = 0; bit < bits; bit++)
        {
            var power = BigInteger.One << bit;
            foreach (var nearby in new[] { power - 1, power, power + 1, -power - 1, -power, -power + 1 })
                if (nearby >= minimum && nearby <= maximum) values.Add(nearby);
        }
        ulong randomState = 0x71A43BB298D6205FUL;
        byte[] randomBytes = new byte[bits / 8];
        var mask = (BigInteger.One << bits) - 1;
        for (int sample = 0; sample < 4096; sample++)
        {
            for (int index = 0; index < randomBytes.Length; index++)
            {
                randomState ^= randomState << 13;
                randomState ^= randomState >> 7;
                randomState ^= randomState << 17;
                randomBytes[index] = (byte)randomState;
            }
            var raw = new BigInteger(randomBytes, isUnsigned: true) & mask;
            values.Add(signed ? (raw >> 1) ^ -(raw & 1) : raw);
        }
        using var stream = new MemoryStream(20);
        foreach (var modelValue in values)
        {
            T value = convert(modelValue);
            BigInteger raw = signed ? (modelValue << 1) ^ (modelValue >> (bits - 1)) : modelValue;
            var expected = new List<byte>(width);
            do
            {
                byte group = (byte)(raw & 0x7F);
                raw >>= 7;
                expected.Add((byte)(group | (raw.IsZero ? 0 : 0x80)));
            } while (!raw.IsZero);
            stream.SetLength(0);
            stream.Position = 0;
            write(stream, value);
            if (!stream.GetBuffer().AsSpan(0, (int)stream.Length).SequenceEqual(expected.ToArray()))
                throw new InvalidOperationException($"{typeof(T).Name} writer changed encoding for {modelValue}.");
            stream.Position = 0;
            if (!EqualityComparer<T>.Default.Equals(read(stream), value) || stream.Position != stream.Length || !stream.CanRead)
                throw new InvalidOperationException($"{typeof(T).Name} full-range round-trip failed for {modelValue}.");
        }
        for (int length = 0; length < width; length++)
            ExpectFailure<T, EndOfStreamException>(Enumerable.Repeat((byte)0x80, length).ToArray(), length, read);
        for (int terminal = maximumFinal + 1; terminal <= byte.MaxValue; terminal++)
        {
            var invalid = Enumerable.Repeat((byte)0x80, width + 1).ToArray();
            invalid[width - 1] = (byte)terminal;
            invalid[width] = 0x55; // Must remain unread: no consumption beyond the width limit.
            ExpectFailure<T, FormatException>(invalid, width, read);
        }
        for (int length = 1; length <= width; length++)
        {
            var redundantZero = Enumerable.Repeat((byte)0x80, length).ToArray();
            redundantZero[^1] = 0;
            using var redundantStream = new MemoryStream(redundantZero);
            if (!EqualityComparer<T>.Default.Equals(read(redundantStream), convert(BigInteger.Zero)) || redundantStream.Position != length)
                throw new InvalidOperationException($"{typeof(T).Name} compatible redundant zero encoding failed.");
        }
        byte[] widest = Enumerable.Repeat((byte)0xFF, width).ToArray();
        widest[^1] = (byte)maximumFinal;
        using var measured = new MemoryStream(widest);
        for (int repeat = 0; repeat < 512; repeat++) { measured.Position = 0; _ = read(measured); }
        long before = GC.GetAllocatedBytesForCurrentThread();
        for (int repeat = 0; repeat < 10_000; repeat++) { measured.Position = 0; _ = read(measured); }
        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        if (allocated != 0) throw new InvalidOperationException($"{typeof(T).Name} reader allocated {allocated} managed bytes in 10,000 warmed reads.");
        Console.WriteLine($"PASS {typeof(T).Name}: {values.Count} independent byte/round-trip cases; truncated/overflow/redundant guards; 10,000 warmed reads = 0 managed bytes.");
    }

    /// <summary>Checks the exact malformed-input exception and cursor boundary while ensuring the caller's stream remains open.<br/></summary>
    /// <typeparam name="T">Integer produced by a successful reader.<br/></typeparam>
    /// <typeparam name="TException">Expected format or EOF exception type.<br/></typeparam>
    /// <param name="bytes">Complete bounded malformed-input fixture, optionally followed by an unread sentinel.<br/></param>
    /// <param name="consumed">Expected byte count consumed before rejection.<br/></param>
    /// <param name="read">Actual public production helper to exercise.<br/></param>
    private static void ExpectFailure<T, TException>(byte[] bytes, int consumed, Func<Stream, T> read) where TException : Exception
    {
        using var stream = new MemoryStream(bytes);
        try { _ = read(stream); }
        catch (TException)
        {
            if (stream.Position != consumed || !stream.CanRead)
                throw new InvalidOperationException($"{typeof(T).Name} rejection consumed unexpected bytes or closed the stream.");
            return;
        }
        throw new InvalidOperationException($"{typeof(T).Name} accepted malformed input instead of throwing {typeof(TException).Name}.");
    }

    /// <summary>
    /// Checks that production changes are confined to exactly two factory-forwarding methods and six Stream readers.<br/>
    /// Normalizes metadata tokens and checks all other instructions, locals and exception regions against the pre-fix packed DLL.<br/>
    /// </summary>
    /// <param name="candidate">Actual production assembly loaded by the independent package consumer.<br/></param>
    /// <param name="baselinePath">Absolute path of the saved pre-fix production DLL.<br/></param>
    public static void CompareUnchangedProduction(Assembly candidate, string baselinePath)
    {
        var context = new AssemblyLoadContext("inheto-helper-before", isCollectible: true);
        context.Resolving += (_, name) => AssemblyLoadContext.Default.Assemblies.FirstOrDefault(item => item.GetName().Name == name.Name);
        try
        {
            var baseline = context.LoadFromAssemblyPath(Path.GetFullPath(baselinePath));
            ProductionIlAudit.ValidateControls();
            var before = ProductionIlAudit.Instructions(baseline);
            var after = ProductionIlAudit.Instructions(candidate);
            if (before.Count != after.Count || before.Keys.Any(key => !after.ContainsKey(key)))
                throw new InvalidOperationException("Production signatures changed during the helper correction.");
            string[] expected = {
                "Inheto.UtilsAndExtensions::Int32 Read7BitEncodedInt(System.IO.Stream)",
                "Inheto.UtilsAndExtensions::UInt32 Read7BitEncodedUInt(System.IO.Stream)",
                "Inheto.UtilsAndExtensions::Int64 Read7BitEncodedInt64(System.IO.Stream)",
                "Inheto.UtilsAndExtensions::UInt64 Read7BitEncodedUInt64(System.IO.Stream)",
                "Inheto.UtilsAndExtensions::System.Int128 Read7BitEncodedInt128(System.IO.Stream)",
                "Inheto.UtilsAndExtensions::System.UInt128 Read7BitEncodedUInt128(System.IO.Stream)",
                "Inheto.InhetoBinary::Inheto.InhetoBinary FromObject(System.Object, Int32, Inheto.SerializationOptions, Inheto.DeserializationOptions)",
                "Inheto.InhetoBinary::Inheto.InhetoBinary FromObject(System.Object, Byte[], Int64, Inheto.SerializationOptions, Inheto.DeserializationOptions)"
            };
            var changed = before.Where(pair => pair.Value != after[pair.Key]).Select(pair => pair.Key).ToHashSet(StringComparer.Ordinal);
            if (!changed.SetEquals(expected))
                throw new InvalidOperationException("Unexpected production change inventory: " + string.Join("; ", changed));
            Console.WriteLine($"PASS production IL scope: exactly 8 corrected methods; {before.Count - changed.Count} other methods/constructors unchanged, including serializer and BufferStream-based read paths.");
        }
        finally { context.Unload(); }
    }

    /// <summary>
    /// Verifies the serialization-only cleanup against the saved helper-fixed runtime: expected helper/factory removals only and unchanged shared production instructions.<br/>
    /// Also serializes the same caller objects through both assemblies to compare actual payload bytes, including collections and shared cyclic references.<br/>
    /// The legacy Brotli dependency resolves only in the isolated baseline context and never becomes a candidate runtime/package dependency.<br/>
    /// </summary>
    /// <param name="candidate">Actual serialization-only library loaded from the independent package consumer.<br/></param>
    /// <param name="baselinePath">Absolute path of the saved pre-boundary production DLL, with its legacy inspection dependency beside it.<br/></param>
    public static void CompareSerializationBoundary(Assembly candidate, string baselinePath)
    {
        string absoluteBaseline = Path.GetFullPath(baselinePath);
        var context = new AssemblyLoadContext("inheto-boundary-before", isCollectible: true);
        context.Resolving += (loader, name) =>
        {
            var shared = AssemblyLoadContext.Default.Assemblies.FirstOrDefault(item => item.GetName().Name == name.Name);
            if (shared is not null) return shared;
            string dependency = Path.Combine(Path.GetDirectoryName(absoluteBaseline)!, name.Name + ".dll");
            return File.Exists(dependency) ? loader.LoadFromAssemblyPath(dependency) : null;
        };
        try
        {
            var baseline = context.LoadFromAssemblyPath(absoluteBaseline);
            ProductionIlAudit.ValidateControls();
            var before = ProductionIlAudit.Instructions(baseline);
            var after = ProductionIlAudit.Instructions(candidate);
            string[] removedFactories = {
                "Inheto.InhetoBinary::Inheto.InhetoBinary FromObject(System.Object, Int32, Inheto.SerializationOptions, Inheto.DeserializationOptions)",
                "Inheto.InhetoBinary::Inheto.InhetoBinary FromObject(System.Object, Byte[], Int64, Inheto.SerializationOptions, Inheto.DeserializationOptions)",
                "Inheto.InhetoBinary::Inheto.InhetoBinary FromObject(System.Object, Byte[], Int64, Int32, Inheto.SerializationOptions, Inheto.DeserializationOptions)",
                "Inheto.InhetoBinary::Void .ctor(System.IO.BufferStream, Byte[], Int64, Inheto.SerializationOptions, Inheto.DeserializationOptions)"
            };
            before = RemapCompilerSymbols(baseline, candidate, before);
            var expectedRemoved = before.Keys.Where(key =>
                key.StartsWith("System.Security.Cryptography.AesEncryptionHelper::", StringComparison.Ordinal) ||
                key.StartsWith("Inheto.BrotliExtensions::", StringComparison.Ordinal))
                .Concat(removedFactories).ToHashSet(StringComparer.Ordinal);
            var actualRemoved = before.Keys.Where(key => !after.ContainsKey(key)).ToHashSet(StringComparer.Ordinal);
            if (!actualRemoved.SetEquals(expectedRemoved))
                throw new InvalidOperationException("Unexpected removed production methods: " + string.Join("; ", actualRemoved));
            string[] expectedAdded = {
                "Inheto.InhetoBinary::Void .ctor(System.IO.BufferStream, Inheto.SerializationOptions, Inheto.DeserializationOptions)"
            };
            if (!after.Keys.Where(key => !before.ContainsKey(key)).ToHashSet(StringComparer.Ordinal).SetEquals(expectedAdded))
                throw new InvalidOperationException("Unexpected added production methods during boundary cleanup.");
            string[] expectedChanged = {
                "Inheto.InhetoBinary::Inheto.InhetoBinary FromObject(System.Object, Inheto.SerializationOptions, Inheto.DeserializationOptions)",
                "Inheto.InhetoBinary::Inheto.InhetoBinary FromStream(System.IO.BufferStream, Inheto.DeserializationOptions)",
                "Inheto.InhetoBinary::Inheto.InhetoBinary ShallowClone()"
            };
            var changed = before.Keys.Where(key => after.TryGetValue(key, out string? body) && body != before[key]).ToHashSet(StringComparer.Ordinal);
            if (!changed.SetEquals(expectedChanged))
                throw new InvalidOperationException("Unexpected changed production bodies: " + string.Join("; ", changed));
            Console.WriteLine($"PASS boundary IL scope: {actualRemoved.Count} expected method removals, one simplified constructor, three expected call-site changes; {after.Count - 4} other methods/constructors unchanged.");

            var baselineType = baseline.GetType("Inheto.InhetoBinary", throwOnError: true)!;
            var baselineFactory = baselineType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(method => method.Name == "FromObject" && method.GetParameters().Length == 3);
            var sharedArray = new[] { 3, 5, 8 };
            var self = new Dictionary<string, object>();
            self["self"] = self;
            object[] fixtures = {
                new DocumentationOptionsFixture { Visible = 1, Hidden = 2 },
                new LinkedList<int>(new[] { 1, 2, 3 }),
                new List<string?> { "Unicode: \u03a9 \U0001f642", null, "" },
                new object[] { sharedArray, sharedArray, self },
                new Dictionary<string, object> {
                    ["date"] = new DateTime(638961234567890123, DateTimeKind.Utc),
                    ["maximum"] = UInt128.MaxValue,
                    ["minimum"] = Int128.MinValue,
                    ["matrix"] = new int[,] { { 1, 2 }, { 3, 4 } }
                }
            };
            foreach (object fixture in fixtures)
            {
                object earlier = baselineFactory.Invoke(null, new object?[] { fixture, null, null })!;
                var earlierBytes = (ReadOnlyMemory<byte>)baselineType.GetProperty("RawMemory")!.GetValue(earlier)!;
                var current = InhetoBinary.FromObject(fixture);
                if (!earlierBytes.Span.SequenceEqual(current.RawSpan))
                    throw new InvalidOperationException("Factory boundary cleanup changed payload bytes for " + fixture.GetType());
            }
            Console.WriteLine($"PASS byte identity against pre-boundary runtime: {fixtures.Length} object/collection/shared-cycle/128-bit/date/MD-array fixtures.");
        }
        finally { context.Unload(); }
    }

    /// <summary>
    /// Matches compiler-generated closure types and delegates by source-lambda names, signatures and captured field shapes rather than unstable declaration ordinals.<br/>
    /// Only renames symbols proven to have a unique counterpart; preserves lambda/local suffixes and rejects ambiguous, missing or non-bijective matches.<br/>
    /// Does not erase generated methods or their instructions from comparison; literal strings remain encoded by the existing IL normalizer.<br/>
    /// </summary>
    /// <param name="baseline">Saved pre-cleanup library containing the original generated declaration ordinals.<br/></param>
    /// <param name="candidate">Current library with three source overload declarations removed.<br/></param>
    /// <param name="instructions">Baseline instruction inventory with metadata tokens already resolved.<br/></param>
    /// <returns>Baseline inventory with uniquely matched compiler-symbol names rewritten to candidate names.<br/></returns>
    private static Dictionary<string, string> RemapCompilerSymbols(Assembly baseline, Assembly candidate, Dictionary<string, string> instructions)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        string EraseOrdinal(string text) => System.Text.RegularExpressions.Regex.Replace(text,
            @"(<>c__DisplayClass|<>9__|[bg]__|\|)(\d+)(?=_\d+)", "$1@",
            System.Text.RegularExpressions.RegexOptions.CultureInvariant);
        string Shape(Type type) => EraseOrdinal(type.FullName!) + "|" +
            string.Join(";", type.GetFields(flags).Select(field => EraseOrdinal(field.Name + ":" + field.FieldType)).Order(StringComparer.Ordinal)) + "|" +
            string.Join(";", type.GetMethods(flags).Select(method => EraseOrdinal(method.ToString()!)).Order(StringComparer.Ordinal));
        var replacements = new Dictionary<string, string>(StringComparer.Ordinal);
        var ordinals = new Dictionary<string, string>(StringComparer.Ordinal);
        var generatedAfter = candidate.GetTypes().Where(type => type.FullName!.StartsWith("Inheto.InhetoBinary+<>c", StringComparison.Ordinal))
            .GroupBy(Shape).ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.Ordinal);
        var usedTypes = new HashSet<Type>();
        foreach (var earlier in baseline.GetTypes().Where(type => type.FullName!.StartsWith("Inheto.InhetoBinary+<>c", StringComparison.Ordinal)))
        {
            if (!generatedAfter.TryGetValue(Shape(earlier), out var matches) || matches.Length != 1 || !usedTypes.Add(matches[0]))
                throw new InvalidOperationException("No unique generated-symbol counterpart for " + earlier.FullName);
            var current = matches[0];
            replacements.Add(earlier.Name, current.Name);
            var earlierMethods = earlier.GetMethods(flags);
            var currentMethods = current.GetMethods(flags);
            foreach (var method in earlierMethods)
            {
                var match = currentMethods.Single(other => EraseOrdinal(other.ToString()!) == EraseOrdinal(method.ToString()!));
                var oldOrdinal = System.Text.RegularExpressions.Regex.Match(method.Name, @"(?:[bg]__|\|)(\d+)(?=_\d+)");
                var newOrdinal = System.Text.RegularExpressions.Regex.Match(match.Name, @"(?:[bg]__|\|)(\d+)(?=_\d+)");
                if (oldOrdinal.Success != newOrdinal.Success)
                    throw new InvalidOperationException("Generated delegate ordinal shape changed.");
                if (oldOrdinal.Success)
                {
                    string oldNumber = oldOrdinal.Groups[1].Value, newNumber = newOrdinal.Groups[1].Value;
                    if (ordinals.TryGetValue(oldNumber, out var existing) && existing != newNumber)
                        throw new InvalidOperationException("Inconsistent generated delegate ordinal mapping.");
                    ordinals[oldNumber] = newNumber;
                }
            }
        }
        if (usedTypes.Count != generatedAfter.Values.Sum(group => group.Length))
            throw new InvalidOperationException("Unexpected candidate generated closure type.");
        var earlierOwner = baseline.GetType("Inheto.InhetoBinary", throwOnError: true)!;
        var currentOwner = candidate.GetType("Inheto.InhetoBinary", throwOnError: true)!;
        var localAfter = currentOwner.GetMethods(flags).Where(method => method.Name.StartsWith("<", StringComparison.Ordinal))
            .GroupBy(method => EraseOrdinal(method.ToString()!)).ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.Ordinal);
        int localFunctions = 0;
        foreach (var method in earlierOwner.GetMethods(flags).Where(method => method.Name.StartsWith("<", StringComparison.Ordinal)))
        {
            if (!localAfter.TryGetValue(EraseOrdinal(method.ToString()!), out var matches) || matches.Length != 1)
                throw new InvalidOperationException("No unique generated local-function counterpart for " + method);
            var oldOrdinal = System.Text.RegularExpressions.Regex.Match(method.Name, @"(?:[bg]__|\|)(\d+)(?=_\d+)");
            var newOrdinal = System.Text.RegularExpressions.Regex.Match(matches[0].Name, @"(?:[bg]__|\|)(\d+)(?=_\d+)");
            if (!oldOrdinal.Success || !newOrdinal.Success)
                throw new InvalidOperationException("Unrecognized generated local-function ordinal.");
            string oldNumber = oldOrdinal.Groups[1].Value, newNumber = newOrdinal.Groups[1].Value;
            if (ordinals.TryGetValue(oldNumber, out var existing) && existing != newNumber)
                throw new InvalidOperationException("Inconsistent generated local-function mapping.");
            ordinals[oldNumber] = newNumber;
            localFunctions++;
        }
        if (localFunctions != localAfter.Values.Sum(group => group.Length))
            throw new InvalidOperationException("Unexpected candidate generated local function.");
        string Rewrite(string value)
        {
            value = System.Text.RegularExpressions.Regex.Replace(value, @"(Inheto\.InhetoBinary\+)?(<>c__DisplayClass\d+_\d+(?:`\d+)?)",
                match => replacements.TryGetValue(match.Groups[2].Value, out var target) ? match.Groups[1].Value + target : throw new InvalidOperationException("Unknown generated closure identity: " + match.Value));
            return System.Text.RegularExpressions.Regex.Replace(value, @"(<>9__|[bg]__|\|)(\d+)(?=_\d+)", match =>
                ordinals.TryGetValue(match.Groups[2].Value, out var target) ? match.Groups[1].Value + target : match.Value);
        }
        var rewritten = instructions.ToDictionary(
            pair => pair.Key.StartsWith("Inheto.InhetoBinary", StringComparison.Ordinal) ? Rewrite(pair.Key) : pair.Key,
            pair => pair.Key.StartsWith("Inheto.InhetoBinary", StringComparison.Ordinal) ? Rewrite(pair.Value) : pair.Value, StringComparer.Ordinal);
        Console.WriteLine($"PASS bijective compiler-symbol correspondence: {usedTypes.Count} closure/delegate types and {localFunctions} local functions; instructions and source-lambda identities retained.");
        return rewritten;
    }
}
