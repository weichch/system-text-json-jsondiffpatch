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
            var a = JsonNode.Parse("{\"prop\":[{},{},{}]}");
            var b = JsonNode.Parse("[]");

            var diff = JsonDiffPatcher.Diff(a, b);

        }
    }
}
