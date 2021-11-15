using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json.Linq;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    [MinColumn, MaxColumn, MemoryDiagnoser]
    public class SimpleDiffBenchmark
    {
        private readonly JToken _jsonObj11;
        private readonly JToken _jsonObj12;
        private readonly JsonNode _jsonObj21;
        private readonly JsonNode _jsonObj22;

        private readonly JsonDiffPatchDotNet.JsonDiffPatch _instance1;

        public SimpleDiffBenchmark()
        {
            var jsonBefore = $@"{{ ""a"": [ {string.Join(",", Enumerable.Range(1, 1000))} ], ""b"": null }}";
            var jsonAfter = @"{ ""a"": [ 1,3,2,4 ], ""c"": 123 }";

            _jsonObj11 = JToken.Parse(jsonBefore);
            _jsonObj12 = JToken.Parse(jsonAfter);
            _jsonObj21 = JsonNode.Parse(jsonBefore)!;
            _jsonObj22 = JsonNode.Parse(jsonAfter)!;

            _instance1 = new JsonDiffPatchDotNet.JsonDiffPatch();
        }

        [Benchmark]
        public JsonNode? SystemTextJsonJsonDiffPatcher()
            => _jsonObj21.Diff(_jsonObj22, new JsonDiffOptions
            {
                // JsonDiffPatchDotNet does not support array move
                SuppressDetectArrayMove = true
            });

        [Benchmark]
        public JToken? JsonDiffPatchDotNet()
            => _instance1.Diff(_jsonObj11, _jsonObj12);
    }
}
