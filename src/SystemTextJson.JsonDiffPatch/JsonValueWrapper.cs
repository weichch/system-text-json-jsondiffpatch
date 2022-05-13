using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    /// <summary>
    /// Wrapper of <see cref="JsonNode"/> for value comparison purpose.
    /// </summary>
    internal struct JsonValueWrapper
    {
        // Keep as fields to avoid copy
        public JsonNumber NumberValue;
        public JsonString StringValue;
        
        public JsonValueWrapper(JsonValue value)
        {
            Value = value;
            ValueKind = JsonValueKind.Undefined;
            StringValue = default;

            if (JsonNumber.TryGetJsonNumber(value, out NumberValue))
            {
                ValueKind = JsonValueKind.Number;
            }
            else if (JsonString.TryGetJsonString(value, out StringValue))
            {
                ValueKind = JsonValueKind.String;
            }
            else if (value.TryGetValue<bool>(out var booleanValue))
            {
                ValueKind = booleanValue ? JsonValueKind.True : JsonValueKind.False;
            }
        }
        
        public JsonValueKind ValueKind { get; }
        public JsonValue Value { get; }

        public bool DeepEquals(ref JsonValueWrapper another, in JsonComparerOptions comparerOptions)
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

        public bool DeepEquals(ref JsonValueWrapper another, JsonElementComparison jsonElementComparison)
        {
            if (ReferenceEquals(Value, another.Value))
            {
                return true;
            }
            
            if (ValueKind != another.ValueKind)
            {
                return false;
            }

            switch (ValueKind)
            {
                case JsonValueKind.Number:
                    if (jsonElementComparison is JsonElementComparison.RawText &&
                        NumberValue.HasElement &&
                        another.NumberValue.HasElement)
                    {
                        return NumberValue.RawTextEquals(ref another.NumberValue);
                    }

                    return NumberValue.CompareTo(ref another.NumberValue) == 0;

                case JsonValueKind.String:
                    if (jsonElementComparison is JsonElementComparison.RawText &&
                        StringValue.HasElement &&
                        another.StringValue.HasElement)
                    {
                        return StringValue.ValueEquals(ref another.StringValue);
                    }

                    return StringValue.Equals(ref another.StringValue);

                case JsonValueKind.True:
                case JsonValueKind.False:
                    return true;

                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                case JsonValueKind.Object:
                case JsonValueKind.Array:
                default:
                    return Value.TryGetValue<object>(out var objX)
                           && another.Value.TryGetValue<object>(out var objY)
                           && Equals(objX, objY);
            }
        }

        public int CompareTo(ref JsonValueWrapper another)
        {
            if (ValueKind != another.ValueKind)
            {
                return -((int) ValueKind - (int) another.ValueKind);
            }

            switch (ValueKind)
            {
                case JsonValueKind.Number:
                    return NumberValue.CompareTo(ref another.NumberValue);

                case JsonValueKind.String:
                    return StringValue.CompareTo(ref another.StringValue);

                case JsonValueKind.True:
                case JsonValueKind.False:
                    return 0;
                
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                case JsonValueKind.Object:
                case JsonValueKind.Array:
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(ValueKind), $"Unexpected value kind {ValueKind:G}");
            }
        }
    }
}
