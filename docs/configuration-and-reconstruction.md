# Configuration and reconstruction

Guides supplement the [README](../README.md). Examples refer to its `Order` and `OrderLine` types unless stated otherwise.

Public instance properties and fields are selected by default. `SerializationOptions` lets an application choose a member-visibility policy, include/exclude member paths, exclude exact runtime types, or register custom representations. When using an inclusion list, include the containing member paths needed to reach nested values; it is not a recursive wildcard query.

Path-based inclusion/exclusion can target properties, fields, collection elements and array elements. Read-time aliases map stored paths to destination paths without rewriting the binary. Custom serializers and deserializers can be supplied independently or as a pair; activators support types that need constructor arguments or factory methods rather than a parameterless constructor.

```csharp
var writeOptions = new SerializationOptions
{
    MemberTypes = MemberTypesEnum.PublicPropertiesAndFields,
    FailOnUnsupportedType = true
};
writeOptions.ExcludedMembers.Add(".InternalNote");

var readOptions = new DeserializationOptions();
readOptions.Aliases.Add(".Status", ".State"); // Read-time equivalent paths; no rewrite.

var configured = InhetoBinary.FromObject(order, writeOptions, readOptions);
```

Custom member serializers can supply text or bytes; exact-type serializers supply bytes. Pair them with corresponding deserialization registrations when reconstructing application values. Text member hooks use UTF-8. Finish configuring options before use; registrations are mutable and must not be edited concurrently with serialization or reads. `DeserializationOptions` also supports activation rules and comparer-aware reconstruction.

`FailOnUnsupportedType` defaults to `false`. When enabled, it rejects specified unsupported ordinary representations, including native addresses/handles, delegates, selected unboxable members, and stateful structs opaque under the chosen member policy. Exclusions and registered custom serializers take precedence. A strict failure is a `NotSupportedException` identifying the type and path, also available through `exception.Data["Inheto.Type"]` and `exception.Data["Inheto.Path"]`.

Serialization and reconstruction use the getters, enumerators, constructors, setters and custom hooks belonging to your application types. Choose these members with the same care as when calling them directly.

For an explicit compatibility probe during development:

```csharp
var diagnosticWriter = new InhetoSerializer(writeOptions);
var report = diagnosticWriter.TestType(order);
foreach (var issue in report.Issues)
    Console.WriteLine($"{issue.MemberPath}: {issue.Message}");
```

`TestType` is a developer-facing diagnostic, not a test harness or a required serialization step. Its bounded traversal can report throwing getters, enumeration problems, unsupported keys/values, and configuration suggestions. It may execute getters, enumeration, and custom serializers; by default it can call getters twice to probe stability. A clean report is guidance, not proof about unvisited values or future mutations.

## Know your graph

For source graphs you own, start with the behavior of the types you store and the destination types you request.

- Reading an ordinary scalar does not reconstruct its surrounding object. A projection omitting a member does not construct that member.
- Writing objects uses selected getters and collection enumerators. Reconstructing ordinary objects uses the destination types you request and their constructors/setters, plus any application hooks you register.
- A special zero-input/singleton representation (`StaticClass` in the format) records a CLR type name so it can restore a type with no ordinary input state. This can use a public parameterless constructor, a static singleton property/field, or a public/nonpublic parameterless factory. It is distinct from ordinary shape-based object reconstruction.
- An explicit `System.Type` value records a type-name string. The .NET reader resolves that metadata; resolving it is not the same as constructing an instance. Type-valued dictionary keys also resolve type names.
- Registered decoders/activators and explicitly requested CLR continuations invoke the developer-selected code. No binary payload installs a new decoder or application callback.

Know whether your graph includes special zero-input shapes or Type values, and which construction/member behavior you want on the destination. Writer exclusions can omit exact nested members or runtime types; they do not apply to the root and do not rewrite data received from elsewhere.

When accepting data from outside your application, choose the expected destination shapes, appropriate message-size/provenance policies, and the optional construction behavior you intend to permit. These choices belong to the application; the controls below govern one specific reconstruction feature, not input authenticity or every callback.

### Optional control over stored-type construction

The compatible default remains `AllowStoredTypeActivation = true`. Existing graphs do not need an approval registry.

To use only caller-selected concrete destinations or explicitly approved stored zero-input types:

```csharp
var options = new DeserializationOptions { AllowStoredTypeActivation = false };
// Add only if this graph uses a zero-input/singleton type in an object/interface
// member, an untyped read, or ReadStaticClass:
options.AllowStoredType<MySingleton>();

var reader = InhetoBinary.FromStream(input, options);
```

Here `input` is an application-owned BufferStream containing plain Inheto bytes and `MySingleton` is an application type.

With that flag false:

- A concrete typed destination can supply the reconstruction Type directly, without resolving the historical source class name.
- An object/interface/abstract destination or native `ReadStaticClass` requires an exact `AllowStoredType<T>()` approval. Without one it throws `NotSupportedException` before stored-type construction.
- An approved stored Type must be assignable to the requested destination; otherwise it is rejected before construction. Approval takes precedence over choosing a different concrete shape.
- Approvals retain the Type directly under its exact assembly-qualified name. They do not install a constructor or decoder. Configure mutable options before use.
- `AllowStoredType(typeof(MySingleton))` supplies the same registration dynamically. `RemoveAllowedStoredType<MySingleton>()` removes it. Native reads check approval before returning even a cached value.
- An exact `AddActivator<T>(...)` still controls construction, including an intentional null result. Registering an activator alone does not implicitly approve an untyped stored identity.

This option does not alter ordinary scalar/object/collection handling, explicit System.Type metadata, Type keys, registered codecs, or CLR continuation expressions. It adds no automatic preflight traversal.

A native `ReadStaticClass` checks for the corresponding encoded representation; a string or Type value is not treated as a request to construct its named type. Singleton/factory discovery returns its first successful result directly rather than calling that successful getter/factory a second time.

## Boundaries to know

- **Root values must be non-null.** Ordinary null properties/fields are omitted; collection null slots can be explicitly represented. Missing and explicit-null are not interchangeable in every API. Native nullable readers commonly return null for either; use the path-state APIs when the distinction matters.
- **Filling is not unconditional replacement.** `ReadPropOnto` keeps existing destination/member semantics. Existing collections can retain/add entries; omitted members are not a universal instruction to clear the destination.
- **Custom opaque payloads hide their internals.** Inheto can address their containing entry, but it cannot navigate fields inside an arbitrary custom byte representation unless application decoding supplies that behavior.
- **Compression and encryption belong to callers.** Transform the serialized bytes for storage or transport, then reverse those transformations before opening them with Inheto. Selective object reconstruction does not imply selective decryption/decompression of a whole-payload envelope.
- **Application contracts still matter.** Names, member policies, custom codecs, activation rules, and destination types are part of your data contract. A binary format version is not automatic application-schema migration.
