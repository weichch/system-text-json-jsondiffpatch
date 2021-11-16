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

            if (ReferenceEquals(node, another))
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
            var ret1 = val1.TryGetValue<JsonElement>(out var e1);
            var ret2 = val2.TryGetValue<JsonElement>(out var e2);

            // Case 1: both backed by JsonElement
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
                // and compare raw text values. This may consume a lot memory for large JSON objects
                return Equals(e1.GetRawText(), e2.GetRawText());
            }

            if (ret1 || ret2)
            {
                var materialized = (ret1 ? val1 : val2).Clone(true);
                if (materialized is null)
                {
                    return false;
                }

                var value = ret1 ? val2 : val1;

                return ValueEquals(materialized, value);
            }

            var innerValue1 = val1.GetObjectValue();
            var innerValue2 = val2.GetObjectValue();

            if (innerValue1 is bool b1 && innerValue2 is bool b2)
            {
                return b1.Equals(b2);
            }

            if (innerValue1 is char c1 && innerValue2 is char c2)
            {
                return c1.Equals(c2);
            }

            if (innerValue1 is ulong or decimal || innerValue2 is ulong or decimal)
            {
                return Convert.ToDecimal(innerValue1).Equals(Convert.ToDecimal(innerValue2));
            }

            if (innerValue1 is float or double || innerValue2 is float or double)
            {
                var d1 = Convert.ToDouble(innerValue1);
                var d2 = Convert.ToDouble(innerValue2);

                // This is the same epsilon value:
                // https://github.com/JamesNK/Newtonsoft.Json/blob/f7e7bd05d9280f17993500085202ff4ea150564a/Src/Newtonsoft.Json/Utilities/MathUtils.cs#L174
                // Also: https://docs.microsoft.com/en-us/dotnet/api/system.double.equals?view=net-5.0
                if (Math.Abs(d1 - d2) < 2.2204460492503131E-16)
                {
                    return true;
                }
            }

            if (innerValue1 is DateTime or DateTimeOffset && innerValue2 is DateTime or DateTimeOffset)
            {
                return innerValue1 switch
                {
                    DateTime dt1 when innerValue2 is DateTime dt2 => dt1.Equals(dt2),
                    DateTime dt1 when innerValue2 is DateTimeOffset dt2 => dt1.Equals(dt2.DateTime),
                    DateTimeOffset dt1 when innerValue2 is DateTime dt2 => dt1.Equals((DateTimeOffset) dt2),
                    DateTimeOffset dt1 when innerValue2 is DateTimeOffset dt2 => dt1.Equals(dt2),
                    _ => false
                };
            }

            if (innerValue1 is Guid g1 && innerValue2 is Guid g2)
            {
                return g1.Equals(g2);
            }

            return Equals(innerValue1, innerValue2);
        }
    }
}
