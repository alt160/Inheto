using System.Text;








namespace Inheto
{
    //====== TYPES ======
    internal partial struct NameHeaderEntry : IEquatable<NameHeaderEntry>
    {








        //======  FIELDS  ======
        private string? _cleanName;
        private BufferStream buffer;
        private bool bufferBorrowed;
        private string? propPath;








        public uint Index;
        public InhetoKindMasks Kind;
        public string Name;
        public uint NextEntryIndex;
        public uint ParentIndex;
        public NameHeaderEntryType Type;
        public uint ValueOffset;








        //======  CONSTRUCTORS  ======
        public NameHeaderEntry(BufferStream buffer, uint index, string name) : this(buffer, index, false) => Name = name == null ? Name : string.Intern(name);
        public NameHeaderEntry(BufferStream buffer, uint index) : this(buffer, index, false)
        {
        }

        /// <summary>
        /// Parses one name-header entry while choosing whether the entry retains its own stream segment or borrows an InhetoBinary-owned stream.<br/>
        /// Borrowed entries parse through one short-lived pooled segment so they never move a shared cursor and can reuse the same segment object throughout one payload materialization.<br/>
        /// </summary>
        /// <param name="buffer">Payload stream whose logical index zero matches the name-header offsets.<br/></param>
        /// <param name="index">Entry byte offset within the payload stream.<br/></param>
        /// <param name="borrowBuffer">Whether the resulting entry may retain the supplied stream without owning a segment.<br/></param>
        private NameHeaderEntry(BufferStream buffer, uint index, bool borrowBuffer)
        {
            ArgumentNullException.ThrowIfNull(buffer);
            this = default;
            this.buffer = borrowBuffer
                ? buffer
                : index == 0
                    ? buffer.Segment(1)
                    : buffer.Segment(0);
            bufferBorrowed = borrowBuffer;

            if (borrowBuffer)
            {
                var reader = new NameHeaderSpanReader(
                    buffer.AsReadOnlySpan,
                    checked((int)index),
                    buffer.StringEncoding);
                Index = index;
                ParentIndex = reader.Read7BitEncodedUInt();

                var kindByte = reader.PeekByte();
                var kind = (InhetoKindMasks)(kindByte & 0b1100_0000);
                Kind = kind;
                Type = NameHeaderEntryType.ReadBorrowed(ref reader);
                Name = string.Intern(reader.ReadString()!);
                ValueOffset = reader.ReadUInt32();
                NextEntryIndex = Type.BaseType == InhetoBaseTypes.EndOfNamesHeader ? 0 : checked((uint)reader.Position);
                return;
            }

            Index = index;
            buffer.Position = index;
            ParentIndex = buffer.Read7BitEncodedUInt();

            var ownedKindByte = buffer.PeekByte();
            var ownedKind = (InhetoKindMasks)(ownedKindByte & 0b1100_0000);
            Kind = ownedKind;
            Type = new NameHeaderEntryType(buffer);
            Name = string.Intern(buffer.ReadString()!);
            ValueOffset = buffer.ReadUInt32();
            NextEntryIndex = Type.BaseType == InhetoBaseTypes.EndOfNamesHeader ? 0 : (uint)buffer.Position;
        }





        public enum EntryTypes : byte
        {
            Any = 0,
            PropOrField = (byte)'.',
            KeyString = (byte)'!',
            NumberedElement = (byte)'#',
            MDArrayElement = (byte)'@',

        }


        //======  PROPERTIES  ======
        public string CleanName { get => _cleanName ??= Name == "" ? "" : string.Intern(Name.Substring(1)); }

        public bool IsEmpty => Index == 0 && ParentIndex == 0 && Name == null && ValueOffset == 0 && NextEntryIndex == 0;

        public NameHeaderEntry Next
        {
            get
            {
                if (IsEmpty) return default;
                if (Type.BaseType == InhetoBaseTypes.EndOfNamesHeader || NextEntryIndex == 0) return default;
                var next = bufferBorrowed
                    ? CreateBorrowed(buffer, NextEntryIndex)
                    : new NameHeaderEntry(buffer, NextEntryIndex);
                return next;
            }
        }

        public NameHeaderEntry NextSibling(EntryTypes entryType)
        {
            if (IsEmpty) return default;
            if (Type.BaseType == InhetoBaseTypes.EndOfNamesHeader || NextEntryIndex == 0) return default;
            var sib = this;
TryNext:
            sib = sib.Next;
            if (sib.IsEmpty || sib.Type.BaseType == InhetoBaseTypes.EndOfNamesHeader || sib.ParentIndex < ParentIndex) return default;
            if (sib.ParentIndex == ParentIndex && (sib.Name[0] == (char)entryType || entryType == EntryTypes.Any)) return sib;
            goto TryNext;
        }
        public NameHeaderEntry FirstChild(EntryTypes entryType)
        {
            if (IsEmpty) return default;
            if (Type.BaseType == InhetoBaseTypes.EndOfNamesHeader || NextEntryIndex == 0) return default;
            var sib = this;
TryNext:
            sib = sib.Next;
            if (sib.ParentIndex == Index && (sib.Name[0] == (char)entryType || entryType == EntryTypes.Any)) return sib;
            if (sib.IsEmpty || sib.Type.BaseType == InhetoBaseTypes.EndOfNamesHeader || sib.ParentIndex < Index) return default;
            goto TryNext;
        }

