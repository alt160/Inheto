using Inheto;
using System.Reflection;
using System.Runtime.Loader;
using System.IO.Compression;
using System.Security.Cryptography;

var assembly = typeof(InhetoSerializer).Assembly;
string[] forbiddenNames = { "Program", "ReviewCases", "EnumerableCases", "ConversionCases", "PerformanceHolder", "SmokeRecord", "ReleaseBoundaryThrowingGetter", "DocumentationOptionsFixture", "ProductionIlAudit", "HelperRegression", "UnsupportedPolicyRegression", "LiteralPathReview", "PathRecord", "PathNode", "CallbackRoot", "CallbackNode", "AesEncryptionHelper", "BrotliExtensions" };
if (assembly.GetTypes().Any(type => forbiddenNames.Contains(type.Name, StringComparer.Ordinal)))
    throw new InvalidOperationException("A validation-harness type leaked into the packaged runtime assembly.");
if (assembly.GetReferencedAssemblies().Any(name => name.Name?.Contains("TestHarness", StringComparison.OrdinalIgnoreCase) == true || name.Name?.Contains("Review", StringComparison.OrdinalIgnoreCase) == true))
    throw new InvalidOperationException("The packaged runtime assembly references a validation harness.");
if (assembly.GetReferencedAssemblies().Any(name => name.Name?.Contains("Brotli", StringComparison.OrdinalIgnoreCase) == true))
    throw new InvalidOperationException("The serialization-only runtime still references the external Brotli implementation.");
var factories = typeof(InhetoBinary).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(method => method.Name == "FromObject").ToArray();
Type[] expectedFactoryParameters = { typeof(object), typeof(SerializationOptions), typeof(DeserializationOptions) };
if (factories.Length != 1 || !factories[0].GetParameters().Select(parameter => parameter.ParameterType).SequenceEqual(expectedFactoryParameters))
    throw new InvalidOperationException("Inheto's public factory still exposes transformation arguments or lost its plain option contract.");

var source = new LinkedList<int>(new[] { 1, 2, 3 });
var binary = InhetoBinary.FromObject(source);
if (!binary.ToObject<LinkedList<int>>()!.SequenceEqual(source) ||
    !binary.ToObject<Stack<string>>()!.SequenceEqual(new[] { "1", "2", "3" }))
    throw new InvalidOperationException("The packed reader failed generic-enumerable reconstruction or conversion order.");

var serializer = new InhetoSerializer();
var clean = serializer.TestType(source);
var missing = serializer.TestType<object?>(null);
var throwing = serializer.TestType(new ReleaseBoundaryThrowingGetter());
if (clean.Issues.Count != 0 ||
    !missing.Issues.Any(issue => issue.Kind == InhetoSerializer.TestTypeIssueKind.NullRoot) ||
    !throwing.Issues.Any(issue => issue.Kind == InhetoSerializer.TestTypeIssueKind.GetterThrows))
    throw new InvalidOperationException("The retained developer compatibility diagnostic is unavailable or changed behavior.");
Console.WriteLine($"PASS packed consumer .NET {Environment.Version}: no harness types/references; Enumerable conversions; retained TestType diagnostics. Library: {assembly.Location}; BufferStream: {typeof(System.IO.BufferStream).Assembly.Location}");
if (args.Length == 2 && args[0] == "--compare-il") CompareMethodBodies(assembly, args[1]);
if (args.Length == 2 && args[0] == "--compare-helper-fixes") HelperRegression.CompareUnchangedProduction(assembly, args[1]);
if (args.Length == 2 && args[0] == "--compare-boundary") HelperRegression.CompareSerializationBoundary(assembly, args[1]);
if (args.Length == 1 && args[0] == "--verify-boundary") VerifySerializationBoundary();
if (args.Length == 1 && args[0] == "--characterize-unsupported") UnsupportedPolicyRegression.Characterize();
if (args.Length == 1 && args[0] == "--verify-unsupported") UnsupportedPolicyRegression.Verify();
if (args.Length == 2 && args[0] == "--compare-unsupported") UnsupportedPolicyRegression.CompareScope(assembly, args[1]);
if (args.Length == 3 && args[0] == "--verify-symbols") PackageMetadataRegression.VerifySymbols(assembly, args[1], args[2]);

