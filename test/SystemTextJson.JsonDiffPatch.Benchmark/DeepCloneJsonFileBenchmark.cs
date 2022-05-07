using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json.Linq;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    public class DeepCloneJsonFileBenchmark : JsonFileBenchmark
    {
        [Benchmark]
        public JsonNode SystemTextJson()
        {
            return JsonNode.Parse(JsonLeft).DeepClone()!;
        }

        [Benchmark]
        public JToken JsonNet()
        {
            return JToken.Parse(JsonLeft).DeepClone();
        }
    }
}
