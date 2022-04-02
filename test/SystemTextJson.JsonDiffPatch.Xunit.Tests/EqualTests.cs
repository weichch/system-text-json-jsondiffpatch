using System.Text.Json.JsonDiffPatch.Xunit;
using System.Text.Json.Nodes;
using Xunit;

namespace SystemTextJson.JsonDiffPatch.Xunit.Tests
{
    public class EqualTests
    {
        [Fact]
        public void Equal_SameObjects()
        {
            var json1 = "{\"a\":\"b\",\"c\":\"d\",\"e\":[1,2,{\"f\":\"g\",\"h\":\"i\"}]}";
            var json2 = "{\"a\":\"b\",\"c\":\"d\",\"e\":[1,2,{\"f\":\"g\",\"h\":\"i\"}]}";

            JsonAssert.Equal(json1, json2);
        }
        
        [Fact]
        public void Equal_IgnoreMemberOrdering()
        {
            var json1 = "{\"a\":\"b\",\"c\":\"d\",\"e\":[1,2,{\"f\":\"g\",\"h\":\"i\"}]}";
            var json2 = "{\"c\":\"d\",\"e\":[1,2,{\"h\":\"i\",\"f\":\"g\"}],\"a\":\"b\"}";

            JsonAssert.Equal(json1, json2);
        }
        
        [Fact]
        public void Equal_ExtensionMethod()
        {
            var json1 = JsonNode.Parse("{\"a\":\"b\",\"c\":\"d\",\"e\":[1,2,{\"f\":\"g\",\"h\":\"i\"}]}");
            var json2 = JsonNode.Parse("{\"c\":\"d\",\"e\":[1,2,{\"h\":\"i\",\"f\":\"g\"}],\"a\":\"b\"}");

            json1.ShouldEqual(json2);
        }
        
        [Fact]
        public void Equal_Nulls()
        {
            JsonNode? json1 = null;
            JsonNode? json2 = null;

            JsonAssert.Equal(json1, json2);
        }

        [Fact]
        public void Equal_FailWithMessage()
        {
            JsonNode? json1 = "[]";
            JsonNode? json2 = "{}";

            var error = Record.Exception(() => json1.ShouldEqual(json2));

            Assert.IsType<JsonEqualException>(error);
            Assert.Contains("JsonAssert.Equal() failure: The specified two JSON objects are not equal.", error.Message);
            Assert.Contains("Expected:", error.Message);
            Assert.Contains("Actual:", error.Message);
            Assert.Contains("Delta:", error.Message);
        }
        
        [Fact]
        public void NotEqual_DifferentObjects()
        {
            var json1 = "{\"a\":\"b\",\"c\":\"d\",\"e\":[1,2,{\"f\":\"g\",\"h\":\"i\"}]}";
            var json2 = "{\"a\":\"b\",\"c\":\"d\"}";

            JsonAssert.NotEqual(json1, json2);
        }

        [Fact]
        public void NotEqual_ExtensionMethod()
        {
            var json1 = JsonNode.Parse("{\"a\":\"b\",\"c\":\"d\",\"e\":[1,2,{\"f\":\"g\",\"h\":\"i\"}]}");
            var json2 = JsonNode.Parse("{\"a\":\"b\",\"c\":\"d\"}");

            json1.ShouldNotEqual(json2);
        }

        [Fact]
        public void NotEqual_Nulls()
        {
            JsonNode? json1 = null;
            JsonNode? json2 = null;

            var error = Record.Exception(() => json1.ShouldNotEqual(json2));

            Assert.NotNull(error);
        }

        [Fact]
        public void NotEqual_FailWithMessage()
        {
            var json1 = JsonNode.Parse("{\"a\":\"b\",\"c\":\"d\",\"e\":[1,2,{\"f\":\"g\",\"h\":\"i\"}]}");
            var json2 = JsonNode.Parse("{\"c\":\"d\",\"e\":[1,2,{\"h\":\"i\",\"f\":\"g\"}],\"a\":\"b\"}");

            var error = Record.Exception(() => json1.ShouldNotEqual(json2));

            Assert.IsType<JsonNotEqualException>(error);
            Assert.Contains("JsonAssert.NotEqual() failure: The specified two JSON objects are equal.", error.Message);
        }
    }
}