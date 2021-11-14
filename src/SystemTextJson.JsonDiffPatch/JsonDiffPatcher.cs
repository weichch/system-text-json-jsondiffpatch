using System.Diagnostics;
using System.Text.Json.Diffs;
using System.Text.Json.Nodes;

namespace System.Text.Json
{
    public static partial class JsonDiffPatcher
    {
        public static JsonNode? Diff(JsonNode? left, JsonNode? right, JsonDiffOptions options = default)
        {
            var delta = new JsonDiffDelta();

            left ??= string.Empty;
            right ??= string.Empty;

            if (left is JsonObject leftObj && right is JsonObject rightObj)
            {
                DiffObject(ref delta, leftObj, rightObj, options);
                return delta.Result;
            }

            if (left is JsonArray leftArr && right is JsonArray rightArr)
            {
                DiffArray(ref delta, leftArr, rightArr, options);
                return delta.Result;
            }

            if (!left.DeepEquals(right))
            {
                delta.Modified(left, right);
                return delta.Result;
            }

            Debug.Assert(delta.Result is null);
            return delta.Result;
        }
    }
}
