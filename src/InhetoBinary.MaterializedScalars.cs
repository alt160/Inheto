using System.Numerics;
using System.Text;
using System.Runtime.InteropServices;

namespace Inheto;

public partial class InhetoBinary
{
    /// <summary>Materializes a bool scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private bool? ReadMaterializedBoolean(string propPath)
        {
            var nvn = GetInlineNavigation(propPath);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
                return ReadMaterializedAs<bool?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return null;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return null;

            // value was inlined in valueOffset
            return nvn.ValueOffset != 0;
        }

    /// <summary>Materializes a byte scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private byte? ReadMaterializedByte(string propPath)
        {
            var nvn = GetInlineNavigation(propPath);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
                return ReadMaterializedAs<byte?>(propPath, materializationOptions ?? deserializationOptions);
            if ((nvn.Type.BaseType | nvn.Type.EnumType) == InhetoBaseTypes.Unknown) return null;
            if ((nvn.Type.BaseType | nvn.Type.EnumType) == InhetoBaseTypes.Null) return null;
            // value was inlined in valueOffset
            return (byte)nvn.ValueOffset;
        }

    /// <summary>Materializes a sbyte scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private sbyte? ReadMaterializedSByte(string propPath)
        {
            var nvn = GetInlineNavigation(propPath);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
                return ReadMaterializedAs<sbyte?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return null;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return null;

            // value was inlined in valueOffset
            return (sbyte)nvn.ValueOffset;
        }

    /// <summary>Materializes a char scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private char? ReadMaterializedChar(string propPath)
        {
            var nvn = GetInlineNavigation(propPath);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
                return ReadMaterializedAs<char?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return null;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return null;

            // value was inlined in valueOffset
            return (char)nvn.ValueOffset;
        }

    /// <summary>Materializes a Rune scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private Rune? ReadMaterializedRune(string propPath)
        {
            var nvn = GetInlineNavigation(propPath);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
                return ReadMaterializedAs<Rune?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return null;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return null;

            // value was inlined in valueOffset
            return (Rune)nvn.ValueOffset;
        }

    /// <summary>Materializes a short scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private short? ReadMaterializedInt16(string propPath)
        {
            var nvn = GetInlineNavigation(propPath);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
                return ReadMaterializedAs<short?>(propPath, materializationOptions ?? deserializationOptions);
            if ((nvn.Type.BaseType | nvn.Type.EnumType) == InhetoBaseTypes.Unknown) return null;
            if ((nvn.Type.BaseType | nvn.Type.EnumType) == InhetoBaseTypes.Null) return null;
            // value was inlined in valueOffset
            return (short)nvn.ValueOffset;
        }

    /// <summary>Materializes a ushort scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private ushort? ReadMaterializedUInt16(string propPath)
        {
            var nvn = GetInlineNavigation(propPath);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
                return ReadMaterializedAs<ushort?>(propPath, materializationOptions ?? deserializationOptions);
            if ((nvn.Type.BaseType | nvn.Type.EnumType) == InhetoBaseTypes.Unknown) return null;
            if ((nvn.Type.BaseType | nvn.Type.EnumType) == InhetoBaseTypes.Null) return null;
            // value was inlined in valueOffset
            return (ushort)nvn.ValueOffset;
        }

    /// <summary>Materializes a int scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private int? ReadMaterializedInt32(string propPath)
        {
            if (TryReadInlineInt32Compiled(propPath, out int? compiledValue, out var readKind) && readKind != CompiledInt32ReadKind.Other)
                return compiledValue;

            var nvn = GetInlineNavigation(propPath);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
                return ReadMaterializedAs<int?>(propPath, materializationOptions ?? deserializationOptions);
            if ((nvn.Type.BaseType | nvn.Type.EnumType) == InhetoBaseTypes.Unknown) return null;
            if ((nvn.Type.BaseType | nvn.Type.EnumType) == InhetoBaseTypes.Null) return null;
            // value was inlined in valueOffset
            return (int)nvn.ValueOffset;
        }

    /// <summary>Materializes a uint scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private uint? ReadMaterializedUInt32(string propPath)
        {
            var nvn = GetInlineNavigation(propPath);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
                return ReadMaterializedAs<uint?>(propPath, materializationOptions ?? deserializationOptions);
            if ((nvn.Type.BaseType | nvn.Type.EnumType) == InhetoBaseTypes.Unknown) return null;
            if ((nvn.Type.BaseType | nvn.Type.EnumType) == InhetoBaseTypes.Null) return null;
            // value was inlined in valueOffset
            return (uint)nvn.ValueOffset;
        }

