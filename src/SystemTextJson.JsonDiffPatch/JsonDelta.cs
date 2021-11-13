using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace System.Text.Json
{
    /// <summary>
    /// 
    /// </summary>
    public static class JsonDelta
    {
        public const int TypeDeleted = 0;
        private const int ArrayMoved = 3;

        public static JsonArray Added(JsonNode? newValue)
        {
            return new(newValue.Clone());
        }

        public static JsonArray Modified(JsonNode? oldValue, JsonNode? newValue)
        {
            return new(oldValue.Clone(), newValue.Clone());
        }

        public static JsonArray Deleted(JsonNode? oldValue)
        {
            return new(oldValue.Clone(), 0, TypeDeleted);
        }

        public static JsonObject Object(IEnumerable<KeyValuePair<string, JsonNode?>>? innerChanges = null)
        {
            var delta = new JsonObject();

            if (innerChanges is not null)
            {
                foreach (var kvp in innerChanges)
                {
                    delta[kvp.Key] = kvp.Value;
                }
            }

            return delta;
        }

        public static JsonObject Array(IEnumerable<KeyValuePair<string, JsonNode?>>? innerChanges = null)
        {
            var delta = new JsonObject {{"_t", "a"}};

            if (innerChanges is not null)
            {
                foreach (var kvp in innerChanges)
                {
                    delta[kvp.Key] = kvp.Value;
                }
            }

            return delta;
        }
    }
}
