using System.Text.Json.JsonDiffPatch.Diffs;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    static partial class JsonDiffPatcher
    {
        private static void PatchArray(JsonArray left, JsonObject patch, JsonPatchOptions options)
        {
            // When make changes in this method, also copy the changes to ReversePatch* method

            var arrayDelta = new JsonDiffDelta(patch);
            foreach (var entry in arrayDelta.GetPatchableArrayChangeEnumerable(left))
            {
                var delta = entry.Diff;
                var kind = delta.Kind;
                if (kind == DeltaKind.Deleted)
                {
                    CheckForIndex(entry.Index, left.Count - 1);
                    left.RemoveAt(entry.Index);
                }
                else if (kind == DeltaKind.Added)
                {
                    CheckForIndex(entry.Index, left.Count);
                    left.Insert(entry.Index, delta.GetAdded());
                }
                else
                {
                    CheckForIndex(entry.Index, left.Count - 1);
                    var value = left[entry.Index];
                    var oldValue = value;
                    Patch(ref value, delta.Document, options);
                    if (!ReferenceEquals(oldValue, value))
                    {
                        left[entry.Index] = value;
                    }
                }
            }
        }

        private static void ReversePatchArray(JsonArray left, JsonObject patch, JsonReversePatchOptions options)
        {
            // When make changes in this method, also copy the changes to Patch* method

            var arrayDelta = new JsonDiffDelta(patch);
            foreach (var entry in arrayDelta.GetPatchableArrayChangeEnumerable(left, true))
            {
                var delta = entry.Diff;
                var kind = delta.Kind;
                if (kind == DeltaKind.Deleted)
                {
                    CheckForIndex(entry.Index, left.Count);
                    left.Insert(entry.Index, delta.GetDeleted());
                }
                else if (kind == DeltaKind.Added)
                {
                    CheckForIndex(entry.Index, left.Count - 1);
                    left.RemoveAt(entry.Index);
                }
                else
                {
                    CheckForIndex(entry.Index, left.Count - 1);
                    var value = left[entry.Index];
                    var oldValue = value;
                    ReversePatch(ref value, delta.Document, options);
                    if (!ReferenceEquals(oldValue, value))
                    {
                        left[entry.Index] = value;
                    }
                }
            }
        }

        private static void CheckForIndex(int index, int upperLimit)
        {
            if (index > upperLimit)
            {
                throw new FormatException(JsonDiffDelta.InvalidPatchDocument);
            }
        }
    }
}
