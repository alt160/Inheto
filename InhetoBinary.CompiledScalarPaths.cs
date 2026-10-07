using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text;

namespace Inheto
{
    public partial class InhetoBinary
    {
        private static readonly ConcurrentDictionary<string, Lazy<PreparedInhetoPath>> preparedPaths =
            new ConcurrentDictionary<string, Lazy<PreparedInhetoPath>>(StringComparer.Ordinal);
        private static int nextPreparedPathToken;

        /// <summary>
        /// Gets or creates the process-local physical-path plan used by all payloads that read the same ordinary serialized path.<br/>
        /// Stable property paths receive a numeric cache identity; complete names are matched progressively without splitting or encoding the request.<br/>
        /// Synthesized collection paths remain allocation-free value descriptors unless an existing prepared plan already owns them.<br/>
        /// </summary>
        /// <param name="propPath">Exact serialized Inheto property path.<br/></param>
        /// <param name="prepareDynamicPath">Whether paths containing collection-key or element markers may be retained explicitly.<br/></param>
        /// <returns>An immutable shared plan, or a transient token-zero plan for a synthesized dynamic path.<br/></returns>
        private static PreparedInhetoPath GetPreparedPath(string propPath, bool prepareDynamicPath = false)
        {
            propPath ??= string.Empty;
            if (!prepareDynamicPath && PreparedInhetoPath.ContainsDynamicMarker(propPath))
            {
                if (preparedPaths.TryGetValue(propPath, out Lazy<PreparedInhetoPath>? existing))
                    return existing.Value;
                return PreparedInhetoPath.Create(propPath, 0);
            }

            return preparedPaths.GetOrAdd(
                propPath,
                static path => new Lazy<PreparedInhetoPath>(
                    () => PreparedInhetoPath.Create(path, NextPreparedPathToken()),
                    LazyThreadSafetyMode.ExecutionAndPublication)).Value;
        }

        /// <summary>
        /// Allocates the next nonzero process-local prepared-path token.<br/>
        /// Tokens identify path meaning only for the current process and are never serialized into an Inheto payload.<br/>
        /// </summary>
        /// <returns>The next stable nonzero token.<br/></returns>
        private static uint NextPreparedPathToken()
        {
            int token = Interlocked.Increment(ref nextPreparedPathToken);
            if (token <= 0)
                throw new InvalidOperationException("The process-local Inheto prepared-path token space has been exhausted.");
            return checked((uint)token);
        }

        /// <summary>
        /// Identifies the payload outcome produced by the compiled 32-bit names-header scanner.<br/>
        /// The distinction lets prepared property-path reads preserve missing, explicit-null, scalar, enum, and incompatible-shape behavior without constructing legacy entry objects.<br/>
        /// </summary>
        private enum CompiledInt32ReadKind : byte
        {
            Unsupported = 0,
            Missing = 1,
            Null = 2,
            Int32 = 3,
            Int32Enum = 4,
            Other = 5
        }

        /// <summary>
        /// Identifies whether one closed generic prepared-path target can consume the compiled 32-bit payload result.<br/>
        /// Classification runs once per closed <typeparamref name="TValue"/> instead of repeating reflection for every record.<br/>
        /// </summary>
        /// <typeparam name="TValue">Prepared property-path result type.<br/></typeparam>
        private static class CompiledInt32Target<TValue>
        {
            /// <summary>
            /// Gets the process-lifetime classification for this closed result type.<br/>
            /// Only exact <see cref="int"/> values and enums whose declared underlying type is <see cref="int"/> are admitted.<br/>
            /// </summary>
            internal static readonly CompiledInt32ReadKind Kind = Classify();

            /// <summary>
            /// Classifies the closed generic result type once without broad numeric coercion.<br/>
            /// </summary>
            /// <returns>The compatible scalar or enum shape, or <see cref="CompiledInt32ReadKind.Unsupported"/>.<br/></returns>
            private static CompiledInt32ReadKind Classify()
            {
                Type target = typeof(TValue);
                if (target == typeof(int))
                    return CompiledInt32ReadKind.Int32;
                if (target.IsEnum && Enum.GetUnderlyingType(target) == typeof(int))
                    return CompiledInt32ReadKind.Int32Enum;
                return CompiledInt32ReadKind.Unsupported;
            }
        }

