using System.Globalization;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    /// <summary>
    /// Comparer for <see cref="JsonValue"/>.
    /// </summary>
    static partial class JsonValueComparer
    {
        private static int CompareString(JsonValue x, JsonValue y)
        {
            if (x.TryGetValue<string>(out var valueX) && y.TryGetValue<string>(out var valueY))
            {
                return StringComparer.Ordinal.Compare(valueX, valueY);
            }

            return StringComparer.Ordinal.Compare(
                Convert.ToString(x.GetValue<object>(), CultureInfo.InvariantCulture),
                Convert.ToString(y.GetValue<object>(), CultureInfo.InvariantCulture));
        }

        private static bool TryCompareDateTime(JsonValue x, JsonValue y, out int result)
        {
            if (x.TryGetValue<DateTimeOffset>(out var dateTimeOffsetX)
                && y.TryGetValue<DateTimeOffset>(out var dateTimeOffsetY))
            {
                result = dateTimeOffsetX.CompareTo(dateTimeOffsetY);
                return true;
            }

            if (x.TryGetValue<DateTime>(out var dateTimeX)
                && y.TryGetValue<DateTime>(out var dateTimeY))
            {
                result = dateTimeX.CompareTo(dateTimeY);
                return true;
            }
            
            if (x.TryGetValue(out dateTimeOffsetX)
                && y.TryGetValue(out dateTimeY))
            {
                result = dateTimeOffsetX.CompareTo(new DateTimeOffset(dateTimeY));
                return true;
            }
            
            if (x.TryGetValue(out dateTimeX)
                && y.TryGetValue(out dateTimeOffsetY))
            {
                result = new DateTimeOffset(dateTimeX).CompareTo(dateTimeOffsetY);
                return true;
            }

            result = -1;
            return false;
        }

        private static bool TryCompareGuid(JsonValue x, JsonValue y, out int result)
        {
            if (x.TryGetValue<Guid>(out var valueX)
                && y.TryGetValue<Guid>(out var valueY))
            {
                result = valueX.CompareTo(valueY);
                return true;
            }

            result = -1;
            return false;
        }

        private static bool TryCompareChar(JsonValue x, JsonValue y, out int result)
        {
            if (x.TryGetValue<char>(out var valueX)
                && y.TryGetValue<char>(out var valueY))
            {
                result = valueX.CompareTo(valueY);
                return true;
            }

            result = -1;
            return false;
        }

        private static bool TryCompareByteArray(JsonValue x, JsonValue y, out int result)
        {
            if (!x.TryGetValue<byte[]>(out var valueX))
            {
                if (x.TryGetValue<JsonElement>(out var jsonElement))
                {
                    jsonElement.TryGetBytesFromBase64(out valueX);
                }
            }

            if (!y.TryGetValue<byte[]>(out var valueY))
            {
                if (y.TryGetValue<JsonElement>(out var jsonElement))
                {
                    jsonElement.TryGetBytesFromBase64(out valueY);
                }
            }

            if (valueX is not null && valueY is not null)
            {
                result = CompareByteArray(valueX, valueY);
                return true;
            }

            result = -1;
            return false;
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
    }
}

