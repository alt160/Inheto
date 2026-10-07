using System.Collections;
using System.Reflection;
using Fasterflect;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text.RegularExpressions;

namespace Inheto
{
    /// <summary>
    /// Contains shared stream encoding, generic-family analysis and CLR-member utility helpers used by Inheto.<br/>
    /// </summary>
    public static class UtilsAndExtensions
    {
        /// <summary>
        /// Returns a dictionary value, or invokes the factory and assigns its result when the key is absent. This IDictionary helper is not atomic and adds no locking.<br/>
        /// </summary>
        /// <typeparam name="TKey">Dictionary key type.<br/></typeparam>
        /// <typeparam name="TValue">Dictionary value type.<br/></typeparam>
        /// <param name="dict">Dictionary to query and update; the caller supplies any required synchronization.<br/></param>
        /// <param name="key">Dictionary key interpreted using the current dictionary equality semantics.<br/></param>
        /// <param name="factory">Value factory invoked only when the key is absent; factory exceptions propagate.<br/></param>
        /// <returns>Converted or retrieved value, according to the documented operation; no blanket exception suppression is performed.<br/></returns>
        public static TValue GetOrAdd<TKey, TValue>(
            this IDictionary<TKey, TValue> dict,
            TKey key,
            Func<TKey, TValue> factory)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            if (!dict.TryGetValue(key, out var value))
            {
                value = factory(key);
                dict[key] = value;
            }
            return value;
        }

        /// <summary>
        /// Writes a ZigZag-mapped signed integer in seven-bit continuation groups; this differs from BinaryReader/BinaryWriter signed 7-bit encoding. Advances the current stream position and leaves the stream open.<br/>
        /// </summary>
        /// <param name="stream">Stream at the current read/write position; this helper advances it and leaves it open.<br/></param>
        /// <param name="value">Integer to encode with the documented continuation/ZigZag representation.<br/></param>
        public static void Write7BitEncodedInt(this Stream stream, int value)
        {
            uint zigzag = (uint)((value << 1) ^ (value >> 31));
            while (zigzag >= 0x80)
            {
                stream.WriteByte((byte)(zigzag | 0x80));
                zigzag >>= 7;
            }
            stream.WriteByte((byte)zigzag);
        }

