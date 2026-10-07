using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Fasterflect;

/// <summary>
/// Resolves and caches a single-element mutation delegate for a runtime collection type. LinkedList families bind AddLast(T) to preserve insertion order.<br/>
/// </summary>
public static class EnumerableMutator
{
    private static readonly Mutator MissingMutator = new(static (_, _) =>
        throw new InvalidOperationException("A missing enumerable mutator cannot add values."), typeof(object));
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, Mutator> _cache = new();

    // Default pattern: names starting with Add or Insert, optional suffix
    private static Regex _methodPattern = new(@"^(Add|Insert)\w*$", RegexOptions.Compiled);

    /// <summary>
    /// Gets or sets the regex pattern used to match candidate Add/Insert methods.<br/>
    /// Default: ^(Add|Insert)\w*$
    /// </summary>
    public static Regex MethodPattern
    {
        get => _methodPattern;
        set => _methodPattern = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Finds or caches an insertion delegate for the target runtime type. Returns false for a null target or a type without a supported mutation method; changing MethodPattern does not invalidate cached resolutions.<br/>
    /// </summary>
    /// <param name="target">Collection instance whose runtime type selects the cached mutation method.<br/></param>
    /// <param name="mutator">Receives the cached mutation helper on success, or null when no supported method exists.<br/></param>
    /// <returns>True when a supported cached mutation helper is available; otherwise false.<br/></returns>
    public static bool TryGetMutator(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] object? target,
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Mutator? mutator)
    {
        mutator = null;
        if (target == null)
            return false;

        var type = target.GetType();
        Mutator cached = _cache.GetOrAdd(type.TypeHandle, static h =>
        {
            var t = Type.GetTypeFromHandle(h)!;
            var flags = Fasterflect.Flags.InstancePublic;

            MethodInfo? method = null;
            Type? argType = null;

            // LinkedList<T>: prefer AddLast(T), fallback to AddFirst(T)
            if (method is null)
            {
                // Reflection order is not name priority, and AddLast also accepts LinkedListNode<T>.
                // Bind the declared element overload once for actual LinkedList families.
                for (Type? linkedType = t; linkedType is not null; linkedType = linkedType.BaseType)
                {
                    if (!linkedType.IsGenericType || linkedType.GetGenericTypeDefinition() != typeof(LinkedList<>)) continue;
                    argType = linkedType.GetGenericArguments()[0];
                    method = t.GetMethod("AddLast", BindingFlags.Instance | BindingFlags.Public, null, new[] { argType }, null);
                    break;
                }
                var linkedAdd = method ?? t.Methods(flags, "AddLast", "AddFirst").FirstOrDefault();

                if (linkedAdd != null)
                {
                    method = linkedAdd;
                    argType = linkedAdd.Parameters()[0].ParameterType;
                }
            }

            if (method is null)
            {
                // ICollection<T>
                var icoll = t.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));
                if (icoll != null)
                {
                    method = icoll.Method("Add", flags);
                    argType = icoll.GetGenericArguments()[0];
                }
            }

            // Non-generic IList
            if (method is null && typeof(IList).IsAssignableFrom(t))
            {
                method = typeof(IList).Method("Add", flags);
                argType = typeof(object);
            }

            // Insert(T, int)
            if (method is null)
            {
                var insert = t.Methods(flags, "Insert")
                    .FirstOrDefault(m => m.Parameters().Count == 1);
                if (insert != null)
                {
                    method = insert;
                    argType = insert.Parameters()[0].ParameterType;
                }
            }

            // Add(T)
            if (method is null)
            {
                var add = t.Methods(flags, "Add")
                    .FirstOrDefault(m => m.Parameters().Count == 1);
                if (add != null)
                {
                    method = add;
                    argType = add.GetParameters()[0].ParameterType;
                }
            }
            if (method is null)
            {
                var add = t.Methods(flags, "Enqueue", "Push")
                    .FirstOrDefault(m =>  m.Parameters().Count == 1);
                if (add != null)
                {
                    method = add;
                    argType = add.Parameters()[0].ParameterType;
                }
            }

            // Pattern-based fallback (Add*, Insert*, single-parameter only)
            if (method is null)
            {
                var match = t.Methods(flags)
                    .FirstOrDefault(m =>
                        _methodPattern.IsMatch(m.Name) &&
                        m.Parameters().Count == 1);
                if (match != null)
                {
                    method = match;
                    argType = match.Parameters()[0].ParameterType;
                }
            }

            if (method is null || argType == null)
                return MissingMutator;

            return BuildMutator(t, method, argType);
        });

        if (ReferenceEquals(cached, MissingMutator))
            return false;

        mutator = cached;
        return true;
    }

    private static Mutator BuildMutator(Type targetType, MethodInfo method, Type argType)
    {
        bool reverseEncodedOrder = IsStackType(targetType);
        try
        {
            // Fasterflect: JIT path
            var invoker = targetType.DelegateForCallMethod(method.Name, argType);
            Action<object, object> action = (obj, val) => invoker(obj, val);
            return new Mutator(action, argType, reverseEncodedOrder);
        }
        catch (PlatformNotSupportedException)
        {
            // Fallback: compiled expression for AOT/WASM
            var paramTarget = Expression.Parameter(typeof(object), "target");
            var paramValue = Expression.Parameter(typeof(object), "value");

            var call = Expression.Call(
                Expression.Convert(paramTarget, targetType),
                method,
                Expression.Convert(paramValue, argType)
            );

            var lambda = Expression.Lambda<Action<object, object>>(call, paramTarget, paramValue).Compile();
            return new Mutator(lambda, argType, reverseEncodedOrder);
        }
    }

    /// <summary>
    /// Holds the compiled single-element mutation delegate and its required parameter type; invocation follows the target collection semantics.<br/>
    /// </summary>
    public sealed class Mutator
    {
        private readonly Action<object, object> _add;
        /// <summary>Retains the compiled add operation and its already-resolved argument type without repeating type analysis during filling.<br/></summary>
        /// <param name="add">Cached mutation delegate; the missing sentinel still throws and is never returned by TryGetMutator.<br/></param>
        /// <param name="elementType">Declared argument type selected while constructing this mutator.<br/></param>
        /// <param name="reverseEncodedOrder">Whether encoded top-to-bottom slots must be visited backwards before pushing.<br/></param>
        internal Mutator(Action<object, object> add, Type elementType, bool reverseEncodedOrder = false) { _add = add; ElementType = elementType; ReverseEncodedOrder = reverseEncodedOrder; }
        /// <summary>
        /// Invokes the cached mutation delegate on a compatible target with an element convertible by its compiled parameter cast. Does not make the target collection thread-safe or impose ordering beyond that mutator.<br/>
        /// </summary>
        /// <param name="target">Collection instance whose runtime type selects the cached mutation method.<br/></param>
        /// <param name="value">Element whose runtime value must match the compiled mutation parameter cast.<br/></param>
        public void Add(object target, object? value) => _add(target, value!);

        /// <summary>Declared add-argument type, retained once with the existing cache entry; not inferred from values or serialization shape.<br/></summary>
        internal readonly Type ElementType;

        /// <summary>Cached destination-order policy for Stack and ConcurrentStack, including their subclasses; no per-element type analysis is required.<br/></summary>
        internal readonly bool ReverseEncodedOrder;
    }

    /// <summary>
    /// Recognizes supported stack families once while constructing a cached mutator.<br/>
    /// Walks the destination's base chain so concrete subclasses retain push-order semantics.<br/>
    /// Ordinary Add/Enqueue collections remain forward; merely naming a custom method Push does not imply a stack contract.<br/>
    /// </summary>
    /// <param name="type">Concrete destination runtime type.<br/></param>
    /// <returns>True for Stack or ConcurrentStack ancestry.<br/></returns>
    private static bool IsStackType(Type type)
    {
        for (Type? current = type; current is not null; current = current.BaseType)
            if (current.IsGenericType && (current.GetGenericTypeDefinition() == typeof(Stack<>) ||
                current.GetGenericTypeDefinition() == typeof(ConcurrentStack<>))) return true;
        return false;
    }
}
