using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace SystemTextJson.JsonDiffPatch.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var a = JsonNode.Parse("[1,2,3,4]");
            var b = JsonNode.Parse("[1,3,2,4]");

            var diff = JsonDiffPatcher.Diff(a, b);

        }
    }
}