        public string PropPath
        {
            get
            {
                if (propPath != null) return propPath;

                // Two-pass, allocation-minimal build:
                // 1) Walk parent chain and compute total character count by reading
                //    the encoded name byte lengths and asking the Encoding for char count.
                //    We avoid constructing NameHeaderEntry for parents (which would allocate strings).
                // 2) Allocate the final string once via string.Create and decode each name
                //    directly into the target char span.

                int totalChars = Name?.Length ?? 0;

                uint parent = ParentIndex;
                while (parent != 0)
                {
                    var reader = new NameHeaderSpanReader(
                        buffer.AsReadOnlySpan,
                        checked((int)parent),
                        buffer.StringEncoding);
                    uint parentOfParent = reader.Read7BitEncodedUInt();
                    reader.SkipType();
                    int nameByteCount = reader.Read7BitEncodedInt();
                    if (nameByteCount > 0)
                    {
                        var bytes = reader.ReadSpan(nameByteCount);
                        totalChars += reader.StringEncoding.GetCharCount(bytes);
                    }

                    parent = parentOfParent;
                }

                propPath = string.Create(totalChars, this, (span, state) =>
                {
                    int writePos = span.Length;

                    // Write the current entry's name (we have the actual string already)
                    if (!string.IsNullOrEmpty(state.Name))
                    {
                        var nameSpan = state.Name.AsSpan();
                        writePos -= nameSpan.Length;
                        nameSpan.CopyTo(span.Slice(writePos, nameSpan.Length));
                    }

                    // Walk parents again, decoding bytes directly into the target span from the end.
                    uint p = state.ParentIndex;
                    var main = state.buffer;
                    while (p != 0)
                    {
                        var reader = new NameHeaderSpanReader(
                            main.AsReadOnlySpan,
                            checked((int)p),
                            main.StringEncoding);
                        uint parentOfParent = reader.Read7BitEncodedUInt();
                        reader.SkipType();
                        int nameByteCount = reader.Read7BitEncodedInt();
                        if (nameByteCount > 0)
                        {
                            var bytes = reader.ReadSpan(nameByteCount);
                            int charCount = reader.StringEncoding.GetCharCount(bytes);
                            writePos -= charCount;
                            // Decode bytes directly into the destination slice
                            reader.StringEncoding.GetChars(bytes, span.Slice(writePos, charCount));
                        }
                        p = parentOfParent;
                    }
                });

                return propPath;
            }
        }








        //------ Public Methods -----
        //======  METHODS  ======
        /// <summary>
        /// Compares the established stored-header fields without boxing or following native child pointers.<br/>
        /// Backing-buffer lengths participate, but buffer identity, value bytes and cached path text do not.<br/>
        /// Two bufferless entries compare their remaining fields; a bufferless entry differs from an attached entry.<br/>
        /// This is storage equality, not recursive semantic type equality; borrowed entries retain their existing lifetime requirements.<br/>
        /// </summary>
        /// <param name="nhe">Other stored header to compare using the same field/address semantics.<br/></param>
        /// <returns>True when all established equality fields match.<br/></returns>
        public readonly bool Equals(NameHeaderEntry nhe)
        {
            if (buffer is null || nhe.buffer is null)
            {
                if (!ReferenceEquals(buffer, nhe.buffer)) return false;
            }
            else if (buffer.Length != nhe.buffer.Length) return false;
            if (Index != nhe.Index) return false;
            if (ParentIndex != nhe.ParentIndex) return false;
            if (Kind != nhe.Kind) return false;
            if (NextEntryIndex != nhe.NextEntryIndex) return false;
            if (Name != nhe.Name) return false;
            if (Type != nhe.Type) return false;
            if (ValueOffset != nhe.ValueOffset) return false;
            return true;
        }

        /// <summary>Accepts boxed headers through the ordinary object contract and delegates to typed storage equality.<br/></summary>
        /// <param name="obj">A boxed header, or an unrelated/null object which compares unequal.<br/></param>
        /// <returns>True only for an equal stored header.<br/></returns>
        public override readonly bool Equals(object? obj) => obj is NameHeaderEntry nhe && Equals(nhe);

        /// <summary>
        /// Hashes the same six fields used by the former string hash, without allocating or accessing borrowed storage.<br/>
        /// Type metadata and buffer length remain excluded: unequal headers may collide, while equal headers always hash equally.<br/>
        /// The hash is process-local collection machinery, not a persisted wire identifier or a semantic type fingerprint.<br/>
        /// </summary>
        /// <returns>An allocation-free hash compatible with stored-header equality.<br/></returns>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Index, ParentIndex, Kind, Name, ValueOffset, NextEntryIndex);
        }

        public override string ToString() => $"Index: {Index}, ParentIndex: {ParentIndex}, Name: {Name}, Kind: {Kind}, Type: {Type}, ValueOffset: {ValueOffset}, NextEntryIndex: {NextEntryIndex}";








        // override static == and != operators
        public static bool operator ==(NameHeaderEntry x, NameHeaderEntry y) => x.Equals(y);

        public static bool operator !=(NameHeaderEntry x, NameHeaderEntry y) => !x.Equals(y);

