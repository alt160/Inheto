using Inheto;
using System.IO;
using System.Text.RegularExpressions;

string readmePath = args[0];
string programPath = args[1];
string readmeText = File.ReadAllText(readmePath);
var readmeBlocks = Regex.Matches(readmeText, @"```csharp\r?\n(.*?)```", RegexOptions.Singleline);
var compiledBlocks = Regex.Matches(File.ReadAllText(programPath), @"// README-SAMPLE-(\d+)\r?\n(.*?)// END-README-SAMPLE", RegexOptions.Singleline);
Require(readmeBlocks.Count == 3 && compiledBlocks.Count == 8, "Expected three README examples and eight compiled README/guide examples.");
string guideDirectory = Path.Combine(Path.GetDirectoryName(readmePath)!, "docs");
var performanceBlocks = Regex.Matches(File.ReadAllText(Path.Combine(guideDirectory, "performance-and-memory.md")), @"```csharp\r?\n(.*?)```", RegexOptions.Singleline);
var configurationBlocks = Regex.Matches(File.ReadAllText(Path.Combine(guideDirectory, "configuration-and-reconstruction.md")), @"```csharp\r?\n(.*?)```", RegexOptions.Singleline);
var pathBlocks = Regex.Matches(File.ReadAllText(Path.Combine(guideDirectory, "property-paths.md")), @"```csharp\r?\n(.*?)```", RegexOptions.Singleline);
Require(performanceBlocks.Count == 2 && configurationBlocks.Count == 3 && pathBlocks.Count == 1, "Guide sample counts changed; update explicit compilation coverage.");
string[] publishedSamples = [readmeBlocks[1].Groups[1].Value, readmeBlocks[0].Groups[1].Value,
    performanceBlocks[0].Groups[1].Value, performanceBlocks[1].Groups[1].Value,
    configurationBlocks[0].Groups[1].Value, configurationBlocks[1].Groups[1].Value, pathBlocks[0].Groups[1].Value,
    readmeBlocks[2].Groups[1].Value];
foreach (Match sample in compiledBlocks)
{
    int index = int.Parse(sample.Groups[1].Value) - 1;
    Require(Normalize(sample.Groups[2].Value) == Normalize(publishedSamples[index]),
        $"Compiled sample {index + 1} differs from the published README/guide.");
}

// README-SAMPLE-1
var order = new Order
{
    Id = 501,
    Status = "ready",
    Lines =
    [
        new OrderLine { Sku = "A-100", Quantity = 2 },
        new OrderLine { Sku = "B-200", Quantity = 5 }
    ],
    Tags = new Dictionary<string, string> { ["region"] = "west" }
};

var binary = InhetoBinary.FromObject(order);

// Hydrate the stored data back into an Order.
Order? restored = binary.ToObject<Order>();

// Or read selected values without creating the Order or its entire Lines list.
long? id = binary.ReadInt64(".Id");
string? status = binary.ReadString(".Status");
int? quantity = binary.ReadInt32(".Lines#1.Quantity"); // Second line: 5.
string? region = binary.ReadString(".Tags!region");

// Hydrate just one element or just the collection.
OrderLine? line = binary.ReadPropExact<OrderLine>(".Lines#1");
List<OrderLine>? lines = binary.ReadPropExact<List<OrderLine>>(".Lines");
// END-README-SAMPLE
Require(id == 501 && status == "ready" && quantity == 5 && region == "west", "Direct selected reads.");
Require(line is { Sku: "B-200", Quantity: 5 }, "Selected collection element.");
Require(lines is { Count: 2 } && lines[0].Sku == "A-100" && lines[1].Quantity == 5, "Selected complete collection.");
Require(restored is { Id: 501, Status: "ready", Lines.Count: 2 } && restored.Tags["region"] == "west", "Whole-object reconstruction.");
Require(binary.ReadInt32(".Absent") is null && binary.ReadPropAs<long>(".Id") == 501, "Missing native and typed general reads.");

// README-SAMPLE-8
Order? copy = InhetoBinary.FromObject(order).ToObject<Order>();

// Changing the copy's nested data does not change the original order.
if (copy is not null)
    copy.Lines[0].Quantity = 99;
