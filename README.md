# Inheto

Inheto is a .NET-native binary serializer for the objects your application already uses. The intent is simple: persist your data without first redesigning your objects around a serializer. No serialization attributes, declared contracts, generated models, or application-assigned object identities are needed for supported shapes.

It handles both classes and structs, including sealed classes, nested collections, supported collection subclasses, and shared or circular references. The normal starting point is your existing objects—not a second set of serialization-only types. More than 50 built-in .NET types combine with supported arrays and collections into more than 25,000 tested shapes without custom handlers.

The binary is an addressable abstract syntax tree (AST). It records an object's data shape, not a requirement to recreate its original CLR class. You can read a property, a collection element, or a subtree directly, or hydrate the complete object into the same type or another compatible type.

Inheto is not BinaryFormatter and does not use its implementation or payload format. It uses BufferStream rather than BinaryReader/BinaryWriter. Ordinary member matching follows names and paths, not the source class's assembly version or declaration order. Adding members does not require regenerating a serialization contract; renamed paths can be mapped with aliases. Changes to incompatible value types still need an application migration policy.

## Quick start

Install the NuGet package with `dotnet add package Inheto`. The package provides a .NET 8 library asset usable by .NET 8, 9, and 10 applications. Its dependencies are restored automatically by NuGet.

Consider existing application types that look like this. They have no serialization-specific declarations, and being sealed does not change that:

```csharp
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
```

Use them as they are:

```csharp
using Inheto;

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
```

That is the basic workflow: serialize the object you have, then read only what you need. Public properties and fields are the default; options let you choose other supported member sets, include or exclude paths, map names, and supply custom serialization or activation where needed.

Native readers such as `ReadInt64` and `ReadInt32` read their corresponding stored representations. Use `ReadPropAs<T>` for requested-type hydration/conversion. `ReadPropExact<T>` also requires the result's runtime type to equal `T`; otherwise it returns `default(T)`.