        /// <summary>
        /// Resolves an inline 32-bit scalar through a cached, allocation-free property-path plan.<br/>
        /// The plan retains unchanged request text and a numeric identity; each payload contributes its parent chain, intact names, type marker and inline value.<br/>
        /// Unsupported or malformed name-header shapes return <see langword="false"/> so the established object-oriented reader remains the compatibility fallback.<br/>
        /// </summary>
        /// <param name="propPath">Exact Inheto property path using the normal dot, key, array, or multidimensional segment markers.<br/></param>
        /// <param name="value">Resolved inline value, or <see langword="null"/> when the path is absent or explicitly null.<br/></param>
        /// <returns><see langword="true"/> when the payload was completely handled by the compiled reader; otherwise <see langword="false"/> to request legacy resolution.<br/></returns>
        private bool TryReadInlineInt32Compiled(string propPath, out int? value)
        {
            bool handled = TryReadInlineInt32Compiled(propPath, out value, out _);
            return handled;
        }

        /// <summary>
        /// Resolves the byte offset of one exact ordinary property path through the cached allocation-free names-header plan.<br/>
        /// Successful missing-path results return offset zero, while malformed or unsupported payloads return <see langword="false"/> so the compatibility resolver can retain its established behavior.<br/>
        /// </summary>
        /// <param name="propPath">Exact Inheto property path using normal structural segment markers.<br/></param>
        /// <param name="entryIndex">Matched names-header byte offset, or zero when a valid payload does not contain the path.<br/></param>
        /// <returns><see langword="true"/> when the payload was parsed completely; otherwise <see langword="false"/> to request legacy resolution.<br/></returns>
        private bool TryFindCompiledNameEntry(string propPath, out uint entryIndex)
            => TryFindCompiledNameEntry(GetPreparedPath(propPath), out entryIndex);

        /// <summary>
        /// Resolves one exact path against intact stored names and parent/reference offsets without splitting or encoding its text.<br/>
        /// The returned entry index is suitable for the payload-local token cache and for creating a borrowed <see cref="NameHeaderEntry"/> only when a caller actually needs the richer view.<br/>
        /// </summary>
        /// <param name="selector">Prepared or transient physical-path plan.<br/></param>
        /// <param name="entryIndex">Matched names-header byte offset, or zero for a confirmed missing path.<br/></param>
        /// <returns><see langword="true"/> when the payload was parsed completely; otherwise <see langword="false"/> to request compatibility resolution.<br/></returns>
        private bool TryFindCompiledNameEntry(PreparedInhetoPath selector, out uint entryIndex)
        {
            entryIndex = 0;
            if (!selector.IsSupported ||
                !TryResolveLiteralNameEntry(selector.Path, out var entry, out int matched, out _))
                return false;
            if (matched == selector.Path.Length) entryIndex = entry.Index;
            return true;
        }

