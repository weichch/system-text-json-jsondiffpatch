using System.Diagnostics;

namespace System.Text.Json.JsonDiffPatch
{
    internal readonly ref struct JsonNumber
    {
        private readonly long? _longValue;
        private readonly decimal? _decimalValue;
        private readonly double? _doubleValue;

        public JsonNumber(in JsonElement element)
        {
            _longValue = null;
            _decimalValue = null;
            _doubleValue = null;

            if (element.TryGetInt64(out var longValue))
            {
                _longValue = longValue;
            }
            else if (element.TryGetDecimal(out var decimalValue))
            {
                _decimalValue = decimalValue;
            }
            else if (element.TryGetDouble(out var doubleValue))
            {
                _doubleValue = doubleValue;
            }

            Debug.Assert(_longValue is not null || _decimalValue is not null || _doubleValue is not null);
        }

        private long GetInt64() => _longValue!.Value;

        private decimal GetDecimal()
        {
            if (_longValue.HasValue)
            {
                return Convert.ToDecimal(_longValue.Value);
            }

            if (_doubleValue.HasValue)
            {
                return Convert.ToDecimal(_doubleValue.Value);
            }

            return _decimalValue!.Value;
        }

        private double GetDouble() => _doubleValue!.Value;

        public int CompareTo(in JsonNumber another)
        {
            if (_longValue.HasValue && another._longValue.HasValue)
            {
                return GetInt64().CompareTo(another.GetInt64());
            }

            if (_doubleValue.HasValue && another._doubleValue.HasValue)
            {
                return JsonValueComparer.CompareDouble(GetDouble(), another.GetDouble());
            }

            return GetDecimal().CompareTo(another.GetDecimal());
        }
    }
}