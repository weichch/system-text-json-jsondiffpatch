using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    public class OptionsBenchmarks : JsonFileBenchmark
    {
        [Params(JsonFileSize.Small)]
        public override JsonFileSize FileSize { get; set; }

        [Benchmark]
        public JsonNode RawText()
        {
            var node1 = JsonNode.Parse(JsonLeft);
            var node2 = JsonNode.Parse(JsonRight);

            return node1.Diff(node2, new JsonDiffOptions
            {
                JsonElementComparison = JsonElementComparison.RawText
            })!;
        }

        [Benchmark]
        public JsonNode Semantic()
        {
            var node1 = JsonNode.Parse(JsonLeft);
            var node2 = JsonNode.Parse(JsonRight);

            return node1.Diff(node2, new JsonDiffOptions
            {
                JsonElementComparison = JsonElementComparison.Semantic
            })!;
        }
    }
}