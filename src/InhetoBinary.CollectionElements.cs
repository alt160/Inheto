using System.Collections.Concurrent;
using System.Reflection;

namespace Inheto;

public partial class InhetoBinary
{
    private static readonly ConcurrentDictionary<Type, Action<InhetoBinary, object, int, string, DeserializationOptions>> collectionElementFillers = new();

    /// <summary>
    /// Fills a complex-shape collection using its declared element type and complete encoded child values.<br/>
    /// Binds one closed typed loop per element type; no reflection or delegate lookup occurs inside that loop.<br/>
    /// Existing scalar, array-element, comparer and destination-construction paths remain with their callers.<br/>
    /// </summary>
    /// <param name="elementType">Declared destination element type, including nullable value types.<br/></param>
    /// <param name="destination">Already constructed destination; existing members and comparer are retained.<br/></param>
    /// <param name="count">Number of encoded collection slots.<br/></param>
    /// <param name="path">Resolved collection path used to address each child.<br/></param>
    /// <param name="options">Current operation's activation and codec options.<br/></param>
    private void FillCollectionElements(Type elementType, object destination, uint count, string path, DeserializationOptions options)
    {
        var fill = collectionElementFillers.GetOrAdd(elementType, static type =>
            typeof(InhetoBinary).GetMethod(nameof(FillTypedCollectionElements), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(type)
                .CreateDelegate<Action<InhetoBinary, object, int, string, DeserializationOptions>>());
        fill(this, destination, checked((int)count), path, options);
    }

    /// <summary>
    /// Adds materialized values instead of discarding replacement results from codecs or value-type readers.<br/>
    /// Reuses nullable-element reading and ordinary typed ReadPropAs activation/cache semantics.<br/>
    /// Typed ICollection additions avoid boxing value elements; other families retain their established mutator.<br/>
    /// Explicit nulls occupy a slot; missing encoded children throw rather than silently shortening the result.<br/>
    /// Stack families visit encoded slots in reverse so pushing retains the serialized top-to-bottom order without a temporary buffer.<br/>
    /// </summary>
    /// <typeparam name="T">Declared closed destination element type.<br/></typeparam>
    /// <param name="reader">Current reader, with operation-local caches and payload.<br/></param>
    /// <param name="destination">Collection being filled; no replacement or deep clone is introduced.<br/></param>
    /// <param name="count">Validated encoded slot count.<br/></param>
    /// <param name="path">Resolved collection path.<br/></param>
    /// <param name="options">Current codec and activator policy.<br/></param>
    private static void FillTypedCollectionElements<T>(InhetoBinary reader, object destination, int count, string path, DeserializationOptions options)
    {
        var collection = destination as ICollection<T>;
        EnumerableMutator.Mutator? mutator = null;
        if (collection is null && !EnumerableMutator.TryGetMutator(destination, out mutator))
            throw new InvalidOperationException($"No element mutator is available for '{destination.GetType()}'.");
        bool reverse = destination is Stack<T> or ConcurrentStack<T>;
        for (int slot = 0; slot < count; slot++)
        {
            int i = reverse ? count - 1 - slot : slot;
            // This shared reader already distinguishes nullable underlying types, custom nodes and explicit nulls.
            T value = reader.ReadNullableElement<T>($"{path}#{i}", options, collectionText: true);
            if (collection is not null) collection.Add(value);
            else mutator!.Add(destination, value);
        }
    }

    /// <summary>
    /// Recognizes existing concrete list-like destination metadata for the generic-enumerable wire descriptor.<br/>
    /// Dictionary, arbitrary interface, custom unknown collection and array destination contracts are not broadened here.<br/>
    /// List of string has an intrinsic marker rather than collection metadata and is handled explicitly.<br/>
    /// The lookup uses the existing immutable type cache and runs only on generic-enumerable source paths.<br/>
    /// </summary>
    /// <param name="destinationType">Declared CLR destination requested by a typed read or member plan.<br/></param>
    /// <param name="elementType">Declared element type when this destination is supported; otherwise null.<br/></param>
    /// <returns>True for a recognized list-like destination, leaving its mutability and activation checks to reconstruction.<br/></returns>
    private static bool TryGetEnumerableCollectionElementType(Type destinationType,
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Type? elementType)
    {
        if (destinationType == typeof(List<string>)) { elementType = typeof(string); return true; }
        var info = InhetoTypeResolver.GetInhetoTypeInfo(destinationType);
        elementType = info.Kind == InhetoKindRoute.Collection && info.KeyType is null ? info.ElementType : null;
        return elementType is not null;
    }

    /// <summary>
    /// Reconstructs a known mutable CLR collection from count and complete numbered Enumerable child headers.<br/>
    /// Reuses operation-local identity, cached activation, declared-member plans, typed element fillers and mutation delegates.<br/>
    /// Publishes the actual container before traversing members or elements so owner/self references do not reconstruct it twice.<br/>
    /// Distinct supplied or incompatible projections temporarily replace the publication and restore the prior referent in finally.<br/>
    /// Intentional null activation is retained for later missing aliases; failed new fills remove their publication.<br/>
    /// Object elements retain navigable Inheto wrapper semantics; typed elements retain nullable, enum, array and codec readers.<br/>
    /// Ordinary paths reuse existing fillers; a weak-keyed typed binding is used only when logical and storage prefixes differ.<br/>
    /// No staging buffer, heap operation frame or wire-format reinterpretation is introduced.<br/>
    /// </summary>
    /// <param name="destinationType">Declared known list-like CLR destination type.<br/></param>
    /// <param name="elementType">Declared element type obtained from existing destination metadata.<br/></param>
    /// <param name="destination">Developer-owned destination, or null to activate a new one.<br/></param>
    /// <param name="entry">Resolved Enumerable header carrying physical reference identity and element count.<br/></param>
    /// <param name="resolvedPath">Existing resolved source path used for activation context.<br/></param>
    /// <param name="requestedPath">Logical requested prefix retained for alias and custom-child decoder precedence.<br/></param>
    /// <param name="options">Current member, activation, alias and codec policy; never retained in static dispatch state.<br/></param>
    /// <returns>The exact operation/supplied referent, intentional null, or established standalone mutable copy.<br/></returns>
    private object? ReadEnumerableCollection(Type destinationType, Type elementType, object? destination,
        NameHeaderEntry entry, string resolvedPath, string requestedPath, DeserializationOptions options)
    {
        bool operation = materializationOperationDepth != 0;
        object? previous = null;
        bool hadPrevious = refCache is not null && refCache.TryGetValue(entry.Index, out previous);
        if (operation && hadPrevious && previous is null && destination is null) return null;
        if (hadPrevious && previous is not null && destinationType.IsInstanceOfType(previous) &&
            (destination is null || ReferenceEquals(destination, previous)))
            return operation ? previous : CloneIfMutable(previous);
        try
        {
            if (destination is null)
            {
                var activate = options.ActivatorWithComparerHelper.GetActivatorDelegate(destinationType);
                destination = activate(CreateActivatorContext(resolvedPath));
            }
            if (destination is null)
            {
                if (operation) RefCache[entry.Index] = null!;
                return null;
            }
            if (!destinationType.IsInstanceOfType(destination))
                throw new InvalidCastException($"Activator returned '{destination.GetType()}' instead of '{destinationType}'.");
            if (!EnumerableMutator.TryGetMutator(destination, out var mutator))
                throw new InvalidOperationException($"No element mutator is available for '{destination.GetType()}'.");
            if (operation) RefCache[entry.Index] = destination;
            if (destination.GetType().BaseType != typeof(object))
                destination = doPropsAndFields(options, requestedPath, ref destination, true, true, entry.Index);
            if (!StringComparer.Ordinal.Equals(requestedPath, resolvedPath))
            {
                var fill = aliasedEnumerableFillers.GetValue(elementType, static type =>
                    typeof(InhetoBinary).GetMethod(nameof(FillAliasedEnumerableElements), BindingFlags.NonPublic | BindingFlags.Static)!
                        .MakeGenericMethod(type)
                        .CreateDelegate<Action<InhetoBinary, object, int, string, string, DeserializationOptions>>());
                fill(this, destination, checked((int)entry.ValueOffset), resolvedPath, requestedPath, options);
            }
            else if (elementType == typeof(object))
            {
                int count = checked((int)entry.ValueOffset);
                for (int slot = 0; slot < count; slot++)
                {
                    int i = mutator.ReverseEncodedOrder ? count - 1 - slot : slot;
                    mutator.Add(destination, ReadPropInternal($"{requestedPath}#{i}", options));
                }
            }
            else FillCollectionElements(elementType, destination, entry.ValueOffset, requestedPath, options);
            if (operation) return destination;
            var stored = CloneIfMutable(destination);
            RefCache[entry.Index] = stored;
            return CloneIfMutable(stored);
        }
        catch
        {
            if (operation && !hadPrevious && refCache is not null) refCache.Remove(entry.Index);
            throw;
        }
        finally
        {
            if (operation && hadPrevious) RefCache[entry.Index] = previous!;
        }
    }

    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Type, Action<InhetoBinary, object, int, string, string, DeserializationOptions>> aliasedEnumerableFillers = new();

    /// <summary>
    /// Fills an Enumerable projection whose logical requested prefix differs from its physical storage prefix.<br/>
    /// Resolves each persisted child from storage while passing its requested name to the existing custom-value core.<br/>
    /// Ordinary typed values reuse the established nullable/enum/array/text reader; object values retain navigable wrappers.<br/>
    /// The binding is created once per weak-keyed element type and reflection/delegate lookup stays outside the loop.<br/>
    /// This uncommon path does not alter global alias parsing or add branches to native collection element loops.<br/>
    /// </summary>
    /// <typeparam name="T">Declared destination element type.<br/></typeparam>
    /// <param name="reader">Current reader and operation-local identity map.<br/></param>
    /// <param name="destination">Already activated and published developer/operation-owned destination.<br/></param>
    /// <param name="count">Validated persisted slot count.<br/></param>
    /// <param name="storagePath">Physical resolved collection prefix used for child navigation.<br/></param>
    /// <param name="logicalPath">Caller-requested collection prefix used for unshared decoder precedence.<br/></param>
    /// <param name="options">Current aliases, activators and custom decoders.<br/></param>
    private static void FillAliasedEnumerableElements<T>(InhetoBinary reader, object destination, int count,
        string storagePath, string logicalPath, DeserializationOptions options)
    {
        var collection = destination as ICollection<T>;
        EnumerableMutator.Mutator? mutator = null;
        if (collection is null && !EnumerableMutator.TryGetMutator(destination, out mutator))
            throw new InvalidOperationException($"No element mutator is available for '{destination.GetType()}'.");
        bool reverse = destination is Stack<T> or ConcurrentStack<T>;
        for (int slot = 0; slot < count; slot++)
        {
            int i = reverse ? count - 1 - slot : slot;
            string childPath = $"{storagePath}#{i}";
            var child = reader.GetNameEntryFromPropPath(childPath, options, out var resolved, cacheResolution: false);
            if (child.IsEmpty) throw new InvalidDataException($"Missing collection or array element at '{childPath}'.");
            var terminal = child;
            while (terminal.Kind == InhetoKindMasks.InhetoBaseType && terminal.Type.BaseType is InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
                terminal = NameHeaderEntry.CreateBorrowed(reader.stream, terminal.ValueOffset);
            T value;
            if (terminal.Kind == InhetoKindMasks.InhetoBaseType && terminal.Type.BaseType is InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType)
                value = reader.ReadMaterializedAs<T>(default, child, resolved, options, true, $"{logicalPath}#{i}")!;
            else if (typeof(T) == typeof(object)) value = (T)reader.ReadPropInternal(resolved, options)!;
            else value = reader.ReadNullableElement<T>(resolved, options, collectionText: true);
            if (collection is not null) collection.Add(value);
            else mutator!.Add(destination, value);
        }
    }
}
