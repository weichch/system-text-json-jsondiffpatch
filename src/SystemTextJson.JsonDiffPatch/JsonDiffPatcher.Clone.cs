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
            return node.Clone(true);
        }

        /// <summary>
        /// Creates a clone of the <see cref="JsonNode"/>.
        /// </summary>
        /// <param name="node">The <see cref="JsonNode"/>.</param>
        /// <param name="copyJsonElement">Whether to copy JSON element.</param>
        internal static T? Clone<T>(this T? node, bool copyJsonElement)
            where T : JsonNode
        {
            return (T?) (node switch
            {
                null => (JsonNode?) null,
                JsonObject obj => new JsonObject(Enumerate(obj, copyJsonElement), obj.Options),
                JsonArray array => CloneArray(array),
                JsonValue value => CloneJsonValue(value, copyJsonElement),
                _ => throw new NotSupportedException(
                    $"JsonNode of type '{node.GetType().Name}' is not supported.")
            });

            static IEnumerable<KeyValuePair<string, JsonNode?>> Enumerate(JsonObject obj, bool copyJsonElement)
            {
                foreach (var kvp in obj)
                {
                    yield return new KeyValuePair<string, JsonNode?>(kvp.Key, kvp.Value.Clone(copyJsonElement));
                }
            }
        }

        private static JsonValue? CloneJsonValue(JsonValue? value, bool copyJsonElement)
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
                JsonElement actualValue => JsonValue.Create(
                    copyJsonElement ? actualValue.Clone() : actualValue,
                    value.Options),
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
