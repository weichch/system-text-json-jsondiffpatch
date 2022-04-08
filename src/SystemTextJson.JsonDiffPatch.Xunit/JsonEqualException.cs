using Xunit.Sdk;

namespace System.Text.Json.JsonDiffPatch.Xunit
{
    /// <summary>
    /// Exception thrown when two JSON objects are unexpectedly not equal.
    /// </summary>
    public class JsonEqualException : XunitException
    {
        public JsonEqualException()
            : base("JsonAssert.Equal() failure.")
        {

        }

        public JsonEqualException(string output)
            : base($"JsonAssert.Equal() failure.{Environment.NewLine}{output}")
        {
        }
    }
}