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
            return DeepEquals(left, right, new JsonComparerOptions(default, valueComparer));
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
                JsonObject obj1 when right is JsonObject obj2 => ObjectEquals(obj1, obj2, comparerOptions),
                JsonArray arr1 when right is JsonArray arr2 => ArrayEquals(arr1, arr2, comparerOptions),
                JsonValue val1 when right is JsonValue val2 => ValueEquals(val1, val2, comparerOptions),
                _ => false
            };
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

            return CompareJsonValueWithOptions(val1, val2, comparerOptions);
        }

        private static bool CompareJsonValueWithOptions(JsonValue x, JsonValue y,
            in JsonComparerOptions comparerOptions)
        {
            var kindX = x.GetValueKind(false, out var typeX);
            var kindY = y.GetValueKind(false, out var typeY);

            if (kindX != kindY)
            {
                return false;
            }

            if (kindX is JsonValueKind.Null or JsonValueKind.True or JsonValueKind.False)
            {
                return true;
            }

            if (kindX is not JsonValueKind.Number && kindX is not JsonValueKind.String)
            {
                x.TryGetValue<object>(out var objX);
                y.TryGetValue<object>(out var objY);
                return Equals(objX, objY);
            }

            var isJsonElementX = typeX == typeof(JsonElement);
            var isJsonElementY = typeY == typeof(JsonElement);

            if (comparerOptions.JsonElementComparison is JsonElementComparison.RawText)
            {
                // Happy scenario: both backed by JsonElement
                if (isJsonElementX && isJsonElementY)
                {
                    return kindX is JsonValueKind.String
                        ? x.GetValue<JsonElement>().ValueEquals(y.GetValue<JsonElement>().GetString())
                        : string.Equals(x.GetValue<JsonElement>().GetRawText(),
                            y.GetValue<JsonElement>().GetRawText(),
                            StringComparison.Ordinal);
                }

                if (isJsonElementX && y.TryGetValue<string>(out var stringY))
                {
                    return x.GetValue<JsonElement>().ValueEquals(stringY);
                }

                if (isJsonElementY && x.TryGetValue<string>(out var stringX))
                {
                    return y.GetValue<JsonElement>().ValueEquals(stringX);
                }
            }

            if (isJsonElementX)
            {
                typeX = x.GetValue<JsonElement>().GetValueType();
            }

            if (isJsonElementY)
            {
                typeY = y.GetValue<JsonElement>().GetValueType();
            }

            return JsonValueComparer.Compare(kindX, x, typeX, y, typeY) == 0;
        }
    }
}
