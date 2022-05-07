using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json.Linq;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    public class DeepEqualsJsonFileBenchmark : JsonFileBenchmark
    {
        [Benchmark]
        public bool JsonNet()
        {
            var token1 = JToken.Parse(JsonLeft);
            var token2 = JToken.Parse(JsonLeft);

            return JToken.DeepEquals(token1, token2);
        }

        [Benchmark]
        public bool SystemTextJson()
        {
            var node1 = JsonNode.Parse(JsonLeft);
            var node2 = JsonNode.Parse(JsonLeft);

            return node1.DeepEquals(node2, JsonElementComparison.Semantic);
        }
    }
}
