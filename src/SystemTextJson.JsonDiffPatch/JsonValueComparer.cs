using System.Diagnostics;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    /// <summary>
    /// Comparer for <see cref="JsonValue"/>.
    /// </summary>
    public static partial class JsonValueComparer
    {
        /// <summary>
        /// Compares two <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="x">The left value.</param>
        /// <param name="y">The right value.</param>
        public static int Compare(JsonValue? x, JsonValue? y)
        {
            var valueKindX = x.GetValueKind(true, out var typeX);
            var valueKindY = y.GetValueKind(true, out var typeY);

            if (valueKindX != valueKindY)
            {
                return -((int) valueKindX - (int) valueKindY);
            }

            Debug.Assert(typeX != typeof(JsonElement));
            Debug.Assert(typeY != typeof(JsonElement));

            return Compare(valueKindX, x, typeX, y, typeY);
        }

        internal static int Compare(JsonValueKind valueKind, JsonValue? x, Type? typeX, JsonValue? y, Type? typeY)
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

            switch (valueKind)
            {
                case JsonValueKind.Number:

                    if (typeX == typeof(int))
                        return CompareNumber(x.GetValue<int>(), y, typeY!);
                    if (typeX == typeof(long))
                        return CompareNumber(x.GetValue<long>(), y, typeY!);
                    if (typeX == typeof(decimal))
                        return CompareNumber(x.GetValue<decimal>(), y, typeY!);
                    if (typeX == typeof(double))
                        return CompareNumber(x.GetValue<double>(), y, typeY!);
                    if (typeX == typeof(short))
                        return CompareNumber(x.GetValue<short>(), y, typeY!);
                    if (typeX == typeof(byte))
                        return CompareNumber(x.GetValue<byte>(), y, typeY!);
                    if (typeX == typeof(float))
                        return CompareNumber(x.GetValue<float>(), y, typeY!);
                    if (typeX == typeof(uint))
                        return CompareNumber(x.GetValue<uint>(), y, typeY!);
                    if (typeX == typeof(ushort))
                        return CompareNumber(x.GetValue<ushort>(), y, typeY!);
                    if (typeX == typeof(ulong))
                        return CompareNumber(x.GetValue<ulong>(), y, typeY!);
                    if (typeX == typeof(sbyte))
                        return CompareNumber(x.GetValue<sbyte>(), y, typeY!);

                    return CompareNumberWithAllocation(x.GetValue<object>(), y);

                case JsonValueKind.String:

                    if (TryCompareDateTime(x, y, out var compareResult))
                        return compareResult;
                    if (TryCompareGuid(x, y, out compareResult))
                        return compareResult;
                    if (TryCompareChar(x, y, out compareResult))
                        return compareResult;
                    if (TryCompareByteArray(x, y, out compareResult))
                        return compareResult;

                    return CompareString(x, y);

                case JsonValueKind.Null:
                case JsonValueKind.False:
                case JsonValueKind.True:
                    return 0;

                case JsonValueKind.Undefined:
                case JsonValueKind.Object:
                case JsonValueKind.Array:
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(valueKind), $"Unexpected value kind {valueKind:G}");
            }
        }
    }
}