For repeated work or explicit buffer ownership, use a reusable writer. Its returned BufferStream is disposed only after its final use; see [writer reuse and memory ownership](docs/performance-and-memory.md#reuse-a-writer-and-reader).

## Why choose Inheto?

- **Less preparation.** Common data objects already contain the values, arrays and collections Inheto understands. Custom handlers are an option for exceptional cases, not the normal starting point.
- **The graph remains a graph.** Shared references remain shared within a hydrated graph, and circular references are restored. A supported collection subclass keeps both its elements and its own selected properties/fields.
- **Read without rebuilding everything.** Check a status, a key, or an element inside the binary before deciding whether to hydrate the rest.
- **The data is not locked to its source class.** Hydrate matching paths into another compatible type, even one unrelated to the original. There is no requirement to instantiate the source class first.
- **Performance is part of the design.** Pooled buffers, reusable readers/writers and cached member access reduce repeated setup, copies and allocation pressure.

## Where it is useful

- **Keep data between application runs.** Save settings, a large configuration hierarchy, or ordinary application state without maintaining separate persistence models.
- **Make an independent copy of an object and everything inside it.** No hand-written clone method for every class and nested collection. Within the copy, shared objects are still shared and cycles still connect correctly.
- **Send objects over IPC or a network.** Use the same binary data for transport and persistence; the application supplies framing, compression and encryption when needed.
- **Hydrate stored Inheto data into a different type.** Populate a smaller view or a changed model from matching property, field and element paths. For example, `binary.ToObject<OrderSummary>()` can fill an unrelated type containing only `Id` and `Status`.
- **Inspect many records without creating many objects.** Read the few values needed to make a decision, then hydrate only the matching records.

The independent-copy case is often called *deep cloning*. With the `order` above:

```csharp
Order? copy = InhetoBinary.FromObject(order).ToObject<Order>();

// Changing the copy's nested data does not change the original order.
if (copy is not null)
    copy.Lines[0].Quantity = 99;
```

This copies the selected data graph, not every detail of a live runtime object. Application-supplied activators or singleton restoration can intentionally return existing instances; see [reconstruction rules](docs/configuration-and-reconstruction.md).

## Property paths without escape rules

The notation uses just a few markers:

| Syntax | Meaning |
| --- | --- |
| `.Name` | Named property or field |
| `#n` | Zero-based element number in serialized collection/array order |
| `!key` | Dictionary value under a supported text-addressable key; the key itself is literal text |
| `@i,j,...` | Rectangular array coordinates |
| `.Method()` | Explicit .NET continuation into a supported parameterless, non-void method |

These markers describe the notation, not a preliminary token-splitting step. For example:

| Path | Meaning |
| --- | --- |
| `.Status` | Property or field |
| `.Lines#1.Quantity` | Property of the second serialized collection element |
| `.Tags!region` | Dictionary value under a text-addressable key |
| `.Grid@1,2` | Rectangular array element at coordinates 1,2 |

More involved paths work the same way. For these examples, `Addresses` is a supported list subclass whose `Home` property references element `#0`. The customer and company share the same contact, and that contact's `Company` points back to the company:

| Path or equivalent paths | What it shows |
| --- | --- |
| `.Addresses#0.Street.Number` = `.Addresses.Home.Street.Number` | Two routes to the same address: its collection position and a named reference to it. |
| `.Addresses.Home.Notes!Important!.Value` | The key is `Important!`. The second `!` is part of that key and needs no escape. |
| `.Customer.Company.Contact.Company.Contact.Company.Name` = `.Customer.Company.Name` | A circular-reference route back to the same company's name. |
| `.Customer.Company.Contact.Company.Contact.Name` = `.Customer.Contact.Name` | A route through the cycle to the shared contact's name. |

Here `=` means both paths reach the same stored value in the described graph; it is not an expression passed to Inheto. Equivalent routes come from actual shared references, not an assumption that similarly named members are aliases.

Paths are checked progressively against complete stored names. They are not first split into tokens, and punctuation in supported text keys does not need escape codes. At `.Dict!a!b`, the literal key `a!b` takes precedence over key `a` followed by child `b`.

Shared or circular references do not make a path invalid, and binary navigation has no fixed 64-segment depth limit. A path can also continue beyond stored data into a .NET property or a supported parameterless, non-void method: `.Status.Length` and `.Status.ToUpperInvariant()` are examples. Those continuations are explicitly requested .NET operations, not extra data in the payload. See [literal keys, reference paths and CLR continuations](docs/property-paths.md).

## Built-in types

These values have built-in representations. They do not require an application converter just because they are more involved than a primitive. Notes below call out details that matter to an honest round trip; ordinary scalar rows need no extra qualification.

| Type | Notes |
| --- | --- |
| `bool` | |
| `sbyte` | |
| `byte` | |
| `short` | |
| `ushort` | |
| `int` | |
| `uint` | |
| `long` | |
| `ulong` | |
| `Int128` | |
| `UInt128` | |
| `Half` | |
| `float` | |
| `double` | |
| `decimal` | Preserves precision, sign, and scale; no conversion through `double`. |
| `BigInteger` | Preserves sign and arbitrary precision. |
| Enum types | Preserves the underlying signed/unsigned value; the textual member name is not stored. Can hydrate into a different enum with differently named members when their numeric meanings agree. |
| `char` | Preserves the UTF-16 code unit. |
| `Rune` | Preserves the Unicode scalar value, including characters outside the BMP. |
| `string` | Stores UTF-8 text; repeated equal strings can share payload storage. |
| `Guid` | Preserves the binary identifier, not formatted text. |
| `DateOnly` | Preserves the date without inventing a time or timezone. |
| `DateTime` | Preserves `Kind` through .NET's binary representation; local-time restoration follows .NET's timezone rules. |
| `DateTimeOffset` | Preserves clock ticks and the original offset, not just a UTC-normalized instant. |
| `TimeOnly` | Preserves time-of-day ticks, including subsecond precision. |
| `TimeSpan` | Preserves signed duration ticks. |
| `Version` | Preserves omitted components: `1.2` does not become `1.2.0.0`. |
| `Uri` | Restores relative and absolute URI values. |
| `Vector2` | |
| `Vector3` | |
| `Vector4` | |
| `Complex` | Preserves real and imaginary components. |
| `Quaternion` | Preserves components without normalizing them. |
| `Plane` | Preserves components without normalizing them. |
| `Matrix3x2` | |
| `Matrix4x4` | |
| `byte[]` | Preserves binary content, not base64 text. |
| `Memory<byte>` | Preserves content, not the original memory owner or slice identity. |
| `ReadOnlyMemory<byte>` | Preserves content, not the original memory owner or slice identity. |
| `MemoryStream` | Preserves content; the restored cursor starts at zero. |
| `StringBuilder` | Preserves text content, not capacity or chunk layout. |
| `SecureString` | Preserves character content as **unencrypted text**; apply payload protection outside Inheto. |
| `System.Type` | Stores type-name text marked as a Type value; the .NET reader resolves that name. |
| `CultureInfo` | Preserves the culture name, not customized instance settings. |
| `TimeZoneInfo` | Preserves the serialized timezone definition, not just an OS identifier. |
| `IPAddress` | Preserves address bytes and IPv6 scope ID. |
| `IPEndPoint` | Preserves address, scope ID and port. |
| `MailAddress` | Preserves address and display name. |
| `Regex` | Preserves pattern, options and match timeout. |
| `NameValueCollection` | Preserves separate values for a multi-value name. |
| `StringCollection` | Preserves string order and null slots. |
| `List<string>` | Preserves elements and null slots in a dedicated, individually addressable representation. |

Arrays and collections combine those values with nested application objects:

| Shape / type | Notes |
| --- | --- |
| `T[]` | Preserves length and individually addressable elements. |
| `T[,]` | Preserves rectangular dimensions and coordinate-addressable elements; core values need no converter. |
| `T[,,]` and higher rectangular ranks | Preserves the multidimensional shape, not just a flattened list. |
| Jagged arrays such as `T[][]` | Preserves nested lengths and supported shared array references. |
| Nullable values and arrays such as `int?` / `int?[]` | Preserves null values instead of using a sentinel value. |
| `List<T>` | |
| `LinkedList<T>` | Preserves enumeration order. |
| `Queue<T>` | Preserves FIFO order. |
| `Stack<T>` | Preserves stack order rather than reversing it. |
| `HashSet<T>` | Supported comparer-aware reconstruction. |
| `SortedSet<T>` | Supported comparer-aware reconstruction. |
| `Dictionary<TKey, TValue>` | Supported text keys can be addressed directly by their literal spelling. |
| `SortedDictionary<TKey, TValue>` | Supported comparer-aware reconstruction. |
| `SortedList<TKey, TValue>` | Supported comparer-aware reconstruction. |
| `ConcurrentDictionary<TKey, TValue>` | |
| `ConcurrentQueue<T>` | Preserves FIFO order. |
| `ConcurrentStack<T>` | Preserves stack order rather than reversing it. |
| `ConcurrentBag<T>` | Preserves bag contents; ordering is not its contract. |
| `ObservableCollection<T>` | Preserves contents, not event subscribers. |
| Supported enumerable and derived-collection shapes | Elements plus selected subclass members, using a supported construction/mutation contract. |
| Application structs | Selected properties and fields. |
| Application classes, including sealed classes | Selected properties and fields, shared references and cycles. |

Rectangular array support is for zero-based CLR arrays. Custom comparers and unusual construction/mutation requirements can use application hooks; recognizing an interface does not make every implementation reconstructible without configuration.

Strings and selected built-in value-like values can share larger payload data. Small inline header values can still repeat; arbitrary boxed structs are not universally deduplicated by value. See [types and round-tripping](docs/types-and-round-tripping.md) for the full representation and reconstruction details.

## Performance and practical use

Much of Inheto's performance comes from work it does not need to do. Reading one value does not require rebuilding the containing object. Raw span/memory views do not require copying the payload. Reusable readers/writers and pooled BufferStream storage reduce repeated setup and allocation pressure when processing many objects.

It is a .NET implementation with no separately deployed native engine. Cold setup, whole-object hydration and selective access are different workloads, so measure the graph and operations your application actually uses. See [performance, reuse and ownership](docs/performance-and-memory.md) and [comparison with other serializers](docs/comparisons.md); the comparisons distinguish supported features from measured speed or size claims.

Use a separate writer/reader per worker, or synchronize access. One writer/reader and its cursor are not safe for simultaneous operations; separate readers may share immutable, live backing bytes.

## Know your graph

The destination type and your application hooks determine how data becomes an object. Know what your getters, enumerators, constructors and setters do, just as you would when calling them directly. Special zero-input/singleton values and explicit `System.Type` values have additional reconstruction rules. Stored zero-input types can be restored automatically or restricted to explicitly approved types.

See [configuration and reconstruction](docs/configuration-and-reconstruction.md) for those controls, member inclusion/exclusion, aliases, custom codecs, activators, and the optional `TestType` compatibility diagnostic. Defaults preserve the normal supported reconstruction behavior.

## More detail

- [Types and round-tripping](docs/types-and-round-tripping.md)
- [Property paths](docs/property-paths.md)
- [Configuration and reconstruction](docs/configuration-and-reconstruction.md)
- [Performance and memory ownership](docs/performance-and-memory.md)
- [Comparison with .NET serializers and other formats](docs/comparisons.md)

## Runtime and license

The library targets .NET 8; independent package consumers also exercise .NET 9 and .NET 10. Later compatible runtimes can use the .NET 8 asset, with future runtime compatibility verified as those releases become available.

Runtime dependencies: BufferStream 1.0.2 and fasterflect 3.0.0. Compression/encryption helpers, test harnesses and review artifacts do not ship in the runtime assembly/package.

Apache-2.0.
