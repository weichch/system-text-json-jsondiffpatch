using System.Collections.Generic;
using System.Text.Json.Diffs;
using System.Text.Json.Nodes;

namespace System.Text.Json
{
    static partial class JsonDiffPatcher
    {
        /// <summary>
        /// Implements JSON object diff:
        /// <see link="https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#object-with-inner-changes"/>
        /// </summary>
        private static void DiffObject(
            ref JsonDiffDelta delta, 
            JsonObject left, 
            JsonObject right,
            JsonDiffOptions options)
        {
            var leftProperties = (left as IDictionary<string, JsonNode?>).Keys;
            var rightProperties = (right as IDictionary<string, JsonNode?>).Keys;

            foreach (var prop in leftProperties)
            {
                var leftValue = left[prop];
                if (!right.TryGetPropertyValue(prop, out var rightValue))
                {
                    // Deleted: https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#deleted
                    delta.ObjectChange(prop, JsonDiffDelta.CreateDeleted(leftValue));
                }
                else
                {
                    // Modified: https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#modified
                    var valueDiff = Diff(leftValue, rightValue, options);
                    delta.ObjectChange(prop, valueDiff);
                }
            }

            foreach (var prop in rightProperties)
            {
                var rightValue = right[prop];
                if (!left.ContainsKey(prop))
                {
                    // Added: https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#added
                    delta.ObjectChange(prop, JsonDiffDelta.CreateAdded(rightValue));
                }
            }
        }
    }
}
