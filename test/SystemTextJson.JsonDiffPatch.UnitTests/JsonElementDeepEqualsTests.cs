using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using SystemTextJson.JsonDiffPatch.UnitTests.TestData;
using Xunit;

namespace SystemTextJson.JsonDiffPatch.UnitTests
{
    public class JsonElementDeepEqualsTests
    {
        [Fact]
        public void Object_Identical()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");

            Assert.True(json1.DeepEquals(json2));
        }

        [Fact]
        public void Object_Whitespace()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{  \"foo\":    \"bar\",    \"baz\":\"qux\"  }");

            Assert.True(json1.DeepEquals(json2));
        }

        [Fact]
        public void Object_PropertyOrdering()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"baz\":\"qux\",\"foo\":\"bar\"}");

            Assert.True(json1.DeepEquals(json2));
        }

        [Fact]
        public void Object_PropertyValue()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"quz\"}");

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Object_MissingProperty()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"bar\"}");

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Object_ExtraProperty()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_Identical()
        {
            var json1 = JsonNode.Parse("[1,2,3]");
            var json2 = JsonNode.Parse("[1,2,3]");

            Assert.True(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_Whitespace()
        {
            var json1 = JsonNode.Parse("[1,2,3]");
            var json2 = JsonNode.Parse("[ 1, 2, 3 ]");

            Assert.True(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_ItemOrdering()
        {
            var json1 = JsonNode.Parse("[1,2,3]");
            var json2 = JsonNode.Parse("[1,3,2]");

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_ItemValue()
        {
            var json1 = JsonNode.Parse("[1,2,3]");
            var json2 = JsonNode.Parse("[1,2,5]");

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_MissingItem()
        {
            var json1 = JsonNode.Parse("[1,2,3]");
            var json2 = JsonNode.Parse("[1,2]");

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_ExtraItem()
        {
            var json1 = JsonNode.Parse("[1,2]");
            var json2 = JsonNode.Parse("[1,2,3]");

            Assert.False(json1.DeepEquals(json2));
        }

        [Theory]
        [InlineData("1", "1", true)]
        [InlineData("1", "1.0", false)]
        [InlineData("1.1e1", "11", false)]
        public void Value_RawText(string jsonValue1, string jsonValue2, bool expected)
        {
            var json1 = JsonNode.Parse($"[{jsonValue1}]")!.AsArray()[0];
            var json2 = JsonNode.Parse($"[{jsonValue2}]")!.AsArray()[0];

            Assert.Equal(expected, json1.DeepEquals(json2));
        }

        [Theory]
        [InlineData("1", "1.000", true)]
        [InlineData("1.0", "1.000", true)]
        [InlineData("1", "1", true)]
        [InlineData("1.00", "1", true)]
        [InlineData("1.1e1", "11", true)]
        public void Value_Semantic(string jsonValue1, string jsonValue2, bool expected)
        {
            var json1 = JsonNode.Parse($"[{jsonValue1}]")!.AsArray()[0];
            var json2 = JsonNode.Parse($"[{jsonValue2}]")!.AsArray()[0];

            Assert.Equal(expected, json1.DeepEquals(json2, new JsonComparerOptions(JsonElementComparison.Semantic)));
        }

        [Theory]
        [MemberData(nameof(JsonValueTestData.ElementObjectSemanticEqual), MemberType = typeof(JsonValueTestData))]
        public void Value_ElementObjectSemanticEqual(JsonValue json1, JsonValue json2, bool expected)
        {
            Assert.Equal(expected, json1.DeepEquals(json2));
        }
        
        [Theory]
        [MemberData(nameof(JsonValueTestData.ObjectSemanticEqual), MemberType = typeof(JsonValueTestData))]
        public void Value_ObjectSemanticEqual(JsonValue json1, JsonValue json2, bool expected)
        {
            Assert.Equal(expected, json1.DeepEquals(json2));
        }
    }
}
