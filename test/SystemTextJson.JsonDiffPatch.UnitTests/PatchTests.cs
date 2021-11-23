using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace SystemTextJson.JsonDiffPatch.UnitTests
{
    public class PatchTests
    {
        [Fact]
        public void Patch_AddedValue()
        {
            var left = JsonNode.Parse("1");
            var diff = JsonNode.Parse("[3]");

            Assert.Throws<FormatException>(
                () => JsonDiffPatcher.Patch(ref left, diff));
        }

        [Fact]
        public void Patch_ModifiedValue()
        {
            var left = JsonNode.Parse("1");
            var diff = JsonNode.Parse("[1,3]");

            JsonDiffPatcher.Patch(ref left, diff);

            Assert.Equal("3", left!.ToJsonString());
        }

        [Fact]
        public void Patch_DeletedValue()
        {
            var left = JsonNode.Parse("1");
            var diff = JsonNode.Parse("[1,0,0]");

            Assert.Throws<FormatException>(
                () => JsonDiffPatcher.Patch(ref left, diff));
        }

        [Fact]
        public void Patch_Object()
        {
            var left = JsonNode.Parse("{\"a\":{\"a2\":2,\"a3\":3}}");
            var diff = JsonNode.Parse("{\"a\":{\"a1\":[1],\"a2\":[2,3],\"a3\":[3,0,0]},\"b\":[1]}");
            var result = new JsonObject
            {
                {
                    "a", new JsonObject
                    {
                        {"a2", 3},
                        {"a1", 1},
                    }
                },
                {"b", 1}
            };

            JsonDiffPatcher.Patch(ref left, diff);

            Assert.Equal(result.ToJsonString(), left!.ToJsonString());
        }

        [Fact]
        public void Patch_Array()
        {
            var left = JsonNode.Parse("[1,2,3,4]");
            var diff = JsonNode.Parse("{\"_t\":\"a\",\"_0\":[\"\",5,3],\"_1\":[2,0,0],\"0\":[6],\"1\":[3,5],\"3\":[3],\"4\":[2]}");
            var result = new JsonArray(6,5,4,3,2,1);

            JsonDiffPatcher.Patch(ref left, diff);

            Assert.Equal(result.ToJsonString(), left!.ToJsonString());
        }

        [Fact]
        public void ReversePatch_AddedValue()
        {
            var right = JsonNode.Parse("1");
            var diff = JsonNode.Parse("[3]");

            Assert.Throws<FormatException>(
                () => JsonDiffPatcher.ReversePatch(ref right, diff));
        }

        [Fact]
        public void ReversePatch_ModifiedValue()
        {
            var right = JsonNode.Parse("3");
            var diff = JsonNode.Parse("[1,3]");

            JsonDiffPatcher.ReversePatch(ref right, diff);

            Assert.Equal("1", right!.ToJsonString());
        }

        [Fact]
        public void ReversePatch_DeletedValue()
        {
            var right = JsonNode.Parse("1");
            var diff = JsonNode.Parse("[1,0,0]");

            Assert.Throws<FormatException>(
                () => JsonDiffPatcher.ReversePatch(ref right, diff));
        }

        [Fact]
        public void ReversePatch_Object()
        {
            var right = JsonNode.Parse("{\"a\":{\"a2\":3,\"a1\":1},\"b\":1}");
            var diff = JsonNode.Parse("{\"a\":{\"a1\":[1],\"a2\":[2,3],\"a3\":[3,0,0]},\"b\":[1]}");
            var result = new JsonObject
            {
                {
                    "a",
                    new JsonObject
                    {
                        {"a2", 2},
                        {"a3", 3}
                    }
                }
            };

            JsonDiffPatcher.ReversePatch(ref right, diff);

            Assert.Equal(result.ToJsonString(), right!.ToJsonString());
        }

        [Fact]
        public void ReversePatch_Array()
        {
            var right = JsonNode.Parse("[6,5,4,3,2,1]");
            var diff = JsonNode.Parse("{\"_t\":\"a\",\"_0\":[\"\",5,3],\"_1\":[2,0,0],\"0\":[6],\"1\":[3,5],\"3\":[3],\"4\":[2]}");
            var result = new JsonArray(1, 2, 3, 4);

            JsonDiffPatcher.ReversePatch(ref right, diff);

            Assert.Equal(result.ToJsonString(), right!.ToJsonString());
        }
    }
}
