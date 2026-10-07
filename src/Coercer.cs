using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Inheto
{
    /// <summary>
    /// Configures the supported runtime conversion branches. Configure before concurrent use; this mutable object does not synchronize option updates.<br/>
    /// </summary>
    public sealed class CoerceOptions
    {
        /// <summary>
        /// Gets or sets the culture used for supported textual conversions; the default is invariant culture.<br/>
        /// </summary>
        public CultureInfo Culture { get; init; } = CultureInfo.InvariantCulture;
        /// <summary>
        /// Gets or sets whether floating-point and decimal inputs may be rounded and converted to integral destinations; disabled by default.<br/>
        /// </summary>
        public bool AllowFloatToInt { get; init; } = false;                 // disallow by default (STJ-like)
        /// <summary>
        /// Gets or sets midpoint rounding used before an enabled floating-point-to-integral conversion; the default is ToEven.<br/>
        /// </summary>
        public MidpointRounding FloatRounding { get; init; } = MidpointRounding.ToEven;
        /// <summary>
        /// Gets or sets whether string inputs may be parsed as case-insensitive enum names; enabled by default.<br/>
        /// </summary>
        public bool EnumParseNames { get; init; } = true;
        /// <summary>
        /// Gets or sets an additional admission flag for string conversion. Disabling it does not block values already admitted through IFormattable or IConvertible.<br/>
        /// </summary>
        public bool AllowNumberToString { get; init; } = false;
        /// <summary>
        /// Gets or sets whether string inputs may enter the numeric parsing branch; enabled by default.<br/>
        /// </summary>
        public bool AllowStringToNumber { get; init; } = true;
        /// <summary>
        /// Gets or sets whether TypeConverter conversion is attempted after the direct branches; disabled by default. Developer converters may execute arbitrary code.<br/>
        /// </summary>
        public bool UseTypeConvertersFallback { get; init; } = false;       // off for perf unless you need it
    }

    /// <summary>
    /// Converts supported scalar values using direct conversion branches and cached user-defined operators, with optional TypeConverter fallback.<br/>
    /// </summary>
    public static class FastCoerce
    {
        private readonly struct TypePair : IEquatable<TypePair>
        {
            public readonly Type Src;
            public readonly Type Dst;
            public TypePair(Type s, Type d) { Src = s; Dst = d; }
            public bool Equals(TypePair other) => Src == other.Src && Dst == other.Dst;
            public override bool Equals(object? obj) => obj is TypePair tp && Equals(tp);
            public override int GetHashCode() => HashCode.Combine(Src, Dst);
        }

        private static readonly ConcurrentDictionary<TypePair, Func<object, object?>> _opCache = new();
        private static readonly Func<object, object?> _missingOperator = static _ => null;

        /// <summary>
        /// Converts a supported runtime value to the requested CLR type, preserving already assignable values. Throws InvalidCastException when TryCoerce reports no supported conversion; other conversion or developer-operator exceptions may propagate.<br/>
        /// </summary>
        /// <typeparam name="T">CLR type used by this operation; result conversion or exact-type registration follows the documented contract.<br/></typeparam>
        /// <param name="value">Input member or scalar value, including null only where the declared type and conversion allow it.<br/></param>
        /// <param name="opt">Optional conversion policy; omission creates the default conversion policy.<br/></param>
        /// <returns>Converted or retrieved value, according to the documented operation; no blanket exception suppression is performed.<br/></returns>
        public static T? Coerce<T>(object? value, CoerceOptions? opt = null)
            => (T?)Coerce(value, typeof(T), opt);

        /// <summary>
        /// Converts a supported runtime value to the requested CLR type, preserving already assignable values. Throws InvalidCastException when TryCoerce reports no supported conversion; other conversion or developer-operator exceptions may propagate.<br/>
        /// </summary>
        /// <param name="value">Input member or scalar value, including null only where the declared type and conversion allow it.<br/></param>
        /// <param name="targetType">Requested destination CLR type, including a nullable wrapper when applicable.<br/></param>
        /// <param name="opt">Optional conversion policy; omission creates the default conversion policy.<br/></param>
        /// <returns>Converted or retrieved value, according to the documented operation; no blanket exception suppression is performed.<br/></returns>
        public static object? Coerce(object? value, Type targetType, CoerceOptions? opt = null)
            => TryCoerce(value, targetType, out var r, opt) ? r : throw new InvalidCastException(
                $"Cannot coerce {value?.GetType().FullName ?? "null"} to {targetType.FullName}.");

        /// <summary>
        /// Attempts a supported runtime conversion, returning false and a null result when no branch succeeds. This is not a catch-all exception boundary: developer operators and some conversion failures can still throw.<br/>
        /// </summary>
        /// <param name="value">Input member or scalar value, including null only where the declared type and conversion allow it.<br/></param>
        /// <param name="targetType">Requested destination CLR type, including a nullable wrapper when applicable.<br/></param>
        /// <param name="result">Receives the converted result on success, or null when no supported conversion succeeds.<br/></param>
        /// <param name="opt">Optional conversion policy; omission creates the default conversion policy.<br/></param>
        /// <returns>True when a supported branch produced a result, including an admissible null; otherwise false.<br/></returns>
        public static bool TryCoerce(object? value, Type targetType, out object? result, CoerceOptions? opt = null)
        {
            opt ??= new CoerceOptions();
            var nnT = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // null
            if (value is null)
            {
                if (Nullable.GetUnderlyingType(targetType) != null || !targetType.IsValueType)
                { result = null; return true; }
                result = null; return false;
            }

            var srcT = value.GetType();

            

            // identity / assignable
            if (srcT == nnT || nnT.IsInstanceOfType(value))
            { result = value; return true; }

            // bool coercion
            if (nnT == typeof(bool))
            {
                if (value is bool b) { result = b; return true; }
                if (IsNumericType(srcT)) { result = Convert.ToInt64(value) != 0; return true; }
                if (value is string sb && bool.TryParse(sb, out var bparsed))
                { result = bparsed; return true; }
                result = null; return false;
            }

            if(targetType == System.Net.IPAddress.Loopback.GetType())
            {
                if (value is string ipStr && IPAddress.TryParse(ipStr, out var parsed))
                    value = parsed;

                if (value is IPAddress ip)
                {
                    if (ip.Equals(IPAddress.None)) { result = System.Net.IPAddress.None; return true; }
                    if (ip.Equals(IPAddress.Any)) { result = System.Net.IPAddress.Any; return true; }
                    if (ip.Equals(IPAddress.Broadcast)) { result = System.Net.IPAddress.Broadcast; return true; }
                    if (ip.Equals(IPAddress.IPv6Any)) { result = System.Net.IPAddress.IPv6Any; return true; }
                    if (ip.Equals(IPAddress.IPv6Loopback)) { result = System.Net.IPAddress.IPv6Loopback; return true; }
                    if (ip.Equals(IPAddress.IPv6None)) { result = System.Net.IPAddress.IPv6None; return true; }
                    if (ip.Equals(IPAddress.Loopback)) { result = System.Net.IPAddress.Loopback; return true; }
                    result = null; return false; 
                }
            }

            // numeric family
            if (IsNumericType(srcT) && IsNumericType(nnT))
            {
                if (TryCoerceNumeric(value, srcT, nnT, opt, out result)) return true;
                result = null; return false;
            }

            // enums
            if (nnT.IsEnum)
            {
                if (opt.EnumParseNames && value is string es && Enum.TryParse(nnT, es, true, out var eparsed))
                { result = eparsed; return true; }

                if (IsNumericType(srcT))
                {
                    var ut = Enum.GetUnderlyingType(nnT);
                    if (TryCoerceNumeric(value, srcT, ut, opt, out var numBox))
                    { result = Enum.ToObject(nnT, numBox!); return true; }
                }
                result = null; return false;
            }

            // string -> primitives
            if (value is string s)
            {
                // string -> numeric
                if (opt.AllowStringToNumber && IsNumericType(nnT) &&
                    TryParseNumericFromString(s, nnT, opt.Culture, out result))
                    return true;

                // common string parsables
                if (nnT == typeof(Guid) && Guid.TryParse(s, out var g)) { result = g; return true; }
                if (nnT == typeof(Uri) && Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out var u)) { result = u; return true; }
                if (nnT == typeof(TimeSpan) && TimeSpan.TryParse(s, opt.Culture, out var ts)) { result = ts; return true; }
                if (nnT == typeof(DateTime) && DateTime.TryParse(s, opt.Culture, DateTimeStyles.RoundtripKind, out var dt)) { result = dt; return true; }
                if (nnT == typeof(DateTimeOffset) && DateTimeOffset.TryParse(s, opt.Culture, DateTimeStyles.RoundtripKind, out var dto)) { result = dto; return true; }
#if NET8_0_OR_GREATER
                if (nnT == typeof(DateOnly) && DateOnly.TryParse(s, opt.Culture, DateTimeStyles.None, out var dOnly)) { result = dOnly; return true; }
                if (nnT == typeof(TimeOnly) && TimeOnly.TryParse(s, opt.Culture, DateTimeStyles.None, out var tOnly)) { result = tOnly; return true; }
#endif
            }
            else if (nnT == typeof(string) &&
                     (opt.AllowNumberToString || value is IFormattable || value is IConvertible))
            {
                result = Convert.ToString(value, opt.Culture); return true;
            }

            // user-defined implicit/explicit operators
            if (TryOperator(value, srcT, nnT, out result)) return true;

            // optional TypeConverter fallback (slower)
            if (opt.UseTypeConvertersFallback)
            {
                var toConv = System.ComponentModel.TypeDescriptor.GetConverter(nnT);
                if (toConv.CanConvertFrom(srcT))
                {
                    try { result = toConv.ConvertFrom(null, opt.Culture, value); return true; } catch { }
                }
                var fromConv = System.ComponentModel.TypeDescriptor.GetConverter(srcT);
                if (fromConv.CanConvertTo(nnT))
                {
                    try { result = fromConv.ConvertTo(null, opt.Culture, value, nnT); return true; } catch { }
                }
            }

            result = null;
            return false;
        }

        // ---------- helpers ----------

        private static bool TryOperator(object value, Type src, Type dst, out object? result)
        {
            var key = new TypePair(src, dst);
            var del = _opCache.GetOrAdd(key, static k =>
            {
                static MethodInfo? Find(Type decl, Type s, Type d, string name)
                    => decl.GetMethods(BindingFlags.Public | BindingFlags.Static)
                           .FirstOrDefault(m => m.Name == name
                                && m.ReturnType == d
                                && m.GetParameters() is { Length: 1 } p
                                && p[0].ParameterType.IsAssignableFrom(s));

                var mi = Find(k.Src, k.Src, k.Dst, "op_Implicit")
                      ?? Find(k.Dst, k.Src, k.Dst, "op_Implicit")
                      ?? Find(k.Src, k.Src, k.Dst, "op_Explicit")
                      ?? Find(k.Dst, k.Src, k.Dst, "op_Explicit");

                if (mi is null) return _missingOperator;

                // build Func<object, object?>
                var pObj = System.Linq.Expressions.Expression.Parameter(typeof(object), "o");
                var body = System.Linq.Expressions.Expression.Convert(
                    System.Linq.Expressions.Expression.Call(mi,
                        System.Linq.Expressions.Expression.Convert(pObj, k.Src)),
                    typeof(object));
                var lambda = System.Linq.Expressions.Expression.Lambda<Func<object, object?>>(body, pObj);
                return lambda.Compile();
            });

            if (ReferenceEquals(del, _missingOperator)) { result = null; return false; }
            result = del(value); return true;
        }

        private static bool IsNumericType(Type t)
        {
            t = Nullable.GetUnderlyingType(t) ?? t;
            return Type.GetTypeCode(t) switch
            {
                TypeCode.SByte or TypeCode.Byte or TypeCode.Int16 or TypeCode.UInt16 or
                TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64 or
                TypeCode.Single or TypeCode.Double or TypeCode.Decimal => true,
                _ => false
            };
        }

        private static bool IsFloaty(Type t)
        {
            t = Nullable.GetUnderlyingType(t) ?? t;
            return Type.GetTypeCode(t) is TypeCode.Single or TypeCode.Double or TypeCode.Decimal;
        }

        private static bool IsIntegral(Type t)
        {
            t = Nullable.GetUnderlyingType(t) ?? t;
            return Type.GetTypeCode(t) is TypeCode.SByte or TypeCode.Byte or TypeCode.Int16 or TypeCode.UInt16
                                          or TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64;
        }

        private static bool TryCoerceNumeric(object value, Type srcT, Type dstT, CoerceOptions opt, out object? result)
        {
            var srcCode = Type.GetTypeCode(Nullable.GetUnderlyingType(srcT) ?? srcT);
            var dstCode = Type.GetTypeCode(Nullable.GetUnderlyingType(dstT) ?? dstT);

            // floats to ints policy
            if (IsFloaty(srcT) && IsIntegral(dstT) && !opt.AllowFloatToInt)
            { result = null; return false; }

            // normalize to double for rounding when needed
            if (IsFloaty(srcT) && IsIntegral(dstT))
            {
                double d = srcCode switch
                {
                    TypeCode.Single => (double)(float)value,
                    TypeCode.Double => (double)value,
                    TypeCode.Decimal => (double)(decimal)value,
                    _ => throw new InvalidOperationException()
                };
                d = Math.Round(d, 0, opt.FloatRounding);
                // fall through as if source is Int64 (fits range checks below)
                srcCode = TypeCode.Int64;
                value = (long)d;
            }

            // fast path: switch per destination
            checked
            {
                try
                {
                    switch (dstCode)
                    {
                        case TypeCode.SByte:
                            result = srcCode switch
                            {
                                TypeCode.SByte => (sbyte)value,
                                TypeCode.Byte => (sbyte)(byte)value,
                                TypeCode.Int16 => (sbyte)(short)value,
                                TypeCode.UInt16 => (sbyte)(ushort)value,
                                TypeCode.Int32 => (sbyte)(int)value,
                                TypeCode.UInt32 => (sbyte)(uint)value,
                                TypeCode.Int64 => (sbyte)(long)value,
                                TypeCode.UInt64 => (sbyte)(ulong)value,
                                TypeCode.Single => (sbyte)(float)value,
                                TypeCode.Double => (sbyte)(double)value,
                                TypeCode.Decimal => (sbyte)(decimal)value,
                                _ => throw new InvalidCastException()
                            }; return true;

                        case TypeCode.Byte:
                            result = srcCode switch
                            {
                                TypeCode.SByte => (byte)(sbyte)value,
                                TypeCode.Byte => (byte)value,
                                TypeCode.Int16 => (byte)(short)value,
                                TypeCode.UInt16 => (byte)(ushort)value,
                                TypeCode.Int32 => (byte)(int)value,
                                TypeCode.UInt32 => (byte)(uint)value,
                                TypeCode.Int64 => (byte)(long)value,
                                TypeCode.UInt64 => (byte)(ulong)value,
                                TypeCode.Single => (byte)(float)value,
                                TypeCode.Double => (byte)(double)value,
                                TypeCode.Decimal => (byte)(decimal)value,
                                _ => throw new InvalidCastException()
                            }; return true;

                        case TypeCode.Int16:
                            result = srcCode switch
                            {
                                TypeCode.SByte => (short)(sbyte)value,
                                TypeCode.Byte => (short)(byte)value,
                                TypeCode.Int16 => (short)value,
                                TypeCode.UInt16 => (short)(ushort)value,
                                TypeCode.Int32 => (short)(int)value,
                                TypeCode.UInt32 => (short)(uint)value,
                                TypeCode.Int64 => (short)(long)value,
                                TypeCode.UInt64 => (short)(ulong)value,
                                TypeCode.Single => (short)(float)value,
                                TypeCode.Double => (short)(double)value,
                                TypeCode.Decimal => (short)(decimal)value,
                                _ => throw new InvalidCastException()
                            }; return true;

                        case TypeCode.UInt16:
                            result = srcCode switch
                            {
                                TypeCode.SByte => (ushort)(sbyte)value,
                                TypeCode.Byte => (ushort)(byte)value,
                                TypeCode.Int16 => (ushort)(short)value,
                                TypeCode.UInt16 => (ushort)value,
                                TypeCode.Int32 => (ushort)(int)value,
                                TypeCode.UInt32 => (ushort)(uint)value,
                                TypeCode.Int64 => (ushort)(long)value,
                                TypeCode.UInt64 => (ushort)(ulong)value,
                                TypeCode.Single => (ushort)(float)value,
                                TypeCode.Double => (ushort)(double)value,
                                TypeCode.Decimal => (ushort)(decimal)value,
                                _ => throw new InvalidCastException()
                            }; return true;

                        case TypeCode.Int32:
                            result = srcCode switch
                            {
                                TypeCode.SByte => (int)(sbyte)value,
                                TypeCode.Byte => (int)(byte)value,
                                TypeCode.Int16 => (int)(short)value,
                                TypeCode.UInt16 => (int)(ushort)value,
                                TypeCode.Int32 => (int)value,
                                TypeCode.UInt32 => checked((int)(uint)value),
                                TypeCode.Int64 => checked((int)(long)value),
                                TypeCode.UInt64 => checked((int)(ulong)value),
                                TypeCode.Single => (int)(float)value,
                                TypeCode.Double => (int)(double)value,
                                TypeCode.Decimal => (int)(decimal)value,
                                _ => throw new InvalidCastException()
                            }; return true;

                        case TypeCode.UInt32:
                            result = srcCode switch
                            {
                                TypeCode.SByte => checked((uint)(sbyte)value),
                                TypeCode.Byte => (uint)(byte)value,
                                TypeCode.Int16 => checked((uint)(short)value),
                                TypeCode.UInt16 => (uint)(ushort)value,
                                TypeCode.Int32 => checked((uint)(int)value),
                                TypeCode.UInt32 => (uint)value,
                                TypeCode.Int64 => checked((uint)(long)value),
                                TypeCode.UInt64 => checked((uint)(ulong)value),
                                TypeCode.Single => checked((uint)(float)value),
                                TypeCode.Double => checked((uint)(double)value),
                                TypeCode.Decimal => checked((uint)(decimal)value),
                                _ => throw new InvalidCastException()
                            }; return true;

                        case TypeCode.Int64:
                            result = srcCode switch
                            {
                                TypeCode.SByte => (long)(sbyte)value,
                                TypeCode.Byte => (long)(byte)value,
                                TypeCode.Int16 => (long)(short)value,
                                TypeCode.UInt16 => (long)(ushort)value,
                                TypeCode.Int32 => (long)(int)value,
                                TypeCode.UInt32 => (long)(uint)value,
                                TypeCode.Int64 => (long)value,
                                TypeCode.UInt64 => checked((long)(ulong)value),
                                TypeCode.Single => (long)(float)value,
                                TypeCode.Double => (long)(double)value,
                                TypeCode.Decimal => (long)(decimal)value,
                                _ => throw new InvalidCastException()
                            }; return true;

                        case TypeCode.UInt64:
                            result = srcCode switch
                            {
                                TypeCode.SByte => checked((ulong)(sbyte)value),
                                TypeCode.Byte => (ulong)(byte)value,
                                TypeCode.Int16 => checked((ulong)(short)value),
                                TypeCode.UInt16 => (ulong)(ushort)value,
                                TypeCode.Int32 => checked((ulong)(int)value),
                                TypeCode.UInt32 => (ulong)(uint)value,
                                TypeCode.Int64 => checked((ulong)(long)value),
                                TypeCode.UInt64 => (ulong)value,
                                TypeCode.Single => checked((ulong)(float)value),
                                TypeCode.Double => checked((ulong)(double)value),
                                TypeCode.Decimal => checked((ulong)(decimal)value),
                                _ => throw new InvalidCastException()
                            }; return true;

                        case TypeCode.Single:
                            result = srcCode switch
                            {
                                TypeCode.SByte => (float)(sbyte)value,
                                TypeCode.Byte => (float)(byte)value,
                                TypeCode.Int16 => (float)(short)value,
                                TypeCode.UInt16 => (float)(ushort)value,
                                TypeCode.Int32 => (float)(int)value,
                                TypeCode.UInt32 => (float)(uint)value,
                                TypeCode.Int64 => (float)(long)value,
                                TypeCode.UInt64 => (float)(ulong)value,
                                TypeCode.Single => (float)value,
                                TypeCode.Double => (float)(double)value,
                                TypeCode.Decimal => (float)(decimal)value,
                                _ => throw new InvalidCastException()
                            }; return true;

                        case TypeCode.Double:
                            result = srcCode switch
                            {
                                TypeCode.SByte => (double)(sbyte)value,
                                TypeCode.Byte => (double)(byte)value,
                                TypeCode.Int16 => (double)(short)value,
                                TypeCode.UInt16 => (double)(ushort)value,
                                TypeCode.Int32 => (double)(int)value,
                                TypeCode.UInt32 => (double)(uint)value,
                                TypeCode.Int64 => (double)(long)value,
                                TypeCode.UInt64 => (double)(ulong)value,
                                TypeCode.Single => (double)(float)value,
                                TypeCode.Double => (double)value,
                                TypeCode.Decimal => (double)(decimal)value,
                                _ => throw new InvalidCastException()
                            }; return true;

                        case TypeCode.Decimal:
                            result = srcCode switch
                            {
                                TypeCode.SByte => (decimal)(sbyte)value,
                                TypeCode.Byte => (decimal)(byte)value,
                                TypeCode.Int16 => (decimal)(short)value,
                                TypeCode.UInt16 => (decimal)(ushort)value,
                                TypeCode.Int32 => (decimal)(int)value,
                                TypeCode.UInt32 => (decimal)(uint)value,
                                TypeCode.Int64 => (decimal)(long)value,
                                TypeCode.UInt64 => (decimal)(ulong)value,
                                TypeCode.Single => (decimal)(float)value,
                                TypeCode.Double => (decimal)(double)value,
                                TypeCode.Decimal => (decimal)value,
                                _ => throw new InvalidCastException()
                            }; return true;
                    }
                }
                catch { /* overflow or cast */ }
            }

            result = null;
            return false;
        }

        private static bool TryParseNumericFromString(string s, Type target, CultureInfo culture, out object? result)
        {
            result = null;
            var tc = Type.GetTypeCode(Nullable.GetUnderlyingType(target) ?? target);
            // use invariant culture parsing to avoid allocations
            switch (tc)
            {
                case TypeCode.SByte: if (sbyte.TryParse(s, NumberStyles.Integer, culture, out var sb)) { result = sb; return true; } break;
                case TypeCode.Byte: if (byte.TryParse(s, NumberStyles.Integer, culture, out var b)) { result = b; return true; } break;
                case TypeCode.Int16: if (short.TryParse(s, NumberStyles.Integer, culture, out var i16)) { result = i16; return true; } break;
                case TypeCode.UInt16: if (ushort.TryParse(s, NumberStyles.Integer, culture, out var u16)) { result = u16; return true; } break;
                case TypeCode.Int32: if (int.TryParse(s, NumberStyles.Integer, culture, out var i32)) { result = i32; return true; } break;
                case TypeCode.UInt32: if (uint.TryParse(s, NumberStyles.Integer, culture, out var u32)) { result = u32; return true; } break;
                case TypeCode.Int64: if (long.TryParse(s, NumberStyles.Integer, culture, out var i64)) { result = i64; return true; } break;
                case TypeCode.UInt64: if (ulong.TryParse(s, NumberStyles.Integer, culture, out var u64)) { result = u64; return true; } break;
                case TypeCode.Single: if (float.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, culture, out var f)) { result = f; return true; } break;
                case TypeCode.Double: if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, culture, out var d)) { result = d; return true; } break;
                case TypeCode.Decimal: if (decimal.TryParse(s, NumberStyles.Number, culture, out var m)) { result = m; return true; } break;
            }
            return false;
        }
    }
}
