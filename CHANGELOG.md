# Changelog

## 1.1.0 - 2026-10-12

- Add `DeserializationOptions.UnhandledCustomProperties` with `Ignore` and `AsBinary` policies for member-specific custom payloads without an applicable reader.
- The default now retains initialized property/field values instead of attempting an incompatible binary assignment. Select `AsBinary` for the previous raw-payload conversion behavior. Explicit raw reads and registered readers remain unchanged.
- Preserve the policy when options are forked for selected hydration; registered-reader null results and exceptions retain their normal behavior.
- Correct collection comparer lookup to use the requested collection type rather than its owning object's type.
- No wire-format change. Continue using the public BufferStream 1.0.2 package and validating package-only consumers on .NET 8, 9 and 10.

## 1.0.1

- Explain the name Inheto: **IN**dexed **HE**ader **T**o **O**ffsets.
- Documentation-only release; no serializer implementation or public API changes.

## 1.0.0

First public release of Inheto.

- Binary object-shape serialization and compatible-type hydration without required attributes or declared contracts.
- Shared and circular reference preservation, built-in scalar/value-like types, arrays, collections, and supported collection subclasses.
- Selective property-path reads, path aliases and exclusions, custom handlers, and activation hooks.
- Pooled BufferStream binary I/O; compression and encryption remain caller-owned operations outside Inheto.
- Public `TestType` compatibility diagnostics, XML documentation, README, and focused usage guides.
- A .NET 8 library asset, validated through independent NuGet consumers on .NET 8, 9, and 10.

See the usage guides for type-specific behavior, reconstruction policies, and ownership/lifetime considerations.
