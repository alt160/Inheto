using Fasterflect;
using System.Collections;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace Inheto
{
    /// <summary>
    /// Exposes the encoded name, path and borrowed shape metadata of a navigable Inheto value. Metadata remains valid only while its payload remains unchanged and alive.<br/>
    /// </summary>
    public interface IInhetoObject : IDynamicMetaObjectProvider
    {
        /// <summary>
        /// Gets the encoded member token, including its property, key or collection-element prefix when present.<br/>
        /// </summary>
        string InhetoName { get; }
        /// <summary>
        /// Gets the navigable Inheto path identifying this value in its source payload.<br/>
        /// </summary>
        string InhetoPropPath { get; }
        /// <summary>Borrowed read-only encoded shape; no mutable header mechanics are exposed.<br/></summary>
        InhetoShape InhetoTypeInfo { get; }
    }

    /// <summary>
    /// Extends a navigable Inheto value with its read-only child-property view; read-only navigation does not imply deeply immutable materialized values.<br/>
    /// </summary>
    public interface IInhetoObjectEx : IInhetoObject
    {
        /// <summary>
        /// Gets the read-only child-property map, lazily built for complex/collection views. Values remain navigable wrappers and the map does not imply deep immutability or thread-safe lazy initialization.<br/>
        /// </summary>
        ReadOnlyDictionary<string, IInhetoObject?> Properties { get; }
    }

    internal static class InhetoObjectProperties
    {
        internal static readonly ReadOnlyDictionary<string, IInhetoObject?> Empty
            = new ReadOnlyDictionary<string, IInhetoObject?>(new Dictionary<string, IInhetoObject?>());
    }

    /// <summary>
    /// Provides lazy child navigation over an encoded complex object. Keep its source reader and payload alive and unchanged while navigating.<br/>
    /// </summary>
    public class InhetoObject : IInhetoObjectEx
    {
        private ReadOnlyDictionary<string, IInhetoObject?>? properties;
        private readonly InhetoBinary inhetoBinary;
        private readonly uint entryIndex;
        internal InhetoObject(InhetoBinary inhetoBinary, NameHeaderEntry nameEntry)
        {
            this.inhetoBinary = inhetoBinary;
            entryIndex = nameEntry.Index;
            InhetoName = nameEntry.Name;
            InhetoPropPath = nameEntry.PropPath;
            InhetoTypeInfo = nameEntry.GetShape();
        }
        /// <summary>
        /// Gets the read-only child-property map, lazily built for complex/collection views. Values remain navigable wrappers and the map does not imply deep immutability or thread-safe lazy initialization.<br/>
        /// </summary>
        public ReadOnlyDictionary<string, IInhetoObject?> Properties => properties ??= BuildProperties();

        private ReadOnlyDictionary<string, IInhetoObject?> BuildProperties()
        {
            var names = inhetoBinary.GetAllNames();
            if (names.Count == 0) return InhetoObjectProperties.Empty;

            var props = new Dictionary<string, IInhetoObject?>();
            foreach (var name in from name in names.Values where name.ParentIndex == entryIndex && name.Name != "" select name)
            {
                props.Add(name.CleanName, inhetoBinary.ReadProp(name.PropPath, inhetoBinary.deserializationOptions));
            }
            return props.Count == 0 ? InhetoObjectProperties.Empty : props.AsReadOnly();
        }

        /// <summary>
        /// Gets the encoded member token, including its property, key or collection-element prefix when present.<br/>
        /// </summary>
        public string InhetoName { get; }

        /// <summary>
        /// Gets the navigable Inheto path identifying this value in its source payload.<br/>
        /// </summary>
        public string InhetoPropPath { get; }

        /// <summary>Read-only shape view with the same borrowed payload lifetime as this output.<br/></summary>
        public InhetoShape InhetoTypeInfo { get; }

        /// <summary>
        /// Creates the dynamic binder for child-property navigation on this wrapper; this does not reconstruct the root object.<br/>
        /// </summary>
        /// <param name="parameter">Expression representing this wrapper in the dynamic binding operation.<br/></param>
        /// <returns>New dynamic binding metadata for this wrapper expression.<br/></returns>
        public DynamicMetaObject GetMetaObject(Expression parameter)
            => new InhetoMetaObject(parameter, this);
    }

    /// <summary>
    /// Wraps a materialized three-dimensional array together with borrowed source shape metadata. Value exposes the actual mutable array, not a copy.<br/>
    /// </summary>
    /// <typeparam name="T">Materialized element or value CLR type.<br/></typeparam>
    public class InhetoArray3<T> : IInhetoObjectEx, IEnumerable
    {
        private ReadOnlyDictionary<string, IInhetoObject?> properties;
        internal InhetoArray3(InhetoBinary inhetoBinary, NameHeaderEntry nameEntry)
        {
            properties = InhetoObjectProperties.Empty;
            InhetoName = nameEntry.Name;
            InhetoPropPath = nameEntry.PropPath;
            InhetoTypeInfo = nameEntry.GetShape();
            Value = (T[,,])(inhetoBinary.ReadMDArrayOf<T>(nameEntry.PropPath, inhetoBinary.deserializationOptions)
                ?? throw new InvalidDataException($"Array wrapper construction produced no value for '{nameEntry.PropPath}'."));
        }

        //
        // Summary:
        //     Gets a 64-bit integer that represents the total number of elements in all the
        //     dimensions of the System.Array.
        //
        // Returns:
        //     A 64-bit integer that represents the total number of elements in all the dimensions
        //     of the System.Array.
        /// <summary>
        /// Gets the total number of materialized array elements as a 64-bit count.<br/>
        /// </summary>
        public long LongLength => Value.LongLength;
        //
        // Summary:
        //     Gets the total number of elements in all the dimensions of the System.Array.
        //
        //
        // Returns:
        //     The total number of elements in all the dimensions of the System.Array; zero
        //     if there are no elements in the array.
        //
        // Exceptions:
        //   T:System.OverflowException:
        //     The array is multidimensional and contains more than Int32.MaxValue elements.
        /// <summary>
        /// Gets the total number of materialized array elements as a 32-bit count, following CLR Array.Length semantics.<br/>
        /// </summary>
        public int Length => Value.Length;
        //
        // Summary:
        //     Gets a value indicating whether access to the System.Array is synchronized (thread
        //     safe).
        //
        // Returns:
        //     This property is always false for all arrays.
        /// <summary>
        /// Gets the underlying collection synchronization flag; no automatic locking is added by this wrapper.<br/>
        /// </summary>
        public bool IsSynchronized => Value.IsSynchronized;
        //
        // Summary:
        //     Gets a value indicating whether the System.Array is read-only.
        //
        // Returns:
        //     This property is always false for all arrays.
        /// <summary>
        /// Gets the underlying CLR array read-only flag; array elements can still be changed through Value.<br/>
        /// </summary>
        public bool IsReadOnly => Value.IsReadOnly;
        //
        // Summary:
        //     Gets a value indicating whether the System.Array has a fixed size.
        //
        // Returns:
        //     This property is always true for all arrays.
        /// <summary>
        /// Gets true because this view does not support changing its element/entry count.<br/>
        /// </summary>
        public bool IsFixedSize => true;
        //
        // Summary:
        //     Gets the rank (number of dimensions) of the System.Array. For example, a one-dimensional
        //     array returns 1, a two-dimensional array returns 2, and so on.
        //
        // Returns:
        //     The rank (number of dimensions) of the System.Array.
        /// <summary>
        /// Gets the number of dimensions of the materialized CLR array.<br/>
        /// </summary>
        public int Rank => Value.Rank;


        /// <summary>
        /// Gets the empty declared-child-property map for this materialized array wrapper; use Value or the array indexer for elements.<br/>
        /// </summary>
        public ReadOnlyDictionary<string, IInhetoObject?> Properties => properties;

        /// <summary>
        /// Gets the encoded member token, including its property, key or collection-element prefix when present.<br/>
        /// </summary>
        public string InhetoName { get; }

        /// <summary>
        /// Gets the navigable Inheto path identifying this value in its source payload.<br/>
        /// </summary>
        public string InhetoPropPath { get; }

        /// <summary>Read-only shape view with the same borrowed payload lifetime as this output.<br/></summary>
        public InhetoShape InhetoTypeInfo { get; }

        /// <summary>
        /// Creates the dynamic binder for child-property navigation on this wrapper; this does not reconstruct the root object.<br/>
        /// </summary>
        /// <param name="parameter">Expression representing this wrapper in the dynamic binding operation.<br/></param>
        /// <returns>New dynamic binding metadata for this wrapper expression.<br/></returns>
        public DynamicMetaObject GetMetaObject(Expression parameter)
            => new InhetoMetaObject(parameter, this);

        /// <summary>
        /// Delegates copying to CLR Array.CopyTo, including its rank, destination-type and capacity restrictions. It does not flatten rectangular arrays into a new representation.<br/>
        /// </summary>
        /// <param name="array">Destination CLR array, subject to Array.CopyTo type, rank and capacity restrictions.<br/></param>
        /// <param name="index">Element or destination starting position, subject to the underlying CLR collection bounds.<br/></param>
        public void CopyTo(Array array, int index)
        {
            Value.CopyTo(array, index);
        }

        /// <summary>
        /// Returns enumeration over the materialized array in CLR array order; no separate element array is created.<br/>
        /// </summary>
        /// <returns>Enumerator over the existing materialized elements or dictionary entries.<br/></returns>
        public IEnumerator GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        /// <summary>
        /// Gets the existing materialized element at the specified position or coordinates; underlying CLR bounds checks apply and no copy is made.<br/>
        /// </summary>
        /// <param name="x">Index in the first dimension of the materialized array.<br/></param>
        /// <param name="y">Index in the second dimension of the materialized array.<br/></param>
        /// <param name="z">Index in the third dimension of the materialized array.<br/></param>
        /// <returns>Existing element or value wrapper at the supplied key/coordinates; invalid positions or keys throw.<br/></returns>
        public T this[int x, int y, int z] => Value[x, y, z];

        /// <summary>
        /// Gets the materialized CLR value held by this wrapper without cloning it. Mutable values remain mutable; borrowed shape metadata has a separate source-payload lifetime.<br/>
        /// </summary>
        public T[,,] Value { get; }

        /// <summary>
        /// Gets the underlying collection synchronization object for caller-managed coordination; accessing it does not acquire a lock.<br/>
        /// </summary>
        public object SyncRoot => Value.SyncRoot;

    }
    /// <summary>
    /// Wraps a materialized two-dimensional array together with borrowed source shape metadata. Value exposes the actual mutable array, not a copy.<br/>
    /// </summary>
    /// <typeparam name="T">Materialized element or value CLR type.<br/></typeparam>
    public class InhetoArray2<T> : IInhetoObjectEx, IEnumerable
    {
        private ReadOnlyDictionary<string, IInhetoObject?> properties;
        internal InhetoArray2(InhetoBinary inhetoBinary, NameHeaderEntry nameEntry)
        {
            properties = InhetoObjectProperties.Empty;
            InhetoName = nameEntry.Name;
            InhetoPropPath = nameEntry.PropPath;
            InhetoTypeInfo = nameEntry.GetShape();
            Value = (T[,])(inhetoBinary.ReadMDArrayOf<T>(nameEntry.PropPath, inhetoBinary.deserializationOptions)
                ?? throw new InvalidDataException($"Array wrapper construction produced no value for '{nameEntry.PropPath}'."));
        }

        //
        // Summary:
        //     Gets a 64-bit integer that represents the total number of elements in all the
        //     dimensions of the System.Array.
        //
        // Returns:
        //     A 64-bit integer that represents the total number of elements in all the dimensions
        //     of the System.Array.
        /// <summary>
        /// Gets the total number of materialized array elements as a 64-bit count.<br/>
        /// </summary>
        public long LongLength => Value.LongLength;
        //
        // Summary:
        //     Gets the total number of elements in all the dimensions of the System.Array.
        //
        //
        // Returns:
        //     The total number of elements in all the dimensions of the System.Array; zero
        //     if there are no elements in the array.
        //
        // Exceptions:
        //   T:System.OverflowException:
        //     The array is multidimensional and contains more than Int32.MaxValue elements.
        /// <summary>
        /// Gets the total number of materialized array elements as a 32-bit count, following CLR Array.Length semantics.<br/>
        /// </summary>
        public int Length => Value.Length;
        //
        // Summary:
        //     Gets a value indicating whether access to the System.Array is synchronized (thread
        //     safe).
        //
        // Returns:
        //     This property is always false for all arrays.
        /// <summary>
        /// Gets the underlying collection synchronization flag; no automatic locking is added by this wrapper.<br/>
        /// </summary>
        public bool IsSynchronized => Value.IsSynchronized;
        //
        // Summary:
        //     Gets a value indicating whether the System.Array is read-only.
        //
        // Returns:
        //     This property is always false for all arrays.
        /// <summary>
        /// Gets the underlying CLR array read-only flag; array elements can still be changed through Value.<br/>
        /// </summary>
        public bool IsReadOnly => Value.IsReadOnly;
        //
        // Summary:
        //     Gets a value indicating whether the System.Array has a fixed size.
        //
        // Returns:
        //     This property is always true for all arrays.
        /// <summary>
        /// Gets true because this view does not support changing its element/entry count.<br/>
        /// </summary>
        public bool IsFixedSize => true;
        //
        // Summary:
        //     Gets the rank (number of dimensions) of the System.Array. For example, a one-dimensional
        //     array returns 1, a two-dimensional array returns 2, and so on.
        //
        // Returns:
        //     The rank (number of dimensions) of the System.Array.
        /// <summary>
        /// Gets the number of dimensions of the materialized CLR array.<br/>
        /// </summary>
        public int Rank => Value.Rank;


        /// <summary>
        /// Gets the empty declared-child-property map for this materialized array wrapper; use Value or the array indexer for elements.<br/>
        /// </summary>
        public ReadOnlyDictionary<string, IInhetoObject?> Properties => properties;

        /// <summary>
        /// Gets the encoded member token, including its property, key or collection-element prefix when present.<br/>
        /// </summary>
        public string InhetoName { get; }

        /// <summary>
        /// Gets the navigable Inheto path identifying this value in its source payload.<br/>
        /// </summary>
        public string InhetoPropPath { get; }

        /// <summary>Read-only shape view with the same borrowed payload lifetime as this output.<br/></summary>
        public InhetoShape InhetoTypeInfo { get; }

        /// <summary>
        /// Creates the dynamic binder for child-property navigation on this wrapper; this does not reconstruct the root object.<br/>
        /// </summary>
        /// <param name="parameter">Expression representing this wrapper in the dynamic binding operation.<br/></param>
        /// <returns>New dynamic binding metadata for this wrapper expression.<br/></returns>
        public DynamicMetaObject GetMetaObject(Expression parameter)
            => new InhetoMetaObject(parameter, this);

        /// <summary>
        /// Delegates copying to CLR Array.CopyTo, including its rank, destination-type and capacity restrictions. It does not flatten rectangular arrays into a new representation.<br/>
        /// </summary>
        /// <param name="array">Destination CLR array, subject to Array.CopyTo type, rank and capacity restrictions.<br/></param>
        /// <param name="index">Element or destination starting position, subject to the underlying CLR collection bounds.<br/></param>
        public void CopyTo(Array array, int index)
        {
            Value.CopyTo(array, index);
        }

        /// <summary>
        /// Returns enumeration over the materialized array in CLR array order; no separate element array is created.<br/>
        /// </summary>
        /// <returns>Enumerator over the existing materialized elements or dictionary entries.<br/></returns>
        public IEnumerator GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        /// <summary>
        /// Gets the existing materialized element at the specified position or coordinates; underlying CLR bounds checks apply and no copy is made.<br/>
        /// </summary>
        /// <param name="x">Index in the first dimension of the materialized array.<br/></param>
        /// <param name="y">Index in the second dimension of the materialized array.<br/></param>
        /// <returns>Existing element or value wrapper at the supplied key/coordinates; invalid positions or keys throw.<br/></returns>
        public T this[int x, int y] => Value[x, y];

        /// <summary>
        /// Gets the materialized CLR value held by this wrapper without cloning it. Mutable values remain mutable; borrowed shape metadata has a separate source-payload lifetime.<br/>
        /// </summary>
        public T[,] Value { get; }

        /// <summary>
        /// Gets the underlying collection synchronization object for caller-managed coordination; accessing it does not acquire a lock.<br/>
        /// </summary>
        public object SyncRoot => Value.SyncRoot;

    }
    /// <summary>
    /// Wraps a materialized one-dimensional array together with borrowed source shape metadata. Value exposes the actual mutable array, not a copy.<br/>
    /// </summary>
    /// <typeparam name="T">Materialized element or value CLR type.<br/></typeparam>
    public class InhetoArray1<T> : IInhetoObjectEx, IEnumerable, IEnumerable<T>, IReadOnlyList<T>
    {
        private ReadOnlyDictionary<string, IInhetoObject?> properties;
        internal InhetoArray1(InhetoBinary inhetoBinary, NameHeaderEntry nameEntry)
        {
            properties = InhetoObjectProperties.Empty;
            InhetoName = nameEntry.Name;
            InhetoPropPath = nameEntry.PropPath;
            InhetoTypeInfo = nameEntry.GetShape();
            Value = (T[])(inhetoBinary.ReadArrayOf<T>(nameEntry.PropPath, inhetoBinary.deserializationOptions, true)
                ?? throw new InvalidDataException($"Array wrapper construction produced no value for '{nameEntry.PropPath}'."));
        }

        //
        // Summary:
        //     Gets a 64-bit integer that represents the total number of elements in all the
        //     dimensions of the System.Array.
        //
        // Returns:
        //     A 64-bit integer that represents the total number of elements in all the dimensions
        //     of the System.Array.
        /// <summary>
        /// Gets the total number of materialized array elements as a 64-bit count.<br/>
        /// </summary>
        public long LongLength => Value.LongLength;
        //
        // Summary:
        //     Gets the total number of elements in all the dimensions of the System.Array.
        //
        //
        // Returns:
        //     The total number of elements in all the dimensions of the System.Array; zero
        //     if there are no elements in the array.
        //
        // Exceptions:
        //   T:System.OverflowException:
        //     The array is multidimensional and contains more than Int32.MaxValue elements.
        /// <summary>
        /// Gets the total number of materialized array elements as a 32-bit count, following CLR Array.Length semantics.<br/>
        /// </summary>
        public int Length => Value.Length;
        //
        // Summary:
        //     Gets a value indicating whether access to the System.Array is synchronized (thread
        //     safe).
        //
        // Returns:
        //     This property is always false for all arrays.
        /// <summary>
        /// Gets the underlying collection synchronization flag; no automatic locking is added by this wrapper.<br/>
        /// </summary>
        public bool IsSynchronized => Value.IsSynchronized;
        //
        // Summary:
        //     Gets a value indicating whether the System.Array is read-only.
        //
        // Returns:
        //     This property is always false for all arrays.
        /// <summary>
        /// Gets the underlying CLR array read-only flag; array elements can still be changed through Value.<br/>
        /// </summary>
        public bool IsReadOnly => Value.IsReadOnly;
        //
        // Summary:
        //     Gets a value indicating whether the System.Array has a fixed size.
        //
        // Returns:
        //     This property is always true for all arrays.
        /// <summary>
        /// Gets true because this view does not support changing its element/entry count.<br/>
        /// </summary>
        public bool IsFixedSize => true;
        //
        // Summary:
        //     Gets the rank (number of dimensions) of the System.Array. For example, a one-dimensional
        //     array returns 1, a two-dimensional array returns 2, and so on.
        //
        // Returns:
        //     The rank (number of dimensions) of the System.Array.
        /// <summary>
        /// Gets the number of dimensions of the materialized CLR array.<br/>
        /// </summary>
        public int Rank => Value.Rank;


        /// <summary>
        /// Gets the empty declared-child-property map for this materialized array wrapper; use Value or the array indexer for elements.<br/>
        /// </summary>
        public ReadOnlyDictionary<string, IInhetoObject?> Properties => properties;

        /// <summary>
        /// Gets the encoded member token, including its property, key or collection-element prefix when present.<br/>
        /// </summary>
        public string InhetoName { get; }

        /// <summary>
        /// Gets the navigable Inheto path identifying this value in its source payload.<br/>
        /// </summary>
        public string InhetoPropPath { get; }

        /// <summary>Read-only shape view with the same borrowed payload lifetime as this output.<br/></summary>
        public InhetoShape InhetoTypeInfo { get; }

        /// <summary>
        /// Creates the dynamic binder for child-property navigation on this wrapper; this does not reconstruct the root object.<br/>
        /// </summary>
        /// <param name="parameter">Expression representing this wrapper in the dynamic binding operation.<br/></param>
        /// <returns>New dynamic binding metadata for this wrapper expression.<br/></returns>
        public DynamicMetaObject GetMetaObject(Expression parameter)
            => new InhetoMetaObject(parameter, this);

        /// <summary>
        /// Delegates copying to CLR Array.CopyTo, including its rank, destination-type and capacity restrictions. It does not flatten rectangular arrays into a new representation.<br/>
        /// </summary>
        /// <param name="array">Destination CLR array, subject to Array.CopyTo type, rank and capacity restrictions.<br/></param>
        /// <param name="index">Element or destination starting position, subject to the underlying CLR collection bounds.<br/></param>
        public void CopyTo(Array array, int index)
        {
            Value.CopyTo(array, index);
        }

        /// <summary>
        /// Returns enumeration over the materialized array in CLR array order; no separate element array is created.<br/>
        /// </summary>
        /// <returns>Enumerator over the existing materialized elements or dictionary entries.<br/></returns>
        public IEnumerator GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)Value).GetEnumerator();
        }

        /// <summary>
        /// Gets the existing materialized element at the specified position or coordinates; underlying CLR bounds checks apply and no copy is made.<br/>
        /// </summary>
        /// <param name="index">Element or destination starting position, subject to the underlying CLR collection bounds.<br/></param>
        /// <returns>Existing element or value wrapper at the supplied key/coordinates; invalid positions or keys throw.<br/></returns>
        public T this[int index] => Value[index];

        /// <summary>
        /// Gets the materialized CLR value held by this wrapper without cloning it. Mutable values remain mutable; borrowed shape metadata has a separate source-payload lifetime.<br/>
        /// </summary>
        public T[] Value { get; }

        /// <summary>
        /// Gets the underlying collection synchronization object for caller-managed coordination; accessing it does not acquire a lock.<br/>
        /// </summary>
        public object SyncRoot => Value.SyncRoot;

        /// <summary>
        /// Gets the number of materialized array elements or dictionary entries represented by this view.<br/>
        /// </summary>
        public int Count => ((IReadOnlyCollection<T>)Value).Count;


        // implicit cast
        /// <summary>
        /// Returns the wrapper&apos;s existing Value without a clone or additional materialization; a null wrapper is not accepted.<br/>
        /// </summary>
        /// <param name="inhetoObject">Wrapper whose existing Value is returned without cloning.<br/></param>
        /// <returns>Wrapped descriptor or existing materialized Value, without cloning the value.<br/></returns>
        public static implicit operator T[](InhetoArray1<T> inhetoObject) => inhetoObject.Value;

        /// <summary>
        /// Wraps an already materialized rank-one array without reading or copying it again.<br/>
        /// T is the immediate element type, which is itself an array for jagged outputs.<br/>
        /// Metadata retains the normal borrowed payload lifetime.<br/>
        /// </summary>
        /// <param name="nameEntry">The resolved source entry supplying the name, path, and shape.<br/></param>
        /// <param name="value">The non-null array produced by the existing materializer.<br/></param>
        internal InhetoArray1(NameHeaderEntry nameEntry, T[] value)
        {
            properties = InhetoObjectProperties.Empty;
            InhetoName = nameEntry.Name;
            InhetoPropPath = nameEntry.PropPath;
            InhetoTypeInfo = nameEntry.GetShape();
            Value = value;
        }

    }
    /// <summary>
    /// Wraps a materialized CLR array of the encoded rank together with borrowed source shape metadata. Value exposes the actual array, not a copy.<br/>
    /// </summary>
    /// <typeparam name="T">Materialized element or value CLR type.<br/></typeparam>
    public class InhetoArray<T> : IInhetoObjectEx, IEnumerable
    {
        private ReadOnlyDictionary<string, IInhetoObject?> properties;
        internal InhetoArray(InhetoBinary inhetoBinary, NameHeaderEntry nameEntry)
        {
            properties = InhetoObjectProperties.Empty;
            InhetoName = nameEntry.Name;
            InhetoPropPath = nameEntry.PropPath;
            InhetoTypeInfo = nameEntry.GetShape();
            Value = inhetoBinary.ReadArrayOf<T>(nameEntry.PropPath, inhetoBinary.deserializationOptions, true)
                ?? throw new InvalidDataException($"Array wrapper construction produced no value for '{nameEntry.PropPath}'.");
        }

        //
        // Summary:
        //     Gets a 64-bit integer that represents the total number of elements in all the
        //     dimensions of the System.Array.
        //
        // Returns:
        //     A 64-bit integer that represents the total number of elements in all the dimensions
        //     of the System.Array.
        /// <summary>
        /// Gets the total number of materialized array elements as a 64-bit count.<br/>
        /// </summary>
        public long LongLength => Value.LongLength;
        //
        // Summary:
        //     Gets the total number of elements in all the dimensions of the System.Array.
        //
        //
        // Returns:
        //     The total number of elements in all the dimensions of the System.Array; zero
        //     if there are no elements in the array.
        //
        // Exceptions:
        //   T:System.OverflowException:
        //     The array is multidimensional and contains more than Int32.MaxValue elements.
        /// <summary>
        /// Gets the total number of materialized array elements as a 32-bit count, following CLR Array.Length semantics.<br/>
        /// </summary>
        public int Length => Value.Length;
        //
        // Summary:
        //     Gets a value indicating whether access to the System.Array is synchronized (thread
        //     safe).
        //
        // Returns:
        //     This property is always false for all arrays.
        /// <summary>
        /// Gets the underlying collection synchronization flag; no automatic locking is added by this wrapper.<br/>
        /// </summary>
        public bool IsSynchronized => Value.IsSynchronized;
        //
        // Summary:
        //     Gets a value indicating whether the System.Array is read-only.
        //
        // Returns:
        //     This property is always false for all arrays.
        /// <summary>
        /// Gets the underlying CLR array read-only flag; array elements can still be changed through Value.<br/>
        /// </summary>
        public bool IsReadOnly => Value.IsReadOnly;
        //
        // Summary:
        //     Gets a value indicating whether the System.Array has a fixed size.
        //
        // Returns:
        //     This property is always true for all arrays.
        /// <summary>
        /// Gets true because this view does not support changing its element/entry count.<br/>
        /// </summary>
        public bool IsFixedSize => true;
        //
        // Summary:
        //     Gets the rank (number of dimensions) of the System.Array. For example, a one-dimensional
        //     array returns 1, a two-dimensional array returns 2, and so on.
        //
        // Returns:
        //     The rank (number of dimensions) of the System.Array.
        /// <summary>
        /// Gets the number of dimensions of the materialized CLR array.<br/>
        /// </summary>
        public int Rank => Value.Rank;


        /// <summary>
        /// Gets the empty declared-child-property map for this materialized array wrapper; use Value or the array indexer for elements.<br/>
        /// </summary>
        public ReadOnlyDictionary<string, IInhetoObject?> Properties => properties;

        /// <summary>
        /// Gets the encoded member token, including its property, key or collection-element prefix when present.<br/>
        /// </summary>
        public string InhetoName { get; }

        /// <summary>
        /// Gets the navigable Inheto path identifying this value in its source payload.<br/>
        /// </summary>
        public string InhetoPropPath { get; }

        /// <summary>Read-only shape view with the same borrowed payload lifetime as this output.<br/></summary>
        public InhetoShape InhetoTypeInfo { get; }

        /// <summary>
        /// Creates the dynamic binder for child-property navigation on this wrapper; this does not reconstruct the root object.<br/>
        /// </summary>
        /// <param name="parameter">Expression representing this wrapper in the dynamic binding operation.<br/></param>
        /// <returns>New dynamic binding metadata for this wrapper expression.<br/></returns>
        public DynamicMetaObject GetMetaObject(Expression parameter)
            => new InhetoMetaObject(parameter, this);

        /// <summary>
        /// Delegates copying to CLR Array.CopyTo, including its rank, destination-type and capacity restrictions. It does not flatten rectangular arrays into a new representation.<br/>
        /// </summary>
        /// <param name="array">Destination CLR array, subject to Array.CopyTo type, rank and capacity restrictions.<br/></param>
        /// <param name="index">Element or destination starting position, subject to the underlying CLR collection bounds.<br/></param>
        public void CopyTo(Array array, int index)
        {
            Value.CopyTo(array, index);
        }

        /// <summary>
        /// Returns enumeration over the materialized array in CLR array order; no separate element array is created.<br/>
        /// </summary>
        /// <returns>Enumerator over the existing materialized elements or dictionary entries.<br/></returns>
        public IEnumerator GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        /// <summary>
        /// Gets the materialized CLR value held by this wrapper without cloning it. Mutable values remain mutable; borrowed shape metadata has a separate source-payload lifetime.<br/>
        /// </summary>
        public Array Value { get; }

        /// <summary>
        /// Gets the underlying collection synchronization object for caller-managed coordination; accessing it does not acquire a lock.<br/>
        /// </summary>
        public object SyncRoot => Value.SyncRoot;


        // implicit cast
        /// <summary>
        /// Returns the wrapper&apos;s existing Value without a clone or additional materialization; a null wrapper is not accepted.<br/>
        /// </summary>
        /// <param name="inhetoObject">Wrapper whose existing Value is returned without cloning.<br/></param>
        /// <returns>Wrapped descriptor or existing materialized Value, without cloning the value.<br/></returns>
        public static implicit operator Array(InhetoArray<T> inhetoObject) => inhetoObject.Value;

    }

    /// <summary>
    /// Exposes a rank-four CLR array through strongly typed, allocation-free coordinate access.<br/>
    /// The dedicated wrapper preserves the specialized rank-one-through-rank-four developer experience while higher ranks use <see cref="InhetoArray{T}"/>.<br/>
    /// </summary>
    /// <typeparam name="T">The declared CLR element type.<br/></typeparam>
    public class InhetoArray4<T> : IInhetoObjectEx, IEnumerable
    {
        private readonly ReadOnlyDictionary<string, IInhetoObject?> properties;

        /// <summary>
        /// Materializes the non-null rank-four value represented by the supplied name entry.<br/>
        /// Construction fails explicitly if array metadata resolves to no value or to an incompatible rank or element type.<br/>
        /// </summary>
        /// <param name="inhetoBinary">The owning binary payload reader.<br/></param>
        /// <param name="nameEntry">The rank-four array entry to materialize.<br/></param>
        /// <exception cref="InvalidDataException">The entry cannot materialize as a non-null <typeparamref name="T"/> rank-four array.<br/></exception>
        internal InhetoArray4(InhetoBinary inhetoBinary, NameHeaderEntry nameEntry)
        {
            properties = InhetoObjectProperties.Empty;
            InhetoName = nameEntry.Name;
            InhetoPropPath = nameEntry.PropPath;
            InhetoTypeInfo = nameEntry.GetShape();
            Value = (T[,,,])(inhetoBinary.ReadMDArrayOf<T>(nameEntry.PropPath, inhetoBinary.deserializationOptions)
                ?? throw new InvalidDataException($"Array wrapper construction produced no value for '{nameEntry.PropPath}'."));
        }

        /// <summary>
        /// Gets the total number of materialized array elements as a 64-bit count.<br/>
        /// </summary>
        public long LongLength => Value.LongLength;
        /// <summary>
        /// Gets the total number of materialized array elements as a 32-bit count, following CLR Array.Length semantics.<br/>
        /// </summary>
        public int Length => Value.Length;
        /// <summary>
        /// Gets the underlying collection synchronization flag; no automatic locking is added by this wrapper.<br/>
        /// </summary>
        public bool IsSynchronized => Value.IsSynchronized;
        /// <summary>
        /// Gets the underlying CLR array read-only flag; array elements can still be changed through Value.<br/>
        /// </summary>
        public bool IsReadOnly => Value.IsReadOnly;
        /// <summary>
        /// Gets true because this view does not support changing its element/entry count.<br/>
        /// </summary>
        public bool IsFixedSize => true;
        /// <summary>
        /// Gets the number of dimensions of the materialized CLR array.<br/>
        /// </summary>
        public int Rank => Value.Rank;
        /// <summary>
        /// Gets the empty declared-child-property map for this materialized array wrapper; use Value or the array indexer for elements.<br/>
        /// </summary>
        public ReadOnlyDictionary<string, IInhetoObject?> Properties => properties;
        /// <summary>
        /// Gets the encoded member token, including its property, key or collection-element prefix when present.<br/>
        /// </summary>
        public string InhetoName { get; }
        /// <summary>
        /// Gets the navigable Inheto path identifying this value in its source payload.<br/>
        /// </summary>
        public string InhetoPropPath { get; }
        /// <summary>Read-only shape view with the same borrowed payload lifetime as this output.<br/></summary>
        public InhetoShape InhetoTypeInfo { get; }
        /// <summary>
        /// Gets the existing materialized element at the specified position or coordinates; underlying CLR bounds checks apply and no copy is made.<br/>
        /// </summary>
        /// <param name="x">Index in the first dimension of the materialized array.<br/></param>
        /// <param name="y">Index in the second dimension of the materialized array.<br/></param>
        /// <param name="z">Index in the third dimension of the materialized array.<br/></param>
        /// <param name="w">Index in the fourth dimension of the materialized array.<br/></param>
        /// <returns>Existing element or value wrapper at the supplied key/coordinates; invalid positions or keys throw.<br/></returns>
        public T this[int x, int y, int z, int w] => Value[x, y, z, w];
        /// <summary>
        /// Gets the materialized CLR value held by this wrapper without cloning it. Mutable values remain mutable; borrowed shape metadata has a separate source-payload lifetime.<br/>
        /// </summary>
        public T[,,,] Value { get; }
        /// <summary>
        /// Gets the underlying collection synchronization object for caller-managed coordination; accessing it does not acquire a lock.<br/>
        /// </summary>
        public object SyncRoot => Value.SyncRoot;

        /// <summary>
        /// Creates the dynamic binding metadata used by the agnostic object surface.<br/>
        /// </summary>
        /// <param name="parameter">The expression representing this wrapper.<br/></param>
        /// <returns>The dynamic meta-object bound to this wrapper.<br/></returns>
        public DynamicMetaObject GetMetaObject(Expression parameter)
            => new InhetoMetaObject(parameter, this);

        /// <summary>
        /// Copies the rank-four value into a compatible destination array beginning at the requested logical offset.<br/>
        /// CLR array rank, shape, and element compatibility rules remain authoritative.<br/>
        /// </summary>
        /// <param name="array">The compatible destination array.<br/></param>
        /// <param name="index">The destination's starting logical offset.<br/></param>
        public void CopyTo(Array array, int index)
        {
            Value.CopyTo(array, index);
        }

        /// <summary>
        /// Enumerates the rank-four CLR array in its native row-major order.<br/>
        /// </summary>
        /// <returns>The CLR array's non-generic enumerator.<br/></returns>
        public IEnumerator GetEnumerator()
        {
            return Value.GetEnumerator();
        }
    }

    /// <summary>
    /// Exposes materialized collection elements by position and lazily navigable declared members. Its shape metadata borrows the source payload lifetime.<br/>
    /// </summary>
    public class InhetoCollection : IInhetoObjectEx, IEnumerable
    {
        private ReadOnlyDictionary<string, IInhetoObject?>? properties;
        private readonly InhetoBinary inhetoBinary;
        private readonly uint entryIndex;
        private List<object?> elements;
        internal InhetoCollection(InhetoBinary inhetoBinary, NameHeaderEntry entry)
        {
            this.inhetoBinary = inhetoBinary;
            entryIndex = entry.Index;
            InhetoName = entry.Name;
            InhetoPropPath = entry.PropPath;
            InhetoTypeInfo = entry.GetShape();
            elements = new List<object?>();
            var dzOptions = inhetoBinary.deserializationOptions;
            switch (entry.Type.CollectionInfo.CollectionType)
            {
                case InhetoCollectionTypes.None:
                    switch (entry.Kind)
                    {
                        case InhetoKindMasks.InhetoBaseType:
                            switch (entry.Type.BaseType)
                            {
                                case InhetoBaseTypes.ListString:
                                    inhetoBinary.FillStringList(entry, elements as IEnumerable, true, false);
                                    break;
                            }
                            break;
                        case InhetoKindMasks.InhetoPromotedType:
                            switch (entry.Type.PromotedType)
                            {
                                case InhetoPromotedTypes.StringCollection:
                                    foreach (var str in inhetoBinary.ReadStringCollection(entry.PropPath)
                                        ?? throw new InvalidDataException("Expected a non-null StringCollection for this collection header."))
                                        elements.Add(str);
                                    break;
                                case InhetoPromotedTypes.Enumerable:
                                    for (int i = 0; i < entry.ValueOffset; i++)
                                    {
                                        elements.Add(inhetoBinary.ReadProp($"{entry.PropPath}#{i}", inhetoBinary.deserializationOptions));
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
                case InhetoCollectionTypes.List:
                case InhetoCollectionTypes.Queue:
                case InhetoCollectionTypes.ConcurrentQueue:
                case InhetoCollectionTypes.Stack:
                case InhetoCollectionTypes.ConcurrentStack:
                case InhetoCollectionTypes.ObservableCollection:
                case InhetoCollectionTypes.ConcurrentBag:
                case InhetoCollectionTypes.SortedSet:
                case InhetoCollectionTypes.HashSet:
                    {
                        Type? tType = InhetoToTypeResolver.Resolve(entry);
                        if (tType is null) tType = typeof(List<object>);

                        var projectedTypeInfo = InhetoTypeResolver.GetInhetoTypeInfo(tType, dzOptions.MemberTypes);
                        var dCount = entry.ValueOffset;

                        if (!EnumerableMutator.TryGetMutator(elements, out var mutator)) return;

                        switch (entry.Type.CollectionInfo.ValueType.Kind)
                        {
                            case InhetoKindMasks.InhetoBaseType:
                                switch (entry.Type.CollectionInfo.ValueType.BaseType)
                                {
                                    case InhetoBaseTypes.ComplexType:
                                        {

                                            for (int i = 0; i < dCount; i++)
                                            {
                                                object? collItem = null;
                                                collItem = inhetoBinary.ReadProp($"{entry.PropPath}#{i}", dzOptions);
                                                mutator.Add(elements, collItem);
                                            }
                                            break;

                                        }
                                    case InhetoBaseTypes.Boolean:
                                        inhetoBinary.FillBooleanList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Char:
                                        inhetoBinary.FillCharList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Rune:
                                        inhetoBinary.FillRuneList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.String:
                                        inhetoBinary.FillStringList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.SByte:
                                        inhetoBinary.FillSByteList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Byte:
                                        inhetoBinary.FillByteList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Int16:
                                        inhetoBinary.FillInt16List(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.UInt16:
                                        inhetoBinary.FillUInt16List(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Int32:
                                        inhetoBinary.FillInt32List(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.UInt32:
                                        inhetoBinary.FillUInt32List(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Int64:
                                        inhetoBinary.FillInt64List(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.UInt64:
                                        inhetoBinary.FillUInt64List(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Int128:
                                        inhetoBinary.FillInt128List(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.UInt128:
                                        inhetoBinary.FillUInt128List(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Half:
                                        inhetoBinary.FillHalfList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Single:
                                        inhetoBinary.FillSingleList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Double:
                                        inhetoBinary.FillDoubleList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Decimal:
                                        inhetoBinary.FillDecimalList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.DateOnly:
                                        inhetoBinary.FillDateOnlyList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.DateTime:
                                        inhetoBinary.FillDateTimeList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.DateTimeOffset:
                                        inhetoBinary.FillDateTimeOffsetList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.TimeOnly:
                                        inhetoBinary.FillTimeOnlyList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.TimeSpan:
                                        inhetoBinary.FillTimeSpanList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Guid:
                                        inhetoBinary.FillGuidList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Version:
                                        inhetoBinary.FillVersionList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Uri:
                                        inhetoBinary.FillUriList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Vector2:
                                        inhetoBinary.FillVector2List(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Vector3:
                                        inhetoBinary.FillVector3List(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Vector4:
                                        inhetoBinary.FillVector4List(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.ComplexNumeric:
                                        inhetoBinary.FillComplexNumericList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Quaternion:
                                        inhetoBinary.FillQuaternionList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Plane:
                                        inhetoBinary.FillPlaneList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Matrix3x2:
                                        inhetoBinary.FillMatrix3x2List(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.Matrix4x4:
                                        inhetoBinary.FillMatrix4x4List(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.BigInteger:
                                        inhetoBinary.FillBigIntegerList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.ByteArray:
                                        inhetoBinary.FillByteArrayList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoBaseTypes.ListString:
                                        inhetoBinary.FillListStringList(entry, elements as IEnumerable, true, false);
                                        break;

                                }
                                break;
                            case InhetoKindMasks.InhetoPromotedType:
                                switch (entry.Type.CollectionInfo.ValueType.PromotedType)
                                {
                                    case InhetoPromotedTypes.Enum:
                                        switch (entry.Type.CollectionInfo.ValueType.EnumType)
                                        {
                                            case InhetoBaseTypes.SByte:
                                                inhetoBinary.FillEnumList<sbyte>(entry, elements as IList, true, false);
                                                break;
                                            case InhetoBaseTypes.Byte:
                                                inhetoBinary.FillEnumList<byte>(entry, elements as IList, true, false);
                                                break;
                                            case InhetoBaseTypes.Int16:
                                                inhetoBinary.FillEnumList<short>(entry, elements as IList, true, false);
                                                break;
                                            case InhetoBaseTypes.UInt16:
                                                inhetoBinary.FillEnumList<ushort>(entry, elements as IList, true, false);
                                                break;
                                            case InhetoBaseTypes.Int32:
                                                inhetoBinary.FillEnumList<int>(entry, elements as IList, true, false);
                                                break;
                                            case InhetoBaseTypes.UInt32:
                                                inhetoBinary.FillEnumList<uint>(entry, elements as IList, true, false);
                                                break;
                                            case InhetoBaseTypes.Int64:
                                                inhetoBinary.FillEnumList<long>(entry, elements as IList, true, false);
                                                break;
                                            case InhetoBaseTypes.UInt64:
                                                inhetoBinary.FillEnumList<ulong>(entry, elements as IList, true, false);
                                                break;
                                        }
                                        break;
                                    case InhetoPromotedTypes.CultureInfo:
                                        inhetoBinary.FillCultureInfoList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoPromotedTypes.IPAddress:
                                        // need to handle ReadOnlyIPAddress
                                        inhetoBinary.FillIPAddressList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoPromotedTypes.IPEndPoint:
                                        inhetoBinary.FillIPEndPointList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoPromotedTypes.MailAddress:
                                        inhetoBinary.FillMailAddressList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoPromotedTypes.MemoryByte:
                                        inhetoBinary.FillMemoryByteList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoPromotedTypes.MemoryStream:
                                        inhetoBinary.FillMemoryStreamList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoPromotedTypes.ReadOnlyMemoryByte:
                                        inhetoBinary.FillReadOnlyMemoryByteList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoPromotedTypes.StringBuilder:
                                        inhetoBinary.FillStringBuilderList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoPromotedTypes.TimeZoneInfo:
                                        inhetoBinary.FillTimeZoneInfoList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoPromotedTypes.Regex:
                                        inhetoBinary.FillRegexList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoPromotedTypes.SecureString:
                                        inhetoBinary.FillSecureStringList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoPromotedTypes.Type:
                                        inhetoBinary.FillTypeList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoPromotedTypes.NameValueCollection:
                                        inhetoBinary.FillNameValueCollectionList(entry, elements as IEnumerable, true, false);
                                        break;
                                    case InhetoPromotedTypes.StringCollection:
                                        inhetoBinary.FillStringCollectionList(entry, elements as IEnumerable, true, false);
                                        break;
                                }
                                break;

                        }

                    }
                    break;
            }
        }

        /// <summary>
        /// Gets the read-only child-property map, lazily built for complex/collection views. Values remain navigable wrappers and the map does not imply deep immutability or thread-safe lazy initialization.<br/>
        /// </summary>
        public ReadOnlyDictionary<string, IInhetoObject?> Properties => properties ??= BuildProperties();

        private ReadOnlyDictionary<string, IInhetoObject?> BuildProperties()
        {
            var names = inhetoBinary.GetAllNames();
            if (names.Count == 0) return InhetoObjectProperties.Empty;

            var props = new Dictionary<string, IInhetoObject?>();
            foreach (var name in from name in names.Values where name.ParentIndex == entryIndex && name.Name != "" && name.Name[0] == '.' select name)
            {
                props.Add(name.CleanName, inhetoBinary.ReadProp(name.PropPath, inhetoBinary.deserializationOptions));
            }
            return props.Count == 0 ? InhetoObjectProperties.Empty : props.AsReadOnly();
        }

        /// <summary>
        /// Gets the encoded member token, including its property, key or collection-element prefix when present.<br/>
        /// </summary>
        public string InhetoName { get; }

        /// <summary>
        /// Gets the navigable Inheto path identifying this value in its source payload.<br/>
        /// </summary>
        public string InhetoPropPath { get; }

        /// <summary>Read-only shape view with the same borrowed payload lifetime as this output.<br/></summary>
        public InhetoShape InhetoTypeInfo { get; }

        /// <summary>
        /// Gets the existing materialized element at the specified position or coordinates; underlying CLR bounds checks apply and no copy is made.<br/>
        /// </summary>
        /// <param name="index">Element or destination starting position, subject to the underlying CLR collection bounds.<br/></param>
        /// <returns>Existing element or value wrapper at the supplied key/coordinates; invalid positions or keys throw.<br/></returns>
        public object? this[int index] => elements[index];

        /// <summary>
        /// Creates the dynamic binder for child-property navigation on this wrapper; this does not reconstruct the root object.<br/>
        /// </summary>
        /// <param name="parameter">Expression representing this wrapper in the dynamic binding operation.<br/></param>
        /// <returns>New dynamic binding metadata for this wrapper expression.<br/></returns>
        public DynamicMetaObject GetMetaObject(Expression parameter)
            => new InhetoMetaObject(parameter, this);


        IEnumerator IEnumerable.GetEnumerator()
        {
            return elements.GetEnumerator();
        }
    }
    /// <summary>
    /// Exposes encoded dictionary keys and navigable value wrappers through a read-only IDictionary surface. Mutation and ICollection.CopyTo are not implemented.<br/>
    /// </summary>
    public class InhetoKvp : IInhetoObjectEx, IDictionary
    {
        private ReadOnlyDictionary<string, IInhetoObject?>? properties;
        private readonly InhetoBinary inhetoBinary;
        private readonly uint entryIndex;
        private Dictionary<object, IInhetoObject?> elements;
        internal InhetoKvp(InhetoBinary inhetoBinary, NameHeaderEntry entry)
        {
            this.inhetoBinary = inhetoBinary;
            entryIndex = entry.Index;
            InhetoName = entry.Name;
            InhetoPropPath = entry.PropPath;
            InhetoTypeInfo = entry.GetShape();
            elements = new Dictionary<object, IInhetoObject?>();

            switch (entry.Type.CollectionInfo.CollectionType)
            {
                case InhetoCollectionTypes.Dictionary:
                case InhetoCollectionTypes.ConcurrentDictionary:
                case InhetoCollectionTypes.SortedDictionary:
                case InhetoCollectionTypes.SortedList:
                case InhetoCollectionTypes.StringKeyDictionary:
                case InhetoCollectionTypes.StringKeyConcurrentDictionary:
                case InhetoCollectionTypes.StringKeySortedDictionary:
                case InhetoCollectionTypes.StringKeySortedList:
                    {
                        Type? tType = InhetoToTypeResolver.Resolve(entry);
                        if (tType is null) tType = typeof(Dictionary<object, object>);
                        if (!tType.IsAssignableTo(typeof(IDictionary))) throw new InvalidCastException($"The type T ({tType.FullName}) isn't an IDictionary based type");
                        var dt = UtilsAndExtensions.GetDictTypes(tType);

                        var node = InhetoBinary.NextDictionaryEntry(entry, true);
                        var keyShape = entry.Type.CollectionInfo.KeyType;
                        for (uint i = 0; i < entry.ValueOffset; i++)
                        {
                            if (node.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            bool formatted = node.Name[0] == '!';
                            object key = formatted ? InhetoBinary.ParseDictionaryKey(node.CleanName, in keyShape)
                                : inhetoBinary.ReadProp($"{node.PropPath}.Key", inhetoBinary.deserializationOptions)
                                    ?? throw new InvalidDataException("Dictionary entry has a missing or null key.");
                            if (key is InhetoObject<object> { Value: null })
                                throw new InvalidDataException("Dictionary key deserializer returned null.");
                            elements.Add(key, inhetoBinary.ReadProp(formatted ? node.PropPath : $"{node.PropPath}.Value", inhetoBinary.deserializationOptions));
                            node = InhetoBinary.NextDictionaryEntry(node, false);
                        }
                        if (!node.IsEmpty) throw new InvalidDataException("Dictionary has more encoded entries than its declared count.");
                    }
                    break;

            }
        }

        /// <summary>
        /// Gets the read-only child-property map, lazily built for complex/collection views. Values remain navigable wrappers and the map does not imply deep immutability or thread-safe lazy initialization.<br/>
        /// </summary>
        public ReadOnlyDictionary<string, IInhetoObject?> Properties => properties ??= BuildProperties();

        private ReadOnlyDictionary<string, IInhetoObject?> BuildProperties()
        {
            var names = inhetoBinary.GetAllNames();
            if (names.Count == 0) return InhetoObjectProperties.Empty;

            var props = new Dictionary<string, IInhetoObject?>();
            foreach (var name in from name in names.Values where name.ParentIndex == entryIndex && name.Name != "" && name.Name[0] == '.' select name)
            {
                props.Add(name.CleanName, inhetoBinary.ReadProp(name.PropPath, inhetoBinary.deserializationOptions));
            }
            return props.Count == 0 ? InhetoObjectProperties.Empty : props.AsReadOnly();
        }

        /// <summary>
        /// Gets the encoded member token, including its property, key or collection-element prefix when present.<br/>
        /// </summary>
        public string InhetoName { get; }

        /// <summary>
        /// Gets the navigable Inheto path identifying this value in its source payload.<br/>
        /// </summary>
        public string InhetoPropPath { get; }

        /// <summary>Read-only shape view with the same borrowed payload lifetime as this output.<br/></summary>
        public InhetoShape InhetoTypeInfo { get; }

        /// <summary>
        /// Gets true because this view does not support changing its element/entry count.<br/>
        /// </summary>
        public bool IsFixedSize => true;

        /// <summary>
        /// Gets true because dictionary-view mutation is not supported.<br/>
        /// </summary>
        public bool IsReadOnly => true;

        /// <summary>
        /// Gets the materialized dictionary key collection; no new key array is created by this accessor.<br/>
        /// </summary>
        public ICollection Keys => elements.Keys;

        /// <summary>
        /// Gets the dictionary value-wrapper collection; entries may be null and this is not an array of unwrapped CLR values.<br/>
        /// </summary>
        public ICollection Values => elements.Values;

        /// <summary>
        /// Gets the number of materialized array elements or dictionary entries represented by this view.<br/>
        /// </summary>
        public int Count => elements.Count;

        /// <summary>
        /// Gets the underlying collection synchronization flag; no automatic locking is added by this wrapper.<br/>
        /// </summary>
        public bool IsSynchronized => ((ICollection)elements).IsSynchronized;

        /// <summary>
        /// Gets the underlying collection synchronization object for caller-managed coordination; accessing it does not acquire a lock.<br/>
        /// </summary>
        public object SyncRoot => ((ICollection)elements).SyncRoot;

        /// <summary>
        /// Gets the value wrapper for an existing materialized key; missing keys throw KeyNotFoundException. Assignment throws NotImplementedException.<br/>
        /// </summary>
        /// <param name="key">Dictionary key interpreted using the current dictionary equality semantics.<br/></param>
        /// <returns>Existing element or value wrapper at the supplied key/coordinates; invalid positions or keys throw.<br/></returns>
        public object? this[object key] { get => elements[key]; set => throw new NotImplementedException(); }

        /// <summary>
        /// Creates the dynamic binder for child-property navigation on this wrapper; this does not reconstruct the root object.<br/>
        /// </summary>
        /// <param name="parameter">Expression representing this wrapper in the dynamic binding operation.<br/></param>
        /// <returns>New dynamic binding metadata for this wrapper expression.<br/></returns>
        public DynamicMetaObject GetMetaObject(Expression parameter)
            => new InhetoMetaObject(parameter, this);

        void IDictionary.Add(object key, object? value)
        {
            throw new NotImplementedException();
        }

        void IDictionary.Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tests whether the materialized key-view dictionary contains the supplied key using its current dictionary equality semantics.<br/>
        /// </summary>
        /// <param name="key">Dictionary key interpreted using the current dictionary equality semantics.<br/></param>
        /// <returns>True when the documented predicate is satisfied; otherwise false.<br/></returns>
        public bool Contains(object key)
        {
            return elements.ContainsKey(key);
        }

        /// <summary>
        /// Returns dictionary-entry enumeration over keys and navigable value wrappers, not unwrapped CLR values.<br/>
        /// </summary>
        /// <returns>Enumerator over the existing materialized elements or dictionary entries.<br/></returns>
        public IDictionaryEnumerator GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            throw new NotImplementedException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    /// <summary>
    /// Wraps a materialized typed value together with borrowed source name and shape metadata. Value is not cloned when accessed.<br/>
    /// </summary>
    /// <typeparam name="T">Materialized element or value CLR type.<br/></typeparam>
    public class InhetoObject<T> : IInhetoObjectEx
    {
        private ReadOnlyDictionary<string, IInhetoObject?> properties;
        internal InhetoObject(InhetoBinary inhetoBinary, NameHeaderEntry nameEntry)
        {
            properties = InhetoObjectProperties.Empty;
            InhetoName = nameEntry.Name;
            InhetoPropPath = nameEntry.PropPath;
            InhetoTypeInfo = nameEntry.GetShape();
            Value = inhetoBinary.ReadPropAs<T>(nameEntry, inhetoBinary.deserializationOptions);
        }

        /// <summary>
        /// Gets the empty declared-child-property map for this typed value wrapper; access the materialized value through Value.<br/>
        /// </summary>
        public ReadOnlyDictionary<string, IInhetoObject?> Properties => properties;

        /// <summary>
        /// Gets the encoded member token, including its property, key or collection-element prefix when present.<br/>
        /// </summary>
        public string InhetoName { get; }

        /// <summary>
        /// Gets the navigable Inheto path identifying this value in its source payload.<br/>
        /// </summary>
        public string InhetoPropPath { get; }

        /// <summary>Read-only shape view with the same borrowed payload lifetime as this output.<br/></summary>
        public InhetoShape InhetoTypeInfo { get; }

        /// <summary>
        /// Creates the dynamic binder for child-property navigation on this wrapper; this does not reconstruct the root object.<br/>
        /// </summary>
        /// <param name="parameter">Expression representing this wrapper in the dynamic binding operation.<br/></param>
        /// <returns>New dynamic binding metadata for this wrapper expression.<br/></returns>
        public DynamicMetaObject GetMetaObject(Expression parameter)
            => new InhetoMetaObject(parameter, this);

        /// <summary>
        /// Gets the materialized CLR value held by this wrapper without cloning it. Mutable values remain mutable; borrowed shape metadata has a separate source-payload lifetime.<br/>
        /// </summary>
        public T? Value { get; }


        // implicit cast
        /// <summary>
        /// Returns the wrapper&apos;s existing Value without a clone or additional materialization; a null wrapper is not accepted.<br/>
        /// </summary>
        /// <param name="inhetoObject">Wrapper whose existing Value is returned without cloning.<br/></param>
        /// <returns>Wrapped descriptor or existing materialized Value, without cloning the value.<br/></returns>
        public static implicit operator T(InhetoObject<T> inhetoObject) => inhetoObject.Value!;




    }

    /// <summary>
    /// Binds dynamic reads to child-property wrappers. Missing dynamic members throw; dynamic assignment is rejected rather than modifying serialized bytes.<br/>
    /// </summary>
    public sealed class InhetoMetaObject : DynamicMetaObject
    {
        /// <summary>
        /// Creates dynamic binding metadata for a navigable wrapper; member binding is prepared when requested.<br/>
        /// </summary>
        /// <param name="parameter">Expression representing this wrapper in the dynamic binding operation.<br/></param>
        /// <param name="value">Navigable wrapper whose child-property map supplies dynamic members.<br/></param>
        public InhetoMetaObject(Expression parameter, IInhetoObjectEx value)
            : base(parameter, BindingRestrictions.Empty, value) { }

        /// <summary>
        /// Enumerates the child-property names exposed by this wrapper. Resolving a lazy property map may navigate the source payload.<br/>
        /// </summary>
        /// <returns>Enumerable of currently exposed child-property names.<br/></returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var self = (IInhetoObjectEx)Value!;
            return self.Properties.Keys;
        }

        /// <summary>
        /// Binds a dynamic member read to the matching child wrapper, not its unwrapped Value. The resulting expression throws MissingMemberException for an absent member.<br/>
        /// </summary>
        /// <param name="binder">Dynamic get/set binder carrying the requested member name and language binding information.<br/></param>
        /// <returns>Binding whose result is a child wrapper and whose missing-member branch throws.<br/></returns>
        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            var self = Expression.Convert(Expression, LimitType);
            var getProps = Expression.Property(self, nameof(IInhetoObjectEx.Properties));

            var propsType = getProps.Type; // ReadOnlyDictionary<string, IInhetoObject?>
            var tryGet = propsType.GetMethod("TryGetValue")!;

            var keyExpr = Expression.Constant(binder.Name);
            var outVar = Expression.Variable(tryGet.GetParameters()[1].ParameterType.GetElementType()!, "tmp");

            var call = Expression.Call(getProps, tryGet, keyExpr, outVar);

            var boxed = Expression.Convert(outVar, typeof(object));

            var block = Expression.Block(
                new[] { outVar },
                Expression.Condition(
                    call,
                    boxed,
                    Expression.Throw(
                        Expression.New(
                            typeof(MissingMemberException).GetConstructor(new[] { typeof(string) })!,
                            Expression.Constant($"Property '{binder.Name}' not found.")
                        ),
                        typeof(object))
                )
            );

            return new DynamicMetaObject(block,
                BindingRestrictions.GetTypeRestriction(Expression, LimitType));
        }




        /// <summary>
        /// Binds dynamic assignment to an InvalidOperationException because the navigable child-property view is read-only.<br/>
        /// </summary>
        /// <param name="binder">Dynamic get/set binder carrying the requested member name and language binding information.<br/></param>
        /// <param name="value">Incoming dynamic assignment value; the assignment is rejected instead of storing it.<br/></param>
        /// <returns>Binding that rejects the assignment with InvalidOperationException.<br/></returns>
        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            // optional: allow writing back into Properties
            var self = Expression.Convert(Expression, LimitType);
            var getProps = Expression.Property(self, nameof(IInhetoObjectEx.Properties));

            var indexer = typeof(IReadOnlyDictionary<string, object?>).GetProperty("Item");
            // ReadOnlyDictionary won't support set, so you may skip or throw
            var expr = Expression.Throw(Expression.New(typeof(InvalidOperationException)
                .GetConstructor(new[] { typeof(string) })!,
                Expression.Constant("Properties are read-only")), typeof(object));

            return new DynamicMetaObject(expr,
                BindingRestrictions.GetTypeRestriction(Expression, LimitType));
        }
    }

}