/// <summary>
/// Validates the plain factory policy, public Stream varint helpers and caller-owned transformation boundary.<br/>
/// Uses bounded in-memory input only; no store files or external services are touched.<br/>
/// </summary>
void VerifySerializationBoundary()
{
    var policy = new SerializationOptions();
    policy.ExcludedMembers.Add(".Hidden");
    var fixture = new DocumentationOptionsFixture { Visible = 1, Hidden = 2 };
    var readerPolicy = new DeserializationOptions();
    readerPolicy.AddActivator<DocumentationOptionsFixture>(_ => null);
    var reader = InhetoBinary.FromObject(fixture, policy, readerPolicy);
    var clone = reader.ShallowClone();
    if (reader.PropExists(".Hidden") || reader.ReadInt32(".Visible") != 1 ||
        reader.ToObject<DocumentationOptionsFixture>() is not null ||
        clone.PropExists(".Hidden") || clone.ReadInt32(".Visible") != 1 ||
        clone.ToObject<DocumentationOptionsFixture>() is not null)
        throw new InvalidOperationException("The plain factory or shallow clone discarded serialization/deserialization policy.");
    var writer = new InhetoSerializer(policy);
    var payload = writer.Serialize(fixture);
    var borrowed = InhetoBinary.FromStream(payload, readerPolicy);
    if (!borrowed.RawSpan.SequenceEqual(reader.RawSpan) ||
        borrowed.ReadInt32(".Visible") != 1 || borrowed.PropExists(".Hidden") ||
        borrowed.ToObject<DocumentationOptionsFixture>() is not null)
        throw new InvalidOperationException("FromStream changed the plain payload or lost deserialization policy.");
    bool nullRejected = false;
    try { _ = InhetoBinary.FromObject(null); }
    catch (ArgumentNullException) { nullRejected = true; }
    if (!nullRejected) throw new InvalidOperationException("The plain factory accepted a null source.");
    Console.WriteLine("PASS sole plain factory, borrowed-stream reader and shallow clone preserve bytes/exclusions/null activators; null root rejected.");
    HelperRegression.VerifyVarints();
    VerifyCallerOwnedTransforms(reader);
    payload.Dispose(); // The writer is no longer reused and all borrowed views above have completed.
}

/// <summary>
/// Applies compression and authenticated encryption in consumer code, then recovers identical plain Inheto bytes and navigates a selected field.<br/>
/// Uses only the .NET runtime codecs, never the removed Inheto helpers; this is bounded integration proof, not a recommended persistent envelope format.<br/>
/// </summary>
/// <param name="binary">Plain payload whose byte identity, excluded member and selected value must survive the caller-owned transformations.<br/></param>
void VerifyCallerOwnedTransforms(InhetoBinary binary)
{
    using var compressed = new MemoryStream();
    using (var compressor = new BrotliStream(compressed, CompressionLevel.Fastest, leaveOpen: true))
        compressor.Write(binary.RawSpan);
    byte[] compressedBytes = compressed.ToArray();
    byte[] ciphertext = new byte[compressedBytes.Length];
    byte[] restoredCompressed = new byte[compressedBytes.Length];
    Span<byte> nonce = stackalloc byte[12];
    Span<byte> tag = stackalloc byte[16];
    RandomNumberGenerator.Fill(nonce);
    using var aes = new AesGcm(RandomNumberGenerator.GetBytes(32), tag.Length);
    aes.Encrypt(nonce, compressedBytes, ciphertext, tag);
    aes.Decrypt(nonce, ciphertext, tag, restoredCompressed);
    using var encryptedInput = new MemoryStream(restoredCompressed, writable: false);
    using var decompressor = new BrotliStream(encryptedInput, CompressionMode.Decompress, leaveOpen: true);
    using var restored = new MemoryStream();
    decompressor.CopyTo(restored);
    using var restoredPayload = new System.IO.BufferStream(restored.ToArray());
    var restoredReader = InhetoBinary.FromStream(restoredPayload);
    if (!restoredReader.RawSpan.SequenceEqual(binary.RawSpan) ||
        restoredReader.ReadInt32(".Visible") != 1 || restoredReader.PropExists(".Hidden"))
        throw new InvalidOperationException("Caller-owned compression/encryption changed bytes or selective member navigation.");
    ciphertext[0] ^= 1;
    bool tamperRejected = false;
    try { aes.Decrypt(nonce, ciphertext, tag, restoredCompressed); }
    catch (CryptographicException) { tamperRejected = true; }
    if (!tamperRejected) throw new InvalidOperationException("The consumer's authenticated encryption accepted tampered ciphertext.");
    Console.WriteLine("PASS caller-owned Brotli/AES-GCM round-trip, byte identity, selective field read and tamper rejection; no transformation implementation in Inheto.");
}

