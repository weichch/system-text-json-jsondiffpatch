using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using Xunit;

namespace SystemTextJson.JsonDiffPatch.UnitTests.NodeTests
{
    public class ElementDeepEqualsTests
    {
        [Fact]
        public void Object_Identical()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");

            Assert.True(json1.DeepEquals(json2, default(JsonComparerOptions)));
        }

        [Fact]
        public void Object_Whitespace()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{  \"foo\":    \"bar\",    \"baz\":\"qux\"  }");

            Assert.True(json1.DeepEquals(json2, default(JsonComparerOptions)));
        }

        [Fact]
        public void Object_PropertyOrdering()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"baz\":\"qux\",\"foo\":\"bar\"}");

            Assert.True(json1.DeepEquals(json2, default(JsonComparerOptions)));
        }

        [Fact]
        public void Object_PropertyValue()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"quz\"}");

            Assert.False(json1.DeepEquals(json2, default(JsonComparerOptions)));
        }

        [Fact]
        public void Object_MissingProperty()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"bar\"}");

            Assert.False(json1.DeepEquals(json2, default(JsonComparerOptions)));
        }

        [Fact]
        public void Object_ExtraProperty()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");

            Assert.False(json1.DeepEquals(json2, default(JsonComparerOptions)));
        }

        [Fact]
        public void Array_Identical()
        {
            var json1 = JsonNode.Parse("[1,2,3]");
            var json2 = JsonNode.Parse("[1,2,3]");

            Assert.True(json1.DeepEquals(json2, default(JsonComparerOptions)));
        }

        [Fact]
        public void Array_Whitespace()
        {
            var json1 = JsonNode.Parse("[1,2,3]");
            var json2 = JsonNode.Parse("[ 1, 2, 3 ]");

            Assert.True(json1.DeepEquals(json2, default(JsonComparerOptions)));
        }

        [Fact]
        public void Array_ItemOrdering()
        {
            var json1 = JsonNode.Parse("[1,2,3]");
            var json2 = JsonNode.Parse("[1,3,2]");

            Assert.False(json1.DeepEquals(json2, default(JsonComparerOptions)));
        }

        [Fact]
        public void Array_ItemValue()
        {
            var json1 = JsonNode.Parse("[1,2,3]");
            var json2 = JsonNode.Parse("[1,2,5]");

            Assert.False(json1.DeepEquals(json2, default(JsonComparerOptions)));
        }

        [Fact]
        public void Array_MissingItem()
        {
            var json1 = JsonNode.Parse("[1,2,3]");
            var json2 = JsonNode.Parse("[1,2]");

            Assert.False(json1.DeepEquals(json2, default(JsonComparerOptions)));
        }

        [Fact]
        public void Array_ExtraItem()
        {
            var json1 = JsonNode.Parse("[1,2]");
            var json2 = JsonNode.Parse("[1,2,3]");

            Assert.False(json1.DeepEquals(json2, default(JsonComparerOptions)));
        }

        [Theory]
        [MemberData(nameof(NodeTestData.ElementRawTextEqual), MemberType = typeof(NodeTestData))]
        public void Value_RawText(JsonValue json1, JsonValue json2, bool expected)
        {
            Assert.Equal(expected, json1.DeepEquals(json2, default(JsonComparerOptions)));
        }
        
        [Theory]
        [MemberData(nameof(NodeTestData.ElementSemanticEqual), MemberType = typeof(NodeTestData))]
        public void Value_Semantic(JsonValue json1, JsonValue json2, bool expected)
        {
            Assert.Equal(expected, json1.DeepEquals(json2, JsonElementComparison.Semantic));
        }

        [Theory]
        [MemberData(nameof(NodeTestData.ElementObjectSemanticEqual), MemberType = typeof(NodeTestData))]
        public void Value_ElementObjectSemanticEqual(JsonValue json1, JsonValue json2, bool expected)
        {
            Assert.Equal(expected, json1.DeepEquals(json2, default(JsonComparerOptions)));
        }
    }
}
