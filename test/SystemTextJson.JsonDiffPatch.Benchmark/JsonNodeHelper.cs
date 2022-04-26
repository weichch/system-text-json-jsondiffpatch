using System;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    public static class JsonNodeHelper
    {
        public static JsonNode? Parse(string json)
        {
            using var document = JsonDocument.Parse(json);
            return CreateNode(document.RootElement);
        }

        private static JsonNode? CreateNode(in JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Number:
                    if (element.TryGetInt64(out var longValue))
                        return JsonValue.Create(longValue);
                    if (element.TryGetDecimal(out var decimalValue))
                        return JsonValue.Create(decimalValue);
                    if (element.TryGetDouble(out var doubleValue))
                        return JsonValue.Create(doubleValue);

                    break;

                case JsonValueKind.String:
                    if (element.TryGetDateTimeOffset(out var dateTimeOffsetValue))
                        return JsonValue.Create(dateTimeOffsetValue);
                    if (element.TryGetDateTime(out var dateTimeValue))
                        return JsonValue.Create(dateTimeValue);
                    if (element.TryGetGuid(out var guidValue))
                        return JsonValue.Create(guidValue);

                    return JsonValue.Create(element.GetString());

                case JsonValueKind.True:
                case JsonValueKind.False:
                    return JsonValue.Create(element.ValueKind == JsonValueKind.True);

                case JsonValueKind.Null:
                    return null;

                case JsonValueKind.Array:
                    var jsonArray = new JsonArray();

                    foreach (var itemElement in element.EnumerateArray())
                    {
                        jsonArray.Add(CreateNode(itemElement));
                    }

                    return jsonArray;

                case JsonValueKind.Object:
                    var jsonObj = new JsonObject();

                    foreach (var prop in element.EnumerateObject())
                    {
                        jsonObj.Add(prop.Name, CreateNode(prop.Value));
                    }

                    return jsonObj;
            }

            throw new ArgumentException("Cannot parse JSON element.");
        }
    }
}