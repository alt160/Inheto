//====== TYPES ======

namespace Inheto
{
    /// <summary>
    /// Stores equivalent property paths used while reading an Inheto payload.<br/>
    /// The caller-requested path is always tried first, followed by the canonical path and then the remaining aliases in registration order.<br/>
    /// Registrations affect reads only; serialization continues to use the runtime object's actual member names.<br/>
    /// </summary>
    public sealed class PropertyAliasCollection
    {
        internal sealed class Group
        {
            internal Group(string canonicalPath, string[] paths)
            {
                CanonicalPath = canonicalPath;
                Paths = paths;
            }

            internal string CanonicalPath { get; }
            internal string[] Paths { get; }
        }

        private readonly object sync = new();
        private Dictionary<string, Group> snapshot = new(StringComparer.Ordinal);

        /// <summary>Gets the number of registered equivalence groups.<br/></summary>
        public int Count
        {
            get
            {
                lock (sync)
                    return snapshot.Values.Distinct().Count();
            }
        }

        /// <summary>
        /// Registers one canonical property path and its equivalent historical or projected names.<br/>
        /// A payload member whose exact requested name exists wins; otherwise the canonical path wins, followed by aliases in the supplied order.<br/>
        /// A path may belong to only one equivalence group so resolution remains deterministic.<br/>
        /// </summary>
        /// <param name="canonicalPath">Stable path used as the first fallback when the requested path is absent.<br/></param>
        /// <param name="aliases">Equivalent stored or requested paths.<br/></param>
        /// <returns><see langword="true"/> when a new group was registered; <see langword="false"/> when the identical group already exists.<br/></returns>
        public bool Add(string canonicalPath, params string[] aliases)
        {
            string canonical = Normalize(canonicalPath, nameof(canonicalPath));
            ArgumentNullException.ThrowIfNull(aliases);
            if (aliases.Length == 0)
                throw new ArgumentException("At least one property-path alias is required.", nameof(aliases));

            var paths = new List<string>(aliases.Length + 1) { canonical };
            var unique = new HashSet<string>(StringComparer.Ordinal) { canonical };
            for (int index = 0; index < aliases.Length; index++)
            {
                string alias = Normalize(aliases[index], nameof(aliases));
                if (unique.Add(alias))
                    paths.Add(alias);
            }
            if (paths.Count == 1)
                throw new ArgumentException("An alias must differ from the canonical property path.", nameof(aliases));

            lock (sync)
            {
                Dictionary<string, Group> current = snapshot;
                Group? existing = null;
                for (int index = 0; index < paths.Count; index++)
                {
                    if (!current.TryGetValue(paths[index], out Group? found))
                        continue;
                    if (existing is null)
                        existing = found;
                    else if (!ReferenceEquals(existing, found))
                        throw new InvalidOperationException($"Property alias '{paths[index]}' already belongs to another alias group.");
                }

                if (existing is not null)
                {
                    if (existing.Paths.Length == paths.Count && existing.Paths.SequenceEqual(paths, StringComparer.Ordinal))
                        return false;
                    throw new InvalidOperationException($"Property alias group '{existing.CanonicalPath}' already contains one or more requested paths.");
                }

                var group = new Group(canonical, paths.ToArray());
                var next = new Dictionary<string, Group>(current, StringComparer.Ordinal);
                for (int index = 0; index < group.Paths.Length; index++)
                    next.Add(group.Paths[index], group);
                snapshot = next;
                return true;
            }
        }

        /// <summary>
        /// Removes the equivalence group containing the supplied path.<br/>
        /// The path may be either the canonical path or any registered alias.<br/>
        /// </summary>
        /// <param name="propPath">Canonical path or alias identifying the group to remove.<br/></param>
        /// <returns><see langword="true"/> when a group was removed; otherwise <see langword="false"/>.<br/></returns>
        public bool Remove(string propPath)
        {
            string path = Normalize(propPath, nameof(propPath));
            lock (sync)
            {
                if (!snapshot.TryGetValue(path, out Group? group))
                    return false;
                var next = new Dictionary<string, Group>(snapshot, StringComparer.Ordinal);
                for (int index = 0; index < group.Paths.Length; index++)
                    next.Remove(group.Paths[index]);
                snapshot = next;
                return true;
            }
        }

