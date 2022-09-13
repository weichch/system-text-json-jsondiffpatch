using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using Xunit;

namespace SystemTextJson.JsonDiffPatch.UnitTests.NodeTests
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

        [Fact]
        public void Diff_NullProperty()
        {
            var left = JsonNode.Parse("{\"a\":1}");
            var right = JsonNode.Parse("{\"a\":null}");

            var diff = left.Diff(right);

            Assert.Equal("{\"a\":[1,null]}", diff!.ToJsonString());
        }

        [Fact]
        public void Diff_NullArrayItem()
        {
            var left = JsonNode.Parse("[1]");
            var right = JsonNode.Parse("[null]");

            var diff = left.Diff(right);

            Assert.Equal("{\"_t\":\"a\",\"_0\":[1,0,0],\"0\":[null]}", diff!.ToJsonString());
        }

        [Fact]
        public void PropertyFilter_LeftProperty()
        {
            var left = JsonNode.Parse("{\"a\":1}");
            var right = JsonNode.Parse("{}");

            var diff = left.Diff(right, new JsonDiffOptions
            {
                PropertyFilter = (prop, _) => !string.Equals(prop, "a")
            });

            Assert.Null(diff);
        }

        [Fact]
        public void PropertyFilter_RightProperty()
        {
            var left = JsonNode.Parse("{}");
            var right = JsonNode.Parse("{\"a\":1}");

            var diff = left.Diff(right, new JsonDiffOptions
            {
                PropertyFilter = (prop, _) => !string.Equals(prop, "a")
            });

            Assert.Null(diff);
        }

        [Fact]
        public void PropertyFilter_NestedProperty()
        {
            var left = JsonNode.Parse("{\"foo\":{\"bar\":{\"a\":1}}}");
            var right = JsonNode.Parse("{\"foo\":{\"bar\":{\"a\":2}}}");

            var diff = left.Diff(right, new JsonDiffOptions
            {
                PropertyFilter = (prop, _) => !string.Equals(prop, "a")
            });

            Assert.Null(diff);
        }

        [Fact]
        public void PropertyFilter_ArrayItem()
        {
            var left = JsonNode.Parse("[{\"a\":1}]");
            var right = JsonNode.Parse("[{\"a\":2}]");

            var diff = left.Diff(right, new JsonDiffOptions
            {
                PropertyFilter = (prop, _) => !string.Equals(prop, "a")
            });

            Assert.Null(diff);
        }

        [Fact]
        public void PropertyFilter_ArrayItem_ExplicitFallbackMatch()
        {
            var left = JsonNode.Parse("[{\"a\":1}]");
            var right = JsonNode.Parse("[{\"a\":2}]");

            var diff = left.Diff(right, new JsonDiffOptions
            {
                ArrayObjectItemMatchByPosition = true,
                PropertyFilter = (prop, _) => !string.Equals(prop, "a")
            });

            Assert.Null(diff);
        }
    }
}
