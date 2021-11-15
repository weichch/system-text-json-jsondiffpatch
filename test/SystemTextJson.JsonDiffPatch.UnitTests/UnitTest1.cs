using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;
using Xunit.Abstractions;

namespace SystemTextJson.JsonDiffPatch.UnitTests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test1()
        {
            var json = $"{{ \"t\":\"abcd\", \"a\" : [{string.Join(",", Enumerable.Range(1, 1000))}] }}";
            var left = JsonNode.Parse(json);
            var right = JsonNode.Parse("{ \"t\":\"abcde\", \"a\" : [] }");


            var sw = Stopwatch.StartNew();
            var diff = JsonDiffPatcher.Diff(left, right, new JsonDiffOptions
            {
                TextDiffMinLength = 10
            });
            sw.Stop();

            _testOutputHelper.WriteLine(sw.Elapsed.TotalMilliseconds.ToString());
        }
    }
}