        /// <summary>Removes every registered property-path alias group.<br/></summary>
        public void Clear()
        {
            lock (sync)
                snapshot = new Dictionary<string, Group>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Reports whether the supplied path is an alias-group member or descends from one.<br/>
        /// Descendant matching allows a renamed complex member to carry the same suffix, such as <c>.Address.Zip</c> and <c>.Location.Zip</c>.<br/>
        /// </summary>
        /// <param name="propPath">Property path to inspect.<br/></param>
        /// <returns><see langword="true"/> when alias resolution can affect the path; otherwise <see langword="false"/>.<br/></returns>
        public bool Contains(string propPath)
        {
            if (string.IsNullOrWhiteSpace(propPath))
                return false;
            string path = propPath.Trim();
            if (path[0] != '.')
                path = "." + path;
            return TryGetGroup(path, out _, out _);
        }

        internal bool TryGetGroup(string requestedPath, out Group? group, out string suffix)
        {
            Dictionary<string, Group> current = snapshot;
            string normalized = requestedPath.Trim();
            if (normalized.Length == 0)
            {
                group = null;
                suffix = string.Empty;
                return false;
            }
            if (normalized[0] != '.')
                normalized = "." + normalized;
            string candidate = normalized;
            while (true)
            {
                if (current.TryGetValue(candidate, out group))
                {
                    suffix = normalized[candidate.Length..];
                    return true;
                }
                int dot = candidate.LastIndexOf('.');
                if (dot <= 0)
                    break;
                candidate = candidate[..dot];
            }
            group = null;
            suffix = string.Empty;
            return false;
        }

        internal void CopyTo(PropertyAliasCollection target)
        {
            ArgumentNullException.ThrowIfNull(target);
            lock (sync)
            {
                var groups = snapshot.Values.Distinct().ToArray();
                for (int index = 0; index < groups.Length; index++)
                {
                    string[] aliases = groups[index].Paths[1..];
                    target.Add(groups[index].CanonicalPath, aliases);
                }
            }
        }

        private static string Normalize(string? propPath, string paramName)
        {
            if (string.IsNullOrWhiteSpace(propPath))
                throw new ArgumentException("A property path is required.", paramName);
            string path = propPath.Trim();
            if (path[0] != '.')
                path = "." + path;
            if (path.Contains('(') || path.Contains(')') || path.Contains('!') || path.Contains('#'))
                throw new ArgumentException($"Property alias '{path}' must contain property or field segments only.", paramName);
            string[] segments = path.Split('.', StringSplitOptions.None);
            for (int index = 1; index < segments.Length; index++)
                if (segments[index].Length == 0)
                    throw new ArgumentException($"Property alias '{path}' contains an empty member segment.", paramName);
            return path;
        }

        /// <summary>
        /// Reports whether the immutable read snapshot contains any aliases, without counting groups or allocating an enumerator.<br/>
        /// Used to skip physical-parent projection fallback entirely when no name remapping is configured.<br/>
        /// </summary>
        internal bool HasEntries => snapshot.Count != 0;
    }

    /// <summary>
    /// Configures member selection, aliases, custom value readers, activators and comparers. Finish registration before use; mutable registrations are not synchronized.<br/>
    /// </summary>
    public class DeserializationOptions
    {








        //======  FIELDS  ======
        internal readonly Dictionary<string, object> MemberComparers;
        internal readonly Dictionary<Type, object> TypeComparers;
        internal readonly Dictionary<string, Func<object, object?>> MemberDeserializers;
        internal readonly Dictionary<Type, Func<object, object?>> TypeDeserializerCache;
        internal readonly Dictionary<Type, Func<object, object?>> TypeDeserializers;
        internal IReadOnlySet<string>? ExcludedMembers;
        private readonly object rootMaterializerSync = new object();
        private RootMaterializerDelegate[] rootMaterializers = Array.Empty<RootMaterializerDelegate>();
        private RootOptionsDelegate[] rootOptionPreparers = Array.Empty<RootOptionsDelegate>();



        /// <summary>
        /// Holds the comparer-aware constructor helper bound to this options instance; configure it before materialization begins.<br/>
        /// </summary>
        public ComparerActivator ActivatorWithComparerHelper;

