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
            _comparisonMode = JsonDiffPatcher.DefaultDeepEqualsComparison;
        }

        [Fact]
        public void DefaultDeepEqualsComparison_ComparerOptions()
        {
            JsonDiffPatcher.DefaultDeepEqualsComparison = JsonElementComparison.Semantic;
            
            JsonComparerOptions comparerOptions = default;

            Assert.Equal(JsonDiffPatcher.DefaultDeepEqualsComparison, comparerOptions.JsonElementComparison);
        }

        [Fact]
        public void DefaultDeepEqualsComparison_DiffOptions()
        {
            JsonDiffPatcher.DefaultDeepEqualsComparison = JsonElementComparison.Semantic;

            var diffOptions = new JsonDiffOptions();

            Assert.Equal(JsonDiffPatcher.DefaultDeepEqualsComparison, diffOptions.JsonElementComparison);
        }

        public void Dispose()
        {
            JsonDiffPatcher.DefaultDeepEqualsComparison = _comparisonMode;
        }
    }
}