using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Diffs;
using System.Text.Json.Nodes;

namespace System.Text.Json
{
    static partial class JsonDiffPatcher
    {
        // Array diff:
        // https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#array-with-inner-changes
        private static void DiffArray(
            ref JsonDiffDelta delta,
            JsonArray left, 
            JsonArray right,
            in JsonDiffOptionsView options)
        {
            // Both are empty arrays
            if (left.Count == 0 && right.Count == 0)
            {
                return;
            }

            var match = options.ArrayItemMatcher 
                        ?? new DefaultArrayItemComparer(options).MatchArrayItem;

            // Find command head
            int commonHead;
            for (commonHead = 0; commonHead < left.Count && commonHead < right.Count; commonHead++)
            {
                if (!match(
                    left[commonHead], commonHead,
                    right[commonHead], commonHead,
                    out var deepEqual))
                {
                    break;
                }

                AddDiffResult(ref delta, left, commonHead,
                    right, commonHead, deepEqual, options);
            }

            // Find common tail
            int commonTail;
            for (commonTail = 0;
                commonHead + commonTail < left.Count && commonHead + commonTail < right.Count;
                commonTail++)
            {
                var leftIndex = left.Count - 1 - commonTail;
                var rightIndex = right.Count - 1 - commonTail;

                if (!match(
                    left[leftIndex], leftIndex,
                    right[rightIndex], rightIndex,
                    out var deepEqual))
                {
                    break;
                }

                AddDiffResult(ref delta, left, leftIndex,
                    right, rightIndex, deepEqual, options);
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
                        var itemDiff = DiffInternal(left[entry.LeftIndex], right[entry.RightIndex], options);
                        if (itemDiff is not null)
                        {
                            delta.ArrayChange(i, false, new JsonDiffDelta(itemDiff, options));
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
                                    JsonDiffDelta.ChangeDeletedToArrayMoved(delta, removedLeftIndex, i,
                                        options.IncludeValueOnMove);

                                    // Diff removed item in left and new item in right
                                    var itemDiff = DiffInternal(left[removedLeftIndex], right[i], options);
                                    if (itemDiff is not null)
                                    {
                                        delta.ArrayChange(i, false, new JsonDiffDelta(itemDiff, options));
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
                JsonArray left,
                int leftIndex,
                JsonArray right,
                int rightIndex,
                bool deepEqual,
                in JsonDiffOptionsView options)
            {
                if (deepEqual)
                {
                    return;
                }

                var itemDiff = DiffInternal(left[leftIndex], right[rightIndex], options);
                if (itemDiff is not null)
                {
                    delta.ArrayChange(rightIndex, false, new JsonDiffDelta(itemDiff, options));
                }
            }
        }
    }
}
