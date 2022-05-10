using System.Text.Json;
using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json.Linq;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    [IterationCount(10)]
    public class QuickDeepEquals : JsonFileBenchmark
    {
        [Params(JsonFileSize.Small)]
        public override JsonFileSize FileSize { get; set; }

        [Benchmark]
        public bool SystemTextJson()
        {
            var node1 = JsonNode.Parse(JsonLeft);
            var node2 = JsonNode.Parse(JsonLeft);

            return node1.DeepEquals(node2, JsonElementComparison.Semantic);
        }

        [Benchmark]
        public bool SystemTextJson_Document()
        {
            var node1 = JsonDocument.Parse(JsonLeft);
            var node2 = JsonDocument.Parse(JsonLeft);

            return node1.DeepEquals(node2, JsonElementComparison.Semantic);
        }

        [Benchmark]
        public bool SystemTextJson_Document_RawText()
        {
            var node1 = JsonDocument.Parse(JsonLeft);
            var node2 = JsonDocument.Parse(JsonLeft);

            return node1.DeepEquals(node2);
        }

        [Benchmark]
        public JToken JsonNet()
        {
            var token1 = JToken.Parse(JsonLeft);
            var token2 = JToken.Parse(JsonLeft);

            return JToken.DeepEquals(token1, token2);
        }
    }
}
