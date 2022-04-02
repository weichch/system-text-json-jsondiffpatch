using Xunit.Sdk;

namespace System.Text.Json.JsonDiffPatch.Xunit
{
    /// <summary>
    /// Exception thrown when two JSON objects are unexpectedly equal.
    /// </summary>
    public class JsonNotEqualException : XunitException
    {
        public JsonNotEqualException()
            : base("JsonAssert.NotEqual() failure: The specified two JSON objects are equal.")
        {
        }
    }
}