using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json.Linq;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    public class DeepEqualsBenchmarks
    {
        [Benchmark]
        public bool JsonNet()
        {
            var tokenX = new JArray(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            var tokenY = new JArray(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);

            return JToken.DeepEquals(tokenX, tokenY);
        }

        [Benchmark]
        public bool JsonNet_String()
        {
            var tokenX = JToken.Parse("[1,2,3,4,5,6,7,8,9,10]");
            var tokenY = JToken.Parse("[1,2,3,4,5,6,7,8,9,10]");

            return JToken.DeepEquals(tokenX, tokenY);
        }

        [Benchmark]
        public bool SystemTextJson()
        {
            var nodeX = new JsonArray(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            var nodeY = new JsonArray(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);

            return nodeX.DeepEquals(nodeY);
        }

        [Benchmark]
        public bool SystemTextJson_String()
        {
            var nodeX = JsonNode.Parse("[1,2,3,4,5,6,7,8,9,10]");
            var nodeY = JsonNode.Parse("[1,2,3,4,5,6,7,8,9,10]");

            return nodeX.DeepEquals(nodeY, JsonElementComparison.Semantic);
        }
    }
}