        /// <summary>
        /// Creates an entry that borrows an InhetoBinary-owned payload stream instead of retaining a new segment object.<br/>
        /// The caller must keep the supplied stream alive for every use of the returned entry; parsing and parent traversal remain cursor-isolated through short-lived pooled segments.<br/>
        /// </summary>
        /// <param name="buffer">Long-lived payload stream owned by the active InhetoBinary instance.<br/></param>
        /// <param name="index">Entry byte offset within that payload.<br/></param>
        /// <param name="name">Optional already-resolved entry name used for duplicate-value routes.<br/></param>
        /// <returns>A name-header entry with no independently retained BufferStream segment.<br/></returns>
        internal static NameHeaderEntry CreateBorrowed(BufferStream buffer, uint index, string? name = null)
        {
            var entry = new NameHeaderEntry(buffer, index, true);
            if (name is not null)
                entry.Name = string.Intern(name);
            return entry;
        }

        /// <summary>
        /// Projects only the encoded shape of this entry without exposing its navigation state or recursive native metadata.<br/>
        /// The returned value borrows the current payload bytes; it creates no stream segment and does not move the reader cursor.<br/>
        /// This deliberately leaves the internal descriptor parser and the entry's storage layout unchanged.<br/>
        /// </summary>
        /// <returns>A read-only shape view, or the empty view for an uninitialized entry.<br/></returns>
        internal InhetoShape GetShape()
        {
            if (IsEmpty) return default;
            var memory = buffer.AsReadOnlyMemory;
            var reader = new NameHeaderSpanReader(memory.Span, checked((int)Index), buffer.StringEncoding);
            _ = reader.Read7BitEncodedUInt();
            return InhetoShape.Read(memory, reader.Position);
        }

        /// <summary>
        /// Prepares dimensions from this entry's own payload context before value construction or callbacks begin.<br/>
        /// A normal owned segment and a borrowed entry share the parser's zero basis; the special owned index-zero segment starts at one.<br/>
        /// Existing borrowed entries must not be retained across rebind, overwrite, disposal or pool return; no new lifetime guarantee is added.<br/>
        /// </summary>
        /// <param name="destination">Caller storage for the array rank; preparation performs no managed allocation.<br/></param>
        /// <returns>The number of MD dimensions written, or zero for a non-MD entry.<br/></returns>
        internal readonly int CopyLengths(Span<int> destination)
        {
            var payload = buffer.AsReadOnlyMemory;
            return Type.ArrayInfo.CopyLengths(payload.Span, destination, !bufferBorrowed && Index == 0 ? 1 : 0);
        }