/// <summary>
/// Compares the packed production method bodies with a saved pre-documentation DLL without loading it into the default context.<br/>
/// Resolves unchanged shared dependencies from the current consumer and unloads the comparison context on every exit.<br/>
/// This bounded check demonstrates that documentation/build-boundary edits did not rewrite production IL; it is not a substitute for runtime assertions.<br/>
/// </summary>
/// <param name="candidate">Actual packaged library loaded by this independent consumer.<br/></param>
/// <param name="baselinePath">Absolute path to the saved production DLL from before this documentation/publication-boundary pass.<br/></param>
void CompareMethodBodies(Assembly candidate, string baselinePath)
{
    var context = new AssemblyLoadContext("inheto-publication-before", isCollectible: true);
    context.Resolving += (_, name) => AssemblyLoadContext.Default.Assemblies.FirstOrDefault(item => item.GetName().Name == name.Name);
    try
    {
        var baseline = context.LoadFromAssemblyPath(Path.GetFullPath(baselinePath));
        var before = MethodBodies(baseline);
        var after = MethodBodies(candidate);
        Console.WriteLine($"IL inventory: baseline={before.Count}; candidate={after.Count}.");
        if (before.Count != after.Count || before.Keys.Any(key => !after.ContainsKey(key)))
            throw new InvalidOperationException("Production method/constructor signatures changed during documentation work.");
        int rawDifferences = before.Count(pair => !pair.Value.AsSpan().SequenceEqual(after[pair.Key]));
        ProductionIlAudit.ValidateControls();
        var normalizedBefore = ProductionIlAudit.Instructions(baseline);
        var normalizedAfter = ProductionIlAudit.Instructions(candidate);
        var differences = normalizedBefore.Where(pair => !normalizedAfter.TryGetValue(pair.Key, out var body) || pair.Value != body).Select(pair => pair.Key).ToArray();
        if (differences.Length != 0)
            throw new InvalidOperationException("Production instructions changed: " + string.Join("; ", differences.Take(12)));
        Console.WriteLine($"PASS production IL comparison: {after.Count} method/constructor signatures, instructions, locals and exception regions unchanged; {rawDifferences} raw bodies differ only in resolved metadata token numbering.");
    }
    finally { context.Unload(); }
}

/// <summary>Indexes declared production method/constructor IL by type and signature, including private generated methods.<br/></summary>
/// <param name="library">Production assembly to inspect without invoking its methods.<br/></param>
/// <returns>The complete declared-method signature map with empty bodies for non-IL members.<br/></returns>
Dictionary<string, byte[]> MethodBodies(Assembly library)
{
    const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
    return library.GetTypes().SelectMany(type => type.GetMethods(flags).Cast<MethodBase>().Concat(type.GetConstructors(flags))
        .Select(method => new KeyValuePair<string, byte[]>(type.FullName + "::" + method, method.GetMethodBody()?.GetILAsByteArray() ?? Array.Empty<byte>())))
        .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
}

/// <summary>Provides a throwing getter solely in the independent packed-package consumer, never the runtime library.<br/></summary>
public sealed class ReleaseBoundaryThrowingGetter
{
    /// <summary>Throws an intentional exception so the retained compatibility diagnostic can report unsafe getter access.<br/></summary>
    public int Value => throw new InvalidOperationException("release-boundary getter sentinel");
}

