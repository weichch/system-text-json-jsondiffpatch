using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    public static class JsonDiffPatch
    {
        public static JsonNode? Diff(JsonNode? left, JsonNode? right)
        {
            left ??= string.Empty;
            right ??= string.Empty;

            if (left is JsonObject leftObj && right is JsonObject rightObj)
            {
                return new ObjectDiff(leftObj, rightObj).GetDelta();
            }

            //if (left is JsonArray leftArray && right is JsonArray rightArray)
            //{

            //}

            //var diff = new JsonArray(left, right);
            //return diff;

            throw new NotSupportedException();
        }
    }
}
