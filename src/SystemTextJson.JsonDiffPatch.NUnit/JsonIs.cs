using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch.Nunit
{
    /// <summary>
    /// Provides methods to create JSON assert constraints.
    /// </summary>
    public static class JsonIs
    {
        /// <summary>
        /// Returns a constraint that tests whether two JSON are equal.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        public static JsonDiffConstraint EqualTo(JsonNode? expected) => new JsonEqualConstraint(expected);
        
        /// <summary>
        /// Returns a constraint that tests whether two JSON are not equal.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        public static JsonDiffConstraint NotEqualTo(JsonNode? expected) => new JsonNotEqualConstraint(expected);
    }
}