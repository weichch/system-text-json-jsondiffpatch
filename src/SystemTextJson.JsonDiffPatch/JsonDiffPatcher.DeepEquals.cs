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
#if HAVE_NEW_JSONNODE_METHODS
            return JsonNode.DeepEquals(left, right);
#else
            return DeepEquals(left, right, default(JsonComparerOptions));
#endif
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
        internal static bool DeepEquals(this JsonNode? left, JsonNode? right, in JsonComparerOptions comparerOptions)
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
        /// <param name="elementComparison">The JSON element comparison.</param>
        public static bool DeepEquals(this JsonDocument? left, JsonDocument? right,
            JsonElementComparison? elementComparison = null)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return left.RootElement.DeepEquals(right.RootElement, elementComparison);
        }

        /// <summary>
        /// Determines whether two <see cref="JsonElement"/> objects are deeply equal.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <param name="elementComparison">The JSON element comparison.</param>
        public static bool DeepEquals(this in JsonElement left, in JsonElement right,
            JsonElementComparison? elementComparison = null)
        {
            if (left.ValueKind != right.ValueKind)
            {
                return false;
            }

            elementComparison ??= DefaultComparison;

            switch (left.ValueKind)
            {
                case JsonValueKind.Number:
                    var leftNumber = new JsonNumber(left);
                    var rightNumber = new JsonNumber(right);

                    return elementComparison is JsonElementComparison.RawText
                        ? leftNumber.RawTextEquals(ref rightNumber)
                        : leftNumber.CompareTo(ref rightNumber) == 0;

                case JsonValueKind.String:
                    var leftString = new JsonString(left);
                    var rightString = new JsonString(right);

                    return elementComparison is JsonElementComparison.RawText
                        ? leftString.ValueEquals(ref rightString)
                        : leftString.Equals(ref rightString);
                
                case JsonValueKind.Object:
                    return ObjectEquals(left, right, elementComparison.Value);

                case JsonValueKind.Array:
                    return ArrayEquals(left, right, elementComparison.Value);
                
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

        private static bool ObjectEquals(JsonObject x, JsonObject y, in JsonComparerOptions comparerOptions)
        {
            if (x.Count == 0 && y.Count == 0)
            {
                // Empty objects
                return true;
            }

            if (x.Count != y.Count)
            {
                // Property count mismatch
                return false;
            }

            foreach (var kvp in x)
            {
                var propertyName = kvp.Key;
                var obj1Value = kvp.Value;

                if (!y.TryGetPropertyValue(propertyName, out var obj2Value))
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

        private static bool ArrayEquals(JsonArray x, JsonArray y, in JsonComparerOptions comparerOptions)
        {
            if (x.Count == 0 && y.Count == 0)
            {
                return true;
            }

            if (x.Count != y.Count)
            {
                // Item count mismatch
                return false;
            }

            for (var i = 0; i < x.Count; i++)
            {
                if (!DeepEquals(x[i], y[i], comparerOptions))
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

        private static bool ValueEquals(JsonValue x, JsonValue y, in JsonComparerOptions comparerOptions)
        {
            var valueComparer = comparerOptions.ValueComparer;
            if (valueComparer is not null)
            {
                var hash1 = valueComparer.GetHashCode(x);
                var hash2 = valueComparer.GetHashCode(y);

                if (hash1 != hash2)
                {
                    return false;
                }

                return valueComparer.Equals(x, y);
            }

            var wrapperX = new JsonValueWrapper(x);
            var wrapperY = new JsonValueWrapper(y);
            return wrapperX.DeepEquals(ref wrapperY, comparerOptions.JsonElementComparison);
        }
    }
}
