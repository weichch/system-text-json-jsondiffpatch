using System.Globalization;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    internal readonly struct JsonValueComparisonContext
    {
        public JsonValueComparisonContext(JsonValueKind valueKind, JsonValue value, Type? valueType)
        {
            ValueKind = valueKind;
            Value = value;
            ValueObject = null;
            var isJsonElement = valueType == typeof(JsonElement);
            IsJsonElement = isJsonElement;
            var actualValueType = isJsonElement ? GetElementValueType(value.GetValue<JsonElement>()) : valueType;
            ValueType = actualValueType;
            StringValueKind = valueKind == JsonValueKind.String
                ? GetStringValueKind(actualValueType)
                : JsonStringValueKind.String;
        }

        public JsonValueComparisonContext(JsonValueKind valueKind, object? valueObject)
        {
            ValueKind = valueKind;
            Value = null;
            ValueObject = valueObject;
            IsJsonElement = false;
            ValueType = null;
            StringValueKind = valueKind == JsonValueKind.String
                ? GetStringValueKind(valueObject)
                : JsonStringValueKind.String;
        }

        public JsonValueKind ValueKind { get; }
        public JsonValue? Value { get; }
        public object? ValueObject { get; }
        public Type? ValueType { get; }
        public bool IsJsonElement { get; }
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

                if (IsJsonElement)
                {
                    var element = Value.GetValue<JsonElement>();
                    if (element.TryGetBytesFromBase64(out value))
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

        public bool DeepEquals(in JsonValueComparisonContext another)
        {
            if (ValueKind != another.ValueKind)
            {
                return false;
            }

            if (Value is null && another.Value is null)
            {
                return JsonValueComparer.CompareValue(ValueKind, this, another) == 0;
            }

            if (Value is not null && another.Value is not null)
            {
                switch (ValueKind)
                {
                    case JsonValueKind.Number:
                        return JsonValueComparer.Compare(ValueKind, this, another) == 0;

                    case JsonValueKind.String:
                        if (StringValueKind != another.StringValueKind)
                        {
                            return false;
                        }

                        // Compare string when possible
                        if (IsJsonElement && (another.ValueType == typeof(string) || another.ValueType == typeof(char)))
                        {
                            if (another.ValueType == typeof(char))
                            {
                                Span<char> valueY = stackalloc char[1];
                                valueY[0] = another.Value.GetValue<char>();
                                return Value.GetValue<JsonElement>().ValueEquals(valueY);
                            }

                            return Value.GetValue<JsonElement>().ValueEquals(another.Value.GetValue<string>());
                        }

                        if (another.IsJsonElement && (ValueType == typeof(string) || ValueType == typeof(char)))
                        {
                            if (ValueType == typeof(char))
                            {
                                Span<char> valueX = stackalloc char[1];
                                valueX[0] = Value.GetValue<char>();
                                return another.Value.GetValue<JsonElement>().ValueEquals(valueX);
                            }

                            return another.Value.GetValue<JsonElement>().ValueEquals(Value.GetValue<string>());
                        }

                        return JsonValueComparer.Compare(ValueKind, this, another) == 0;

                    case JsonValueKind.Null:
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        return true;

                    case JsonValueKind.Undefined:
                    case JsonValueKind.Object:
                    case JsonValueKind.Array:
                    default:
                        return Value.TryGetValue<object>(out var objX)
                               && another.Value.TryGetValue<object>(out var objY)
                               && Equals(objX, objY);
                }
            }

            return false;
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

        public static bool TryCreateFromValueObject(JsonValue jsonValue, out JsonValueComparisonContext result)
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
                    result = new JsonValueComparisonContext(JsonValueKind.Number, valueObj);
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