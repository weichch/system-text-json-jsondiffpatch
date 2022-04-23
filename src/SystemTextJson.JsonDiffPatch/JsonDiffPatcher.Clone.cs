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
            => CloneNode(obj, false);

        /// <summary>
        /// Creates a deep copy of the <see cref="JsonNode"/> and materialize readonly <see cref="JsonNode"/>,
        /// i.e. backed by <see cref="JsonElement"/>, with value of the most significant type in the processing.
        /// </summary>
        /// <param name="obj">The <see cref="JsonNode"/>.</param>
        public static T? Materialize<T>(this T? obj)
            where T : JsonNode
            => CloneNode(obj, true);

        private static T? CloneNode<T>(T? obj, bool materialize)
            where T : JsonNode
        {
            return (T?)(obj switch
            {
                null => (JsonNode?)null,
                JsonObject jsonObj => new JsonObject(Enumerate(jsonObj, materialize), obj.Options),
                JsonArray array => CloneArray(array, materialize),
                JsonValue value => CloneJsonValue(value, materialize),
                _ => throw new NotSupportedException(
                    $"JsonNode of type '{obj.GetType().Name}' is not supported.")
            });

            static IEnumerable<KeyValuePair<string, JsonNode?>> Enumerate(JsonObject obj, bool materialize)
            {
                foreach (var kvp in obj)
                {
                    yield return new KeyValuePair<string, JsonNode?>(kvp.Key, CloneNode(kvp.Value, materialize));
                }
            }
        }

        private static JsonValue? CloneJsonValue(JsonValue? value, bool materialize)
        {
            if (value is null)
            {
                return null;
            }

            if (value.TryGetValue<JsonElement>(out var element))
            {
                return materialize
                    ? MaterializeJsonElement(element, value)
                    : JsonValue.Create(element.Clone(), value.Options);
            }

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

        private static JsonArray CloneArray(JsonArray arr, bool materialize)
        {
            var newArr = new JsonArray(arr.Options);
            foreach (var cloned in arr.Select(item => CloneNode(item, materialize)))
            {
                newArr.Add(cloned);
            }

            return newArr;
        }

        private static JsonValue? MaterializeJsonElement(in JsonElement element, JsonValue existingValue)
        {
            // If change this, also change in Compare, CompareNumber and CreateNode
            switch (element.ValueKind)
            {
                case JsonValueKind.Number:
                    if (element.TryGetInt64(out var longValue))
                        return JsonValue.Create(longValue, existingValue.Options);
                    if (element.TryGetDecimal(out var decimalValue))
                        return JsonValue.Create(decimalValue, existingValue.Options);
                    if (element.TryGetDouble(out var doubleValue))
                        return JsonValue.Create(doubleValue, existingValue.Options);

                    throw new ArgumentException("Unsupported JSON number.");

                case JsonValueKind.String:
                    if (element.TryGetDateTimeOffset(out var dateTimeOffsetValue))
                        return JsonValue.Create(dateTimeOffsetValue, existingValue.Options);
                    if (element.TryGetDateTime(out var dateTimeValue))
                        return JsonValue.Create(dateTimeValue, existingValue.Options);
                    if (element.TryGetGuid(out var guidValue))
                        return JsonValue.Create(guidValue, existingValue.Options);
                    if (element.TryGetBytesFromBase64(out var byteArrayValue))
                        return JsonValue.Create(byteArrayValue, existingValue.Options);

                    return JsonValue.Create(element.GetString(), existingValue.Options);

                case JsonValueKind.True:
                case JsonValueKind.False:
                    return JsonValue.Create(element.ValueKind == JsonValueKind.True, existingValue.Options);

                case JsonValueKind.Null:
                    return null;

                case JsonValueKind.Undefined:
                case JsonValueKind.Object:
                case JsonValueKind.Array:
                default:
                    throw new ArgumentOutOfRangeException(nameof(element.ValueKind),
                        $"Unexpected value kind {element.ValueKind:G}");
            }
        }
    }
}
