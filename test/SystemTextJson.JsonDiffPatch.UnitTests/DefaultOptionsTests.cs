using System;
using System.Text.Json.JsonDiffPatch;
using Xunit;

namespace SystemTextJson.JsonDiffPatch.UnitTests
{
    public class DefaultOptionsTests : IDisposable
    {
        private readonly JsonElementComparison _comparisonMode;

        public DefaultOptionsTests()
        {
            _comparisonMode = JsonDiffPatcher.DefaultComparison;
        }

        [Fact]
        public void DefaultDeepEqualsComparison_ComparerOptions()
        {
            JsonDiffPatcher.DefaultComparison = JsonElementComparison.Semantic;
            
            JsonComparerOptions comparerOptions = default;

            Assert.Equal(JsonDiffPatcher.DefaultComparison, comparerOptions.JsonElementComparison);
        }

        [Fact]
        public void DefaultDeepEqualsComparison_DiffOptions()
        {
            JsonDiffPatcher.DefaultComparison = JsonElementComparison.Semantic;

            var diffOptions = new JsonDiffOptions();

            Assert.Equal(JsonDiffPatcher.DefaultComparison, diffOptions.JsonElementComparison);
        }

        public void Dispose()
        {
            JsonDiffPatcher.DefaultComparison = _comparisonMode;
        }
    }
}
