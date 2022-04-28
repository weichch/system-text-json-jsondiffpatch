using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    static partial class JsonDiffPatcher
    {
        /// <summary>
        /// Creates a deep copy of the <see cref="JsonNode"/>.
        /// </summary>
        /// <param name="obj">The <see cref="JsonNode"/>.</param>
        public static T? DeepClone<T>(this T? obj) where T : JsonNode
        {
            return (T?)(obj switch
            {
                null => (JsonNode?)null,
                JsonObject jsonObj => new JsonObject(Enumerate(jsonObj), obj.Options),
                JsonArray array => CloneArray(array),
                JsonValue value => CloneJsonValue(value),
                _ => throw new NotSupportedException(
                    $"JsonNode of type '{obj.GetType().Name}' is not supported.")
            });

            static IEnumerable<KeyValuePair<string, JsonNode?>> Enumerate(JsonObject obj)
            {
                foreach (var kvp in obj)
                {
                    yield return new KeyValuePair<string, JsonNode?>(kvp.Key, kvp.Value.DeepClone());
                }
            }
        }

        private static JsonValue? CloneJsonValue(JsonValue? value)
        {
            if (value is null)
            {
                return null;
            }

            if (value.TryGetValue<JsonElement>(out var element))
                return JsonValue.Create(element.Clone(), value.Options);
            if (value.TryGetValue<int>(out var intValue))
                return JsonValue.Create(intValue, value.Options);
            if (value.TryGetValue<long>(out var longValue))
                return JsonValue.Create(longValue, value.Options);
            if (value.TryGetValue<double>(out var doubleValue))
                return JsonValue.Create(doubleValue, value.Options);
            if (value.TryGetValue<short>(out var shortValue))
                return JsonValue.Create(shortValue, value.Options);
            if (value.TryGetValue<decimal>(out var decimalValue))
                return JsonValue.Create(decimalValue, value.Options);
            if (value.TryGetValue<byte>(out var byteValue))
                return JsonValue.Create(byteValue, value.Options);
            if (value.TryGetValue<float>(out var floatValue))
                return JsonValue.Create(floatValue, value.Options);
            if (value.TryGetValue<uint>(out var uintValue))
                return JsonValue.Create(uintValue, value.Options);
            if (value.TryGetValue<ushort>(out var ushortValue))
                return JsonValue.Create(ushortValue, value.Options);
            if (value.TryGetValue<ulong>(out var ulongValue))
                return JsonValue.Create(ulongValue, value.Options);
            if (value.TryGetValue<sbyte>(out var sbyteValue))
                return JsonValue.Create(sbyteValue, value.Options);
            if (value.TryGetValue<string>(out var stringValue))
                return JsonValue.Create(stringValue, value.Options);
            if (value.TryGetValue<DateTime>(out var dateTimeValue))
                return JsonValue.Create(dateTimeValue, value.Options);
            if (value.TryGetValue<DateTimeOffset>(out var dateTimeOffsetValue))
                return JsonValue.Create(dateTimeOffsetValue, value.Options);
            if (value.TryGetValue<Guid>(out var guidValue))
                return JsonValue.Create(guidValue, value.Options);
            if (value.TryGetValue<char>(out var charValue))
                return JsonValue.Create(charValue, value.Options);
            if (value.TryGetValue<byte[]>(out var byteArrayValue))
                return JsonValue.Create(byteArrayValue, value.Options);

            // Perf: This is slower than direct property access
            return JsonValue.Create(value.GetValue<object>(), value.Options);
        }

        private static JsonArray CloneArray(JsonArray arr)
        {
            var newArr = new JsonArray(arr.Options);
            foreach (var cloned in arr.Select(DeepClone))
            {
                newArr.Add(cloned);
            }

            return newArr;
        }
    }
}
