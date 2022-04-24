using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    internal readonly ref struct JsonValueComparisonContext
    {
        public JsonValueComparisonContext(JsonValueKind valueKind, JsonValue value, Type? valueType, bool isJsonElement)
        {
            Value = value;
            ValueType = valueType;
            IsJsonElement = isJsonElement;
            StringValueKind = valueKind == JsonValueKind.String
                ? GetStringValueKind(valueType)
                : JsonStringValueKind.String;
        }

        public JsonValue Value { get; }
        public Type? ValueType { get; }
        public bool IsJsonElement { get; }
        public JsonStringValueKind StringValueKind { get; }

        public decimal GetDecimal()
        {
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

        public DateTime GetDateTime() => Value.GetValue<DateTime>();

        public DateTimeOffset GetDateTimeOffset() => Value.GetValue<DateTimeOffset>();

        public Guid GetGuid() => Value.GetValue<Guid>();

        public char GetChar() => Value.GetValue<char>();

        public byte[] GetByteArray() => Value.GetValue<byte[]>();

        public bool TryGetByteArray(out byte[]? value)
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

            value = null;
            return false;
        }

        public string GetString()
        {
            if (ValueType == typeof(string))
            {
                return Value.GetValue<string>();
            }

            if (ValueType == typeof(char))
            {
                return Value.GetValue<char>().ToString();
            }

            if (Value.TryGetValue<string>(out var strValue))
            {
                return strValue;
            }

            return Value.ToJsonString();
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

            if (valueType == typeof(byte[]))
            {
                return JsonStringValueKind.Bytes;
            }

            return JsonStringValueKind.String;
        }
    }
}