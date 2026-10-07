using System.Buffers;

namespace Inheto;

/// <summary>
/// Provides read-only inspection of an encoded Inheto shape, not a CLR identity or mutable names-header entry.<br/>
/// Values and child views borrow their payload bytes: the caller must keep those bytes alive and unchanged throughout inspection.<br/>
/// Copying this value does not detach its bytes, and a reader's shallow clone does not protect shared bytes from overwrite or pool reuse.<br/>
/// Ordinary inspection allocates no objects, child nodes, or dimension arrays; exceptionally deep branching metadata may rent temporary scanner scratch.<br/>
/// Default is an absent view, distinct from an encoded null value. Inactive shape families return default metadata.<br/>
/// </summary>
public readonly struct InhetoShape
{
    private readonly ReadOnlyMemory<byte> bytes;
    // One process-wide implicit key marker; no array is created per projection or key access.
    private static readonly byte[] stringDescriptor = [(byte)InhetoBaseTypes.String];

    /// <summary>
    /// Wraps an already bounded descriptor slice without copying or retaining mutable parser state.<br/>
    /// Only internal parsing paths may construct views; public callers receive them from Inheto outputs.<br/>
    /// </summary>
    /// <param name="bytes">Exactly one encoded descriptor whose bytes must remain unchanged while borrowed.<br/></param>
    private InhetoShape(ReadOnlyMemory<byte> bytes) => this.bytes = bytes;

    /// <summary>Whether this is an absent/default projection rather than an encoded shape.<br/></summary>
    public bool IsEmpty => bytes.IsEmpty;
    /// <summary>The encoded shape family; default is returned for an absent projection.<br/></summary>
    public InhetoKindRoute Kind => (InhetoKindRoute)(Marker & 0xc0);
    /// <summary>Whether the descriptor declares a nullable base leaf, independently of the current value.<br/></summary>
    public bool IsNullable => !IsEmpty && Kind == InhetoKindRoute.Nullable;
    /// <summary>The base or nullable-underlying base classification; Unknown for other families or absence.<br/></summary>
    public InhetoBaseTypes BaseType => Kind is InhetoKindRoute.Base or InhetoKindRoute.Nullable
        ? (InhetoBaseTypes)(Marker & 0x3f) : InhetoBaseTypes.Unknown;
    /// <summary>The promoted shape classification, or None for other families.<br/></summary>
    public InhetoPromotedTypes PromotedType => Kind == InhetoKindRoute.Promoted
        ? (InhetoPromotedTypes)(Marker & 0x3f) : InhetoPromotedTypes.None;
    /// <summary>The encoded enum backing type, or Unknown when this is not an enum descriptor.<br/></summary>
    public InhetoBaseTypes EnumType => PromotedType == InhetoPromotedTypes.Enum
        ? (InhetoBaseTypes)(bytes.Span[1] & 0x3f) : InhetoBaseTypes.Unknown;
    /// <summary>Whether the shape is an ordinary/jagged or multidimensional array.<br/></summary>
    public bool IsArray => ArrayType != InhetoArrayTypes.NonArray;
    /// <summary>The array family, or NonArray for another shape.<br/></summary>
    public InhetoArrayTypes ArrayType => PromotedType is InhetoPromotedTypes.Array or InhetoPromotedTypes.ArrayMD
        ? (InhetoArrayTypes)PromotedType : InhetoArrayTypes.NonArray;
    /// <summary>Array rank; one for ordinary/jagged arrays and zero for non-array shapes.<br/></summary>
    public int Rank => ArrayType == InhetoArrayTypes.Array ? 1 : ArrayType == InhetoArrayTypes.ArrayMD ? bytes.Span[1] : 0;
    /// <summary>Encoded jagged depth; one for MD arrays and zero for non-array shapes.<br/></summary>
    public int Depth => ArrayType == InhetoArrayTypes.Array ? bytes.Span[1] : ArrayType == InhetoArrayTypes.ArrayMD ? 1 : 0;
    /// <summary>The encoded generic collection family, or None for other shapes.<br/></summary>
    public InhetoCollectionTypes CollectionType => Kind == InhetoKindRoute.Collection
        ? (InhetoCollectionTypes)(Marker & 0x1f) : InhetoCollectionTypes.None;
    /// <summary>The encoded comparer classification, or None when no comparer metadata is present.<br/></summary>
    public InhetoComparerTypes Comparer => Kind == InhetoKindRoute.Collection && (Marker & 0x20) != 0
        ? (InhetoComparerTypes)bytes.Span[1] : InhetoComparerTypes.None;
    /// <summary>The array/enumerable leaf or collection value shape; absent when there is no declared element descriptor.<br/></summary>
    public InhetoShape ElementType => IsArray ? Child(2)
        : PromotedType == InhetoPromotedTypes.Enumerable ? Child(1)
        : Kind == InhetoKindRoute.Collection ? ValueType : default;
    /// <summary>The dictionary key shape, including the implicit string key; absent for non-dictionary shapes.<br/></summary>
    public InhetoShape KeyType => HasExplicitKey(CollectionType) ? Child(CollectionChildrenOffset)
        : HasStringKey(CollectionType) ? new InhetoShape(stringDescriptor) : default;
    /// <summary>The generic collection value shape, or an absent projection for other shape families.<br/></summary>
    public InhetoShape ValueType
    {
        get
        {
            if (Kind != InhetoKindRoute.Collection) return default;
            int offset = CollectionChildrenOffset;
            if (HasExplicitKey(CollectionType)) Skip(bytes.Span, ref offset);
            return Child(offset);
        }
    }

    private byte Marker => IsEmpty ? (byte)0 : bytes.Span[0];
    private int CollectionChildrenOffset => (Marker & 0x20) == 0 ? 1 : 2;

    /// <summary>
    /// Decodes all MD dimension lengths in one forward pass into caller-owned storage.<br/>
    /// Ordinary/jagged arrays have no lengths in their type descriptor and return zero without writing.<br/>
    /// Prefer this method over repeated GetLength calls when consuming multiple dimensions.<br/>
    /// </summary>
    /// <param name="destination">Storage for at least Rank integers for an MD shape; any remaining elements are untouched.<br/></param>
    /// <returns>The number of lengths written, or zero for a non-MD shape.<br/></returns>
    /// <exception cref="ArgumentException">The destination is shorter than the MD rank.<br/></exception>
    public int CopyLengths(Span<int> destination)
    {
        if (ArrayType != InhetoArrayTypes.ArrayMD) return 0;
        int rank = Rank;
        if (destination.Length < rank) throw new ArgumentException("Destination is shorter than the array rank.", nameof(destination));
        int cursor = 2;
        Skip(bytes.Span, ref cursor);
        for (int dimension = 0; dimension < rank; dimension++) destination[dimension] = ReadLength(bytes.Span, ref cursor);
        return rank;
    }

    /// <summary>
    /// Reads one MD dimension without allocating an array; the preceding encoded lengths are scanned in order.<br/>
    /// This is not constant-time indexed storage: use CopyLengths for a full set of dimensions.<br/>
    /// Ordinary/jagged array counts belong to their values and are not available from this descriptor.<br/>
    /// </summary>
    /// <param name="dimension">Zero-based MD dimension index.<br/></param>
    /// <returns>The encoded length, including zero for an empty dimension.<br/></returns>
    /// <exception cref="ArgumentOutOfRangeException">The shape is not MD or the dimension is outside its rank.<br/></exception>
    public int GetLength(int dimension)
    {
        if (ArrayType != InhetoArrayTypes.ArrayMD || (uint)dimension >= (uint)Rank)
            throw new ArgumentOutOfRangeException(nameof(dimension));
        int cursor = 2, length = 0;
        Skip(bytes.Span, ref cursor);
        for (int index = 0; index <= dimension; index++) length = ReadLength(bytes.Span, ref cursor);
        return length;
    }

    /// <summary>
    /// Selects one recursively bounded child without allocating child metadata or copying its bytes.<br/>
    /// The child shares its parent's borrowed lifetime.<br/>
    /// </summary>
    /// <param name="offset">Descriptor-relative start of the selected child.<br/></param>
    /// <returns>A value projection of exactly that child's encoding.<br/></returns>
    private InhetoShape Child(int offset) => Read(bytes, offset);

    /// <summary>
    /// Bounds exactly one descriptor in a larger payload using the existing single-marker collection grammar.<br/>
    /// This reader does not touch the legacy recursive native metadata or the payload cursor.<br/>
    /// </summary>
    /// <param name="payload">Borrowed payload or parent descriptor memory.<br/></param>
    /// <param name="offset">Start of the descriptor, after the header's parent-index encoding.<br/></param>
    /// <returns>A view over exactly one encoded shape.<br/></returns>
    internal static InhetoShape Read(ReadOnlyMemory<byte> payload, int offset)
    {
        int end = offset;
        Skip(payload.Span, ref end);
        return new InhetoShape(payload.Slice(offset, end - offset));
    }

    /// <summary>Identifies dictionary families whose key descriptor is explicitly encoded.<br/></summary>
    /// <param name="type">Encoded generic collection family.<br/></param>
    /// <returns>True only for the four non-string-key dictionary encodings.<br/></returns>
    private static bool HasExplicitKey(InhetoCollectionTypes type) => type is InhetoCollectionTypes.Dictionary
        or InhetoCollectionTypes.ConcurrentDictionary or InhetoCollectionTypes.SortedDictionary or InhetoCollectionTypes.SortedList;

    /// <summary>Identifies dictionary families whose string key has no separate descriptor bytes.<br/></summary>
    /// <param name="type">Encoded generic collection family.<br/></param>
    /// <returns>True only for the four implicit-string-key dictionary encodings.<br/></returns>
    private static bool HasStringKey(InhetoCollectionTypes type) => type is InhetoCollectionTypes.StringKeyDictionary
        or InhetoCollectionTypes.StringKeyConcurrentDictionary or InhetoCollectionTypes.StringKeySortedDictionary or InhetoCollectionTypes.StringKeySortedList;

    /// <summary>
    /// Skips one recursive shape iteratively, retaining only pending sibling/MD-length work on a small stack.<br/>
    /// Common shapes use stack storage only; unusually deep branching rents expandable scratch and returns it on every exit.<br/>
    /// No recursive call-stack growth, child objects, dimension arrays, or persistent ownership leases are introduced.<br/>
    /// </summary>
    /// <param name="source">Bounded descriptor or payload bytes.<br/></param>
    /// <param name="cursor">Descriptor start, advanced to the first following byte.<br/></param>
    /// <exception cref="EndOfStreamException">The encoded shape is truncated.<br/></exception>
    /// <exception cref="InvalidDataException">An array has invalid rank/depth, element classification, or lengths.<br/></exception>
    private static void Skip(ReadOnlySpan<byte> source, ref int cursor)
    {
        Span<int> pending = stackalloc int[32];
        int[]? rented = null;
        int count = 1;
        pending[0] = 0; // zero parses a descriptor; positive values consume MD lengths.
        try
        {
            while (count != 0)
            {
                int work = pending[--count];
                if (work != 0)
                {
                    for (int dimension = 0; dimension < work; dimension++) _ = ReadLength(source, ref cursor);
                    continue;
                }
                byte marker = Take(source, ref cursor);
                int kind = marker & 0xc0;
                if (kind < 0x80) continue;
                if (count > pending.Length - 2)
                {
                    int[] expanded = ArrayPool<int>.Shared.Rent(checked(pending.Length * 2));
                    pending[..count].CopyTo(expanded);
                    if (rented is not null) ArrayPool<int>.Shared.Return(rented);
                    rented = expanded;
                    pending = rented;
                }
                if (kind == 0xc0)
                {
                    if ((marker & 0x20) != 0) _ = Take(source, ref cursor);
                    pending[count++] = 0; // value follows optional key.
                    if (HasExplicitKey((InhetoCollectionTypes)(marker & 0x1f))) pending[count++] = 0;
                    continue;
                }
                switch ((InhetoPromotedTypes)(marker & 0x3f))
                {
                    case InhetoPromotedTypes.Enum:
                        _ = Take(source, ref cursor);
                        break;
                    case InhetoPromotedTypes.Array:
                    case InhetoPromotedTypes.ArrayMD:
                        int rankOrDepth = Take(source, ref cursor);
                        if (rankOrDepth == 0) throw new InvalidDataException("Zero array rank/depth.");
                        int element = cursor;
                        byte elementMarker = Take(source, ref element);
                        if (elementMarker is (byte)InhetoBaseTypes.Unknown or (byte)InhetoBaseTypes.Null)
                            throw new InvalidDataException("An array descriptor must declare an element type.");
                        if ((marker & 0x3f) == (byte)InhetoPromotedTypes.ArrayMD) pending[count++] = rankOrDepth;
                        pending[count++] = 0;
                        break;
                    case InhetoPromotedTypes.Enumerable:
                        pending[count++] = 0;
                        break;
                }
            }
        }
        finally
        {
            if (rented is not null) ArrayPool<int>.Shared.Return(rented);
        }
    }

    /// <summary>Reads one byte with an explicit truncation check, never from outside the supplied region.<br/></summary>
    /// <param name="source">Encoded bytes.<br/></param>
    /// <param name="cursor">Position advanced by one on success.<br/></param>
    /// <returns>The next byte.<br/></returns>
    private static byte Take(ReadOnlySpan<byte> source, ref int cursor)
    {
        if ((uint)cursor >= (uint)source.Length) throw new EndOfStreamException("Truncated Inheto shape.");
        return source[cursor++];
    }

    /// <summary>Decodes a non-negative zigzag/7-bit MD length while rejecting overflow and truncated encodings.<br/></summary>
    /// <param name="source">Encoded bytes.<br/></param>
    /// <param name="cursor">Position advanced past the length encoding.<br/></param>
    /// <returns>The non-negative dimension length.<br/></returns>
    private static int ReadLength(ReadOnlySpan<byte> source, ref int cursor)
    {
        uint encoded = 0;
        for (int shift = 0; shift < 35; shift += 7)
        {
            byte next = Take(source, ref cursor);
            if (shift == 28 && next > 15) throw new InvalidDataException("Array length encoding overflows UInt32.");
            encoded |= (uint)(next & 0x7f) << shift;
            if ((next & 0x80) == 0)
            {
                int length = (int)(encoded >> 1) ^ -((int)encoded & 1);
                if (length < 0) throw new InvalidDataException("Negative array length.");
                return length;
            }
        }
        throw new InvalidDataException("Invalid array length encoding.");
    }
}