// END-README-SAMPLE
Require(copy is { Lines.Count: 2 } && copy.Lines[0].Quantity == 99 && order.Lines[0].Quantity == 2 &&
    !ReferenceEquals(copy, order) && !ReferenceEquals(copy.Lines, order.Lines) &&
    !ReferenceEquals(copy.Lines[0], order.Lines[0]) && !ReferenceEquals(copy.Tags, order.Tags),
    "README cloning creates independent ordinary mutable objects and collections.");
OrderSummary? summary = binary.ToObject<OrderSummary>();
Require(summary is { Id: 501, Status: "ready" }, "README inline example hydrates an unrelated smaller destination type.");
var graphRoot = new CloneNode { Label = "root" };
var graphChild = new CloneNode { Label = "child", Next = graphRoot };
graphRoot.Next = graphChild;
graphRoot.Other = graphChild;
CloneNode? graphCopy = InhetoBinary.FromObject(graphRoot).ToObject<CloneNode>();
Require(graphCopy is { Next: not null } && !ReferenceEquals(graphCopy, graphRoot) &&
    !ReferenceEquals(graphCopy.Next, graphChild) && ReferenceEquals(graphCopy.Next, graphCopy.Other) &&
    ReferenceEquals(graphCopy.Next.Next, graphCopy), "Cloning preserves aliasing and cycles inside an independent graph.");
graphCopy!.Next!.Label = "changed copy";
Require(graphChild.Label == "child", "Cloned shared child mutations do not reach the original graph.");

// These exact README paths require the documented reference topology, not just matching names.
string[] documentedPaths = [".Addresses#0.Street.Number", ".Addresses.Home.Street.Number",
    ".Addresses.Home.Notes!Important!.Value", ".Customer.Company.Contact.Company.Contact.Company.Name",
    ".Customer.Company.Name", ".Customer.Company.Contact.Company.Contact.Name", ".Customer.Contact.Name"];
foreach (string documentedPath in documentedPaths)
    Require(readmeText.Contains($"`{documentedPath}`", StringComparison.Ordinal), "Update the complex-path fixture when its documented path changes.");
Require(readmeText.IndexOf("| Syntax | Meaning |", StringComparison.Ordinal) <
    readmeText.IndexOf("| Path | Meaning |", StringComparison.Ordinal), "Path notation precedes path examples.");
