using System;
using System.Linq;
using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using Xunit;

namespace SystemTextJson.JsonDiffPatch.UnitTests
{
    public class DeepEqualsTests
    {
        [Fact]
        public void DeepEquals_BigArray()
        {
            var bigArrayJson = $@"[ {string.Join(",", Enumerable.Range(1, 1000))} ]";
            var bigArray = JsonNode.Parse(bigArrayJson)!;

            foreach (var item in bigArray.AsArray())
            {
                foreach (var item1 in bigArray.AsArray().Reverse())
                {
                    item.DeepEquals(item1);
                }
            }
        }
    }
}
