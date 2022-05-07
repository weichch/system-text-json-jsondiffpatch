using System.Collections.Generic;
using System.Text.Json.JsonDiffPatch;
using System.Text.Json.JsonDiffPatch.Diffs.Formatters;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using JsonDiffPatchDotNet.Formatters.JsonPatch;
using Newtonsoft.Json.Linq;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    public class DiffJsonFileBenchmark : JsonFileBenchmark
    {
        [Benchmark]
        public JToken JsonNet()
        {
            var token1 = JToken.Parse(JsonLeft);
            var token2 = JToken.Parse(JsonRight);

            return BenchmarkHelper.CreateJsonNetDiffPatch().Diff(token1, token2);
        }

        [Benchmark]
        public JsonNode? SystemTextJson()
        {
            var node1 = JsonNode.Parse(JsonLeft);
            var node2 = JsonNode.Parse(JsonRight);

            return node1.Diff(node2, BenchmarkHelper.CreateDiffOptionsWithJsonNetMatch());
        }

        [Benchmark]
        public IList<Operation> JsonNet_Rfc()
        {
            var token1 = JToken.Parse(JsonLeft);
            var token2 = JToken.Parse(JsonRight);

            return new JsonDeltaFormatter().Format(
                BenchmarkHelper.CreateJsonNetDiffPatch().Diff(token1, token2));
        }

        [Benchmark]
        public JsonNode? SystemTextJson_Rfc()
        {
            var node1 = JsonNode.Parse(JsonLeft);
            var node2 = JsonNode.Parse(JsonRight);

            return node1.Diff(node2, new JsonPatchDeltaFormatter(),
                BenchmarkHelper.CreateDiffOptionsWithJsonNetMatch());
        }
    }
}
