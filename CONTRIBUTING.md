# Contributing

Bug reports should include the Inheto version, .NET version, relevant options, a small object graph, and the expected versus actual behavior. Omit private data. Please discuss substantial API or binary-format changes before implementation.

Inheto prioritizes low developer friction, correctness, low allocation, and shallow hot paths. Preserve whole-object and selective-read behavior, collection order, reference identity, path handling, and existing reconstruction policies. Performance changes need representative measurement, not only a shorter implementation.

Build the library, then test the actual NuGet package rather than substituting a project reference. The maintained consumer suites cover README examples, activation, literal property paths, collection conversions, and the serialization-only release boundary. See [the release checklist](docs/RELEASING.md) for commands and validation scope.

Tests and profiling utilities are not part of the shipped library. Contributions are licensed under Apache-2.0, as is this repository.
