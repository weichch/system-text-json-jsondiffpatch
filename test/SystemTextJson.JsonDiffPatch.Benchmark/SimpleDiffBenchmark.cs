using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json.Linq;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    public class SimpleDiffBenchmark
    {
        private readonly JsonDiffPatchDotNet.JsonDiffPatch _instance1;
        private readonly string _jsonBefore;
        private readonly string _jsonAfter;
        private readonly string _jsonLargeBefore;
        private readonly string _jsonLargeAfter;

        public SimpleDiffBenchmark()
        {
            _jsonBefore = File.ReadAllText(@"Examples\demo_left.json");
            _jsonAfter = File.ReadAllText(@"Examples\demo_right.json");

            _jsonLargeBefore = File.ReadAllText(@"Examples\large_left.json");
            _jsonLargeAfter = File.ReadAllText(@"Examples\large_right.json");

            _instance1 = new JsonDiffPatchDotNet.JsonDiffPatch();
        }

        [Benchmark]
        public JToken? DemoObject_JsonNet()
        {
            var token1 = JToken.Parse(_jsonBefore);
            var token2 = JToken.Parse(_jsonAfter);
            return _instance1.Diff(token1, token2);
        }

        [Benchmark]
        public JsonDocument? DemoObject_DefaultOptions()
        {
            return JsonDiffPatcher.Diff(_jsonBefore, _jsonAfter,
                new JsonDiffOptions
                {
                    ArrayItemMatcher = JsonNetArrayItemMatch
                });
        }

        [Benchmark]
        public JsonDocument? DemoObject_NoArrayMove()
        {
            return JsonDiffPatcher.Diff(_jsonBefore, _jsonAfter,
                new JsonDiffOptions
                {
                    SuppressDetectArrayMove = true,
                    ArrayItemMatcher = JsonNetArrayItemMatch
                });
        }

        [Benchmark]
        public JsonNode? DemoObject_Mutable()
        {
            var left = JsonNode.Parse(_jsonBefore);
            var right = JsonNode.Parse(_jsonAfter);

            return left.Diff(right,
                new JsonDiffOptions
                {
                    ArrayItemMatcher = JsonNetArrayItemMatch
                });
        }

        [Benchmark]
        public JToken? LargeObject_JsonNet()
        {
            var token1 = JToken.Parse(_jsonLargeBefore);
            var token2 = JToken.Parse(_jsonLargeAfter);
            return _instance1.Diff(token1, token2);
        }

        [Benchmark]
        public JsonDocument? LargeObject_DefaultOptions()
        {
            return JsonDiffPatcher.Diff(_jsonLargeBefore, _jsonLargeAfter,
                new JsonDiffOptions
                {
                    ArrayItemMatcher = JsonNetArrayItemMatch
                });
        }

        [Benchmark]
        public JsonDocument? LargeObject_NoArrayMove()
        {
            return JsonDiffPatcher.Diff(_jsonLargeBefore, _jsonLargeAfter,
                new JsonDiffOptions
                {
                    SuppressDetectArrayMove = true,
                    ArrayItemMatcher = JsonNetArrayItemMatch
                });
        }

        [Benchmark]
        public JsonNode? LargeObject_Mutable()
        {
            var left = JsonNode.Parse(_jsonLargeBefore);
            var right = JsonNode.Parse(_jsonLargeAfter);

            return left.Diff(right,
                new JsonDiffOptions
                {
                    ArrayItemMatcher = JsonNetArrayItemMatch
                });
        }

        // Simulate array item match algorithm in JsonNet version
        private static bool JsonNetArrayItemMatch(JsonNode? x, int i, JsonNode? y, int j, out bool deepEq)
        {
            deepEq = false;

            if (x.DeepEquals(y) 
                || (x is JsonObject && y is JsonObject) 
                || (x is JsonArray && y is JsonArray))
            {
                return true;
            }

            return false;
        }
    }
}
