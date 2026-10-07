# Performance and memory

Guides supplement the [README](../README.md). Examples refer to its `Order` and `OrderLine` types unless stated otherwise.

Inheto is designed to minimize avoidable work, not to claim that serialization itself is allocation-free.

- **Read less.** Direct typed reads can avoid creating enclosing objects and collections. A string result still needs a string; a reconstructed object still needs its requested state.
- **Reuse the writer.** `InhetoSerializer.Serialize` resets and reuses its BufferStream instead of requiring a new output buffer for every graph. BufferStream uses pooled backing storage.
- **Reuse the reader.** `RebindBorrowedPayload` replaces the payload without retaining the prior payload's name/value materialization state. Reusable type and prepared-path machinery remains available.
- **Avoid unnecessary copies.** `RawSpan` and `RawMemory` expose the encoded payload without a `ToArray()` copy. Existing array-backed `ReadOnlyMemory<byte>` can back a reader through BufferStream without another payload copy; non-array-backed read-only memory uses BufferStream's copying fallback.
- **Amortize setup.** Type dispatch, accessors, and materialization plans are cached; cold-start setup and warmed repeated calls are different workloads.
- **Match paths without token allocations.** Binary lookup compares borrowed header names with unchanged request text. Dynamic collection paths do not need per-call path-plan objects, split strings, or encoded request arrays. Numeric resolution caches retain offsets, not decoded objects.
- **Keep validation explicit.** Normal serialization does not run an automatic `TestType` preflight over the graph. Optional strict representation checks run during the existing writer traversal.

The addressable header has storage and traversal overhead. A tiny object that is always reconstructed in full may benefit much less than a large graph that is usually read selectively. Whole-object throughput, payload size, cold-start costs, and allocations should be measured with the application's actual types and access pattern. No comparative throughput/allocation ranking is currently published. Historical measurements can motivate benchmarks, but any published comparison must use equivalent correctness, reference semantics and access patterns.

### Reuse a writer and reader

For one graph, `FromObject` is convenient. For repeated work, reuse the writer explicitly. Assuming the `order` from the quick start:

```csharp
using System.IO;

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
```

`RebindBorrowedPayload` requires a reader over a borrowed root BufferStream, as created from memory in this example. A reader opened directly over the writer's owning stream cannot use this rebinding operation. The extra wrapper borrows the bytes; it does not copy the payload.

Consume or detach each result before the next `Serialize` call. An old reader or borrowed view must not be used after its backing bytes are replaced; rebind the reader before reading the new payload. Do not dispose the writer's returned stream inside a loop if you intend to keep using that writer. After a serialization exception, discard the partial payload; the next call starts a fresh operation.

## Binary data and ownership

| API | What it provides | Lifetime |
| --- | --- | --- |
| `RawSpan` | Read-only span over the encoded payload; no copy | Borrowed synchronous view |
| `RawMemory` | Read-only memory over the encoded payload; no copy | Borrowed memory, not an ownership transfer |
| `RawBytes` | Pooled BufferStream segment with an independent cursor | Dispose the segment; keep its backing payload valid |
| `FromStream(stream)` | Reader over an existing plain Inheto payload; no copy | Supplied stream must remain alive and unchanged; its cursor is reset to zero |
| `ShallowClone()` | Reader with its own cursor and reference cache over the same bytes | Shared backing bytes, not a detached payload copy |

To open bytes that your application already owns:

```csharp
using System.IO;

// This copy is intentional: savedBytes will outlive the original payload.
// Skip the copy when your application's existing memory-owner contract is sufficient.
byte[] savedBytes = binary.RawMemory.ToArray();
using var input = new BufferStream((ReadOnlyMemory<byte>)savedBytes);
var reopened = InhetoBinary.FromStream(input);
string? savedStatus = reopened.ReadString(".Status");
```

Keep borrowed storage alive and do not modify, dispose, reset, or rebind it while readers, name entries, or raw views depend on it. A read-only view does not make the underlying memory immutable. Detached results such as an independently reconstructed object can outlive the payload; borrowed wrappers and segments cannot.

`InhetoBinary` and `InhetoSerializer` do not themselves implement `IDisposable`. `FromObject` creates an internal writer; use the explicit writer/stream pattern above when you need deterministic disposal of its pooled output storage. Disposing `RawBytes` releases that segment, not the owning writer's payload.

## Thread safety

Use a separate writer and reader per worker, or synchronize access yourself. One `InhetoSerializer`, one `InhetoBinary`, and their stream/cursor state are not safe for simultaneous operations. A thread-safe source collection does not make the serializer instance thread-safe, nor does it guarantee a consistent snapshot while the collection is changing.

Separate readers can use the same immutable payload with separate cursors. Keep the backing memory alive, keep options/registrations stable, and make any shared developer callbacks safe for concurrent use. Shallow clones share bytes; they do not create independent ownership. Borrowed low-level name entries and views remain subject to their own lifetime rules.

