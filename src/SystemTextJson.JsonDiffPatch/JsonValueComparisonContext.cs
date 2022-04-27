using System.Diagnostics;
using System.Globalization;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    /// <summary>
    /// Wrapper of <see cref="JsonNode"/> for value comparison purpose.
    /// </summary>
    internal readonly struct JsonValueComparisonContext
    {
        private readonly bool _isJsonElement;
        private readonly JsonElement _element;
        private readonly bool _isJsonElementRead;

        public JsonValueComparisonContext(JsonValue value)
        {
            var isJsonElement = value.TryGetValue(out _element);
            _isJsonElement = isJsonElement;
            Value = value;
            ValueKind = GetValueKind(value, out var valueType);
            var actualValueType = isJsonElement ? GetElementValueType(_element) : valueType;
            ValueType = actualValueType;
            StringValueKind = GetStringValueKind(actualValueType);

            ValueObject = null;
            _isJsonElementRead = false;
        }

        public JsonValueComparisonContext(in JsonElement element, bool readElement = false)
        {
            _element = element;
            ValueKind = element.ValueKind;
            StringValueKind = JsonStringValueKind.RawText;
            ValueObject = readElement ? element.GetRawText() : null;
            _isJsonElement = true;

            Value = null;
            ValueType = null;
            _isJsonElementRead = false;
        }

        public JsonValueComparisonContext(JsonValueKind valueKind, object? valueObject)
        {
            _element = default;
            ValueKind = valueKind;
            Value = null;
            ValueObject = valueObject;
            _isJsonElement = false;
            ValueType = null;
            StringValueKind = valueKind is JsonValueKind.String
                ? GetStringValueKind(valueObject)
                : JsonStringValueKind.String;
            _isJsonElementRead = false;
        }

        public JsonValueKind ValueKind { get; }
        public JsonValue? Value { get; }
        public object? ValueObject { get; }
        public Type? ValueType { get; }
        public JsonStringValueKind StringValueKind { get; }

        public decimal GetDecimal()
        {
            if (Value is null)
            {
                return Convert.ToDecimal(ValueObject, CultureInfo.InvariantCulture);
            }

            if (ValueType == typeof(decimal))
                return Value.GetValue<decimal>();
            if (ValueType == typeof(ulong))
                return Convert.ToDecimal(Value.GetValue<ulong>());
            if (ValueType == typeof(long))
                return Convert.ToDecimal(Value.GetValue<long>());
            if (ValueType == typeof(double))
                return Convert.ToDecimal(Value.GetValue<double>());
            if (ValueType == typeof(int))
                return Convert.ToDecimal(Value.GetValue<int>());
            if (ValueType == typeof(short))
                return Convert.ToDecimal(Value.GetValue<short>());
            if (ValueType == typeof(byte))
                return Convert.ToDecimal(Value.GetValue<byte>());
            if (ValueType == typeof(float))
                return Convert.ToDecimal(Value.GetValue<float>());
            if (ValueType == typeof(uint))
                return Convert.ToDecimal(Value.GetValue<uint>());
            if (ValueType == typeof(ushort))
                return Convert.ToDecimal(Value.GetValue<ushort>());
            if (ValueType == typeof(sbyte))
                return Convert.ToDecimal(Value.GetValue<sbyte>());

            throw new ArgumentOutOfRangeException($"Unsupported value type '{ValueType?.FullName ?? "null"}'.");
        }

        public double GetDouble()
        {
            if (Value is null)
            {
                return Convert.ToDouble(ValueObject, CultureInfo.InvariantCulture);
            }

            if (ValueType == typeof(double))
                return Value.GetValue<double>();
            if (ValueType == typeof(long))
                return Convert.ToDouble(Value.GetValue<long>());
            if (ValueType == typeof(decimal))
                return Convert.ToDouble(Value.GetValue<decimal>());
            if (ValueType == typeof(int))
                return Convert.ToDouble(Value.GetValue<int>());
            if (ValueType == typeof(short))
                return Convert.ToDouble(Value.GetValue<short>());
            if (ValueType == typeof(byte))
                return Convert.ToDouble(Value.GetValue<byte>());
            if (ValueType == typeof(float))
                return Convert.ToDouble(Value.GetValue<float>());
            if (ValueType == typeof(uint))
                return Convert.ToDouble(Value.GetValue<uint>());
            if (ValueType == typeof(ushort))
                return Convert.ToDouble(Value.GetValue<ushort>());
            if (ValueType == typeof(ulong))
                return Convert.ToDouble(Value.GetValue<ulong>());
            if (ValueType == typeof(sbyte))
                return Convert.ToDouble(Value.GetValue<sbyte>());

            throw new ArgumentOutOfRangeException($"Unsupported value type '{ValueType?.FullName ?? "null"}'.");
        }

        public long GetInt64()
        {
            if (Value is null)
            {
                return Convert.ToInt64(ValueObject, CultureInfo.InvariantCulture);
            }

            if (ValueType == typeof(long))
                return Value.GetValue<long>();
            if (ValueType == typeof(decimal))
                return Convert.ToInt64(Value.GetValue<decimal>());
            if (ValueType == typeof(double))
                return Convert.ToInt64(Value.GetValue<double>());
            if (ValueType == typeof(int))
                return Convert.ToInt64(Value.GetValue<int>());
            if (ValueType == typeof(short))
                return Convert.ToInt64(Value.GetValue<short>());
            if (ValueType == typeof(byte))
                return Convert.ToInt64(Value.GetValue<byte>());
            if (ValueType == typeof(float))
                return Convert.ToInt64(Value.GetValue<float>());
            if (ValueType == typeof(uint))
                return Convert.ToInt64(Value.GetValue<uint>());
            if (ValueType == typeof(ushort))
                return Convert.ToInt64(Value.GetValue<ushort>());
            if (ValueType == typeof(ulong))
                return Convert.ToInt64(Value.GetValue<ulong>());
            if (ValueType == typeof(sbyte))
                return Convert.ToInt64(Value.GetValue<sbyte>());

            throw new ArgumentOutOfRangeException($"Unsupported value type '{ValueType?.FullName ?? "null"}'.");
        }

        public DateTime GetDateTime() =>
            Value?.GetValue<DateTime>() ?? Convert.ToDateTime(ValueObject, CultureInfo.InvariantCulture);

        public DateTimeOffset GetDateTimeOffset() =>
            Value?.GetValue<DateTimeOffset>() ?? new DateTimeOffset(Convert.ToDateTime(ValueObject,
                CultureInfo.InvariantCulture));

        private char GetChar() => Value?.GetValue<char>() ?? (char) ValueObject!;

        public Guid GetGuid() => Value?.GetValue<Guid>() ?? (Guid) ValueObject!;

        public byte[] GetByteArray() => Value is null ? (byte[]) ValueObject! : Value.GetValue<byte[]>();

        public bool TryGetByteArray(out byte[]? value)
        {
            if (Value is null)
            {
                if (ValueObject is byte[] byteArray)
                {
                    value = byteArray;
                    return true;
                }
            }
            else
            {
                if (ValueType == typeof(byte[]))
                {
                    value = Value.GetValue<byte[]>();
                    return true;
                }

                if (_isJsonElement)
                {
                    if (_element.TryGetBytesFromBase64(out value))
                    {
                        return true;
                    }
                }
            }

            value = null;
            return false;
        }

        public string? GetString()
        {
            if (StringValueKind is JsonStringValueKind.RawText)
            {
                return _element.GetString();
            }

            if (Value is null)
            {
                if (ValueObject is byte[] byteArray)
                {
                    return Convert.ToBase64String(byteArray);
                }

                return Convert.ToString(ValueObject, CultureInfo.InvariantCulture);
            }

            return ValueType == typeof(char)
                ? Value.GetValue<char>().ToString()
                : Value.GetValue<string>();
        }

        private string GetRawText()
        {
            if (StringValueKind != JsonStringValueKind.RawText)
            {
                throw new InvalidOperationException("No raw JSON text.");
            }

            if (_isJsonElementRead)
            {
                return (string) ValueObject!;
            }

            return _element.GetRawText();
        }

        private bool StringValueEquals(string? text) => _element.ValueEquals(text);

        private bool StringValueEquals(in ReadOnlySpan<char> text) => _element.ValueEquals(text);

        public bool DeepEquals(in JsonValueComparisonContext another)
        {
            if (ValueKind != another.ValueKind)
            {
                return false;
            }

            if (ValueKind is JsonValueKind.Null or JsonValueKind.True or JsonValueKind.False)
            {
                return true;
            }

            // Fast: raw text comparison
            if (StringValueKind is JsonStringValueKind.RawText ||
                another.StringValueKind is JsonStringValueKind.RawText)
            {
                if (StringValueKind != another.StringValueKind)
                {
                    Debug.Assert(false, "Unexpected comparison of raw text and value");
                    return false;
                }

                switch (ValueKind)
                {
                    case JsonValueKind.String:
                        return StringValueEquals(another.GetString());
                    default:
                        return string.Equals(GetRawText(), another.GetRawText());
                }
            }

            // Slow: value semantic comparison without cached value
            if (Value is not null || another.Value is not null)
            {
                if (Value is null || another.Value is null)
                {
                    Debug.Assert(false, "Unexpected comparison of value and cached value");
                    return false;
                }

                switch (ValueKind)
                {
                    case JsonValueKind.Number:
                        return JsonValueComparer.Compare(ValueKind, this, another) == 0;

                    case JsonValueKind.String:
                        if (StringValueKind is JsonStringValueKind.DateTime or JsonStringValueKind.Guid &&
                            another.StringValueKind is JsonStringValueKind.DateTime or JsonStringValueKind.Guid)
                        {
                            if (StringValueKind != another.StringValueKind)
                            {
                                return false;
                            }
                        }

                        // Try compare strings when possible
                        if (_isJsonElement &&
                            (another.ValueType == typeof(string) || another.ValueType == typeof(char)))
                        {
                            if (another.ValueType == typeof(char))
                            {
                                Span<char> valueY = stackalloc char[1];
                                valueY[0] = another.GetChar();
                                return StringValueEquals(valueY);
                            }

                            return StringValueEquals(another.GetString());
                        }

                        if (another._isJsonElement && (ValueType == typeof(string) || ValueType == typeof(char)))
                        {
                            if (ValueType == typeof(char))
                            {
                                Span<char> valueX = stackalloc char[1];
                                valueX[0] = GetChar();
                                return another.StringValueEquals(valueX);
                            }

                            return another.StringValueEquals(GetString());
                        }

                        return JsonValueComparer.Compare(ValueKind, this, another) == 0;

                    default:
                        return Value.TryGetValue<object>(out var objX)
                               && another.Value.TryGetValue<object>(out var objY)
                               && Equals(objX, objY);
                }
            }

            // Perf: cached value semantic comparison
            if (Value is null && another.Value is null)
            {
                return JsonValueComparer.CompareValue(ValueKind, this, another) == 0;
            }

            Debug.Assert(false, "Unknown JSON value comparison type");
            return false;
        }

        public static bool IsValueKind(JsonValueKind jsonValueKind)
        {
            return jsonValueKind is JsonValueKind.Number or JsonValueKind.String or JsonValueKind.True
                or JsonValueKind.False or JsonValueKind.Null;
        }

        private static JsonStringValueKind GetStringValueKind(Type? valueType)
        {
            if (valueType == typeof(DateTime) || valueType == typeof(DateTimeOffset))
            {
                return JsonStringValueKind.DateTime;
            }

            if (valueType == typeof(Guid))
            {
                return JsonStringValueKind.Guid;
            }

            return JsonStringValueKind.String;
        }

        private static JsonStringValueKind GetStringValueKind(object? valueObject)
        {
            if (valueObject is DateTime or DateTimeOffset)
            {
                return JsonStringValueKind.DateTime;
            }

            if (valueObject is Guid)
            {
                return JsonStringValueKind.Guid;
            }

            return JsonStringValueKind.String;
        }

        private static Type GetElementValueType(in JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Number:
                    if (element.TryGetInt64(out _))
                        return typeof(long);
                    if (element.TryGetDecimal(out _))
                        return typeof(decimal);
                    if (element.TryGetDouble(out _))
                        return typeof(double);

                    throw new ArgumentException("Unsupported JSON number.");

                case JsonValueKind.String:
                    if (element.TryGetDateTimeOffset(out _))
                        return typeof(DateTimeOffset);
                    if (element.TryGetDateTime(out _))
                        return typeof(DateTime);
                    if (element.TryGetGuid(out _))
                        return typeof(Guid);

                    // Don't test base64 here, it's too expensive to test

                    return typeof(string);

                case JsonValueKind.True:
                case JsonValueKind.False:
                    return typeof(bool);

                case JsonValueKind.Undefined:
                case JsonValueKind.Object:
                case JsonValueKind.Array:
                default:
                    throw new ArgumentOutOfRangeException(nameof(element.ValueKind),
                        $"Unexpected value kind {element.ValueKind:G}");
            }
        }

        private static JsonValueKind GetValueKind(JsonValue? value, out Type? valueType)
        {
            if (value is null)
            {
                valueType = null;
                return JsonValueKind.Null;
            }

            if (value.TryGetValue<JsonElement>(out var element))
            {
                valueType = typeof(JsonElement);
                return element.ValueKind;
            }

            if (TryGetNumberValueKind(value, out valueType))
            {
                return JsonValueKind.Number;
            }

            if (TryGetStringValueKind(value, out valueType))
            {
                return JsonValueKind.String;
            }

            if (value.TryGetValue<bool>(out var booleanValue))
            {
                valueType = typeof(bool);
                return booleanValue ? JsonValueKind.True : JsonValueKind.False;
            }

            // Returns undefined to indicate object.Equals should be used to compare encapsulated values
            valueType = null;
            return JsonValueKind.Undefined;
        }

        private static bool TryGetNumberValueKind(JsonValue value, out Type? valueType)
        {
            if (value.TryGetValue<int>(out _))
            {
                valueType = typeof(int);
                return true;
            }

            if (value.TryGetValue<long>(out _))
            {
                valueType = typeof(long);
                return true;
            }

            if (value.TryGetValue<double>(out _))
            {
                valueType = typeof(double);
                return true;
            }

            if (value.TryGetValue<short>(out _))
            {
                valueType = typeof(short);
                return true;
            }

            if (value.TryGetValue<decimal>(out _))
            {
                valueType = typeof(decimal);
                return true;
            }

            if (value.TryGetValue<byte>(out _))
            {
                valueType = typeof(byte);
                return true;
            }

            if (value.TryGetValue<float>(out _))
            {
                valueType = typeof(float);
                return true;
            }

            if (value.TryGetValue<uint>(out _))
            {
                valueType = typeof(uint);
                return true;
            }

            if (value.TryGetValue<ushort>(out _))
            {
                valueType = typeof(ushort);
                return true;
            }

            if (value.TryGetValue<ulong>(out _))
            {
                valueType = typeof(ulong);
                return true;
            }

            if (value.TryGetValue<sbyte>(out _))
            {
                valueType = typeof(sbyte);
                return true;
            }

            valueType = null;
            return false;
        }

        private static bool TryGetStringValueKind(JsonValue value, out Type? valueType)
        {
            if (value.TryGetValue<string>(out _))
            {
                valueType = typeof(string);
                return true;
            }

            if (value.TryGetValue<DateTime>(out _))
            {
                valueType = typeof(DateTime);
                return true;
            }

            if (value.TryGetValue<DateTimeOffset>(out _))
            {
                valueType = typeof(DateTimeOffset);
                return true;
            }

            if (value.TryGetValue<Guid>(out _))
            {
                valueType = typeof(Guid);
                return true;
            }

            if (value.TryGetValue<char>(out _))
            {
                valueType = typeof(char);
                return true;
            }

            if (value.TryGetValue<byte[]>(out _))
            {
                valueType = typeof(byte[]);
                return true;
            }

            valueType = null;
            return false;
        }

        public static bool TryCreateValueCached(JsonValue jsonValue, out JsonValueComparisonContext result)
        {
            if (jsonValue.TryGetValue<JsonElement>(out var element))
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.Number:
                        if (element.TryGetInt64(out var longValue))
                        {
                            result = new JsonValueComparisonContext(JsonValueKind.Number, longValue);
                            return true;
                        }

                        if (element.TryGetDecimal(out var decimalValue))
                        {
                            result = new JsonValueComparisonContext(JsonValueKind.Number, decimalValue);
                            return true;
                        }

                        if (element.TryGetDouble(out var doubleValue))
                        {
                            result = new JsonValueComparisonContext(JsonValueKind.Number, doubleValue);
                            return true;
                        }

                        break;

                    case JsonValueKind.String:
                        if (element.TryGetDateTimeOffset(out var dateTimeOffsetValue))
                        {
                            result = new JsonValueComparisonContext(JsonValueKind.String, dateTimeOffsetValue);
                            return true;
                        }

                        if (element.TryGetDateTime(out var dateTimeValue))
                        {
                            result = new JsonValueComparisonContext(JsonValueKind.String, dateTimeValue);
                            return true;
                        }

                        if (element.TryGetGuid(out var guidValue))
                        {
                            result = new JsonValueComparisonContext(JsonValueKind.String, guidValue);
                            return true;
                        }

                        result = new JsonValueComparisonContext(JsonValueKind.String, element.GetString());
                        return true;

                    case JsonValueKind.True:
                    case JsonValueKind.False:
                    case JsonValueKind.Null:
                        result = new JsonValueComparisonContext(element.ValueKind, null);
                        return true;
                }
            }
            else
            {
                var valueObj = jsonValue.GetValue<object>();
                if (valueObj is int or long or double or short or decimal or byte or float or uint or ushort
                    or ulong or sbyte)
                {
                    result = new JsonValueComparisonContext(JsonValueKind.Number, valueObject: valueObj);
                    return true;
                }

                if (valueObj is string or DateTime or DateTimeOffset or Guid or char or byte[])
                {
                    result = new JsonValueComparisonContext(JsonValueKind.String, valueObj);
                    return true;
                }

                if (valueObj is bool boolValue)
                {
                    result = new JsonValueComparisonContext(boolValue ? JsonValueKind.True : JsonValueKind.False, null);
                }
            }

            result = default;
            return false;
        }
    }
}