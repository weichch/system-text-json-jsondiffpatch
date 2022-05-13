using System.Text.Json;
using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json.Linq;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    public class DeepEqualsJsonFileBenchmark : JsonFileBenchmark
    {
        [Benchmark]
        public bool SystemTextJson_Node()
        {
            var json1 = JsonNode.Parse(JsonLeft);
            var json2 = JsonNode.Parse(JsonLeft);

            return json1.DeepEquals(json2, JsonElementComparison.Semantic);
        }

        [Benchmark]
        public bool SystemTextJson_Document()
        {
            using var json1 = JsonDocument.Parse(JsonLeft);
            using var json2 = JsonDocument.Parse(JsonLeft);

            return json1.DeepEquals(json2, JsonElementComparison.Semantic);
        }

        [Benchmark]
        public bool JsonNet()
        {
            var json1 = JToken.Parse(JsonLeft);
            var json2 = JToken.Parse(JsonLeft);

            return JToken.DeepEquals(json1, json2);
        }
    }
}
