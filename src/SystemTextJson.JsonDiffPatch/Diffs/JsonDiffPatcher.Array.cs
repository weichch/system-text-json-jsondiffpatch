﻿using System.Collections.Generic;
using System.Linq;
using System.Text.Json.JsonDiffPatch.Diffs;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    static partial class JsonDiffPatcher
    {
        // Array diff:
        // https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#array-with-inner-changes
        private static void DiffArray(
            ref JsonDiffDelta delta,
            JsonArray left, 
            JsonArray right,
            JsonDiffOptions options)
        {
            // Both are empty arrays
            if (left.Count == 0 && right.Count == 0)
            {
                return;
            }

            var match = options.ArrayItemMatcher;
            if (match is null)
            {
                var defaultComparer = new DefaultArrayItemComparer(options);
                match = defaultComparer.MatchArrayItem;
            }

            // Find command head
            int commonHead;
            for (commonHead = 0; commonHead < left.Count && commonHead < right.Count; commonHead++)
            {
                var matchContext = new ArrayItemMatchContext(left[commonHead], commonHead,
                    right[commonHead], commonHead);

                if (!match(ref matchContext))
                {
                    break;
                }

                AddDiffResult(ref delta, ref matchContext, options);
            }

            // Find common tail
            int commonTail;
            for (commonTail = 0;
                commonHead + commonTail < left.Count && commonHead + commonTail < right.Count;
                commonTail++)
            {
                var leftIndex = left.Count - 1 - commonTail;
                var rightIndex = right.Count - 1 - commonTail;
                var matchContext = new ArrayItemMatchContext(left[leftIndex], leftIndex,
                    right[rightIndex], rightIndex);

                if (!match(ref matchContext))
                {
                    break;
                }

                AddDiffResult(ref delta, ref matchContext, options);
            }

            if (commonHead + commonTail == left.Count)
            {
                if (left.Count == right.Count)
                {
                    // Should have a diff result when the arrays have identical length
                    return;
                }

                // Otherwise, the left is shorter, so there were items added to the middle of the right array
                for (var i = commonHead /* pointed to the first non-equal item */;
                    i + commonTail < right.Count;
                    i++)
                {
                    delta.ArrayChange(i, false, JsonDiffDelta.CreateAdded(right[i]));
                }

                return;
            }
            
            if (commonHead + commonTail == right.Count)
            {
                // The right is shorter, so there were items removed from the middle of left array
                for (var i = commonHead /* pointed to the first non-equal item */;
                    i + commonTail < left.Count;
                    i++)
                {
                    delta.ArrayChange(i, true, JsonDiffDelta.CreateDeleted(left[i]));
                }

                return;
            }

            var trimmedLeft = left.ToArray().AsSpan(commonHead, left.Count - commonTail - commonHead);
            var trimmedRight = right.ToArray().AsSpan(commonHead, right.Count - commonTail - commonHead);
            var lcs = Lcs.Get(trimmedLeft, trimmedRight, match);

            try
            {
                // Find removed in left array as per LCS
                List<int>? removedIndices = null;
                for (var i = commonHead; i + commonTail < left.Count; i++)
                {
                    if (lcs.FindLeftIndex(i - commonHead, out _))
                    {
                        continue;
                    }

                    delta.ArrayChange(i, true, JsonDiffDelta.CreateDeleted(left[i]));
                    removedIndices ??= new List<int>();
                    removedIndices.Add(i);
                }

                // Find added and modified in right array as per LCS
                for (var i = commonHead; i + commonTail < right.Count; i++)
                {
                    if (lcs.FindRightIndex(i - commonHead, out var entry))
                    {
                        if (entry.AreDeepEqual)
                        {
                            continue;
                        }

                        // We have two objects equal by position or other criteria
                        var itemDiff = new JsonDiffDelta();
                        DiffInternal(ref itemDiff, left[entry.LeftIndex], right[entry.RightIndex], options);
                        if (itemDiff.Document is not null)
                        {
                            delta.ArrayChange(i, false, itemDiff);
                        }
                    }
                    else
                    {
                        // Added, detect if it's moved item
                        var isMoved = false;
                        if (!options.SuppressDetectArrayMove && removedIndices is not null)
                        {
                            for (var j = 0; j < removedIndices.Count; j++)
                            {
                                var removedLeftIndex = removedIndices[j];
                                if (lcs.AreEqual(
                                    removedLeftIndex - commonHead /* Deleted in left */,
                                    i - commonHead /* Current in right */,
                                    out _))
                                {
                                    delta.ArrayMoveFromDeleted(removedLeftIndex, i);

                                    // Diff removed item in left and new item in right
                                    var itemDiff = new JsonDiffDelta();
                                    DiffInternal(ref itemDiff, left[removedLeftIndex], right[i], options);
                                    if (itemDiff.Document is not null)
                                    {
                                        delta.ArrayChange(i, false, itemDiff);
                                    }

                                    removedIndices.RemoveAt(j);
                                    isMoved = true;

                                    break;
                                }
                            }
                        }

                        if (!isMoved)
                        {
                            // Added
                            delta.ArrayChange(i, false, JsonDiffDelta.CreateAdded(right[i]));
                        }
                    }
                }
            }
            finally
            {
                lcs.Free();
            }

            static void AddDiffResult(
                ref JsonDiffDelta delta,
                ref ArrayItemMatchContext context,
                JsonDiffOptions options)
            {
                if (context.IsDeepEqual)
                {
                    return;
                }

                var itemDiff = new JsonDiffDelta();
                DiffInternal(ref itemDiff, context.Left, context.Right, options);
                if (itemDiff.Document is not null)
                {
                    delta.ArrayChange(context.RightPosition, false, itemDiff);
                }
            }
        }
    }
}
