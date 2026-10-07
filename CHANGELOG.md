# Changelog

## 1.0.0

First public release of Inheto.

- Binary object-shape serialization and compatible-type hydration without required attributes or declared contracts.
- Shared and circular reference preservation, built-in scalar/value-like types, arrays, collections, and supported collection subclasses.
- Selective property-path reads, path aliases and exclusions, custom handlers, and activation hooks.
- Pooled BufferStream binary I/O; compression and encryption remain caller-owned operations outside Inheto.
- Public `TestType` compatibility diagnostics, XML documentation, README, and focused usage guides.
- A .NET 8 library asset, validated through independent NuGet consumers on .NET 8, 9, and 10.

See the usage guides for type-specific behavior, reconstruction policies, and ownership/lifetime considerations.
