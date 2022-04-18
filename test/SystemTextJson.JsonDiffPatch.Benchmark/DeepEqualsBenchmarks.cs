using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json.Linq;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    public class DeepEqualsBenchmarks
    {
        private JToken _tokenX = null!;
        private JToken _tokenY= null!;
        private JsonNode _nodeX = null!;
        private JsonNode _nodeY= null!;
        private JsonNode _elementNodeX = null!;
        private JsonNode _elementNodeY= null!;

        [GlobalSetup]
        public void Setup()
        {
            _tokenX = new JArray(1, 2, 3, 4);
            _tokenY = new JArray(1, 2, 3, 5);
            _nodeX = new JsonArray(
                JsonValue.Create(1), JsonValue.Create(2),
                JsonValue.Create(3), JsonValue.Create(4));
            _nodeY = new JsonArray(
                JsonValue.Create(1), JsonValue.Create(2),
                JsonValue.Create(3), JsonValue.Create(5));
            _elementNodeX = JsonNode.Parse("[1,2,3,4]")!;
            _elementNodeY = JsonNode.Parse("[1,2,3,5]")!;
        }

        [Benchmark]
        public bool JsonNet_Array()
        {
            return JToken.DeepEquals(_tokenX, _tokenY);
        }

        [Benchmark]
        public bool SystemTextJson_Array()
        {
            return _nodeX.DeepEquals(_nodeY);
        }
        
        [Benchmark]
        public bool SystemTextJson_ElementArray()
        {
            return _elementNodeX.DeepEquals(_elementNodeY);
        }
    }
}
