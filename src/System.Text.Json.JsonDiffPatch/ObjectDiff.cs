using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    internal readonly struct ObjectDiff
    {
        private readonly JsonObject _left;
        private readonly JsonObject _right;

        public ObjectDiff(JsonObject left, JsonObject right)
        {
            _left = left;
            _right = right;
        }

        public JsonObject? GetDelta()
        {
            JsonObject? diff = null;
            var leftProperties = (_left as IDictionary<string, JsonNode?>).Keys;
            var rightProperties = (_right as IDictionary<string, JsonNode?>).Keys;

            foreach (var prop in leftProperties)
            {
                var leftValue = _left[prop];
                if (!_right.TryGetPropertyValue(prop, out var rightValue))
                {
                    // Deleted: https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#deleted
                    diff ??= new JsonObject();
                    diff[prop] = new JsonArray(
                        Clone(leftValue),
                        0,
                        DeltaTypes.Deleted);
                }
                else
                {
                    // Modified: https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#modified
                    var valueDiff = JsonDiffPatch.Diff(leftValue, rightValue);
                    if (valueDiff is not null)
                    {
                        diff ??= new JsonObject();
                        diff[prop] = valueDiff;
                    }
                }
            }

            foreach (var prop in rightProperties)
            {
                var rightValue = _right[prop];
                if (!_left.ContainsKey(prop))
                {
                    // Added: https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#added
                    diff ??= new JsonObject();
                    diff[prop] = new JsonArray(Clone(rightValue));
                }
            }

            return diff;
        }

        private static JsonNode? Clone(JsonNode? node)
        {
            return node switch
            {
                null => null,
                JsonObject obj => new JsonObject(obj),
                JsonArray array => new JsonArray(array.Select(Clone).ToArray()),
                JsonValue value => JsonValue.Create(value),
                _ => throw new NotSupportedException($"JsonNode of type '{node.GetType().Name}' is not supported.")
            };
        }
    }
}