/// <summary>Provides deterministic properties only in the packed-consumer characterization fixture.<br/></summary>
public sealed class DocumentationOptionsFixture
{
    /// <summary>Gets or sets the included scalar control value.<br/></summary>
    public int Visible { get; set; }
    /// <summary>Gets or sets the member excluded by the explicit policy.<br/></summary>
    public int Hidden { get; set; }
}

/// <summary>
/// Compares instruction semantics independently of module-local metadata token numbering.<br/>
/// Retains opcode order, raw non-token operands, resolved member/string operands, local types and exception-region boundaries.<br/>
/// Unknown opcodes or unsupported standalone-signature operands fail closed instead of masking possible changes.<br/>
/// </summary>
public static class ProductionIlAudit
{
    private static readonly Dictionary<ushort, System.Reflection.Emit.OpCode> Codes = typeof(System.Reflection.Emit.OpCodes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(field => field.FieldType == typeof(System.Reflection.Emit.OpCode))
        .Select(field => (System.Reflection.Emit.OpCode)field.GetValue(null)!)
        .ToDictionary(code => unchecked((ushort)code.Value));

    /// <summary>Indexes normalized IL for all declared methods and constructors without executing them.<br/></summary>
    /// <param name="library">Production assembly to inspect in its isolated or default load context.<br/></param>
    /// <returns>Method-signature map containing complete normalized instruction/body metadata.<br/></returns>
    public static Dictionary<string, string> Instructions(Assembly library)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        return library.GetTypes().SelectMany(type => type.GetMethods(flags).Cast<MethodBase>().Concat(type.GetConstructors(flags))
            .Select(method => new KeyValuePair<string, string>(type.FullName + "::" + method, Normalize(method))))
            .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
    }

