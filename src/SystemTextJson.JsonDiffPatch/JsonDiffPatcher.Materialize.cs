using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    static partial class JsonDiffPatcher
    {
        /// <summary>
        /// Finds readonly <see cref="JsonNode"/>, i.e. backed by <see cref="JsonElement"/>, materializes with
        /// value of the most significant type, and in-place replaces them with materialized <see cref="JsonNode"/>.
        /// </summary>
        /// <param name="obj">The <see cref="JsonNode"/>.</param>
        public static T? Materialize<T>(this T? obj)
            where T : JsonNode
        {
            Debug.Assert(obj is null or JsonValue or JsonObject or JsonArray);

            switch (obj)
            {
                case JsonValue value when value.TryGetValue<JsonElement>(out var element):
                    return (T?) (object?) MaterializeJsonElement(element, value);
                case JsonValue:
                    return obj;
                case JsonObject jsonObject:
                {
                    var keys = ((IDictionary<string, JsonNode?>) jsonObject).Keys;
                    string[] propNames = null!;
                    try
                    {
                        propNames = ArrayPool<string>.Shared.Rent(keys.Count);
                        keys.CopyTo(propNames, 0);

                        for (var i = 0; i < keys.Count; i++)
                        {
                            var materialized = jsonObject[propNames[i]].Materialize();
                            if (!ReferenceEquals(materialized, jsonObject[propNames[i]]))
                            {
                                jsonObject[propNames[i]] = materialized;
                            }
                        }
                    }
                    finally
                    {
                        ArrayPool<string>.Shared.Return(propNames, true);
                    }

                    return obj;
                }
                case JsonArray jsonArray:
                {
                    for (var i = 0; i < jsonArray.Count; i++)
                    {
                        var materialized = jsonArray[i].Materialize();
                        if (!ReferenceEquals(materialized, jsonArray[i]))
                        {
                            jsonArray[i] = materialized;
                        }
                    }

                    return obj;
                }
                default:
                    return obj;
            }
        }

        private static JsonValue? MaterializeJsonElement(in JsonElement element, JsonValue existingValue)
        {
            // If change this, also change in MaterializeJsonElement, CompareNumber and CreateNode
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