foreach (bool literalFirst in new[] { false, true })
{
    var pathHome = new PathAddress { Street = new PathStreet { Number = 17 } };
    if (literalFirst) pathHome.Notes.Add("Important!", new PathNote { Value = "literal key" });
    pathHome.Notes.Add("Important", new PathNote { Value = "shorter key" });
    if (!literalFirst) pathHome.Notes.Add("Important!", new PathNote { Value = "literal key" });
    var pathAddresses = new PathAddressBook { Home = pathHome };
    pathAddresses.Add(pathHome);
    var pathCompany = new PathCompany { Name = "Company name" };
    var pathContact = new PathContact { Name = "Contact name", Company = pathCompany };
    pathCompany.Contact = pathContact;
    var pathSource = new PathDemo
    {
        Addresses = pathAddresses,
        Customer = new PathCustomer { Company = pathCompany, Contact = pathContact },
        State = SourceStatus.StoredReadyName
    };
    using BufferStream pathPayload = new InhetoSerializer().Serialize(pathSource);
    var pathBinary = InhetoBinary.FromStream(pathPayload);
    Require(pathBinary.ReadInt32(documentedPaths[0]) == 17 && pathBinary.ReadInt32(documentedPaths[1]) == 17,
        "Collection element and named Home alias reach the same street number.");
    Require(pathBinary.ReadString(documentedPaths[2]) == "literal key" &&
        pathBinary.ReadString(".Addresses.Home.Notes!Important.Value") == "shorter key",
        "Trailing exclamation mark belongs to the literal key regardless of dictionary insertion order.");
    Require(pathBinary.ReadString(documentedPaths[3]) == "Company name" && pathBinary.ReadString(documentedPaths[4]) == "Company name" &&
        pathBinary.ReadString(documentedPaths[5]) == "Contact name" && pathBinary.ReadString(documentedPaths[6]) == "Contact name",
        "Circular routes distinguish Company.Name from Contact.Name and reach the documented equivalents.");
    var pathRestored = pathBinary.ToObject<PathDemo>();
    Require(pathRestored is { Addresses.Count: 1 } && ReferenceEquals(pathRestored.Addresses.Home, pathRestored.Addresses[0]) &&
        ReferenceEquals(pathRestored.Customer.Contact, pathRestored.Customer.Company.Contact) &&
        ReferenceEquals(pathRestored.Customer.Company, pathRestored.Customer.Contact.Company),
        "Whole-object hydration independently retains the inherited-list alias and circular/shared topology.");
    Require(pathBinary.ReadPropAs<TargetStatus>(".State") == TargetStatus.Approved &&
        pathBinary.ToObject<PathStatusView>() is { State: TargetStatus.Approved },
        "Stored enum numeric value hydrates into a differently named target member.");
    Require(pathBinary.RawSpan.IndexOf(System.Text.Encoding.UTF8.GetBytes(nameof(SourceStatus.StoredReadyName))) < 0,
        "Source enum textual member name is absent from the payload.");
}
foreach (SourceStatus sourceState in new[] { SourceStatus.StoredNegativeName, (SourceStatus)99 })
{
    using BufferStream enumPayload = new InhetoSerializer().Serialize(new PathDemo { State = sourceState });
    var enumBinary = InhetoBinary.FromStream(enumPayload);
    Require((int)enumBinary.ReadPropAs<TargetStatus>(".State") == (int)sourceState &&
        enumBinary.ToObject<PathStatusView>() is { } enumView && (int)enumView.State == (int)sourceState,
        "Enum reprojection preserves negative and undefined numeric values without member-name matching.");
}

// README-SAMPLE-3
var serializer = new InhetoSerializer();
BufferStream payload = serializer.Serialize(order);
try
{
    using var readBuffer = new BufferStream(payload.AsReadOnlyMemory);
    var reader = InhetoBinary.FromStream(readBuffer);
    Console.WriteLine(reader.ReadInt64(".Id")); // 501.

    var nextOrder = new Order { Id = 502, Status = "queued" };
    serializer.Serialize(nextOrder); // Resets the SAME writer-owned stream.
    reader.RebindBorrowedPayload(payload.AsReadOnlyMemory);
    Console.WriteLine(reader.ReadInt64(".Id")); // 502, not a cached value from 501.
}
finally
{
    payload.Dispose(); // Only after the writer and all readers/views are finished.
}
// END-README-SAMPLE
var repeatWriter = new InhetoSerializer();
BufferStream repeatPayload = repeatWriter.Serialize(order);
try
{
    using var repeatReadBuffer = new BufferStream(repeatPayload.AsReadOnlyMemory);
    var repeatReader = InhetoBinary.FromStream(repeatReadBuffer);
    Require(repeatReader.ReadInt64(".Id") == 501, "First reused payload.");
    for (int iteration = 0; iteration < 100; iteration++)
    {
        Require(ReferenceEquals(repeatWriter.Serialize(new Order { Id = 502 + iteration }), repeatPayload), "Writer-owned stream identity.");
        repeatReader.RebindBorrowedPayload(repeatPayload.AsReadOnlyMemory);
        Require(repeatReader.ReadInt64(".Id") == 502 + iteration && repeatReader.ReadInt32(".Lines#0.Quantity") is null,
            "Rebinding clears prior payload values and missing collection entries.");
    }
}
finally { repeatPayload.Dispose(); }

// README-SAMPLE-4
// This copy is intentional: savedBytes will outlive the original payload.
// Skip the copy when your application's existing memory-owner contract is sufficient.
byte[] savedBytes = binary.RawMemory.ToArray();
using var input = new BufferStream((ReadOnlyMemory<byte>)savedBytes);
var reopened = InhetoBinary.FromStream(input);
string? savedStatus = reopened.ReadString(".Status");
// END-README-SAMPLE
Require(savedStatus == "ready" && reopened.RawSpan.SequenceEqual(savedBytes), "Detached saved bytes reopened without another payload copy.");
using (BufferStream segment = binary.RawBytes)
    Require(segment.AsReadOnlySpan.SequenceEqual(binary.RawSpan), "Raw segment bytes.");