        /// <summary>
        /// Reads a navigation-only header and validates its encoded type through the existing skip-only grammar.<br/>
        /// No recursive type, native child storage, stream segment or MD dimension array is constructed.<br/>
        /// A temporary entry reuses the established PropPath implementation but never escapes with its borrowed stream.<br/>
        /// </summary>
        /// <param name="buffer">Borrowed payload whose bytes remain stable throughout the synchronous scan.<br/></param>
        /// <param name="index">Name-header byte offset after the format-version byte.<br/></param>
        /// <param name="next">The next header offset, or zero for a fully read end marker.<br/></param>
        /// <returns>Captured immutable navigation fields; the end marker returns the default value.<br/></returns>
        internal static NameHeaderNavigation ReadNavigation(BufferStream buffer, uint index, out uint next)
        {
            var reader = new NameHeaderSpanReader(buffer.AsReadOnlySpan, checked((int)index), buffer.StringEncoding);
            var entry = new NameHeaderEntry { buffer = buffer, bufferBorrowed = true, Index = index };
            entry.ParentIndex = reader.Read7BitEncodedUInt();
            byte marker = reader.PeekByte();
            reader.SkipType();
            entry.Name = string.Intern(reader.ReadString()!);
            _ = reader.ReadUInt32();
            next = marker == (byte)InhetoBaseTypes.EndOfNamesHeader ? 0 : checked((uint)reader.Position);
            return next == 0 ? default : new NameHeaderNavigation(entry.Index, entry.ParentIndex, entry.Name, entry.PropPath);
        }








    }








    internal partial record struct NameHeaderEntryType
    {








        //======  FIELDS  ======
        public InhetoBaseTypes BaseType;
        public InhetoBaseTypes EnumType;
        public bool IsNullable;
        public InhetoKindMasks Kind;
        public InhetoBaseTypes NullableBaseType;
        public InhetoPromotedTypes PromotedType;
        // Preserve the established native offsets after removing the managed dimension field.
        public NameHeaderArrayEntry ArrayInfo;
        public NameHeaderCollectionEntry CollectionInfo;
        public NameHeaderEnumerableEntry EnumerableInfo;








        //======  CONSTRUCTORS  ======
        public NameHeaderEntryType(BufferStream buffer)
        {
            this = default;
            Kind = (InhetoKindMasks)(buffer.PeekByte() & 0b1100_0000);
            switch (Kind)
            {
                case InhetoKindMasks.InhetoBaseType:
                    BaseType = (InhetoBaseTypes)(buffer.ReadByte() & 0b0011_1111);
                    break;
                case InhetoKindMasks.InhetoNullableBaseType:
                    IsNullable = true;
                    NullableBaseType = (InhetoBaseTypes)(buffer.ReadByte() & 0b0011_1111);
                    break;
                case InhetoKindMasks.InhetoPromotedType:
                    var pType = (InhetoPromotedTypes)(buffer.ReadByte() & 0b0011_1111);
                    PromotedType = pType;
                    switch (pType)
                    {
                        case InhetoPromotedTypes.Enum:
                            EnumType = (InhetoBaseTypes)(buffer.ReadByte() & 0b0011_1111);
                            break;
                        case InhetoPromotedTypes.Array:
                        case InhetoPromotedTypes.ArrayMD:
                            ArrayInfo = new(buffer, (InhetoArrayTypes)pType);
                            break;
                        case InhetoPromotedTypes.Enumerable:
                            EnumerableInfo = new(buffer);
                            break;
                        default:
                            break;
                    }
                    break;
                case InhetoKindMasks.InhetoCollectionType:
                    CollectionInfo = new(buffer);
                    break;

            }
        }








        //======  PROPERTIES  ======
        public static NameHeaderEntryType StringType { get => new() { Kind = InhetoKindMasks.InhetoBaseType, BaseType = InhetoBaseTypes.String }; }








        //------ Public Methods -----
        //======  METHODS  ======
        public override string? ToString()
        {
            switch (Kind)
            {
                case InhetoKindMasks.InhetoBaseType:
                    return $"{BaseType}";
                case InhetoKindMasks.InhetoPromotedType:
                    return $"{PromotedType}";
                case InhetoKindMasks.InhetoNullableBaseType:
                    return $"Nullable<{NullableBaseType}>";
                case InhetoKindMasks.InhetoCollectionType:
                    return $"{CollectionInfo.CollectionType}";
            }
            return base.ToString();
        }

        /// <summary>
        /// Parses one recursive name-entry type descriptor from an allocation-free payload cursor.<br/>
        /// The resulting public type metadata is identical to the BufferStream constructor while scalar payloads avoid creating a stream segment.<br/>
        /// </summary>
        /// <param name="reader">Borrowed payload cursor positioned at the type marker.<br/></param>
        /// <returns>The fully parsed type descriptor.<br/></returns>
        internal static NameHeaderEntryType ReadBorrowed(ref NameHeaderSpanReader reader)
        {
            var result = default(NameHeaderEntryType);
            result.Kind = (InhetoKindMasks)(reader.PeekByte() & 0b1100_0000);
            switch (result.Kind)
            {
                case InhetoKindMasks.InhetoBaseType:
                    result.BaseType = (InhetoBaseTypes)(reader.ReadByte() & 0b0011_1111);
                    break;
                case InhetoKindMasks.InhetoNullableBaseType:
                    result.IsNullable = true;
                    result.NullableBaseType = (InhetoBaseTypes)(reader.ReadByte() & 0b0011_1111);
                    break;
                case InhetoKindMasks.InhetoPromotedType:
                    var promotedType = (InhetoPromotedTypes)(reader.ReadByte() & 0b0011_1111);
                    result.PromotedType = promotedType;
                    switch (promotedType)
                    {
                        case InhetoPromotedTypes.Enum:
                            result.EnumType = (InhetoBaseTypes)(reader.ReadByte() & 0b0011_1111);
                            break;
                        case InhetoPromotedTypes.Array:
                        case InhetoPromotedTypes.ArrayMD:
                            result.ArrayInfo = NameHeaderArrayEntry.ReadBorrowed(ref reader, (InhetoArrayTypes)promotedType);
                            break;
                        case InhetoPromotedTypes.Enumerable:
                            result.EnumerableInfo = NameHeaderEnumerableEntry.ReadBorrowed(ref reader);
                            break;
                    }
                    break;
                case InhetoKindMasks.InhetoCollectionType:
                    result.CollectionInfo = NameHeaderCollectionEntry.ReadBorrowed(ref reader);
                    break;
            }
            return result;
        }








    }








    internal unsafe partial struct NameHeaderEnumerableEntry
    {








        //======  FIELDS  ======
        private NameHeaderEntryType* ElementTypePtr;
        public NameHeaderEntryType ElementType
        {
            get
            {
                if (ElementTypePtr == null) return default;
                return *ElementTypePtr;
            }
            private set => ElementTypePtr = (NameHeaderEntryType*)NameHeaderTypeStorage.Create(in value);
        }








        //======  CONSTRUCTORS  ======
        public NameHeaderEnumerableEntry(BufferStream buffer)
        {
            ElementType = new(buffer);
        }

        /// <summary>
        /// Parses an enumerable element descriptor from an allocation-free payload cursor.<br/>
        /// Nested type metadata retains the established unmanaged representation used by the public structure.<br/>
        /// </summary>
        /// <param name="reader">Borrowed payload cursor positioned at the enumerable element descriptor.<br/></param>
        /// <returns>The parsed enumerable metadata.<br/></returns>
        internal static NameHeaderEnumerableEntry ReadBorrowed(ref NameHeaderSpanReader reader)
        {
            var result = default(NameHeaderEnumerableEntry);
            result.ElementType = NameHeaderEntryType.ReadBorrowed(ref reader);
            return result;
        }













    }








    internal unsafe partial struct NameHeaderArrayEntry
    {








        //======  FIELDS  ======
        // Numeric locations use the former reference-sized slot; no managed reference may enter native children.
        internal int LengthsOffset;
        internal int LengthsBytes;
        private NameHeaderEntryType* ElementTypePtr;
        public InhetoArrayTypes ArrayType;
        public byte Depth;
        public NameHeaderEntryType ElementType
        {
            get
            {
                if (ElementTypePtr == null) return default;
                return *ElementTypePtr;
            }
            private set
            {
                ValidateElementType(value);
                ElementTypePtr = (NameHeaderEntryType*)NameHeaderTypeStorage.Create(in value);
            }
        }
        public byte Rank;








        //======  CONSTRUCTORS  ======
        public NameHeaderArrayEntry(BufferStream buffer, InhetoArrayTypes arrayType)
        {
            this = default;
            ArrayType = arrayType;
            Rank = ArrayType == InhetoArrayTypes.Array ? (byte)1 : buffer.ReadByte();
            Depth = ArrayType == InhetoArrayTypes.Array ? buffer.ReadByte() : (byte)1;
            ElementType = new(buffer);
            if (arrayType == InhetoArrayTypes.ArrayMD)
            {
                LengthsOffset = checked((int)buffer.Position);
                for (int i = 0; i < Rank; i++)
                {
                    _ = buffer.Read7BitEncodedInt();
                }
                LengthsBytes = checked((int)buffer.Position) - LengthsOffset;
            }
        }

        /// <summary>
        /// Parses array rank, depth, element type, and multidimensional lengths from an allocation-free payload cursor.<br/>
        /// MD lengths are consumed for validation/cursor parity; only numeric offset and encoded extent are retained.<br/>
        /// </summary>
        /// <param name="reader">Borrowed payload cursor positioned immediately after the promoted array marker.<br/></param>
        /// <param name="arrayType">Single- or multidimensional array classification encoded by the promoted marker.<br/></param>
        /// <returns>The parsed array metadata.<br/></returns>
        internal static NameHeaderArrayEntry ReadBorrowed(
            ref NameHeaderSpanReader reader,
            InhetoArrayTypes arrayType)
        {
            var result = default(NameHeaderArrayEntry);
            result.ArrayType = arrayType;
            result.Rank = arrayType == InhetoArrayTypes.Array ? (byte)1 : reader.ReadByte();
            result.Depth = arrayType == InhetoArrayTypes.Array ? reader.ReadByte() : (byte)1;
            result.ElementType = NameHeaderEntryType.ReadBorrowed(ref reader);
            if (arrayType == InhetoArrayTypes.ArrayMD)
            {
                result.LengthsOffset = reader.Position;
                for (int dimension = 0; dimension < result.Rank; dimension++)
                    _ = reader.Read7BitEncodedInt();
                result.LengthsBytes = reader.Position - result.LengthsOffset;
            }
            return result;
        }






        /// <summary>
        /// Rejects control-only base types where a typed array element descriptor is required.<br/>
        /// Array nullability is represented by the containing value, while null elements retain the array's declared CLR element type.<br/>
        /// </summary>
        /// <param name="elementType">The decoded or internally constructed array element descriptor.<br/></param>
        /// <exception cref="InvalidDataException">The descriptor declares <see cref="InhetoBaseTypes.Unknown"/> or <see cref="InhetoBaseTypes.Null"/> as its element type.<br/></exception>
        private static void ValidateElementType(in NameHeaderEntryType elementType)
        {
            if (elementType.Kind == InhetoKindMasks.InhetoBaseType
                && (elementType.BaseType == InhetoBaseTypes.Unknown
                    || elementType.BaseType == InhetoBaseTypes.Null))
            {
                throw new InvalidDataException(
                    $"An array descriptor cannot declare {elementType.BaseType} as its element type.");
            }
        }

        /// <summary>
        /// Decodes this MD descriptor's dimensions once into caller-owned storage without allocating or moving a shared cursor.<br/>
        /// The payload must be the same stable logical byte view used by parsing; numeric bounds cannot establish payload identity.<br/>
        /// Descriptor copies borrow both the native child lifetime and the payload lifetime; this does not create a detached snapshot.<br/>
        /// </summary>
        /// <param name="payload">Matching encoded payload, valid and unchanged for this synchronous read.<br/></param>
        /// <param name="destination">Storage for at least Rank integers; ordinary arrays return zero without writing.<br/></param>
        /// <param name="payloadOrigin">Parser-coordinate offset corresponding to payload[0], normally zero.<br/></param>
        /// <returns>The number of copied dimensions; no materialization rank/count policy is imposed here.<br/></returns>
        internal readonly int CopyLengths(ReadOnlySpan<byte> payload, Span<int> destination, int payloadOrigin = 0)
        {
            if (ArrayType != InhetoArrayTypes.ArrayMD) return 0;
            if (destination.Length < Rank) throw new ArgumentException("The destination is smaller than the array rank.", nameof(destination));
            var reader = new NameHeaderSpanReader(payload.Slice(checked(LengthsOffset - payloadOrigin), LengthsBytes), 0, Encoding.UTF8);
            for (int dimension = 0; dimension < Rank; dimension++) destination[dimension] = reader.Read7BitEncodedInt();
            if (reader.Position != LengthsBytes) throw new InvalidDataException("The MD dimension extent does not match its rank.");
            return Rank;
        }




    }








    internal unsafe partial struct NameHeaderCollectionEntry
    {








        //======  FIELDS  ======
        public InhetoCollectionTypes CollectionType;
        public InhetoComparerTypes Comparer;
        private NameHeaderEntryType* KeyTypePtr;
        public NameHeaderEntryType KeyType
        {
            get
            {
                if (KeyTypePtr == null) return default;
                return *KeyTypePtr;
            }
            private set => KeyTypePtr = (NameHeaderEntryType*)NameHeaderTypeStorage.Create(in value);
        }
        private NameHeaderEntryType* ValueTypePtr;
        public NameHeaderEntryType ValueType
        {
            get
            {
                if (ValueTypePtr == null) return default;
                return *ValueTypePtr;
            }
            private set => ValueTypePtr = (NameHeaderEntryType*)NameHeaderTypeStorage.Create(in value);
        }








        //======  CONSTRUCTORS  ======
        public NameHeaderCollectionEntry(BufferStream buffer)
        {
            var typeByte = buffer.ReadByte();
            CollectionType = (InhetoCollectionTypes)(typeByte & 0b0001_1111);
            var hasComparer = (typeByte & (byte)0b0010_0000) != 0;
            Comparer = hasComparer ? (InhetoComparerTypes)(buffer.ReadByte()) : InhetoComparerTypes.None;
            switch (CollectionType)
            {
                case InhetoCollectionTypes.Dictionary:
                case InhetoCollectionTypes.ConcurrentDictionary:
                case InhetoCollectionTypes.SortedDictionary:
                case InhetoCollectionTypes.SortedList:
                case InhetoCollectionTypes.StringKeyDictionary:
                case InhetoCollectionTypes.StringKeyConcurrentDictionary:
                case InhetoCollectionTypes.StringKeySortedDictionary:
                case InhetoCollectionTypes.StringKeySortedList:
                    KeyType = CollectionType >= InhetoCollectionTypes.StringKeyDictionary ? NameHeaderEntryType.StringType : new(buffer);
                    break;

            }
            ValueType = new(buffer);
        }

        /// <summary>
        /// Parses collection kind, optional comparer, key type, and value type from an allocation-free payload cursor.<br/>
        /// String-key collections retain their implicit string key descriptor; other dictionary shapes parse the encoded key recursively.<br/>
        /// </summary>
        /// <param name="reader">Borrowed payload cursor positioned immediately after the collection type marker.<br/></param>
        /// <returns>The parsed collection metadata.<br/></returns>
        internal static NameHeaderCollectionEntry ReadBorrowed(ref NameHeaderSpanReader reader)
        {
            var result = default(NameHeaderCollectionEntry);
            byte typeByte = reader.ReadByte();
            result.CollectionType = (InhetoCollectionTypes)(typeByte & 0b0001_1111);
            bool hasComparer = (typeByte & 0b0010_0000) != 0;
            result.Comparer = hasComparer
                ? (InhetoComparerTypes)reader.ReadByte()
                : InhetoComparerTypes.None;
            switch (result.CollectionType)
            {
                case InhetoCollectionTypes.Dictionary:
                case InhetoCollectionTypes.ConcurrentDictionary:
                case InhetoCollectionTypes.SortedDictionary:
                case InhetoCollectionTypes.SortedList:
                case InhetoCollectionTypes.StringKeyDictionary:
                case InhetoCollectionTypes.StringKeyConcurrentDictionary:
                case InhetoCollectionTypes.StringKeySortedDictionary:
                case InhetoCollectionTypes.StringKeySortedList:
                    result.KeyType = result.CollectionType >= InhetoCollectionTypes.StringKeyDictionary
                        ? NameHeaderEntryType.StringType
                        : NameHeaderEntryType.ReadBorrowed(ref reader);
                    break;
            }
            result.ValueType = NameHeaderEntryType.ReadBorrowed(ref reader);
            return result;
        }








    }

    /// <summary>
    /// Provides a stack-only cursor over one borrowed Inheto payload.<br/>
    /// It mirrors the primitive operations used by names-header parsing without allocating or mutating a shared BufferStream cursor.<br/>
    /// </summary>
    internal ref struct NameHeaderSpanReader
    {
        private readonly ReadOnlySpan<byte> payload;
        private int position;

        /// <summary>
        /// Creates a cursor over an existing payload at one validated logical offset.<br/>
        /// </summary>
        /// <param name="payload">Complete borrowed Inheto payload bytes.<br/></param>
        /// <param name="position">Initial zero-based byte offset.<br/></param>
        /// <param name="stringEncoding">Encoding used by the owning BufferStream for length-prefixed strings.<br/></param>
        internal NameHeaderSpanReader(
            ReadOnlySpan<byte> payload,
            int position,
            Encoding stringEncoding)
        {
            if ((uint)position > (uint)payload.Length)
                throw new ArgumentOutOfRangeException(nameof(position));
            this.payload = payload;
            this.position = position;
            StringEncoding = stringEncoding ?? throw new ArgumentNullException(nameof(stringEncoding));
        }

        /// <summary>Gets the current zero-based payload offset.<br/></summary>
        internal int Position => position;

        /// <summary>Gets the payload string encoding inherited from the owning stream.<br/></summary>
        internal Encoding StringEncoding { get; }

        /// <summary>
        /// Returns the next byte without advancing the cursor.<br/>
        /// </summary>
        /// <returns>The byte at the current position.<br/></returns>
        internal byte PeekByte()
        {
            if ((uint)position >= (uint)payload.Length)
                throw new EndOfStreamException();
            return payload[position];
        }

        /// <summary>
        /// Reads one byte and advances the cursor.<br/>
        /// </summary>
        /// <returns>The consumed byte.<br/></returns>
        internal byte ReadByte()
        {
            byte value = PeekByte();
            position++;
            return value;
        }

        /// <summary>
        /// Reads one little-endian UInt32 and advances the cursor by four bytes.<br/>
        /// </summary>
        /// <returns>The decoded value.<br/></returns>
        internal uint ReadUInt32()
        {
            ReadOnlySpan<byte> bytes = ReadSpan(sizeof(uint));
            return System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bytes);
        }

        /// <summary>
        /// Reads one unsigned 7-bit encoded UInt32 using the BufferStream wire representation.<br/>
        /// </summary>
        /// <returns>The decoded unsigned value.<br/></returns>
        internal uint Read7BitEncodedUInt()
        {
            uint value = 0;
            for (int shift = 0; shift < 35; shift += 7)
            {
                byte current = ReadByte();
                if (shift == 28 && (current & 0xF0) != 0)
                    throw new FormatException("The names header contains an invalid UInt32 7-bit encoding.");
                value |= (uint)(current & 0x7F) << shift;
                if ((current & 0x80) == 0)
                    return value;
            }
            throw new FormatException("The names header contains an unterminated UInt32 7-bit encoding.");
        }

        /// <summary>
        /// Reads one zigzag-encoded Int32 using the BufferStream wire representation.<br/>
        /// </summary>
        /// <returns>The decoded signed value.<br/></returns>
        internal int Read7BitEncodedInt()
        {
            uint raw = Read7BitEncodedUInt();
            return unchecked((int)(raw >> 1)) ^ -unchecked((int)(raw & 1));
        }

        /// <summary>
        /// Reads a length-prefixed encoded string and advances past its payload bytes.<br/>
        /// </summary>
        /// <returns>The decoded string, or <see langword="null"/> for the encoded negative-length marker.<br/></returns>
        internal string? ReadString()
        {
            int byteCount = Read7BitEncodedInt();
            if (byteCount < 0)
                return null;
            return StringEncoding.GetString(ReadSpan(byteCount));
        }

        /// <summary>
        /// Borrows a bounded byte slice at the current position and advances by its length.<br/>
        /// </summary>
        /// <param name="count">Number of bytes to consume.<br/></param>
        /// <returns>The requested borrowed payload span.<br/></returns>
        internal ReadOnlySpan<byte> ReadSpan(int count)
        {
            if (count < 0 || position > payload.Length - count)
                throw new EndOfStreamException();
            ReadOnlySpan<byte> result = payload.Slice(position, count);
            position += count;
            return result;
        }

        /// <summary>
        /// Advances past one encoded type without constructing decoded metadata or allocating child storage.<br/>
        /// Used when parent navigation needs the following name, not a retained type descriptor.<br/>
        /// Mirrors ReadBorrowed grammar and validation, including single-marker collections and MD length suffixes.<br/>
        /// Scalar parents return after one marker; recursive shapes retain the existing parser's call-depth model.<br/>
        /// This does not change ownership or representation of descriptors that callers actually retain.<br/>
        /// </summary>
        /// <exception cref="EndOfStreamException">Required descriptor bytes are missing.<br/></exception>
        /// <exception cref="FormatException">An MD length has an invalid 7-bit UInt32 encoding.<br/></exception>
        /// <exception cref="InvalidDataException">An array declares Unknown or Null as its element type.<br/></exception>
        internal void SkipType()
        {
            byte marker = ReadByte();
            switch ((InhetoKindMasks)(marker & 0xc0))
            {
                case InhetoKindMasks.InhetoPromotedType:
                    switch ((InhetoPromotedTypes)(marker & 0x3f))
                    {
                        case InhetoPromotedTypes.Enum:
                            _ = ReadByte();
                            break;
                        case InhetoPromotedTypes.Array:
                        case InhetoPromotedTypes.ArrayMD:
                            int rankOrDepth = ReadByte();
                            byte elementMarker = PeekByte();
                            SkipType();
                            if (elementMarker is (byte)InhetoBaseTypes.Unknown or (byte)InhetoBaseTypes.Null)
                                throw new InvalidDataException($"An array descriptor cannot declare {(InhetoBaseTypes)elementMarker} as its element type.");
                            if ((marker & 0x3f) == (byte)InhetoPromotedTypes.ArrayMD)
                                for (int dimension = 0; dimension < rankOrDepth; dimension++) _ = Read7BitEncodedInt();
                            break;
                        case InhetoPromotedTypes.Enumerable:
                            SkipType();
                            break;
                    }
                    break;
                case InhetoKindMasks.InhetoCollectionType:
                    if ((marker & 0x20) != 0) _ = ReadByte();
                    if ((InhetoCollectionTypes)(marker & 0x1f) is InhetoCollectionTypes.Dictionary
                        or InhetoCollectionTypes.ConcurrentDictionary or InhetoCollectionTypes.SortedDictionary
                        or InhetoCollectionTypes.SortedList) SkipType();
                    SkipType();
                    break;
            }
        }
    }
}

