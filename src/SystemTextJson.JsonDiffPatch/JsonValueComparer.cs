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
    }
}

