using System.Linq;
using System.Text.Json.JsonDiffPatch.Diffs;
using System.Text.Json.Nodes;
using DiffMatchPatch;

namespace System.Text.Json.JsonDiffPatch
{
    static partial class JsonDiffPatcher
    {
        private static JsonValue PatchLongText(string text, JsonDiffDelta delta, JsonPatchOptions options)
        {
            // When make changes in this method, also copy the changes to ReversePatch* method

            var textPatch = options.TextPatchProvider ?? DefaultLongTextPatch;
            return JsonValue.Create(textPatch(text, delta.GetTextDiff()))!;

            static string DefaultLongTextPatch(string text, string patchText)
            {
                var alg = DefaultTextDiffAlgorithm.Value;
                var patches = alg.patch_fromText(patchText);
                var results = alg.patch_apply(patches, text);

                if (((bool[]) results[1]).Any(flag => !flag))
                {
                    throw new InvalidOperationException(TextPatchFailed);
                }

                return (string) results[0];
            }
        }

        private static JsonValue ReversePatchLongText(string text, JsonDiffDelta delta, JsonReversePatchOptions options)
        {
            // When make changes in this method, also copy the changes to Patch* method

            var textPatch = options.ReverseTextPatchProvider ?? DefaultLongTextReversePatch;
            return JsonValue.Create(textPatch(text, delta.GetTextDiff()))!;

            static string DefaultLongTextReversePatch(string text, string patchText)
            {
                var alg = DefaultTextDiffAlgorithm.Value;
                var patches = alg.patch_fromText(patchText);

                // Reverse patches
                var reversedPatches = alg.patch_deepCopy(patches);
                foreach (var diff in reversedPatches.SelectMany(p => p.diffs))
                {
                    diff.operation = diff.operation switch
                    {
                        Operation.DELETE => Operation.INSERT,
                        Operation.INSERT => Operation.DELETE,
                        _ => diff.operation
                    };
                }

                var results = alg.patch_apply(reversedPatches, text);

                if (((bool[])results[1]).Any(flag => !flag))
                {
                    throw new InvalidOperationException(TextPatchFailed);
                }

                return (string)results[0];
            }
        }
    }
}
