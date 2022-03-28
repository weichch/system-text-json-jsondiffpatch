using System.Text.Json.JsonDiffPatch.Diffs;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    static partial class JsonDiffPatcher
    {
        private static void PatchObject(JsonObject left, JsonObject patch, JsonPatchOptions options)
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
                    if (left.ContainsKey(propertyName))
                    {
                        left.Remove(propertyName);
                    }

                    left.Add(propertyName, propPatch.GetAdded());
                }
                else if (kind == DeltaKind.Deleted)
                {
                    if (left.ContainsKey(propertyName))
                    {
                        left.Remove(propertyName);
                    }
                }
                else
                {
                    if (left.TryGetPropertyValue(propertyName, out var value))
                    {
                        var oldValue = value;
                        Patch(ref value, innerPatch, options);
                        if (!ReferenceEquals(oldValue, value))
                        {
                            left[propertyName] = value;
                        }
                    }
                }
            }
        }

        private static void ReversePatchObject(JsonObject left, JsonObject patch, JsonReversePatchOptions options)
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
                    if (left.ContainsKey(propertyName))
                    {
                        left.Remove(propertyName);
                    }
                }
                else if (kind == DeltaKind.Deleted)
                {
                    if (left.ContainsKey(propertyName))
                    {
                        left.Remove(propertyName);
                    }

                    left.Add(propertyName, propPatch.GetDeleted());
                }
                else
                {
                    if (left.TryGetPropertyValue(propertyName, out var value))
                    {
                        var oldValue = value;
                        ReversePatch(ref value, innerPatch, options);
                        if (!ReferenceEquals(oldValue, value))
                        {
                            left[propertyName] = value;
                        }
                    }
                }
            }
        }
    }
}
