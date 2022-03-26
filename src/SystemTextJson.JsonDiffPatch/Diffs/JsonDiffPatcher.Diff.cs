using System.Diagnostics;
using System.IO;
using System.Text.Json.JsonDiffPatch.Diffs;
using System.Text.Json.JsonDiffPatch.Diffs.Formatters;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    static partial class JsonDiffPatcher
    {
        /// <summary>
        /// Compares two JSON objects and generates a diff document.
        /// </summary>
        /// <param name="leftJson">The left object.</param>
        /// <param name="rightJson">The right object.</param>
        /// <param name="options">The diffing options.</param>
        public static JsonNode? Diff(ReadOnlySpan<byte> leftJson, ReadOnlySpan<byte> rightJson,
            JsonDiffOptions? options = default)
        {
            return DiffAndFormat<JsonNode>(JsonNode.Parse(leftJson), JsonNode.Parse(rightJson), null, options);
        }

        /// <summary>
        /// Compares two JSON objects and generates a diff document.
        /// </summary>
        /// <param name="leftJson">The left object.</param>
        /// <param name="rightJson">The right object.</param>
        /// <param name="formatter">The result formatter.</param>
        /// <param name="options">The diffing options.</param>
        public static TResult? Diff<TResult>(ReadOnlySpan<byte> leftJson, ReadOnlySpan<byte> rightJson,
            IJsonDiffDeltaFormatter<TResult> formatter,
            JsonDiffOptions? options = default)
        {
            _ = formatter ?? throw new ArgumentNullException(nameof(formatter));
            
            return DiffAndFormat(JsonNode.Parse(leftJson), JsonNode.Parse(rightJson), formatter, options);
        }

        /// <summary>
        /// Compares two JSON objects and generates a diff document.
        /// </summary>
        /// <param name="leftJson">The left object.</param>
        /// <param name="rightJson">The right object.</param>
        /// <param name="options">The diffing options.</param>
        public static JsonNode? Diff(Stream leftJson, Stream rightJson,
            JsonDiffOptions? options = default)
        {
            _ = leftJson ?? throw new ArgumentNullException(nameof(leftJson));
            _ = rightJson ?? throw new ArgumentNullException(nameof(rightJson));

            return DiffAndFormat<JsonNode>(JsonNode.Parse(leftJson), JsonNode.Parse(rightJson), null, options);
        }
        
        /// <summary>
        /// Compares two JSON objects and generates a diff document.
        /// </summary>
        /// <param name="leftJson">The left object.</param>
        /// <param name="rightJson">The right object.</param>
        /// <param name="formatter">The result formatter.</param>
        /// <param name="options">The diffing options.</param>
        public static TResult? Diff<TResult>(Stream leftJson, Stream rightJson,
            IJsonDiffDeltaFormatter<TResult> formatter,
            JsonDiffOptions? options = default)
        {
            _ = leftJson ?? throw new ArgumentNullException(nameof(leftJson));
            _ = rightJson ?? throw new ArgumentNullException(nameof(rightJson));
            _ = formatter ?? throw new ArgumentNullException(nameof(formatter));

            return DiffAndFormat(JsonNode.Parse(leftJson), JsonNode.Parse(rightJson), formatter, options);
        }

        /// <summary>
        /// Compares two JSON objects and generates a diff document.
        /// </summary>
        /// <param name="leftJson">The left object.</param>
        /// <param name="rightJson">The right object.</param>
        /// <param name="options">The diffing options.</param>
        public static JsonNode? Diff(ref Utf8JsonReader leftJson, ref Utf8JsonReader rightJson,
            JsonDiffOptions? options = default)
        {
            return DiffAndFormat<JsonNode>(JsonNode.Parse(ref leftJson), JsonNode.Parse(ref rightJson), null, options);
        }
        
        /// <summary>
        /// Compares two JSON objects and generates a diff document.
        /// </summary>
        /// <param name="leftJson">The left object.</param>
        /// <param name="rightJson">The right object.</param>
        /// <param name="formatter">The result formatter.</param>
        /// <param name="options">The diffing options.</param>
        public static TResult? Diff<TResult>(ref Utf8JsonReader leftJson, ref Utf8JsonReader rightJson,
            IJsonDiffDeltaFormatter<TResult> formatter,
            JsonDiffOptions? options = default)
        {
            _ = formatter ?? throw new ArgumentNullException(nameof(formatter));
            
            return DiffAndFormat(JsonNode.Parse(ref leftJson), JsonNode.Parse(ref rightJson), formatter, options);
        }

        /// <summary>
        /// Compares two JSON objects and generates a diff document.
        /// </summary>
        /// <param name="leftJson">The left object.</param>
        /// <param name="rightJson">The right object.</param>
        /// <param name="options">The diffing options.</param>
        public static JsonNode? Diff(string? leftJson, string? rightJson,
            JsonDiffOptions? options = default)
        {
            return DiffAndFormat<JsonNode>(leftJson is null ? null : JsonNode.Parse(leftJson),
                rightJson is null ? null : JsonNode.Parse(rightJson), null, options);
        }
        
        /// <summary>
        /// Compares two JSON objects and generates a diff document.
        /// </summary>
        /// <param name="leftJson">The left object.</param>
        /// <param name="rightJson">The right object.</param>
        /// <param name="formatter">The result formatter.</param>
        /// <param name="options">The diffing options.</param>
        public static TResult? Diff<TResult>(string? leftJson, string? rightJson,
            IJsonDiffDeltaFormatter<TResult> formatter,
            JsonDiffOptions? options = default)
        {
            _ = formatter ?? throw new ArgumentNullException(nameof(formatter));
            
            return DiffAndFormat(leftJson is null ? null : JsonNode.Parse(leftJson),
                rightJson is null ? null : JsonNode.Parse(rightJson), formatter, options);
        }

        /// <summary>
        /// Compares two JSON objects and generates a mutable diff document.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <param name="options">The diffing options.</param>
        public static JsonNode? Diff(this JsonNode? left, JsonNode? right, JsonDiffOptions? options = default)
        {
            return DiffAndFormat<JsonNode>(left, right, null, options);
        }

        /// <summary>
        /// Compares two JSON objects and generates a mutable diff document.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <param name="formatter">The result formatter.</param>
        /// <param name="options">The diffing options.</param>
        public static TResult? Diff<TResult>(this JsonNode? left, JsonNode? right,
            IJsonDiffDeltaFormatter<TResult> formatter,
            JsonDiffOptions? options = default)
        {
            _ = formatter ?? throw new ArgumentNullException(nameof(formatter));
            
            return DiffAndFormat(left, right, formatter, options);
        }

        /// <summary>
        /// Compares two JSON objects from files and generates a diff document.
        /// </summary>
        /// <param name="leftFilePath">The path to the file containing left object.</param>
        /// <param name="rightFilePath">The path to the file containing left object.</param>
        /// <param name="options">The diffing options.</param>
        public static JsonNode? DiffFile(string leftFilePath, string rightFilePath,
            JsonDiffOptions? options = default)
        {
            _ = leftFilePath ?? throw new ArgumentNullException(nameof(leftFilePath));
            _ = rightFilePath ?? throw new ArgumentNullException(nameof(rightFilePath));

            using var fsLeft = File.OpenRead(leftFilePath);
            using var fsRight = File.OpenRead(rightFilePath);

            return Diff(fsLeft, fsRight, options);
        }
        
        /// <summary>
        /// Compares two JSON objects from files and generates a diff document.
        /// </summary>
        /// <param name="leftFilePath">The path to the file containing left object.</param>
        /// <param name="rightFilePath">The path to the file containing left object.</param>
        /// <param name="formatter">The result formatter.</param>
        /// <param name="options">The diffing options.</param>
        public static TResult? DiffFile<TResult>(string leftFilePath, string rightFilePath,
            IJsonDiffDeltaFormatter<TResult> formatter,
            JsonDiffOptions? options = default)
        {
            _ = leftFilePath ?? throw new ArgumentNullException(nameof(leftFilePath));
            _ = rightFilePath ?? throw new ArgumentNullException(nameof(rightFilePath));
            _ = formatter ?? throw new ArgumentNullException(nameof(formatter));

            using var fsLeft = File.OpenRead(leftFilePath);
            using var fsRight = File.OpenRead(rightFilePath);

            return Diff(fsLeft, fsRight, formatter, options);
        }

        private static TResult? DiffAndFormat<TResult>(
            JsonNode? left,
            JsonNode? right,
            IJsonDiffDeltaFormatter<TResult>? formatter,
            JsonDiffOptions? options)
        {
            var delta = new JsonDiffDelta();
            DiffInternal(ref delta, left, right, options);

            if (formatter is not null)
            {
                return formatter.Format(ref delta);
            }

            if (delta.Document is TResult result)
            {
                return result;
            }

            return (TResult?) (object?) delta.Document;
        }

        private static void DiffInternal(
            ref JsonDiffDelta delta,
            JsonNode? left,
            JsonNode? right,
            JsonDiffOptions? options)
        {
            Debug.Assert(delta.Document is null);
            
            options ??= JsonDiffOptions.Default;

            left ??= "";
            right ??= "";

            // Compare two objects
            if (left is JsonObject leftObj && right is JsonObject rightObj)
            {
                DiffObject(ref delta, leftObj, rightObj, options);
                return;
            }

            // Compare two arrays
            if (left is JsonArray leftArr && right is JsonArray rightArr)
            {
                DiffArray(ref delta, leftArr, rightArr, options);
                return;
            }

            // For long texts
            // Compare two long texts
            if (IsLongText(left, right, options, out var leftText, out var rightText))
            {
                DiffLongText(ref delta, leftText!, rightText!, options);
                return;
            }

            // None of the above methods returned a result, fallback to check if both values are deeply equal
            // This should also handle DateTime and other CLR types that are strings in JSON
            if (!left.DeepEquals(right))
            {
                delta.Modified(left, right);
                return;
            }
        }
    }
}
