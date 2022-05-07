using System.Text.Json.JsonDiffPatch;
using System.Text.Json.JsonDiffPatch.Diffs;
using System.Text.Json.Nodes;
using JsonDiffPatchDotNet;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    internal static class BenchmarkHelper
    {
        public static JsonDiffPatchDotNet.JsonDiffPatch CreateJsonNetDiffPatch()
        {
            return new JsonDiffPatchDotNet.JsonDiffPatch(new Options
            {
                TextDiff = TextDiffMode.Simple
            });
        }

        public static JsonDiffOptions CreateDiffOptionsWithJsonNetMatch()
        {
            return new JsonDiffOptions
            {
                TextDiffMinLength = 0,
                SuppressDetectArrayMove = true,
                ArrayItemMatcher = JsonNetArrayItemMatch,
                JsonElementComparison = JsonElementComparison.Semantic
            };
        }

        // Simulate array item match algorithm in JsonNet version:
        // https://github.com/wbish/jsondiffpatch.net/blob/master/Src/JsonDiffPatchDotNet/Lcs.cs#L51
        private static bool JsonNetArrayItemMatch(ref ArrayItemMatchContext context)
        {
            if (context.Left is JsonObject && context.Right is JsonObject ||
                context.Left is JsonArray && context.Right is JsonArray)
            {
                return true;
            }

            return false;
        }
    }
}