using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json.Linq;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    public class QuickDiff : JsonFileBenchmark
    {
        [Params(JsonFileSize.Small)]
        public override JsonFileSize FileSize { get; set; }

        [Benchmark]
        public JsonNode? SystemTextJson()
        {
            var node1 = JsonNode.Parse(JsonLeft);
            var node2 = JsonNode.Parse(JsonRight);

            return node1.Diff(node2, BenchmarkHelper.CreateDiffOptionsWithJsonNetMatch());
        }

        [Benchmark]
        public JToken JsonNet()
        {
            var token1 = JToken.Parse(JsonLeft);
            var token2 = JToken.Parse(JsonRight);

            return BenchmarkHelper.CreateJsonNetDiffPatch().Diff(token1, token2);
        }
    }
}
