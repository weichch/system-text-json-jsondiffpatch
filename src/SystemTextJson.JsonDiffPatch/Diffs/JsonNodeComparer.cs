using System.Diagnostics;
using System.Text.Json.Nodes;

namespace System.Text.Json.Diffs
{
    internal static class JsonNodeComparer
    {
        public static bool AreEqual(JsonNode? node1, JsonNode? node2)
        {
            Debug.Assert(node1 is null or JsonObject or JsonArray or JsonValue);
            Debug.Assert(node2 is null or JsonObject or JsonArray or JsonValue);

            if (Equals(node1, node2))
            {
                return true;
            }

            if (node1 is null || node2 is null)
            {
                return false;
            }

            return node1 switch
            {
                JsonObject obj1 when node2 is JsonObject obj2 => ObjectEquals(obj1, obj2),
                JsonArray arr1 when node2 is JsonArray arr2 => ArrayEquals(arr1, arr2),
                JsonValue val1 when node2 is JsonValue val2 => ValueEquals(val1, val2),
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

                if (!AreEqual(obj1Value, obj2Value))
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
                if (!AreEqual(arr1[i], arr2[i]))
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
                        return element1.GetBoolean().CompareTo(element2.GetBoolean()) == 0;
                    case JsonValueKind.String:
                        return string.Equals(element1.GetString(), element2.GetString(), StringComparison.Ordinal);
                    case JsonValueKind.Number:
                        // We can't know the exact type of the number in CLR, so decimal is used
                        // This might cause precision loss for doubles passed as numbers
                        return element1.GetDecimal().CompareTo(element2.GetDecimal()) == 0;
                    case JsonValueKind.Null:
                    case JsonValueKind.Undefined:
                        return true;
                    // The two below should not happen and it's not efficient comparison
                    case JsonValueKind.Object:
                        return AreEqual(JsonObject.Create(element1), JsonObject.Create(element2));
                    case JsonValueKind.Array:
                        return AreEqual(JsonArray.Create(element1), JsonArray.Create(element2));
                    default:
                        Debug.Assert(false, $"Unknown value kind '{element1.ValueKind}'.");
                        break;
                }
            }

            // We don't have JSON value type, fallback to object equals
            // This do not support unboxing objects and use overloaded operators
            return Equals(innerValue1, innerValue2);
        }
    }
}
