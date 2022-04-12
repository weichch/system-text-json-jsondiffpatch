using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using Xunit;

namespace SystemTextJson.JsonDiffPatch.UnitTests
{
    public class DeepEqualsTests
    {
        [Fact]
        public void Object_JsonElement_Identical()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            
            Assert.True(json1.DeepEquals(json2));
        }
        
        [Fact]
        public void Object_JsonElement_Whitespace()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{  \"foo\":    \"bar\",    \"baz\":\"qux\"  }");
            
            Assert.True(json1.DeepEquals(json2));
        }
        
        [Fact]
        public void Object_JsonElement_PropertyOrdering()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"baz\":\"qux\",\"foo\":\"bar\"}");
            
            Assert.True(json1.DeepEquals(json2));
        }
        
        [Fact]
        public void Object_JsonElement_PropertyValue()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"quz\"}");
            
            Assert.False(json1.DeepEquals(json2));
        }
        
        [Fact]
        public void Object_JsonElement_MissingProperty()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"bar\"}");
            
            Assert.False(json1.DeepEquals(json2));
        }
        
        [Fact]
        public void Object_JsonElement_ExtraProperty()
        {
            var json1 = JsonNode.Parse("{\"foo\":\"bar\"}");
            var json2 = JsonNode.Parse("{\"foo\":\"bar\",\"baz\":\"qux\"}");

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_JsonElement_Identical()
        {
            var json1 = JsonNode.Parse("[1,2,3]");
            var json2 = JsonNode.Parse("[1,2,3]");
            
            Assert.True(json1.DeepEquals(json2));
        }
        
        [Fact]
        public void Array_JsonElement_Whitespace()
        {
            var json1 = JsonNode.Parse("[1,2,3]");
            var json2 = JsonNode.Parse("[ 1, 2, 3 ]");
            
            Assert.True(json1.DeepEquals(json2));
        }
        
        [Fact]
        public void Array_JsonElement_ItemOrdering()
        {
            var json1 = JsonNode.Parse("[1,2,3]");
            var json2 = JsonNode.Parse("[1,3,2]");
            
            Assert.False(json1.DeepEquals(json2));
        }
        
        [Fact]
        public void Array_JsonElement_ItemValue()
        {
            var json1 = JsonNode.Parse("[1,2,3]");
            var json2 = JsonNode.Parse("[1,2,5]");
            
            Assert.False(json1.DeepEquals(json2));
        }
        
        [Fact]
        public void Array_JsonElement_MissingItem()
        {
            var json1 = JsonNode.Parse("[1,2,3]");
            var json2 = JsonNode.Parse("[1,2]");
            
            Assert.False(json1.DeepEquals(json2));
        }
        
        [Fact]
        public void Array_JsonElement_ExtraItem()
        {
            var json1 = JsonNode.Parse("[1,2]");
            var json2 = JsonNode.Parse("[1,2,3]");

            Assert.False(json1.DeepEquals(json2));
        }
        
        [Fact]
        public void Value_JsonElement_RawText()
        {
            var json1 = JsonNode.Parse("[1]")!.AsArray()[0];
            var json2 = JsonNode.Parse("[1.0]")!.AsArray()[0];

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Value_JsonElement_Semantic()
        {
            var json1 = JsonNode.Parse("[1]")!.AsArray()[0];
            var json2 = JsonNode.Parse("[1.0]")!.AsArray()[0];

            Assert.True(json1.DeepEquals(json2, new JsonComparerOptions(JsonElementComparison.Semantic)));
        }

        [Fact]
        public void Value_JsonElement_IntegerInstance()
        {
            var json1 = JsonNode.Parse("[1]")!.AsArray()[0];
            var json2 = JsonValue.Create(1);
            
            Assert.True(json1.DeepEquals(json2));
        }
        
        [Fact]
        public void Value_JsonElement_LongInstance()
        {
            var json1 = JsonNode.Parse("[1]")!.AsArray()[0];
            var json2 = JsonValue.Create(1L);
            
            Assert.True(json1.DeepEquals(json2));
        }
        
        [Fact]
        public void Value_JsonElement_DecimalInstance()
        {
            var json1 = JsonNode.Parse("[1]")!.AsArray()[0];
            var json2 = JsonValue.Create(1.0m);
            
            Assert.True(json1.DeepEquals(json2));
        }
        
        [Fact]
        public void Value_JsonElement_DoubleInstance()
        {
            var json1 = JsonNode.Parse("[1]")!.AsArray()[0];
            var json2 = JsonValue.Create(1.0d);
            
            Assert.True(json1.DeepEquals(json2));
        }
        
        [Fact]
        public void Value_JsonElement_TrailingZero()
        {
            var json1 = JsonNode.Parse("[1.0]")!.AsArray()[0];
            var json2 = JsonValue.Create(1);
            
            Assert.True(json1.DeepEquals(json2));
        }
    }
}