        /// <summary>
        /// Reads a full-range ZigZag-mapped signed 32-bit integer in at most five seven-bit continuation groups; this differs from BinaryReader/BinaryWriter signed 7-bit encoding.<br/>
        /// Advances the stream and leaves it open. Redundant leading zero groups are accepted within the width limit; overflow and excessive continuation are rejected.<br/>
        /// </summary>
        /// <param name="stream">Stream at the current read/write position; this helper advances it and leaves it open.<br/></param>
        /// <returns>Decoded signed integer in the complete Int32 range.<br/></returns>
        /// <exception cref="EndOfStreamException">The stream ends before the integer terminates.<br/></exception>
        /// <exception cref="FormatException">The encoded integer exceeds 32 bits or five bytes.<br/></exception>
        public static int Read7BitEncodedInt(this Stream stream)
        {
            uint result = 0;
            for (int shift = 0; ; shift += 7)
            {
                int b = stream.ReadByte();
                if (b < 0) throw new EndOfStreamException("Incomplete 7-bit encoded Int32.");
                if (shift == 28 && b > 0x0F)
                    throw new FormatException("Invalid final byte for 7-bit encoded Int32.");
                result |= (uint)(b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                    return (int)(result >> 1) ^ -(int)(result & 1);
            }
        }

        /// <summary>
        /// Writes a ZigZag-mapped signed integer in seven-bit continuation groups; this differs from BinaryReader/BinaryWriter signed 7-bit encoding. Advances the current stream position and leaves the stream open.<br/>
        /// </summary>
        /// <param name="stream">Stream at the current read/write position; this helper advances it and leaves it open.<br/></param>
        /// <param name="value">Integer to encode with the documented continuation/ZigZag representation.<br/></param>
        public static void Write7BitEncodedInt64(this Stream stream, long value)
        {
            ulong zigzag = (ulong)((value << 1) ^ (value >> 63));
            while (zigzag >= 0x80)
            {
                stream.WriteByte((byte)(zigzag | 0x80));
                zigzag >>= 7;
            }
            stream.WriteByte((byte)zigzag);
        }

        /// <summary>
        /// Reads a full-range ZigZag-mapped signed 64-bit integer in at most ten seven-bit continuation groups; this differs from BinaryReader/BinaryWriter signed 7-bit encoding.<br/>
        /// Advances the stream and leaves it open. Redundant leading zero groups are accepted within the width limit; overflow and excessive continuation are rejected.<br/>
        /// </summary>
        /// <param name="stream">Stream at the current read/write position; this helper advances it and leaves it open.<br/></param>
        /// <returns>Decoded signed integer in the complete Int64 range.<br/></returns>
        /// <exception cref="EndOfStreamException">The stream ends before the integer terminates.<br/></exception>
        /// <exception cref="FormatException">The encoded integer exceeds 64 bits or ten bytes.<br/></exception>
        public static long Read7BitEncodedInt64(this Stream stream)
        {
            ulong result = 0;
            for (int shift = 0; ; shift += 7)
            {
                int b = stream.ReadByte();
                if (b < 0) throw new EndOfStreamException("Incomplete 7-bit encoded Int64.");
                if (shift == 63 && b > 0x01)
                    throw new FormatException("Invalid final byte for 7-bit encoded Int64.");
                result |= (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                    return (long)(result >> 1) ^ -(long)(result & 1);
            }
        }

        /// <summary>
        /// Writes a unsigned integer in seven-bit continuation groups. Advances the current stream position and leaves the stream open.<br/>
        /// </summary>
        /// <param name="stream">Stream at the current read/write position; this helper advances it and leaves it open.<br/></param>
        /// <param name="value">Integer to encode with the documented continuation/ZigZag representation.<br/></param>
        public static void Write7BitEncodedUInt(this Stream stream, uint value)
        {
            while (value >= 0x80)
            {
                stream.WriteByte((byte)(value | 0x80));
                value >>= 7;
            }
            stream.WriteByte((byte)value);
        }

        /// <summary>
        /// Reads a full-range unsigned 32-bit integer in at most five seven-bit continuation groups.<br/>
        /// Advances the stream and leaves it open. Redundant leading zero groups are accepted within the width limit; overflow and excessive continuation are rejected.<br/>
        /// </summary>
        /// <param name="stream">Stream at the current read/write position; this helper advances it and leaves it open.<br/></param>
        /// <returns>Decoded unsigned integer in the complete UInt32 range.<br/></returns>
        /// <exception cref="EndOfStreamException">The stream ends before the integer terminates.<br/></exception>
        /// <exception cref="FormatException">The encoded integer exceeds 32 bits or five bytes.<br/></exception>
        public static uint Read7BitEncodedUInt(this Stream stream)
        {
            uint result = 0;
            for (int shift = 0; ; shift += 7)
            {
                int b = stream.ReadByte();
                if (b < 0) throw new EndOfStreamException("Incomplete 7-bit encoded UInt32.");
                if (shift == 28 && b > 0x0F)
                    throw new FormatException("Invalid final byte for 7-bit encoded UInt32.");
                result |= (uint)(b & 0x7F) << shift;
                if ((b & 0x80) == 0) return result;
            }
        }

        /// <summary>
        /// Writes a unsigned integer in seven-bit continuation groups. Advances the current stream position and leaves the stream open.<br/>
        /// </summary>
        /// <param name="stream">Stream at the current read/write position; this helper advances it and leaves it open.<br/></param>
        /// <param name="value">Integer to encode with the documented continuation/ZigZag representation.<br/></param>
        public static void Write7BitEncodedUInt64(this Stream stream, ulong value)
        {
            while (value >= 0x80)
            {
                stream.WriteByte((byte)(value | 0x80));
                value >>= 7;
            }
            stream.WriteByte((byte)value);
        }

        /// <summary>
        /// Reads a full-range unsigned 64-bit integer in at most ten seven-bit continuation groups.<br/>
        /// Advances the stream and leaves it open. Redundant leading zero groups are accepted within the width limit; overflow and excessive continuation are rejected.<br/>
        /// </summary>
        /// <param name="stream">Stream at the current read/write position; this helper advances it and leaves it open.<br/></param>
        /// <returns>Decoded unsigned integer in the complete UInt64 range.<br/></returns>
        /// <exception cref="EndOfStreamException">The stream ends before the integer terminates.<br/></exception>
        /// <exception cref="FormatException">The encoded integer exceeds 64 bits or ten bytes.<br/></exception>
        public static ulong Read7BitEncodedUInt64(this Stream stream)
        {
            ulong result = 0;
            for (int shift = 0; ; shift += 7)
            {
                int b = stream.ReadByte();
                if (b < 0) throw new EndOfStreamException("Incomplete 7-bit encoded UInt64.");
                if (shift == 63 && b > 0x01)
                    throw new FormatException("Invalid final byte for 7-bit encoded UInt64.");
                result |= (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) == 0) return result;
            }
        }

        /// <summary>
        /// Writes a unsigned integer in seven-bit continuation groups. Advances the current stream position and leaves the stream open.<br/>
        /// </summary>
        /// <param name="stream">Stream at the current read/write position; this helper advances it and leaves it open.<br/></param>
        /// <param name="value">Integer to encode with the documented continuation/ZigZag representation.<br/></param>
        public static void Write7BitEncodedUInt128(this Stream stream, UInt128 value)
        {
            while (value >= 0x80)
            {
                stream.WriteByte((byte)(value | 0x80));
                value >>= 7;
            }
            stream.WriteByte((byte)value);
        }

        /// <summary>
        /// Reads a full-range unsigned 128-bit integer in at most nineteen seven-bit continuation groups.<br/>
        /// Advances the stream and leaves it open. Redundant leading zero groups are accepted within the width limit; overflow and excessive continuation are rejected.<br/>
        /// </summary>
        /// <param name="stream">Stream at the current read/write position; this helper advances it and leaves it open.<br/></param>
        /// <returns>Decoded unsigned integer in the complete UInt128 range.<br/></returns>
        /// <exception cref="EndOfStreamException">The stream ends before the integer terminates.<br/></exception>
        /// <exception cref="FormatException">The encoded integer exceeds 128 bits or nineteen bytes.<br/></exception>
        public static UInt128 Read7BitEncodedUInt128(this Stream stream)
        {
            UInt128 result = 0;
            for (int shift = 0; ; shift += 7)
            {
                int b = stream.ReadByte();
                if (b < 0) throw new EndOfStreamException("Incomplete 7-bit encoded UInt128.");
                if (shift == 126 && b > 0x03)
                    throw new FormatException("Invalid final byte for 7-bit encoded UInt128.");
                result |= (UInt128)(b & 0x7F) << shift;
                if ((b & 0x80) == 0) return result;
            }
        }

        /// <summary>
        /// Writes a ZigZag-mapped signed integer in seven-bit continuation groups; this differs from BinaryReader/BinaryWriter signed 7-bit encoding. Advances the current stream position and leaves the stream open.<br/>
        /// </summary>
        /// <param name="stream">Stream at the current read/write position; this helper advances it and leaves it open.<br/></param>
        /// <param name="value">Integer to encode with the documented continuation/ZigZag representation.<br/></param>
        public static void Write7BitEncodedInt128(this Stream stream, Int128 value)
        {
            UInt128 zigzag = (UInt128)((value << 1) ^ (value >> 127));
            while (zigzag >= 0x80)
            {
                stream.WriteByte((byte)(zigzag | 0x80));
                zigzag >>= 7;
            }
            stream.WriteByte((byte)zigzag);
        }

        /// <summary>
        /// Reads a full-range ZigZag-mapped signed 128-bit integer in at most nineteen seven-bit continuation groups; this differs from BinaryReader/BinaryWriter signed 7-bit encoding.<br/>
        /// Advances the stream and leaves it open. Redundant leading zero groups are accepted within the width limit; overflow and excessive continuation are rejected.<br/>
        /// </summary>
        /// <param name="stream">Stream at the current read/write position; this helper advances it and leaves it open.<br/></param>
        /// <returns>Decoded signed integer in the complete Int128 range.<br/></returns>
        /// <exception cref="EndOfStreamException">The stream ends before the integer terminates.<br/></exception>
        /// <exception cref="FormatException">The encoded integer exceeds 128 bits or nineteen bytes.<br/></exception>
        public static Int128 Read7BitEncodedInt128(this Stream stream)
        {
            UInt128 result = 0;
            for (int shift = 0; ; shift += 7)
            {
                int b = stream.ReadByte();
                if (b < 0) throw new EndOfStreamException("Incomplete 7-bit encoded Int128.");
                if (shift == 126 && b > 0x03)
                    throw new FormatException("Invalid final byte for 7-bit encoded Int128.");
                result |= (UInt128)(b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                    return (Int128)(result >> 1) ^ -(Int128)(result & 1);
            }
        }


        /// <summary>
        /// Identifies recognized generic nullable and collection families, including compact string-key variants and their offset constant.<br/>
        /// </summary>
        public enum GenericFamily : byte
        {
            /// <summary>
            /// No representation or comparer selected in this descriptor family.<br/>
            /// </summary>
            None = 0,
            /// <summary>
            /// Nullable representation route.<br/>
            /// </summary>
            Nullable,               // Nullable<T>

            // list types
            /// <summary>
            /// Identifies the List representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
            /// </summary>
            List,                   // List<T>
            /// <summary>
            /// Identifies the Queue representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
            /// </summary>
            Queue,                  // Queue<T>
            /// <summary>
            /// Identifies the ConcurrentQueue representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
            /// </summary>
            ConcurrentQueue,        // ConcurrentQueue<T>.
            /// <summary>
            /// Identifies the Stack representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
            /// </summary>
            Stack,                  // Stack<T>
            /// <summary>
            /// Identifies the ConcurrentStack representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
            /// </summary>
            ConcurrentStack,        // ConcurrentStack<T>.
            /// <summary>
            /// Identifies the ObservableCollection representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
            /// </summary>
            ObservableCollection,   // ObservableCollection<T>.  
            /// <summary>
            /// Identifies the ConcurrentBag representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
            /// </summary>
            ConcurrentBag,          // ConcurrentBag<T>.  
            /// <summary>
            /// Identifies the ReadOnlyCollection representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
            /// </summary>
            ReadOnlyCollection,     // ReadOnlyCollection<T>.

            // set types
            /// <summary>
            /// Identifies the SortedSet representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
            /// </summary>
            SortedSet,              // SortedSet<T>
            /// <summary>
            /// Identifies the HashSet representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
            /// </summary>
            HashSet,                // HashSet<T>

            // dictionary types
            /// <summary>
            /// Identifies the Dictionary representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
            /// </summary>
            Dictionary,             // Dictionary<TKey, TValue>.  a 3rd byte is used after this one to indicate the TValue type of the dictionary
            /// <summary>
            /// Identifies the ConcurrentDictionary representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
            /// </summary>
            ConcurrentDictionary,   // ConcurrentDictionary<TKey, TValue>.  a 3rd byte is used after this one to indicate the TValue type of the dictionary
            /// <summary>
            /// Identifies the SortedDictionary representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
            /// </summary>
            SortedDictionary,       // SortedDictionary<TKey, TValue>.  a 3rd byte is used after this one to indicate the TValue type of the dictionary
            /// <summary>
            /// Identifies the SortedList representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
            /// </summary>
            SortedList,             // SortedList<TKey, TValue>.  a 3rd byte is used after this one to indicate the TValue type of the dictionary

            /// <summary>
            /// Compact Dictionary family identifier for string keys; values retain their own type descriptor.<br/>
            /// </summary>
            StringKeyDictionary,  // Dictionary<String, TValue>.  prior byte is TValue type
            /// <summary>
            /// Compact ConcurrentDictionary family identifier for string keys; values retain their own type descriptor.<br/>
            /// </summary>
            StringKeyConcurrentDictionary, // ConcurrentDictionary<String, TValue>.  prior byte is TValue type
            /// <summary>
            /// Compact SortedDictionary family identifier for string keys; values retain their own type descriptor.<br/>
            /// </summary>
            StringKeySortedDictionary, // SortedDictionary<String, TValue>.  prior byte is TValue type
            /// <summary>
            /// Compact SortedList family identifier for string keys; values retain their own type descriptor.<br/>
            /// </summary>
            StringKeySortedList,  // SortedList<String, TValue>.  prior byte is TValue type

            /// <summary>
            /// Identifier offset between the ordinary and compact string-key dictionary family ranges.<br/>
            /// </summary>
            StringKeyEnumOffset = StringKeyDictionary - Dictionary,
        }

        /// <summary>
        /// Describes the closed generic family recognized on a CLR type or its ancestry; a default value represents no populated match.<br/>
        /// </summary>
        public readonly struct GenericMatch
        {
            /// <summary>
            /// Gets the recognized generic family, or None for an unpopulated match.<br/>
            /// </summary>
            public GenericFamily Family { get; init; }
            /// <summary>
            /// Gets the matched closed generic CLR type; a default match has no populated type.<br/>
            /// </summary>
            public Type ClosedType { get; init; }      // the ancestor/interface that matched (closed)
            /// <summary>
            /// Gets the first generic argument, normally the element type or dictionary key type.<br/>
            /// </summary>
            public Type? T0 { get; init; }             // inner/element/key
            /// <summary>
            /// Gets the second generic argument, normally a dictionary value type; null for a one-argument family.<br/>
            /// </summary>
            public Type? T1 { get; init; }             // value (for Dictionary<,>)
            /// <summary>
            /// Gets the number of generic arguments populated by this match.<br/>
            /// </summary>
            public byte Arity { get; init; }           // 0..2 for now
            /// <summary>
            /// Gets whether the recognized family was obtained from an implemented interface rather than a concrete ancestor.<br/>
            /// </summary>
            public bool FromInterface { get; init; }   // true if the match came via an interface
        }

        /// <summary>
        /// Inspects generic ancestry and recognized collection families using cached CLR type metadata.<br/>
        /// </summary>
        public static class GenericAnalyzer
        {
            /// <remarks>
            /// Single-source generic shape detection.<br/>
            /// Returns true when the type matches a known family and fills a small, fixed-arity descriptor.<br/>
            /// Extend by adding more family probes (e.g., SortedDictionary&lt;,&gt;, IReadOnlyList&lt;&gt;, etc.).<br/>
            /// <example>
            /// <code>
            /// if (GenericAnalyzer.TryAnalyzeForGenericType(t, out var m))
            ///     switch (m.Family) { /* List/Dictionary/Nullable handling */ }
            /// </code>
            /// </example>
            /// </remarks>
            /// <param name="sourceType">Type to inspect; null is supported and returns false with a default match.<br/></param>
            /// <param name="match">Matched generic family metadata, or default when the type is absent or unsupported.<br/></param>
            /// <returns>True for a supported generic family; false for null or a nonmatching type.<br/></returns>
            public static bool TryAnalyzeForGenericType(Type? sourceType, out GenericMatch match)
            {
                match = default;
                if (sourceType is null) return false;

                // 1) Nullable<T> — unwrap early; preserves inner metadata in your FromType by marking IsNullable
                var u = Nullable.GetUnderlyingType(sourceType);
                if (u is not null)
                {
                    match = new GenericMatch
                    {
                        Family = GenericFamily.Nullable,
                        ClosedType = sourceType,
                        T0 = u,
                        Arity = 1
                    };
                    return true;
                }

                Type? ancestorType = null;

                // 2) List<T> (exact or subclass) — class chain only
                if (TryGetGenericAncestorType(sourceType, typeof(List<>), out ancestorType, includeInterfaces: false, includeSelf: true))
                {
                    var elem = ancestorType!.GenericTypeArguments[0]; // closed arg
                    match = new GenericMatch
                    {
                        Family = GenericFamily.List,
                        ClosedType = ancestorType,
                        T0 = elem,
                        Arity = 1,
                        FromInterface = false
                    };
                    return true;
                }

                // 3) Dictionary<TKey,TValue> (exact or subclass) — class chain only
                if (TryGetGenericAncestorType(sourceType, typeof(Dictionary<,>), out ancestorType, includeInterfaces: false, includeSelf: true))
                {
                    var args = ancestorType!.GenericTypeArguments; // closed args (alloc once; you cache results anyway)
                    match = new GenericMatch
                    {
                        Family = GenericFamily.Dictionary,
                        ClosedType = ancestorType,
                        T0 = args[0],  // key
                        T1 = args[1],  // value
                        Arity = 2,
                        FromInterface = false
                    };
                    return true;
                }

                if (TryGetGenericAncestorType(sourceType, typeof(HashSet<>), out ancestorType, includeInterfaces: false, includeSelf: true))
                {
                    var elem = ancestorType!.GenericTypeArguments[0]; // closed arg
                    match = new GenericMatch
                    {
                        Family = GenericFamily.HashSet,
                        ClosedType = ancestorType,
                        T0 = elem,
                        Arity = 1,
                        FromInterface = false
                    };
                    return true;
                }

                if (TryGetGenericAncestorType(sourceType, typeof(SortedSet<>), out ancestorType, includeInterfaces: false, includeSelf: true))
                {
                    var elem = ancestorType!.GenericTypeArguments[0]; // closed arg
                    match = new GenericMatch
                    {
                        Family = GenericFamily.SortedSet,
                        ClosedType = ancestorType,
                        T0 = elem,
                        Arity = 1,
                        FromInterface = false
                    };
                    return true;
                }

                if (TryGetGenericAncestorType(sourceType, typeof(Queue<>), out ancestorType, includeInterfaces: false, includeSelf: true))
                {
                    var elem = ancestorType!.GenericTypeArguments[0]; // closed arg
                    match = new GenericMatch
                    {
                        Family = GenericFamily.Queue,
                        ClosedType = ancestorType,
                        T0 = elem,
                        Arity = 1,
                        FromInterface = false
                    };
                    return true;
                }

                if (TryGetGenericAncestorType(sourceType, typeof(ConcurrentQueue<>), out ancestorType, includeInterfaces: false, includeSelf: true))
                {
                    var elem = ancestorType!.GenericTypeArguments[0]; // closed arg
                    match = new GenericMatch
                    {
                        Family = GenericFamily.ConcurrentQueue,
                        ClosedType = ancestorType,
                        T0 = elem,
                        Arity = 1,
                        FromInterface = false
                    };
                    return true;
                }

                if (TryGetGenericAncestorType(sourceType, typeof(Stack<>), out ancestorType, includeInterfaces: false, includeSelf: true))
                {
                    var elem = ancestorType!.GenericTypeArguments[0]; // closed arg
                    match = new GenericMatch
                    {
                        Family = GenericFamily.Stack,
                        ClosedType = ancestorType,
                        T0 = elem,
                        Arity = 1,
                        FromInterface = false
                    };
                    return true;
                }

                if (TryGetGenericAncestorType(sourceType, typeof(ConcurrentStack<>), out ancestorType, includeInterfaces: false, includeSelf: true))
                {
                    var elem = ancestorType!.GenericTypeArguments[0]; // closed arg
                    match = new GenericMatch
                    {
                        Family = GenericFamily.ConcurrentStack,
                        ClosedType = ancestorType,
                        T0 = elem,
                        Arity = 1,
                        FromInterface = false
                    };
                    return true;
                }

                if (TryGetGenericAncestorType(sourceType, typeof(ObservableCollection<>), out ancestorType, includeInterfaces: false, includeSelf: true))
                {
                    var elem = ancestorType!.GenericTypeArguments[0]; // closed arg
                    match = new GenericMatch
                    {
                        Family = GenericFamily.ObservableCollection,
                        ClosedType = ancestorType,
                        T0 = elem,
                        Arity = 1,
                        FromInterface = false
                    };
                    return true;
                }

                if (TryGetGenericAncestorType(sourceType, typeof(ConcurrentBag<>), out ancestorType, includeInterfaces: false, includeSelf: true))
                {
                    var elem = ancestorType!.GenericTypeArguments[0]; // closed arg
                    match = new GenericMatch
                    {
                        Family = GenericFamily.ConcurrentBag,
                        ClosedType = ancestorType,
                        T0 = elem,
                        Arity = 1,
                        FromInterface = false
                    };
                    return true;
                }

                if (TryGetGenericAncestorType(sourceType, typeof(ReadOnlyCollection<>), out ancestorType, includeInterfaces: false, includeSelf: true))
                {
                    var elem = ancestorType!.GenericTypeArguments[0]; // closed arg
                    match = new GenericMatch
                    {
                        Family = GenericFamily.ReadOnlyCollection,
                        ClosedType = ancestorType,
                        T0 = elem,
                        Arity = 1,
                        FromInterface = false
                    };
                    return true;
                }

                if (TryGetGenericAncestorType(sourceType, typeof(ConcurrentDictionary<,>), out ancestorType, includeInterfaces: false, includeSelf: true))
                {
                    var args = ancestorType!.GenericTypeArguments; // closed args (alloc once; you cache results anyway)
                    match = new GenericMatch
                    {
                        Family = GenericFamily.ConcurrentDictionary,
                        ClosedType = ancestorType,
                        T0 = args[0],  // key
                        T1 = args[1],  // value
                        Arity = 2,
                        FromInterface = false
                    };
                    return true;
                }

                if (TryGetGenericAncestorType(sourceType, typeof(SortedDictionary<,>), out ancestorType, includeInterfaces: false, includeSelf: true))
                {
                    var args = ancestorType!.GenericTypeArguments; // closed args (alloc once; you cache results anyway)
                    match = new GenericMatch
                    {
                        Family = GenericFamily.SortedDictionary,
                        ClosedType = ancestorType,
                        T0 = args[0],  // key
                        T1 = args[1],  // value
                        Arity = 2,
                        FromInterface = false
                    };
                    return true;
                }


                if (TryGetGenericAncestorType(sourceType, typeof(SortedList<,>), out ancestorType, includeInterfaces: false, includeSelf: true))
                {
                    var args = ancestorType!.GenericTypeArguments; // closed args (alloc once; you cache results anyway)
                    match = new GenericMatch
                    {
                        Family = GenericFamily.SortedList,
                        ClosedType = ancestorType,
                        T0 = args[0],  // key
                        T1 = args[1],  // value
                        Arity = 2,
                        FromInterface = false
                    };
                    return true;
                }

                // TODO (future): add more families in precedence order, e.g.:
                // - SortedDictionary<,> (class chain)
                // - ReadOnlyDictionary<,> (class chain; hydrate as wrapper)
                // - IReadOnlyList<> / IReadOnlyDictionary<,> (interfaces; set FromInterface=true)
                // - KeyValuePair<,> (arity=2)
                // - ValueTuple<T...> (flatten TRest; use handle-based stackalloc to avoid heap)
                // - Tuple<T...> (reference-type analog)

                return false;
            }

            private static readonly ConcurrentDictionary<(RuntimeTypeHandle, RuntimeTypeHandle, byte),
                                                   (bool found, Type? match)> s_cache = new();

            /// <summary>
            /// Finds the closed ancestor (base class or, optionally, interface) of <paramref name="sourceType"/>
            /// whose generic type definition equals <paramref name="openGenericDefinition"/>.
            /// </summary>
            private static bool TryGetGenericAncestorType(
                Type sourceType,
                Type openGenericDefinition,
                out Type? ancestorType,
                bool includeInterfaces = true,
                bool includeSelf = true)
            {
                ancestorType = null;
                if (sourceType is null || openGenericDefinition is null || !openGenericDefinition.IsGenericTypeDefinition)
                    return false;

                byte flags = (byte)((includeInterfaces ? 1 : 0) | (includeSelf ? 2 : 0));
                var key = (sourceType.TypeHandle, openGenericDefinition.TypeHandle, flags);

                if (s_cache.TryGetValue(key, out var cached))
                {
                    ancestorType = cached.match;
                    return cached.found;
                }

                // Track base-chain types for backfill.
                var visited = new List<Type>(8) { sourceType };

                // Self (only if requested)
                if (includeSelf && sourceType.IsGenericType &&
                    sourceType.GetGenericTypeDefinition() == openGenericDefinition)
                {
                    ancestorType = sourceType;
                    BackfillPositive(visited, ancestorType, includeSelf, flags, openGenericDefinition);
                    return true;
                }

                // Walk bases
                for (var cur = sourceType.BaseType; cur is not null; cur = cur.BaseType)
                {
                    visited.Add(cur);

                    // Early hit via cache for 'cur'
                    var curKey = (cur.TypeHandle, openGenericDefinition.TypeHandle, flags);
                    if (s_cache.TryGetValue(curKey, out var c2))
                    {
                        if (c2.found)
                        {
                            ancestorType = c2.match!;
                            BackfillPositive(visited, ancestorType, includeSelf, flags, openGenericDefinition);
                            return true;
                        }
                        // negative cache for 'cur' doesn't prove sourceType has no interface match; keep going
                    }

                    if (cur.IsGenericType && cur.GetGenericTypeDefinition() == openGenericDefinition)
                    {
                        ancestorType = cur;
                        BackfillPositive(visited, ancestorType, includeSelf, flags, openGenericDefinition);
                        return true;
                    }
                }
                // Interfaces (only cache for the source type; bases may not implement it)
                if (includeInterfaces)
                {
                    foreach (var itf in sourceType.GetInterfaces())
                    {
                        if (itf.IsGenericType && itf.GetGenericTypeDefinition() == openGenericDefinition)
                        {
                            ancestorType = itf;
                            s_cache.TryAdd(key, (true, ancestorType)); // only sourceType entry
                            return true;
                        }
                    }
                }

                // Negative: no base nor interface matches
                foreach (var t in visited)
                {
                    var k = (t.TypeHandle, openGenericDefinition.TypeHandle, flags);
                    s_cache.TryAdd(k, (false, null));
                }
                return false;
            }

            private static void BackfillPositive(List<Type> visited, Type match, bool includeSelf, byte flags, Type openDef)
            {
                foreach (var t in visited)
                {
                    var k = (t.TypeHandle, openDef.TypeHandle, flags);
                    if (!includeSelf && t == match)
                    {
                        // For the exact ancestor when includeSelf==false, the correct answer is "not found".
                        s_cache.TryAdd(k, (false, null));
                    }
                    else
                    {
                        s_cache.TryAdd(k, (true, match));
                    }
                }
            }

        }

        private static ConcurrentDictionary<Type, (Type KeyType, Type ValueType)> _dictTypes = new(); // <T>
        /// <summary>
        /// Retrieves cached dictionary key/value CLR types for the supported concrete/interface branches. An unsupported type returns a default tuple whose type entries are null; this is not a universal generic-argument analyzer.<br/>
        /// </summary>
        /// <typeparam name="T">CLR type used by this operation; result conversion or exact-type registration follows the documented contract.<br/></typeparam>
        /// <returns>Key/value CLR types, or a default tuple with null entries when the supported analyzer does not recognize the type.<br/></returns>
        public static (Type KeyType, Type ValueType) GetDictTypes<T>()
        {
            return GetDictTypes(typeof(T));
        }
        /// <summary>
        /// Retrieves cached dictionary key/value CLR types for the supported concrete/interface branches. An unsupported type returns a default tuple whose type entries are null; this is not a universal generic-argument analyzer.<br/>
        /// </summary>
        /// <param name="t">CLR type whose supported representation or dictionary arguments are inspected.<br/></param>
        /// <returns>Key/value CLR types, or a default tuple with null entries when the supported analyzer does not recognize the type.<br/></returns>
        public static (Type KeyType, Type ValueType) GetDictTypes(Type t)
        {
            if (!_dictTypes.TryGetValue(t, out var result))
            {
                if (t.IsGenericType)
                {
                    var def = t.GetGenericTypeDefinition();
                    if (def == typeof(Dictionary<,>) || def.Implements(typeof(IDictionary<,>)) || def == typeof(SortedDictionary<,>))
                    {
                        var args = t.GetGenericArguments();
                        result = (args[0], args[1]);
                        _dictTypes.TryAdd(t, result);
                    }
                }
                else
                {
                    // check implemented interfaces only if needed
                    result = default!;
                    var iface = t.GetInterfaces()
                                 .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
                    if (iface != null)
                    {
                        var args = iface.GetGenericArguments();
                        result = (args[0], args[1]);
                        _dictTypes.TryAdd(t, result);
                    }
                    _dictTypes.TryAdd(t, result);
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Provides cached CLR property/field/method path resolution and value access; executing a path may invoke developer getters or methods.<br/>
    /// </summary>
    public static class ValuePathRetriever
    {
        private static readonly ConcurrentDictionary<(RuntimeTypeHandle Source, RuntimeTypeHandle Result, string Path, Flags Binding, bool IgnoreCase, string Methods), Delegate> _compiledAccessors = new();

        /// <summary>
        /// Resolves a dotted property/field/method path (e.g. <c>".Customer.Address.ZipCode"</c> or <c>"Customer.TotalAmount()"</c>) against the given object instance.
        /// <br/>
        /// Uses compiled expression trees for full-path access and caches the delegates per type and path.
        /// </summary>
        /// <typeparam name="T">The expected type of the value at the end of the path.</typeparam>
        /// <param name="source">The root object to navigate from.</param>
        /// <param name="propPath">The dotted property path (leading dot optional). Method segments may include or omit "()".</param>
        /// <param name="throwOnError">When <c>true</c>, throws descriptive exceptions if any segment is invalid or inaccessible; otherwise returns <c>default</c>.</param>
        /// <param name="ignoreCase">When <c>true</c>, member lookup is case-insensitive.</param>
        /// <param name="bindingFlags">Member visibility and scope selection (default is <see cref="Flags.InstancePublic"/>).</param>
        /// <param name="includeMethodNames">Specific method names allowed to be invoked when no property or field is found. If null or empty, parameterless methods returning allowed types are considered automatically.</param>
        /// <param name="allowedMethodReturnTypes">Optional per-call whitelist of allowed method return types. Null uses the built-in safe set.</param>
        /// <param name="appendTypesToDefault">When true (default), the provided <paramref name="allowedMethodReturnTypes"/> extend the default safe set; when false, they replace it entirely.</param>
        /// <returns>The resolved value or <c>default</c> if not found and <paramref name="throwOnError"/> is <c>false</c>.</returns>
        public static T? GetValue<T>(
            object source,
            string propPath,
            bool throwOnError = false,
            bool ignoreCase = false,
            Flags bindingFlags = default,
            IEnumerable<string>? includeMethodNames = null,
            IEnumerable<Type>? allowedMethodReturnTypes = null,
            bool appendTypesToDefault = true)
        {
            if (bindingFlags == Flags.None)
                bindingFlags = Flags.InstancePublic;

            if (source is null)
            {
                if (throwOnError)
                    throw new ArgumentNullException(nameof(source));
                return default;
            }

            propPath ??= string.Empty;
            if (propPath.StartsWith('.'))
                propPath = propPath[1..];

            if (string.IsNullOrWhiteSpace(propPath))
                return CastOrDefault<T>(source, throwOnError);

            var type = source.GetType();

            string includeMethodsKey = string.Empty;
            if (includeMethodNames != null && includeMethodNames.Any())
            {
                includeMethodsKey = string.Join("|",
                    includeMethodNames
                        .Where(n => !string.IsNullOrWhiteSpace(n))
                        .Select(n => ignoreCase ? n.Trim().ToLowerInvariant() : n.Trim())
                        .OrderBy(n => n));
            }
            else
            {
                includeMethodsKey = "<auto>"; // sentinel for automatic safe-method lookup
            }

            var key = (
                type.TypeHandle,
                typeof(T).TypeHandle,
                ignoreCase ? propPath.ToLowerInvariant() : propPath,
                bindingFlags,
                ignoreCase,
                includeMethodsKey);

            if (!_compiledAccessors.TryGetValue(key, out var del))
            {
                del = BuildAccessor<T>(type, propPath, throwOnError, ignoreCase, bindingFlags, includeMethodNames, allowedMethodReturnTypes, appendTypesToDefault);
                _compiledAccessors.TryAdd(key, del);
            }

            try
            {
                return ((Func<object, T?>)del)(source);
            }
            catch when (!throwOnError)
            {
                return default;
            }
        }

        private static readonly HashSet<Type> DefaultAllowedMethodReturnTypes = new()
{
    // --- Group 1: Core Text and Boolean ---
    typeof(bool),
    typeof(char),
    typeof(System.Text.Rune),
    typeof(string),

    // --- Group 2b: Base Numerics ---
    typeof(sbyte),
    typeof(byte),
    typeof(short),
    typeof(ushort),
    typeof(int),
    typeof(uint),
    typeof(long),
    typeof(ulong),

    // --- Group 2c: Extended Numerics ---
    typeof(System.Int128),
    typeof(System.UInt128),

    // --- Group 2d: Floating Point Numerics ---
    typeof(Half),
    typeof(float),
    typeof(double),
    typeof(decimal),

    // --- Group 3: Common Value-Like Framework Types ---
    typeof(DateOnly),
    typeof(DateTime),
    typeof(TimeOnly),
    typeof(TimeSpan),
    typeof(DateTimeOffset),
    typeof(Guid),
    typeof(Version),
    typeof(Uri),

    // --- System.Numerics family ---
    typeof(System.Numerics.Vector2),
    typeof(System.Numerics.Vector3),
    typeof(System.Numerics.Vector4),
    typeof(System.Numerics.Complex),
    typeof(System.Numerics.Quaternion),
    typeof(System.Numerics.Plane),
    typeof(System.Numerics.Matrix3x2),
    typeof(System.Numerics.Matrix4x4),
    typeof(System.Numerics.BigInteger),

    // --- Group 4: Promoted Common Use Types ---
    typeof(byte[]),
    typeof(List<string>),

    // Collection and container bases
    typeof(Array),
    typeof(System.Collections.ICollection),

    // Special collection types
    typeof(System.Collections.Specialized.NameValueCollection),
    typeof(System.Collections.Specialized.StringCollection),

    // Chosen complex / contextual types
    typeof(System.Globalization.CultureInfo),
    typeof(System.Net.IPAddress),
    typeof(System.Net.IPEndPoint),
    typeof(System.Net.Mail.MailAddress),

    typeof(System.Text.RegularExpressions.Regex),
    typeof(System.Text.StringBuilder),
    typeof(System.TimeZoneInfo),
    typeof(System.Type)
};

        // ... class + fields unchanged ...

        private static Delegate BuildAccessor<T>(
            Type sourceType,
            string propPath,
            bool throwOnError,
            bool ignoreCase,
            Flags bindingFlags,
            IEnumerable<string>? excludedMethodNames,
            IEnumerable<Type>? allowedMethodReturnTypes,
            bool appendTypesToDefault)
        {
            var param = Expression.Parameter(typeof(object), "src");
            Expression current = Expression.Convert(param, sourceType);

            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            var segments = propPath.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            HashSet<string>? excludedNames = null;
            bool hasExclusions = excludedMethodNames != null && excludedMethodNames.Any();
            if (hasExclusions)
            {
                excludedNames = new HashSet<string>(
                    excludedMethodNames!
                        .Where(n => !string.IsNullOrWhiteSpace(n))
                        .Select(n => NormalizeSegment(n, ignoreCase)),
                    ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
            }

            HashSet<Type> allowedTypes =
                allowedMethodReturnTypes == null
                    ? DefaultAllowedMethodReturnTypes
                    : (appendTypesToDefault
                        ? new HashSet<Type>(DefaultAllowedMethodReturnTypes.Concat(allowedMethodReturnTypes))
                        : new HashSet<Type>(allowedMethodReturnTypes));

            foreach (var rawSeg in segments)
            {
                var seg = rawSeg.Trim();
                var segName = NormalizeSegment(seg, ignoreCase);

                // PROPERTY/FIELD
                if (TryBindPropertyOrField(ref current, ref sourceType, segName, ignoreCase, bindingFlags))
                    continue;

                if (IsNullableValueType(sourceType))
                {
                    var underlying = Nullable.GetUnderlyingType(sourceType)!;
                    current = Expression.Property(current, sourceType.GetProperty("Value")!);
                    sourceType = underlying;

                    if (TryBindPropertyOrField(ref current, ref sourceType, segName, ignoreCase, bindingFlags))
                        continue;
                }

                // METHODS (auto or not excluded)
                bool considerMethods = !hasExclusions || !(excludedNames?.Contains(segName) ?? false);
                if (considerMethods)
                {
                    var candidateMethods = sourceType.Methods(bindingFlags)
                        .Where(m => string.Equals(m.Name, segName, comparison))
                        .Where(m => SafeGetParametersCount(m) == 0)
                        .ToList();

                    MethodInfo? chosen = null;
                    foreach (var m in candidateMethods)
                    {
                        var retType = SafeGetReturnType(m);
                        if (retType == null || retType == typeof(void))
                            continue;

                        bool allowed =
                            allowedTypes.Any(t => t.IsAssignableFrom(retType))
                            || (retType.Namespace?.StartsWith("System.Numerics", StringComparison.Ordinal) ?? false)
                            || retType.IsEnum
                            || retType.IsPrimitive;

                        if (!allowed) continue;

                        chosen = m;
                        break;
                    }

                    if (chosen != null)
                    {
                        current = Expression.Call(current, chosen);
                        sourceType = chosen.ReturnType;
                        continue;
                    }
                }

                if (throwOnError)
                    throw new MissingMemberException(sourceType.FullName, segName);

                return (Func<object, T?>)(_ => default);
            }

            var body = Expression.Convert(current, typeof(T));
            var lambda = Expression.Lambda<Func<object, T?>>(body, param);
            return lambda.Compile();
        }

        // Resolves a property or field using actual reflection infos (handles non-public and avoids string overload pitfalls).
        private static bool TryBindPropertyOrField(
            ref Expression current,
            ref Type sourceType,
            string name,
            bool ignoreCase,
            Flags flags)
        {
            var bf = ToBindingFlags(flags, ignoreCase);

            // Try property first (prefer properties over fields when both exist)
            var pi = sourceType.GetProperty(name, bf);
            if (pi != null)
            {
                current = Expression.Property(current, pi);
                sourceType = pi.PropertyType;
                return true;
            }

            var fi = sourceType.GetField(name, bf);
            if (fi != null)
            {
                current = Expression.Field(current, fi);
                sourceType = fi.FieldType;
                return true;
            }

            return false;
        }

        // Map your Flags to System.Reflection.BindingFlags.
        // Assumes Flags has Instance/Static/Public/NonPublic bits similar to standard BindingFlags.
        private static BindingFlags ToBindingFlags(Flags flags, bool ignoreCase)
        {
            BindingFlags bf = 0;
            if ((flags & Flags.Instance) != 0) bf |= BindingFlags.Instance;
            if ((flags & Flags.Static) != 0) bf |= BindingFlags.Static;
            if ((flags & Flags.Public) != 0) bf |= BindingFlags.Public;
            if ((flags & Flags.NonPublic) != 0) bf |= BindingFlags.NonPublic;
            if (ignoreCase) bf |= BindingFlags.IgnoreCase;
            return bf;
        }

        private static bool IsNullableValueType(Type t) =>
            t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);

        private static int SafeGetParametersCount(MethodInfo m)
        {
            try { return m.GetParameters().Length; } catch { return -1; }
        }

        private static Type? SafeGetReturnType(MethodInfo m)
        {
            try { return m.ReturnType; } catch { return null; }
        }

        private static string NormalizeSegment(string seg, bool ignoreCase)
        {
            var s = seg.Trim();
            if (s.EndsWith("()", StringComparison.Ordinal))
                s = s[..^2].TrimEnd();
            return ignoreCase ? s.ToLowerInvariant() : s;
        }

        private static T? CastOrDefault<T>(object value, bool throwOnError)
        {
            if (value is T t)
                return t;

            try
            {
                return (T?)Convert.ChangeType(value, typeof(T));
            }
            catch when (!throwOnError)
            {
                return default;
            }
        }

        /// <summary>
        /// Checks and canonicalizes one property, field, or admitted parameterless-method path without invoking any resolved member.<br/>
        /// This resolver shares the property-first and safe-method policy used by <see cref="GetValue{T}(object, string, bool, bool, Flags, IEnumerable{string}?, IEnumerable{Type}?, bool)"/>.<br/>
        /// </summary>
        /// <param name="sourceType">Declared CLR type from which resolution begins.<br/></param>
        /// <param name="propPath">Dotted property path whose method parentheses are optional.<br/></param>
        /// <param name="canonicalPath">Receives the unambiguous canonical path for every successfully resolved segment.<br/></param>
        /// <param name="serializedPrefixCandidate">Receives the longest prefix expected to exist directly in an Inheto name table.<br/></param>
        /// <param name="clrRemainderCandidate">Receives the CLR member suffix that must execute after reading the serialized prefix.<br/></param>
        /// <param name="resultType">Receives the final CLR member type when resolution succeeds.<br/></param>
        /// <param name="containsMethodInvocation">Receives whether at least one segment is a parameterless method call.<br/></param>
        /// <param name="failureKind">Receives the structural failure classification.<br/></param>
        /// <param name="failedSegment">Receives the first unresolved or blocked segment.<br/></param>
        /// <returns><see langword="true"/> when the complete path resolves under the default Inheto invocation policy.<br/></returns>
        internal static bool TryCheckPath(
            Type sourceType,
            string propPath,
            out string canonicalPath,
            out string serializedPrefixCandidate,
            out string clrRemainderCandidate,
            out Type? resultType,
            out bool containsMethodInvocation,
            out PropPathFailureKind failureKind,
            out string? failedSegment)
        {
            ArgumentNullException.ThrowIfNull(sourceType);
            canonicalPath = string.Empty;
            serializedPrefixCandidate = string.Empty;
            clrRemainderCandidate = string.Empty;
            resultType = null;
            containsMethodInvocation = false;
            failureKind = PropPathFailureKind.None;
            failedSegment = null;

            propPath ??= string.Empty;
            string[] segments = propPath.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (segments.Length == 0)
            {
                failureKind = PropPathFailureKind.EmptyPath;
                return false;
            }

            var canonical = new List<string>(segments.Length);
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            Type current = sourceType;
            int continuationStart = -1;
            for (int segmentIndex = 0; segmentIndex < segments.Length; segmentIndex++)
            {
                string rawSegment = segments[segmentIndex];
                string segment = NormalizeSegment(rawSegment, ignoreCase: false);
                if (segment.Length == 0)
                {
                    failureKind = PropPathFailureKind.MissingMember;
                    failedSegment = rawSegment;
                    canonicalPath = canonical.Count == 0 ? string.Empty : "." + string.Join('.', canonical);
                    return false;
                }

                Type lookupType = Nullable.GetUnderlyingType(current) ?? current;
                PropertyInfo? property = lookupType.GetProperty(segment, flags);
                if (property is not null)
                {
                    canonical.Add(property.Name);
                    current = property.PropertyType;
                    if (continuationStart < 0 && segmentIndex + 1 < segments.Length && IsSerializedTerminalType(current))
                        continuationStart = canonical.Count;
                    continue;
                }

                FieldInfo? field = lookupType.GetField(segment, flags);
                if (field is not null)
                {
                    canonical.Add(field.Name);
                    current = field.FieldType;
                    if (continuationStart < 0 && segmentIndex + 1 < segments.Length && IsSerializedTerminalType(current))
                        continuationStart = canonical.Count;
                    continue;
                }

                MethodInfo[] namedMethods = lookupType
                    .GetMethods(flags)
                    .Where(method => string.Equals(method.Name, segment, StringComparison.Ordinal) && SafeGetParametersCount(method) == 0)
                    .ToArray();
                MethodInfo? method = namedMethods.FirstOrDefault(method => IsAllowedMethodReturnType(SafeGetReturnType(method)));
                if (method is null)
                {
                    failureKind = namedMethods.Length == 0 ? PropPathFailureKind.MissingMember : PropPathFailureKind.BlockedMethod;
                    failedSegment = rawSegment;
                    canonicalPath = canonical.Count == 0 ? string.Empty : "." + string.Join('.', canonical);
                    resultType = current;
                    return false;
                }

                canonical.Add(method.Name + "()");
                if (continuationStart < 0)
                    continuationStart = canonical.Count - 1;
                current = method.ReturnType;
                containsMethodInvocation = true;
            }

            canonicalPath = "." + string.Join('.', canonical);
            if (continuationStart >= 0)
            {
                serializedPrefixCandidate = continuationStart == 0
                    ? string.Empty
                    : "." + string.Join('.', canonical.Take(continuationStart));
                clrRemainderCandidate = "." + string.Join('.', canonical.Skip(continuationStart));
            }
            resultType = current;
            return true;
        }

        /// <summary>Tests whether Inheto stores a CLR type as one terminal value rather than recursively naming its CLR members.<br/></summary>
        private static bool IsSerializedTerminalType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            return type.IsEnum || InhetoSerializer.InhetoBaseTypesMap.ContainsKey(type.TypeHandle);
        }

        /// <summary>Tests whether a parameterless method return type belongs to Inheto's default value-path invocation policy.<br/></summary>
        private static bool IsAllowedMethodReturnType(Type? returnType)
        {
            if (returnType is null || returnType == typeof(void))
                return false;

            return DefaultAllowedMethodReturnTypes.Any(type => type.IsAssignableFrom(returnType))
                || (returnType.Namespace?.StartsWith("System.Numerics", StringComparison.Ordinal) ?? false)
                || returnType.IsEnum
                || returnType.IsPrimitive;
        }
    }

    internal class RefEqualityComparer : IEqualityComparer<object>
    {
        private static readonly ConcurrentDictionary<Type, bool> _recordTypeCache = new();
        private static readonly HashSet<Type> _immutableRefTypes = new()
    {
        typeof(string),
        typeof(Uri),
        typeof(Version),
        typeof(IPAddress),
        typeof(System.Net.IPEndPoint),
        typeof(System.Net.Mail.MailAddress),
        typeof(System.Text.RegularExpressions.Regex),
        typeof(System.Globalization.CultureInfo),
        typeof(Type),
        typeof(SecureString)
        // Add more immutable ref types as needed
    };

        public new bool Equals(object? x, object? y)
        {
            if (x is null || y is null)
                return false;

            if (ReferenceEquals(x, y))
                return true;

            Type typeX = x.GetType();
            Type typeY = y.GetType();

            if (typeX != typeY)
                return false;

#if DEBUG
            var typeXName = typeX.Name;
#endif

            if (IsImmutableRefType(typeX))
            {
                if (x is Regex rx && y is Regex ry)
                {
                    if (rx.ToString() != ry.ToString()) return false;
                    if (rx.Options != ry.Options) return false;
                    if (rx.MatchTimeout != ry.MatchTimeout) return false;
                    return true;
                }
                return x.Equals(y);
            }

            if (IsRecordType(typeX))
            {
                return x.Equals(y);
            }

            // Default: reference equality
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            if (obj is null) throw new ArgumentNullException(nameof(obj));

            Type type = obj.GetType();

            if (IsLargeValueType(type) || IsImmutableRefType(type) || IsRecordType(type))
            {
                return obj.GetHashCode();
            }

            return RuntimeHelpers.GetHashCode(obj);
        }

        public static bool HasWritableProperties(Type type)
        {
            foreach (var prop in type.Properties(Flags.InstancePublic))
            {
                if (prop.CanWrite) return true;
            }
            return false;
        }

        public static bool IsImmutableRefType(Type type)
        {
            if (_immutableRefTypes.Contains(type)) return true;
            if (IsRecordType(type)) return IsImmutableRecord(type);
            return false;
        }

        public static bool IsImmutableRecord(Type type)
        {
            if (!IsRecordType(type)) return false;
            return !HasWritableProperties(type);
        }


        public static bool IsRecordType(Type type)
        {
            return _recordTypeCache.GetOrAdd(type, static t =>
            {
                if (t.IsValueType) return false;  // ignore valueType records for this check
                // Heuristic: presence of compiler-generated PrintMembers method
                var printMembers = t.GetMethod("<PrintMembers>", BindingFlags.Instance | BindingFlags.NonPublic);
                if (printMembers != null)
                    return true;

                // Check if Equals(object) is overridden (and not Object.Equals)
                var equalsMethod = t.GetMethod("Equals", new[] { typeof(object) });
                if (equalsMethod != null && equalsMethod.DeclaringType != typeof(object))
                    return true;

                return false;
            });
        }

        public static bool IsLargeValueType(Type type, int sizeThreshold = 12)
        {
            if (!type.IsValueType || type.IsPrimitive)
                return false;

            try
            {
                if (type.Properties().Count + type.Fields(Flags.InstancePublic).Count > sizeThreshold)
                    return true;
            }
            catch { }

            return false;
        }


        public static RefEqualityComparer Instance { get; } = new RefEqualityComparer();
    }

    /// <summary>
    /// Wraps either PropertyInfo or FieldInfo without forcing callers to branch for common metadata. A default descriptor is uninitialized.<br/>
    /// </summary>
    public struct PropField
    {
        /// <summary>
        /// Creates a descriptor for the supplied non-null reflected member; the other member slot is left null.<br/>
        /// </summary>
        /// <param name="prop">Non-null reflected property to wrap.<br/></param>
        public PropField(PropertyInfo prop)
        {
            ArgumentNullException.ThrowIfNull(prop);
            Prop = prop;
            Field = null;
        }

        /// <summary>
        /// Creates a descriptor for the supplied non-null reflected member; the other member slot is left null.<br/>
        /// </summary>
        /// <param name="field">Non-null reflected field to wrap.<br/></param>
        public PropField(FieldInfo field)
        {
            ArgumentNullException.ThrowIfNull(field);
            Prop = null;
            Field = field;
        }

        /// <summary>
        /// Returns the wrapped reflection member hash, or zero for an uninitialized descriptor.<br/>
        /// </summary>
        /// <returns>Wrapped member hash, or zero for an uninitialized descriptor.<br/></returns>
        public override int GetHashCode() => Prop?.GetHashCode() ?? Field?.GetHashCode() ?? 0;
        /// <summary>
        /// Tests whether another descriptor wraps the same property and field references; non-descriptor inputs compare unequal.<br/>
        /// </summary>
        /// <param name="obj">Object to compare with this property-or-field descriptor; null or another type compares unequal.<br/></param>
        /// <returns>True when the documented predicate is satisfied; otherwise false.<br/></returns>
        public override bool Equals(object? obj) => obj is PropField other && Prop == other.Prop && Field == other.Field;
        /// <summary>
        /// Compares whether the descriptors wrap the same reflected property/field; the inequality operator negates that comparison.<br/>
        /// </summary>
        /// <param name="left">First property-or-field descriptor to compare.<br/></param>
        /// <param name="right">Second property-or-field descriptor to compare.<br/></param>
        /// <returns>True when the documented predicate is satisfied; otherwise false.<br/></returns>
        public static bool operator ==(PropField left, PropField right) => left.Equals(right);
        /// <summary>
        /// Compares whether the descriptors wrap the same reflected property/field; the inequality operator negates that comparison.<br/>
        /// </summary>
        /// <param name="left">First property-or-field descriptor to compare.<br/></param>
        /// <param name="right">Second property-or-field descriptor to compare.<br/></param>
        /// <returns>True when the documented predicate is satisfied; otherwise false.<br/></returns>
        public static bool operator !=(PropField left, PropField right) => !left.Equals(right);

        /// <summary>
        /// Gets the wrapped member name; throws InvalidOperationException for an uninitialized descriptor.<br/>
        /// </summary>
        public string Name => Prop?.Name ?? Field?.Name ?? throw new InvalidOperationException("The property-or-field descriptor is uninitialized.");
        /// <summary>
        /// Gets the wrapped property or field value type; throws InvalidOperationException for an uninitialized descriptor.<br/>
        /// </summary>
        public Type Type => Prop?.PropertyType ?? Field?.FieldType ?? throw new InvalidOperationException("The property-or-field descriptor is uninitialized.");
        /// <summary>
        /// Gets PropertyInfo.CanWrite or whether a field is not init-only. This is metadata capability, not proof that assignment is visible, permitted or side-effect-free.<br/>
        /// </summary>
        public bool CanWrite => Prop is not null ? Prop.CanWrite : !(Field ?? throw new InvalidOperationException("The property-or-field descriptor is uninitialized.")).IsInitOnly;

        /// <summary>
        /// Wraps the supplied non-null reflection member in a new property-or-field descriptor.<br/>
        /// </summary>
        /// <param name="prop">Non-null reflected property to wrap.<br/></param>
        /// <returns>Wrapped descriptor or existing materialized Value, without cloning the value.<br/></returns>
        public static implicit operator PropField(PropertyInfo prop) => new PropField(prop);
        /// <summary>
        /// Wraps the supplied non-null reflection member in a new property-or-field descriptor.<br/>
        /// </summary>
        /// <param name="field">Non-null reflected field to wrap.<br/></param>
        /// <returns>Wrapped descriptor or existing materialized Value, without cloning the value.<br/></returns>
        public static implicit operator PropField(FieldInfo field) => new PropField(field);

        /// <summary>
        /// Gets the wrapped property, or null when this descriptor wraps a field or is uninitialized.<br/>
        /// </summary>
        public PropertyInfo? Prop { get; }
        /// <summary>
        /// Gets the wrapped field, or null when this descriptor wraps a property or is uninitialized.<br/>
        /// </summary>
        public FieldInfo? Field { get; }

    }

    /// <summary>
    /// Heuristically classifies zero-input object shapes. Eligibility is not proof of process-wide uniqueness or of semantic equivalence between instances.<br/>
    /// </summary>
    public static class ZeroInputSingletonDetector
    {
        private static readonly ConcurrentDictionary<Type, bool> _typeCache = new();

        /// <summary>
        /// Tests cached zero-input shape eligibility: no input-taking public construction/factories, mutable public members or public collection-valued members. Null returns false; nonsealed abstract types are rejected.<br/>
        /// </summary>
        /// <param name="runtimeType">Optional runtime CLR type to test for zero-input shape eligibility.<br/></param>
        /// <returns>True when the documented predicate is satisfied; otherwise false.<br/></returns>
        public static bool IsMaybeZeroInputType(Type? runtimeType)
        {
            if (runtimeType == null)
                return false;

            var type = runtimeType;

            if (type.IsAbstract && !type.IsSealed)
                throw new InvalidOperationException(
                    $"Type '{type.FullName}' is abstract and cannot be evaluated as a runtime singleton.");

            // short-circuit on cached decision
            if (!_typeCache.TryGetValue(type, out bool isTypeEligible))
            {
                isTypeEligible = EvaluateTypeShape(type);
                _typeCache[type] = isTypeEligible;
            }

            if (!isTypeEligible)
                return false;

            // maybe.  only instance detail can fully determine.
            return true;
        }

        /// <summary>
        /// Tests zero-input shape eligibility and optionally probes public static self-returning properties. An eligible instance returns true even without matching a static property, so true does not establish singleton identity; probed getter failures are ignored.<br/>
        /// </summary>
        /// <param name="instance">Compatible owner instance to read or mutate; a by-reference argument preserves value-type changes.<br/></param>
        /// <returns>True when the documented predicate is satisfied; otherwise false.<br/></returns>
        public static bool IsZeroInputSingleton(object? instance)
        {
            if (instance == null)
                return false;

            var type = instance.GetType();

            if (type.IsAbstract && !type.IsSealed)
                throw new InvalidOperationException(
                    $"Type '{type.FullName}' is abstract and cannot be evaluated as a runtime singleton.");

            // short-circuit on cached decision
            if (!_typeCache.TryGetValue(type, out bool isTypeEligible))
            {
                isTypeEligible = EvaluateTypeShape(type);
                _typeCache[type] = isTypeEligible;
            }

            if (!isTypeEligible)
                return false;

            // only instance-specific part: does it match a static self-returning property
            var staticSelfProps = type
                .Properties(Flags.StaticPublic)
                .Where(p =>
                    type.IsAssignableFrom(p.PropertyType) &&
                    p.GetIndexParameters().Length == 0 &&
                    p.CanRead);

            foreach (var prop in staticSelfProps)
            {
                try
                {
                    if (ReferenceEquals(instance, prop.GetValue(null, null)))
                        return true;
                }
                catch { /* ignore */ }
            }

            // if type was eligible but instance not a known static, still considered fine
            return true;
        }

        private static bool EvaluateTypeShape(Type type)
        {
            // 1. Must have no public ctors or only parameterless one
            var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (ctors.Any(c => c.GetParameters().Length > 0))
                return false;

            // 2. Must not expose static factories requiring input
            bool hasInputFactory = type
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Any(m => type.IsAssignableFrom(m.ReturnType) && m.GetParameters().Length > 0);
            if (hasInputFactory)
                return false;

            // 3. No mutable public members
            bool hasMutable =
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Any(p => p.CanWrite && p.SetMethod!.IsPublic) ||
                type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Any(f => !f.IsInitOnly);
            if (hasMutable)
                return false;

            // 4. No collection members (except string)
            bool hasCollectionMember =
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Any(p => typeof(System.Collections.IEnumerable).IsAssignableFrom(p.PropertyType)
                              && p.PropertyType != typeof(string)) ||
                type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Any(f => typeof(System.Collections.IEnumerable).IsAssignableFrom(f.FieldType)
                              && f.FieldType != typeof(string));
            if (hasCollectionMember)
                return false;

            return true;
        }
    }

    /// <summary>
    /// Caches constructor delegates bound to one deserialization policy, preferring registered activators and comparer-aware construction.<br/>
    /// </summary>
    public class ComparerActivator
    {
        private readonly ConcurrentDictionary<RuntimeTypeHandle, Func<DeserializationOptions.ActivatorContext, object>> _activatorCache = new();

        private readonly DeserializationOptions dzOpts;

        /// <summary>
        /// Creates a comparer-aware activator bound to one deserialization policy.<br/>
        /// The bound policy supplies custom activators and comparer rules for cached constructor delegates.<br/>
        /// </summary>
        /// <param name="dzOptions">Required owning deserialization policy.<br/></param>
        public ComparerActivator(DeserializationOptions dzOptions)
        {
            ArgumentNullException.ThrowIfNull(dzOptions);
            dzOpts = dzOptions;
        }

        /// <summary>
        /// Returns cached construction dispatch for this type: a registered activator, a comparer-aware public constructor, or a parameterless public constructor. The returned delegate throws MissingMethodException if none applies; later registration changes do not invalidate this cache.<br/>
        /// </summary>
        /// <param name="type">Exact CLR type used for registration, lookup or runtime shape analysis.<br/></param>
        /// <returns>Cached construction delegate; no instance is created until it is invoked.<br/></returns>
        public Func<DeserializationOptions.ActivatorContext, object> GetActivatorDelegate(Type type)
        {

            return _activatorCache.GetOrAdd(type.TypeHandle, _ =>
            {
                if (dzOpts.TryGetActivator(type, out var act))
                {
                    return new Func<DeserializationOptions.ActivatorContext, object>(ctx => act(ctx)!);
                }

                var ctors = type.Constructors(Flags.InstancePublic);

                // prefer constructor that accepts an IComparer<T>
                var comparerCtor = ctors.FirstOrDefault(c =>
                {
                    var p = c.GetParameters();
                    if (p.Length != 1)
                        return false;

                    var paramType = p[0].ParameterType;
                    return paramType.IsGenericType && (
                               paramType.GetGenericTypeDefinition() == typeof(IComparer<>) ||
                               paramType.GetGenericTypeDefinition() == typeof(IEqualityComparer<>)) ||
                           paramType.GetInterfaces()
                                    .Any(i => i.IsGenericType && (
                                        i.GetGenericTypeDefinition() == typeof(IComparer<>) ||
                                        i.GetGenericTypeDefinition() == typeof(IEqualityComparer<>)));
                });

                if (comparerCtor is not null)
                {
                    var ctorDelegate = type.DelegateForCreateInstance(comparerCtor.GetParameters()[0].ParameterType);
                    return new Func<DeserializationOptions.ActivatorContext, object>(ctx => ctorDelegate(ctx.StoredContext));
                }

                // fall back to parameterless constructor
                var defaultCtor = ctors.FirstOrDefault(c => c.GetParameters().Length == 0);
                if (defaultCtor is not null)
                {
                    var ctorDelegate = type.DelegateForCreateInstance();
                    return new Func<DeserializationOptions.ActivatorContext, object>(_ => ctorDelegate());
                }

                // no suitable constructor found
                return new Func<DeserializationOptions.ActivatorContext, object>(_ =>
                    throw new MissingMethodException($"No suitable constructor found for type {type.FullName}."));
            });
        }
    }




    /// <summary>
    /// Enumerates array elements together with Inheto multidimensional coordinate tokens, using cached element/rank dispatch.<br/>
    /// </summary>
    public static class MdPathEnumerable
    {
        // Blind caller surface
        /// <summary>
        /// Returns deferred coordinate/value enumeration over a zero-based CLR array, with coordinate tokens such as @0,1. Rank-specific dispatch is cached; value-type elements are boxed by the object-valued result.<br/>
        /// </summary>
        /// <param name="a">Zero-based CLR array to enumerate; its contents are read during iteration.<br/></param>
        /// <returns>Deferred sequence of coordinate tokens and element values; value types are boxed.<br/></returns>
        public static IEnumerable<(string Path, object Value)> Enumerate(Array a)
        {
            var t = a.GetType().GetElementType()!;
            return (IEnumerable<(string, object)>)_cache.GetOrAdd((t, a.Rank), Build)(a);
        }

        // Cache: (T, rank) -> factory(Array) => IEnumerable<(string, object)>
        private static readonly ConcurrentDictionary<(Type, int), Func<Array, object>> _cache = new();

        private static Func<Array, object> Build((Type T, int Rank) key)
        {
            var method = typeof(MdPathEnumerable).GetMethod(nameof(CreateEnumerable), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                                                 .MakeGenericMethod(key.T);
            return arr => method.Invoke(null, new object[] { arr, key.Rank })!;
        }

        private static object CreateEnumerable<T>(Array a, int rank)
            => new AnyRankEnumerable<T>(a, rank);

        // -------- Core enumerable for any rank --------
        private sealed class AnyRankEnumerable<T> : IEnumerable<(string, object)>
        {
            private readonly Array _a;
            private readonly int _rank;
            private readonly int[] _lengths;
            private readonly Func<Array, int[], T> _indexer;

            public AnyRankEnumerable(Array a, int rank)
            {
                _a = a; _rank = rank;
                _lengths = new int[rank];
                for (int d = 0; d < rank; d++) _lengths[d] = a.GetLength(d);
                _indexer = IndexerCache<T>.Get(rank);
            }

            public Enumerator GetEnumerator() => new(_a, _lengths, _indexer);
            IEnumerator<(string, object)> IEnumerable<(string, object)>.GetEnumerator() => GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public struct Enumerator : IEnumerator<(string, object)>
            {
                private readonly Array _a;
                private readonly int[] _lens;
                private readonly Func<Array, int[], T> _idx;
                private readonly int[] _ix;
                private readonly int _total;
                private int _flat;
                private (string, object) _current;

                public Enumerator(Array a, int[] lengths, Func<Array, int[], T> idx)
                {
                    _a = a; _lens = lengths; _idx = idx;
                    _ix = new int[lengths.Length];
                    _total = 1; foreach (var L in lengths) _total *= L;
                    _flat = -1; _current = default;
                }

                public (string, object) Current => _current;
                object IEnumerator.Current => _current;

                public bool MoveNext()
                {
                    if (++_flat >= _total) return false;

                    // Format "@i,j,k,..." with stackalloc
                    Span<char> buf = stackalloc char[1 + _lens.Length * 12]; // '@' + up to 11 chars per dim incl separator
                    int p = 0; buf[p++] = '@';
                    for (int d = 0; d < _lens.Length; d++)
                    {
                        if (!(_ix[d]).TryFormat(buf[p..], out int w)) w = 0;
                        p += w;
                        if (d + 1 < _lens.Length) buf[p++] = ',';
                    }
                    string path = new string(buf[..p]);

                    // Read value via typed indexer. No boxing here. Box once into object for the tuple.
                    T value = _idx(_a, _ix);
                    _current = (path, value!);

                    // Odometer increment
                    for (int d = _lens.Length - 1; d >= 0; d--)
                    {
                        if (++_ix[d] < _lens[d]) break;
                        _ix[d] = 0;
                    }

                    return true;
                }

                public void Reset() { /* not used */ }
                public void Dispose() { }
            }
        }

        // Build a typed indexer delegate for rank N: (Array a, int[] ix) => ((T[,,,,])a)[ix0,...,ixN-1]
        private static class IndexerCache<T>
        {
            private static readonly ConcurrentDictionary<int, Func<Array, int[], T>> _byRank = new();
            public static Func<Array, int[], T> Get(int rank)
                => _byRank.GetOrAdd(rank, Build);

            private static Func<Array, int[], T> Build(int rank)
            {
                var aParam = Expression.Parameter(typeof(Array), "a");
                var ixParam = Expression.Parameter(typeof(int[]), "ix");

                // cast to T[,,,] with rank dims
                var mdType = rank switch
                {
                    2 => typeof(T[,]),
                    3 => typeof(T[,,]),
                    4 => typeof(T[,,,]),
                    5 => typeof(T[,,,,]),
                    6 => typeof(T[,,,,,]),
                    7 => typeof(T[,,,,,,]),
                    8 => typeof(T[,,,,,,,]),
                    _ => ArrayTypeOf(typeof(T), rank)
                };
                var castA = Expression.Convert(aParam, mdType);

                var indices = new Expression[rank];
                for (int d = 0; d < rank; d++)
                    indices[d] = Expression.ArrayIndex(ixParam, Expression.Constant(d));

                var access = Expression.ArrayAccess(castA, indices);
                var lambda = Expression.Lambda<Func<Array, int[], T>>(access, aParam, ixParam);
                return lambda.Compile();
            }

            private static Type ArrayTypeOf(Type t, int rank) => rank switch
            {
                <= 8 => throw new InvalidOperationException(),
                _ => Type.GetType($"{t.FullName}[{new string(',', rank - 1)}]")!.MakeArrayType(rank) // Fallback for 9..32
            };
        }
    }




    /// <summary>
    /// Contains cached generic dictionary construction dispatch used by the Inheto reader.<br/>
    /// </summary>
    public static class DictFactory
    {
        private static readonly ConcurrentDictionary<(Type, Type), Func<object>> _cache = new();

        /// <summary>
        /// Creates or reuses a cached constructor delegate for Dictionary&lt;TKey,TValue&gt;.<br/>
        /// Returns instance as IDictionary.
        /// </summary>
        public static IDictionary Create(Type keyType, Type valueType)
        {
            if (keyType is null || valueType is null)
                throw new ArgumentNullException();

            var ctor = _cache.GetOrAdd((keyType, valueType), CreateConstructor);
            return (IDictionary)ctor();
        }

        private static Func<object> CreateConstructor((Type Key, Type Value) types)
        {
            var dictType = typeof(Dictionary<,>).MakeGenericType(types.Key, types.Value);
            return () => Activator.CreateInstance(dictType)!;
        }
    }

}