    /// <summary>Resolves metadata operands while preserving all other IL and method-body structure.<br/></summary>
    /// <param name="method">Declared method or constructor to inspect; abstract/native bodies yield an empty representation.<br/></param>
    /// <returns>Canonical instructions, locals, flags and exception-region description.<br/></returns>
    private static string Normalize(MethodBase method)
    {
        var body = method.GetMethodBody();
        if (body is null) return "";
        var bytes = body.GetILAsByteArray()!;
        var text = new System.Text.StringBuilder();
        text.Append(method.Attributes).Append('|').Append(method.GetMethodImplementationFlags()).Append('|')
            .Append(body.InitLocals).Append('|').Append(body.MaxStackSize).Append('|');
        foreach (var local in body.LocalVariables)
            text.Append(local.LocalIndex).Append(':').Append(TypeIdentity(local.LocalType)).Append(':').Append(local.IsPinned).Append('|');
        foreach (var clause in body.ExceptionHandlingClauses)
        {
            text.Append(clause.Flags).Append(':').Append(clause.TryOffset).Append(':').Append(clause.TryLength).Append(':')
                .Append(clause.HandlerOffset).Append(':').Append(clause.HandlerLength).Append(':');
            if (clause.Flags == ExceptionHandlingClauseOptions.Clause) text.Append(TypeIdentity(clause.CatchType!));
            if (clause.Flags == ExceptionHandlingClauseOptions.Filter) text.Append(clause.FilterOffset);
            text.Append('|');
        }
        var typeArguments = method.DeclaringType!.GetGenericArguments();
        var methodArguments = method.IsGenericMethod ? method.GetGenericArguments() : Type.EmptyTypes;
        int position = 0;
        while (position < bytes.Length)
        {
            ushort value = bytes[position++];
            if (value == 0xfe) value = (ushort)(0xfe00 | bytes[position++]);
            if (!Codes.TryGetValue(value, out var code)) throw new InvalidOperationException("Unknown IL opcode.");
            text.Append(code.Name).Append(':');
            int size = code.OperandType switch
            {
                System.Reflection.Emit.OperandType.InlineNone => 0,
                System.Reflection.Emit.OperandType.ShortInlineBrTarget or System.Reflection.Emit.OperandType.ShortInlineI or System.Reflection.Emit.OperandType.ShortInlineVar => 1,
                System.Reflection.Emit.OperandType.InlineVar => 2,
                System.Reflection.Emit.OperandType.InlineI8 or System.Reflection.Emit.OperandType.InlineR => 8,
                System.Reflection.Emit.OperandType.InlineSwitch => checked(4 + 4 * BitConverter.ToInt32(bytes, position)),
                _ => 4
            };
            switch (code.OperandType)
            {
                case System.Reflection.Emit.OperandType.InlineField:
                case System.Reflection.Emit.OperandType.InlineMethod:
                case System.Reflection.Emit.OperandType.InlineType:
                case System.Reflection.Emit.OperandType.InlineTok:
                    var member = method.Module.ResolveMember(BitConverter.ToInt32(bytes, position), typeArguments, methodArguments)!;
                    text.Append(member.MemberType).Append(':')
                        .Append(member is Type resolvedType ? TypeIdentity(resolvedType) : TypeIdentity(member.DeclaringType!) + "::" + member);
                    break;
                case System.Reflection.Emit.OperandType.InlineString:
                    text.Append(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(method.Module.ResolveString(BitConverter.ToInt32(bytes, position)))));
                    break;
                case System.Reflection.Emit.OperandType.InlineSig:
                    throw new InvalidOperationException("Standalone IL signature requires explicit normalization.");
                default:
                    text.Append(Convert.ToHexString(bytes.AsSpan(position, size)));
                    break;
            }
            position += size;
            text.Append('|');
        }
        return text.ToString();
    }

    /// <summary>Uses assembly identity and full CLR type syntax instead of a module-local TypeRef token.<br/></summary>
    /// <param name="type">Resolved operand, local or exception type to identify.<br/></param>
    /// <returns>Stable identity including generic/array type syntax and owning assembly name.<br/></returns>
    private static string TypeIdentity(Type type) => type.Assembly.GetName().Name + ":" + type;

    /// <summary>
    /// Checks that normalization rejects changed constants and changed call targets while accepting a method compared with itself.<br/>
    /// Runs only against local consumer fixtures; no production body is rewritten or invoked.<br/>
    /// </summary>
    public static void ValidateControls()
    {
        const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;
        var owner = typeof(ProductionIlAudit);
        string first = Normalize(owner.GetMethod(nameof(AddOne), flags)!);
        if (first != Normalize(owner.GetMethod(nameof(AddOne), flags)!) ||
            first == Normalize(owner.GetMethod(nameof(AddTwo), flags)!) ||
            Normalize(owner.GetMethod(nameof(Absolute), flags)!) == Normalize(owner.GetMethod(nameof(Sign), flags)!))
            throw new InvalidOperationException("IL audit positive/negative controls failed.");
        Console.WriteLine("PASS IL audit controls: identical code accepted; changed constant and changed call target rejected.");
    }

    /// <summary>Supplies the constant-one negative-control instruction stream.<br/></summary>
    /// <param name="value">Control integer input; the method is inspected, not executed.<br/></param>
    /// <returns>Input plus one.<br/></returns>
    private static int AddOne(int value) => value + 1;

    /// <summary>Supplies the different constant-two instruction stream.<br/></summary>
    /// <param name="value">Control integer input; the method is inspected, not executed.<br/></param>
    /// <returns>Input plus two.<br/></returns>
    private static int AddTwo(int value) => value + 2;

    /// <summary>Supplies one resolved method-call target for negative-control comparison.<br/></summary>
    /// <param name="value">Control integer input; the method is inspected, not executed.<br/></param>
    /// <returns>The absolute-value call result if executed.<br/></returns>
    private static int Absolute(int value) => Math.Abs(value);

    /// <summary>Supplies a different resolved method-call target with the same argument/result types.<br/></summary>
    /// <param name="value">Control integer input; the method is inspected, not executed.<br/></param>
    /// <returns>The sign call result if executed.<br/></returns>
    private static int Sign(int value) => Math.Sign(value);
}
