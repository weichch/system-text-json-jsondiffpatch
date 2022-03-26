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
                    var valueDiff = new JsonDiffDelta();
                    DiffInternal(ref valueDiff, leftValue, rightValue, options);
                    if (valueDiff.Result is not null)
                    {
                        delta.ObjectChange(prop, valueDiff);
                    }
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

        private static void PatchObject(JsonObject target, JsonObject patch, JsonPatchOptions options)
        {
            // When make changes in this method, also copy the changes to ReversePatch* method

            foreach (var prop in patch)
            {
                var innerPatch = prop.Value;
                if (innerPatch is null)
                {
                    continue;
                }

                var propertyName = prop.Key;
                var propPatch = new JsonDiffDelta(innerPatch);
                var kind = propPatch.Kind;

                if (kind == DeltaKind.Added)
                {
                    if (target.ContainsKey(propertyName))
                    {
                        target.Remove(propertyName);
                    }

                    target.Add(propertyName, propPatch.GetAdded());
                }
                else if (kind == DeltaKind.Deleted)
                {
                    if (target.ContainsKey(propertyName))
                    {
                        target.Remove(propertyName);
                    }
                }
                else
                {
                    if (target.TryGetPropertyValue(propertyName, out var value))
                    {
                        var oldValue = value;
                        Patch(ref value, innerPatch, options);
                        if (!ReferenceEquals(oldValue, value))
                        {
                            target[propertyName] = value;
                        }
                    }
                }
            }
        }

        private static void ReversePatchObject(JsonObject target, JsonObject patch, JsonReversePatchOptions options)
        {
            // When make changes in this method, also copy the changes to Patch* method

            foreach (var prop in patch)
            {
                var innerPatch = prop.Value;
                if (innerPatch is null)
                {
                    continue;
                }

                var propertyName = prop.Key;
                var propPatch = new JsonDiffDelta(innerPatch);
                var kind = propPatch.Kind;

                if (kind == DeltaKind.Added)
                {
                    if (target.ContainsKey(propertyName))
                    {
                        target.Remove(propertyName);
                    }
                }
                else if (kind == DeltaKind.Deleted)
                {
                    if (target.ContainsKey(propertyName))
                    {
                        target.Remove(propertyName);
                    }

                    target.Add(propertyName, propPatch.GetDeleted());
                }
                else
                {
                    if (target.TryGetPropertyValue(propertyName, out var value))
                    {
                        var oldValue = value;
                        ReversePatch(ref value, innerPatch, options);
                        if (!ReferenceEquals(oldValue, value))
                        {
                            target[propertyName] = value;
                        }
                    }
                }
            }
        }
    }
}
