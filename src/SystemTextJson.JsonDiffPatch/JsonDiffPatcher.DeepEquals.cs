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
                JsonValue val1 when right is JsonValue val2 => ValueEquals(val1, val2, comparerOptions),
                JsonObject obj1 when right is JsonObject obj2 => ObjectEquals(obj1, obj2, comparerOptions),
                JsonArray arr1 when right is JsonArray arr2 => ArrayEquals(arr1, arr2, comparerOptions),
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
            var kindX = x.GetValueKind(out var typeX);
            var kindY = y.GetValueKind(out var typeY);

            if (kindX != kindY)
            {
                return false;
            }

            // Fast: raw text comparison
            if (comparerOptions.JsonElementComparison is JsonElementComparison.RawText
                && typeX == typeof(JsonElement)
                && typeY == typeof(JsonElement))
            {
                return kindX is JsonValueKind.String
                    ? x.GetValue<JsonElement>().ValueEquals(y.GetValue<JsonElement>().GetString())
                    : string.Equals(x.GetValue<JsonElement>().GetRawText(),
                        y.GetValue<JsonElement>().GetRawText());
            }

            // Slow: semantic comparison
            var contextX = new JsonValueComparisonContext(kindX, x, typeX);
            var contextY = new JsonValueComparisonContext(kindY, y, typeY);
            
            switch (kindX)
            {
                case JsonValueKind.Number:
                    return JsonValueComparer.Compare(kindX, contextX, contextY) == 0;

                case JsonValueKind.String:
                    if (contextX.StringValueKind != contextY.StringValueKind)
                    {
                        return false;
                    }
                    
                    // Compare string when possible
                    if (contextX.IsJsonElement && (typeY == typeof(string) || typeY == typeof(char)))
                    {
                        if (typeY == typeof(char))
                        {
                            Span<char> valueY = stackalloc char[1];
                            valueY[0] = y.GetValue<char>();
                            return x.GetValue<JsonElement>().ValueEquals(valueY);
                        }

                        return x.GetValue<JsonElement>().ValueEquals(y.GetValue<string>());
                    }

                    if (contextY.IsJsonElement && (typeX == typeof(string) || typeX == typeof(char)))
                    {
                        if (typeX == typeof(char))
                        {
                            Span<char> valueX = stackalloc char[1];
                            valueX[0] = x.GetValue<char>();
                            return y.GetValue<JsonElement>().ValueEquals(valueX);
                        }

                        return y.GetValue<JsonElement>().ValueEquals(x.GetValue<string>());
                    }

                    return JsonValueComparer.Compare(kindX, contextX, contextY) == 0;

                case JsonValueKind.Null:
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return true;

                case JsonValueKind.Undefined:
                case JsonValueKind.Object:
                case JsonValueKind.Array:
                default:
                    return x.TryGetValue<object>(out var objX)
                           && y.TryGetValue<object>(out var objY)
                           && Equals(objX, objY);
            }
        }
    }
}
