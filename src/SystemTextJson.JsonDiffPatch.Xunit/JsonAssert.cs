using System.Text.Json.JsonDiffPatch.Diffs.Formatters;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch.Xunit
{
    /// <summary>
    /// Provides a set of static methods to verify that JSON objects can meet criteria in tests.
    /// </summary>
    public static class JsonAssert
    {
        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        public static void Equal(string? expected, string? actual)
            => HandleEqual(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual),
                null, null);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void Equal(string? expected, string? actual, JsonDiffOptions diffOptions)
            => HandleEqual(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual),
                diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)),
                null);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="deltaFormatter">The delta formatter.</param>
        public static void Equal(string? expected, string? actual, IJsonDiffDeltaFormatter<string> deltaFormatter)
            => HandleEqual(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual), null,
                deltaFormatter ?? throw new ArgumentNullException(nameof(deltaFormatter)));

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="deltaFormatter">The delta formatter.</param>
        public static void Equal(string? expected, string? actual, JsonDiffOptions diffOptions,
            IJsonDiffDeltaFormatter<string> deltaFormatter)
            => HandleEqual(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual),
                diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)),
                deltaFormatter ?? throw new ArgumentNullException(nameof(deltaFormatter)));

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        public static void Equal<T>(T? expected, T? actual)
            where T : JsonNode
            => HandleEqual(expected, actual, null, null);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void Equal<T>(T? expected, T? actual, JsonDiffOptions diffOptions)
            where T : JsonNode
            => HandleEqual(expected, actual,
                diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)), null);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="deltaFormatter">The delta formatter.</param>
        public static void Equal<T>(T? expected, T? actual, IJsonDiffDeltaFormatter<string> deltaFormatter)
            where T : JsonNode
            => HandleEqual(expected, actual, null,
                deltaFormatter ?? throw new ArgumentNullException(nameof(deltaFormatter)));

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="deltaFormatter">The delta formatter.</param>
        public static void Equal<T>(T? expected, T? actual, JsonDiffOptions diffOptions,
            IJsonDiffDeltaFormatter<string> deltaFormatter)
            where T : JsonNode
            => HandleEqual(expected, actual,
                diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)),
                deltaFormatter ?? throw new ArgumentNullException(nameof(deltaFormatter)));

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        public static void ShouldEqual<T>(this T? actual, T? expected)
            where T : JsonNode
            => Equal(expected, actual);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void ShouldEqual<T>(this T? actual, T? expected,
            JsonDiffOptions diffOptions)
            where T : JsonNode
            => Equal(expected, actual, diffOptions);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="deltaFormatter">The delta formatter.</param>
        public static void ShouldEqual<T>(this T? actual, T? expected,
            IJsonDiffDeltaFormatter<string> deltaFormatter)
            where T : JsonNode
            => Equal(expected, actual, deltaFormatter);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="deltaFormatter">The delta formatter.</param>
        public static void ShouldEqual<T>(this T? actual, T? expected,
            JsonDiffOptions diffOptions,
            IJsonDiffDeltaFormatter<string> deltaFormatter)
            where T : JsonNode
            => Equal(expected, actual, diffOptions, deltaFormatter);

        private static void HandleEqual(JsonNode? expected, JsonNode? actual,
            JsonDiffOptions? diffOptions,
            IJsonDiffDeltaFormatter<string>? deltaFormatter)
        {
            var diff = expected.Diff(actual, diffOptions);
            if (diff is null)
            {
                return;
            }

            throw new JsonEqualException(expected, actual, diff, deltaFormatter);
        }

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        public static void NotEqual(string? expected, string? actual)
            => HandleNotEqual(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual), null);

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void NotEqual(string? expected, string? actual, JsonDiffOptions diffOptions)
            => HandleNotEqual(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual),
                diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)));

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        public static void NotEqual<T>(T? expected, T? actual)
            where T : JsonNode
            => HandleNotEqual(expected, actual, null);

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void NotEqual<T>(T? expected, T? actual, JsonDiffOptions diffOptions)
            where T : JsonNode
            => HandleNotEqual(expected, actual,
                diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)));

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        public static void ShouldNotEqual<T>(this T? actual, T? expected)
            where T : JsonNode
            => NotEqual(expected, actual);

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void ShouldNotEqual<T>(this T? actual, T? expected, JsonDiffOptions diffOptions)
            where T : JsonNode
            => NotEqual(expected, actual, diffOptions);

        private static void HandleNotEqual(JsonNode? expected, JsonNode? actual, JsonDiffOptions? diffOptions)
        {
            var diff = expected.Diff(actual, diffOptions);
            if (diff is not null)
            {
                return;
            }

            throw new JsonNotEqualException();
        }
    }
}