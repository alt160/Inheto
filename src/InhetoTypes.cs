using System;
using System.Collections.Concurrent;
using System.Numerics;








namespace Inheto
{
    //====== TYPES ======

    /// <summary>
    /// Defines the 6-bit Base Type ID for the Abraxas DB serialization format.
    /// The primary organization is by thematic group. Within each group of fixed-size
    /// types, the order is by storage size. Variable-size types are placed last.
    /// </summary>
    public enum InhetoBaseTypes : byte // Fits within 6 bits (0-63)
    {
        // Group 1: Control Codes (0-9)
        /// <summary>
        /// Unrecognized or absent encoded type; not a serialized non-null CLR value.<br/>
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Marker indicating that a promoted representation descriptor follows.<br/>
        /// </summary>
        PromotedType, // marker for an richer type that is not a generic collection type, for example IPAddress, MailAddress, Regex, etc.
        /// <summary>
        /// Custom member-path serializer payload marker.<br/>
        /// </summary>
        CustomProp,  // explicitly registered prop/field serializer, along any prop path of the object.
        /// <summary>
        /// Custom exact-type serializer payload marker.<br/>
        /// </summary>
        CustomType,  // explicitly registered type serializer
        /// <summary>
        /// Reference alias to a previously encoded name-header entry.<br/>
        /// </summary>
        DuplicateRef, // types that are duplicated in the object being serialized.  This is a storage feature for shared references, even circular ones.  Modifications cause all pointers to this type to see the same change.
        /// <summary>
        /// Value alias to a previously encoded entry; materialization may copy mutable values according to the reader path.<br/>
        /// </summary>
        DuplicateValue, // types that are immutable or valuetypes that are duplicated in the object being serialized.  This is a storage optimization for immutable types.  On modification, the shared reference is replaced with a new value.
        /// <summary>
        /// Complex-object marker carrying inherited declared-member information.<br/>
        /// </summary>
        InheritedType, // types that inherit or implement one of the core types of this enum, primarily for the collection types in the ExtendedInhetoTypes enum.  Second byte is used to indicate the base type.
        /// <summary>
        /// Complex-object representation whose children are separately addressable name entries.<br/>
        /// </summary>
        ComplexType, // types that themself have props/fields that need to be graphed and serialized.  The complex type will navigate the entire object graph and serialize the props/fields.
        /// <summary>
        /// Explicit serialized null marker, distinct from an absent name entry.<br/>
        /// </summary>
        Null,

        // Group 2a: simple primitives (10-13)
        /// <summary>
        /// Identifies the Boolean representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Boolean = 10,       // 1 byte
        /// <summary>
        /// Identifies the Char representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Char,          // 2 bytes
        /// <summary>
        /// Identifies the Rune representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Rune,          // 4 bytes
        /// <summary>
        /// Identifies the String representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        String,        // Variable size

        // Group 2b: Base Numerics and usable in Enum types (14-21)
        /// <summary>
        /// Identifies the SByte representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        SByte,         // 1 byte
        /// <summary>
        /// Identifies the Byte representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Byte,          // 1 byte
        /// <summary>
        /// Identifies the Int16 representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Int16,         // 2 bytes
        /// <summary>
        /// Identifies the UInt16 representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        UInt16,        // 2 bytes
        /// <summary>
        /// Identifies the Int32 representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Int32,         // 4 bytes
        /// <summary>
        /// Identifies the UInt32 representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        UInt32,        // 4 bytes
        /// <summary>
        /// Identifies the Int64 representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Int64,         // 8 bytes
        /// <summary>
        /// Identifies the UInt64 representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        UInt64,        // 8 bytes

        // Group 2c: Extended Numerics (22-23)
        /// <summary>
        /// Identifies the Int128 representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Int128,        // 16 bytes
        /// <summary>
        /// Identifies the UInt128 representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        UInt128,       // 16 bytes

        // Group 2d: Floating Point Numerics (24-27)
        /// <summary>
        /// Identifies the Half representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Half,          // 2 bytes
        /// <summary>
        /// Identifies the Single representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Single,        // 4 bytes -- value here and above are inlined in the inheto header for performance
        /// <summary>
        /// Identifies the Double representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Double,        // 8 bytes
        /// <summary>
        /// Identifies the Decimal representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Decimal,       // 16 bytes