        /// <summary>
        /// Gets a new deserialization options instance on each access; this is not a shared mutable singleton.<br/>
        /// </summary>
        public static DeserializationOptions Default => new DeserializationOptions();


        //======  CONSTRUCTORS  ======
        /// <summary>
        /// Creates independent mutable deserialization registrations, aliases and comparer-aware activation dispatch.<br/>
        /// </summary>
        public DeserializationOptions()
        {
            MemberDeserializers = new Dictionary<string, Func<object, object?>>();
            TypeDeserializers = new Dictionary<Type, Func<object, object?>>();
            TypeDeserializerCache = new Dictionary<Type, Func<object, object?>>();
            MemberComparers = new Dictionary<string, object>();
            TypeComparers = new Dictionary<Type, object>();
            ActivatorWithComparerHelper = new ComparerActivator(this);
            Aliases = new PropertyAliasCollection();
        }

        /// <summary>
        /// Gets or sets which instance members are considered for materialization; the default enum value includes public properties and fields.<br/>
        /// </summary>
        public MemberTypesEnum MemberTypes { get; set; }
        /// <summary>Gets property-path equivalence groups used by direct reads and object materialization.<br/></summary>
        public PropertyAliasCollection Aliases { get; }


        private readonly Dictionary<Type, ActivatorDelegate> activators = new Dictionary<Type, ActivatorDelegate>();

        /// <summary>
        /// Carries the source reader, logical member path and optional stored construction context to a developer activator.<br/>
        /// </summary>
        /// <param name="Binary">Source reader available to the construction callback.<br/></param>
        /// <param name="PropPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
        /// <param name="StoredContext">Optional stored construction context, such as encoded comparer metadata.<br/></param>
        public readonly record struct ActivatorContext(InhetoBinary Binary, string PropPath, object? StoredContext);

        /// <summary>
        /// Represents one owner-scoped operation performed after a root object has been materialized.<br/>
        /// The callback may return a replacement boxed value so value-type projections can be updated without losing their mutations.<br/>
        /// </summary>
        /// <param name="binary">Source binary that produced the value.<br/></param>
        /// <param name="value">Materialized root value, which may be null.<br/></param>
        /// <param name="requestedType">CLR shape requested by the caller.<br/></param>
        /// <param name="context">Optional owner-supplied payload context.<br/></param>
        /// <param name="effectiveOptions">Options governing this materialization, including temporary exclusions.<br/></param>
        /// <returns>The original or replacement boxed root value.<br/></returns>
        internal delegate object? RootMaterializerDelegate(
            InhetoBinary binary,
            object? value,
            Type requestedType,
            object? context,
            DeserializationOptions effectiveOptions);

        /// <summary>
        /// Prepares effective options before one root value is constructed.<br/>
        /// Owners use this boundary to suppress members that must be assigned after the requested projection shape is known.<br/>
        /// </summary>
        internal delegate DeserializationOptions RootOptionsDelegate(
            Type requestedType,
            object? context,
            DeserializationOptions effectiveOptions);

        /// <summary>
        /// Delegate used to create an instance of a type.<br/>
        /// Provides activation context (binary instance, prop path, optional stored context such as a comparer).<br/>
        /// A null result declines creation: ordinary complex-object materialization leaves that destination null
        /// and skips its descendants instead of attempting default construction.<br/>
        /// This is an activation decision, not a filter on already initialized or reused shared instances.<br/>
        /// </summary>
        public delegate object? ActivatorDelegate(ActivatorContext context);


        //======  METHODS  ======

        /// <summary>
        /// Adds a special deserialization method for a given property of the stored object.<br/>
        /// The provided function is used to convert the stored value from a <c>byte[]</c> back to the object's native value type.<br/>
        /// If a serialization function is used it is common for a deserialization function to also be used.
        /// </summary>
        public void AddDeserializer<ValueType>(string propPath, DeserializerDelegate<ValueType> deserializer)
        {
            MemberDeserializers[propPath] = o => deserializer((byte[])o);
        }