    /// <summary>Materializes a ulong scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private ulong? ReadMaterializedUInt64(string propPath)
        {
            var nvn = GetNameEntryFromPropPath(propPath);
RedoAfterDupe:
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef)
                return ReadMaterializedAs<ulong?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.DuplicateValue)
            {
                nvn = NameHeaderEntry.CreateBorrowed(stream, nvn.ValueOffset);
                goto RedoAfterDupe;
            }
            stream.Position = nvn.ValueOffset;
            return stream.ReadUInt64();
        }

    /// <summary>Materializes a Int128 scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private Int128? ReadMaterializedInt128(string propPath)
        {
            var nvn = GetNameEntryFromPropPath(propPath);
RedoAfterDupe:
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef)
                return ReadMaterializedAs<Int128?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.DuplicateValue)
            {
                nvn = NameHeaderEntry.CreateBorrowed(stream, nvn.ValueOffset);
                goto RedoAfterDupe;
            }

            stream.Position = nvn.ValueOffset;
            return stream.ReadInt128();
        }

    /// <summary>Materializes a UInt128 scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private UInt128? ReadMaterializedUInt128(string propPath)
        {
            var nvn = GetNameEntryFromPropPath(propPath);
RedoAfterDupe:
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef)
                return ReadMaterializedAs<UInt128?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.DuplicateValue)
            {
                nvn = NameHeaderEntry.CreateBorrowed(stream, nvn.ValueOffset);
                goto RedoAfterDupe;
            }

            stream.Position = nvn.ValueOffset;
            return stream.ReadUInt128();
        }

    /// <summary>Materializes a Half scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private Half? ReadMaterializedHalf(string propPath)
        {
            uint? value = GetMaterializedInlineValue(propPath, out bool custom);
            if (custom) return ReadMaterializedAs<Half?>(propPath, materializationOptions ?? deserializationOptions);
            if (!value.HasValue) return null;

            // value was inlined in valueOffset
            Span<uint> hs = stackalloc uint[1] { value.Value };
            var hb = MemoryMarshal.Read<Half>(MemoryMarshal.AsBytes(hs));
            return hb;
        }

    /// <summary>Materializes a float scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private float? ReadMaterializedSingle(string propPath)
        {
            var nvn = GetInlineNavigation(propPath);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
                return ReadMaterializedAs<float?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return null;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return null;

            // value was inlined in valueOffset
            Span<uint> fs = stackalloc uint[1] { nvn.ValueOffset };
            var fb = MemoryMarshal.AsBytes(fs);
            var fl = MemoryMarshal.Read<float>(fb);
            return fl;
        }

    /// <summary>Materializes a double scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private double? ReadMaterializedDouble(string propPath)
        {
            var nvn = GetNameEntryFromPropPath(propPath);
RedoAfterDupe:
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef)
                return ReadMaterializedAs<double?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.DuplicateValue)
            {
                nvn = NameHeaderEntry.CreateBorrowed(stream, nvn.ValueOffset);
                goto RedoAfterDupe;
            }

            stream.Position = nvn.ValueOffset;
            return stream.ReadDouble();

        }

    /// <summary>Materializes a decimal scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private decimal? ReadMaterializedDecimal(string propPath)
        {
            var nvn = GetNameEntryFromPropPath(propPath);
RedoAfterDupe:
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef)
                return ReadMaterializedAs<decimal?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.DuplicateValue)
            {
                nvn = NameHeaderEntry.CreateBorrowed(stream, nvn.ValueOffset);
                goto RedoAfterDupe;
            }

            stream.Position = nvn.ValueOffset;
            return stream.ReadDecimal();

        }

    /// <summary>Materializes a DateOnly scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private DateOnly? ReadMaterializedDateOnly(string propPath)
        {
            uint? value = GetMaterializedInlineValue(propPath, out bool custom);
            if (custom) return ReadMaterializedAs<DateOnly?>(propPath, materializationOptions ?? deserializationOptions);
            if (!value.HasValue) return null;

            // value was inlined in valueOffset
            Span<uint> fs = stackalloc uint[1] { value.Value };
            var fb = MemoryMarshal.AsBytes(fs);
            var fl = MemoryMarshal.Read<DateOnly>(fb);
            return fl;
        }

    /// <summary>Materializes a DateTime scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private DateTime? ReadMaterializedDateTime(string propPath)
        {
            var nvn = GetNameEntryFromPropPath(propPath);
RedoAfterDupe:
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef)
                return ReadMaterializedAs<DateTime?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.DuplicateValue)
            {
                nvn = NameHeaderEntry.CreateBorrowed(stream, nvn.ValueOffset);
                goto RedoAfterDupe;
            }

            stream.Position = nvn.ValueOffset;
            return stream.ReadDateTime();

        }

    /// <summary>Materializes a DateTimeOffset scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private DateTimeOffset? ReadMaterializedDateTimeOffset(string propPath)
        {
            var nvn = GetNameEntryFromPropPath(propPath);
RedoAfterDupe:
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef)
                return ReadMaterializedAs<DateTimeOffset?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.DuplicateValue)
            {
                nvn = NameHeaderEntry.CreateBorrowed(stream, nvn.ValueOffset);
                goto RedoAfterDupe;
            }

            stream.Position = nvn.ValueOffset;
            return stream.ReadDateTimeOffset();

        }

    /// <summary>Materializes a TimeOnly scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private TimeOnly? ReadMaterializedTimeOnly(string propPath)
        {
            var entry = GetNameEntryFromPropPath(propPath);
            if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
                return ReadMaterializedAs<TimeOnly?>(propPath, materializationOptions ?? deserializationOptions);
            if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType == InhetoBaseTypes.Unknown) return default;
            if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType == InhetoBaseTypes.Null) return default;
            if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType == InhetoBaseTypes.DuplicateRef)
            {
                entry = NameHeaderEntry.CreateBorrowed(stream, entry.ValueOffset, entry.Name);
            }

            return (TimeOnly)GetOrAddCached(entry.Index, k =>
            {
                stream.Position = entry.ValueOffset;
                return stream.ReadTimeOnly();
            });

        }

    /// <summary>Materializes a TimeSpan scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private TimeSpan? ReadMaterializedTimeSpan(string propPath)
        {
            var nvn = GetNameEntryFromPropPath(propPath);
RedoAfterDupe:
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef)
                return ReadMaterializedAs<TimeSpan?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.DuplicateValue)
            {
                nvn = NameHeaderEntry.CreateBorrowed(stream, nvn.ValueOffset);
                goto RedoAfterDupe;
            }

            stream.Position = nvn.ValueOffset;
            return stream.ReadTimeSpan();

        }

    /// <summary>Materializes a Guid scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private Guid? ReadMaterializedGuid(string propPath)
        {
            var nvn = GetNameEntryFromPropPath(propPath);
RedoAfterDupe:
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef)
                return ReadMaterializedAs<Guid?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.DuplicateValue)
            {
                nvn = NameHeaderEntry.CreateBorrowed(stream, nvn.ValueOffset);
                goto RedoAfterDupe;
            }
            stream.Position = nvn.ValueOffset;
            return stream.ReadGuid();
        }

    /// <summary>Materializes a Vector2 scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private Vector2? ReadMaterializedVector2(string propPath)
        {
            var nvn = GetNameEntryFromPropPath(propPath);
RedoAfterDupe:
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef)
                return ReadMaterializedAs<Vector2?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.DuplicateValue)
            {
                nvn = NameHeaderEntry.CreateBorrowed(stream, nvn.ValueOffset);
                goto RedoAfterDupe;
            }

            stream.Position = nvn.ValueOffset;
            return stream.ReadVector2();
        }

    /// <summary>Materializes a Vector3 scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private Vector3? ReadMaterializedVector3(string propPath)
        {
            var nvn = GetNameEntryFromPropPath(propPath);
RedoAfterDupe:
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef)
                return ReadMaterializedAs<Vector3?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.DuplicateValue)
            {
                nvn = NameHeaderEntry.CreateBorrowed(stream, nvn.ValueOffset);
                goto RedoAfterDupe;
            }

            stream.Position = nvn.ValueOffset;
            return stream.ReadVector3();
        }

    /// <summary>Materializes a Vector4 scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private Vector4? ReadMaterializedVector4(string propPath)
        {
            var nvn = GetNameEntryFromPropPath(propPath);
RedoAfterDupe:
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef)
                return ReadMaterializedAs<Vector4?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.DuplicateValue)
            {
                nvn = NameHeaderEntry.CreateBorrowed(stream, nvn.ValueOffset);
                goto RedoAfterDupe;
            }

            stream.Position = nvn.ValueOffset;
            return stream.ReadVector4();
        }

    /// <summary>Materializes a Complex scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private Complex? ReadMaterializedComplex(string propPath)
        {
            var nvn = GetNameEntryFromPropPath(propPath);
RedoAfterDupe:
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef)
                return ReadMaterializedAs<Complex?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.DuplicateValue)
            {
                nvn = NameHeaderEntry.CreateBorrowed(stream, nvn.ValueOffset);
                goto RedoAfterDupe;
            }

            stream.Position = nvn.ValueOffset;
            return stream.ReadComplex();
        }

    /// <summary>Materializes a Quaternion scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private Quaternion? ReadMaterializedQuaternion(string propPath)
        {
            var nvn = GetNameEntryFromPropPath(propPath);
RedoAfterDupe:
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef)
                return ReadMaterializedAs<Quaternion?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.DuplicateValue)
            {
                nvn = NameHeaderEntry.CreateBorrowed(stream, nvn.ValueOffset);
                goto RedoAfterDupe;
            }
            stream.Position = nvn.ValueOffset;
            return stream.ReadQuaternion();
        }

    /// <summary>Materializes a Plane scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private Plane? ReadMaterializedPlane(string propPath)
        {
            var nvn = GetNameEntryFromPropPath(propPath);
RedoAfterDupe:
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef)
                return ReadMaterializedAs<Plane?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.DuplicateValue)
            {
                nvn = NameHeaderEntry.CreateBorrowed(stream, nvn.ValueOffset);
                goto RedoAfterDupe;
            }
            stream.Position = nvn.ValueOffset;
            return stream.ReadPlane();
        }

    /// <summary>Materializes a Matrix3x2 scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private Matrix3x2? ReadMaterializedMatrix3x2(string propPath)
        {
            var nvn = GetNameEntryFromPropPath(propPath);
RedoAfterDupe:
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef)
                return ReadMaterializedAs<Matrix3x2?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.DuplicateValue)
            {
                nvn = NameHeaderEntry.CreateBorrowed(stream, nvn.ValueOffset);
                goto RedoAfterDupe;
            }
            stream.Position = nvn.ValueOffset;
            return stream.ReadMatrix3x2();
        }

    /// <summary>Materializes a Matrix4x4 scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private Matrix4x4? ReadMaterializedMatrix4x4(string propPath)
        {
            var nvn = GetNameEntryFromPropPath(propPath);
RedoAfterDupe:
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef)
                return ReadMaterializedAs<Matrix4x4?>(propPath, materializationOptions ?? deserializationOptions);
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Unknown) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.Null) return default;
            if (nvn.Kind == InhetoKindMasks.InhetoBaseType && nvn.Type.BaseType == InhetoBaseTypes.DuplicateValue)
            {
                nvn = NameHeaderEntry.CreateBorrowed(stream, nvn.ValueOffset);
                goto RedoAfterDupe;
            }
            stream.Position = nvn.ValueOffset;
            return stream.ReadMatrix4x4();
        }

    /// <summary>Materializes a BigInteger scalar while preserving the existing native reader body and cursor/null behavior.<br/>
    /// Only custom terminals enter typed codec/core conversion using the current operation options; native reads add no decoder, boxing or delegate.<br/>
    /// Persisted duplicate handling is retained; public low-level readers and their contracts are unchanged.<br/></summary>
    /// <param name="propPath">Progressive-prefix element path, including reference ancestry.<br/></param>
    /// <returns>The native or custom result, including intentional null; unsupported custom conversion throws.<br/></returns>
    private BigInteger? ReadMaterializedBigInteger(string propPath)
        {
            var entry = GetNameEntryFromPropPath(propPath);
            if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType is
                InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
                return ReadMaterializedAs<BigInteger?>(propPath, materializationOptions ?? deserializationOptions);
            if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType == InhetoBaseTypes.Unknown) return default;
            if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType == InhetoBaseTypes.Null) return default;
            if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType == InhetoBaseTypes.DuplicateRef)
            {
                entry = NameHeaderEntry.CreateBorrowed(stream, entry.ValueOffset, entry.Name);
            }

            return (BigInteger)GetOrAddCached(entry.Index, k =>
            {
                stream.Position = entry.ValueOffset;
                return stream.ReadBigInteger();
            });

        }

    /// <summary>
    /// Compact lookup for the audited Half and DateOnly inline readers.<br/>
    /// Keeps resolution order, numeric cache publication and literal-key behavior; known offsets use the same validating span primitives without an NHE.<br/>
    /// Forward fallback keeps the existing navigation representation local; only a nullable 32-bit slot crosses this return boundary.<br/>
    /// Unknown/null base-kind headers and missing entries produce absence; zero and every other slot bit pattern remain valid values.<br/>
    /// Reports custom/duplicate markers for cold typed dispatch; native values keep the compact slot-only path.<br/>
    /// </summary>
    /// <param name="propPath">Logical path, retaining the existing null/empty root alias and borrowed-payload lifetime.<br/></param>
    /// <param name="custom">True for a custom or duplicate terminal requiring typed materialization.<br/></param>
    /// <returns>The original four-byte slot, or null under precisely the two readers' original missing/unknown/null rules.<br/></returns>
    private uint? GetMaterializedInlineValue(string propPath, out bool custom)
    {
        custom = false;
        var preparedPath = GetPreparedPath(propPath);
        propPath = preparedPath.Path;
        NameHeaderEntry entry;
        if (TryGetResolvedNameEntry(preparedPath, out uint index))
        {
            return index == 0 ? null : ReadMaterializedInlineValue(stream, index, out custom);
        }
        if (TryFindCompiledNameEntry(preparedPath, out index))
        {
            uint? value = index == 0 ? null : ReadMaterializedInlineValue(stream, index, out custom);
            CacheResolvedNameEntry(preparedPath, index);
            return value;
        }
        if (propPath.Length == 0)
        {
            uint? value = ReadMaterializedInlineValue(stream, 1, out custom);
            CacheResolvedNameEntry(preparedPath, 1);
            return value;
        }
        entry = NameHeaderEntry.CreateInlineNavigation(stream, 1);
        while (entry.Type.BaseType != InhetoBaseTypes.EndOfNamesHeader)
        {
            if (entry.IsEmpty || entry.NextEntryIndex == 0) break;
            var next = NameHeaderEntry.CreateInlineNavigation(stream, entry.NextEntryIndex);
            if (next.IsEmpty) break;
            string path = next.PropPath;
            CacheResolvedNameEntry(path, next);
            if (path == propPath) { entry = next; goto Found; }
            entry = next;
        }
        // The physical scan missed; continue through references without rewriting the request.
        if (TryFindReferencedNameEntry(propPath, out uint referenceIndex))
        {
            CacheResolvedNameEntry(preparedPath, referenceIndex);
            return ReadMaterializedInlineValue(stream, referenceIndex, out custom);
        }
        CacheResolvedNameEntry(preparedPath, 0);
        return null;

    Found:
        custom = entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType is InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue;
        if (entry.Kind == InhetoKindMasks.InhetoBaseType &&
            (entry.Type.BaseType == InhetoBaseTypes.Unknown || entry.Type.BaseType == InhetoBaseTypes.Null)) return null;
        return entry.ValueOffset;
    }

    /// <summary>
    /// Reads a known header's inline slot without constructing or transporting a NameHeaderEntry.<br/>
    /// Consumes the parent, complete type grammar, decoded/interned name and four-byte slot in the original order.<br/>
    /// Name decoding and interning are deliberately retained for encoding callbacks and exception parity; no shared cursor is moved.<br/>
    /// SkipType retains enum-byte consumption and recursive/MD/comparer validation without native child storage.<br/>
    /// Only exact base-kind Unknown/Null markers produce absence, and only after the complete header has been validated.<br/>
    /// Other markers retain the existing readers' raw inline interpretation, including duplicates and unrelated shapes.<br/>
    /// </summary>
    /// <param name="buffer">Stable borrowed payload and encoding for the synchronous read.<br/></param>
    /// <param name="index">Exact logical header offset, validated by the existing span reader.<br/></param>
    /// <param name="custom">True for a custom or duplicate marker; no registry is examined here.<br/></param>
    /// <returns>The original 32 bits or absence under the Half/DateOnly contract; no managed references cross this boundary.<br/></returns>
    private static uint? ReadMaterializedInlineValue(BufferStream buffer, uint index, out bool custom)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        var reader = new NameHeaderSpanReader(buffer.AsReadOnlySpan, checked((int)index), buffer.StringEncoding);
        _ = reader.Read7BitEncodedUInt();
        byte marker = reader.PeekByte();
        custom = marker is (byte)InhetoBaseTypes.CustomProp or (byte)InhetoBaseTypes.CustomType or (byte)InhetoBaseTypes.DuplicateRef or (byte)InhetoBaseTypes.DuplicateValue;
        reader.SkipType();
        _ = string.Intern(reader.ReadString()!);
        uint value = reader.ReadUInt32();
        return marker is (byte)InhetoBaseTypes.Unknown or (byte)InhetoBaseTypes.Null ? null : value;
    }

    /// <summary>Materializes Int64 with the scoped native navigation/cleanup contract and a cold custom decoder path.<br/></summary>
    /// <param name="propPath">Progressive-prefix path to the scalar.<br/></param>
    /// <returns>The native/custom scalar or intentional null; unsupported core conversion throws.<br/></returns>
    private long? ReadMaterializedInt64(string propPath)
    {
        NameHeaderAllocationGroup group = default;
        try
        {
            if (!TryGetScalarNavigation(GetPreparedPath(propPath), ref group, out var entry))
                return ReadMaterializedAs<long?>(propPath, materializationOptions ?? deserializationOptions);
            while (true)
            {
                if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType is InhetoBaseTypes.Unknown or InhetoBaseTypes.Null) return null;
                if (entry.Kind == InhetoKindMasks.InhetoBaseType && entry.Type.BaseType is InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType or InhetoBaseTypes.DuplicateRef)
                    return ReadMaterializedAs<long?>(propPath, materializationOptions ?? deserializationOptions);
                if (entry.Kind != InhetoKindMasks.InhetoBaseType || entry.Type.BaseType != InhetoBaseTypes.DuplicateValue) break;
                if (!NameHeaderEntry.TryCreateScoped(stream, entry.ValueOffset, null, ref group, out entry))
                    return ReadMaterializedAs<long?>(propPath, materializationOptions ?? deserializationOptions);
            }
            stream.Position = entry.ValueOffset;
            return stream.ReadInt64();
        }
        finally { group.Release(); }
    }

    /// <summary>
    /// Detects a cold custom-result projection across different scalar element types without changing native same-type loops.<br/>
    /// Same-type collections and conversions with no registered decoders return before scanning any element.<br/>
    /// Only direct scalar siblings are inspected with borrowed navigation metadata; no native descriptors or path index are created.<br/>
    /// A positive result lets the caller coerce decoded values to its destination type rather than back through the source type.<br/>
    /// </summary>
    /// <param name="entry">Resolved collection header whose scalar children follow it in wire order.<br/></param>
    /// <param name="target">Scalar marker expected by the existing strongly typed fill loop.<br/></param>
    /// <returns>True for a differing source shape containing a custom terminal and potentially applicable registrations.<br/></returns>
    private bool HasCustomScalarProjection(in NameHeaderEntry entry, InhetoBaseTypes target)
    {
        if (entry.Type.CollectionInfo.ValueType.BaseType == target) return false;
        var options = materializationOptions ?? deserializationOptions;
        if (options.MemberDeserializers.Count == 0 && options.TypeDeserializers.Count == 0) return false;
        uint index = entry.NextEntryIndex;
        for (int i = 0; i < entry.ValueOffset && index != 0; i++)
        {
            var child = NameHeaderEntry.CreateInlineNavigation(stream, index);
            if (child.ParentIndex != entry.Index) return false;
            index = child.NextEntryIndex;
            while (child.Kind == InhetoKindMasks.InhetoBaseType &&
                child.Type.BaseType is InhetoBaseTypes.DuplicateValue or InhetoBaseTypes.DuplicateRef)
                child = NameHeaderEntry.CreateInlineNavigation(stream, child.ValueOffset);
            if (child.Kind == InhetoKindMasks.InhetoBaseType &&
                child.Type.BaseType is InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType) return true;
        }
        return false;
    }
    /// <summary>
    /// Reads a Type collection slot while keeping native text parsing separate from custom materialization.<br/>
    /// Explicit Null remains a slot value; missing children remain exceptional. Native unresolved names retain Type.GetType's null result.<br/>
    /// Only custom payloads enter the existing typed core with the original header and active operation options, preserving decoder nulls and references.<br/>
    /// Native reads use the same scoped navigation and BufferStream decoder as ReadString, without a per-element public operation or delegate.<br/>
    /// </summary>
    /// <param name="path">Progressive collection child path, including reference ancestry.<br/></param>
    /// <returns>The native or custom Type, including an intentional null or unresolved native name.<br/></returns>
    private Type? ReadMaterializedTypeValue(string path)
    {
        NameHeaderAllocationGroup group = default;
        try
        {
            if (!TryGetScalarNavigation(GetPreparedPath(path), ref group, out var entry))
                return ReadMaterializedAs<Type>(path, materializationOptions ?? deserializationOptions);
            var original = entry;
            if (entry.Kind == InhetoKindMasks.InhetoBaseType &&
                entry.Type.BaseType is InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
                if (!NameHeaderEntry.TryCreateScoped(stream, entry.ValueOffset, null, ref group, out entry))
                    return ReadMaterializedAs<Type>(path, materializationOptions ?? deserializationOptions);
            if (entry.Kind == InhetoKindMasks.InhetoBaseType)
            {
                if (entry.Type.BaseType == InhetoBaseTypes.Null) return null;
                if (entry.Type.BaseType == InhetoBaseTypes.Unknown)
                    throw new InvalidDataException($"Missing Type collection element at '{path}'.");
                if (entry.Type.BaseType is InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType)
                    return ReadMaterializedAs<Type>(default, original, path, materializationOptions ?? deserializationOptions, true);
            }
            stream.Position = entry.ValueOffset;
            return Type.GetType(stream.ReadString()!);
        }
        finally { group.Release(); }
    }

    /// <summary>
    /// Reads a TimeZoneInfo collection slot, preserving explicit nulls and custom decoder results.<br/>
    /// Native strings retain FromSerializedString conversion, including its existing failure for empty strings or zone IDs; scalar ID fallback is not added here.<br/>
    /// Custom and referenced-custom elements use the current internal materialization operation, never a nested public read wrapper.<br/>
    /// Scoped native descriptors are released on success and exception; ordinary native reads add no callback, reflection or cache lookup for decoded values.<br/>
    /// </summary>
    /// <param name="path">Progressive collection child path used for existing name and reference resolution.<br/></param>
    /// <returns>The reconstructed zone or explicit/custom null; invalid native payloads remain exceptional.<br/></returns>
    private TimeZoneInfo? ReadMaterializedTimeZoneValue(string path)
    {
        NameHeaderAllocationGroup group = default;
        try
        {
            if (!TryGetScalarNavigation(GetPreparedPath(path), ref group, out var entry))
                return ReadMaterializedAs<TimeZoneInfo>(path, materializationOptions ?? deserializationOptions);
            var original = entry;
            if (entry.Kind == InhetoKindMasks.InhetoBaseType &&
                entry.Type.BaseType is InhetoBaseTypes.DuplicateRef or InhetoBaseTypes.DuplicateValue)
                if (!NameHeaderEntry.TryCreateScoped(stream, entry.ValueOffset, null, ref group, out entry))
                    return ReadMaterializedAs<TimeZoneInfo>(path, materializationOptions ?? deserializationOptions);
            if (entry.Kind == InhetoKindMasks.InhetoBaseType)
            {
                if (entry.Type.BaseType == InhetoBaseTypes.Null) return null;
                if (entry.Type.BaseType == InhetoBaseTypes.Unknown)
                    throw new InvalidDataException($"Missing TimeZoneInfo collection element at '{path}'.");
                if (entry.Type.BaseType is InhetoBaseTypes.CustomProp or InhetoBaseTypes.CustomType)
                    return ReadMaterializedAs<TimeZoneInfo>(default, original, path, materializationOptions ?? deserializationOptions, true);
            }
            stream.Position = entry.ValueOffset;
            return TimeZoneInfo.FromSerializedString(stream.ReadString()!);
        }
        finally { group.Release(); }
    }
}
