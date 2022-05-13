using System.Collections.Generic;
using System.Text.Json;

namespace SystemTextJson.JsonDiffPatch.UnitTests.DocumentTests
{
    public class DocumentTestData
    {
        public static IEnumerable<object?[]> RawTextEqual => DeepEqualsTestData.RawTextEqual(Json);

        public static IEnumerable<object?[]> SemanticEqual => DeepEqualsTestData.SemanticEqual(Json);

        private static JsonDocument? Json(string? jsonValue)
        {
            return jsonValue is null ? null : JsonDocument.Parse($"{jsonValue}");
        }
    }
}
