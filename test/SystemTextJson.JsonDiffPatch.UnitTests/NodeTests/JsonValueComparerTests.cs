using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using Xunit;

namespace SystemTextJson.JsonDiffPatch.UnitTests.NodeTests
{
    public class JsonValueComparerTests
    {
        [Theory]
        [InlineData("1", "2")]
        [InlineData("1.0", "10")]
        [InlineData("\"2019-11-27T09:00:00.000\"", "\"2019-11-27T10:00:00.000\"")]
        [InlineData("\"819f0a05-905d-4c40-a3d7-2e033757496b\"", "\"819f0a05-905d-4c40-a3d7-2e033757496c\"")]
        [InlineData("\"Shaun is a rabbit\"", "\"Shawn is a rabbit\"")]
        [InlineData("\"2019-11-27T09:00:00.000\"", "\"Shaun is a rabbit\"")]
        [InlineData("1", "\"Shaun is a rabbit\"")]
        [InlineData("true", "\"Shaun is a rabbit\"")]
        [InlineData("true", "1")]
        [InlineData("false", "true")]
        [InlineData(null, "1")]
        [InlineData("null", "1")]
        public void Compare_LessThanZero(string? json1, string? json2)
        {
            var node1 = json1 == null ? null : JsonNode.Parse(json1)?.AsValue();
            var node2 = json2 == null ? null : JsonNode.Parse(json2)?.AsValue();

            var result = JsonValueComparer.Compare(node1, node2);

            Assert.True(result < 0);
        }

        [Theory]
        [InlineData("2", "1")]
        [InlineData("10", "1.0")]
        [InlineData("\"2019-11-27T10:00:00.000\"", "\"2019-11-27T09:00:00.000\"")]
        [InlineData("\"819f0a05-905d-4c40-a3d7-2e033757496c\"", "\"819f0a05-905d-4c40-a3d7-2e033757496b\"")]
        [InlineData("\"Shawn is a rabbit\"", "\"Shaun is a rabbit\"")]
        [InlineData("\"Shaun is a rabbit\"", "\"2019-11-27T09:00:00.000\"")]
        [InlineData("\"Shaun is a rabbit\"", "1")]
        [InlineData("\"Shaun is a rabbit\"", "true")]
        [InlineData("1", "true")]
        [InlineData("true", "false")]
        [InlineData("1", null)]
        [InlineData("1", "null")]
        public void Compare_GreaterThanZero(string? json1, string? json2)
        {
            var node1 = json1 == null ? null : JsonNode.Parse(json1)?.AsValue();
            var node2 = json2 == null ? null : JsonNode.Parse(json2)?.AsValue();

            var result = JsonValueComparer.Compare(node1, node2);

            Assert.True(result > 0);
        }

        [Theory]
        [InlineData("1", "1")]
        [InlineData("1.0", "1")]
        [InlineData("10", "1.0e1")]
        [InlineData("\"2019-11-27T00:00:00.000\"", "\"2019-11-27\"")]
        [InlineData("\"819f0a05-905d-4c40-a3d7-2e033757496b\"", "\"819F0A05-905D-4C40-A3D7-2E033757496B\"")]
        [InlineData("\"Shaun is a rabbit\"", "\"Shaun is a rabbit\"")]
        [InlineData("true", "true")]
        [InlineData("false", "false")]
        [InlineData("null", null)]
        [InlineData(null, null)]
        [InlineData("null", "null")]
        public void Compare_EqualToZero(string? json1, string? json2)
        {
            var node1 = json1 == null ? null : JsonNode.Parse(json1)?.AsValue();
            var node2 = json2 == null ? null : JsonNode.Parse(json2)?.AsValue();

            var result = JsonValueComparer.Compare(node1, node2);

            Assert.Equal(0, result);
        }
    }
}
