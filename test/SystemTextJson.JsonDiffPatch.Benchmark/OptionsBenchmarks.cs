using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    [IterationCount(50)]
    public class OptionsBenchmarks : ExampleJsonFileBenchmark
    {
        public OptionsBenchmarks()
            : base(GetExampleFile("demo_left.json"),
                GetExampleFile("demo_right.json"),
                GetExampleFile("demo_diff_notext.json"))
        {
        }

        [Benchmark]
        public JsonNode RawText()
        {
            var node1 = JsonNode.Parse(_jsonBefore);
            var node2 = JsonNode.Parse(_jsonAfter);

            return node1.Diff(node2, new JsonDiffOptions
            {
                JsonElementComparison = JsonElementComparison.RawText
            })!;
        }
        
        [Benchmark]
        public JsonNode Semantic()
        {
            var node1 = JsonNode.Parse(_jsonBefore);
            var node2 = JsonNode.Parse(_jsonAfter);

            return node1.Diff(node2, new JsonDiffOptions
            {
                JsonElementComparison = JsonElementComparison.Semantic
            })!;
        }
    }
}