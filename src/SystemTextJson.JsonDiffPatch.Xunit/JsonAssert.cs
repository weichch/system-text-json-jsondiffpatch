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
        /// Tests whether two JSON objects have no difference. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="messageProvider">The message provider.</param>
        public static void Same(string? expected, string? actual,
            JsonDiffOptions? diffOptions = null,
            Func<JsonNode, string>? messageProvider = null)
            => Same(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual),
                diffOptions, messageProvider);

        /// <summary>
        /// Tests whether two JSON objects have no difference. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="messageProvider">The message provider.</param>
        public static void Same(JsonNode? expected, JsonNode? actual,
            JsonDiffOptions? diffOptions = null,
            Func<JsonNode, string>? messageProvider = null)
        {
            var diff = expected.Diff(actual, diffOptions ?? DefaultOptions);
            if (diff is null)
            {
                return;
            }

            var ex = messageProvider is null
                ? new JsonSameException(expected, actual, diff)
                : new JsonSameException(messageProvider(diff));
            throw ex;
        }
        
        /// <summary>
        /// Tests whether two JSON objects have differences. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void NotSame(string? expected, string? actual,
            JsonDiffOptions? diffOptions = null)
            => NotSame(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual), diffOptions);

        /// <summary>
        /// Tests whether two JSON objects have differences. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void NotSame(JsonNode? expected, JsonNode? actual,
            JsonDiffOptions? diffOptions = null)
        {
            var diff = expected.Diff(actual, diffOptions ?? DefaultOptions);
            if (diff is not null)
            {
                return;
            }

            throw new JsonNotSameException();
        }

        /// <summary>
        /// Tests whether two JSON objects have no difference. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="actual">The actual value.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="messageProvider">The message provider.</param>
        public static void ShouldBeSameAs<T>(this T? actual, T? expected,
            JsonDiffOptions? diffOptions = null,
            Func<JsonNode, string>? messageProvider = null)
            where T : JsonNode
            => Same(expected, actual, diffOptions, messageProvider);

        /// <summary>
        /// Tests whether two JSON objects have differences. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="actual">The actual value.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void ShouldNotBeSameAs<T>(this T? actual, T? expected,
            JsonDiffOptions? diffOptions = null)
            where T : JsonNode
            => NotSame(expected, actual, diffOptions);

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