        /// <summary>
        /// Registers or replaces the activator for one exact CLR type. A callback returning null intentionally suppresses that materialized instance rather than requesting default construction.<br/>
        /// </summary>
        /// <typeparam name="ValueType">Exact CLR type targeted by this registration or lookup.<br/></typeparam>
        /// <param name="activator">Construction callback to register, or the callback received from a successful lookup; null callback results suppress an instance.<br/></param>
        public void AddActivator<ValueType>(ActivatorDelegate activator)
        {
            AddActivator(typeof(ValueType), activator);
        }

        /// <summary>
        /// Registers or replaces the activator for one exact CLR type. A callback returning null intentionally suppresses that materialized instance rather than requesting default construction.<br/>
        /// </summary>
        /// <param name="type">Exact CLR type used for registration, lookup or runtime shape analysis.<br/></param>
        /// <param name="activator">Construction callback to register, or the callback received from a successful lookup; null callback results suppress an instance.<br/></param>
        public void AddActivator(Type type, ActivatorDelegate activator)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(activator);
            activators[type] = activator;
        }

        /// <summary>
        /// Removes the activator registration for one exact CLR type; does not instantiate or traverse any objects.<br/>
        /// </summary>
        /// <typeparam name="ValueType">Exact CLR type targeted by this registration or lookup.<br/></typeparam>
        /// <returns>True when a registration was removed; false when none existed.<br/></returns>
        public bool RemoveActivator<ValueType>()
        {
            return RemoveActivator(typeof(ValueType));
        }

        /// <summary>
        /// Removes the activator registration for one exact CLR type; does not instantiate or traverse any objects.<br/>
        /// </summary>
        /// <param name="type">Exact CLR type used for registration, lookup or runtime shape analysis.<br/></param>
        /// <returns>True when a registration was removed; false when none existed.<br/></returns>
        public bool RemoveActivator(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);
            return activators.Remove(type);
        }

