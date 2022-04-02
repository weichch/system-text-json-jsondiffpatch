using System.Text.Json.Nodes;
using Xunit.Sdk;

namespace System.Text.Json.JsonDiffPatch.Xunit
{
    /// <summary>
    /// Exception thrown when two JSON objects have unexpected differences.
    /// </summary>
    public class JsonSameException : XunitException
    {
        public JsonSameException(JsonNode? expected, JsonNode? actual, JsonNode diff)
            : base(CreateUserMessage(expected, actual, diff))
        {
        }

        public JsonSameException(string message)
            : base(CreateUserMessage(message))
        {
        }

        private static string CreateUserMessage(JsonNode? expected, JsonNode? actual, JsonNode diff)
        {
            var sb = new StringBuilder();
            sb.Append("JsonAssert.Same() failure: The specified two JSON objects have differences.");
            sb.AppendLine();
            sb.Append("Expected:");
            sb.AppendLine();
            sb.Append(expected is null
                ? "null"
                : expected.ToJsonString(new JsonSerializerOptions {WriteIndented = true}));
            sb.AppendLine();
            sb.Append("Actual:");
            sb.AppendLine();
            sb.Append(actual is null
                ? "null"
                : actual.ToJsonString(new JsonSerializerOptions {WriteIndented = true}));
            sb.AppendLine();
            sb.Append("Diff:");
            sb.AppendLine();
            sb.Append(diff.ToJsonString(new JsonSerializerOptions {WriteIndented = true}));
            return sb.ToString();
        }

        private static string CreateUserMessage(string message)
        {
            var sb = new StringBuilder();
            sb.Append("JsonAssert.Same() failure: The specified two JSON objects have differences.");
            sb.AppendLine();
            sb.Append(message);
            return sb.ToString();
        }
    }
}