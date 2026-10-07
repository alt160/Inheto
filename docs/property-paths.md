# Property paths

Guides supplement the [README](../README.md). Examples refer to its `Order` and `OrderLine` types unless stated otherwise.

Inheto stores a name header describing members and their encoded values. A collection member's entry carries collection metadata, such as its count or shape, and its child elements have numbered names: `#0`, `#1`, and so on. The same representation supports both addressing an element and reconstructing a collection instance.

### Notation first

| Syntax | Meaning |
| --- | --- |
| `""` | Root value, where supported by the selected API |
| `.Name` | Named property or field |
| `#n` | Zero-based element number in serialized collection/array order |
| `!key` | Dictionary value under an admitted text-addressable key; the key itself is literal text |
| `@i,j,...` | Rectangular array coordinates |
| `.Method()` | Explicit CLR continuation into an admitted parameterless, non-void method |

These are notation markers, not an instruction to split a binary path into tokens. A dot beyond a natively stored value can also continue into an admitted CLR property; explicit CLR continuation is explained below.

### Examples

| Path | Meaning |
| --- | --- |
| `""` | Root value, where supported by the selected API |
| `.Status` | Named property or field |
| `.Customer.Name` | Nested named member |
| `.Lines` | Collection itself |
| `.Lines#0` | First collection element |
| `.Lines#0.Sku` | Member of the first collection element |
| `.Tags!region` | Dictionary value under an admitted, text-addressable key |
| `.Grid@1,2` | Rectangular array element at coordinates 1,2 |

`#n` follows the serialized enumeration order. It is not an identity, and it does not impose stable ordering on unordered sets or dictionaries. Text-addressable dictionary keys use `!key`; keys that require structured representations use numbered entries with separate Key/Value children instead.

Arrays, common generic collections, and supported inherited collection types retain collection structure rather than being treated solely as ordinary objects. A derived collection can have both its inherited elements and its own declared members. Reconstruction still depends on the destination's supported construction and mutation contract.

For a concrete example, let `Addresses` be a supported list subclass with a `Home` property referencing its first element. Each address has a `Street` and a dictionary of `Notes`:

- `.Addresses#0.Street.Number` and `.Addresses.Home.Street.Number` reach the same street number through different routes to the same address instance.
- `.Addresses.Home.Notes!Important!.Value` addresses the `Value` of the note stored under key `Important!`. The first `!` introduces dictionary addressing; the second belongs to the key. It does not need to be escaped.

For a circular graph, let `Customer.Company.Contact` reference `Customer.Contact`, and let that contact's `Company` reference `Customer.Company`:

- `.Customer.Company.Contact.Company.Contact.Company.Name` reaches the same value as `.Customer.Company.Name`.
- `.Customer.Company.Contact.Company.Contact.Name` reaches the same value as `.Customer.Contact.Name`.

The repeated steps traverse actual references, not copies of objects. The final member still matters: `Company.Name` and `Contact.Name` belong to different objects in this graph. Path equivalence follows the graph's reference relationships, not the spelling of member names.

The name header also carries type/value descriptors and locations. Some small scalar values are held inline; other values are reached through offsets. Selective reads avoid reconstructing unrelated CLR objects, but path navigation still has a cost: this is not a blanket constant-time lookup claim or a database index.

### Paths match the stored structure, not pre-split tokens

Binary navigation matches complete stored names progressively against the unchanged request. It does not split the request at dots or other markers, decode escape sequences, or allocate an array of path tokens. Prepared binary paths retain the same unchanged text and a numeric cache identity; preparation does not introduce tokenization.

For dictionary values, the most-specific matching key wins. At `.Dict!a!b`, Inheto first reaches `.Dict` and looks for the literal key `a!b`. Only when that key is absent does it consider key `a` and continue into its child `b`. The rule is independent of dictionary enumeration order and applies to longer continuations too. Once the most-specific key is selected, a null value or missing child does not redirect the request to a shorter key.

Consequently, punctuation in an admitted text key—including `.`, `!`, `#`, `@`, quotes and backslashes—does not require escape syntax. Shared-reference and circular-reference paths can continue through the stored references just like direct paths; the binary resolver has no fixed 64-segment depth limit.

Two physical locations can nevertheless spell the same path. The literal-key rule determines which one a direct read addresses; whole-object reconstruction retains both entries. `AllNames` returns unique addressable path strings, not every physical location under a distinct spelling. Optional CLR member/method expression helpers are a separate feature with their own expression syntax, not the binary path resolver.

### Continuing beyond the stored path into .NET

A path can reach a natively stored value and then continue through its CLR properties or admitted parameterless, non-void methods. For example, `.Status.Length` reads the stored string and then its .NET `Length` property; `.Status.ToUpperInvariant()` reads that string and invokes the method. A path through a stored `DateTime` can continue to `.Year` or `.ToLongDateString()` without reconstructing the enclosing application object.

For repeated logical-path checks, `CheckPropPath` prepares a nonexecuting structural plan and `TryReadPropPath` reports value, null, missing-source, invalid-path, and invocation-failure outcomes. Using the `Order` from the quick start:

```csharp
var lengthPath = InhetoBinary.CheckPropPath<Order, int>(".Status.Length");
var upperPath = InhetoBinary.CheckPropPath<Order, string>(".Status.ToUpperInvariant()");

if (binary.TryReadPropPath<int>(lengthPath, out var length) == PropPathReadState.Value)
    Console.WriteLine(length); // 5: the stored "ready" string's Length.

if (binary.TryReadPropPath<string>(upperPath, out var upper) == PropPathReadState.Value)
    Console.WriteLine(upper); // READY: the .NET method's result, not a stored member.
```

The reader materializes the deepest stored prefix needed for this operation, then executes only the remaining CLR suffix through cached accessor machinery. Properties/fields use the admitted CLR member policy; methods must be parameterless and return a type admitted by that policy—non-void alone is not sufficient to admit every possible method. Methods with arguments and void methods are not supported by this path facility. Use `()` to make method intent clear.

This is an executable .NET continuation, not more data stored in the AST or runtime-independent evaluation. Getters/methods can allocate, throw, have side effects or depend on culture/current state. `CheckPropPath` does not execute them; the actual read does. Prepare and reuse the plan when possible, and keep CLR continuation expressions separate from binary literal-key navigation: the binary resolver still matches intact stored names without tokenizing the request.
