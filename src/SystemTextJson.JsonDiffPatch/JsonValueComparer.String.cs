using System.Globalization;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    static partial class JsonValueComparer
    {
        private static int CompareString(JsonValue x, Type typeX, JsonValue y, Type typeY)
        {
            if ((typeX == typeof(string) || typeX == typeof(JsonElement))
                && (typeY == typeof(string) || typeY == typeof(JsonElement)))
            {
                if (x.TryGetValue<string>(out var valueX) && y.TryGetValue<string>(out var valueY))
                {
                    return StringComparer.Ordinal.Compare(valueX, valueY);
                }
            }

            return StringComparer.Ordinal.Compare(
                Convert.ToString(x.GetValue<object>(), CultureInfo.InvariantCulture),
                Convert.ToString(y.GetValue<object>(), CultureInfo.InvariantCulture));
        }

        private static bool TryCompareDateTime(JsonValue x, Type typeX, JsonValue y, Type typeY, out int result)
        {
            if ((typeX == typeof(DateTime) || typeX == typeof(DateTimeOffset) || typeX == typeof(JsonElement))
                && (typeY == typeof(DateTime) || typeY == typeof(DateTimeOffset) || typeY == typeof(JsonElement)))
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

                if (x.TryGetValue(out dateTimeOffsetX) && y.TryGetValue(out dateTimeY))
                {
                    result = dateTimeOffsetX.CompareTo(new DateTimeOffset(dateTimeY));
                    return true;
                }

                if (x.TryGetValue(out dateTimeX) && y.TryGetValue(out dateTimeOffsetY))
                {
                    result = new DateTimeOffset(dateTimeX).CompareTo(dateTimeOffsetY);
                    return true;
                }
            }

            result = -1;
            return false;
        }

        private static bool TryCompareGuid(JsonValue x, Type typeX, JsonValue y, Type typeY, out int result)
        {
            if ((typeX == typeof(Guid) || typeX == typeof(JsonElement))
                && (typeY == typeof(Guid) || typeY == typeof(JsonElement)))
            {
                if (x.TryGetValue<Guid>(out var valueX) && y.TryGetValue<Guid>(out var valueY))
                {
                    result = valueX.CompareTo(valueY);
                    return true;
                }
            }

            result = -1;
            return false;
        }

        private static bool TryCompareChar(JsonValue x, Type typeX, JsonValue y, Type typeY, out int result)
        {
            if ((typeX == typeof(char) || typeX == typeof(JsonElement))
                && (typeY == typeof(char) || typeY == typeof(JsonElement)))
            {
                if (x.TryGetValue<char>(out var valueX) && y.TryGetValue<char>(out var valueY))
                {
                    result = valueX.CompareTo(valueY);
                    return true;
                }
            }

            result = -1;
            return false;
        }

        private static bool TryCompareByteArray(JsonValue x, Type typeX, JsonValue y, Type typeY, out int result)
        {
            if ((typeX == typeof(byte[]) || typeX == typeof(JsonElement))
                && (typeY == typeof(byte[]) || typeY == typeof(JsonElement)))
            {
                var valueX = GetByteArray(x);
                if (valueX is not null)
                {
                    var valueY = GetByteArray(y);
                    if (valueY is not null)
                    {
                        result = CompareByteArray(valueX, valueY);
                        return true;
                    }
                }
            }

            result = -1;
            return false;

            static byte[]? GetByteArray(JsonValue value)
            {
                if (!value.TryGetValue<byte[]>(out var byteArray))
                {
                    if (value.TryGetValue<JsonElement>(out var jsonElement))
                    {
                        jsonElement.TryGetBytesFromBase64(out byteArray);
                    }
                }

                return byteArray;
            }
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

