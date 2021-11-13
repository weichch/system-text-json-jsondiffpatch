using System.Text.Json.Diffs;
using System.Text.Json.Nodes;

namespace System.Text.Json
{
    public static class JsonDiffPatcher
    {
        public static JsonNode? Diff(JsonNode? left, JsonNode? right)
        {
            left ??= string.Empty;
            right ??= string.Empty;

            if (left is JsonObject leftObj && right is JsonObject rightObj)
            {
                return new ObjectDiff(leftObj, rightObj).GetDelta();
            }

            if (left is JsonArray leftArr && right is JsonArray rightArr)
            {
                return new ArrayDiff(leftArr, rightArr).GetDelta();
            }

            if (!left.DeepEquals(right))
            {
                var diff = new JsonArray(left, right);
                return diff;
            }

            return null;
        }
    }
}
