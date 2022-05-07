using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json.Linq;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    public class PatchJsonFileBenchmark : JsonFileBenchmark
    {
        [Benchmark]
        public JsonNode SystemTextJson()
        {
            var node1 = JsonNode.Parse(JsonLeft);
            var diff = JsonNode.Parse(JsonDiff);

            JsonDiffPatcher.Patch(ref node1, diff);
            return node1!;
        }

        [Benchmark]
        public JToken JsonNet()
        {
            var token1 = JToken.Parse(JsonLeft);
            var diff = JToken.Parse(JsonDiff);

            return BenchmarkHelper.CreateJsonNetDiffPatch().Patch(token1, diff);
        }
    }
}
