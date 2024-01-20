using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json.JsonDiffPatch.Xunit
{
    /// <summary>
    /// Provides a set of static methods to verify that JSON objects can meet criteria in tests.
    /// </summary>
    public static class JsonAssert
    {
        private static readonly JsonSerializerOptions SerializerOptions;

        static JsonAssert()
        {
            SerializerOptions = new()
            {
                TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
                WriteIndented = true
            };

            SerializerOptions.MakeReadOnly();
        }

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        public static void Equal(string? expected, string? actual)
            => Equal(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual));

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="output">Whether to print diff result.</param>
        public static void Equal(string? expected, string? actual, bool output)
            => Equal(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual), output);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void Equal(string? expected, string? actual, JsonDiffOptions diffOptions)
            => Equal(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual), diffOptions);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="output">Whether to print diff result.</param>
        public static void Equal(string? expected, string? actual, JsonDiffOptions diffOptions, bool output)
            => Equal(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual), diffOptions, output);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="outputFormatter">The output formatter.</param>
        public static void Equal(string? expected, string? actual, Func<JsonNode, string> outputFormatter)
            => Equal(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual), outputFormatter);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="outputFormatter">The output formatter.</param>
        public static void Equal(string? expected, string? actual, JsonDiffOptions diffOptions,
            Func<JsonNode, string> outputFormatter)
            => Equal(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual), diffOptions, outputFormatter);

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
        /// <param name="output">Whether to print diff result.</param>
        public static void Equal<T>(T? expected, T? actual, bool output)
            where T : JsonNode
            => HandleEqual(expected, actual, null,
                output ? delta => CreateDefaultOutput(expected, actual, delta) : null);

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
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="output">Whether to print diff result.</param>
        public static void Equal<T>(T? expected, T? actual, JsonDiffOptions diffOptions, bool output)
            where T : JsonNode
            => HandleEqual(expected, actual,
                diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)),
                output ? delta => CreateDefaultOutput(expected, actual, delta) : null);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="outputFormatter">The output formatter.</param>
        public static void Equal<T>(T? expected, T? actual, Func<JsonNode, string> outputFormatter)
            where T : JsonNode
            => HandleEqual(expected, actual, null,
                outputFormatter ?? throw new ArgumentNullException(nameof(outputFormatter)));

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="outputFormatter">The output formatter.</param>
        public static void Equal<T>(T? expected, T? actual, JsonDiffOptions diffOptions,
            Func<JsonNode, string> outputFormatter)
            where T : JsonNode
            => HandleEqual(expected, actual,
                diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)),
                outputFormatter ?? throw new ArgumentNullException(nameof(outputFormatter)));

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
        /// <param name="output">Whether to print diff result.</param>
        public static void ShouldEqual<T>(this T? actual, T? expected, bool output)
            where T : JsonNode
            => Equal(expected, actual, output);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void ShouldEqual<T>(this T? actual, T? expected, JsonDiffOptions diffOptions)
            where T : JsonNode
            => Equal(expected, actual, diffOptions);
        
        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="output">Whether to print diff result.</param>
        public static void ShouldEqual<T>(this T? actual, T? expected, JsonDiffOptions diffOptions, bool output)
            where T : JsonNode
            => Equal(expected, actual, diffOptions, output);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="outputFormatter">The output formatter.</param>
        public static void ShouldEqual<T>(this T? actual, T? expected,
            Func<JsonNode, string> outputFormatter)
            where T : JsonNode
            => Equal(expected, actual, outputFormatter);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="outputFormatter">The output formatter.</param>
        public static void ShouldEqual<T>(this T? actual, T? expected,
            JsonDiffOptions diffOptions,
            Func<JsonNode, string> outputFormatter)
            where T : JsonNode
            => Equal(expected, actual, diffOptions, outputFormatter);

        private static void HandleEqual(JsonNode? expected, JsonNode? actual,
            JsonDiffOptions? diffOptions,
            Func<JsonNode, string>? outputFormatter)
        {
            var diff = expected.Diff(actual, diffOptions);
            if (diff is null)
            {
                return;
            }

            var output = outputFormatter?.Invoke(diff);
            if (output is null)
            {
                throw new JsonEqualException();
            }

            throw new JsonEqualException(output);
        }

        private static string CreateDefaultOutput(JsonNode? expected, JsonNode? actual, JsonNode diff)
        {
            var sb = new StringBuilder();
            
            sb.Append("Expected:");
            sb.AppendLine();
            sb.Append(expected is null
                ? "null"
                : expected.ToJsonString(SerializerOptions));
            sb.AppendLine();

            sb.Append("Actual:");
            sb.AppendLine();
            sb.Append(actual is null
                ? "null"
                : actual.ToJsonString(SerializerOptions));
            sb.AppendLine();

            sb.Append("Delta:");
            sb.AppendLine();
            sb.Append(diff.ToJsonString(SerializerOptions));

            return sb.ToString();
        }

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        public static void NotEqual(string? expected, string? actual)
            => NotEqual(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual));

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void NotEqual(string? expected, string? actual, JsonDiffOptions diffOptions)
            => NotEqual(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual), diffOptions);

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