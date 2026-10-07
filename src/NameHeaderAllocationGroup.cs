using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Inheto;

/// <summary>
/// Stack-local sole ownership of append-only native descriptor chunks for bounded independent-value reads (ReadType, ReadString and ReadInt64).<br/>
/// Ordinary struct copies are aliases, not ownership transfers; pass by ref and release only at the owning boundary.<br/>
/// No descriptor or group escapes that boundary. Terminal descriptors remain shared and are never freed here.<br/>
/// Embedded managed references are forbidden: MD descriptors retain only numeric payload locators, checked before native storage.<br/>
/// </summary>
internal unsafe struct NameHeaderAllocationGroup
{
    private struct Block { internal Block* Next; internal int Capacity, Used; }
    private Block* first;
    internal int Capacity;
    internal int FailAtRequest;
    internal int Requests, Allocations, Frees;
    internal long AllocatedBytes, FreedBytes;
    internal readonly bool IsEmpty => first == null;

    /// <summary>
    /// Shares immutable terminals or blits one managed-reference-free recursive descriptor into stable group storage.<br/>
    /// Default capacity is one actual descriptor initially and four descriptors in overflow chunks; positive Capacity is a test/control policy.<br/>
    /// Allocation counters and optional per-group failure injection need no global state, locks or thread-local owner lookup.<br/>
    /// </summary>
    /// <param name="value">Normalized scoped-parser value containing no managed dimension reference.<br/></param>
    /// <returns>The shared terminal pointer or new native slot, valid until this group's Release.<br/></returns>
    internal nint Store(in NameHeaderEntryType value)
    {
        if (value.Kind != InhetoKindMasks.InhetoCollectionType
            && (value.Kind != InhetoKindMasks.InhetoPromotedType
                || value.PromotedType is not (InhetoPromotedTypes.Array or InhetoPromotedTypes.ArrayMD or InhetoPromotedTypes.Enumerable)))
            return NameHeaderTypeStorage.Create(in value);
        if (RuntimeHelpers.IsReferenceOrContainsReferences<NameHeaderEntryType>())
            throw new InvalidOperationException("Scoped native descriptors cannot retain managed references.");
        Requests++;
        if (FailAtRequest != 0 && Requests == FailAtRequest)
            throw new OutOfMemoryException("Injected scoped descriptor storage failure.");
        int size = Unsafe.SizeOf<NameHeaderEntryType>();
        Block* block = first;
        if (block == null || size > block->Capacity - block->Used)
        {
            int capacity = Math.Max(size, Capacity > 0 ? Capacity : block == null ? size : checked(size * 4));
            int bytes = checked(sizeof(Block) + capacity);
            block = (Block*)Marshal.AllocHGlobal(bytes);
            // Nothing that can throw occurs between acquisition and linking into the release chain.
            block->Next = first; block->Capacity = capacity; block->Used = 0; first = block;
            Allocations++; AllocatedBytes += bytes;
        }
        byte* slot = (byte*)(block + 1) + block->Used;
        Unsafe.Write(slot, value);
        block->Used += size;
        return (nint)slot;
    }

    /// <summary>
    /// Releases every chunk without walking semantic child pointers; safe for partial construction and repeated release of this owner.<br/>
    /// Clears the owner before freeing, preserves diagnostic totals, and never frees the immutable terminal vocabulary.<br/>
    /// All borrowed descriptor copies must have reached their last use before this call; copied owners must not be released independently.<br/>
    /// </summary>
    internal void Release()
    {
        Block* block = first; first = null;
        while (block != null)
        {
            Block* next = block->Next;
            int bytes = sizeof(Block) + block->Capacity;
            Marshal.FreeHGlobal((nint)block);
            Frees++; FreedBytes += bytes; block = next;
        }
    }
}