Require(binary.ReadString(".Status") == "ready" && binary.ShallowClone().ReadString(".Status") == "ready", "Segment disposal and shallow-reader navigation.");

// README-SAMPLE-5
var writeOptions = new SerializationOptions
{
    MemberTypes = MemberTypesEnum.PublicPropertiesAndFields,
    FailOnUnsupportedType = true
};
writeOptions.ExcludedMembers.Add(".InternalNote");

var readOptions = new DeserializationOptions();
readOptions.Aliases.Add(".Status", ".State"); // Read-time equivalent paths; no rewrite.

var configured = InhetoBinary.FromObject(order, writeOptions, readOptions);
// END-README-SAMPLE
Require(configured.ReadPropAs<string>(".State") == "ready" && configured.ToObject<Order>()?.Id == 501, "Strict policy and aliases.");

// README-SAMPLE-6
var diagnosticWriter = new InhetoSerializer(writeOptions);
var report = diagnosticWriter.TestType(order);
foreach (var issue in report.Issues)
    Console.WriteLine($"{issue.MemberPath}: {issue.Message}");
// END-README-SAMPLE
Require(report.Issues.Count == 0 && report.NodesVisited > 0, "Explicit compatibility probe.");

// README-SAMPLE-7
var lengthPath = InhetoBinary.CheckPropPath<Order, int>(".Status.Length");
var upperPath = InhetoBinary.CheckPropPath<Order, string>(".Status.ToUpperInvariant()");

if (binary.TryReadPropPath<int>(lengthPath, out var length) == PropPathReadState.Value)
    Console.WriteLine(length); // 5: the stored "ready" string's Length.

if (binary.TryReadPropPath<string>(upperPath, out var upper) == PropPathReadState.Value)
    Console.WriteLine(upper); // READY: the .NET method's result, not a stored member.
// END-README-SAMPLE
Require(lengthPath.IsValid && lengthPath.RequiresClrContinuation && lengthPath.SerializedPrefixCandidate == ".Status" &&
    lengthPath.ClrRemainderCandidate == ".Length", "Property continuation plan identifies stored prefix.");
Require(upperPath.IsValid && upperPath.ContainsMethodInvocation && upperPath.RequiresClrContinuation,
    "Parameterless method continuation plan.");
Require(binary.TryReadPropPath<int>(lengthPath, out var verifiedLength) == PropPathReadState.Value && verifiedLength == 5,
    "Property continuation result.");
Require(binary.TryReadPropPath<string>(upperPath, out var verifiedUpper) == PropPathReadState.Value && verifiedUpper == "READY" &&
    binary.ReadString(".Status") == "ready", "Method continuation result does not rewrite stored text.");
Require(!InhetoBinary.CheckPropPath<Order, string>(".Status.Substring()").IsValid,
    "Method requiring arguments is not admitted as a parameterless continuation.");

var statusPlan = InhetoBinary.CheckPropPath<Order, string>(".Status");
Require(binary.TryReadPropPath<string>(statusPlan, out var plannedStatus) == PropPathReadState.Value && plannedStatus == "ready", "Prepared logical path.");

