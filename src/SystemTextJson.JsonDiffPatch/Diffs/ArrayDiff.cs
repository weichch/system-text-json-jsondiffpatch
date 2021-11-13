using System.Linq;
using System.Text.Json.Nodes;

namespace System.Text.Json.Diffs
{
    /// <summary>
    /// Implements JSON array diff:
    /// <see link="https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#array-with-inner-changes"/>
    /// </summary>
    internal readonly struct ArrayDiff
    {
        private readonly JsonArray _left;
        private readonly JsonArray _right;
        private readonly ArrayItemMatch _match;

        public ArrayDiff(JsonArray left, JsonArray right, ArrayItemMatch? match)
        {
            _left = left;
            _right = right;
            _match = match ?? MatchItem;
        }

        public JsonObject? GetDelta()
        {
            JsonObject? diff = null;

            // Both are empty arrays
            if (_left.Count == 0 && _right.Count == 0)
            {
                return diff;
            }

            // Find command head
            int commonHead;
            for (commonHead = 0; commonHead < _left.Count && commonHead < _right.Count; commonHead++)
            {
                if (!_match(
                    _left[commonHead], commonHead,
                    _right[commonHead], commonHead,
                    out var deepEqual))
                {
                    break;
                }

                AddDiffResult(ref diff, _left, commonHead,
                    _right, commonHead, deepEqual);
            }

            // Find common tail
            int commonTail;
            for (commonTail = 0;
                commonHead + commonTail < _left.Count && commonHead + commonTail < _right.Count;
                commonTail++)
            {
                var leftIndex = _left.Count - 1 - commonTail;
                var rightIndex = _right.Count - 1 - commonTail;

                if (!_match(
                    _left[leftIndex], leftIndex,
                    _right[rightIndex], rightIndex,
                    out var deepEqual))
                {
                    break;
                }

                AddDiffResult(ref diff, _left, leftIndex,
                    _right, rightIndex, deepEqual);
            }

            if (commonHead + commonTail == _left.Count)
            {
                if (_left.Count == _right.Count)
                {
                    // Should have a diff result when the arrays have identical length
                    return diff;
                }

                // Otherwise, the left is shorter, so there were items added to the middle of the right array
                diff ??= JsonDelta.Array();
                for (var i = commonHead /* pointed to the first non-equal item */;
                    i + commonTail < _right.Count;
                    i++)
                {
                    diff[$"{i:D}"] = JsonDelta.Added(_right[i]);
                }

                return diff;
            }
            
            if (commonHead + commonTail == _right.Count)
            {
                // The right is shorter, so there were items removed from the middle of left array
                diff ??= JsonDelta.Array();
                for (var i = commonHead /* pointed to the first non-equal item */;
                    i + commonTail < _left.Count;
                    i++)
                {
                    diff[$"_{i:D}"] = JsonDelta.Deleted(_left[i]);
                }

                return diff;
            }

            var trimmedLeft = _left.ToArray().AsSpan(0, _left.Count - commonTail);
            var trimmedRight = _right.ToArray().AsSpan(0, _right.Count - commonTail);
            var lcs = Lcs.Get(trimmedLeft, trimmedRight, _match);


            static void AddDiffResult(
                ref JsonObject? diffResult,
                JsonArray left,
                int leftIndex,
                JsonArray right,
                int rightIndex,
                bool deepEqual)
            {
                if (deepEqual)
                {
                    return;
                }

                var itemDiff = JsonDiffPatcher.Diff(left[leftIndex], right[rightIndex]);
                if (itemDiff is null)
                {
                    return;
                }

                diffResult ??= JsonDelta.Array();
                diffResult[$"{rightIndex:D}"] = itemDiff;
            }

            return null;
        }

        /// <summary>
        /// Returns if one JSON node matches another. Nodes are considered match if:
        /// <list type="bullet">
        ///     <item>they are deeply equal, OR</item>
        ///     <item>they are of JavaScript object type (<see cref="JsonObject"/> and <see cref="JsonArray"/>)
        ///           and their positions in corresponding arrays are equal
        ///     </item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// See: https://github.com/benjamine/jsondiffpatch/blob/a8cde4c666a8a25d09d8f216c7f19397f2e1b569/src/filters/arrays.js#L43
        /// </remarks>
        private static bool MatchItem(JsonNode? x, int indexX, JsonNode? y, int indexY, out bool deepEqual)
        {
            deepEqual = false;

            if (x is JsonObject or JsonArray && y is JsonObject or JsonArray)
            {
                if (indexX == indexY)
                {
                    return true;
                }
            }

            if (x.DeepEquals(y))
            {
                deepEqual = true;
                return true;
            }

            return false;
        }
    }
}
