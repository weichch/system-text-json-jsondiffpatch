namespace System.Text.Json.JsonDiffPatch
{
    static partial class JsonValueComparer
    {
        private static int CompareDateTime(ref JsonValueComparisonContext x, ref JsonValueComparisonContext y)
        {
            if (x.ValueType == typeof(DateTime))
            {
                return x.GetDateTime().CompareTo(y.GetDateTime());
            }

            return x.GetDateTimeOffset().CompareTo(y.GetDateTimeOffset());
        }

        private static bool TryCompareByteArray(ref JsonValueComparisonContext x, ref JsonValueComparisonContext y,
            out int result)
        {
            if (x.ValueType == typeof(byte[]))
            {
                if (y.TryGetByteArray(out var byteArrayY))
                {
                    result = CompareByteArray(x.GetByteArray(), byteArrayY);
                    return true;
                }
            }
            else if (y.ValueType == typeof(byte[]))
            {
                if (x.TryGetByteArray(out var byteArrayX))
                {
                    result = CompareByteArray(byteArrayX, y.GetByteArray());
                    return true;
                }
            }

            result = -1;
            return false;
        }

        private static int CompareByteArray(byte[]? x, byte[]? y)
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

