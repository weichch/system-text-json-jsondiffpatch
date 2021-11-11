using System;
using System.Text.Json.Nodes;
using Xunit;

namespace System.Text.Json.JsonDiffPatch.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var a = JsonNode.Parse("{\"prop\":[{},{},{}]}");
            var b = JsonNode.Parse("{}");

            var diff = JsonDiffPatch.Diff(a, b);

        }
    }
}
