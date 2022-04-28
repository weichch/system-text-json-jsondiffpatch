using System.Globalization;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    /// <summary>
    /// Wrapper of <see cref="JsonNode"/> for value comparison purpose.
    /// </summary>
    internal struct JsonValueComparisonContext
    {
        private readonly bool _cacheRead;
        private readonly bool _isJsonElement;
        private readonly JsonElement _element;
        private object? _valueObject;
        private string? _rawText;

        public JsonValueComparisonContext(JsonValue value, bool cacheRead = false)
        {
            _cacheRead = cacheRead;
            var isJsonElement = value.TryGetValue(out _element);
            _isJsonElement = isJsonElement;
            Value = value;
            Type? valueType = null;
            ValueKind = isJsonElement ? _element.ValueKind : GetValueKind(value, out valueType);
            var actualValueType = isJsonElement ? GetElementValueType(_element) : valueType;
            ValueType = actualValueType;
            StringValueKind = GetStringValueKind(actualValueType);

            _valueObject = null;
            _rawText = null;
        }

        public JsonValueKind ValueKind { get; }
        public JsonValue Value { get; }
        public Type? ValueType { get; }
        public JsonStringValueKind StringValueKind { get; }

        private T ReadValue<T>()
        {
            var valueObj = Value.GetValue<T>();

            if (_cacheRead && _valueObject is null)
            {
                _valueObject = valueObj;
            }

            return valueObj;
        }

        public decimal GetDecimal()
        {
            if (_valueObject is not null)
            {
                return Convert.ToDecimal(_valueObject, CultureInfo.InvariantCulture);
            }

            if (ValueType == typeof(long))
                return Convert.ToDecimal(ReadValue<long>());
            if (ValueType == typeof(decimal))
                return ReadValue<decimal>();
            if (ValueType == typeof(double))
                return Convert.ToDecimal(ReadValue<double>());
            if (ValueType == typeof(int))
                return Convert.ToDecimal(ReadValue<int>());
            if (ValueType == typeof(short))
                return Convert.ToDecimal(ReadValue<short>());
            if (ValueType == typeof(byte))
                return Convert.ToDecimal(ReadValue<byte>());
            if (ValueType == typeof(float))
                return Convert.ToDecimal(ReadValue<float>());
            if (ValueType == typeof(uint))
                return Convert.ToDecimal(ReadValue<uint>());
            if (ValueType == typeof(ulong))
                return Convert.ToDecimal(ReadValue<ulong>());
            if (ValueType == typeof(ushort))
                return Convert.ToDecimal(ReadValue<ushort>());
            if (ValueType == typeof(sbyte))
                return Convert.ToDecimal(ReadValue<sbyte>());

            throw new ArgumentOutOfRangeException($"Unsupported value type '{ValueType?.FullName ?? "null"}'.");
        }

        public double GetDouble()
        {
            if (_valueObject is not null)
            {
                return Convert.ToDouble(_valueObject, CultureInfo.InvariantCulture);
            }

            if (ValueType == typeof(long))
                return Convert.ToDouble(ReadValue<long>());
            if (ValueType == typeof(decimal))
                return Convert.ToDouble(ReadValue<decimal>());
            if (ValueType == typeof(double))
                return ReadValue<double>();
            if (ValueType == typeof(int))
                return Convert.ToDouble(ReadValue<int>());
            if (ValueType == typeof(short))
                return Convert.ToDouble(ReadValue<short>());
            if (ValueType == typeof(byte))
                return Convert.ToDouble(ReadValue<byte>());
            if (ValueType == typeof(float))
                return Convert.ToDouble(ReadValue<float>());
            if (ValueType == typeof(uint))
                return Convert.ToDouble(ReadValue<uint>());
            if (ValueType == typeof(ulong))
                return Convert.ToDouble(ReadValue<ulong>());
            if (ValueType == typeof(ushort))
                return Convert.ToDouble(ReadValue<ushort>());
            if (ValueType == typeof(sbyte))
                return Convert.ToDouble(ReadValue<sbyte>());

            throw new ArgumentOutOfRangeException($"Unsupported value type '{ValueType?.FullName ?? "null"}'.");
        }

        public long GetInt64()
        {
            if (_valueObject is not null)
            {
                return Convert.ToInt64(_valueObject, CultureInfo.InvariantCulture);
            }

            if (ValueType == typeof(long))
                return ReadValue<long>();
            if (ValueType == typeof(decimal))
                return Convert.ToInt64(ReadValue<decimal>());
            if (ValueType == typeof(double))
                return Convert.ToInt64(ReadValue<double>());
            if (ValueType == typeof(int))
                return Convert.ToInt64(ReadValue<int>());
            if (ValueType == typeof(short))
                return Convert.ToInt64(ReadValue<short>());
            if (ValueType == typeof(byte))
                return Convert.ToInt64(ReadValue<byte>());
            if (ValueType == typeof(float))
                return Convert.ToInt64(ReadValue<float>());
            if (ValueType == typeof(uint))
                return Convert.ToInt64(ReadValue<uint>());
            if (ValueType == typeof(ulong))
                return Convert.ToInt64(ReadValue<ulong>());
            if (ValueType == typeof(ushort))
                return Convert.ToInt64(ReadValue<ushort>());
            if (ValueType == typeof(sbyte))
                return Convert.ToInt64(ReadValue<sbyte>());

            throw new ArgumentOutOfRangeException($"Unsupported value type '{ValueType?.FullName ?? "null"}'.");
        }

        public DateTime GetDateTime()
        {
            if (_valueObject is not null)
            {
                if (_valueObject is DateTime dateTime)
                {
                    return dateTime;
                }

                if (_valueObject is DateTimeOffset dateTimeOffset)
                {
                    return dateTimeOffset.DateTime;
                }

                return Convert.ToDateTime(_valueObject, CultureInfo.InvariantCulture);
            }

            if (ValueType == typeof(DateTime))
            {
                return ReadValue<DateTime>();
            }

            if (ValueType == typeof(DateTimeOffset))
            {
                return ReadValue<DateTimeOffset>().DateTime;
            }

            return Convert.ToDateTime(ReadValue<object>(), CultureInfo.InvariantCulture);
        }

        public DateTimeOffset GetDateTimeOffset()
        {
            if (_valueObject is not null)
            {
                if (_valueObject is DateTimeOffset dateTimeOffset)
                {
                    return dateTimeOffset;
                }

                if (_valueObject is DateTime dateTime)
                {
                    return new DateTimeOffset(dateTime);
                }

                return new DateTimeOffset(Convert.ToDateTime(_valueObject, CultureInfo.InvariantCulture));
            }

            if (ValueType == typeof(DateTimeOffset))
            {
                return ReadValue<DateTimeOffset>();
            }

            if (ValueType == typeof(DateTime))
            {
                return new DateTimeOffset(ReadValue<DateTime>());
            }

            return new DateTimeOffset(Convert.ToDateTime(ReadValue<object>(), CultureInfo.InvariantCulture));
        }

        private char GetChar() => _valueObject is not null
            ? (char) _valueObject
            : ReadValue<char>();

        public Guid GetGuid() => _valueObject is not null
            ? (Guid) _valueObject
            : ReadValue<Guid>();

        public byte[] GetByteArray() => _valueObject is not null
            ? (byte[]) _valueObject
            : ReadValue<byte[]>();

        public bool TryGetByteArray(out byte[]? value)
        {
            if (_valueObject is not null)
            {
                value = (byte[]) _valueObject;
                return true;
            }

            if (ValueType == typeof(byte[]))
            {
                value = ReadValue<byte[]>();
                return true;
            }

            if (_isJsonElement)
            {
                if (_element.TryGetBytesFromBase64(out value))
                {
                    if (_cacheRead)
                    {
                        _valueObject = value;
                    }

                    return true;
                }
            }

            value = null;
            return false;
        }

        public string? GetString()
        {
            if (_valueObject is not null)
            {
                if (_valueObject is string str)
                {
                    return str;
                }

                if (_valueObject is char charValue)
                {
                    return charValue.ToString();
                }

                if (_valueObject is byte[] byteArray)
                {
                    return Convert.ToBase64String(byteArray);
                }

                return Convert.ToString(_valueObject, CultureInfo.InvariantCulture);
            }

            if (ValueType == typeof(string))
            {
                return ReadValue<string>();
            }

            if (ValueType == typeof(char))
            {
                return ReadValue<char>().ToString();
            }

            if (ValueType == typeof(byte[]))
            {
                return Convert.ToBase64String(ReadValue<byte[]>());
            }

            return Convert.ToString(ReadValue<object>(), CultureInfo.InvariantCulture);
        }

        public string GetRawText()
        {
            if (_rawText is not null)
            {
                return _rawText;
            }

            var rawText = _isJsonElement ? _element.GetRawText() : Value.ToJsonString();
            if (_cacheRead)
            {
                _rawText = rawText;
            }

            return rawText;
        }

        private bool StringValueEquals(string? text) => _element.ValueEquals(text);

        private bool StringValueEquals(in ReadOnlySpan<char> text) => _element.ValueEquals(text);

        public bool DeepEquals(ref JsonValueComparisonContext another, in JsonComparerOptions comparerOptions)
        {
            var valueComparer = comparerOptions.ValueComparer;
            if (valueComparer is not null)
            {
                var hash1 = valueComparer.GetHashCode(Value);
                var hash2 = valueComparer.GetHashCode(another.Value);

                if (hash1 != hash2)
                {
                    return false;
                }

                return valueComparer.Equals(Value, another.Value);
            }

            return DeepEquals(ref another, comparerOptions.JsonElementComparison);
        }

        public bool DeepEquals(ref JsonValueComparisonContext another, JsonElementComparison jsonElementComparison)
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
            if (jsonElementComparison is JsonElementComparison.RawText
                && _isJsonElement
                && another._isJsonElement)
            {
                return ValueKind switch
                {
                    JsonValueKind.String => StringValueEquals(another.GetString()),
                    _ => string.Equals(GetRawText(), another.GetRawText())
                };
            }

            // Slow: value semantic comparison
            switch (ValueKind)
            {
                case JsonValueKind.Number:
                    return JsonValueComparer.Compare(ValueKind, ref this, ref another) == 0;

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
                        (another.ValueType == typeof(string) || another.ValueType == typeof(char) ||
                         another.ValueType == typeof(byte[])))
                    {
                        if (another.ValueType == typeof(char))
                        {
                            Span<char> valueY = stackalloc char[1];
                            valueY[0] = another.GetChar();
                            return StringValueEquals(valueY);
                        }

                        return StringValueEquals(another.GetString());
                    }

                    if (another._isJsonElement &&
                        (ValueType == typeof(string) || ValueType == typeof(char) || ValueType == typeof(byte[])))
                    {
                        if (ValueType == typeof(char))
                        {
                            Span<char> valueX = stackalloc char[1];
                            valueX[0] = GetChar();
                            return another.StringValueEquals(valueX);
                        }

                        return another.StringValueEquals(GetString());
                    }

                    return JsonValueComparer.Compare(ValueKind, ref this, ref another) == 0;

                default:
                    return Value.TryGetValue<object>(out var objX)
                           && another.Value.TryGetValue<object>(out var objY)
                           && Equals(objX, objY);
            }
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

        private static JsonValueKind GetValueKind(JsonValue value, out Type? valueType)
        {
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
    }
}