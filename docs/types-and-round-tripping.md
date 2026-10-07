# Types and round-tripping

Guides supplement the [README](../README.md). Examples refer to its `Order` and `OrderLine` types unless stated otherwise.

Inheto has native representations for a broad set of .NET values, rather than limiting every nontrivial value to a developer-supplied converter.

The built-in set covers more than 50 concrete .NET types. Together with supported nullable values, arrays and collection families, it supports more than 25,000 type combinations without custom handlers. Each supported shape has been exercised at least once in testing. Application-defined objects and inherited collections extend that range further, subject to their member and construction contracts. Rectangular multidimensional arrays of core elements—such as `int[,]`, `decimal[,,]` or `Guid[,]`—are native supported shapes too, not something requiring a custom converter.

| Family | Examples |
| --- | --- |
| Numeric and scalar | Boolean; signed/unsigned integers through 128 bits; `Half`, `float`, `double`, `decimal`, `BigInteger`; enums |
| Text and identifiers | `char`, `Rune`, `string`, `Guid`, `Uri`, `Version`, `SecureString` |
| Date and time | `DateOnly`, `DateTime`, `DateTimeOffset`, `TimeOnly`, `TimeSpan` |
| Numerics structures | `Vector2/3/4`, `Complex`, `Quaternion`, `Plane`, `Matrix3x2/4x4` |
| Binary and selected framework values | `byte[]`, `Memory<byte>`, `ReadOnlyMemory<byte>`, `MemoryStream`, `StringBuilder`, selected networking/culture/time-zone/regex types |
| Collections and graphs | Single-dimensional, jagged and rectangular multidimensional arrays (including core-type elements), supported nullable arrays, common lists/queues/stacks/sets/dictionaries, supported enumerable and inherited-collection shapes, nested members and shared references |

### Built-in values and round-trip details

Each row below is a supported value type or framework type. The notes describe what its native representation retains, rather than assuming that turning every value into text or an unspecified timestamp is sufficient. Collections and arrays compose these representations; they do not require a separate converter for every element-type combination.

| Family | Type | Important representation / round-trip notes |
| --- | --- | --- |
| Scalar | `bool` | Native Boolean value. |
| Integer | `sbyte` | Signed 8-bit value. |
| Integer | `byte` | Unsigned 8-bit value. |
| Integer | `short` | Signed 16-bit value. |
| Integer | `ushort` | Unsigned 16-bit value. |
| Integer | `int` | Signed 32-bit value. |
| Integer | `uint` | Unsigned 32-bit value. |
| Integer | `long` | Signed 64-bit value; no conversion through a floating-point number. |
| Integer | `ulong` | Unsigned 64-bit value, including values above `long.MaxValue`. |
| Integer | `Int128` | Native signed 128-bit representation. |
| Integer | `UInt128` | Native unsigned 128-bit representation. |
| Floating point | `Half` | Native 16-bit floating-point representation, not formatted text. |
| Floating point | `float` | Native 32-bit floating-point representation. |
| Floating point | `double` | Native 64-bit floating-point representation. |
| Decimal | `decimal` | Preserves precision, sign, and scale; not converted through `double`. |
| Integer | `BigInteger` | Preserves sign and arbitrary precision in integer bytes, not a fixed-width approximation. |
| Enumeration | Enum types | Preserves signed/unsigned underlying values, not textual member names or a required source enum class name. The destination supplies the enum type, allowing hydration into differently named enum members when their numeric meanings agree. |
| Text | `char` | Preserves one UTF-16 code unit. |
| Text | `Rune` | Preserves the Unicode scalar value, including characters outside the BMP. |
| Text | `string` | Stores UTF-8 text; repeated equal strings can share payload storage. |
| Identifier | `Guid` | Preserves the native 128-bit identifier, not a formatted string. |
| Date/time | `DateOnly` | Preserves the day number; no invented time or timezone. |
| Date/time | `DateTime` | .NET binary date/time representation retains `Kind`; not reduced to raw ticks without kind or normalized indiscriminately to UTC. Local-time restoration follows .NET's timezone rules. |
| Date/time | `DateTimeOffset` | Clock ticks and the original offset are stored separately; the offset is not discarded by converting everything to UTC. |
| Date/time | `TimeOnly` | Preserves time-of-day ticks, including subsecond precision. |
| Date/time | `TimeSpan` | Preserves signed duration ticks; not rounded to whole seconds. |
| Versioning | `Version` | Preserves major/minor/build/revision, including omitted build/revision components; `1.2` is not changed into `1.2.0.0`. |
| Address | `Uri` | Reconstructs relative or absolute URI values; not a promise to preserve every original textual spelling or URI implementation detail. |
| Numerics | `Vector2` | Preserves both floating-point components. |
| Numerics | `Vector3` | Preserves all three floating-point components. |
| Numerics | `Vector4` | Preserves all four floating-point components. |
| Numerics | `Complex` | Preserves real and imaginary double-precision components. |
| Numerics | `Quaternion` | Preserves all four components without normalizing the supplied value. |
| Numerics | `Plane` | Preserves normal components and distance without normalizing the supplied value. |
| Numerics | `Matrix3x2` | Preserves all six floating-point components. |
| Numerics | `Matrix4x4` | Preserves all sixteen floating-point components. |
| Binary | `byte[]` | Preserves binary content, not a base64/text representation. |
| Binary | `Memory<byte>` | Contents are stored; the original memory owner and slice identity are not reconstructed. |
| Binary | `ReadOnlyMemory<byte>` | Contents are stored; original backing ownership is not reconstructed. |
| Stream | `MemoryStream` | Stream contents; a reconstructed stream starts at position zero. Original capacity, writability and cursor are not its data contract. |
| Text builder | `StringBuilder` | Preserves text content, not capacity, chunk layout or maximum-capacity settings. |
| Sensitive text | `SecureString` | Character content can be reconstructed as a `SecureString`, but it is serialized as **unencrypted text**. Read-only state and protection of the original instance are not carried into the payload. |
| Reflection | `System.Type` | Stores assembly-qualified type-name text plus the Type value kind. The .NET reader resolves the name; another runtime can treat it as text. |
| Globalization | `CultureInfo` | Preserves the culture name, not arbitrary customized formatting/calendar state on an instance. |
| Timezone | `TimeZoneInfo` | Preserves .NET's serialized timezone definition, rather than only an OS timezone identifier. |
| Networking | `IPAddress` | Preserves address bytes and IPv6 scope ID. |
| Networking | `IPEndPoint` | Preserves address bytes, IPv6 scope ID and port. |
| Networking | `MailAddress` | Preserves address and display name. |
| Pattern | `Regex` | Preserves pattern, options and match timeout; not just the pattern string. |
| Specialized collection | `NameValueCollection` | Preserves named entries and their separate value arrays, rather than flattening a multi-value entry into one joined string. |
| Specialized collection | `StringCollection` | Preserves string element order and null slots. |
| Specialized collection | `List<string>` | Preserves elements and null slots in a dedicated string-list representation with individually addressable numbered elements. |

