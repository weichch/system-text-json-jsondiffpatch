using System.Collections.Generic;
using System.Text.Json.JsonDiffPatch.Diffs;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    static partial class JsonDiffPatcher
    {
        // Object diff:
        // https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#object-with-inner-changes
        private static void DiffObject(
            ref JsonDiffDelta delta, 
            JsonObject left, 
            JsonObject right,
            JsonDiffOptions? options)
        {
            var leftProperties = (left as IDictionary<string, JsonNode?>).Keys;
            var rightProperties = (right as IDictionary<string, JsonNode?>).Keys;

            JsonDiffContext? diffContext = null;
            var propertyFilter = options?.PropertyFilter;
            if (propertyFilter is not null)
            {
                diffContext = new JsonDiffContext(left, right);
            }

            foreach (var prop in leftProperties)
            {
                if (propertyFilter is not null && !propertyFilter(prop, diffContext!))
                {
                    continue;
                }

                var leftValue = left[prop];
                if (!right.TryGetPropertyValue(prop, out var rightValue))
                {
                    // Deleted: https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#deleted
                    delta.ObjectChange(prop, JsonDiffDelta.CreateDeleted(leftValue));
                }
                else
                {
                    // Modified: https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#modified
                    var valueDiff = new JsonDiffDelta();
                    DiffInternal(ref valueDiff, leftValue, rightValue, options);
                    if (valueDiff.Document is not null)
                    {
                        delta.ObjectChange(prop, valueDiff);
                    }
                }
            }

            foreach (var prop in rightProperties)
            {
                if (propertyFilter is not null && !propertyFilter(prop, diffContext!))
                {
                    continue;
                }

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
