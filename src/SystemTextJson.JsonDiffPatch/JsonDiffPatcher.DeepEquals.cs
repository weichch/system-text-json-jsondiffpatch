using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.JsonDiffPatch.Comparison;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    static partial class JsonDiffPatcher
    {

        /// <summary>
        /// Determines whether two <see cref="JsonNode"/> objects are deeply equal.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        public static bool DeepEquals(this JsonNode? left, JsonNode? right)
        {
            return DeepEquals(left, right, JsonValueComparer.DefaultEquality);
        }

        /// <summary>
        /// Determines whether two <see cref="JsonNode"/> objects are deeply equal.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <param name="valueComparer">The comparer used to compare two <see cref="JsonValue"/>.</param>
        public static bool DeepEquals(this JsonNode? left, JsonNode? right, IEqualityComparer<JsonValue> valueComparer)
        {
            _ = valueComparer ?? throw new ArgumentNullException(nameof(valueComparer));

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
                JsonValue val1 when right is JsonValue val2 => ValueEquals(val1, val2, valueComparer),
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

        private static bool ValueEquals(JsonValue val1, JsonValue val2, IEqualityComparer<JsonValue> valueComparer)
        {
            var hash1 = valueComparer.GetHashCode(val1);
            var hash2 = valueComparer.GetHashCode(val2);

            return hash1 == hash2 || valueComparer.Equals(val1, val2);
        }
    }
}