        // Group 3: Common Value-Like Framework Types (30-49)
        /// <summary>
        /// Identifies the DateOnly representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        DateOnly = 30,      // 4 bytes -- this value is inlined in the inheto header for performance
        /// <summary>
        /// Identifies the DateTime representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        DateTime = 31,      // 8 bytes
        /// <summary>
        /// Identifies the TimeOnly representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        TimeOnly = 33,      // 8 bytes
        /// <summary>
        /// Identifies the TimeSpan representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        TimeSpan = 32,      // 8 bytes
        /// <summary>
        /// Identifies the DateTimeOffset representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        DateTimeOffset = 34,// 16 bytes (DateTime Ticks + Offset Ticks)
        /// <summary>
        /// Identifies the Guid representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Guid = 35,          // 16 bytes
        /// <summary>
        /// Identifies the Version representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Version = 36,       // 16 bytes (4x int)
        /// <summary>
        /// Identifies the Uri representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Uri = 37,           // Variable size

        // --- System.Numerics Family (ordered by size) ---
        /// <summary>
        /// Identifies the Vector2 representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Vector2 = 40,       // 8 bytes (2x float)
        /// <summary>
        /// Identifies the Vector3 representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Vector3 = 41,       // 12 bytes (3x float)
        /// <summary>
        /// Identifies the Vector4 representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Vector4 = 42,       // 16 bytes (4x float)
        /// <summary>
        /// Identifies the System.Numerics.Complex representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        ComplexNumeric = 43,       // 16 bytes (2x double)
        /// <summary>
        /// Identifies the Quaternion representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Quaternion = 44,    // 16 bytes (4x float)
        /// <summary>
        /// Identifies the Plane representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Plane = 45,         // 16 bytes (Vector3 + float)
        /// <summary>
        /// Identifies the Matrix3x2 representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Matrix3x2 = 46,     // 24 bytes (6x float)
        /// <summary>
        /// Identifies the Matrix4x4 representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Matrix4x4 = 47,     // 64 bytes (16x float)
        /// <summary>
        /// Identifies the BigInteger representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        BigInteger = 48,    // Variable size

        // Group 4: Promoted Common Use Types (50-59)
        /// <summary>
        /// Identifies the byte[] representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        ByteArray = 50,
        /// <summary>
        /// Identifies the List&lt;string&gt; representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        ListString = 51,

        /// <summary>
        /// Terminator of the encoded names-header region.<br/>
        /// </summary>
        EndOfNamesHeader = 63,

        /// <summary>
        /// Obsolete reserved kind-mask value; not an admissible base-type marker.<br/>
        /// </summary>
        [Obsolete("Not a value; InhetoTypeFlags enum for kind info", true)]
        RESERVED_KIND_MASKS = 0xC0,
    }


    /// <summary>
    /// Defines kind selectors and masks used to interpret the encoded type header; auxiliary flags must be interpreted in the applicable representation family.<br/>
    /// </summary>
    public enum InhetoKindMasks : byte
    {
        /// <summary>
        /// Base-family selector.<br/>
        /// </summary>
        InhetoBaseType = 0b0000_0000,             // bits 6,7 value of 00 mean to use InhetoBaseTypes enum for this byte.
        /// <summary>
        /// Nullable base-family selector.<br/>
        /// </summary>
        InhetoNullableBaseType = 0b0100_0000,     // bits 6,7 value of 01 mean to use InhetoBaseTypes enum and apply IS_NULLABLE on the type (when its a valuetype) for this byte.
        /// <summary>
        /// Nullable promoted-family selector.<br/>
        /// </summary>
        InhetoNullablePromotedType = 0b0100_0001, // bits 6,7 value of 01 and bit 0 true mean to use InhetoNullablePromotedTypes enum for this byte.
        /// <summary>
        /// Promoted-family selector.<br/>
        /// </summary>
        InhetoPromotedType = 0b1000_0000,         // bits 6,7 value of 10 mean to use InhetoPromotedTypes enum for this byte.
        /// <summary>
        /// Collection-family selector.<br/>
        /// </summary>
        InhetoCollectionType = 0b1100_0000,       // bits 6,7 value of 11 mean to use InhetoCollectionTypes enum for this byte.
        /// <summary>
        /// Auxiliary metadata bit for applicable descriptor families.<br/>
        /// </summary>
        InhetoAuxiliaryData = 0b0010_0000,        // bit 5 true means to use InhetoAuxiliaryTypes enum for this byte.
        /// <summary>
        /// Mask isolating the base type identifier bits.<br/>
        /// </summary>
        InhetoBaseTypeMask = 0b0011_1111,
        /// <summary>
        /// Mask isolating the representation-kind bits.<br/>
        /// </summary>
        InhetoKindMask = 0b1100_0000,
    }

