using System.Text.Json.JsonDiffPatch.Xunit;
using System.Text.Json.Nodes;
using Xunit;

namespace SystemTextJson.JsonDiffPatch.Xunit.Tests
{
    public class JsonAssertTests
    {
        [Fact]
        public void Equal_String()
        {
            var json1 = "{\"foo\":\"bar\",\"baz\":\"qux\"}";
            var json2 = "{\"baz\":\"qux\",\"foo\":\"bar\"}";

            JsonAssert.Equal(json1, json2);
        }
        
        [Fact]
        public void Equal_JsonNode()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"baz\":\"qux\",\"foo\":\"bar\"}");

            JsonAssert.Equal(json1, json2);
        }

        [Fact]
        public void Equal_ExtensionMethod()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"baz\":\"qux\",\"foo\":\"bar\"}");

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
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"baz\"}");

            var error = Record.Exception(() => json1.ShouldEqual(json2));

            Assert.IsType<JsonEqualException>(error);
            Assert.Contains("JsonAssert.Equal() failure.", error.Message);
        }

        [Fact]
        public void Equal_FailWithDefaultOutput()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"baz\"}");

            var error = Record.Exception(() => json1.ShouldEqual(json2, true));

            Assert.IsType<JsonEqualException>(error);
            Assert.Contains("JsonAssert.Equal() failure.", error.Message);
            Assert.Contains("Expected:", error.Message);
            Assert.Contains("Actual:", error.Message);
            Assert.Contains("Delta:", error.Message);
        }

        [Fact]
        public void Equal_FailWithCustomOutput()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"baz\"}");

            var error = Record.Exception(() => json1.ShouldEqual(json2,
                (e, a, d) => "Custom message"));

            Assert.IsType<JsonEqualException>(error);
            Assert.Contains("JsonAssert.Equal() failure.", error.Message);
            Assert.Contains("Custom message", error.Message);
        }

        [Fact]
        public void NotEqual_String()
        {
            var json1 = "{\"foo\":\"bar\",\"baz\":\"qux\"}";
            var json2 = "{\"foo\":\"baz\"}";

            JsonAssert.NotEqual(json1, json2);
        }
        
        [Fact]
        public void NotEqual_JsonNode()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"baz\"}");

            JsonAssert.NotEqual(json1, json2);
        }

        [Fact]
        public void NotEqual_ExtensionMethod()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"baz\"}");

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
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"baz\":\"qux\",\"foo\":\"bar\"}");

            var error = Record.Exception(() => json1.ShouldNotEqual(json2));

            Assert.IsType<JsonNotEqualException>(error);
            Assert.Contains("JsonAssert.NotEqual() failure.", error.Message);
        }
    }
}