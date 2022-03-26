using System.Text.Json.JsonDiffPatch;
using System.Text.Json.JsonDiffPatch.Diffs;
using System.Text.Json.JsonDiffPatch.Diffs.Formatters;
using System.Text.Json.Nodes;
using Xunit;

namespace SystemTextJson.JsonDiffPatch.UnitTests.Formatters
{
    public class JsonPatchDeltaFormatterTests
    {
        [Fact]
        public void ArrayElementAdd_Number()
        {
            var left = JsonNode.Parse("{\"data\":[1]}");
            var right = JsonNode.Parse("{\"data\":[1,2]}");

            var diff = left.Diff(right, new JsonPatchDeltaFormatter());

            Assert.Equal("[{\"op\":\"add\",\"path\":\"/data/1\",\"value\":2}]", diff!.ToJsonString());
        }

        [Fact]
        public void ArrayElementModify_Number()
        {
            var left = JsonNode.Parse("{\"data\":[1]}");
            var right = JsonNode.Parse("{\"data\":[2]}");

            var diff = left.Diff(right, new JsonPatchDeltaFormatter());

            Assert.Equal("[{\"op\":\"remove\",\"path\":\"/data/0\"},{\"op\":\"add\",\"path\":\"/data/0\",\"value\":2}]",
                diff!.ToJsonString());
        }
        
        [Fact]
        public void ArrayElementModify_Number_FuzzyMatch()
        {
            var left = JsonNode.Parse("{\"data\":[1]}");
            var right = JsonNode.Parse("{\"data\":[2]}");

            var diff = left.Diff(right, new JsonPatchDeltaFormatter(), new JsonDiffOptions
            {
                ArrayItemMatcher = (ref ArrayItemMatchContext _) => true
            });

            Assert.Equal("[{\"op\":\"replace\",\"path\":\"/data/0\",\"value\":2}]", diff!.ToJsonString());
        }
    }
}