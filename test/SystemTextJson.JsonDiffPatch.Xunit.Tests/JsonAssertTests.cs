using System.Text.Json.JsonDiffPatch.Xunit;
using System.Text.Json.Nodes;
using Xunit;

namespace SystemTextJson.JsonDiffPatch.Xunit.Tests
{
    public class JsonAssertTests
    {
        [Fact]
        public void Same_SameObjects()
        {
            var json1 = "{\"a\":\"b\",\"c\":\"d\",\"e\":[1,2,{\"f\":\"g\",\"h\":\"i\"}]}";
            var json2 = "{\"a\":\"b\",\"c\":\"d\",\"e\":[1,2,{\"f\":\"g\",\"h\":\"i\"}]}";

            JsonAssert.Same(json1, json2);
        }

        [Fact]
        public void Same_IgnoreMemberOrdering()
        {
            var json1 = "{\"a\":\"b\",\"c\":\"d\",\"e\":[1,2,{\"f\":\"g\",\"h\":\"i\"}]}";
            var json2 = "{\"c\":\"d\",\"e\":[1,2,{\"h\":\"i\",\"f\":\"g\"}],\"a\":\"b\"}";

            JsonAssert.Same(json1, json2);
        }

        [Fact]
        public void Same_ExtensionMethod()
        {
            var json1 = JsonNode.Parse("{\"a\":\"b\",\"c\":\"d\",\"e\":[1,2,{\"f\":\"g\",\"h\":\"i\"}]}");
            var json2 = JsonNode.Parse("{\"c\":\"d\",\"e\":[1,2,{\"h\":\"i\",\"f\":\"g\"}],\"a\":\"b\"}");

            json1.ShouldBeSameAs(json2);
        }

        [Fact]
        public void Same_Nulls()
        {
            JsonNode? json1 = null;
            JsonNode? json2 = null;

            JsonAssert.Same(json1, json2);
        }

        [Fact]
        public void Same_FailWithMessage()
        {
            JsonNode? json1 = "[]";
            JsonNode? json2 = "{}";

            var error = Record.Exception(() => json1.ShouldBeSameAs(json2));

            Assert.IsType<JsonSameException>(error);
            Assert.Contains("JsonAssert.Same() failure: The specified two JSON objects have differences.",
                error.Message);
            Assert.Contains("Expected:", error.Message);
            Assert.Contains("Actual:", error.Message);
            Assert.Contains("Diff:", error.Message);
        }

        [Fact]
        public void NotSame_DifferentObjects()
        {
            var json1 = "{\"a\":\"b\",\"c\":\"d\",\"e\":[1,2,{\"f\":\"g\",\"h\":\"i\"}]}";
            var json2 = "{\"a\":\"b\",\"c\":\"d\"}";

            JsonAssert.NotSame(json1, json2);
        }

        [Fact]
        public void NotSame_ExtensionMethod()
        {
            var json1 = JsonNode.Parse("{\"a\":\"b\",\"c\":\"d\",\"e\":[1,2,{\"f\":\"g\",\"h\":\"i\"}]}");
            var json2 = JsonNode.Parse("{\"a\":\"b\",\"c\":\"d\"}");

            json1.ShouldNotBeSameAs(json2);
        }

        [Fact]
        public void NotSame_Nulls()
        {
            JsonNode? json1 = null;
            JsonNode? json2 = null;

            var error = Record.Exception(() => json1.ShouldNotBeSameAs(json2));

            Assert.NotNull(error);
        }

        [Fact]
        public void NotSame_FailWithMessage()
        {
            var json1 = JsonNode.Parse("{\"a\":\"b\",\"c\":\"d\",\"e\":[1,2,{\"f\":\"g\",\"h\":\"i\"}]}");
            var json2 = JsonNode.Parse("{\"c\":\"d\",\"e\":[1,2,{\"h\":\"i\",\"f\":\"g\"}],\"a\":\"b\"}");

            var error = Record.Exception(() => json1.ShouldNotBeSameAs(json2));

            Assert.IsType<JsonNotSameException>(error);
            Assert.Contains("JsonAssert.NotSame() failure: The specified two JSON objects have no difference.",
                error.Message);
        }
    }
}