// Focused verification of the README's representation notes, not an exhaustive type matrix.
using var secureSource = new System.Security.SecureString();
foreach (char character in "demo-not-a-secret") secureSource.AppendChar(character);
secureSource.MakeReadOnly();
using var memorySource = new MemoryStream(new byte[] { 10, 20, 30, 40 }, writable: false);
memorySource.Position = 2;
var gridSource = new int[2, 3];
gridSource[1, 2] = 42;
var cubeSource = new Guid[1, 2, 1];
cubeSource[0, 1, 0] = Guid.Parse("c69c3530-cf00-4c9b-a310-22142dfed168");
var multiSource = new System.Collections.Specialized.NameValueCollection();
multiSource.Add("tags", "one");
multiSource.Add("tags", "two");
var stringsSource = new System.Collections.Specialized.StringCollection();
stringsSource.Add("first");
stringsSource.Add(null!);
stringsSource.Add("last");
var notesSource = new
{
    Utc = new DateTime(2026, 10, 6, 12, 34, 56, DateTimeKind.Utc).AddTicks(7),
    Local = new DateTime(2026, 10, 6, 12, 34, 56, DateTimeKind.Local).AddTicks(7),
    Unspecified = new DateTime(2026, 10, 6, 12, 34, 56, DateTimeKind.Unspecified).AddTicks(7),
    Offset = new DateTimeOffset(2026, 10, 6, 12, 34, 56, TimeSpan.FromMinutes(345)).AddTicks(7),
    Day = new DateOnly(2026, 10, 6),
    Clock = new TimeOnly(12, 34, 56).Add(TimeSpan.FromTicks(7)),
    Duration = TimeSpan.FromTicks(-123456789),
    Amount = 1.2300m,
    Huge = -(System.Numerics.BigInteger.One << 140) + 19,
    VersionTwo = new Version(1, 2),
    VersionThree = new Version(1, 2, 3),
    VersionFour = new Version(1, 2, 3, 4),
    Relative = new Uri("folder/item?x=1", UriKind.Relative),
    Absolute = new Uri("https://example.invalid/path?x=1"),
    Pattern = new Regex("a+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(250)),
    Zone = TimeZoneInfo.CreateCustomTimeZone("Readme/+0545", TimeSpan.FromMinutes(345), "Readme zone", "Readme standard"),
    Culture = System.Globalization.CultureInfo.GetCultureInfo("fr-FR"),
    Address = System.Net.IPAddress.Parse("fe80::1234%42"),
    Endpoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("fe80::1234%42"), 4567),
    Mail = new System.Net.Mail.MailAddress("sample@example.invalid", "Sample User"),
    Builder = new System.Text.StringBuilder("sample", capacity: 64),
    Stream = memorySource,
    Secure = secureSource,
    Grid = gridSource,
    Cube = cubeSource,
    NullableGrid = new int?[,] { { null, 7 } },
    Many = multiSource,
    Strings = stringsSource
};
using var notesPayload = new InhetoSerializer().Serialize(notesSource);
var notesReader = InhetoBinary.FromStream(notesPayload);
Require(notesReader.ReadDateTime(".Utc") is { } restoredUtc && restoredUtc.Ticks == notesSource.Utc.Ticks && restoredUtc.Kind == DateTimeKind.Utc,
    "DateTime UTC ticks and kind.");
Require(notesReader.ReadDateTime(".Local") is { } restoredLocal && restoredLocal.Ticks == notesSource.Local.Ticks && restoredLocal.Kind == DateTimeKind.Local,
    "DateTime local ticks and kind under the current timezone.");
Require(notesReader.ReadDateTime(".Unspecified") is { } restoredUnspecified && restoredUnspecified.Ticks == notesSource.Unspecified.Ticks && restoredUnspecified.Kind == DateTimeKind.Unspecified,
    "DateTime unspecified ticks and kind.");
Require(notesReader.ReadDateTimeOffset(".Offset") is { } restoredOffset && restoredOffset.Ticks == notesSource.Offset.Ticks && restoredOffset.Offset == notesSource.Offset.Offset,
    "DateTimeOffset clock ticks and original non-hour offset.");
Require(notesReader.ReadDateOnly(".Day") == notesSource.Day && notesReader.ReadTimeOnly(".Clock") == notesSource.Clock &&
    notesReader.ReadTimeSpan(".Duration") == notesSource.Duration, "DateOnly, TimeOnly and signed TimeSpan precision.");
Require(notesReader.ReadDecimal(".Amount") is { } restoredAmount && decimal.GetBits(restoredAmount).SequenceEqual(decimal.GetBits(notesSource.Amount)),
    "Decimal sign, precision and scale bits.");