        /// <summary>
        /// Tests whether an activator is registered for the exact CLR type.<br/>
        /// </summary>
        /// <param name="type">Exact CLR type used for registration, lookup or runtime shape analysis.<br/></param>
        /// <returns>True when the documented predicate is satisfied; otherwise false.<br/></returns>
        public bool HasActivator(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);
            return activators.ContainsKey(type);
        }

        /// <summary>
        /// Looks up the activator registered for the exact CLR type without invoking it.<br/>
        /// </summary>
        /// <param name="type">Exact CLR type used for registration, lookup or runtime shape analysis.<br/></param>
        /// <param name="activator">Construction callback to register, or the callback received from a successful lookup; null callback results suppress an instance.<br/></param>
        /// <returns>True when an exact-type registration exists; otherwise false.<br/></returns>
        public bool TryGetActivator(Type type, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out ActivatorDelegate? activator)
        {
            ArgumentNullException.ThrowIfNull(type);
            return activators.TryGetValue(type, out activator);
        }

        /// <summary>
        /// Adds one owner-scoped root materializer without replacing existing registrations.<br/>
        /// Registrations are expected to be rare; reads use an immutable snapshot and do not acquire this lock.<br/>
        /// </summary>
        internal void AddRootMaterializer(RootMaterializerDelegate materializer)
        {
            ArgumentNullException.ThrowIfNull(materializer);
            lock (rootMaterializerSync)
            {
                for (int index = 0; index < rootMaterializers.Length; index++)
                    if (rootMaterializers[index] == materializer)
                        return;
                var next = new RootMaterializerDelegate[rootMaterializers.Length + 1];
                Array.Copy(rootMaterializers, next, rootMaterializers.Length);
                next[^1] = materializer;
                rootMaterializers = next;
            }
        }

        /// <summary>
        /// Removes one previously registered owner-scoped root materializer.<br/>
        /// A missing registration is a no-op.<br/>
        /// </summary>
        internal void RemoveRootMaterializer(RootMaterializerDelegate materializer)
        {
            ArgumentNullException.ThrowIfNull(materializer);
            lock (rootMaterializerSync)
            {
                int found = Array.IndexOf(rootMaterializers, materializer);
                if (found < 0)
                    return;
                var next = new RootMaterializerDelegate[rootMaterializers.Length - 1];
                if (found != 0)
                    Array.Copy(rootMaterializers, 0, next, 0, found);
                if (found != rootMaterializers.Length - 1)
                    Array.Copy(rootMaterializers, found + 1, next, found, rootMaterializers.Length - found - 1);
                rootMaterializers = next;
            }
        }

        /// <summary>Adds one owner-scoped root-options preparer.<br/></summary>
        internal void AddRootOptionsPreparer(RootOptionsDelegate preparer)
        {
            ArgumentNullException.ThrowIfNull(preparer);
            lock (rootMaterializerSync)
            {
                for (int index = 0; index < rootOptionPreparers.Length; index++)
                    if (rootOptionPreparers[index] == preparer)
                        return;
                var next = new RootOptionsDelegate[rootOptionPreparers.Length + 1];
                Array.Copy(rootOptionPreparers, next, rootOptionPreparers.Length);
                next[^1] = preparer;
                rootOptionPreparers = next;
            }
        }

        /// <summary>Removes one owner-scoped root-options preparer.<br/></summary>
        internal void RemoveRootOptionsPreparer(RootOptionsDelegate preparer)
        {
            ArgumentNullException.ThrowIfNull(preparer);
            lock (rootMaterializerSync)
            {
                int found = Array.IndexOf(rootOptionPreparers, preparer);
                if (found < 0)
                    return;
                var next = new RootOptionsDelegate[rootOptionPreparers.Length - 1];
                if (found != 0)
                    Array.Copy(rootOptionPreparers, 0, next, 0, found);
                if (found != rootOptionPreparers.Length - 1)
                    Array.Copy(rootOptionPreparers, found + 1, next, found, rootOptionPreparers.Length - found - 1);
                rootOptionPreparers = next;
            }
        }

        /// <summary>Applies the immutable root-options snapshot before root-object construction begins.<br/></summary>
        internal DeserializationOptions PrepareRootOptions(
            Type requestedType,
            object? context,
            DeserializationOptions effectiveOptions)
        {
            RootOptionsDelegate[] snapshot = rootOptionPreparers;
            for (int index = 0; index < snapshot.Length; index++)
                effectiveOptions = snapshot[index](requestedType, context, effectiveOptions);
            return effectiveOptions;
        }

        /// <summary>
        /// Applies the current immutable root-materializer snapshot in registration order.<br/>
        /// </summary>
        internal object? ApplyRootMaterializers(
            InhetoBinary binary,
            object? value,
            Type requestedType,
            object? context,
            DeserializationOptions effectiveOptions)
        {
            RootMaterializerDelegate[] snapshot = rootMaterializers;
            for (int index = 0; index < snapshot.Length; index++)
                value = snapshot[index](binary, value, requestedType, context, effectiveOptions);
            return value;
        }

        /// <summary>
        /// Registers or replaces a comparer for one logical member path; the comparer is supplied to compatible collection construction.<br/>
        /// </summary>
        /// <typeparam name="T">CLR type used by this operation; result conversion or exact-type registration follows the documented contract.<br/></typeparam>
        /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
        /// <param name="comparer">Comparer instance supplied to compatible collection construction; no clone is made.<br/></param>
        public void AddMemberComparer<T>(string propPath, IComparer<T> comparer)
        {
            MemberComparers[propPath] = comparer;
        }

        /// <summary>
        /// Registers or replaces a comparer for one exact collection-consumer CLR type; the element type determines the comparer contract.<br/>
        /// </summary>
        /// <typeparam name="TCompareConsumer">Exact collection-consumer CLR type receiving the comparer.<br/></typeparam>
        /// <typeparam name="TCompareElement">Element or key type governed by the comparer.<br/></typeparam>
        /// <param name="comparer">Comparer instance supplied to compatible collection construction; no clone is made.<br/></param>
        public void AddTypeComparer<TCompareConsumer, TCompareElement>(IComparer<TCompareElement> comparer)
        {
            TypeComparers[typeof(TCompareConsumer)] = comparer;
        }

        /// <summary>
        /// Adds a special deserialization method for a given Type found in the stored object.<br/>
        /// The provided function is used to convert instances of <typeparamref name="ValueType"/> from a <c>byte[]</c> back to the object's native value type.<br/>
        /// If a serialization function is used it is common for a deserialization function to also be used.
        /// </summary>
        /// <remarks>This deserializer is only used if there's not a property or field specific deserializer.</remarks>
        public void AddTypeDeserializer<ValueType>(DeserializerDelegate<ValueType> deserializer)
        {
            TypeDeserializers[typeof(ValueType)] = o => deserializer((byte[])o);
        }

        /// <summary>
        /// Determines whether a custom binary deserializer is registered for an exact property path or CLR value type.<br/>
        /// Property-path handlers retain the same precedence used during normal Inheto deserialization, followed by the exact type-wide handler.<br/>
        /// This method performs registration discovery only; it does not invoke developer code or allocate a converted value.<br/>
        /// </summary>
        /// <param name="valueType">Exact CLR value type requested by the consumer.<br/></param>
        /// <param name="propPath">Optional canonical property path whose member-specific registration should be preferred.<br/></param>
        /// <returns><see langword="true"/> when a compatible custom binary deserializer is registered; otherwise <see langword="false"/>.<br/></returns>
        public bool HasBinaryDeserializer(Type valueType, string? propPath = null)
        {
            ArgumentNullException.ThrowIfNull(valueType);
            if (!string.IsNullOrEmpty(propPath) && MemberDeserializers.ContainsKey(propPath))
                return true;
            return TypeDeserializers.ContainsKey(valueType);
        }

        /// <summary>
        /// Determines whether a custom binary deserializer is registered for <typeparamref name="ValueType"/> at an optional exact property path.<br/>
        /// The check mirrors Inheto's member-before-type resolution and never invokes the registered handler.<br/>
        /// </summary>
        /// <typeparam name="ValueType">Exact CLR value type requested by the consumer.<br/></typeparam>
        /// <param name="propPath">Optional canonical property path whose member-specific registration should be preferred.<br/></param>
        /// <returns><see langword="true"/> when a compatible custom binary deserializer is registered; otherwise <see langword="false"/>.<br/></returns>
        public bool HasBinaryDeserializer<ValueType>(string? propPath = null)
            => HasBinaryDeserializer(typeof(ValueType), propPath);

        /// <summary>
        /// Reads one custom binary payload directly from a borrowed span. Copy data before retaining it beyond the callback.<br/>
        /// </summary>
        /// <typeparam name="ValType">CLR value type produced by the developer deserializer.<br/></typeparam>
        /// <param name="data">Complete byte payload supplied to the helper; a span argument is borrowed only for the callback duration.<br/></param>
        /// <returns>Value decoded by the developer callback from this custom payload.<br/></returns>
        public delegate ValType DeserializerDelegate<ValType>(ReadOnlySpan<byte> data);
        /// <summary>
        /// Adds a special deserialization method for a given Type found in the stored object, optionally including its base types.<br/>
        /// The provided function is used to convert instances of <typeparamref name="ValueType"/> or any of its base types from a <c>byte[]</c> back to the object's native value type.<br/>
        /// If a serialization function is used it is common for a deserialization function to also be used.
        /// </summary>
        /// <remarks>This deserializer is only used if there's not a property or field specific deserializer.</remarks>
        public void AddTypeDeserializer<ValueType>(bool includeBaseTypes, DeserializerDelegate<ValueType> deserializer)
        {
            if (includeBaseTypes)
            {
                TypeDeserializers[typeof(ValueType)] = o => deserializer((byte[])o);
                var baseType = typeof(ValueType).BaseType;
                while (baseType is not null && baseType != typeof(object))
                {
                    TypeDeserializers[baseType] = o => deserializer((byte[])o);
                    baseType = baseType.BaseType;
                }
            }
            else
            {
                TypeDeserializers[typeof(ValueType)] = o => deserializer((byte[])o);
            }
        }

        /// <summary>
        /// Removes a member-path custom deserializer registration; an absent registration is a no-op.<br/>
        /// </summary>
        /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
        public void RemoveDeserializer(string propPath)
        {
            MemberDeserializers.Remove(propPath);
        }

        /// <summary>
        /// Removes an exact-type custom deserializer registration; an absent registration is a no-op.<br/>
        /// </summary>
        /// <param name="type">Exact CLR type used for registration, lookup or runtime shape analysis.<br/></param>
        public void RemoveTypeDeserializer(Type type)
        {
            TypeDeserializers.Remove(type);
        }

        /// <summary>
        /// Removes a member-path comparer registration; an absent registration is a no-op.<br/>
        /// </summary>
        /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
        public void RemoveMemberComparer(string propPath)
        {
            MemberComparers.Remove(propPath);
        }

        /// <summary>
        /// Removes an exact-type comparer registration; an absent registration is a no-op.<br/>
        /// </summary>
        /// <param name="type">Exact CLR type used for registration, lookup or runtime shape analysis.<br/></param>
        public void RemoveTypeComparer(Type type)
        {
            TypeComparers.Remove(type);
        }

        /// <summary>
        /// Removes an exact-type comparer registration; an absent registration is a no-op.<br/>
        /// </summary>
        /// <typeparam name="T">CLR type used by this operation; result conversion or exact-type registration follows the documented contract.<br/></typeparam>
        public void RemoveTypeComparer<T>()
        {
            TypeComparers.Remove(typeof(T));
        }

        internal DeserializationOptions ForkExcluding(IReadOnlySet<string> excludedMembers)
        {
            ArgumentNullException.ThrowIfNull(excludedMembers);
            var ret = new DeserializationOptions
            {
                MemberTypes = MemberTypes,
                AllowStoredTypeActivation = AllowStoredTypeActivation,
                ExcludedMembers = excludedMembers
            };
            foreach (var entry in MemberDeserializers)
                ret.MemberDeserializers.Add(entry.Key, entry.Value);
            foreach (var entry in TypeDeserializers)
                ret.TypeDeserializers.Add(entry.Key, entry.Value);
            foreach (var entry in TypeDeserializerCache)
                ret.TypeDeserializerCache.Add(entry.Key, entry.Value);
            foreach (var entry in MemberComparers)
                ret.MemberComparers.Add(entry.Key, entry.Value);
            foreach (var entry in TypeComparers)
                ret.TypeComparers.Add(entry.Key, entry.Value);
            foreach (var entry in activators)
                ret.activators.Add(entry.Key, entry.Value);
            Aliases.CopyTo(ret.Aliases);
            ret.rootMaterializers = rootMaterializers;
            ret.rootOptionPreparers = rootOptionPreparers;
            if (allowedStoredTypes is not null)
                ret.allowedStoredTypes = new Dictionary<string, Type>(allowedStoredTypes, StringComparer.Ordinal);
            return ret;
        }
        private Dictionary<string, Type>? allowedStoredTypes;

        /// <summary>
        /// Gets or sets whether zero-input/singleton representations may resolve and construct their stored CLR type automatically; defaults to true for compatibility.<br/>
        /// Set false to use a concrete caller-selected destination, or explicitly registered types for object/interface/native zero-input reads.<br/>
        /// This controls special stored-type construction only, not ordinary shape reconstruction, System.Type metadata values, registered codecs or CLR continuation expressions.<br/>
        /// Finish configuring options before use; this is not a universal validation mode for arbitrary binary input.<br/>
        /// </summary>
        public bool AllowStoredTypeActivation { get; set; } = true;

        /// <summary>
        /// Approves one exact stored zero-input CLR identity when automatic stored-type activation is disabled.<br/>
        /// The Type is retained directly; reading its registered name does not require unrestricted Type.GetType resolution.<br/>
        /// An approval does not install a constructor or codec; existing explicit activators and intrinsic construction rules still apply.<br/>
        /// </summary>
        /// <typeparam name="T">Exact CLR type whose assembly-qualified name is approved for exceptional stored-type construction.<br/></typeparam>
        public void AllowStoredType<T>() => AllowStoredType(typeof(T));

        /// <summary>
        /// Approves the supplied CLR Type under its exact assembly-qualified name for exceptional stored-type construction.<br/>
        /// Allocates the approval map only on its first registration; configure it before using these mutable options.<br/>
        /// Approval is used when AllowStoredTypeActivation is false, and never changes writer member/type exclusions.<br/>
        /// </summary>
        /// <param name="type">Application-selected CLR Type with a non-null assembly-qualified name.<br/></param>
        /// <exception cref="ArgumentNullException">The Type is null.<br/></exception>
        /// <exception cref="ArgumentException">The Type has no assembly-qualified name.<br/></exception>
        public void AllowStoredType(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);
            string name = type.AssemblyQualifiedName ?? throw new ArgumentException("A stored type requires an assembly-qualified name.", nameof(type));
            (allowedStoredTypes ??= new Dictionary<string, Type>(StringComparer.Ordinal))[name] = type;
        }

        /// <summary>
        /// Removes an exact stored-type approval; a missing registration returns false without allocating a map.<br/>
        /// Does not remove a custom activator, or affect the compatible default while automatic stored-type activation is enabled.<br/>
        /// </summary>
        /// <typeparam name="T">Exact CLR Type whose assembly-qualified approval is removed.<br/></typeparam>
        /// <returns>True when an explicit approval was removed; otherwise false.<br/></returns>
        public bool RemoveAllowedStoredType<T>() => allowedStoredTypes?.Remove(typeof(T).AssemblyQualifiedName!) == true;

        /// <summary>
        /// Resolves authority for one special zero-input representation before any constructor/factory or native cache result is used.<br/>
        /// Compatible mode retains stored-name resolution. Opt-in mode first recognizes an explicit approval, otherwise uses a concrete requested destination without resolving the historical CLR identity.<br/>
        /// Untyped and abstract/interface destinations need an exact approval; ordinary scalar/object/collection navigation never calls this helper.<br/>
        /// </summary>
        /// <param name="storedName">Assembly-qualified identity read as data from the validated representation.<br/></param>
        /// <param name="requestedType">Caller-selected concrete destination, or null/object for a native/untyped read.<br/></param>
        /// <param name="propPath">Logical path included in a rejected-construction diagnostic.<br/></param>
        /// <param name="ignoreCase">Existing stored-name resolution convention used only in compatible mode.<br/></param>
        /// <returns>Resolved/approved destination Type, or null for an unresolved name in compatible mode.<br/></returns>
        /// <exception cref="NotSupportedException">Opt-in mode has no approved construction target, or the approved type is incompatible with the requested destination.<br/></exception>
        internal Type? ResolveStoredActivationType(string storedName, Type? requestedType, string propPath, bool ignoreCase = true)
        {
            if (AllowStoredTypeActivation) return Type.GetType(storedName, false, ignoreCase);
            if (allowedStoredTypes?.TryGetValue(storedName, out var approved) == true)
            {
                if (requestedType is not null && requestedType != typeof(object) && !requestedType.IsAssignableFrom(approved))
                    throw new NotSupportedException($"Approved stored type '{approved}' cannot reconstruct '{requestedType}' at '{propPath}'.");
                return approved;
            }
            if (requestedType is not null && requestedType != typeof(object) && !requestedType.IsAbstract && !requestedType.IsInterface)
                return requestedType;
            throw new NotSupportedException($"Automatic stored-type construction is disabled at '{propPath}'. Request a concrete destination or approve the stored type with AllowStoredType<T>().");
        }
    }

    /// <summary>
    /// Provides text-facing custom decoding without changing overload resolution for existing byte-span callbacks.<br/>
    /// </summary>
    public static class DeserializationOptionsTextExtensions
    {
        /// <summary>
        /// Registers a property-path decoder that receives UTF-8 text from the ordinary custom byte payload.<br/>
        /// Use an explicitly typed string lambda, such as (string text) =&gt; Parse(text), or a Func&lt;string, T&gt;.<br/>
        /// Existing byte-span instance overloads keep precedence for callbacks that can bind to both forms.<br/>
        /// Null markers bypass decoding; an empty payload supplies String.Empty. Standard UTF-8 replacement fallback applies.<br/>
        /// Registration creates one adapter; decoding creates the requested string without an additional byte-array copy.<br/>
        /// </summary>
        /// <typeparam name="ValueType">The developer-selected decoded value type.<br/></typeparam>
        /// <param name="options">Options receiving the registration, including normal replacement/removal behavior.<br/></param>
        /// <param name="propPath">The exact Inheto property path, or an empty string for the root.<br/></param>
        /// <param name="deserializer">Callback converting decoded text to the destination value.<br/></param>
        public static void AddDeserializer<ValueType>(this DeserializationOptions options, string propPath, Func<string, ValueType> deserializer)
        {
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(deserializer);
            options.MemberDeserializers[propPath] = payload => deserializer(System.Text.Encoding.UTF8.GetString((byte[])payload));
        }
    }
}
