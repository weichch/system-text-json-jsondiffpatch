using System.Diagnostics;
using System.Text.Json.Diffs;
using System.Text.Json.Nodes;

namespace System.Text.Json
{
    /// <summary>
    /// Provides methods to diff and patch JSON objects.
    /// </summary>
    public static partial class JsonDiffPatcher
    {
        /// <summary>
        /// Compares two JSON objects and generates a diff in a format described
        /// in <see link="https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md"/>.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <param name="options">The diffing options.</param>
        public static JsonNode? Diff(JsonNode? left, JsonNode? right, JsonDiffOptions options = default)
        {
            return Diff(left, right, new JsonDiffOptionsView(options));
        }

        private static JsonNode? Diff(JsonNode? left, JsonNode? right, in JsonDiffOptionsView options)
        {
            var delta = new JsonDiffDelta();

            left ??= string.Empty;
            right ??= string.Empty;

            // Compare two objects
            if (left is JsonObject leftObj && right is JsonObject rightObj)
            {
                DiffObject(ref delta, leftObj, rightObj, options);
                return delta.Result;
            }

            // Compare two arrays
            if (left is JsonArray leftArr && right is JsonArray rightArr)
            {
                DiffArray(ref delta, leftArr, rightArr, options);
                return delta.Result;
            }

            // For long texts
            // Compare two long texts
            if (IsLongText(left, right, options, out var leftText, out var rightText))
            {
                DiffLongText(ref delta, leftText!, rightText!, options);
                return delta.Result;
            }

            // None of the above methods returned a result, fallback to check if both values are deeply equal
            // This should also handle DateTime and other CLR types that are strings in JSON
            if (!left.DeepEquals(right))
            {
                delta.Modified(left, right);
                return delta.Result;
            }

            Debug.Assert(delta.Result is null);
            return null;
        }
    }
}
