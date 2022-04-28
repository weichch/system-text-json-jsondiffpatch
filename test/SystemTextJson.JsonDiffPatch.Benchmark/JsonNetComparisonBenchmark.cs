using System.Text.Json.JsonDiffPatch;
using System.Text.Json.JsonDiffPatch.Diffs;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using JsonDiffPatchDotNet;
using Newtonsoft.Json.Linq;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    [IterationCount(50)]
    public abstract class JsonNetComparisonBenchmark : ExampleJsonFileBenchmark
    {
        protected JsonNetComparisonBenchmark(string beforeFile, string afterFile, string diffFile)
            : base(beforeFile, afterFile, diffFile)
        {
        }

        [Benchmark]
        public JToken Diff_JsonNet()
        {
            var token1 = JToken.Parse(_jsonBefore);
            var token2 = JToken.Parse(_jsonAfter);

            return CreateJsonNetDiffPatch().Diff(token1, token2);
        }

        [Benchmark]
        public JsonNode? Diff_SystemTextJson()
        {
            var node1 = JsonNode.Parse(_jsonBefore);
            var node2 = JsonNode.Parse(_jsonAfter);

            return node1.Diff(node2, CreateDiffOptionsWithJsonNetMatch());
        }

        [Benchmark]
        public JToken Patch_JsonNet()
        {
            var token1 = JToken.Parse(_jsonBefore);
            var diff = JToken.Parse(_jsonDiff);

            return CreateJsonNetDiffPatch().Patch(token1, diff);
        }

        [Benchmark]
        public JsonNode Patch_SystemTextJson()
        {
            var node1 = JsonNode.Parse(_jsonBefore);
            var diff = JsonNode.Parse(_jsonDiff);

            JsonDiffPatcher.Patch(ref node1, diff);
            return node1!;
        }

        [Benchmark]
        public bool DeepEquals_JsonNet()
        {
            var token1 = JToken.Parse(_jsonBefore);
            var token2 = JToken.Parse(_jsonBefore);

            return JToken.DeepEquals(token1, token2);
        }

        [Benchmark]
        public bool DeepEquals_SystemTextJson()
        {
            var node1 = JsonNode.Parse(_jsonBefore);
            var node2 = JsonNode.Parse(_jsonBefore);

            return node1.DeepEquals(node2, JsonElementComparison.Semantic);
        }

        [Benchmark]
        public JToken DeepClone_JsonNet()
        {
            return JToken.Parse(_jsonBefore).DeepClone();
        }

        [Benchmark]
        public JsonNode DeepClone_SystemTextJson()
        {
            return JsonNode.Parse(_jsonBefore).DeepClone()!;
        }

        private static JsonDiffPatchDotNet.JsonDiffPatch CreateJsonNetDiffPatch()
        {
            return new JsonDiffPatchDotNet.JsonDiffPatch(new Options
            {
                TextDiff = TextDiffMode.Simple
            });
        }

        private static JsonDiffOptions CreateDiffOptionsWithJsonNetMatch()
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