    /// <summary>
    /// Identifies non-array, single-dimensional/jagged and rectangular multidimensional array representations.<br/>
    /// </summary>
    public enum InhetoArrayTypes : byte
    {
        /// <summary>
        /// No array representation.<br/>
        /// </summary>
        NonArray = 0,
        /// <summary>
        /// Single-dimensional or jagged array representation.<br/>
        /// </summary>
        Array = InhetoPromotedTypes.Array,
        /// <summary>
        /// Rectangular multidimensional CLR array representation.<br/>
        /// </summary>
        ArrayMD = InhetoPromotedTypes.ArrayMD,
    }
    /// <summary>
    /// Defines the specific type when the BaseInhetoType is SpecialTypeMarker.
    /// This enum's value is stored in the second byte of a 2-byte type identifier.
    /// </summary>
    public enum InhetoPromotedTypes : byte
    {
        /// <summary>
        /// No representation or comparer selected in this descriptor family.<br/>
        /// </summary>
        None = 0,

        /// <summary>
        /// Single-dimensional or jagged array representation.<br/>
        /// </summary>
        Array,  // single-dimensional array, jagged array, or unknown rank array. following byte is used to indicate the element type
        /// <summary>
        /// Rectangular multidimensional CLR array representation.<br/>
        /// </summary>
        ArrayMD,  // multi-dimensional array. following byte is used to indicate the element type
        /// <summary>
        /// Identifies the Enum representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Enum,  // following byte is used to indicate the underlying type for this enum.
        /// <summary>
        /// General enumerable representation with a count slot and separately numbered element headers.<br/>
        /// </summary>
        Enumerable, // following byte is used to indicate the element type
        /// <summary>
        /// Zero-input object representation; the name is a wire-format label, not a requirement that the CLR type be static.<br/>
        /// </summary>
        StaticClass,  // indicates a static class.  one with no properties or fields.  AssemblyQualifiedName is used to identify the type.

        // special collection types
        /// <summary>
        /// Identifies the NameValueCollection representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        NameValueCollection,
        /// <summary>
        /// Identifies the StringCollection representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        StringCollection,


        // chosen complex or other types
        /// <summary>
        /// Identifies the CultureInfo representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        CultureInfo,
        /// <summary>
        /// Identifies the IPAddress representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        IPAddress,
        /// <summary>
        /// Identifies the IPEndPoint representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        IPEndPoint,
        /// <summary>
        /// Identifies the MailAddress representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        MailAddress,
        /// <summary>
        /// Identifies the Memory&lt;byte&gt; representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        MemoryByte,
        /// <summary>
        /// Identifies the MemoryStream representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        MemoryStream,
        /// <summary>
        /// Identifies the ReadOnlyMemory&lt;byte&gt; representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        ReadOnlyMemoryByte,
        /// <summary>
        /// Identifies the Regex representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Regex,
        /// <summary>
        /// Identifies the SecureString representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        SecureString,
        /// <summary>
        /// Identifies the StringBuilder representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        StringBuilder,
        /// <summary>
        /// Identifies the TimeZoneInfo representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        TimeZoneInfo,
        /// <summary>
        /// Identifies the Type representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Type,

        // clr comparer types
        /// <summary>
        /// Identifier for the built-in StringComparer.CurrentCulture comparer; case/culture behavior follows that comparer.<br/>
        /// </summary>
        CurrentCulture,
        /// <summary>
        /// Identifier for the built-in StringComparer.CurrentCultureIgnoreCase comparer; case/culture behavior follows that comparer.<br/>
        /// </summary>
        CurrentCultureIgnoreCase,
        /// <summary>
        /// Identifier for the built-in StringComparer.InvariantCulture comparer; case/culture behavior follows that comparer.<br/>
        /// </summary>
        InvariantCulture,
        /// <summary>
        /// Identifier for the built-in StringComparer.InvariantCultureIgnoreCase comparer; case/culture behavior follows that comparer.<br/>
        /// </summary>
        InvariantCultureIgnoreCase,
        /// <summary>
        /// Identifier for the built-in StringComparer.Ordinal comparer; case/culture behavior follows that comparer.<br/>
        /// </summary>
        Ordinal,
        /// <summary>
        /// Identifier for the built-in StringComparer.OrdinalIgnoreCase comparer; case/culture behavior follows that comparer.<br/>
        /// </summary>
        OrdinalIgnoreCase,

