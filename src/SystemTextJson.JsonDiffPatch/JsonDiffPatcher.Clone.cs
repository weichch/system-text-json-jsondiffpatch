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
            return node.Clone(false);
        }

        /// <summary>
        /// Creates a clone of the <see cref="JsonNode"/>.
        /// </summary>
        /// <param name="node">The <see cref="JsonNode"/>.</param>
        /// <param name="materialize">Whether to materialize the <see cref="JsonNode"/> if it is backed by a <see cref="JsonElement"/>.</param>
        internal static T? Clone<T>(this T? node, bool materialize)
            where T : JsonNode
        {
            return (T?) (node switch
            {
                null => (JsonNode?) null,
                JsonObject obj => new JsonObject(obj.Select(kvp =>
                    new KeyValuePair<string, JsonNode?>(kvp.Key, kvp.Value.Clone(materialize))), obj.Options),
                JsonArray array => CloneArray(array, materialize),
                JsonValue value => CloneJsonValue(value, materialize),
                _ => throw new NotSupportedException(
                    $"JsonNode of type '{node.GetType().Name}' is not supported.")
            });
        }

        /// <summary>
        /// Creates a clone of the JSON value using the most appropriate method.
        /// </summary>
        private static JsonValue? CloneJsonValue(JsonValue? value, bool materialize)
        {
            if (value is null)
            {
                return null;
            }

            var objValue = value.GetObjectValue();
            var cloned = objValue switch
            {
                null => null,
                JsonElement actualValue => materialize
                    ? MaterializeJsonElement(actualValue)
                    : JsonValue.Create(actualValue, value.Options),
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

        private static JsonArray CloneArray(JsonArray arr, bool materialize)
        {
            var newArr = new JsonArray(arr.Options);
            foreach (var cloned in arr.Select(item => item.Clone(materialize)))
            {
                newArr.Add(cloned);
            }

            return newArr;
        }

        private static JsonValue? MaterializeJsonElement(JsonElement element)
        {
            // We need to box the result so that access to the property could be faster
            // via GetObjectResult method
            object? result = null;
            switch (element.ValueKind)
            {
                case JsonValueKind.False:
                case JsonValueKind.True:
                    result = false;
                    break;
                case JsonValueKind.String:
                    if (element.TryGetDateTimeOffset(out var dt))
                    {
                        result = dt;
                    }
                    else if (element.TryGetDateTime(out var d))
                    {
                        result = d;
                    }
                    else if (element.TryGetGuid(out var g))
                    {
                        result = g;
                    }
                    else
                    {
                        result = element.GetString();
                    }

                    break;
                case JsonValueKind.Number:
                    if (element.TryGetDecimal(out var m))
                    {
                        result = m;
                    }
                    else if (element.TryGetDouble(out var d))
                    {
                        result = d;
                    }

                    break;
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                case JsonValueKind.Object:
                case JsonValueKind.Array:
                default:
                    result = null;
                    break;
            }

            return JsonValue.Create(result);
        }
    }
}