namespace Inheto
{
    /// <summary>
    /// Supplies the original native child pointers, sharing immutable terminal descriptors from a fixed wire-vocabulary table.<br/>
    /// Zero remains the absent/default child; recursive metadata retains its original independent native allocation.<br/>
    /// Only parser-normalized, construction-time descriptors may enter this internal allocation boundary. Published children are immutable.<br/>
    /// This removes per-parse terminal-child allocations without adding a payload cache, owner handle, or wire-format change.<br/>
    /// Recursive native allocation lifetime and embedded managed MD references remain separate unresolved concerns.<br/>
    /// </summary>
    internal static unsafe class NameHeaderTypeStorage
    {
        /// <summary>
        /// Selects parser-produced base, nullable, enum and other nonrecursive promoted descriptors from one fixed native table.<br/>
        /// The first terminal child initializes that table; subsequent terminal selections allocate nothing.<br/>
        /// Arrays, enumerables and collections retain one original native child block; no reclamation guarantee is added for that fallback.<br/>
        /// The slot is assigned only by private construction setters; no caller can mutate the shared table through a child property.<br/>
        /// </summary>
        /// <param name="value">A fully parsed normalized descriptor, not arbitrary combinations of independently mutated family fields.<br/></param>
        /// <returns>An immutable terminal-table pointer or recursive native child slot.<br/></returns>
        internal static nint Create(in NameHeaderEntryType value)
        {
            if (value.Kind != InhetoKindMasks.InhetoCollectionType
                && (value.Kind != InhetoKindMasks.InhetoPromotedType
                    || value.PromotedType is not (InhetoPromotedTypes.Array or InhetoPromotedTypes.ArrayMD or InhetoPromotedTypes.Enumerable)))
            {
                byte type = value.Kind switch
                {
                    InhetoKindMasks.InhetoBaseType => (byte)value.BaseType,
                    InhetoKindMasks.InhetoNullableBaseType => (byte)value.NullableBaseType,
                    _ => (byte)value.PromotedType
                };
                int index = value.Kind == InhetoKindMasks.InhetoPromotedType && value.PromotedType == InhetoPromotedTypes.Enum
                    ? 192 + (byte)value.EnumType
                    : (byte)value.Kind | type;
                if ((uint)index >= 256)
                    throw new InvalidDataException("A terminal child must contain a parser-normalized type marker.");
                return (nint)(TerminalTable.Start + (nuint)(index * sizeof(NameHeaderEntryType)));
            }

            var pointer = (NameHeaderEntryType*)System.Runtime.InteropServices.Marshal.AllocHGlobal(sizeof(NameHeaderEntryType));
            *pointer = value;
            return (nint)pointer;
        }

