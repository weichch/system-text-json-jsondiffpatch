using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using Xunit;

namespace SystemTextJson.JsonDiffPatch.UnitTests
{
    public class DiffTests
    {
        [Fact]
        public void Diff_Added()
        {
            var left = JsonNode.Parse("{}");
            var right = JsonNode.Parse("{\"a\":1}");

            var diff = left.Diff(right);

            Assert.Equal("{\"a\":[1]}", diff!.ToJsonString());
        }

        [Fact]
        public void Diff_Modified()
        {
            var left = JsonNode.Parse("1");
            var right = JsonNode.Parse("2");

            var diff = left.Diff(right);

            Assert.Equal("[1,2]", diff!.ToJsonString());
        }

        [Fact]
        public void Diff_Deleted()
        {
            var left = JsonNode.Parse("{\"a\":1}");
            var right = JsonNode.Parse("{}");

            var diff = left.Diff(right);

            Assert.Equal("{\"a\":[1,0,0]}", diff!.ToJsonString());
        }

        [Fact]
        public void Diff_ArrayMove()
        {
            var left = JsonNode.Parse("[1,2,3]");
            var right = JsonNode.Parse("[2,1,3]");

            var diff = left.Diff(right);

            Assert.Equal("{\"_t\":\"a\",\"_0\":[\"\",1,3]}", diff!.ToJsonString());
        }
    }
}