Require(notesReader.ReadBigInteger(".Huge") == notesSource.Huge, "BigInteger sign and arbitrary precision.");
Require(notesReader.ReadVersion(".VersionTwo") is { Build: -1, Revision: -1 } &&
    notesReader.ReadVersion(".VersionThree") is { Build: 3, Revision: -1 } &&
    notesReader.ReadVersion(".VersionFour") is { Build: 3, Revision: 4 }, "Omitted Version components.");
Require(notesReader.ReadUri(".Relative") is { IsAbsoluteUri: false } restoredRelative && restoredRelative.OriginalString == notesSource.Relative.OriginalString &&
    notesReader.ReadUri(".Absolute") == notesSource.Absolute, "Relative and absolute Uri values.");
Require(notesReader.ReadRegex(".Pattern") is { } restoredPattern && restoredPattern.ToString() == notesSource.Pattern.ToString() &&
    restoredPattern.Options == notesSource.Pattern.Options && restoredPattern.MatchTimeout == notesSource.Pattern.MatchTimeout,
    "Regex pattern, options and timeout.");
Require(notesReader.ReadTimeZoneInfo(".Zone") is { } restoredZone && restoredZone.ToSerializedString() == notesSource.Zone.ToSerializedString() &&
    notesReader.ReadCultureInfo(".Culture")?.Name == notesSource.Culture.Name, "Timezone definition and culture name.");
Require(notesReader.ReadIPAddress(".Address") is { } restoredAddress && restoredAddress.GetAddressBytes().SequenceEqual(notesSource.Address.GetAddressBytes()) &&
    restoredAddress.ScopeId == 42 && notesReader.ReadIPEndPoint(".Endpoint") is { Port: 4567, Address.ScopeId: 42 }, "IPv6 scopes and endpoint port.");
Require(notesReader.ReadMailAddress(".Mail") is { } restoredMail && restoredMail.Address == notesSource.Mail.Address && restoredMail.DisplayName == notesSource.Mail.DisplayName,
    "MailAddress address and display name.");
Require(notesReader.ReadStringBuilder(".Builder")?.ToString() == "sample", "StringBuilder contents.");
using var restoredStream = notesReader.ReadMemoryStream(".Stream");
Require(restoredStream is { Position: 0 } && restoredStream.ToArray().SequenceEqual(new byte[] { 10, 20, 30, 40 }), "MemoryStream contents and reset cursor.");
using var restoredSecure = notesReader.ReadSecureString(".Secure");
Require(restoredSecure is not null && new System.Net.NetworkCredential("", restoredSecure).Password == "demo-not-a-secret",
    "SecureString character contents.");
Require(notesReader.RawSpan.IndexOf(System.Text.Encoding.UTF8.GetBytes("demo-not-a-secret")) >= 0,
    "SecureString characters are unencrypted in the serialized payload.");
Require(notesReader.ReadInt32(".Grid@1,2") == 42 && notesReader.ReadPropExact<int[,]>(".Grid") is { } restoredGrid &&
    restoredGrid.Rank == 2 && restoredGrid.GetLength(0) == 2 && restoredGrid.GetLength(1) == 3 && restoredGrid[1, 2] == 42,
    "Core multidimensional array dimensions, coordinates and reconstruction.");
Require(notesReader.ReadPropExact<Guid[,,]>(".Cube") is { } restoredCube && restoredCube.Rank == 3 && restoredCube[0, 1, 0] == cubeSource[0, 1, 0] &&
    notesReader.ReadPropExact<int?[,]>(".NullableGrid") is { } restoredNullableGrid && restoredNullableGrid[0, 0] is null && restoredNullableGrid[0, 1] == 7,
    "Core Guid and nullable multidimensional array shapes.");
Require(notesReader.ReadNameValueCollection(".Many")?.GetValues("tags") is { Length: 2 } restoredMany && restoredMany.SequenceEqual(new[] { "one", "two" }),
    "NameValueCollection multi-value entry.");
Require(notesReader.ReadStringCollection(".Strings") is { Count: 3 } restoredStrings && restoredStrings[0] == "first" && restoredStrings[1] is null && restoredStrings[2] == "last",
    "StringCollection ordering and explicit null slot.");
