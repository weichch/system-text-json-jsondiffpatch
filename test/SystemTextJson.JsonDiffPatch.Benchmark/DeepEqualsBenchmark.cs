using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json.Linq;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    [IterationCount(50)]
    public class DeepEqualsBenchmark
    {
        [Benchmark]
        public JToken JsonNet()
        {
            var tokenX = JToken.Parse("[1,2,3,0,1,2,3,4,5,6,7,8,9,10,1,2,3]");
            var tokenY = JToken.Parse("[1,2,3,10,0,1,7,2,4,5,6,88,9,3,1,2,3]");

            return new JsonDiffPatchDotNet.JsonDiffPatch().Diff(tokenX, tokenY);
        }

        [Benchmark]
        public JsonNode SystemTextJson()
        {
            var nodeX = JsonNode.Parse("[1,2,3,0,1,2,3,4,5,6,7,8,9,10,1,2,3]");
            var nodeY = JsonNode.Parse("[1,2,3,10,0,1,7,2,4,5,6,88,9,3,1,2,3]");
        
            return nodeX.Diff(nodeY, new JsonDiffOptions
            {
                SuppressDetectArrayMove = true,
                JsonElementComparison = JsonElementComparison.Semantic
            })!;
        }
    }
}