**SecureString is not payload encryption.** Inheto obtains ordinary text when serializing it and stores those characters in the binary. Do not assume that a binary payload hides a password or secret. Apply the application's encryption/access policy outside Inheto, as with any other sensitive serialized value.

### Arrays, collections and inherited shapes

| Shape / type | Important structure / reconstruction notes |
| --- | --- |
| `T[]` | Element type, length and individually addressable numbered elements. |
| `T[,]` | Native zero-based rectangular array with rank, dimension lengths and coordinate-addressable elements such as `.Grid@1,2`. Core elements need no converter. |
| `T[,,]` and higher rectangular ranks | Native multidimensional shape, not merely a flat list with dimensions left to application code; supported core elements use their native representations. |
| Jagged arrays such as `T[][]` | Separate nested array shapes/lengths, including supported shared array references. |
| Nullable value shapes such as `int?` / `int?[]` | Nullable value representations and array null slots, not a sentinel numeric value standing in for null. |
| `List<T>` | Numbered elements; `List<string>` also has a dedicated native representation. |
| `LinkedList<T>` | Supported enumerable reconstruction restores elements in enumeration order. |
| `Queue<T>` | FIFO collection reconstruction. |
| `Stack<T>` | Reconstruction accounts for stack order; not a naive forward loop that reverses the stack. |
| `HashSet<T>` | Set contents, with comparer-aware reconstruction; no stable enumeration-order promise. |
| `SortedSet<T>` | Sorted set reconstruction under the supported/configured comparer. |
| `Dictionary<TKey, TValue>` | Key/value structure; admitted text keys use literal-key addressing, other supported key shapes use structured entries. |
| `SortedDictionary<TKey, TValue>` | Key/value structure with comparer-aware sorted reconstruction. |
| `SortedList<TKey, TValue>` | Key/value structure with comparer-aware sorted reconstruction. |
| `ConcurrentDictionary<TKey, TValue>` | Supported dictionary representation; concurrent source changes do not imply an atomic snapshot. |
| `ConcurrentQueue<T>` | Supported FIFO reconstruction; coordinate numbers follow serialized enumeration. |
| `ConcurrentStack<T>` | Supported stack reconstruction without reversing the serialized pop order. |
| `ConcurrentBag<T>` | Supported bag contents; ordering is not part of its contract. |
| `ObservableCollection<T>` | Supported collection contents; notification subscribers are not serialized. |
| Supported derived collections | Both inherited elements and the subclass's selected properties/fields. Construction and mutation still follow the supported destination contract. |
| Ordinary value-type objects | Selected members of structs can be graphed and reconstructed, not only reference-type POCOs. |
| Ordinary reference-type objects | Selected properties/fields, nested objects, shared references and cycles. |

These are supported representations, not a guarantee for every custom implementation of an interface. Arbitrary comparer implementations, constructors and opaque custom codecs can require configuration. Standard multidimensional array handling is for zero-based CLR arrays; do not infer support for nonzero lower bounds from the rank examples above.

Shared references and cycles are represented using reference entries and reconstructed within a materialization operation. Separate `ToObject`/`ReadPropAs` calls are independent operations; do not expect them to return the same mutable instance. Custom activation, member selection, and incompatible destination shapes can affect what is reconstructed.

Repeated strings and selected built-in value-like objects can share an encoded payload instead of repeating their larger data. Small inline header values can still repeat, and distinct boxed structs are not universally deduplicated by value. Both value-type and reference-type application objects are supported; representation and reconstruction depend on their selected members and construction contract.

### Data shape without a dependency on the original runtime

The encoded AST carries names, format-defined value kinds, collection structure, reference relationships and data. Ordinary object graphs do not require an assembly-qualified CLR class contract attached to each object. Reading their structure does not require executing .NET or loading the application's original assemblies. A reader in another runtime could interpret that structure under its own type system; this package supplies the .NET implementation, not a cross-language SDK.

An actual `System.Type` value is a deliberate exception to the idea of having no .NET-related text: Inheto records its assembly-qualified name as a string marked with the format's Type value kind. Special static-class representations can also contain a type-name string. Another runtime can read that text and decide how to interpret it; it is not required to activate the CLR type. Custom opaque codecs may impose their own runtime-specific contract too.

On .NET, the destination type supplies the reconstruction contract. It need not be related to the serialized source type: matching property, field and element paths can populate a different compatible shape. Member selection, aliases, custom decoders and activators let the application control that mapping.
