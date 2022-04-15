using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    /// <summary>
    /// Comparer for <see cref="JsonValue"/>.
    /// </summary>
    public static class JsonValueComparer
    {
        /// <summary>
        /// Compares two <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="x">The left value.</param>
        /// <param name="y">The right value.</param>
        public static int Compare(JsonValue? x, JsonValue? y)
            => Compare(x, y, true);
        
        internal static int Compare(JsonValue? x, JsonValue? y, bool throwWhenNotComparable)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is null)
            {
                return -1;
            }

            if (y is null)
            {
                return 1;
            }

            if (x.TryGetValue<int>(out var intVal))
            {
                return CompareNumber(intVal, y, throwWhenNotComparable);
            }

            if (x.TryGetValue<long>(out var longVal))
            {
                return CompareNumber(longVal, y, throwWhenNotComparable);
            }
            
            if (x.TryGetValue<decimal>(out var decimalVal))
            {
                return CompareNumber(decimalVal, y, throwWhenNotComparable);
            }

            if (x.TryGetValue<double>(out var doubleVal))
            {
                return CompareNumber(doubleVal, y, throwWhenNotComparable);
            }
            
            if (x.TryGetValue<string>(out var stringVal))
            {
                return CompareString(stringVal, y, throwWhenNotComparable);
            }
            
            if (x.TryGetValue<DateTimeOffset>(out var dateTimeOffsetVal))
            {
                return CompareDateTime(dateTimeOffsetVal, y, throwWhenNotComparable);
            }
            
            if (x.TryGetValue<DateTime>(out var dateTimeVal))
            {
                return CompareDateTime(dateTimeVal, y, throwWhenNotComparable);
            }
            
            if (x.TryGetValue<bool>(out var booleanVal))
            {
                return CompareBoolean(booleanVal, y, throwWhenNotComparable);
            }

            if (x.TryGetValue<short>(out var shortVal))
            {
                return CompareNumber(shortVal, y, throwWhenNotComparable);
            }

            if (x.TryGetValue<byte>(out var byteVal))
            {
                return CompareNumber(byteVal, y, throwWhenNotComparable);
            }

            if (x.TryGetValue<float>(out var floatVal))
            {
                return CompareNumber(floatVal, y, throwWhenNotComparable);
            }

            if (x.TryGetValue<uint>(out var uintVal))
            {
                return CompareNumber(uintVal, y, throwWhenNotComparable);
            }

            if (x.TryGetValue<ushort>(out var ushortVal))
            {
                return CompareNumber(ushortVal, y, throwWhenNotComparable);
            }

            if (x.TryGetValue<ulong>(out var ulongVal))
            {
                return CompareNumber(ulongVal, y, throwWhenNotComparable);
            }

            if (x.TryGetValue<sbyte>(out var sbyteVal))
            {
                return CompareNumber(sbyteVal, y, throwWhenNotComparable);
            }
            
            if (x.TryGetValue<Guid>(out var guidVal))
            {
                return CompareGuid(guidVal, y, throwWhenNotComparable);
            }
            
            if (x.TryGetValue<char>(out var charVal))
            {
                return CompareChar(charVal, y, throwWhenNotComparable);
            }

            if (x.TryGetValue<byte[]>(out var byteArrayVal) ||
                x.TryGetValue<JsonElement>(out var elementVal) && elementVal.TryGetBytesFromBase64(out byteArrayVal))
            {
                return CompareByteArray(byteArrayVal, y, throwWhenNotComparable);
            }

            if (throwWhenNotComparable)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "JSON value is not comparable.");
            }

            return -1;
        }

        private static int CompareNumber(int x, JsonValue y, bool throwWhenNotComparable)
        {
            if (y.TryGetValue<int>(out var intVal))
            {
                return x.CompareTo(intVal);
            }

            if (y.TryGetValue<long>(out var longVal))
            {
                return Convert.ToInt64(x).CompareTo(longVal);
            }

            if (y.TryGetValue<double>(out var doubleVal))
            {
                return CompareDouble(Convert.ToDouble(x), doubleVal);
            }

            if (y.TryGetValue<short>(out var shortVal))
            {
                return x.CompareTo(Convert.ToInt32(shortVal));
            }

            if (y.TryGetValue<decimal>(out var decimalVal))
            {
                return Convert.ToDecimal(x).CompareTo(decimalVal);
            }

            if (y.TryGetValue<byte>(out var byteVal))
            {
                return x.CompareTo(Convert.ToInt32(byteVal));
            }

            if (y.TryGetValue<float>(out var floatVal))
            {
                return Convert.ToSingle(x).CompareTo(floatVal);
            }

            if (y.TryGetValue<uint>(out var uintVal))
            {
                return Convert.ToInt64(x).CompareTo(Convert.ToInt64(uintVal));
            }

            if (y.TryGetValue<ushort>(out var ushortVal))
            {
                return x.CompareTo(Convert.ToInt32(ushortVal));
            }

            if (y.TryGetValue<ulong>(out var ulongVal))
            {
                return Convert.ToDecimal(x).CompareTo(Convert.ToDecimal(ulongVal));
            }

            if (y.TryGetValue<sbyte>(out var sbyteVal))
            {
                return x.CompareTo(Convert.ToInt32(sbyteVal));
            }

            if (throwWhenNotComparable)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "JsonValue was not a number.");
            }

            return -1;
        }

        private static int CompareNumber(long x, JsonValue y, bool throwWhenNotComparable)
        {
            if (y.TryGetValue<int>(out var intVal))
            {
                return x.CompareTo(Convert.ToInt64(intVal));
            }

            if (y.TryGetValue<long>(out var longVal))
            {
                return x.CompareTo(longVal);
            }

            if (y.TryGetValue<double>(out var doubleVal))
            {
                return CompareDouble(Convert.ToDouble(x), doubleVal);
            }

            if (y.TryGetValue<short>(out var shortVal))
            {
                return x.CompareTo(Convert.ToInt64(shortVal));
            }

            if (y.TryGetValue<decimal>(out var decimalVal))
            {
                return Convert.ToDecimal(x).CompareTo(decimalVal);
            }

            if (y.TryGetValue<byte>(out var byteVal))
            {
                return x.CompareTo(Convert.ToInt64(byteVal));
            }

            if (y.TryGetValue<float>(out var floatVal))
            {
                return Convert.ToSingle(x).CompareTo(floatVal);
            }

            if (y.TryGetValue<uint>(out var uintVal))
            {
                return x.CompareTo(Convert.ToInt64(uintVal));
            }

            if (y.TryGetValue<ushort>(out var ushortVal))
            {
                return x.CompareTo(Convert.ToInt64(ushortVal));
            }

            if (y.TryGetValue<ulong>(out var ulongVal))
            {
                return Convert.ToDecimal(x).CompareTo(Convert.ToDecimal(ulongVal));
            }

            if (y.TryGetValue<sbyte>(out var sbyteVal))
            {
                return x.CompareTo(Convert.ToInt64(sbyteVal));
            }

            if (throwWhenNotComparable)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "JsonValue was not a number.");
            }

            return -1;
        }

        private static int CompareNumber(short x, JsonValue y, bool throwWhenNotComparable)
        {
            if (y.TryGetValue<int>(out var intVal))
            {
                return Convert.ToInt32(x).CompareTo(intVal);
            }

            if (y.TryGetValue<long>(out var longVal))
            {
                return Convert.ToInt64(x).CompareTo(longVal);
            }

            if (y.TryGetValue<double>(out var doubleVal))
            {
                return CompareDouble(Convert.ToDouble(x), doubleVal);
            }

            if (y.TryGetValue<short>(out var shortVal))
            {
                return x.CompareTo(shortVal);
            }

            if (y.TryGetValue<decimal>(out var decimalVal))
            {
                return Convert.ToDecimal(x).CompareTo(decimalVal);
            }

            if (y.TryGetValue<byte>(out var byteVal))
            {
                return x.CompareTo(Convert.ToInt16(byteVal));
            }

            if (y.TryGetValue<float>(out var floatVal))
            {
                return Convert.ToSingle(x).CompareTo(floatVal);
            }

            if (y.TryGetValue<uint>(out var uintVal))
            {
                return Convert.ToInt64(x).CompareTo(Convert.ToInt64(uintVal));
            }

            if (y.TryGetValue<ushort>(out var ushortVal))
            {
                return Convert.ToInt32(x).CompareTo(Convert.ToInt32(ushortVal));
            }

            if (y.TryGetValue<ulong>(out var ulongVal))
            {
                return Convert.ToDecimal(x).CompareTo(Convert.ToDecimal(ulongVal));
            }

            if (y.TryGetValue<sbyte>(out var sbyteVal))
            {
                return x.CompareTo(Convert.ToInt16(sbyteVal));
            }

            if (throwWhenNotComparable)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "JsonValue was not a number.");
            }

            return -1;
        }
        
        private static int CompareNumber(byte x, JsonValue y, bool throwWhenNotComparable)
        {
            if (y.TryGetValue<int>(out var intVal))
            {
                return Convert.ToInt32(x).CompareTo(intVal);
            }

            if (y.TryGetValue<long>(out var longVal))
            {
                return Convert.ToInt64(x).CompareTo(longVal);
            }

            if (y.TryGetValue<double>(out var doubleVal))
            {
                return CompareDouble(Convert.ToDouble(x), doubleVal);
            }

            if (y.TryGetValue<short>(out var shortVal))
            {
                return Convert.ToInt16(x).CompareTo(shortVal);
            }

            if (y.TryGetValue<decimal>(out var decimalVal))
            {
                return Convert.ToDecimal(x).CompareTo(decimalVal);
            }

            if (y.TryGetValue<byte>(out var byteVal))
            {
                return x.CompareTo(byteVal);
            }

            if (y.TryGetValue<float>(out var floatVal))
            {
                return Convert.ToSingle(x).CompareTo(floatVal);
            }

            if (y.TryGetValue<uint>(out var uintVal))
            {
                return Convert.ToUInt32(x).CompareTo(uintVal);
            }

            if (y.TryGetValue<ushort>(out var ushortVal))
            {
                return Convert.ToUInt16(x).CompareTo(ushortVal);
            }

            if (y.TryGetValue<ulong>(out var ulongVal))
            {
                return Convert.ToUInt64(x).CompareTo(ulongVal);
            }

            if (y.TryGetValue<sbyte>(out var sbyteVal))
            {
                return Convert.ToInt16(x).CompareTo(Convert.ToInt16(sbyteVal));
            }

            if (throwWhenNotComparable)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "JsonValue was not a number.");
            }

            return -1;
        }
        
        private static int CompareNumber(uint x, JsonValue y, bool throwWhenNotComparable)
        {
            if (y.TryGetValue<int>(out var intVal))
            {
                return Convert.ToInt64(x).CompareTo(Convert.ToInt64(intVal));
            }

            if (y.TryGetValue<long>(out var longVal))
            {
                return Convert.ToInt64(x).CompareTo(longVal);
            }

            if (y.TryGetValue<double>(out var doubleVal))
            {
                return CompareDouble(Convert.ToDouble(x), doubleVal);
            }

            if (y.TryGetValue<short>(out var shortVal))
            {
                return Convert.ToInt64(x).CompareTo(Convert.ToInt64(shortVal));
            }

            if (y.TryGetValue<decimal>(out var decimalVal))
            {
                return Convert.ToDecimal(x).CompareTo(decimalVal);
            }

            if (y.TryGetValue<byte>(out var byteVal))
            {
                return x.CompareTo(Convert.ToUInt32(byteVal));
            }

            if (y.TryGetValue<float>(out var floatVal))
            {
                return Convert.ToSingle(x).CompareTo(floatVal);
            }

            if (y.TryGetValue<uint>(out var uintVal))
            {
                return x.CompareTo(uintVal);
            }

            if (y.TryGetValue<ushort>(out var ushortVal))
            {
                return x.CompareTo(Convert.ToUInt32(ushortVal));
            }

            if (y.TryGetValue<ulong>(out var ulongVal))
            {
                return Convert.ToUInt64(x).CompareTo(ulongVal);
            }

            if (y.TryGetValue<sbyte>(out var sbyteVal))
            {
                return Convert.ToInt64(x).CompareTo(Convert.ToInt64(sbyteVal));
            }

            if (throwWhenNotComparable)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "JsonValue was not a number.");
            }

            return -1;
        }
        
        private static int CompareNumber(ushort x, JsonValue y, bool throwWhenNotComparable)
        {
            if (y.TryGetValue<int>(out var intVal))
            {
                return Convert.ToInt32(x).CompareTo(intVal);
            }

            if (y.TryGetValue<long>(out var longVal))
            {
                return Convert.ToInt64(x).CompareTo(longVal);
            }

            if (y.TryGetValue<double>(out var doubleVal))
            {
                return CompareDouble(Convert.ToDouble(x), doubleVal);
            }

            if (y.TryGetValue<short>(out var shortVal))
            {
                return Convert.ToInt32(x).CompareTo(Convert.ToInt32(shortVal));
            }

            if (y.TryGetValue<decimal>(out var decimalVal))
            {
                return Convert.ToDecimal(x).CompareTo(decimalVal);
            }

            if (y.TryGetValue<byte>(out var byteVal))
            {
                return x.CompareTo(Convert.ToUInt16(byteVal));
            }

            if (y.TryGetValue<float>(out var floatVal))
            {
                return Convert.ToSingle(x).CompareTo(floatVal);
            }

            if (y.TryGetValue<uint>(out var uintVal))
            {
                return Convert.ToUInt32(x).CompareTo(uintVal);
            }

            if (y.TryGetValue<ushort>(out var ushortVal))
            {
                return x.CompareTo(ushortVal);
            }

            if (y.TryGetValue<ulong>(out var ulongVal))
            {
                return Convert.ToUInt64(x).CompareTo(ulongVal);
            }

            if (y.TryGetValue<sbyte>(out var sbyteVal))
            {
                return Convert.ToInt32(x).CompareTo(Convert.ToInt32(sbyteVal));
            }

            if (throwWhenNotComparable)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "JsonValue was not a number.");
            }

            return -1;
        }
        
        private static int CompareNumber(ulong x, JsonValue y, bool throwWhenNotComparable)
        {
            if (y.TryGetValue<int>(out var intVal))
            {
                return Convert.ToDecimal(x).CompareTo(Convert.ToDecimal(intVal));
            }

            if (y.TryGetValue<long>(out var longVal))
            {
                return Convert.ToDecimal(x).CompareTo(Convert.ToDecimal(longVal));
            }

            if (y.TryGetValue<double>(out var doubleVal))
            {
                return CompareDouble(Convert.ToDouble(x), doubleVal);
            }

            if (y.TryGetValue<short>(out var shortVal))
            {
                return Convert.ToDecimal(x).CompareTo(Convert.ToDecimal(shortVal));
            }

            if (y.TryGetValue<decimal>(out var decimalVal))
            {
                return Convert.ToDecimal(x).CompareTo(decimalVal);
            }

            if (y.TryGetValue<byte>(out var byteVal))
            {
                return x.CompareTo(Convert.ToUInt64(byteVal));
            }

            if (y.TryGetValue<float>(out var floatVal))
            {
                return Convert.ToSingle(x).CompareTo(floatVal);
            }

            if (y.TryGetValue<uint>(out var uintVal))
            {
                return x.CompareTo(Convert.ToUInt64(uintVal));
            }

            if (y.TryGetValue<ushort>(out var ushortVal))
            {
                return x.CompareTo(Convert.ToUInt64(ushortVal));
            }

            if (y.TryGetValue<ulong>(out var ulongVal))
            {
                return x.CompareTo(ulongVal);
            }

            if (y.TryGetValue<sbyte>(out var sbyteVal))
            {
                return Convert.ToDecimal(x).CompareTo(Convert.ToDecimal(sbyteVal));
            }

            if (throwWhenNotComparable)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "JsonValue was not a number.");
            }

            return -1;
        }
        
        private static int CompareNumber(sbyte x, JsonValue y, bool throwWhenNotComparable)
        {
            if (y.TryGetValue<int>(out var intVal))
            {
                return Convert.ToInt32(x).CompareTo(intVal);
            }

            if (y.TryGetValue<long>(out var longVal))
            {
                return Convert.ToInt64(x).CompareTo(longVal);
            }

            if (y.TryGetValue<double>(out var doubleVal))
            {
                return CompareDouble(Convert.ToDouble(x), doubleVal);
            }

            if (y.TryGetValue<short>(out var shortVal))
            {
                return Convert.ToInt16(x).CompareTo(shortVal);
            }

            if (y.TryGetValue<decimal>(out var decimalVal))
            {
                return Convert.ToDecimal(x).CompareTo(decimalVal);
            }

            if (y.TryGetValue<byte>(out var byteVal))
            {
                return Convert.ToInt16(x).CompareTo(Convert.ToInt16(byteVal));
            }

            if (y.TryGetValue<float>(out var floatVal))
            {
                return Convert.ToSingle(x).CompareTo(floatVal);
            }

            if (y.TryGetValue<uint>(out var uintVal))
            {
                return Convert.ToInt64(x).CompareTo(Convert.ToInt64(uintVal));
            }

            if (y.TryGetValue<ushort>(out var ushortVal))
            {
                return Convert.ToInt32(x).CompareTo(Convert.ToInt32(ushortVal));
            }

            if (y.TryGetValue<ulong>(out var ulongVal))
            {
                return Convert.ToDecimal(x).CompareTo(Convert.ToDecimal(ulongVal));
            }

            if (y.TryGetValue<sbyte>(out var sbyteVal))
            {
                return x.CompareTo(sbyteVal);
            }

            if (throwWhenNotComparable)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "JsonValue was not a number.");
            }

            return -1;
        }
        
        private static int CompareNumber(double x, JsonValue y, bool throwWhenNotComparable)
        {
            if (y.TryGetValue<int>(out var intVal))
            {
                return CompareDouble(x, Convert.ToDouble(intVal));
            }

            if (y.TryGetValue<long>(out var longVal))
            {
                return CompareDouble(x, Convert.ToDouble(longVal));
            }

            if (y.TryGetValue<double>(out var doubleVal))
            {
                return CompareDouble(x, doubleVal);
            }

            if (y.TryGetValue<short>(out var shortVal))
            {
                return CompareDouble(x, Convert.ToDouble(shortVal));
            }

            if (y.TryGetValue<decimal>(out var decimalVal))
            {
                return Convert.ToDecimal(x).CompareTo(decimalVal);
            }

            if (y.TryGetValue<byte>(out var byteVal))
            {
                return CompareDouble(x, Convert.ToDouble(byteVal));
            }

            if (y.TryGetValue<float>(out var floatVal))
            {
                return CompareDouble(x, Convert.ToDouble(floatVal));
            }

            if (y.TryGetValue<uint>(out var uintVal))
            {
                return CompareDouble(x, Convert.ToDouble(uintVal));
            }

            if (y.TryGetValue<ushort>(out var ushortVal))
            {
                return CompareDouble(x, Convert.ToDouble(ushortVal));
            }

            if (y.TryGetValue<ulong>(out var ulongVal))
            {
                return CompareDouble(x, Convert.ToDouble(ulongVal));
            }

            if (y.TryGetValue<sbyte>(out var sbyteVal))
            {
                return CompareDouble(x, Convert.ToDouble(sbyteVal));
            }

            if (throwWhenNotComparable)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "JsonValue was not a number.");
            }

            return -1;
        }
        
        private static int CompareNumber(decimal x, JsonValue y, bool throwWhenNotComparable)
        {
            if (y.TryGetValue<int>(out var intVal))
            {
                return x.CompareTo(Convert.ToDecimal(intVal));
            }

            if (y.TryGetValue<long>(out var longVal))
            {
                return x.CompareTo(Convert.ToDecimal(longVal));
            }

            if (y.TryGetValue<double>(out var doubleVal))
            {
                return x.CompareTo(Convert.ToDecimal(doubleVal));
            }

            if (y.TryGetValue<short>(out var shortVal))
            {
                return x.CompareTo(Convert.ToDecimal(shortVal));
            }

            if (y.TryGetValue<decimal>(out var decimalVal))
            {
                return x.CompareTo(decimalVal);
            }

            if (y.TryGetValue<byte>(out var byteVal))
            {
                return x.CompareTo(Convert.ToDecimal(byteVal));
            }

            if (y.TryGetValue<float>(out var floatVal))
            {
                return x.CompareTo(Convert.ToDecimal(floatVal));
            }

            if (y.TryGetValue<uint>(out var uintVal))
            {
                return x.CompareTo(Convert.ToDecimal(uintVal));
            }

            if (y.TryGetValue<ushort>(out var ushortVal))
            {
                return x.CompareTo(Convert.ToDecimal(ushortVal));
            }

            if (y.TryGetValue<ulong>(out var ulongVal))
            {
                return x.CompareTo(Convert.ToDecimal(ulongVal));
            }

            if (y.TryGetValue<sbyte>(out var sbyteVal))
            {
                return x.CompareTo(Convert.ToDecimal(sbyteVal));
            }

            if (throwWhenNotComparable)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "JsonValue was not a number.");
            }

            return -1;
        }
        
        private static int CompareNumber(float x, JsonValue y, bool throwWhenNotComparable)
        {
            if (y.TryGetValue<int>(out var intVal))
            {
                return x.CompareTo(Convert.ToSingle(intVal));
            }

            if (y.TryGetValue<long>(out var longVal))
            {
                return x.CompareTo(Convert.ToSingle(longVal));
            }

            if (y.TryGetValue<double>(out var doubleVal))
            {
                return CompareDouble(Convert.ToDouble(x), doubleVal);
            }

            if (y.TryGetValue<short>(out var shortVal))
            {
                return x.CompareTo(Convert.ToSingle(shortVal));
            }

            if (y.TryGetValue<decimal>(out var decimalVal))
            {
                return Convert.ToDecimal(x).CompareTo(decimalVal);
            }

            if (y.TryGetValue<byte>(out var byteVal))
            {
                return x.CompareTo(Convert.ToSingle(byteVal));
            }

            if (y.TryGetValue<float>(out var floatVal))
            {
                return x.CompareTo(Convert.ToSingle(floatVal));
            }

            if (y.TryGetValue<uint>(out var uintVal))
            {
                return x.CompareTo(Convert.ToSingle(uintVal));
            }

            if (y.TryGetValue<ushort>(out var ushortVal))
            {
                return x.CompareTo(Convert.ToSingle(ushortVal));
            }

            if (y.TryGetValue<ulong>(out var ulongVal))
            {
                return x.CompareTo(Convert.ToSingle(ulongVal));
            }

            if (y.TryGetValue<sbyte>(out var sbyteVal))
            {
                return x.CompareTo(Convert.ToSingle(sbyteVal));
            }

            if (throwWhenNotComparable)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "JsonValue was not a number.");
            }

            return -1;
        }

        private static bool AreDoubleClose(double x, double y)
        {
            // https://stackoverflow.com/questions/3874627/floating-point-comparison-functions-for-c-sharp
            const double epsilon = 2.22044604925031E-16;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (x == y)
            {
                return true;
            }

            var tolerance = (Math.Abs(x) + Math.Abs(y) + 10.0) * epsilon;
            var difference = x - y;
            
            if (-tolerance < difference)
            {
                return tolerance > difference;
            }

            return false;
        }

        private static int CompareDouble(double x, double y)
        {
            return AreDoubleClose(x, y) ? 0 : x.CompareTo(y);
        }

        private static int CompareString(string x, JsonValue y, bool throwWhenNotComparable)
        {
            if (y.TryGetValue<string>(out var stringVal))
            {
                return StringComparer.Ordinal.Compare(x, stringVal);
            }

            if (throwWhenNotComparable)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "JsonValue was not a string.");
            }

            return -1;
        }
        
        private static int CompareDateTime(DateTime x, JsonValue y, bool throwWhenNotComparable)
        {
            if (y.TryGetValue<DateTime>(out var dateTimeVal))
            {
                return x.CompareTo(dateTimeVal);
            }
            
            if (y.TryGetValue<DateTimeOffset>(out var dateTimeOffsetVal))
            {
                return new DateTimeOffset(x).CompareTo(dateTimeOffsetVal);
            }

            if (throwWhenNotComparable)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "JsonValue was not a datetime.");
            }

            return -1;
        }
        
        private static int CompareDateTime(DateTimeOffset x, JsonValue y, bool throwWhenNotComparable)
        {
            if (y.TryGetValue<DateTimeOffset>(out var dateTimeOffsetVal))
            {
                return x.CompareTo(dateTimeOffsetVal);
            }
            
            if (y.TryGetValue<DateTime>(out var dateTimeVal))
            {
                return x.CompareTo(new DateTimeOffset(dateTimeVal));
            }

            if (throwWhenNotComparable)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "JsonValue was not a datetime.");
            }

            return -1;
        }
        
        private static int CompareGuid(Guid x, JsonValue y, bool throwWhenNotComparable)
        {
            if (y.TryGetValue<Guid>(out var guidVal))
            {
                return x.CompareTo(guidVal);
            }

            if (throwWhenNotComparable)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "JsonValue was not a GUID.");
            }

            return -1;
        }
        
        private static int CompareChar(char x, JsonValue y, bool throwWhenNotComparable)
        {
            if (y.TryGetValue<char>(out var charVal))
            {
                return x.CompareTo(charVal);
            }

            if (throwWhenNotComparable)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "JsonValue was not a character.");
            }

            return -1;
        }
        
        private static int CompareByteArray(byte[] x, JsonValue y, bool throwWhenNotComparable)
        {
            if (y.TryGetValue<byte[]>(out var byteArrayVal))
            {
                return CompareByteArray(x, byteArrayVal);
            }

            if (y.TryGetValue<JsonElement>(out var jsonElement) && jsonElement.TryGetBytesFromBase64(out byteArrayVal))
            {
                return CompareByteArray(x, byteArrayVal);
            }

            if (throwWhenNotComparable)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "JsonValue was not a byte array.");
            }

            return -1;
        }

        private static int CompareByteArray(byte[] x, byte[] y)
        {
            if (x.Length == 0 && y.Length == 0)
            {
                return 0;
            }

            var lengthCompare = x.Length.CompareTo(y.Length);
            if (lengthCompare != 0)
            {
                return lengthCompare;
            }

            for (var i = 0; i < x.Length; i++)
            {
                var valueCompare = x[i].CompareTo(y[i]);
                if (valueCompare != 0)
                {
                    return valueCompare;
                }
            }

            return 0;
        }

        private static int CompareBoolean(bool x, JsonValue y, bool throwWhenNotComparable)
        {
            if (y.TryGetValue<bool>(out var boolVal))
            {
                return x.CompareTo(boolVal);
            }

            if (throwWhenNotComparable)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "JsonValue was not a boolean.");
            }

            return -1;
        }
    }
}