        // this enum supports a max of 64 values.  bits 6/7 are reserved for the InheotKindMasks enum
    }

    /// <summary>
    /// Identifies the supported built-in string comparer representations; it does not encode arbitrary custom comparer implementations.<br/>
    /// </summary>
    public enum InhetoComparerTypes : byte
    {
        /// <summary>
        /// No representation or comparer selected in this descriptor family.<br/>
        /// </summary>
        None = 0,
        /// <summary>
        /// Identifier for the built-in StringComparer.CurrentCulture comparer; case/culture behavior follows that comparer.<br/>
        /// </summary>
        CurrentCulture = InhetoPromotedTypes.CurrentCulture,
        /// <summary>
        /// Identifier for the built-in StringComparer.CurrentCultureIgnoreCase comparer; case/culture behavior follows that comparer.<br/>
        /// </summary>
        CurrentCultureIgnoreCase,
        /// <summary>
        /// Identifier for the built-in StringComparer.InvariantCulture comparer; case/culture behavior follows that comparer.<br/>
        /// </summary>
        InvariantCulture,
        /// <summary>
        /// Identifier for the built-in StringComparer.InvariantCultureIgnoreCase comparer; case/culture behavior follows that comparer.<br/>
        /// </summary>
        InvariantCultureIgnoreCase,
        /// <summary>
        /// Identifier for the built-in StringComparer.Ordinal comparer; case/culture behavior follows that comparer.<br/>
        /// </summary>
        Ordinal,
        /// <summary>
        /// Identifier for the built-in StringComparer.OrdinalIgnoreCase comparer; case/culture behavior follows that comparer.<br/>
        /// </summary>
        OrdinalIgnoreCase,

    }

    /// <summary>
    /// Defines encoded collection-family identifiers and associated layout constants, including compact string-key dictionary variants.<br/>
    /// </summary>
    public enum InhetoCollectionTypes : byte
    {
        /// <summary>
        /// No representation or comparer selected in this descriptor family.<br/>
        /// </summary>
        None = 0,
        // --- Generic Collection types ---  following byte determines TKey or TValue
        // for dictionary types with string keys, the following byte determines the TValue type
        // for dictionary types with non-string keys, the following 2 bytes determines the TKey and TValue type
        // for any type below that uses a comparer, if HAS_AUXILIARY_DATA is set, a subsequent byte is used to indicate the comparer type.

        // list types
        /// <summary>
        /// Identifies the List representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        List = UtilsAndExtensions.GenericFamily.List,        // List<T>.  starting at 2 to align with UtilsAndExtensions.GenericFamily enum
        /// <summary>
        /// Identifies the Queue representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Queue = UtilsAndExtensions.GenericFamily.Queue,       // Queue<T>
        /// <summary>
        /// Identifies the ConcurrentQueue representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        ConcurrentQueue = UtilsAndExtensions.GenericFamily.ConcurrentQueue, // ConcurrentQueue<T>.
        /// <summary>
        /// Identifies the Stack representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Stack = UtilsAndExtensions.GenericFamily.Stack,       // Stack<T>
        /// <summary>
        /// Identifies the ConcurrentStack representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        ConcurrentStack = UtilsAndExtensions.GenericFamily.ConcurrentStack, // ConcurrentStack<T>.
        /// <summary>
        /// Identifies the ObservableCollection representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        ObservableCollection = UtilsAndExtensions.GenericFamily.ObservableCollection, // ObservableCollection<T>.  
        /// <summary>
        /// Identifies the ConcurrentBag representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        ConcurrentBag = UtilsAndExtensions.GenericFamily.ConcurrentBag, // ConcurrentBag<T>.  

        // set types
        /// <summary>
        /// Identifies the SortedSet representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        SortedSet = UtilsAndExtensions.GenericFamily.SortedSet,   // SortedSet<T>
        /// <summary>
        /// Identifies the HashSet representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        HashSet = UtilsAndExtensions.GenericFamily.HashSet,     // HashSet<T>

