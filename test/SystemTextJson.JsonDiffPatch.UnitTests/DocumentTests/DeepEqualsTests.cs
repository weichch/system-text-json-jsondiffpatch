using System.Text.Json;
using System.Text.Json.JsonDiffPatch;
using Xunit;

namespace SystemTextJson.JsonDiffPatch.UnitTests.DocumentTests
{
    public class DeepEqualsTests
    {
        [Fact]
        public void Object_Identical()
        {
            using var json1 = JsonDocument.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            using var json2 = JsonDocument.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");

            Assert.True(json1.DeepEquals(json2));
        }

        [Fact]
        public void Object_Whitespace()
        {
            using var json1 = JsonDocument.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            using var json2 = JsonDocument.Parse("{  \"foo\":    \"bar\",    \"baz\":\"qux\"  }");

            Assert.True(json1.DeepEquals(json2));
        }

        [Fact]
        public void Object_PropertyOrdering()
        {
            using var json1 = JsonDocument.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            using var json2 = JsonDocument.Parse("{\"baz\":\"qux\",\"foo\":\"bar\"}");

            Assert.True(json1.DeepEquals(json2));
        }

        [Fact]
        public void Object_PropertyValue()
        {
            using var json1 = JsonDocument.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            using var json2 = JsonDocument.Parse("{\"foo\":\"bar\",\"baz\":\"quz\"}");

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Object_MissingProperty()
        {
            using var json1 = JsonDocument.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            using var json2 = JsonDocument.Parse("{\"foo\":\"bar\"}");

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Object_ExtraProperty()
        {
            using var json1 = JsonDocument.Parse("{\"foo\":\"bar\"}");
            using var json2 = JsonDocument.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_Identical()
        {
            using var json1 = JsonDocument.Parse("[1,2,3]");
            using var json2 = JsonDocument.Parse("[1,2,3]");

            Assert.True(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_Whitespace()
        {
            using var json1 = JsonDocument.Parse("[1,2,3]");
            using var json2 = JsonDocument.Parse("[ 1, 2, 3 ]");

            Assert.True(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_ItemOrdering()
        {
            using var json1 = JsonDocument.Parse("[1,2,3]");
            using var json2 = JsonDocument.Parse("[1,3,2]");

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_ItemValue()
        {
            using var json1 = JsonDocument.Parse("[1,2,3]");
            using var json2 = JsonDocument.Parse("[1,2,5]");

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_MissingItem()
        {
            using var json1 = JsonDocument.Parse("[1,2,3]");
            using var json2 = JsonDocument.Parse("[1,2]");

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_ExtraItem()
        {
            using var json1 = JsonDocument.Parse("[1,2]");
            using var json2 = JsonDocument.Parse("[1,2,3]");

            Assert.False(json1.DeepEquals(json2));
        }

        [Theory]
        [MemberData(nameof(DocumentTestData.RawTextEqual), MemberType = typeof(DocumentTestData))]
        public void Value_RawText(JsonDocument json1, JsonDocument json2, bool expected)
        {
            using (json1)
            {
                using (json2)
                {
                    Assert.Equal(expected, json1.DeepEquals(json2));
                }
            }
        }

        [Theory]
        [MemberData(nameof(DocumentTestData.SemanticEqual), MemberType = typeof(DocumentTestData))]
        public void Value_Semantic(JsonDocument json1, JsonDocument json2, bool expected)
        {
            using (json1)
            {
                using (json2)
                {
                    Assert.Equal(expected, json1.DeepEquals(json2, JsonElementComparison.Semantic));
                }
            }
        }
    }
}
