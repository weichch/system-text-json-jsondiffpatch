using System.Collections.Generic;
using System.Text.Json;

namespace SystemTextJson.JsonDiffPatch.UnitTests.ElementTests
{
    public class ElementTestData
    {
        public static IEnumerable<object?[]> RawTextEqual => DeepEqualsTestData.RawTextEqual(Json);

        public static IEnumerable<object?[]> SemanticEqual => DeepEqualsTestData.SemanticEqual(Json);

        private static object? Json(string? jsonValue)
        {
            // ReSharper disable once HeapView.BoxingAllocation
            return jsonValue is null
                ? default
                : JsonSerializer.Deserialize<JsonElement>($"{jsonValue}");
        }
    }
}