        /// <summary>
        /// Owns exactly 256 native terminal slots through runtime type-associated storage, never through a payload or developer-object cache.<br/>
        /// Slots 0-191 represent one-byte terminal markers; slots 192-255 represent the 64 masked enum underlying markers.<br/>
        /// Reserved recursive-marker slots are never selected. Every slot has null managed fields and no child pointers.<br/>
        /// </summary>
        private static class TerminalTable
        {
            internal static readonly nuint Start;

            /// <summary>Defers table creation until an actual terminal child needs it, excluding scalar-only root parsing.<br/></summary>
            static TerminalTable() => Start = Build();

            /// <summary>
            /// Initializes every slot before static publication, using the same normalized fields as the parser.<br/>
            /// Runtime type-associated allocation is reclaimed if the owning type is unloaded and must not be manually freed.<br/>
            /// No managed dimension reference is ever stored in this terminal-only table.<br/>
            /// </summary>
            private static nuint Build()
            {
                var start = (NameHeaderEntryType*)System.Runtime.CompilerServices.RuntimeHelpers.AllocateTypeAssociatedMemory(
                    typeof(NameHeaderEntryType), checked(256 * sizeof(NameHeaderEntryType)));
                for (int index = 0; index < 256; index++)
                {
                    int marker = index < 192 ? index : 0x80 | (byte)InhetoPromotedTypes.Enum;
                    var terminal = default(NameHeaderEntryType);
                    terminal.Kind = (InhetoKindMasks)(marker & 0xc0);
                    switch (terminal.Kind)
                    {
                        case InhetoKindMasks.InhetoBaseType:
                            terminal.BaseType = (InhetoBaseTypes)(marker & 0x3f);
                            break;
                        case InhetoKindMasks.InhetoNullableBaseType:
                            terminal.IsNullable = true;
                            terminal.NullableBaseType = (InhetoBaseTypes)(marker & 0x3f);
                            break;
                        case InhetoKindMasks.InhetoPromotedType:
                            terminal.PromotedType = (InhetoPromotedTypes)(marker & 0x3f);
                            if (index >= 192) terminal.EnumType = (InhetoBaseTypes)(index - 192);
                            break;
                    }
                    start[index] = terminal;
                }
                return (nuint)start;
            }
        }
    }

    /// <summary>
    /// Captured navigation data for retained name maps; contains no stream, encoded payload, native pointer or decoded type tree.<br/>
    /// Name/path strings preserve the observed header spelling and parent concatenation semantics.<br/>
    /// </summary>
    internal readonly struct NameHeaderNavigation
    {
        internal readonly uint Index, ParentIndex;
        internal readonly string Name, PropPath;

        /// <summary>Captures one successfully read entry's immutable navigation fields without retaining its parser state.<br/></summary>
        /// <param name="index">This entry's payload offset.<br/></param>
        /// <param name="parentIndex">Its parent's payload offset, or zero at the root.<br/></param>
        /// <param name="name">Interned encoded name, including its entry-kind prefix.<br/></param>
        /// <param name="propPath">Already resolved full path, independent of later payload mutation.<br/></param>
        internal NameHeaderNavigation(uint index, uint parentIndex, string name, string propPath)
        { Index = index; ParentIndex = parentIndex; Name = name; PropPath = propPath; }

        /// <summary>Removes exactly the same first-character entry prefix as NameHeaderEntry.CleanName.<br/></summary>
        internal string CleanName => Name == "" ? "" : string.Intern(Name.Substring(1));
    }
}