        // dictionary types
        /// <summary>
        /// Identifies the Dictionary representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        Dictionary = UtilsAndExtensions.GenericFamily.Dictionary,  // Dictionary<TKey, TValue>.  a 3rd byte is used after this one to indicate the TValue type of the dictionary
        /// <summary>
        /// Identifies the ConcurrentDictionary representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        ConcurrentDictionary = UtilsAndExtensions.GenericFamily.ConcurrentDictionary, // ConcurrentDictionary<TKey, TValue>.  a 3rd byte is used after this one to indicate the TValue type of the dictionary
        /// <summary>
        /// Identifies the SortedDictionary representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        SortedDictionary = UtilsAndExtensions.GenericFamily.SortedDictionary, // SortedDictionary<TKey, TValue>.  a 3rd byte is used after this one to indicate the TValue type of the dictionary
        /// <summary>
        /// Identifies the SortedList representation within this descriptor family; interpret it with its enclosing kind and auxiliary metadata.<br/>
        /// </summary>
        SortedList = UtilsAndExtensions.GenericFamily.SortedList,  // SortedList<TKey, TValue>.  a 3rd byte is used after this one to indicate the TValue type of the dictionary
 
        // string-keyed dictionary types
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

        /// <summary>
        /// Auxiliary metadata bit for an applicable encoded collection/type header.<br/>
        /// </summary>
        HAS_AUXILIARY_DATA = 0b0010_0000,
        /// <summary>
        /// Collection-kind bit pattern used in encoded type headers.<br/>
        /// </summary>
        IS_COLLECTION_TYPE = 0b1100_0000
    }

    /// <summary>
    /// Defines auxiliary collection-header values for built-in string comparers and string-key dictionary information.<br/>
    /// </summary>
    public enum InhetoCollectionAuxiliaryTypes : byte
    {
        /// <summary>
        /// Identifier for the built-in StringComparer.CurrentCulture comparer; case/culture behavior follows that comparer.<br/>
        /// </summary>
        CurrentCulture = InhetoPromotedTypes.CurrentCulture,
        /// <summary>
        /// Identifier for the built-in StringComparer.CurrentCultureIgnoreCase comparer; case/culture behavior follows that comparer.<br/>
        /// </summary>
        CurrentCultureIgnoreCase,
        /// <summary>
        /// Identifier for the built-in StringComparer.InvariantCulture comparer; case/culture behavior follows that comparer.<br/>
        /// </summary>
        InvariantCulture,
        /// <summary>
        /// Identifier for the built-in StringComparer.InvariantCultureIgnoreCase comparer; case/culture behavior follows that comparer.<br/>
        /// </summary>
        InvariantCultureIgnoreCase,
        /// <summary>
        /// Identifier for the built-in StringComparer.Ordinal comparer; case/culture behavior follows that comparer.<br/>
        /// </summary>
        Ordinal,
        /// <summary>
        /// Identifier for the built-in StringComparer.OrdinalIgnoreCase comparer; case/culture behavior follows that comparer.<br/>
        /// </summary>
        OrdinalIgnoreCase,

        /// <summary>
        /// Auxiliary identifier for dictionary string-key information.<br/>
        /// </summary>
        DictionaryStringKey = 0b0000_1000,

    }

    /// <summary>
    /// Provides byte constants for constructing and masking the existing type-header representation.<br/>
    /// </summary>
    public static class InhetoTypeFlags
    {
        /// <summary>
        /// Base representation selector.<br/>
        /// </summary>
        public const byte KIND_BASE = 0b0000_0000;
        /// <summary>
        /// Promoted representation selector.<br/>
        /// </summary>
        public const byte KIND_PROMOTED = 0b1000_0000; // Bit 7 (128)
        /// <summary>
        /// Auxiliary metadata bit for an applicable encoded collection/type header.<br/>
        /// </summary>
        public const byte HAS_AUXILIARY_DATA = 0b0010_0000; // Bit 5.  used only with KIND_COLLECTION types typically for comparer tracking
        /// <summary>
        /// Nullable representation selector.<br/>
        /// </summary>
        public const byte KIND_NULLABLE = 0b0100_0000; // Bit 6 (64)
        /// <summary>
        /// Collection representation selector.<br/>
        /// </summary>
        public const byte KIND_COLLECTION = 0b1100_0000; // Bit 7 and 6
        /// <summary>
        /// Mask isolating the base type identifier bits.<br/>
        /// </summary>
        public const byte BASE_TYPE_MASK = 0b0011_1111; // The mask to get the BaseInhetoType
        /// <summary>
        /// Mask isolating the collection family identifier bits.<br/>
        /// </summary>
        public const byte COLL_TYPE_MASK = 0b0001_1111; // The mask to get the InhetoCollectionTypes
    }



}

