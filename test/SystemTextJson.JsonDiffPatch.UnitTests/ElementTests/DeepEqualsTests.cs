using System.Text.Json;
using System.Text.Json.JsonDiffPatch;
using Xunit;

namespace SystemTextJson.JsonDiffPatch.UnitTests.ElementTests
{
    public class DeepEqualsTests
    {
        [Fact]
        public void Default()
        {
            Assert.True(default(JsonElement).DeepEquals(default));
        }

        [Fact]
        public void Object_Identical()
        {
            var json1 = JsonSerializer.Deserialize<JsonElement>("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonSerializer.Deserialize<JsonElement>("{\"foo\":\"bar\",\"baz\":\"qux\"}");

            Assert.True(json1.DeepEquals(json2));
        }

        [Fact]
        public void Object_Whitespace()
        {
            var json1 = JsonSerializer.Deserialize<JsonElement>("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonSerializer.Deserialize<JsonElement>("{  \"foo\":    \"bar\",    \"baz\":\"qux\"  }");

            Assert.True(json1.DeepEquals(json2));
        }

        [Fact]
        public void Object_PropertyOrdering()
        {
            var json1 = JsonSerializer.Deserialize<JsonElement>("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonSerializer.Deserialize<JsonElement>("{\"baz\":\"qux\",\"foo\":\"bar\"}");

            Assert.True(json1.DeepEquals(json2));
        }

        [Fact]
        public void Object_PropertyValue()
        {
            var json1 = JsonSerializer.Deserialize<JsonElement>("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonSerializer.Deserialize<JsonElement>("{\"foo\":\"bar\",\"baz\":\"quz\"}");

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Object_MissingProperty()
        {
            var json1 = JsonSerializer.Deserialize<JsonElement>("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonSerializer.Deserialize<JsonElement>("{\"foo\":\"bar\"}");

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Object_ExtraProperty()
        {
            var json1 = JsonSerializer.Deserialize<JsonElement>("{\"foo\":\"bar\"}");
            var json2 = JsonSerializer.Deserialize<JsonElement>("{\"foo\":\"bar\",\"baz\":\"qux\"}");

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_Identical()
        {
            var json1 = JsonSerializer.Deserialize<JsonElement>("[1,2,3]");
            var json2 = JsonSerializer.Deserialize<JsonElement>("[1,2,3]");

            Assert.True(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_Whitespace()
        {
            var json1 = JsonSerializer.Deserialize<JsonElement>("[1,2,3]");
            var json2 = JsonSerializer.Deserialize<JsonElement>("[ 1, 2, 3 ]");

            Assert.True(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_ItemOrdering()
        {
            var json1 = JsonSerializer.Deserialize<JsonElement>("[1,2,3]");
            var json2 = JsonSerializer.Deserialize<JsonElement>("[1,3,2]");

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_ItemValue()
        {
            var json1 = JsonSerializer.Deserialize<JsonElement>("[1,2,3]");
            var json2 = JsonSerializer.Deserialize<JsonElement>("[1,2,5]");

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_MissingItem()
        {
            var json1 = JsonSerializer.Deserialize<JsonElement>("[1,2,3]");
            var json2 = JsonSerializer.Deserialize<JsonElement>("[1,2]");

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_ExtraItem()
        {
            var json1 = JsonSerializer.Deserialize<JsonElement>("[1,2]");
            var json2 = JsonSerializer.Deserialize<JsonElement>("[1,2,3]");

            Assert.False(json1.DeepEquals(json2));
        }

        [Theory]
        [MemberData(nameof(ElementTestData.RawTextEqual), MemberType = typeof(ElementTestData))]
        public void Value_RawText(JsonElement json1, JsonElement json2, bool expected)
        {
            Assert.Equal(expected, json1.DeepEquals(json2));
        }
        
        [Theory]
        [MemberData(nameof(ElementTestData.SemanticEqual), MemberType = typeof(ElementTestData))]
        public void Value_Semantic(JsonElement json1, JsonElement json2, bool expected)
        {
            Assert.Equal(expected, json1.DeepEquals(json2, JsonElementComparison.Semantic));
        }
    }
}
