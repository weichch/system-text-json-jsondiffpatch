using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace System.Text.Json
{
    static partial class JsonDiffPatcher
    {
        /// <summary>
        /// Creates a clone of the <see cref="JsonNode"/>.
        /// </summary>
        /// <param name="node">The <see cref="JsonNode"/>.</param>
        public static T? Clone<T>(this T? node)
            where T : JsonNode
        {
            return (T?)(node switch
            {
                null => (JsonNode?)null,
                JsonObject obj => new JsonObject(obj.Select(kvp =>
                    new KeyValuePair<string, JsonNode?>(kvp.Key, kvp.Value.Clone())), obj.Options),
                JsonArray array => CloneArray(array),
                JsonValue value => CloneJsonValue(value),
                _ => throw new NotSupportedException(
                    $"JsonNode of type '{node.GetType().Name}' is not supported.")
            });
        }
        
        /// <summary>
        /// Creates a clone of the JSON value using the most appropriate method.
        /// </summary>
        private static JsonValue? CloneJsonValue(JsonValue? value)
        {
            if (value is null)
            {
                return null;
            }

            // Perf: This is slower than direct property access
            var objValue = value.GetValue<object>();
            var cloned = objValue switch
            {
                null => null,
                JsonElement actualValue => JsonValue.Create(actualValue, value.Options),
                bool actualValue => JsonValue.Create(actualValue, value.Options),
                byte actualValue => JsonValue.Create(actualValue, value.Options),
                char actualValue => JsonValue.Create(actualValue, value.Options),
                DateTime actualValue => JsonValue.Create(actualValue, value.Options),
                DateTimeOffset actualValue => JsonValue.Create(actualValue, value.Options),
                decimal actualValue => JsonValue.Create(actualValue, value.Options),
                double actualValue => JsonValue.Create(actualValue, value.Options),
                Guid actualValue => JsonValue.Create(actualValue, value.Options),
                short actualValue => JsonValue.Create(actualValue, value.Options),
                int actualValue => JsonValue.Create(actualValue, value.Options),
                long actualValue => JsonValue.Create(actualValue, value.Options),
                sbyte actualValue => JsonValue.Create(actualValue, value.Options),
                float actualValue => JsonValue.Create(actualValue, value.Options),
                string actualValue => JsonValue.Create(actualValue, value.Options),
                ushort actualValue => JsonValue.Create(actualValue, value.Options),
                uint actualValue => JsonValue.Create(actualValue, value.Options),
                ulong actualValue => JsonValue.Create(actualValue, value.Options),
                _ => JsonValue.Create(objValue, value.Options)
            };

            return cloned;
        }

        private static JsonArray CloneArray(JsonArray arr)
        {
            var newArr = new JsonArray(arr.Options);
            foreach (var cloned in arr.Select(Clone))
            {
                newArr.Add(cloned);
            }

            return newArr;
        }
    }
}
