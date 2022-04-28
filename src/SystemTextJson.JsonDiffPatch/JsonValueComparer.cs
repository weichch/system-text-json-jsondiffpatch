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

            var contextX = new JsonValueComparisonContext(x);
            var contextY = new JsonValueComparisonContext(y);

            if (contextX.ValueKind != contextY.ValueKind)
            {
                return -((int) contextX.ValueKind - (int) contextY.ValueKind);
            }

            return Compare(contextX.ValueKind, ref contextX, ref contextY);
        }

        internal static int Compare(JsonValueKind valueKind, ref JsonValueComparisonContext x,
            ref JsonValueComparisonContext y)
        {
            Debug.Assert(x.Value is not null);
            Debug.Assert(y.Value is not null);
            Debug.Assert(x.ValueKind == y.ValueKind);

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
                                return CompareDateTime(ref x, ref y);
                            case JsonStringValueKind.Guid:
                                return x.GetGuid().CompareTo(y.GetGuid());
                            case JsonStringValueKind.String:
                                if ((x.ValueType == typeof(string) || x.ValueType == typeof(char))
                                    && (y.ValueType == typeof(string) || y.ValueType == typeof(char)))
                                {
                                    return StringComparer.Ordinal.Compare(x.GetString(), y.GetString());
                                }
                                // Because testing whether string is based64 encoded is expensive operation,
                                // we only test it when comparing with a byte array. Otherwise, we compare raw text.
                                else if (x.ValueType == typeof(byte[]) || y.ValueType == typeof(byte[]))
                                {
                                    if (TryCompareByteArray(ref x, ref y, out var compareResult))
                                    {
                                        return compareResult;
                                    }
                                }

                                break;
                        }
                    }

                    return StringComparer.Ordinal.Compare(x.GetRawText(), y.GetRawText());

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

