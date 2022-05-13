using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using Xunit;

namespace SystemTextJson.JsonDiffPatch.UnitTests.NodeTests
{
    public class ObjectDeepEqualsTests
    {
        [Fact]
        public void Default()
        {
            Assert.True(default(JsonNode).DeepEquals(default));
        }

        [Fact]
        public void Object_Identical()
        {
            var json1 = new JsonObject
            {
                {"foo", "bar"},
                {"baz", "qux"}
            };
            var json2 = new JsonObject
            {
                {"foo", "bar"},
                {"baz", "qux"}
            };

            Assert.True(json1.DeepEquals(json2));
        }

        [Fact]
        public void Object_PropertyOrdering()
        {
            var json1 = new JsonObject
            {
                {"foo", "bar"},
                {"baz", "qux"}
            };
            var json2 = new JsonObject
            {
                {"baz", "qux"},
                {"foo", "bar"}
            };

            Assert.True(json1.DeepEquals(json2));
        }

        [Fact]
        public void Object_PropertyValue()
        {
            var json1 = new JsonObject
            {
                {"foo", "bar"},
                {"baz", "qux"}
            };
            var json2 = new JsonObject
            {
                {"foo", "bar"},
                {"baz", "quz"}
            };

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Object_MissingProperty()
        {
            var json1 = new JsonObject
            {
                {"foo", "bar"},
                {"baz", "qux"}
            };
            var json2 = new JsonObject
            {
                {"foo", "bar"}
            };

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Object_ExtraProperty()
        {
            var json1 = new JsonObject
            {
                {"foo", "bar"}
            };
            var json2 = new JsonObject
            {
                {"foo", "bar"},
                {"baz", "qux"}
            };

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_Identical()
        {
            var json1 = new JsonArray {1, 2, 3};
            var json2 = new JsonArray {1, 2, 3};

            Assert.True(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_ItemOrdering()
        {
            var json1 = new JsonArray {1, 2, 3};
            var json2 = new JsonArray {1, 3, 2};

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_ItemValue()
        {
            var json1 = new JsonArray {1, 2, 3};
            var json2 = new JsonArray {1, 2, 5};

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_MissingItem()
        {
            var json1 = new JsonArray {1, 2, 3};
            var json2 = new JsonArray {1, 2};

            Assert.False(json1.DeepEquals(json2));
        }

        [Fact]
        public void Array_ExtraItem()
        {
            var json1 = new JsonArray {1, 2};
            var json2 = new JsonArray {1, 2, 3};

            Assert.False(json1.DeepEquals(json2));
        }
        
        [Theory]
        [MemberData(nameof(NodeTestData.ObjectSemanticEqual), MemberType = typeof(NodeTestData))]
        public void Value_ObjectSemanticEqual(JsonValue json1, JsonValue json2, bool expected)
        {
            Assert.Equal(expected, json1.DeepEquals(json2));
        }
    }
}
