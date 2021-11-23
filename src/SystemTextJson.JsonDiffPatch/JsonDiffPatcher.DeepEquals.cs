using System.Diagnostics;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    static partial class JsonDiffPatcher
    {
        /// <summary>
        /// Determines whether two <see cref="JsonNode"/> objects are deeply equal.
        /// </summary>
        public static bool DeepEquals(this JsonNode? left, JsonNode? right)
        {
            Debug.Assert(left is null or JsonObject or JsonArray or JsonValue);
            Debug.Assert(right is null or JsonObject or JsonArray or JsonValue);

            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return left switch
            {
                JsonObject obj1 when right is JsonObject obj2 => ObjectEquals(obj1, obj2),
                JsonArray arr1 when right is JsonArray arr2 => ArrayEquals(arr1, arr2),
                JsonValue val1 when right is JsonValue val2 => ValueEquals(val1, val2),
                _ => false
            };
        }

        private static bool ObjectEquals(JsonObject obj1, JsonObject obj2)
        {
            if (obj1.Count == 0 && obj2.Count == 0)
            {
                // Empty objects
                return true;
            }

            if (obj1.Count != obj2.Count)
            {
                // Property count mismatch
                return false;
            }

            foreach (var kvp in obj1)
            {
                var propertyName = kvp.Key;
                var obj1Value = kvp.Value;

                if (!obj2.TryGetPropertyValue(propertyName, out var obj2Value))
                {
                    // Missing property
                    return false;
                }

                if (!DeepEquals(obj1Value, obj2Value))
                {
                    // Value not equal
                    return false;
                }
            }

            return true;
        }

        private static bool ArrayEquals(JsonArray arr1, JsonArray arr2)
        {
            if (arr1.Count == 0 && arr2.Count == 0)
            {
                return true;
            }

            if (arr1.Count != arr2.Count)
            {
                // Item count mismatch
                return false;
            }

            for (var i = 0; i < arr1.Count; i++)
            {
                if (!DeepEquals(arr1[i], arr2[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ValueEquals(JsonValue val1, JsonValue val2)
        {
            var ret1 = val1.TryGetValue<JsonElement>(out var e1);
            var ret2 = val2.TryGetValue<JsonElement>(out var e2);

            // Happy scenario: both backed by JsonElement
            if (ret1 && ret2)
            {
                if (e1.ValueKind != e2.ValueKind)
                {
                    return false;
                }

                if (e1.ValueKind is JsonValueKind.Object or JsonValueKind.Array)
                {
                    // We shouldn't have those two value kinds, but if we do,
                    // just aggressively return, this should be picked by in debugging
                    Debug.Assert(false);
                    return false;
                }

                // Perf: If the values are backed by JsonElement, we need to materialize the values
                // and compare raw text because there is no way to compare the bytes.
                // This may consume a lot memory for large JSON objects.
                return e1.ValueKind switch
                {
                    JsonValueKind.String => e1.ValueEquals(e2.GetString()),
                    _ => Equals(e1.GetRawText(), e2.GetRawText())
                };
            }

            if (ret1 || ret2)
            {
                // Perf: This is slower than direct property access
                var capturedValue = ret1 ? val2.GetValue<object>() : val1.GetValue<object>();
                var element = ret1 ? e1 : e2;
                return CompareJsonElementWithObject(element, capturedValue);
            }

            // Perf: This is slower than direct property access
            var innerValue1 = val1.GetValue<object>();
            var innerValue2 = val2.GetValue<object>();

            // For perf reasons, we don't support unboxing and overloaded operators
            return Equals(innerValue1, innerValue2);
        }

        private static bool CompareJsonElementWithObject(JsonElement element, object objectValue)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.False:
                case JsonValueKind.True:
                    return objectValue is bool booleanValue
                           && booleanValue == (element.ValueKind == JsonValueKind.True);

                case JsonValueKind.String:

                    switch (objectValue)
                    {
                        case DateTime or DateTimeOffset:
                        {
                            if (element.TryGetDateTimeOffset(out var dateTimeOffSetValue))
                            {
                                return objectValue switch
                                {
                                    DateTime dateTime => dateTimeOffSetValue.Equals((DateTimeOffset) dateTime),
                                    DateTimeOffset dateTimeOffset => dateTimeOffSetValue.Equals(dateTimeOffset),
                                    _ => false
                                };
                            }

                            if (element.TryGetDateTime(out var dateTimeValue))
                            {
                                return objectValue switch
                                {
                                    DateTime dateTime => dateTimeValue.Equals(dateTime),
                                    DateTimeOffset dateTimeOffset => dateTimeValue.Equals(dateTimeOffset.DateTime),
                                    _ => false
                                };
                            }

                            return false;
                        }
                        case Guid guid:
                            return element.TryGetGuid(out var guidValue) && guidValue.Equals(guid);
                        case byte[] bytes:
                            return element.TryGetBytesFromBase64(out var bytesValue)
                                   && bytes.AsSpan() == bytesValue.AsSpan();
                    }

                    var strValue = element.GetString();
                    if (strValue is null)
                    {
                        return false;
                    }

                    return objectValue switch
                    {
                        char charValue => strValue.Length == 1 && charValue == strValue[0],
                        string str => string.Equals(str, strValue, StringComparison.Ordinal),
                        _ => false
                    };

                case JsonValueKind.Number:

                    // For perf reasons, we don't support converting value, e.g. int -> long, or float -> double
                    return objectValue switch
                    {
                        byte actualValue => element.TryGetByte(out var value) && actualValue.Equals(value),
                        decimal actualValue => element.TryGetDecimal(out var value) && actualValue.Equals(value),
                        double actualValue => element.TryGetDouble(out var value) && actualValue.Equals(value),
                        short actualValue => element.TryGetInt16(out var value) && actualValue.Equals(value),
                        int actualValue => element.TryGetInt32(out var value) && actualValue.Equals(value),
                        long actualValue => element.TryGetInt64(out var value) && actualValue.Equals(value),
                        sbyte actualValue => element.TryGetSByte(out var value) && actualValue.Equals(value),
                        float actualValue => element.TryGetSingle(out var value) && actualValue.Equals(value),
                        ushort actualValue => element.TryGetUInt16(out var value) && actualValue.Equals(value),
                        uint actualValue => element.TryGetUInt32(out var value) && actualValue.Equals(value),
                        ulong actualValue => element.TryGetUInt64(out var value) && actualValue.Equals(value),
                        _ => false
                    };

                // We won't have JsonValue created from null, array or object
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                case JsonValueKind.Array:
                case JsonValueKind.Object:
                    Debug.Assert(false);
                    return false;
                default:
                    return false;
            }
        }
    }
}
