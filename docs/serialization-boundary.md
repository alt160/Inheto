# Inheto's serialization boundary

Inheto serializes, navigates and reconstructs structured binary data. It does not
compress or encrypt that data. These storage and transport decisions belong to
the application using Inheto, including its choice of codec, keys, envelopes,
buffer ownership and limits.

The ordinary API remains:

```csharp
var binary = InhetoBinary.FromObject(source, serializationOptions, deserializationOptions);
```

For repeated work, reuse an `InhetoSerializer` and consume each result before the
next `Serialize` call resets its writer-owned stream. `FromObject` is the simpler
one-graph convenience factory; it creates a writer for that graph.

## Giving binary data to another component

The reader exposes the same plain indexed payload in three forms:

| API | Use | Ownership |
| --- | --- | --- |
| `RawSpan` | Synchronous span-based consumers | Borrowed view, no payload copy |
| `RawMemory` | Memory-based consumers | Borrowed view, no payload copy |
| `RawBytes` | Consumers requiring a `Stream` | Borrowed pooled segment with its own cursor; dispose the segment after use |

Do not mutate, rebind, reset or dispose the backing payload while a consumer is
using a borrowed view. When work outlives that payload, transfer ownership through
the application's buffer contract or explicitly copy the bytes. Inheto does not
require a `ToArray()` copy merely to hand bytes to another component.

An application can serialize, optionally compress, optionally encrypt, and then
store or transmit the result. On read, it reverses its transformations and gives
the recovered plain Inheto payload to `InhetoBinary.FromStream(...)`.
Inheto neither detects nor opens an application's transformed envelope.

Whole-payload transformations must be reversed before Inheto can navigate fields.
After decoding, a caller can still read just one field or collection element
without reconstructing the complete CLR object. Decoding an outer payload and
materializing an entire object graph are different operations.

## First-release API cleanup

The legacy `FromObject` overloads accepting a compression level, encryption key
or row identity have been removed; those arguments previously performed no
transformation. Use the plain factory or a reused writer, then transform its
binary output in caller code.

The legacy AES/Brotli helper source files remain in the local workspace but are
excluded from Inheto's runtime assembly and NuGet package. Their co-located
non-cryptographic helpers were audited and have no active Inheto callers.
`Brotli.NET` is no longer an Inheto dependency. This change does not remove or
alter Abraxas's separate outer payload transformation implementation.

The name-header layout, collection metadata and numbered-element addressing are
unchanged. There is no implicit graph-validation or transformation pass in the
serialization hot path. `TestType` remains an explicitly invoked developer
compatibility diagnostic, not a test harness or a prerequisite for serialization.

## Opt-in unsupported-representation failures

Set `SerializationOptions.FailOnUnsupportedType = true` to reject values that the
ordinary writer cannot preserve, rather than silently accepting an opaque
representation. It defaults to `false`; ordinary serialization remains the
compatibility path.

Strict mode rejects native addresses (`IntPtr`/`UIntPtr`), live `SafeHandle` and
`CriticalHandle` objects, executable delegates, selected members that cannot be
boxed (byref-like, by-reference, pointer and function-pointer types), and
unrecognized structs with instance state but no readable members under the
chosen `MemberTypes` policy. Empty marker structs and ordinary class graphs are
not rejected merely for being custom CLR types.

Member/type exclusions and the inclusion policy are applied first. Registered
member/type serializers also take precedence over value-type rejection; for an
unboxable member, exclude it or serialize its owning object instead. Explicitly
selecting private-field serialization can provide a representation for a struct
whose state is private. Intentionally excluding otherwise available members does
not turn the type into an unsupported one. Null slots and custom serializers
that intentionally return null retain their existing meaning.

Checks happen during the existing writer traversal, not through an automatic
`TestType` pass. Getter, enumerator and custom-serializer exceptions still
propagate; this flag is not a guarantee of complete CLR reconstruction or a
security boundary for untrusted input.

A strict failure throws `NotSupportedException`. Its message identifies the
type and full Inheto path; `exception.Data["Inheto.Type"]` holds the `Type`, and
`exception.Data["Inheto.Path"]` holds the path (empty for the root). Discard the
partial writer buffer after an exception. The next `Serialize` call starts a
fresh operation and safely reuses the writer, including circular-member rules.
