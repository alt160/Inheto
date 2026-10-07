namespace Inheto;

internal unsafe partial struct NameHeaderEnumerableEntry : IEquatable<NameHeaderEnumerableEntry>
{
    /// <summary>Compares the stored native address without dereferencing it or boxing the descriptor.<br/></summary>
    /// <param name="other">Other enumerable descriptor storage.<br/></param>
    /// <returns>True when the existing pointer field matches, independently of pointed-to semantic content.<br/></returns>
    public readonly bool Equals(NameHeaderEnumerableEntry other) => ElementTypePtr == other.ElementTypePtr;

    /// <summary>Preserves field-based object equality and rejects unrelated/null objects.<br/></summary>
    /// <param name="obj">Potential boxed enumerable descriptor.<br/></param>
    /// <returns>True for equal stored descriptor fields.<br/></returns>
    public override readonly bool Equals(object? obj) => obj is NameHeaderEnumerableEntry other && Equals(other);

    /// <summary>Hashes the stored address without reading native memory or allocating a boxed pointer.<br/></summary>
    /// <returns>A process-local hash compatible with stored-field equality.<br/></returns>
    public override readonly int GetHashCode() => ((nint)ElementTypePtr).GetHashCode();
}

internal unsafe partial struct NameHeaderArrayEntry : IEquatable<NameHeaderArrayEntry>
{
    /// <summary>
    /// Compares every stored array field, including dimension locators and the native child address.<br/>
    /// Does not inspect dimension bytes or child contents, and does not compare padding bytes.<br/>
    /// </summary>
    /// <param name="other">Other array descriptor storage.<br/></param>
    /// <returns>True when all fields used by the previous ValueType equality match.<br/></returns>
    public readonly bool Equals(NameHeaderArrayEntry other) =>
        LengthsOffset == other.LengthsOffset && LengthsBytes == other.LengthsBytes &&
        ElementTypePtr == other.ElementTypePtr && ArrayType == other.ArrayType && Depth == other.Depth && Rank == other.Rank;

    /// <summary>Preserves field-based object equality and rejects unrelated/null objects.<br/></summary>
    /// <param name="obj">Potential boxed array descriptor.<br/></param>
    /// <returns>True for equal stored descriptor fields.<br/></returns>
    public override readonly bool Equals(object? obj) => obj is NameHeaderArrayEntry other && Equals(other);

    /// <summary>Hashes every stored array field directly, without native-memory access or boxing.<br/></summary>
    /// <returns>A process-local hash compatible with stored-field equality.<br/></returns>
    public override readonly int GetHashCode() => HashCode.Combine(LengthsOffset, LengthsBytes, (nint)ElementTypePtr, ArrayType, Depth, Rank);
}

internal unsafe partial struct NameHeaderCollectionEntry : IEquatable<NameHeaderCollectionEntry>
{
    /// <summary>Compares collection/comparer markers and both stored child addresses without following the pointers.<br/></summary>
    /// <param name="other">Other collection descriptor storage.<br/></param>
    /// <returns>True when every previous ValueType equality field matches.<br/></returns>
    public readonly bool Equals(NameHeaderCollectionEntry other) =>
        CollectionType == other.CollectionType && Comparer == other.Comparer && KeyTypePtr == other.KeyTypePtr && ValueTypePtr == other.ValueTypePtr;

    /// <summary>Preserves field-based object equality and rejects unrelated/null objects.<br/></summary>
    /// <param name="obj">Potential boxed collection descriptor.<br/></param>
    /// <returns>True for equal stored descriptor fields.<br/></returns>
    public override readonly bool Equals(object? obj) => obj is NameHeaderCollectionEntry other && Equals(other);

    /// <summary>Hashes collection/comparer markers and native addresses without dereferencing native memory.<br/></summary>
    /// <returns>A process-local hash compatible with stored-field equality.<br/></returns>
    public override readonly int GetHashCode() => HashCode.Combine(CollectionType, Comparer, (nint)KeyTypePtr, (nint)ValueTypePtr);
}
