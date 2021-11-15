﻿using System.Text.Json.Diffs;
using System.Text.Json.Nodes;
using System.Threading;
using DiffMatchPatch;

namespace System.Text.Json
{
    static partial class JsonDiffPatcher
    {
        private static readonly Lazy<diff_match_patch> TextDiffAlgorithm = new(
            () => new diff_match_patch(),
            LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Implements long text diff using custom algorithm or by default Google's diff-match-patch:
        /// <see link="https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#text-diffs"/>
        /// </summary>
        private static void DiffLongText(
            ref JsonDiffDelta delta,
            string left,
            string right,
            in JsonDiffOptionsView options)
        {
            var alg = options.TextMatcher ?? DefaultTextDiff;
            var diff = alg(left, right);
            delta.Text(diff);

            static string DefaultTextDiff(string str1, string str2)
            {
                var alg = TextDiffAlgorithm.Value;
                var patches = alg.patch_make(str1, str2);
                var diff = alg.patch_toText(patches);
                return diff;
            }
        }

        /// <summary>
        /// Checks if both left and right JSON values are long texts.
        /// </summary>
        private static bool IsLongText(
            JsonNode? left,
            JsonNode? right,
            in JsonDiffOptionsView options,
            out string? leftText,
            out string? rightText)
        {
            leftText = null;
            rightText = null;

            if (left is not JsonValue || right is not JsonValue)
            {
                return false;
            }

            while (true)
            {
                if (options.TextDiffMinLength <= 0)
                {
                    break;
                }

                var valueLeft = left.GetValue<object>();
                var valueRight = right.GetValue<object>();

                if (valueLeft is JsonElement elementLeft && valueRight is JsonElement elementRight)
                {
                    if (elementLeft.ValueKind != JsonValueKind.String || elementRight.ValueKind != JsonValueKind.String)
                    {
                        break;
                    }

                    leftText = elementLeft.GetString();
                    rightText = elementRight.GetString();
                }
                else if (valueLeft is string strLeft && valueRight is string strRight)
                {
                    leftText = strLeft;
                    rightText = strRight;
                }

                if (leftText is not null && rightText is not null)
                {
                    // Align with:
                    // https://github.com/benjamine/jsondiffpatch/blob/a8cde4c666a8a25d09d8f216c7f19397f2e1b569/src/filters/texts.js#L68
                    if (leftText.Length >= options.TextDiffMinLength
                        && rightText.Length >= options.TextDiffMinLength)
                    {
                        return true;
                    }
                }

                break;
            }

            // Make sure we clear texts before return
            leftText = null;
            rightText = null;
            return false;
        }
    }
}
