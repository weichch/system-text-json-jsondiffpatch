using System.Diagnostics;
using System.Globalization;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    /// <summary>
    /// Comparer for <see cref="JsonValue"/>.
    /// </summary>
    static partial class JsonValueComparer
    {
        private static int CompareNumber(int x, JsonValue y, Type typeY)
        {
            if (typeY == typeof(int)) 
                return x.CompareTo(y.GetValue<int>());
            if (typeY == typeof(long)) 
                return Convert.ToInt64(x).CompareTo(y.GetValue<long>());
            if (typeY == typeof(double)) 
                return CompareDouble(Convert.ToDouble(x), y.GetValue<double>());
            if (typeY == typeof(short)) 
                return x.CompareTo(Convert.ToInt32(y.GetValue<short>()));
            if (typeY == typeof(decimal)) 
                return Convert.ToDecimal(x).CompareTo(y.GetValue<decimal>());
            if (typeY == typeof(byte)) 
                return x.CompareTo(Convert.ToInt32(y.GetValue<byte>()));
            if (typeY == typeof(float)) 
                return Convert.ToSingle(x).CompareTo(y.GetValue<float>());
            if (typeY == typeof(uint)) 
                return Convert.ToInt64(x).CompareTo(Convert.ToInt64(y.GetValue<uint>()));
            if (typeY == typeof(ushort)) 
                return x.CompareTo(Convert.ToInt32(y.GetValue<ushort>()));
            if (typeY == typeof(ulong)) 
                return Convert.ToDecimal(x).CompareTo(Convert.ToDecimal(y.GetValue<ulong>()));
            if (typeY == typeof(sbyte)) 
                return x.CompareTo(Convert.ToInt32(y.GetValue<sbyte>()));

            return CompareNumberWithAllocation(x, y);
        }

        private static int CompareNumber(long x, JsonValue y, Type typeY)
        {
            if (typeY == typeof(int))
                return x.CompareTo(Convert.ToInt64(y.GetValue<int>()));
            if (typeY == typeof(long))
                return x.CompareTo(y.GetValue<long>());
            if (typeY == typeof(double))
                return CompareDouble(Convert.ToDouble(x), y.GetValue<double>());
            if (typeY == typeof(short))
                return x.CompareTo(Convert.ToInt64(y.GetValue<short>()));
            if (typeY == typeof(decimal))
                return Convert.ToDecimal(x).CompareTo(y.GetValue<decimal>());
            if (typeY == typeof(byte))
                return x.CompareTo(Convert.ToInt64(y.GetValue<byte>()));
            if (typeY == typeof(float))
                return Convert.ToSingle(x).CompareTo(y.GetValue<float>());
            if (typeY == typeof(uint))
                return x.CompareTo(Convert.ToInt64(y.GetValue<uint>()));
            if (typeY == typeof(ushort))
                return x.CompareTo(Convert.ToInt64(y.GetValue<ushort>()));
            if (typeY == typeof(ulong))
                return Convert.ToDecimal(x).CompareTo(Convert.ToDecimal(y.GetValue<ulong>()));
            if (typeY == typeof(sbyte))
                return x.CompareTo(Convert.ToInt64(y.GetValue<sbyte>()));

            return CompareNumberWithAllocation(x, y);
        }

        private static int CompareNumber(short x, JsonValue y, Type typeY)
        {
            if (typeY == typeof(int))
                return Convert.ToInt32(x).CompareTo(y.GetValue<int>());
            if (typeY == typeof(long))
                return Convert.ToInt64(x).CompareTo(y.GetValue<long>());
            if (typeY == typeof(double))
                return CompareDouble(Convert.ToDouble(x), y.GetValue<double>());
            if (typeY == typeof(short))
                return x.CompareTo(y.GetValue<short>());
            if (typeY == typeof(decimal))
                return Convert.ToDecimal(x).CompareTo(y.GetValue<decimal>());
            if (typeY == typeof(byte))
                return x.CompareTo(Convert.ToInt16(y.GetValue<byte>()));
            if (typeY == typeof(float))
                return Convert.ToSingle(x).CompareTo(y.GetValue<float>());
            if (typeY == typeof(uint))
                return Convert.ToInt64(x).CompareTo(Convert.ToInt64(y.GetValue<uint>()));
            if (typeY == typeof(ushort))
                return Convert.ToInt32(x).CompareTo(Convert.ToInt32(y.GetValue<ushort>()));
            if (typeY == typeof(ulong))
                return Convert.ToDecimal(x).CompareTo(Convert.ToDecimal(y.GetValue<ulong>()));
            if (typeY == typeof(sbyte))
                return x.CompareTo(Convert.ToInt16(y.GetValue<sbyte>()));

            return CompareNumberWithAllocation(x, y);
        }
        
        private static int CompareNumber(byte x, JsonValue y, Type typeY)
        {
            if (typeY == typeof(int))
                return Convert.ToInt32(x).CompareTo(y.GetValue<int>());
            if (typeY == typeof(long))
                return Convert.ToInt64(x).CompareTo(y.GetValue<long>());
            if (typeY == typeof(double))
                return CompareDouble(Convert.ToDouble(x), y.GetValue<double>());
            if (typeY == typeof(short))
                return Convert.ToInt16(x).CompareTo(y.GetValue<short>());
            if (typeY == typeof(decimal))
                return Convert.ToDecimal(x).CompareTo(y.GetValue<decimal>());
            if (typeY == typeof(byte))
                return x.CompareTo(y.GetValue<byte>());
            if (typeY == typeof(float))
                return Convert.ToSingle(x).CompareTo(y.GetValue<float>());
            if (typeY == typeof(uint))
                return Convert.ToUInt32(x).CompareTo(y.GetValue<uint>());
            if (typeY == typeof(ushort))
                return Convert.ToUInt16(x).CompareTo(y.GetValue<ushort>());
            if (typeY == typeof(ulong))
                return Convert.ToUInt64(x).CompareTo(y.GetValue<ulong>());
            if (typeY == typeof(sbyte))
                return Convert.ToInt16(x).CompareTo(Convert.ToInt16(y.GetValue<sbyte>()));

            return CompareNumberWithAllocation(x, y);
        }
        
        private static int CompareNumber(uint x, JsonValue y, Type typeY)
        {
            if (typeY == typeof(int))
                return Convert.ToInt64(x).CompareTo(Convert.ToInt64(y.GetValue<int>()));
            if (typeY == typeof(long))
                return Convert.ToInt64(x).CompareTo(y.GetValue<long>());
            if (typeY == typeof(double))
                return CompareDouble(Convert.ToDouble(x), y.GetValue<double>());
            if (typeY == typeof(short))
                return Convert.ToInt64(x).CompareTo(Convert.ToInt64(y.GetValue<short>()));
            if (typeY == typeof(decimal))
                return Convert.ToDecimal(x).CompareTo(y.GetValue<decimal>());
            if (typeY == typeof(byte))
                return x.CompareTo(Convert.ToUInt32(y.GetValue<byte>()));
            if (typeY == typeof(float))
                return Convert.ToSingle(x).CompareTo(y.GetValue<float>());
            if (typeY == typeof(uint))
                return x.CompareTo(y.GetValue<uint>());
            if (typeY == typeof(ushort))
                return x.CompareTo(Convert.ToUInt32(y.GetValue<ushort>()));
            if (typeY == typeof(ulong))
                return Convert.ToUInt64(x).CompareTo(y.GetValue<ulong>());
            if (typeY == typeof(sbyte))
                return Convert.ToInt64(x).CompareTo(Convert.ToInt64(y.GetValue<sbyte>()));

            return CompareNumberWithAllocation(x, y);
        }
        
        private static int CompareNumber(ushort x, JsonValue y, Type typeY)
        {
            if (typeY == typeof(int))
                return Convert.ToInt32(x).CompareTo(y.GetValue<int>());
            if (typeY == typeof(long))
                return Convert.ToInt64(x).CompareTo(y.GetValue<long>());
            if (typeY == typeof(double))
                return CompareDouble(Convert.ToDouble(x), y.GetValue<double>());
            if (typeY == typeof(short))
                return Convert.ToInt32(x).CompareTo(Convert.ToInt32(y.GetValue<short>()));
            if (typeY == typeof(decimal))
                return Convert.ToDecimal(x).CompareTo(y.GetValue<decimal>());
            if (typeY == typeof(byte))
                return x.CompareTo(Convert.ToUInt16(y.GetValue<byte>()));
            if (typeY == typeof(float))
                return Convert.ToSingle(x).CompareTo(y.GetValue<float>());
            if (typeY == typeof(uint))
                return Convert.ToUInt32(x).CompareTo(y.GetValue<uint>());
            if (typeY == typeof(ushort))
                return x.CompareTo(y.GetValue<ushort>());
            if (typeY == typeof(ulong))
                return Convert.ToUInt64(x).CompareTo(y.GetValue<ulong>());
            if (typeY == typeof(sbyte))
                return Convert.ToInt32(x).CompareTo(Convert.ToInt32(y.GetValue<sbyte>()));

            return CompareNumberWithAllocation(x, y);
        }
        
        private static int CompareNumber(ulong x, JsonValue y, Type typeY)
        {
            if (typeY == typeof(int))
                return Convert.ToDecimal(x).CompareTo(Convert.ToDecimal(y.GetValue<int>()));
            if (typeY == typeof(long))
                return Convert.ToDecimal(x).CompareTo(Convert.ToDecimal(y.GetValue<long>()));
            if (typeY == typeof(double))
                return CompareDouble(Convert.ToDouble(x), y.GetValue<double>());
            if (typeY == typeof(short))
                return Convert.ToDecimal(x).CompareTo(Convert.ToDecimal(y.GetValue<short>()));
            if (typeY == typeof(decimal))
                return Convert.ToDecimal(x).CompareTo(y.GetValue<decimal>());
            if (typeY == typeof(byte))
                return x.CompareTo(Convert.ToUInt64(y.GetValue<byte>()));
            if (typeY == typeof(float))
                return Convert.ToSingle(x).CompareTo(y.GetValue<float>());
            if (typeY == typeof(uint))
                return x.CompareTo(Convert.ToUInt64(y.GetValue<uint>()));
            if (typeY == typeof(ushort))
                return x.CompareTo(Convert.ToUInt64(y.GetValue<ushort>()));
            if (typeY == typeof(ulong))
                return x.CompareTo(y.GetValue<ulong>());
            if (typeY == typeof(sbyte))
                return Convert.ToDecimal(x).CompareTo(Convert.ToDecimal(y.GetValue<sbyte>()));

            return CompareNumberWithAllocation(x, y);
        }
        
        private static int CompareNumber(sbyte x, JsonValue y, Type typeY)
        {
            if (typeY == typeof(int))
                return Convert.ToInt32(x).CompareTo(y.GetValue<int>());
            if (typeY == typeof(long))
                return Convert.ToInt64(x).CompareTo(y.GetValue<long>());
            if (typeY == typeof(double))
                return CompareDouble(Convert.ToDouble(x), y.GetValue<double>());
            if (typeY == typeof(short))
                return Convert.ToInt16(x).CompareTo(y.GetValue<short>());
            if (typeY == typeof(decimal))
                return Convert.ToDecimal(x).CompareTo(y.GetValue<decimal>());
            if (typeY == typeof(byte))
                return Convert.ToInt16(x).CompareTo(Convert.ToInt16(y.GetValue<byte>()));
            if (typeY == typeof(float))
                return Convert.ToSingle(x).CompareTo(y.GetValue<float>());
            if (typeY == typeof(uint))
                return Convert.ToInt64(x).CompareTo(Convert.ToInt64(y.GetValue<uint>()));
            if (typeY == typeof(ushort))
                return Convert.ToInt32(x).CompareTo(Convert.ToInt32(y.GetValue<ushort>()));
            if (typeY == typeof(ulong))
                return Convert.ToDecimal(x).CompareTo(Convert.ToDecimal(y.GetValue<ulong>()));
            if (typeY == typeof(sbyte))
                return x.CompareTo(y.GetValue<sbyte>());

            return CompareNumberWithAllocation(x, y);
        }
        
        private static int CompareNumber(double x, JsonValue y, Type typeY)
        {
            if (typeY == typeof(int))
                return CompareDouble(x, Convert.ToDouble(y.GetValue<int>()));
            if (typeY == typeof(long))
                return CompareDouble(x, Convert.ToDouble(y.GetValue<long>()));
            if (typeY == typeof(double))
                return CompareDouble(x, y.GetValue<double>());
            if (typeY == typeof(short))
                return CompareDouble(x, Convert.ToDouble(y.GetValue<short>()));
            if (typeY == typeof(decimal))
                return Convert.ToDecimal(x).CompareTo(y.GetValue<decimal>());
            if (typeY == typeof(byte))
                return CompareDouble(x, Convert.ToDouble(y.GetValue<byte>()));
            if (typeY == typeof(float))
                return CompareDouble(x, Convert.ToDouble(y.GetValue<float>()));
            if (typeY == typeof(uint))
                return CompareDouble(x, Convert.ToDouble(y.GetValue<uint>()));
            if (typeY == typeof(ushort))
                return CompareDouble(x, Convert.ToDouble(y.GetValue<ushort>()));
            if (typeY == typeof(ulong))
                return CompareDouble(x, Convert.ToDouble(y.GetValue<ulong>()));
            if (typeY == typeof(sbyte))
                return CompareDouble(x, Convert.ToDouble(y.GetValue<sbyte>()));

            return CompareNumberWithAllocation(x, y);
        }
        
        private static int CompareNumber(decimal x, JsonValue y, Type typeY)
        {
            if (typeY == typeof(int))
                return x.CompareTo(Convert.ToDecimal(y.GetValue<int>()));
            if (typeY == typeof(long))
                return x.CompareTo(Convert.ToDecimal(y.GetValue<long>()));
            if (typeY == typeof(double))
                return x.CompareTo(Convert.ToDecimal(y.GetValue<double>()));
            if (typeY == typeof(short))
                return x.CompareTo(Convert.ToDecimal(y.GetValue<short>()));
            if (typeY == typeof(decimal))
                return x.CompareTo(y.GetValue<decimal>());
            if (typeY == typeof(byte))
                return x.CompareTo(Convert.ToDecimal(y.GetValue<byte>()));
            if (typeY == typeof(float))
                return x.CompareTo(Convert.ToDecimal(y.GetValue<float>()));
            if (typeY == typeof(uint))
                return x.CompareTo(Convert.ToDecimal(y.GetValue<uint>()));
            if (typeY == typeof(ushort))
                return x.CompareTo(Convert.ToDecimal(y.GetValue<ushort>()));
            if (typeY == typeof(ulong))
                return x.CompareTo(Convert.ToDecimal(y.GetValue<ulong>()));
            if (typeY == typeof(sbyte))
                return x.CompareTo(Convert.ToDecimal(y.GetValue<sbyte>()));

            return CompareNumberWithAllocation(x, y);
        }
        
        private static int CompareNumber(float x, JsonValue y, Type typeY)
        {
            if (typeY == typeof(int))
                return x.CompareTo(Convert.ToSingle(y.GetValue<int>()));
            if (typeY == typeof(long))
                return x.CompareTo(Convert.ToSingle(y.GetValue<long>()));
            if (typeY == typeof(double))
                return CompareDouble(Convert.ToDouble(x), y.GetValue<double>());
            if (typeY == typeof(short))
                return x.CompareTo(Convert.ToSingle(y.GetValue<short>()));
            if (typeY == typeof(decimal))
                return Convert.ToDecimal(x).CompareTo(y.GetValue<decimal>());
            if (typeY == typeof(byte))
                return x.CompareTo(Convert.ToSingle(y.GetValue<byte>()));
            if (typeY == typeof(float))
                return x.CompareTo(Convert.ToSingle(y.GetValue<float>()));
            if (typeY == typeof(uint))
                return x.CompareTo(Convert.ToSingle(y.GetValue<uint>()));
            if (typeY == typeof(ushort))
                return x.CompareTo(Convert.ToSingle(y.GetValue<ushort>()));
            if (typeY == typeof(ulong))
                return x.CompareTo(Convert.ToSingle(y.GetValue<ulong>()));
            if (typeY == typeof(sbyte))
                return x.CompareTo(Convert.ToSingle(y.GetValue<sbyte>()));

            return CompareNumberWithAllocation(x, y);
        }

        private static int CompareNumberWithAllocation<T>(T x, JsonNode y)
        {
#if DEBUG
            // Ideally this method should not be called at all
            Debug.Assert(false);
#endif
            return x switch
            {
                double or float => CompareDouble(
                    // ReSharper disable once HeapView.PossibleBoxingAllocation
                    Convert.ToDouble(x, CultureInfo.InvariantCulture),
                    Convert.ToDouble(y.GetValue<object>(), CultureInfo.InvariantCulture)),
                // ReSharper disable once HeapView.PossibleBoxingAllocation
                _ => Convert.ToDecimal(x, CultureInfo.InvariantCulture).CompareTo(
                    Convert.ToDecimal(y.GetValue<object>(), CultureInfo.InvariantCulture))
            };
        }

        private static bool AreDoubleClose(double x, double y)
        {
            // https://stackoverflow.com/questions/3874627/floating-point-comparison-functions-for-c-sharp
            const double epsilon = 2.22044604925031E-16;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (x == y)
                return true;

            var tolerance = (Math.Abs(x) + Math.Abs(y) + 10.0) * epsilon;
            var difference = x - y;
            
            if (-tolerance < difference)
                return tolerance > difference;

            return false;
        }

        private static int CompareDouble(double x, double y)
        {
            return AreDoubleClose(x, y) ? 0 : x.CompareTo(y);
        }
    }
}