        /// <summary>
        /// Attempts to expose one prepared string property's borrowed UTF-8 payload without constructing a rich <see cref="NameHeaderEntry"/>.<br/>
        /// The shared prepared path supplies unchanged request text and a numeric identity, while this payload contributes its resolved entry index, type marker and value offset.<br/>
        /// Valid missing and null values are handled without compatibility fallback; unsupported or malformed name-table shapes return <see langword="false"/> so the established reader remains authoritative.<br/>
        /// Duplicate-value entries follow their persisted target entry once, matching the ordinary borrowed-entry route without decoding either entry name.<br/>
        /// </summary>
        /// <param name="propPath">Exact physical serialized property path.<br/></param>
        /// <param name="payload">Receives the zero-copy UTF-8 bytes when a stored string is present.<br/></param>
        /// <param name="handled">Receives whether the compiled reader completely handled the payload, including a valid missing or null outcome.<br/></param>
        /// <returns><see langword="true"/> when a present stored string was exposed; otherwise <see langword="false"/>.<br/></returns>
        private bool TryReadCompiledStringUtf8Payload(
            string propPath,
            out ReadOnlyMemory<byte> payload,
            out bool handled)
        {
            PreparedInhetoPath path = GetPreparedPath(propPath);
            uint entryIndex;
            if (!TryGetResolvedNameEntry(path, out entryIndex))
            {
                if (!TryFindCompiledNameEntry(path, out entryIndex))
                {
                    payload = default;
                    handled = false;
                    return false;
                }

                CacheResolvedNameEntry(path, entryIndex);
            }

            if (entryIndex == 0)
            {
                payload = default;
                handled = true;
                return false;
            }

            ReadOnlySpan<byte> source = stream.AsReadOnlySpan;
            int entryCursor = checked((int)entryIndex);
            if (!TryReadCompiledNameEntry(source, ref entryCursor, out CompiledNameEntry entry))
            {
                payload = default;
                handled = false;
                return false;
            }

            if (entry.BaseType is InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
            {
                entryCursor = checked((int)entry.ValueOffset);
                if (!TryReadCompiledNameEntry(source, ref entryCursor, out entry))
                {
                    payload = default;
                    handled = false;
                    return false;
                }
            }

            if (entry.BaseType is InhetoBaseTypes.Unknown or InhetoBaseTypes.Null)
            {
                payload = default;
                handled = true;
                return false;
            }

            if (entry.BaseType != InhetoBaseTypes.String)
            {
                throw new InvalidCastException(
                    $"Property path '{propPath}' contains '{entry.BaseType}' rather than a stored string value.");
            }

            int payloadCursor = checked((int)entry.ValueOffset);
            if (!TryRead7BitEncodedInt32(source, ref payloadCursor, out int payloadLength) ||
                payloadLength < 0 ||
                payloadCursor > source.Length - payloadLength)
            {
                payload = default;
                handled = false;
                return false;
            }

            payload = stream.ReadOnlyMemory(payloadCursor, payloadLength);
            handled = true;
            return true;
        }

        /// <summary>
        /// Resolves an inline 32-bit scalar and reports its exact names-header outcome through the cached allocation-free property-path plan.<br/>
        /// Malformed or unsupported payloads request the established compatibility fallback; valid missing and null values are terminal outcomes.<br/>
        /// </summary>
        /// <param name="propPath">Exact Inheto property path using the normal structural segment markers.<br/></param>
        /// <param name="value">Resolved inline bits for scalar, enum, or legacy-compatible other entries; otherwise <see langword="null"/>.<br/></param>
        /// <param name="readKind">Exact compiled lookup outcome used by prepared typed readers.<br/></param>
        /// <returns><see langword="true"/> when the payload was completely parsed; otherwise <see langword="false"/> to request legacy resolution.<br/></returns>
        private bool TryReadInlineInt32Compiled(
            string propPath,
            out int? value,
            out CompiledInt32ReadKind readKind)
            => TryReadInlineInt32Compiled(GetPreparedPath(propPath), out value, out readKind);

        /// <summary>
        /// Resolves an inline 32-bit scalar through an already prepared physical-path plan.<br/>
        /// This overload is the reader-loop path: it performs no property-path dictionary lookup, parsing, string splitting, or UTF-8 encoding.<br/>
        /// </summary>
        /// <param name="selector">Prepared physical-path plan shared by all payloads in the reader operation.<br/></param>
        /// <param name="value">Resolved inline value, or <see langword="null"/> for missing or explicit-null data.<br/></param>
        /// <param name="readKind">Exact compiled lookup outcome.<br/></param>
        /// <returns><see langword="true"/> when the payload was completely handled; otherwise <see langword="false"/>.<br/></returns>
        private bool TryReadInlineInt32Compiled(
            PreparedInhetoPath selector,
            out int? value,
            out CompiledInt32ReadKind readKind)
        {
            if (!selector.IsSupported)
            {
                value = null;
                readKind = CompiledInt32ReadKind.Unsupported;
                return false;
            }

            if (TryGetResolvedNameEntry(selector, out uint cachedIndex))
            {
                if (cachedIndex == 0)
                {
                    value = null;
                    readKind = CompiledInt32ReadKind.Missing;
                    return true;
                }
                int cursor = checked((int)cachedIndex);
                if (TryReadCompiledNameEntry(stream.AsReadOnlySpan, ref cursor, out var cached))
                    return TryConvertCompiledInt32(cached, out value, out readKind);
            }
            if (!TryResolveLiteralNameEntry(selector.Path, out var entry, out int matched, out _))
            {
                value = null;
                readKind = CompiledInt32ReadKind.Unsupported;
                return false;
            }
            bool exact = matched == selector.Path.Length;
            CacheResolvedNameEntry(selector, exact ? entry.Index : 0);
            if (exact) return TryConvertCompiledInt32(entry, out value, out readKind);
            value = null;
            readKind = CompiledInt32ReadKind.Missing;
            return true;
        }

        /// <summary>
        /// Attempts to execute one exact prepared Int32 or Int32-backed-enum path through the compiled names-header scanner.<br/>
        /// Read-time aliases and CLR continuations deliberately retain the legacy resolver because they may select a different physical path or invoke developer code.<br/>
        /// Serialized type mismatches also fall back so established mismatch diagnostics and coercion policy remain authoritative.<br/>
        /// </summary>
        /// <typeparam name="TValue">Exact prepared result type.<br/></typeparam>
        /// <param name="path">Validated reusable property-path plan.<br/></param>
        /// <param name="dzOptions">Active read options, including aliases.<br/></param>
        /// <param name="value">Resolved scalar or enum value when handled successfully.<br/></param>
        /// <param name="state">Prepared-path semantic result when handled.<br/></param>
        /// <returns><see langword="true"/> when no legacy resolver work remains; otherwise <see langword="false"/>.<br/></returns>
        private bool TryReadCompiledInt32PropPath<TValue>(
            PropPathCheck path,
            DeserializationOptions? dzOptions,
            out TValue? value,
            out PropPathReadState state)
        {
            value = default;
            state = PropPathReadState.Invalid;
            CompiledInt32ReadKind targetKind = CompiledInt32Target<TValue>.Kind;
            if (targetKind == CompiledInt32ReadKind.Unsupported ||
                !path.IsValid ||
                path.SerializedPath is null ||
                path.RequiresClrContinuation ||
                (dzOptions is not null && dzOptions.Aliases.TryGetGroup(path.CanonicalPath, out _, out _)))
            {
                return false;
            }

            if (!TryReadInlineInt32Compiled(path.SerializedPath.Value, out int? rawValue, out CompiledInt32ReadKind readKind))
                return false;

            if (readKind == CompiledInt32ReadKind.Missing)
            {
                state = PropPathReadState.SourceMissing;
                return true;
            }
            if (readKind == CompiledInt32ReadKind.Null)
            {
                state = PropPathReadState.SourceNull;
                return true;
            }
            if (readKind != targetKind || !rawValue.HasValue)
                return false;

            int raw = rawValue.GetValueOrDefault();
            value = Unsafe.As<int, TValue>(ref raw);
            state = PropPathReadState.Value;
            return true;
        }

        /// <summary>
        /// Converts one already resolved name-header entry according to the established <see cref="ReadInt32Core(string)"/> inline-value rules.<br/>
        /// This deliberately preserves the existing base-type-or-enum interpretation rather than adding a stricter contract to an optimization path.<br/>
        /// </summary>
        /// <param name="entry">Resolved binary name-header entry.<br/></param>
        /// <param name="value">Inline signed 32-bit representation, or <see langword="null"/> for unknown/null entries.<br/></param>
        /// <param name="readKind">Classifies the entry as an Int32 scalar, Int32-backed enum, explicit null, or another shape so the caller can retain its established fallback policy.<br/></param>
        /// <returns><see langword="true"/> because a syntactically valid resolved entry requires no legacy fallback.<br/></returns>
        private static bool TryConvertCompiledInt32(
            CompiledNameEntry entry,
            out int? value,
            out CompiledInt32ReadKind readKind)
        {
            InhetoBaseTypes effectiveType = entry.BaseType | entry.EnumType;
            if (effectiveType == InhetoBaseTypes.Null)
            {
                value = null;
                readKind = CompiledInt32ReadKind.Null;
                return true;
            }

            value = effectiveType == InhetoBaseTypes.Unknown
                ? null
                : unchecked((int)entry.ValueOffset);
            readKind = entry.EnumType == InhetoBaseTypes.Int32
                ? CompiledInt32ReadKind.Int32Enum
                : entry.BaseType == InhetoBaseTypes.Int32
                    ? CompiledInt32ReadKind.Int32
                    : CompiledInt32ReadKind.Other;
            return true;
        }

        /// <summary>
        /// Parses one Inheto name-header entry directly from a read-only payload span.<br/>
        /// Parsing advances a local integer cursor only and therefore creates no stream segments, strings, dictionaries, or entry objects.<br/>
        /// </summary>
        /// <param name="payload">Complete uncompressed Inheto payload including its version byte.<br/></param>
        /// <param name="cursor">Current name-header byte offset, advanced past the parsed entry on success.<br/></param>
        /// <param name="entry">Allocation-free entry metadata and borrowed UTF-8 name bytes.<br/></param>
        /// <returns><see langword="true"/> when a complete entry was parsed; otherwise <see langword="false"/> for malformed or unsupported bytes.<br/></returns>
        private static bool TryReadCompiledNameEntry(
            ReadOnlySpan<byte> payload,
            scoped ref int cursor,
            out CompiledNameEntry entry)
        {
            entry = default;
            int entryIndex = cursor;
            if (!TryRead7BitEncodedUInt32(payload, ref cursor, out uint parentIndex) ||
                !TryReadCompiledType(payload, ref cursor, out InhetoBaseTypes baseType, out InhetoBaseTypes enumType) ||
                !TryRead7BitEncodedInt32(payload, ref cursor, out int nameByteCount) ||
                nameByteCount < 0 ||
                cursor > payload.Length - nameByteCount)
            {
                return false;
            }

            ReadOnlySpan<byte> name = payload.Slice(cursor, nameByteCount);
            cursor += nameByteCount;
            if (cursor > payload.Length - sizeof(uint))
                return false;

            uint valueOffset = BinaryPrimitives.ReadUInt32LittleEndian(payload.Slice(cursor, sizeof(uint)));
            cursor += sizeof(uint);
            entry = new CompiledNameEntry(
                checked((uint)entryIndex),
                parentIndex,
                baseType,
                enumType,
                valueOffset,
                name);
            return true;
        }

        /// <summary>
        /// Parses and skips one recursive Inheto name-entry type descriptor while retaining the base or enum scalar classification needed by direct reads.<br/>
        /// Array, enumerable, and collection element descriptors are consumed recursively so the following encoded name begins at the exact established offset.<br/>
        /// A collection's family and optional-comparer flag are encoded in the already consumed marker, not a second byte.<br/>
        /// Arrays with a direct Unknown or Null element marker request compatibility parsing, which preserves the established exception contract.<br/>
        /// </summary>
        /// <param name="payload">Complete Inheto payload.<br/></param>
        /// <param name="cursor">Current type-marker offset, advanced to the encoded name on success.<br/></param>
        /// <param name="baseType">Decoded base or nullable base type when present.<br/></param>
        /// <param name="enumType">Decoded enum backing type when present.<br/></param>
        /// <returns><see langword="true"/> when the descriptor is complete and supported; otherwise <see langword="false"/>.<br/></returns>
        private static bool TryReadCompiledType(
            ReadOnlySpan<byte> payload,
            ref int cursor,
            out InhetoBaseTypes baseType,
            out InhetoBaseTypes enumType)
        {
            baseType = InhetoBaseTypes.Unknown;
            enumType = InhetoBaseTypes.Unknown;
            if ((uint)cursor >= (uint)payload.Length)
                return false;

            byte marker = payload[cursor++];
            InhetoKindMasks kind = (InhetoKindMasks)(marker & 0b1100_0000);
            byte typeCode = (byte)(marker & 0b0011_1111);
            switch (kind)
            {
                case InhetoKindMasks.InhetoBaseType:
                    baseType = (InhetoBaseTypes)typeCode;
                    return true;

                case InhetoKindMasks.InhetoNullableBaseType:
                    baseType = (InhetoBaseTypes)typeCode;
                    return true;

                case InhetoKindMasks.InhetoPromotedType:
                    switch ((InhetoPromotedTypes)typeCode)
                    {
                        case InhetoPromotedTypes.Enum:
                            if ((uint)cursor >= (uint)payload.Length)
                                return false;
                            enumType = (InhetoBaseTypes)(payload[cursor++] & 0b0011_1111);
                            return true;

                        case InhetoPromotedTypes.Array:
                            if ((uint)cursor >= (uint)(payload.Length - 1))
                                return false;
                            cursor++;
                            if (payload[cursor] is (byte)InhetoBaseTypes.Unknown or (byte)InhetoBaseTypes.Null)
                                return false;
                            return TryReadCompiledType(payload, ref cursor, out _, out _);

                        case InhetoPromotedTypes.ArrayMD:
                            if ((uint)cursor >= (uint)(payload.Length - 1))
                                return false;
                            int rank = payload[cursor++];
                            if (payload[cursor] is (byte)InhetoBaseTypes.Unknown or (byte)InhetoBaseTypes.Null)
                                return false;
                            if (!TryReadCompiledType(payload, ref cursor, out _, out _))
                                return false;
                            for (int dimension = 0; dimension < rank; dimension++)
                            {
                                if (!TryRead7BitEncodedInt32(payload, ref cursor, out _))
                                    return false;
                            }
                            return true;

                        case InhetoPromotedTypes.Enumerable:
                            return TryReadCompiledType(payload, ref cursor, out _, out _);

                        default:
                            return true;
                    }

                case InhetoKindMasks.InhetoCollectionType:
                    // typeCode already retains the family and auxiliary bits; no need to keep the full marker live.
                    InhetoCollectionTypes collectionType =
                        (InhetoCollectionTypes)(typeCode & 0b0001_1111);
                    if ((typeCode & 0b0010_0000) != 0)
                    {
                        if ((uint)cursor >= (uint)payload.Length)
                            return false;
                        cursor++;
                    }

                    if (collectionType is InhetoCollectionTypes.Dictionary or
                        InhetoCollectionTypes.ConcurrentDictionary or
                        InhetoCollectionTypes.SortedDictionary or
                        InhetoCollectionTypes.SortedList)
                    {
                        if (!TryReadCompiledType(payload, ref cursor, out _, out _))
                            return false;
                    }
                    return TryReadCompiledType(payload, ref cursor, out _, out _);

                default:
                    return false;
            }
        }

        /// <summary>
        /// Decodes one unsigned 7-bit integer from a span without creating a stream or temporary buffer.<br/>
        /// More than five bytes or a truncated continuation is rejected as malformed input.<br/>
        /// </summary>
        /// <param name="payload">Source bytes.<br/></param>
        /// <param name="cursor">Current offset, advanced past the encoded integer on success.<br/></param>
        /// <param name="value">Decoded unsigned value.<br/></param>
        /// <returns><see langword="true"/> when a complete UInt32 encoding was consumed; otherwise <see langword="false"/>.<br/></returns>
        private static bool TryRead7BitEncodedUInt32(
            ReadOnlySpan<byte> payload,
            ref int cursor,
            out uint value)
        {
            value = 0;
            for (int shift = 0; shift < 35; shift += 7)
            {
                if ((uint)cursor >= (uint)payload.Length)
                    return false;
                byte current = payload[cursor++];
                if (shift == 28 && (current & 0xF0) != 0)
                    return false;
                value |= (uint)(current & 0x7F) << shift;
                if ((current & 0x80) == 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Decodes one signed 7-bit integer using the same two's-complement bit representation consumed by <see cref="BufferStream.Read7BitEncodedInt"/>.<br/>
        /// </summary>
        /// <param name="payload">Source bytes.<br/></param>
        /// <param name="cursor">Current offset, advanced past the encoded integer on success.<br/></param>
        /// <param name="value">Decoded signed value.<br/></param>
        /// <returns><see langword="true"/> when a complete Int32 encoding was consumed; otherwise <see langword="false"/>.<br/></returns>
        private static bool TryRead7BitEncodedInt32(
            ReadOnlySpan<byte> payload,
            ref int cursor,
            out int value)
        {
            bool success = TryRead7BitEncodedUInt32(payload, ref cursor, out uint raw);
            value = unchecked((int)(raw >> 1)) ^ -unchecked((int)(raw & 1));
            return success;
        }

        /// <summary>
        /// Carries allocation-free metadata for one parsed name-header entry and borrows its UTF-8 name bytes from the source payload.<br/>
        /// </summary>
        private readonly ref struct CompiledNameEntry
        {
            /// <summary>
            /// Creates one parsed entry view over the source payload.<br/>
            /// </summary>
            /// <param name="index">Byte offset of the entry within the payload.<br/></param>
            /// <param name="parentIndex">Byte offset of the parent name entry.<br/></param>
            /// <param name="baseType">Base or nullable base type classification.<br/></param>
            /// <param name="enumType">Enum backing type classification when applicable.<br/></param>
            /// <param name="valueOffset">Inline scalar bits or out-of-line payload offset from the names header.<br/></param>
            /// <param name="name">Borrowed UTF-8 name bytes including their structural prefix.<br/></param>
            internal CompiledNameEntry(
                uint index,
                uint parentIndex,
                InhetoBaseTypes baseType,
                InhetoBaseTypes enumType,
                uint valueOffset,
                ReadOnlySpan<byte> name)
            {
                Index = index;
                ParentIndex = parentIndex;
                BaseType = baseType;
                EnumType = enumType;
                ValueOffset = valueOffset;
                Name = name;
            }

            internal uint Index { get; }
            internal uint ParentIndex { get; }
            internal InhetoBaseTypes BaseType { get; }
            internal InhetoBaseTypes EnumType { get; }
            internal uint ValueOffset { get; }
            internal ReadOnlySpan<byte> Name { get; }
        }
    }

    /// <summary>
    /// Holds unchanged path text and a process-local numeric cache identity, not a lexical decomposition.<br/>
    /// Stable plans carry a nonzero token used by payload-local entry-index caches; transient synthesized collection paths deliberately carry token zero and are not retained globally.<br/>
    /// </summary>
    internal readonly struct PreparedInhetoPath
    {
        /// <summary>
        /// Creates an allocation-free immutable selector without splitting, normalizing, or encoding its text.<br/>
        /// </summary>
        /// <param name="path">Canonical serialized property path retained only by stable plans.<br/></param>
        /// <param name="token">Process-local path token, or zero for transient paths.<br/></param>
        private PreparedInhetoPath(string path, uint token)
        {
            Path = path;
            Token = token;
        }

        /// <summary>Gets the retained exact path for stable plans or the transient caller path for token-zero plans.<br/></summary>
        internal string Path { get; }

        /// <summary>Gets the process-local numeric path identity; zero means the plan must not enter a payload-local cache.<br/></summary>
        internal uint Token { get; }

        /// <summary>Gets whether this path can use the compiled binary scanner.<br/></summary>
        internal bool IsSupported => Path.Length == 0 || IsMarker(Path[0]);

        /// <summary>
        /// Retains the exact caller string and cache identity without lexical preparation or heap allocation.<br/>
        /// Empty paths address the root; stored dictionary names determine literal-key boundaries during navigation.<br/>
        /// </summary>
        /// <param name="path">Exact Inheto property path.<br/></param>
        /// <param name="token">Nonzero stable token or zero for a transient plan.<br/></param>
        /// <returns>A small immutable selector; dynamic token-zero selectors are never retained in payload caches.<br/></returns>
        internal static PreparedInhetoPath Create(string path, uint token)
            => new PreparedInhetoPath(path, token);

        /// <summary>
        /// Identifies whether a path includes a synthesized dictionary, array, or multidimensional element segment.<br/>
        /// Ordinary dotted schema paths are safe for automatic process-wide preparation; potentially unbounded element paths require explicit preparation.<br/>
        /// </summary>
        /// <param name="path">Candidate serialized property path.<br/></param>
        /// <returns><see langword="true"/> when the path contains <c>!</c>, <c>#</c>, or <c>@</c> markers.<br/></returns>
        internal static bool ContainsDynamicMarker(string path)
            => path.AsSpan().IndexOfAny('!', '#', '@') >= 0;

        /// <summary>
        /// Identifies the four structural name prefixes supported by ordinary Inheto property paths.<br/>
        /// </summary>
        /// <param name="character">Candidate path character.<br/></param>
        /// <returns><see langword="true"/> for property, dictionary-key, numbered-element, or multidimensional-element markers.<br/></returns>
        private static bool IsMarker(char character)
            => character is '.' or '!' or '#' or '@';
    }
}
