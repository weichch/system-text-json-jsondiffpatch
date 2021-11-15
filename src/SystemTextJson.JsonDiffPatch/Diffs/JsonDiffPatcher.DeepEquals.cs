using System.Diagnostics;
using System.Text.Json.Nodes;

namespace System.Text.Json
{
    static partial class JsonDiffPatcher
    {
        /// <summary>
        /// Determines whether two <see cref="JsonNode"/> objects are deeply equal.
        /// </summary>
        public static bool DeepEquals(this JsonNode? node, JsonNode? another)
        {
            Debug.Assert(node is null or JsonObject or JsonArray or JsonValue);
            Debug.Assert(another is null or JsonObject or JsonArray or JsonValue);

            if (Equals(node, another))
            {
                return true;
            }

            if (node is null || another is null)
            {
                return false;
            }

            return node switch
            {
                JsonObject obj1 when another is JsonObject obj2 => ObjectEquals(obj1, obj2),
                JsonArray arr1 when another is JsonArray arr2 => ArrayEquals(arr1, arr2),
                JsonValue val1 when another is JsonValue val2 => ValueEquals(val1, val2),
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
            var ret1 = val1.TryGetValue<object>(out var innerValue1);
            Debug.Assert(ret1);
            var ret2 = val2.TryGetValue<object>(out var innerValue2);
            Debug.Assert(ret2);

            if (innerValue1 is JsonElement element1 && innerValue2 is JsonElement element2)
            {
                if (element1.ValueKind != element2.ValueKind)
                {
                    return false;
                }

                switch (element1.ValueKind)
                {
                    case JsonValueKind.False:
                    case JsonValueKind.True:
                        return element1.GetBoolean().Equals(element2.GetBoolean());
                    case JsonValueKind.String:
                        return CompareStringElement(element1, element2);
                    case JsonValueKind.Number:
                        return CompareNumberElement(element1, element2);
                    case JsonValueKind.Null:
                    case JsonValueKind.Undefined:
                        return true;
                    // The two below should not happen and it's not efficient comparison
                    case JsonValueKind.Object:
                        return DeepEquals(JsonObject.Create(element1), JsonObject.Create(element2));
                    case JsonValueKind.Array:
                        return DeepEquals(JsonArray.Create(element1), JsonArray.Create(element2));
                    default:
                        Debug.Assert(false, $"Unknown value kind '{element1.ValueKind}'.");
                        break;
                }
            }

            // We don't have JSON value type, fallback to object equals
            // This do not support unboxing objects and use overloaded operators
            return Equals(innerValue1, innerValue2);
        }

        private static bool CompareStringElement(JsonElement element1, JsonElement element2)
        {
            if (element1.TryGetDateTimeOffset(out var d1)
                && element2.TryGetDateTimeOffset(out var d2))
            {
                return d1.Equals(d2);
            }

            if (element1.TryGetDateTime(out var dt1)
                && element2.TryGetDateTime(out var dt2))
            {
                return dt1.Equals(dt2);
            }

            if (element1.TryGetGuid(out var g1)
                && element2.TryGetGuid(out var g2))
            {
                return g1.Equals(g2);
            }

            return string.Equals(element1.GetString(), element2.GetString(), StringComparison.Ordinal);
        }

        private static bool CompareNumberElement(JsonElement element1, JsonElement element2)
        {
            // We can't know the exact type of the number in CLR, so decimal is used as the first choice
            // This might cause precision loss for doubles passed as numbers if the underlying parse
            // method in System.Text.Json allows this conversion
            if (element1.TryGetDecimal(out var m1) && element2.TryGetDecimal(out var m2))
            {
                return m1.Equals(m2);
            }

            // If numbers are not decimal, try doubles
            if (element1.TryGetDouble(out var d1) && element2.TryGetDouble(out var d2))
            {
                return d1.Equals(d2);
            }
            
            return false;
        }
    }
}
