namespace System.Text.Json.JsonDiffPatch
{
    internal readonly ref struct JsonString
    {
        private readonly JsonElement _element;
        private readonly DateTime? _dateTimeValue;
        private readonly DateTimeOffset? _dateTimeOffsetValue;
        private readonly Guid? _guidValue;

        public JsonString(in JsonElement element)
        {
            _element = element;
            _dateTimeValue = null;
            _dateTimeOffsetValue = null;
            _guidValue = null;

            if (element.TryGetDateTime(out var dateTimeValue))
            {
                _dateTimeValue = dateTimeValue;
            }
            else if (element.TryGetDateTimeOffset(out var dateTimeOffsetValue))
            {
                _dateTimeOffsetValue = dateTimeOffsetValue;
            }
            else if (element.TryGetGuid(out var guidValue))
            {
                _guidValue = guidValue;
            }
        }

        public int CompareTo(in JsonString another)
        {
            if (_dateTimeValue.HasValue)
            {
                if (another._dateTimeValue.HasValue)
                {
                    return _dateTimeValue.Value.CompareTo(another._dateTimeValue.Value);
                }

                if (another._dateTimeOffsetValue.HasValue)
                {
                    return _dateTimeValue.Value.CompareTo(another._dateTimeOffsetValue.Value.DateTime);
                }
            }
            else if (_dateTimeOffsetValue.HasValue)
            {
                if (another._dateTimeOffsetValue.HasValue)
                {
                    return _dateTimeOffsetValue.Value.CompareTo(another._dateTimeOffsetValue.Value);
                }

                if (another._dateTimeValue.HasValue)
                {
                    return _dateTimeOffsetValue.Value.CompareTo(new DateTimeOffset(another._dateTimeValue.Value));
                }
            }
            else if (_guidValue.HasValue && another._guidValue.HasValue)
            {
                return _guidValue.Value.CompareTo(another._guidValue.Value);
            }

            return StringComparer.Ordinal.Compare(_element.GetString(), another._element.GetString());
        }

        public bool ValueEquals(JsonString another)
        {
            if (_dateTimeValue.HasValue)
            {
                if (another._dateTimeValue.HasValue)
                {
                    return _dateTimeValue.Value.Equals(another._dateTimeValue.Value);
                }

                if (another._dateTimeOffsetValue.HasValue)
                {
                    return _dateTimeValue.Value.Equals(another._dateTimeOffsetValue.Value.DateTime);
                }
            }
            else if (_dateTimeOffsetValue.HasValue)
            {
                if (another._dateTimeOffsetValue.HasValue)
                {
                    return _dateTimeOffsetValue.Value.Equals(another._dateTimeOffsetValue.Value);
                }

                if (another._dateTimeValue.HasValue)
                {
                    return _dateTimeOffsetValue.Value.Equals(new DateTimeOffset(another._dateTimeValue.Value));
                }
            }
            else if (_guidValue.HasValue && another._guidValue.HasValue)
            {
                return _guidValue.Value.Equals(another._guidValue.Value);
            }

            return _element.ValueEquals(another._element.GetString());
        }
    }
}