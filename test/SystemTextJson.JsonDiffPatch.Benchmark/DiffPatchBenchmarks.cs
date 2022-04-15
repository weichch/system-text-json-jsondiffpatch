using System.Collections.Generic;
using System.IO;
using System.Text.Json.JsonDiffPatch;
using System.Text.Json.JsonDiffPatch.Diffs;
using System.Text.Json.JsonDiffPatch.Diffs.Formatters;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using JsonDiffPatchDotNet;
using JsonDiffPatchDotNet.Formatters.JsonPatch;
using Newtonsoft.Json.Linq;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    [SimpleJob(RuntimeMoniker.Net48), SimpleJob(RuntimeMoniker.Net60)]
    public abstract class DiffPatchBenchmarks
    {
        private JsonDiffPatchDotNet.JsonDiffPatch _jsonNetInstance = null!;
        private JsonDiffOptions _optionsWithJsonNetAlg = null!;
        private JsonDiffOptions _optionsWithJsonNetAlgSemantic = null!;
        private string _jsonBefore = null!;
        private string _jsonAfter = null!;
        private string _jsonDiff = null!;
        
        public abstract string BeforeFile { get; set; }
        public abstract string AfterFile { get; set; }
        public abstract string DiffFile { get; set; }

        [GlobalSetup]
        public virtual void Setup()
        {
            _jsonBefore = File.ReadAllText(BeforeFile);
            _jsonAfter = File.ReadAllText(AfterFile);
            _jsonDiff = File.ReadAllText(DiffFile);
            
            _jsonNetInstance = new JsonDiffPatchDotNet.JsonDiffPatch(new Options
            {
                // Ignore Google diff patch
                TextDiff = TextDiffMode.Simple
            });

            _optionsWithJsonNetAlg = new JsonDiffOptions
            {
                // Ignore Google diff patch
                TextDiffMinLength = 0,
                // There is no array move support in JsonNet version
                SuppressDetectArrayMove = true,
                ArrayItemMatcher = JsonNetArrayItemMatch
            };

            _optionsWithJsonNetAlgSemantic = new JsonDiffOptions
            {
                // Ignore Google diff patch
                TextDiffMinLength = 0,
                // There is no array move support in JsonNet version
                SuppressDetectArrayMove = true,
                ArrayItemMatcher = JsonNetArrayItemMatch,
                JsonElementComparison = JsonElementComparison.Semantic
            };
        }

        [Benchmark]
        public JToken Diff_JsonNet()
        {
            var left = JToken.Parse(_jsonBefore);
            var right = JToken.Parse(_jsonAfter);

            return _jsonNetInstance.Diff(left, right);
        }

        [Benchmark]
        public JsonNode? Diff_SystemTextJson()
        {
            var left = JsonNode.Parse(_jsonBefore)!;
            var right = JsonNode.Parse(_jsonAfter)!;

            return left.Diff(right, _optionsWithJsonNetAlg);
        }
        
        [Benchmark]
        public JsonNode? Diff_SystemTextJson_Semantic()
        {
            var left = JsonNode.Parse(_jsonBefore)!;
            var right = JsonNode.Parse(_jsonAfter)!;

            return left.Diff(right, _optionsWithJsonNetAlgSemantic);
        }
        
        [Benchmark]
        public IList<Operation> Diff_JsonNet_Rfc()
        {
            var token1 = JToken.Parse(_jsonBefore);
            var token2 = JToken.Parse(_jsonAfter);
            return new JsonDeltaFormatter().Format(_jsonNetInstance.Diff(token1, token2));
        }

        [Benchmark]
        public JsonNode? Diff_SystemTextJson_Rfc()
        {
            var left = JsonNode.Parse(_jsonBefore);
            var right = JsonNode.Parse(_jsonAfter);

            return left.Diff(right, new JsonPatchDeltaFormatter(), _optionsWithJsonNetAlg);
        }
        
        [Benchmark]
        public JToken Patch_JsonNet()
        {
            var left = JToken.Parse(_jsonBefore);
            var diff = JToken.Parse(_jsonDiff);
            
            return _jsonNetInstance.Patch(left, diff);
        }

        [Benchmark]
        public JsonNode? Patch_SystemTextJson()
        {
            var left = JsonNode.Parse(_jsonBefore);
            var diff = JsonNode.Parse(_jsonDiff);

            JsonDiffPatcher.Patch(ref left, diff);
            return left;
        }
        
        [Benchmark]
        public bool DeepEquals_JsonNet()
        {
            var left = JToken.Parse(_jsonBefore);
            var right = JToken.Parse(_jsonBefore);

            return JToken.DeepEquals(left, right);
        }

        [Benchmark]
        public bool DeepEquals_SystemTextJson()
        {
            var left = JsonNode.Parse(_jsonBefore);
            var right = JsonNode.Parse(_jsonBefore);

            return left.DeepEquals(right);
        }
        
        [Benchmark]
        public bool DeepEquals_SystemTextJson_Semantic()
        {
            var left = JsonNode.Parse(_jsonBefore);
            var right = JsonNode.Parse(_jsonBefore);

            return left.DeepEquals(right, JsonElementComparison.Semantic);
        }
        
        [Benchmark]
        public JToken DeepClone_JsonNet()
        {
            var left = JToken.Parse(_jsonBefore);

            return left.DeepClone();
        }

        [Benchmark]
        public JsonNode? DeepClone_SystemTextJson()
        {
            var left = JsonNode.Parse(_jsonBefore);

            return left.DeepClone();
        }

        // Simulate array item match algorithm in JsonNet version:
        // https://github.com/wbish/jsondiffpatch.net/blob/master/Src/JsonDiffPatchDotNet/Lcs.cs#L51
        private static bool JsonNetArrayItemMatch(ref ArrayItemMatchContext context)
        {
            if (context.Left.DeepEquals(context.Right)
                || (context.Left is JsonObject && context.Right is JsonObject) 
                || (context.Left is JsonArray && context.Right is JsonArray))
            {
                return true;
            }

            return false;
        }
    }
}
