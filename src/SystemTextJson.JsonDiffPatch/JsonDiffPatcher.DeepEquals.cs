using System.Collections.Generic;
using System.Diagnostics;
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
            return DeepEquals(left, right, default(JsonComparerOptions));
        }

        /// <summary>
        /// Determines whether two <see cref="JsonNode"/> objects are deeply equal.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <param name="jsonElementComparison">The JSON element comparison.</param>
        public static bool DeepEquals(this JsonNode? left, JsonNode? right, JsonElementComparison jsonElementComparison)
        {
            return DeepEquals(left, right, new JsonComparerOptions(jsonElementComparison, null));
        }

        /// <summary>
        /// Determines whether two <see cref="JsonNode"/> objects are deeply equal.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <param name="valueComparer">The JSON value comparer.</param>
        public static bool DeepEquals(this JsonNode? left, JsonNode? right, IEqualityComparer<JsonValue> valueComparer)
        {
            _ = valueComparer ?? throw new ArgumentNullException(nameof(valueComparer));
            return DeepEquals(left, right, new JsonComparerOptions(null, valueComparer));
        }

        /// <summary>
        /// Determines whether two <see cref="JsonNode"/> objects are deeply equal.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <param name="comparerOptions">The value comparer options.</param>
        public static bool DeepEquals(this JsonNode? left, JsonNode? right, in JsonComparerOptions comparerOptions)
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
                JsonValue val1 when right is JsonValue val2 => ValueEquals(val1, val2, comparerOptions),
                JsonObject obj1 when right is JsonObject obj2 => ObjectEquals(obj1, obj2, comparerOptions),
                JsonArray arr1 when right is JsonArray arr2 => ArrayEquals(arr1, arr2, comparerOptions),
                _ => false
            };
        }

        /// <summary>
        /// Determines whether two <see cref="JsonElement"/> objects are deeply equal.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <param name="jsonElementComparison">The JSON element comparison.</param>
        public static bool DeepEquals(this JsonDocument? left, JsonDocument? right,
            JsonElementComparison? jsonElementComparison = null)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return left.RootElement.DeepEquals(right.RootElement, jsonElementComparison);
        }

        /// <summary>
        /// Determines whether two <see cref="JsonElement"/> objects are deeply equal.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <param name="jsonElementComparison">The JSON element comparison.</param>
        public static bool DeepEquals(this in JsonElement left, in JsonElement right,
            JsonElementComparison? jsonElementComparison = null)
        {
            if (left.ValueKind != right.ValueKind)
            {
                return false;
            }

            jsonElementComparison ??= DefaultDeepEqualsComparison;

            switch (left.ValueKind)
            {
                case JsonValueKind.Number:
                    return jsonElementComparison is JsonElementComparison.RawText
                        ? string.Equals(left.GetRawText(), right.GetRawText())
                        : new JsonNumber(left).CompareTo(new JsonNumber(right)) == 0;

                case JsonValueKind.String:
                    return jsonElementComparison is JsonElementComparison.RawText
                        ? left.ValueEquals(right.GetString())
                        : new JsonString(left).ValueEquals(new JsonString(right));
                
                case JsonValueKind.Object:
                    return ObjectEquals(left, right, jsonElementComparison.Value);

                case JsonValueKind.Array:
                    return ArrayEquals(left, right, jsonElementComparison.Value);
                
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return true;

                default:
                    throw new ArgumentOutOfRangeException(nameof(left.ValueKind),
                        $"Unexpected JSON value kind {left.ValueKind:G}.");
            }
        }

        private static bool ObjectEquals(JsonObject obj1, JsonObject obj2, in JsonComparerOptions comparerOptions)
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

                if (!DeepEquals(obj1Value, obj2Value, comparerOptions))
                {
                    // Value not equal
                    return false;
                }
            }

            return true;
        }

        private static bool ObjectEquals(in JsonElement x, in JsonElement y, JsonElementComparison elementComparison)
        {
            EnumerateProperty(x, out var propertiesX);
            EnumerateProperty(y, out var propertiesY);

            if (propertiesX is null && propertiesY is null)
            {
                return true;
            }

            if (propertiesX is null || propertiesY is null)
            {
                return false;
            }

            if (propertiesX.Count != propertiesY.Count)
            {
                return false;
            }

            foreach (var kvp in propertiesX)
            {
                if (!propertiesY.TryGetValue(kvp.Key, out var propertyY))
                {
                    return false;
                }

                if (!kvp.Value.Value.DeepEquals(propertyY.Value, elementComparison))
                {
                    return false;
                }
            }

            return true;

            static void EnumerateProperty(in JsonElement element, out Dictionary<string, JsonProperty>? properties)
            {
                properties = null;

                foreach (var property in element.EnumerateObject())
                {
                    if (properties is null)
                    {
                        properties = new Dictionary<string, JsonProperty>();
                    }

                    properties[property.Name] = property;
                }
            }
        }

        private static bool ArrayEquals(JsonArray arr1, JsonArray arr2, in JsonComparerOptions comparerOptions)
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
                if (!DeepEquals(arr1[i], arr2[i], comparerOptions))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ArrayEquals(in JsonElement x, in JsonElement y, JsonElementComparison elementComparison)
        {
            if (x.GetArrayLength() != y.GetArrayLength())
            {
                return false;
            }

            using var enumeratorX = x.EnumerateArray();
            using var enumeratorY = y.EnumerateArray();

            while (enumeratorX.MoveNext())
            {
                if (!enumeratorY.MoveNext() ||
                    !enumeratorX.Current.DeepEquals(enumeratorY.Current, elementComparison))
                {
                    return false;
                }
            }

            Debug.Assert(enumeratorX.MoveNext() == false);
            Debug.Assert(enumeratorY.MoveNext() == false);

            return true;
        }

        private static bool ValueEquals(JsonValue val1, JsonValue val2, in JsonComparerOptions comparerOptions)
        {
            var valueComparer = comparerOptions.ValueComparer;
            if (valueComparer is not null)
            {
                var hash1 = valueComparer.GetHashCode(val1);
                var hash2 = valueComparer.GetHashCode(val2);

                if (hash1 != hash2)
                {
                    return false;
                }

                return valueComparer.Equals(val1, val2);
            }

            var ctx1 = new JsonValueComparisonContext(val1);
            var ctx2 = new JsonValueComparisonContext(val2);
            return ctx1.DeepEquals(ref ctx2, comparerOptions.JsonElementComparison);
        }
    }
}