Console.WriteLine($"PASS three README blocks and five guide blocks match compiled source; complex literal/alias/circular paths with whole-graph topology, numeric enum reprojection, direct/collection/whole reads, independent cloning, unrelated-type hydration, 100 writer/reader reuse cycles, ownership, aliases, strict policy, diagnostic, CLR continuation and focused round-trip notes. Runtime: {Environment.Version}");

/// <summary>Normalizes sample formatting and removes namespace imports relocated to the compilation unit's start.<br/></summary>
/// <param name="text">One README or compiled fixture sample, never executed as script.<br/></param>
/// <returns>Normalized source used only for exact sample drift detection.<br/></returns>
static string Normalize(string text) => string.Join('\n', text.Replace("\r", "").Split('\n')
    .Where(line => !line.TrimStart().StartsWith("using ", StringComparison.Ordinal) || line.TrimStart().StartsWith("using var ", StringComparison.Ordinal))
    .Select(line => line.TrimEnd())).Trim();

/// <summary>Fails the independent consumer immediately when one bounded documentation contract is not satisfied.<br/></summary>
/// <param name="condition">Verified condition from the packaged public API or sample source comparison.<br/></param>
/// <param name="message">Failure description identifying the contract, without secret or payload dumps.<br/></param>
static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

// README-SAMPLE-2
public sealed class Order
{
    public long Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<OrderLine> Lines { get; set; } = [];
    public Dictionary<string, string> Tags { get; set; } = new();
}

public sealed class OrderLine
{
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
// END-README-SAMPLE

/// <summary>Unrelated smaller destination used to verify the README's inline hydration example.<br/></summary>
public sealed class OrderSummary
{
    public long Id { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>Mutable ordinary data node proving that a clone has its own graph while retaining aliases and cycles within that graph.<br/></summary>
public sealed class CloneNode
{
    public string Label { get; set; } = string.Empty;
    public CloneNode? Next { get; set; }
    public CloneNode? Other { get; set; }
}

/// <summary>Ordinary root shape for the README's inherited-collection and circular-reference path examples.<br/></summary>
public sealed class PathDemo
{
    public PathAddressBook Addresses { get; set; } = new();
    public PathCustomer Customer { get; set; } = new();
    public SourceStatus State { get; set; }
}

/// <summary>List subtype whose named Home member can reference its first inherited element.<br/></summary>
public sealed class PathAddressBook : List<PathAddress>
{
    public PathAddress Home { get; set; } = new();
}

/// <summary>Address data with a street and literal-keyed note values.<br/></summary>
public sealed class PathAddress
{
    public PathStreet Street { get; set; } = new();
    public Dictionary<string, PathNote> Notes { get; set; } = new();
}

/// <summary>Street data providing the shared address paths' terminal value.<br/></summary>
public sealed class PathStreet
{
    public int Number { get; set; }
}

/// <summary>Dictionary value addressed through an unescaped punctuation-bearing key.<br/></summary>
public sealed class PathNote
{
    public string Value { get; set; } = string.Empty;
}

/// <summary>Customer data sharing its contact with its company.<br/></summary>
public sealed class PathCustomer
{
    public PathCompany Company { get; set; } = null!;
    public PathContact Contact { get; set; } = null!;
}

/// <summary>Company node participating in a Company-to-Contact-to-Company cycle.<br/></summary>
public sealed class PathCompany
{
    public string Name { get; set; } = string.Empty;
    public PathContact Contact { get; set; } = null!;
}

/// <summary>Contact node whose name deliberately differs from its company's name.<br/></summary>
public sealed class PathContact
{
    public string Name { get; set; } = string.Empty;
    public PathCompany Company { get; set; } = null!;
}

/// <summary>Source enum whose symbolic names intentionally differ from the destination enum.<br/></summary>
public enum SourceStatus { StoredNegativeName = -1, StoredReadyName = 7 }

/// <summary>Destination enum assigning a different name to each compatible source numeric value.<br/></summary>
public enum TargetStatus { Queued = -1, Approved = 7, StoredReadyName = 8 }

/// <summary>Unrelated destination exercising enum-value reprojection through whole-object hydration.<br/></summary>
public sealed class PathStatusView
{
    public TargetStatus State { get; set; }
}
