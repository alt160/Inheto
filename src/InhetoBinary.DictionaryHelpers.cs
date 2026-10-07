using Inheto;
using System.Collections;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Numerics;
using System.Text;








namespace Inheto
{
    //====== TYPES ======
    public partial class InhetoBinary
    {








        //======  METHODS  ======
        private void fillBigIntegerKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = BigInteger.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<BigInteger, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillBooleanKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Boolean.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = bool.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<bool, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillByteKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = byte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<byte, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillCharKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = char.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName[0];
                            var d = dict as IDictionary<char, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillCultureInfoKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new CultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = CultureInfo.GetCultureInfo(keyEntry.CleanName);
                            var d = dict as IDictionary<CultureInfo, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillDateOnlyKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateOnly, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Fills DateTime-key dictionaries through the existing specialized value readers.<br/>
        /// Round-trip parsing preserves encoded key ticks and Kind; value lookup retains the original entry path.<br/>
        /// Display formatting is not used to reconstruct wire paths, and destination identity/comparer are retained.<br/>
        /// </summary>
        /// <param name="dict">Existing destination dictionary.<br/></param>
        /// <param name="dCount">Encoded number of entries to insert.<br/></param>
        /// <param name="dictEntry">Borrowed dictionary parent header.<br/></param>
        /// <param name="propPath">Dictionary path retained for caller compatibility.<br/></param>
        /// <param name="vType">Value family selecting its specialized reader.<br/></param>
        /// <param name="dzOptions">Optional complex-value activation and codec policy.<br/></param>
        private void fillDateTimeKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, keyEntry.PropPath, dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, string>;
                            d!.Add(keyStr, ReadStringCore(keyEntry.PropPath)!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, bool>;
                            d!.Add(keyStr, ReadBooleanCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, byte>;
                            d!.Add(keyStr, ReadByteCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, char>;
                            d!.Add(keyStr, ReadCharCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, short>;
                            d!.Add(keyStr, ReadInt16Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, Half>;
                            d!.Add(keyStr, ReadHalfCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, Rune>;
                            d!.Add(keyStr, ReadRuneCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, int>;
                            d!.Add(keyStr, ReadInt32Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, uint>;
                            d!.Add(keyStr, ReadUInt32Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, float>;
                            d!.Add(keyStr, ReadSingleCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, long>;
                            d!.Add(keyStr, ReadInt64Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, double>;
                            d!.Add(keyStr, ReadDoubleCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, Int128>;
                            d!.Add(keyStr, ReadInt128Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, Guid>;
                            d!.Add(keyStr, ReadGuidCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, Version>;
                            d!.Add(keyStr, ReadVersionCore(keyEntry.PropPath)!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, Uri>;
                            d!.Add(keyStr, ReadUriCore(keyEntry.PropPath)!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, Complex>;
                            d!.Add(keyStr, ReadComplexCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTime.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var d = dict as IDictionary<DateTime, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(keyEntry.PropPath));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillDateTimeOffsetKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = DateTimeOffset.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<DateTimeOffset, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillDecimalKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = decimal.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<decimal, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillDoubleKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = double.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<double, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillGuidKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Guid.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Guid, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillHalfKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Half.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Half, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillInt128KeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Int128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<Int128, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillInt16KeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = short.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<short, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillInt32KeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = int.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<int, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillInt64KeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = long.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<long, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillIPAddressKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPAddress.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPAddress, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillIPEndPointKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = System.Net.IPEndPoint.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<System.Net.IPEndPoint, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillRuneKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Rune.TryGetRuneAt(keyEntry.CleanName, 0, out var rune) ? rune : default;
                            var d = dict as IDictionary<Rune, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillSByteKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = sbyte.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<sbyte, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillSingleKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = float.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<float, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillStringKeyDictionary(object dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            var isCD = dict.GetType() is Type dt && dt.IsGenericType && dt.GetGenericTypeDefinition() == typeof(ConcurrentDictionary<,>);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        var d = dict as IDictionary<string, string>;
                        {
                            for (int i = 0; i < dCount; i++)
                            {
                                var keyStr = keyEntry.CleanName;
                                d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                                keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                            }
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        var d = dict as IDictionary<string, bool>;
                        {
                            for (int i = 0; i < dCount; i++)
                            {
                                var keyStr = keyEntry.CleanName;
                                d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                                keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                            }
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        var d = dict as IDictionary<string, sbyte>;
                        {
                            for (int i = 0; i < dCount; i++)
                            {
                                var keyStr = keyEntry.CleanName;
                                d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                                keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                            }
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            var d = dict as IDictionary<string, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            var d = dict as IDictionary<string, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            var d = dict as IDictionary<string, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            var d = dict as IDictionary<string, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            var d = dict as IDictionary<string, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            var d = dict as IDictionary<string, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            var d = dict as IDictionary<string, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            var d = dict as IDictionary<string, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            var d = dict as IDictionary<string, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            var d = dict as IDictionary<string, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            var d = dict as IDictionary<string, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            var d = dict as IDictionary<string, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            var d = dict as IDictionary<string, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            var d = dict as IDictionary<string, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            var d = dict as IDictionary<string, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            var d = dict as IDictionary<string, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            var d = dict as IDictionary<string, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            var d = dict as IDictionary<string, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            var d = dict as IDictionary<string, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        var d = dict as IDictionary<string, Guid>;
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        var d = dict as IDictionary<string, DateTimeOffset>;
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        var d = dict as IDictionary<string, Version>;
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        var d = dict as IDictionary<string, Uri>;
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        var d = dict as IDictionary<string, Vector2>;
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        var d = dict as IDictionary<string, Vector3>;
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        var d = dict as IDictionary<string, Vector4>;
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        var d = dict as IDictionary<string, Complex>;
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        var d = dict as IDictionary<string, Quaternion>;
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        var d = dict as IDictionary<string, Plane>;
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        var d = dict as IDictionary<string, Matrix3x2>;
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        var d = dict as IDictionary<string, Matrix4x4>;
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        var d = dict as IDictionary<string, BigInteger>;
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        var d = dict as IDictionary<string, byte[]?>;
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        var d = dict as IDictionary<string, List<string>?>;
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = keyEntry.CleanName;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillTimeOnlyKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeOnly.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeOnly, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillTimeSpanKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeSpan.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<TimeSpan, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillTimeZoneInfoKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = TimeZoneInfo.FindSystemTimeZoneById(keyEntry.CleanName);
                            var d = dict as IDictionary<TimeZoneInfo, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Fills Type-key dictionaries with specialized value readers while retaining encoded entry paths.<br/>
        /// The supplied header must be the dictionary parent, not its first entry.<br/>
        /// Missing entries and unresolved Type keys fail before value reading or insertion; excess entries also fail.<br/>
        /// Existing entries and the destination comparer are retained; filling is not transactional on later failure.<br/>
        /// </summary>
        /// <param name="dict">Existing dictionary receiving additions.<br/></param>
        /// <param name="dCount">Encoded number of dictionary entries.<br/></param>
        /// <param name="dictEntry">Borrowed parent header for the dictionary.<br/></param>
        /// <param name="propPath">Existing dictionary path retained for caller compatibility.<br/></param>
        /// <param name="vType">Value family selecting the specialized reader.<br/></param>
        /// <param name="dzOptions">Optional activation and codec policy for complex values.<br/></param>
        private void fillTypeKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, keyEntry.PropPath, dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        var d = dict as IDictionary<Type, string>;
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            d!.Add(keyStr, ReadStringCore(keyEntry.PropPath)!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        var d = dict as IDictionary<Type, bool>;
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            d!.Add(keyStr, ReadBooleanCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        var d = dict as IDictionary<Type, sbyte>;
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            d!.Add(keyStr, ReadSByteCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        var d = dict as IDictionary<Type, byte>;
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            d!.Add(keyStr, ReadByteCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, char>;
                            d!.Add(keyStr, ReadCharCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, short>;
                            d!.Add(keyStr, ReadInt16Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, Half>;
                            d!.Add(keyStr, ReadHalfCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, Rune>;
                            d!.Add(keyStr, ReadRuneCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, int>;
                            d!.Add(keyStr, ReadInt32Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, uint>;
                            d!.Add(keyStr, ReadUInt32Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, float>;
                            d!.Add(keyStr, ReadSingleCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, long>;
                            d!.Add(keyStr, ReadInt64Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, double>;
                            d!.Add(keyStr, ReadDoubleCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, Int128>;
                            d!.Add(keyStr, ReadInt128Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, Guid>;
                            d!.Add(keyStr, ReadGuidCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, Version>;
                            d!.Add(keyStr, ReadVersionCore(keyEntry.PropPath)!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, Uri>;
                            d!.Add(keyStr, ReadUriCore(keyEntry.PropPath)!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, Complex>;
                            d!.Add(keyStr, ReadComplexCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(keyEntry.PropPath) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            if (keyEntry.IsEmpty) throw new InvalidDataException("Dictionary entry count exceeds its encoded entries.");
                            var keyStr = Type.GetType(keyEntry.CleanName)
                                ?? throw new InvalidDataException("The dictionary Type key could not be resolved.");
                            var d = dict as IDictionary<Type, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(keyEntry.PropPath));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
            if (!keyEntry.IsEmpty) throw new InvalidDataException("Dictionary has more encoded entries than its declared count.");
        }

        private void fillUInt128KeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = UInt128.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<UInt128, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillUInt16KeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ushort.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ushort, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillUInt32KeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = uint.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<uint, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillUInt64KeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = ulong.Parse(keyEntry.CleanName, CultureInfo.InvariantCulture);
                            var d = dict as IDictionary<ulong, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillMailAddressKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new MailAddress(keyEntry.CleanName);
                            var d = dict as IDictionary<MailAddress, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }
        private void fillUriKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = new Uri(keyEntry.CleanName);
                            var d = dict as IDictionary<Uri, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }

        private void fillVersionOffsetKeyDictionary(IDictionary dict, uint dCount, NameHeaderEntry dictEntry, string propPath, InhetoBaseTypes vType, DeserializationOptions? dzOptions = null)
        {
            var keyEntry = dictEntry.FirstChild(NameHeaderEntry.EntryTypes.KeyString);
            switch (vType)
            {
                case InhetoBaseTypes.ComplexType:
                    {
                        var dTypes = UtilsAndExtensions.GetDictTypes(dict.GetType());
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary;
                            d!.Add(keyStr, ReadMaterializedAs(dTypes.ValueType, string.Concat(propPath, keyEntry.Name), dzOptions));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.String:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, string>;
                            d!.Add(keyStr, ReadStringCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Boolean:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, bool>;
                            d!.Add(keyStr, ReadBooleanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.SByte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, sbyte>;
                            d!.Add(keyStr, ReadSByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Byte:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, byte>;
                            d!.Add(keyStr, ReadByteCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Char:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, char>;
                            d!.Add(keyStr, ReadCharCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, short>;
                            d!.Add(keyStr, ReadInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt16:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, ushort>;
                            d!.Add(keyStr, ReadUInt16Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Half:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, Half>;
                            d!.Add(keyStr, ReadHalfCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Rune:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, Rune>;
                            d!.Add(keyStr, ReadRuneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, int>;
                            d!.Add(keyStr, ReadInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt32:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, uint>;
                            d!.Add(keyStr, ReadUInt32Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Single:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, float>;
                            d!.Add(keyStr, ReadSingleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, long>;
                            d!.Add(keyStr, ReadInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt64:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, ulong>;
                            d!.Add(keyStr, ReadUInt64Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Double:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, double>;
                            d!.Add(keyStr, ReadDoubleCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Int128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, Int128>;
                            d!.Add(keyStr, ReadInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.UInt128:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, UInt128>;
                            d!.Add(keyStr, ReadUInt128Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Decimal:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, decimal>;
                            d!.Add(keyStr, ReadDecimalCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, DateOnly>;
                            d!.Add(keyStr, ReadDateOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTime:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, DateTime>;
                            d!.Add(keyStr, ReadDateTimeCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeOnly:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, TimeOnly>;
                            d!.Add(keyStr, ReadTimeOnlyCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.TimeSpan:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, TimeSpan>;
                            d!.Add(keyStr, ReadTimeSpanCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Guid:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, Guid>;
                            d!.Add(keyStr, ReadGuidCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.DateTimeOffset:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, DateTimeOffset>;
                            d!.Add(keyStr, ReadDateTimeOffsetCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Version:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, Version>;
                            d!.Add(keyStr, ReadVersionCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Uri:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, Uri>;
                            d!.Add(keyStr, ReadUriCore(string.Concat(propPath, keyEntry.Name))!);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, Vector2>;
                            d!.Add(keyStr, ReadVector2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector3:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, Vector3>;
                            d!.Add(keyStr, ReadVector3Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Vector4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, Vector4>;
                            d!.Add(keyStr, ReadVector4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ComplexNumeric:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, Complex>;
                            d!.Add(keyStr, ReadComplexCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Quaternion:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, Quaternion>;
                            d!.Add(keyStr, ReadQuaternionCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Plane:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, Plane>;
                            d!.Add(keyStr, ReadPlaneCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix3x2:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, Matrix3x2>;
                            d!.Add(keyStr, ReadMatrix3x2Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.Matrix4x4:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, Matrix4x4>;
                            d!.Add(keyStr, ReadMatrix4x4Core(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.BigInteger:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, BigInteger>;
                            d!.Add(keyStr, ReadBigIntegerCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ByteArray:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, byte[]?>;
                            d!.Add(keyStr, ReadByteArrayCore(string.Concat(propPath, keyEntry.Name)) ?? default);
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
                case InhetoBaseTypes.ListString:
                    {
                        for (int i = 0; i < dCount; i++)
                        {
                            var keyStr = Version.Parse(keyEntry.CleanName);
                            var d = dict as IDictionary<Version, List<string>?>;
                            d!.Add(keyStr, ReadStringListCore(string.Concat(propPath, keyEntry.Name)));
                            keyEntry = keyEntry.NextSibling(NameHeaderEntry.EntryTypes.KeyString);
                        }
                    }
                    break;
            }
        }








    }
}
