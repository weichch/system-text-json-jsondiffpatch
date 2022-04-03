using System.Text.Json.JsonDiffPatch.Diffs;
using System.Text.Json.JsonDiffPatch.Diffs.Formatters;
using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Text.Json.JsonDiffPatch.MsTest
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
        public static void AreEqual(string? expected, string? actual)
            => HandleAreEqual(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual),
                null, null, null);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void AreEqual(string? expected, string? actual, JsonDiffOptions diffOptions)
            => HandleAreEqual(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual),
                diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)),
                null, null);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="deltaFormatter">The delta formatter.</param>
        public static void AreEqual(string? expected, string? actual, IJsonDiffDeltaFormatter<string> deltaFormatter)
            => HandleAreEqual(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual), null,
                deltaFormatter ?? throw new ArgumentNullException(nameof(deltaFormatter)), null);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="deltaFormatter">The delta formatter.</param>
        public static void AreEqual(string? expected, string? actual, JsonDiffOptions diffOptions,
            IJsonDiffDeltaFormatter<string> deltaFormatter)
            => HandleAreEqual(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual),
                diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)),
                deltaFormatter ?? throw new ArgumentNullException(nameof(deltaFormatter)), null);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="message">The failure message.</param>
        public static void AreEqual(string? expected, string? actual, string message)
            => HandleAreEqual(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual),
                null, null, message ?? throw new ArgumentNullException(nameof(message)));

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="message">The failure message.</param>
        public static void AreEqual(string? expected, string? actual, JsonDiffOptions diffOptions, string message)
            => HandleAreEqual(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual),
                diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)),
                null, message ?? throw new ArgumentNullException(nameof(message)));

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        public static void AreEqual<T>(T? expected, T? actual)
            where T : JsonNode
            => HandleAreEqual(expected, actual, null, null, null);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void AreEqual<T>(T? expected, T? actual, JsonDiffOptions diffOptions)
            where T : JsonNode
            => HandleAreEqual(expected, actual,
                diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)),
                null, null);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="deltaFormatter">The delta formatter.</param>
        public static void AreEqual<T>(T? expected, T? actual,
            IJsonDiffDeltaFormatter<string> deltaFormatter)
            where T : JsonNode
            => HandleAreEqual(expected, actual, null,
                deltaFormatter ?? throw new ArgumentNullException(nameof(deltaFormatter)), null);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="deltaFormatter">The delta formatter.</param>
        public static void AreEqual<T>(T? expected, T? actual, JsonDiffOptions diffOptions,
            IJsonDiffDeltaFormatter<string> deltaFormatter)
            where T : JsonNode
            => HandleAreEqual(expected, actual,
                diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)),
                deltaFormatter ?? throw new ArgumentNullException(nameof(deltaFormatter)), null);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="message">The failure message.</param>
        public static void AreEqual<T>(T? expected, T? actual, string message)
            where T : JsonNode
            => HandleAreEqual(expected, actual, null, null,
                message ?? throw new ArgumentNullException(nameof(message)));

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="message">The failure message.</param>
        public static void AreEqual<T>(T? expected, T? actual, JsonDiffOptions diffOptions,
            string message)
            where T : JsonNode
            => HandleAreEqual(expected, actual,
                diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)),
                null, message ?? throw new ArgumentNullException(nameof(message)));

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="assert">The assert object.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        public static void JsonAreEqual(this Assert assert, string? expected, string? actual)
            => AreEqual(expected, actual);
        
        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="assert">The assert object.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void JsonAreEqual(this Assert assert, string? expected, string? actual,
            JsonDiffOptions diffOptions)
            => AreEqual(expected, actual, diffOptions);
        
        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="assert">The assert object.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="deltaFormatter">The delta formatter.</param>
        public static void JsonAreEqual(this Assert assert, string? expected, string? actual,
            IJsonDiffDeltaFormatter<string> deltaFormatter)
            => AreEqual(expected, actual, deltaFormatter);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="assert">The assert object.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="deltaFormatter">The delta formatter.</param>
        public static void JsonAreEqual(this Assert assert, string? expected, string? actual,
            JsonDiffOptions diffOptions,
            IJsonDiffDeltaFormatter<string> deltaFormatter)
            => AreEqual(expected, actual, diffOptions, deltaFormatter);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="assert">The assert object.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="message">The failure message.</param>
        public static void JsonAreEqual(this Assert assert, string? expected, string? actual, string message)
            => AreEqual(expected, actual, message);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="assert">The assert object.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="message">The failure message.</param>
        public static void JsonAreEqual(this Assert assert, string? expected, string? actual,
            JsonDiffOptions diffOptions, string message)
            => AreEqual(expected, actual, diffOptions, message);
        
        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="assert">The assert object.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        public static void JsonAreEqual<T>(this Assert assert, T? expected, T? actual)
            where T : JsonNode
            => AreEqual(expected, actual);
        
        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="assert">The assert object.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void JsonAreEqual<T>(this Assert assert, T? expected, T? actual,
            JsonDiffOptions diffOptions)
            where T : JsonNode
            => AreEqual(expected, actual, diffOptions);
        
        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="assert">The assert object.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="deltaFormatter">The delta formatter.</param>
        public static void JsonAreEqual<T>(this Assert assert, T? expected, T? actual,
            IJsonDiffDeltaFormatter<string> deltaFormatter)
            where T : JsonNode
            => AreEqual(expected, actual, deltaFormatter);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="assert">The assert object.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="deltaFormatter">The delta formatter.</param>
        public static void JsonAreEqual<T>(this Assert assert, T? expected, T? actual,
            JsonDiffOptions diffOptions,
            IJsonDiffDeltaFormatter<string> deltaFormatter)
            where T : JsonNode
            => AreEqual(expected, actual, diffOptions, deltaFormatter);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="assert">The assert object.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="message">The failure message.</param>
        public static void JsonAreEqual<T>(this Assert assert, T? expected, T? actual, string message)
            where T : JsonNode
            => AreEqual(expected, actual, message);

        /// <summary>
        /// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="assert">The assert object.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="message">The failure message.</param>
        public static void JsonAreEqual<T>(this Assert assert, T? expected, T? actual,
            JsonDiffOptions diffOptions, string message)
            where T : JsonNode
            => AreEqual(expected, actual, diffOptions, message);
        
        private static void HandleAreEqual(JsonNode? expected, JsonNode? actual,
            JsonDiffOptions? diffOptions,
            IJsonDiffDeltaFormatter<string>? deltaFormatter,
            string? message)
        {
            var diff = expected.Diff(actual, diffOptions);
            if (diff is null)
            {
                return;
            }

            message ??= CreateAreEqualFailureMessage(expected, actual, diff, deltaFormatter);
            throw new AssertFailedException(message);
        }

        private static string CreateAreEqualFailureMessage(JsonNode? expected, JsonNode? actual,
            JsonNode diff, IJsonDiffDeltaFormatter<string>? deltaFormatter)
        {
            var sb = new StringBuilder();
            sb.Append("JsonAssert.AreEqual() failure: The specified two JSON objects are not equal.");
            sb.AppendLine();

            if (deltaFormatter is null)
            {
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
            }
            else
            {
                var delta = new JsonDiffDelta(diff);
                sb.Append(deltaFormatter.Format(ref delta, expected));
            }

            sb.AppendLine();

            return sb.ToString();
        }

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        public static void AreNotEqual(string? expected, string? actual)
            => HandleAreNotEqual(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual), null, null);

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void AreNotEqual(string? expected, string? actual, JsonDiffOptions diffOptions)
            => HandleAreNotEqual(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual),
                diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)), null);

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="message">The failure message.</param>
        public static void AreNotEqual(string? expected, string? actual, string message)
            => HandleAreNotEqual(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual), null,
                message ?? throw new ArgumentNullException(nameof(message)));

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="message">The failure message.</param>
        public static void AreNotEqual(string? expected, string? actual, JsonDiffOptions diffOptions, string message)
            => HandleAreNotEqual(expected is null ? null : JsonNode.Parse(expected),
                actual is null ? null : JsonNode.Parse(actual),
                diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)),
                message ?? throw new ArgumentNullException(nameof(message)));

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        public static void AreNotEqual<T>(T? expected, T? actual)
            where T : JsonNode
            => HandleAreNotEqual(expected, actual, null, null);

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void AreNotEqual<T>(T? expected, T? actual, JsonDiffOptions diffOptions)
            where T : JsonNode
            => HandleAreNotEqual(expected, actual,
                diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)), null);

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="message">The failure message.</param>
        public static void AreNotEqual<T>(T? expected, T? actual, string message)
            where T : JsonNode
            => HandleAreNotEqual(expected, actual, null,
                message ?? throw new ArgumentNullException(nameof(message)));

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="message">The failure message.</param>
        public static void AreNotEqual<T>(T? expected, T? actual, JsonDiffOptions diffOptions, string message)
            where T : JsonNode
            => HandleAreNotEqual(expected, actual,
                diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)),
                message ?? throw new ArgumentNullException(nameof(message)));

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="assert">The assert.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        public static void JsonAreNotEqual(this Assert assert, string? expected, string? actual)
            => AreNotEqual(expected, actual);

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="assert">The assert.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void JsonAreNotEqual(this Assert assert, string? expected, string? actual,
            JsonDiffOptions diffOptions)
            => AreNotEqual(expected, actual, diffOptions);

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="assert">The assert.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="message">The failure message.</param>
        public static void JsonAreNotEqual(this Assert assert, string? expected, string? actual, string message)
            => AreNotEqual(expected, actual, message);

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="assert">The assert.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="message">The failure message.</param>
        public static void JsonAreNotEqual(this Assert assert, string? expected, string? actual,
            JsonDiffOptions diffOptions, string message)
            => AreNotEqual(expected, actual, diffOptions, message);

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="assert">The assert.</param>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        public static void JsonAreNotEqual<T>(this Assert assert, T? expected, T? actual)
            where T : JsonNode
            => AreNotEqual(expected, actual);

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="assert">The assert.</param>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        public static void JsonAreNotEqual<T>(this Assert assert, T? expected, T? actual, JsonDiffOptions diffOptions)
            where T : JsonNode
            => AreNotEqual(expected, actual, diffOptions);

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="assert">The assert.</param>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="message">The failure message.</param>
        public static void JsonAreNotEqual<T>(this Assert assert, T? expected, T? actual, string message)
            where T : JsonNode
            => AreNotEqual(expected, actual, message);

        /// <summary>
        /// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
        /// the ordering of members in the objects is not significant.
        /// </summary>
        /// <param name="assert">The assert.</param>
        /// <typeparam name="T">The type of JSON object to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="diffOptions">The diff options.</param>
        /// <param name="message">The failure message.</param>
        public static void JsonAreNotEqual<T>(this Assert assert, T? expected, T? actual, JsonDiffOptions diffOptions,
            string message)
            where T : JsonNode
            => AreNotEqual(expected, actual, diffOptions, message);

        private static void HandleAreNotEqual(JsonNode? expected, JsonNode? actual,
            JsonDiffOptions? diffOptions, string? message)
        {
            var diff = expected.Diff(actual, diffOptions);
            if (diff is not null)
            {
                return;
            }

            throw new AssertFailedException(
                message ?? "JsonAssert.AreNotEqual() failure: The specified two JSON objects are equal.");
        }
    }
}