using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace Inheto;

public partial class InhetoBinary
{
    /// <summary>
    /// Tests whether a property path resolves to an encoded name entry, including entries whose stored value is null.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>True when the documented predicate is satisfied; otherwise false.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool PropExists(string propPath)
    {
        if (literalReadAnchorIndex == 0) return PropExistsCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return PropExistsCore(propPath);
    }

    /// <summary>
    /// Reports whether a logical property path or one of its registered equivalents exists in this payload.<br/>
    /// The exact requested path retains precedence over alias fallbacks.<br/>
    /// </summary>
    /// <param name="propPath">Logical property path to inspect.<br/></param>
    /// <param name="dzOptions">Read policy containing optional property aliases.<br/></param>
    /// <returns><see langword="true"/> when an exact or aliased stored member exists; otherwise <see langword="false"/>.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool PropExists(string propPath, DeserializationOptions? dzOptions)
    {
        if (literalReadAnchorIndex == 0) return PropExistsCore(propPath, dzOptions);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return PropExistsCore(propPath, dzOptions);
    }

    /// <summary>
    /// Reads the encoded BigInteger representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BigInteger? ReadBigInteger(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadBigIntegerCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadBigIntegerCore(propPath);
    }

    /// <summary>
    /// Reads the encoded BitArray representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitArray? ReadBitArray(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadBitArrayCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadBitArrayCore(propPath);
    }

    /// <summary>
    /// Reads the encoded Boolean representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool? ReadBoolean(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadBooleanCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadBooleanCore(propPath);
    }

    /// <summary>
    /// Reads the encoded Byte representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte? ReadByte(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadByteCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadByteCore(propPath);
    }

    /// <summary>
    /// Reads the encoded ByteArray representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>Native payload reads allocate detached byte storage; these are not zero-copy slices of the source stream.<br/></remarks>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[]? ReadByteArray(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadByteArrayCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadByteArrayCore(propPath);
    }

    /// <summary>
    /// Reads the encoded MemoryByte representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>Native payload reads allocate detached byte storage; these are not zero-copy slices of the source stream.<br/></remarks>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Memory<byte>? ReadMemoryByte(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadMemoryByteCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadMemoryByteCore(propPath);
    }

    /// <summary>
    /// Reads the encoded ReadOnlyMemoryByte representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>Native payload reads allocate detached byte storage; these are not zero-copy slices of the source stream.<br/></remarks>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyMemory<byte>? ReadReadOnlyMemoryByte(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadReadOnlyMemoryByteCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadReadOnlyMemoryByteCore(propPath);
    }

    /// <summary>
    /// Reads the encoded MemoryStream representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>The returned stream may be cached by the reader. Do not assume repeated reads create independent streams; coordinate disposal and mutation when aliases can share it.<br/></remarks>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MemoryStream? ReadMemoryStream(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadMemoryStreamCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadMemoryStreamCore(propPath);
    }

    /// <summary>
    /// Reads the encoded Char representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char? ReadChar(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadCharCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadCharCore(propPath);
    }

    /// <summary>
    /// Reads the encoded Complex representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Complex? ReadComplex(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadComplexCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadComplexCore(propPath);
    }

    /// <summary>
    /// Reads the encoded DateOnly representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateOnly? ReadDateOnly(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadDateOnlyCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadDateOnlyCore(propPath);
    }

    /// <summary>
    /// Reads the encoded DateTime representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateTime? ReadDateTime(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadDateTimeCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadDateTimeCore(propPath);
    }

    /// <summary>
    /// Reads the encoded DateTimeOffset representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateTimeOffset? ReadDateTimeOffset(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadDateTimeOffsetCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadDateTimeOffsetCore(propPath);
    }

    /// <summary>
    /// Reads the encoded Decimal representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public decimal? ReadDecimal(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadDecimalCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadDecimalCore(propPath);
    }

    /// <summary>
    /// Reads the encoded Double representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double? ReadDouble(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadDoubleCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadDoubleCore(propPath);
    }

    /// <summary>
    /// Reads the encoded Guid representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Guid? ReadGuid(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadGuidCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadGuidCore(propPath);
    }

    /// <summary>
    /// Reads the encoded Half representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Half? ReadHalf(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadHalfCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadHalfCore(propPath);
    }

    /// <summary>
    /// Reads the encoded Int128 representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Int128? ReadInt128(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadInt128Core(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadInt128Core(propPath);
    }

    /// <summary>
    /// Reads the encoded Int16 representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short? ReadInt16(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadInt16Core(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadInt16Core(propPath);
    }

    /// <summary>
    /// Reads the encoded Int32 representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int? ReadInt32(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadInt32Core(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadInt32Core(propPath);
    }

    /// <summary>
    /// Reads the encoded IPAddress representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPAddress? ReadIPAddress(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadIPAddressCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadIPAddressCore(propPath);
    }

    /// <summary>
    /// Reads the encoded IPEndPoint representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPEndPoint? ReadIPEndPoint(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadIPEndPointCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadIPEndPointCore(propPath);
    }

    /// <summary>
    /// Reads the encoded MailAddress representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MailAddress? ReadMailAddress(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadMailAddressCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadMailAddressCore(propPath);
    }

    /// <summary>
    /// Reads the encoded Matrix3x2 representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3x2? ReadMatrix3x2(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadMatrix3x2Core(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadMatrix3x2Core(propPath);
    }

    /// <summary>
    /// Reads the encoded Matrix4x4 representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4x4? ReadMatrix4x4(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadMatrix4x4Core(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadMatrix4x4Core(propPath);
    }

    /// <summary>
    /// Reads the encoded NameValueCollection representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <param name="ontoThisInstance">Optional destination collection to populate according to the reader&apos;s existing fill semantics; not a request to clone it.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NameValueCollection? ReadNameValueCollection(string propPath, NameValueCollection? ontoThisInstance = null)
    {
        if (literalReadAnchorIndex == 0) return ReadNameValueCollectionCore(propPath, ontoThisInstance);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadNameValueCollectionCore(propPath, ontoThisInstance);
    }

    /// <summary>
    /// Reads the encoded Plane representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Plane? ReadPlane(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadPlaneCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadPlaneCore(propPath);
    }

    /// <summary>
    /// Returns a type-agnostic navigable wrapper for an encoded member, collection or scalar. Returns null for missing/null entries or an unsupported wrapper shape; shape metadata borrows the reader payload.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <param name="dzOptions">Optional deserialization policy; see this overload&apos;s remarks for forwarding limitations.<br/></param>
    /// <returns>Navigable wrapper, or null for a missing/null entry or a shape without an implemented wrapper.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IInhetoObject? ReadProp(string propPath, DeserializationOptions? dzOptions = null)
    {
        if (literalReadAnchorIndex == 0) return ReadPropCore(propPath, dzOptions);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadPropCore(propPath, dzOptions);
    }

    /// <summary>
    /// Reads the encoded Quaternion representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion? ReadQuaternion(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadQuaternionCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadQuaternionCore(propPath);
    }

    /// <summary>
    /// Reads the encoded Regex representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Regex? ReadRegex(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadRegexCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadRegexCore(propPath);
    }

    /// <summary>
    /// Reads the encoded TimeZoneInfo representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeZoneInfo? ReadTimeZoneInfo(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadTimeZoneInfoCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadTimeZoneInfoCore(propPath);
    }

    /// <summary>
    /// Reads the encoded SecureString representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SecureString? ReadSecureString(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadSecureStringCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadSecureStringCore(propPath);
    }

    /// <summary>
    /// Reads the encoded CultureInfo representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CultureInfo? ReadCultureInfo(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadCultureInfoCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadCultureInfoCore(propPath);
    }

    /// <summary>
    /// Reads the encoded Rune representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Rune? ReadRune(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadRuneCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadRuneCore(propPath);
    }

    /// <summary>
    /// Reads the encoded SByte representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte? ReadSByte(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadSByteCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadSByteCore(propPath);
    }

    /// <summary>
    /// Reads the encoded Single representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float? ReadSingle(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadSingleCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadSingleCore(propPath);
    }

    /// <summary>
    /// Reads the encoded StringBuilder representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StringBuilder? ReadStringBuilder(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadStringBuilderCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadStringBuilderCore(propPath);
    }

    /// <summary>
    /// Reads the encoded StaticClass representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object? ReadStaticClass(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadStaticClassCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadStaticClassCore(propPath);
    }

    /// <summary>
    /// Reads the encoded StringCollection representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <param name="ontoThisInstance">Optional destination collection to populate according to the reader&apos;s existing fill semantics; not a request to clone it.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StringCollection? ReadStringCollection(string propPath, StringCollection? ontoThisInstance = null)
    {
        if (literalReadAnchorIndex == 0) return ReadStringCollectionCore(propPath, ontoThisInstance);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadStringCollectionCore(propPath, ontoThisInstance);
    }

    /// <summary>
    /// Reads the encoded StringList representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <param name="ontoThisInstance">Optional destination collection to populate according to the reader&apos;s existing fill semantics; not a request to clone it.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public List<string>? ReadStringList(string propPath, List<string>? ontoThisInstance = null)
    {
        if (literalReadAnchorIndex == 0) return ReadStringListCore(propPath, ontoThisInstance);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadStringListCore(propPath, ontoThisInstance);
    }

    /// <summary>
    /// Reads the encoded TimeOnly representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly? ReadTimeOnly(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadTimeOnlyCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadTimeOnlyCore(propPath);
    }

    /// <summary>
    /// Reads the encoded TimeSpan representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeSpan? ReadTimeSpan(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadTimeSpanCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadTimeSpanCore(propPath);
    }

    /// <summary>
    /// Reads the encoded UInt128 representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UInt128? ReadUInt128(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadUInt128Core(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadUInt128Core(propPath);
    }

    /// <summary>
    /// Reads the encoded UInt16 representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort? ReadUInt16(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadUInt16Core(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadUInt16Core(propPath);
    }

    /// <summary>
    /// Reads the encoded UInt32 representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint? ReadUInt32(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadUInt32Core(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadUInt32Core(propPath);
    }

    /// <summary>
    /// Reads the encoded UInt64 representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong? ReadUInt64(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadUInt64Core(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadUInt64Core(propPath);
    }

    /// <summary>
    /// Reads the encoded Uri representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Uri? ReadUri(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadUriCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadUriCore(propPath);
    }

    /// <summary>
    /// Reads the encoded Vector2 representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2? ReadVector2(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadVector2Core(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadVector2Core(propPath);
    }

    /// <summary>
    /// Reads the encoded Vector3 representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3? ReadVector3(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadVector3Core(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadVector3Core(propPath);
    }

    /// <summary>
    /// Reads the encoded Vector4 representation at a property path; missing or explicit-null entries return null. Use ReadPropAs for general requested-type conversion rather than assuming this native reader accepts every wire shape.<br/>
    /// </summary>
    /// <param name="propPath">Logical Inheto member path; use the empty path for the root where supported and numbered tokens for collection elements.<br/></param>
    /// <returns>Native typed value, or null for a missing or explicit-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector4? ReadVector4(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadVector4Core(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadVector4Core(propPath);
    }

    /// <summary>
    /// Reads a Version while retaining its original two-, three- or four-component shape.<br/>
    /// The wire always stores four integers; -1 marks an unspecified trailing Build or Revision.<br/>
    /// Invalid negative components or a Revision without a Build throw instead of fabricating a Version.<br/>
    /// Missing/null paths, duplicate handling and the existing value cache retain their current semantics.<br/>
    /// </summary>
    /// <param name="propPath">Progressive property path to the encoded Version value.<br/></param>
    /// <returns>The materialized Version, or null for an absent/null path.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Version? ReadVersion(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadVersionCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadVersionCore(propPath);
    }

    /// <summary>
    /// Reads an independent Int64 while releasing temporary recursive name-header descriptors at the end of this call.<br/>
    /// Preserves exact-path/cache resolution, DuplicateValue chains, missing/null checks and BufferStream value/cursor behavior.<br/>
    /// No native descriptor escapes in the nullable scalar; native layout, wire format and other reader contracts are unchanged.<br/>
    /// </summary>
    /// <param name="propPath">Exact physical path, including the existing empty/null root alias.<br/></param>
    /// <returns>The decoded Int64, or the existing null result for an absent or encoded-null entry.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long? ReadInt64(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadInt64Core(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadInt64Core(propPath);
    }

    /// <summary>
    /// Reads an independent string while releasing temporary recursive name-header storage at the end of this call.<br/>
    /// Preserves exact-path/cache resolution, one-hop duplicate handling, stream encoding/cursor behavior and missing/null/empty distinctions.<br/>
    /// No decoded descriptor escapes with the string; native layout, wire format and other materialization routes are unchanged.<br/>
    /// </summary>
    /// <param name="propPath">Exact physical property path, or the empty/null root path accepted by the existing resolver.<br/></param>
    /// <returns>The decoded string, including empty, or the existing null result for absent/encoded-null values.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string? ReadString(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadStringCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadStringCore(propPath);
    }

    /// <summary>
    /// Reads a serialized CLR Type while reclaiming temporary native descriptors, including MD, at the end of this call.<br/>
    /// Preserves missing/null, duplicate, cache and exact-path behavior; missing required type-name payloads throw with their path.<br/>
    /// The returned Type has no dependency on native header storage. No developer disposal or reader-lifetime owner is introduced.<br/>
    /// </summary>
    /// <param name="propPath">Exact physical property path, including the empty root path.<br/></param>
    /// <returns>The resolved CLR Type or the existing null result when absent/unresolved.<br/></returns>
    /// <remarks>
    /// Public paths start at the payload root, including callback reentry during dictionary reconstruction.<br/>
    /// Known physical entries are an internal fill concern; their context is restored on every exit.<br/>
    /// Native type, null, conversion, cache and cursor behavior is otherwise unchanged.<br/>
    /// The zero-anchor fast route creates no scope, heap object, delegate or path tokens.<br/>
    /// This does not isolate low-level caches/cursors or make a reader thread-safe.<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Type? ReadType(string propPath)
    {
        if (literalReadAnchorIndex == 0) return ReadTypeCore(propPath);
        using var rooted = new LiteralValueReadScope(this, 0, null);
        return ReadTypeCore(propPath);
    }

}
