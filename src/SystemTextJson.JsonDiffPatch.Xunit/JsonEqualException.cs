using System.Text.Json.Nodes;
using Xunit.Sdk;

namespace System.Text.Json.JsonDiffPatch.Xunit
{
    /// <summary>
    /// Exception thrown when two JSON objects are unexpectedly not equal.
    /// </summary>
    public class JsonEqualException : XunitException
    {
        public JsonEqualException(JsonNode? expected, JsonNode? actual, JsonNode diff)
            : base(CreateUserMessage(expected, actual, diff))
        {
        }

        public JsonEqualException(string message)
            : base(CreateUserMessage(message))
        {
        }

        private static string CreateUserMessage(JsonNode? expected, JsonNode? actual, JsonNode diff)
        {
            var sb = new StringBuilder();
            sb.Append("JsonAssert.Equal() failure: The specified two JSON objects are not equal.");
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
            sb.Append("Delta:");
            sb.AppendLine();
            sb.Append(diff.ToJsonString(new JsonSerializerOptions {WriteIndented = true}));
            return sb.ToString();
        }

        private static string CreateUserMessage(string message)
        {
            var sb = new StringBuilder();
            sb.Append("JsonAssert.Equal() failure: The specified two JSON objects are not equal.");
            sb.AppendLine();
            sb.Append(message);
            return sb.ToString();
        }
    }
}