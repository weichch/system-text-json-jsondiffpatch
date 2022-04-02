using Xunit.Sdk;

namespace System.Text.Json.JsonDiffPatch.Xunit
{
    /// <summary>
    /// Exception thrown when two JSON objects unexpectedly have no difference.
    /// </summary>
    public class JsonNotSameException : XunitException
    {
        public JsonNotSameException()
            : base("JsonAssert.NotSame() failure: The specified two JSON objects have no difference.")
        {
        }
    }
}