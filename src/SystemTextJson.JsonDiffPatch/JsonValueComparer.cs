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

            return Compare(contextX.ValueKind, contextX, contextY);
        }

        internal static int Compare(JsonValueKind valueKind, in JsonValueComparisonContext x,
            in JsonValueComparisonContext y)
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
                    if (x.StringValueKind is JsonStringValueKind.RawText ||
                        y.StringValueKind is JsonStringValueKind.RawText)
                    {
                        throw new ArgumentOutOfRangeException(nameof(x.StringValueKind),
                            $"Unexpected string value kind {x.StringValueKind:G}");
                    }

                    if (x.StringValueKind == y.StringValueKind)
                    {
                        switch (x.StringValueKind)
                        {
                            case JsonStringValueKind.DateTime:
                                return CompareDateTime(x, y);
                            case JsonStringValueKind.Guid:
                                return x.GetGuid().CompareTo(y.GetGuid());
                            case JsonStringValueKind.String:
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

                    return StringComparer.Ordinal.Compare(x.Value!.ToJsonString(), y.Value!.ToJsonString());

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

        internal static int CompareValue(JsonValueKind valueKind, in JsonValueComparisonContext x,
            in JsonValueComparisonContext y)
        {
            Debug.Assert(x.Value is null);
            Debug.Assert(y.Value is null);
            Debug.Assert(x.ValueKind == y.ValueKind);

            if (valueKind is JsonValueKind.True or JsonValueKind.False or JsonValueKind.Null)
            {
                return 0;
            }

            if (x.ValueObject is null && y.ValueObject is null)
            {
                return 0;
            }

            if (x.ValueObject is null)
            {
                return -1;
            }

            if (y.ValueObject is null)
            {
                return 1;
            }

            switch (valueKind)
            {
                case JsonValueKind.Number:
                    if (x.ValueObject is decimal or ulong || y.ValueObject is decimal or ulong)
                    {
                        return x.GetDecimal().CompareTo(y.GetDecimal());
                    }
                    else if (x.ValueObject is double or float || y.ValueObject is double or float)
                    {
                        return CompareDouble(x.GetDouble(), y.GetDouble());
                    }

                    return x.GetInt64().CompareTo(y.GetInt64());

                case JsonValueKind.String:
                    if (x.StringValueKind is JsonStringValueKind.RawText ||
                        y.StringValueKind is JsonStringValueKind.RawText)
                    {
                        throw new ArgumentOutOfRangeException(nameof(x.StringValueKind),
                            $"Unexpected string value kind {x.StringValueKind:G}");
                    }

                    if (x.StringValueKind == y.StringValueKind)
                    {
                        switch (x.StringValueKind)
                        {
                            case JsonStringValueKind.DateTime:
                                return CompareDateTimeValue(x.ValueObject!, y.ValueObject!);
                            case JsonStringValueKind.Guid:
                                return x.GetGuid().CompareTo(y.GetGuid());
                        }
                    }

                    var strX = x.GetString();
                    var strY = y.GetString();

                    return StringComparer.Ordinal.Compare(strX, strY);

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(valueKind), $"Unexpected value kind {valueKind:G}");
            }
        }
    }
}

