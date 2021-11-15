using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;
using Xunit.Abstractions;

namespace SystemTextJson.JsonDiffPatch.UnitTests
{
    public class DiffTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DiffTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            // Initialize the patcher type
            JsonDiffPatcher.Diff(null, null);
        }

        [Fact]
        public void Diff_DemoJson()
        {
            // Compare the two JSON objects from https://benjamine.github.io/jsondiffpatch/demo/index.html
            var left = JsonNode.Parse(File.ReadAllText(@"Examples\demo_left.json"));
            var right = JsonNode.Parse(File.ReadAllText(@"Examples\demo_right.json"));
            var result = JsonNode.Parse(File.ReadAllText(@"Examples\demo_result.json"));

            var sw = Stopwatch.StartNew();
            var diff = JsonDiffPatcher.Diff(left, right, new JsonDiffOptions
            {
                TextDiffMinLength = 60,
                // https://github.com/benjamine/jsondiffpatch/blob/a8cde4c666a8a25d09d8f216c7f19397f2e1b569/docs/demo/demo.js#L163
                ArrayObjectItemKeyFinder = (n, i) =>
                {
                    if (n is JsonObject obj
                        && obj.TryGetPropertyValue("name", out var value))
                    {
                        return value?.GetValue<string>() ?? "";
                    }

                    return null;
                }
            });
            sw.Stop();

            var time = sw.ElapsedMilliseconds == 0
                ? $"{sw.ElapsedTicks} ticks"
                : $"{sw.Elapsed.TotalMilliseconds}ms";
            _testOutputHelper.WriteLine($"Diff completed in {time}");

            Assert.True(result.DeepEquals(diff));
        }
    }
}
