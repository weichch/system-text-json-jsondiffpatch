using System.Globalization;
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

            var valueKindX = x.GetValueKind(out var typeX);
            var valueKindY = y.GetValueKind(out var typeY);

            if (valueKindX != valueKindY)
            {
                return -((int) valueKindX - (int) valueKindY);
            }

            return Compare(valueKindX, new JsonValueComparisonContext(valueKindX, x, typeX),
                new JsonValueComparisonContext(valueKindY, y, typeY));
        }

        internal static int Compare(JsonValueKind valueKind, in JsonValueComparisonContext x,
            in JsonValueComparisonContext y)
        {
            switch (valueKind)
            {
                case JsonValueKind.Number:
                    if (x.ValueType == typeof(decimal) || y.ValueType == typeof(decimal) ||
                        x.ValueType == typeof(ulong) || y.ValueType == typeof(ulong))
                    {
                        return x.GetDecimal().CompareTo(y.GetDecimal());
                    }

                    if (x.ValueType == typeof(double) || y.ValueType == typeof(double) ||
                        x.ValueType == typeof(float) || y.ValueType == typeof(float))
                    {
                        return CompareDouble(x.GetDouble(), y.GetDouble());
                    }

                    return x.GetInt64().CompareTo(y.GetInt64());

                case JsonValueKind.String:
                    if (x.StringValueKind == y.StringValueKind)
                    {
                        switch (x.StringValueKind)
                        {
                            case JsonStringValueKind.DateTime:
                                return CompareDateTime(x, y);
                            case JsonStringValueKind.Guid:
                                return x.GetGuid().CompareTo(y.GetGuid());
                            case JsonStringValueKind.String:
                            default:
                                if ((x.ValueType == typeof(string) || x.ValueType == typeof(char))
                                    && (y.ValueType == typeof(string) || y.ValueType == typeof(char)))
                                {
                                    return StringComparer.Ordinal.Compare(x.GetString(), y.GetString());
                                }
                                else if (x.ValueType == typeof(byte[]) || y.ValueType == typeof(byte[]))
                                {
                                    if (TryCompareByteArray(x, y, out var compareResult))
                                    {
                                        return compareResult;
                                    }
                                }

                                break;
                        }
                    }

                    return StringComparer.Ordinal.Compare(x.Value.ToJsonString(), y.Value.ToJsonString());

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
        
        internal static int CompareValue(JsonValueKind valueKind, object? x, object? y)
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
                    if (x is decimal or ulong || y is decimal or ulong)
                    {
                        return Convert.ToDecimal(x, CultureInfo.InvariantCulture).CompareTo(
                            Convert.ToDecimal(y, CultureInfo.InvariantCulture));
                    }
                    else if (x is double or float || y is double or float)
                    {
                        return CompareDouble(Convert.ToDouble(x, CultureInfo.InvariantCulture),
                            Convert.ToDouble(y, CultureInfo.InvariantCulture));
                    }

                    return Convert.ToInt64(x, CultureInfo.InvariantCulture).CompareTo(
                        Convert.ToInt64(y, CultureInfo.InvariantCulture));

                case JsonValueKind.String:
                    if (x is DateTime or DateTimeOffset && y is DateTime or DateTimeOffset)
                    {
                        if (x is DateTime dateTimeX)
                        {
                            if (y is DateTime dateTimeY)
                            {
                                return dateTimeX.CompareTo(dateTimeY);
                            }

                            if (y is DateTimeOffset dateTimeOffsetY)
                            {
                                return new DateTimeOffset(dateTimeX).CompareTo(dateTimeOffsetY);
                            }
                        }
                        else if (x is DateTimeOffset dateTimeOffsetX)
                        {
                            if (y is DateTime dateTimeY)
                            {
                                return dateTimeOffsetX.CompareTo(new DateTimeOffset(dateTimeY));
                            }

                            if (y is DateTimeOffset dateTimeOffsetY)
                            {
                                return dateTimeOffsetX.CompareTo(dateTimeOffsetY);
                            }
                        }
                    }
                    else if (x is Guid guidX && y is Guid guidY)
                    {
                        return guidX.CompareTo(guidY);
                    }

                    var strX = x is byte[] bytesX
                        ? Convert.ToBase64String(bytesX)
                        : Convert.ToString(x, CultureInfo.InvariantCulture);
                    var strY = y is byte[] bytesY
                        ? Convert.ToBase64String(bytesY)
                        : Convert.ToString(y, CultureInfo.InvariantCulture);

                    return StringComparer.Ordinal.Compare(strX, strY);

                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                    return 0;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(valueKind), $"Unexpected value kind {valueKind:G}");
            }
        }
    }
}

