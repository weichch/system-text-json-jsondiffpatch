using System.Text.Json.JsonDiffPatch.Diffs;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch.Xunit
{
    /// <summary>
    /// Provides a set of static methods to verify that JSON objects can meet criteria in tests.
    /// </summary>
    public static class JsonAssert
    {
        private static readonly JsonDiffOptions DefaultOptions = new JsonDiffOptions
        {
            SuppressDetectArrayMove = true,
            TextDiffMinLength = 0,
            ArrayItemMatcher = DefaultArrayItemComparer
        };
        
        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="messageProvider">The message provider.</param>
        public static void Equal(string? expected, string? actual,
            JsonDiffOptions? diffOptions = null,
            Func<JsonNode, string>? messageProvider = null)
            => Equal(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual),
                diffOptions, messageProvider);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="messageProvider">The message provider.</param>
        public static void Equal(JsonNode? expected, JsonNode? actual,
            JsonDiffOptions? diffOptions = null,
            Func<JsonNode, string>? messageProvider = null)
        {
            var diff = expected.Diff(actual, diffOptions ?? DefaultOptions);
            if (diff is null)
            {
                return;
            }

            var ex = messageProvider is null
                ? new JsonEqualException(expected, actual, diff)
                : new JsonEqualException(messageProvider(diff));
            throw ex;
        }
        
        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void NotEqual(string? expected, string? actual,
            JsonDiffOptions? diffOptions = null)
            => NotEqual(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual), diffOptions);

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void NotEqual(JsonNode? expected, JsonNode? actual,
            JsonDiffOptions? diffOptions = null)
        {
            var diff = expected.Diff(actual, diffOptions ?? DefaultOptions);
            if (diff is not null)
            {
                return;
            }

            throw new JsonNotEqualException();
        }

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="actual">The actual value.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="messageProvider">The message provider.</param>
        public static void ShouldEqual<T>(this T? actual, T? expected,
            JsonDiffOptions? diffOptions = null,
            Func<JsonNode, string>? messageProvider = null)
            where T : JsonNode
            => Equal(expected, actual, diffOptions, messageProvider);

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="actual">The actual value.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void ShouldNotEqual<T>(this T? actual, T? expected,
            JsonDiffOptions? diffOptions = null)
            where T : JsonNode
            => NotEqual(expected, actual, diffOptions);

        private static bool DefaultArrayItemComparer(ref ArrayItemMatchContext context)
        {
            // A quick comparison and might generate more compact delta
            if (context.Left.DeepEquals(context.Right)
                || (context.Left is JsonObject && context.Right is JsonObject)
                || (context.Left is JsonArray && context.Right is JsonArray))
            {
                return true;
            }

            return false;
        }
    }
}

