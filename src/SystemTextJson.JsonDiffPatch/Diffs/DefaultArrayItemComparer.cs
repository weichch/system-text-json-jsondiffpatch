using System.Text.Json.Nodes;

namespace System.Text.Json.Diffs
{
    internal readonly struct DefaultArrayItemComparer
    {
        private readonly JsonDiffOptionsView _options;

        public DefaultArrayItemComparer(in JsonDiffOptionsView options)
        {
            _options = options;
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
        public bool MatchArrayItem(
            JsonNode? x, int indexX, JsonNode? y, int indexY,
            out bool deepEqual)
        {
            deepEqual = false;

            if (_options.PreferFuzzyArrayItemMatch
                && x is JsonObject or JsonArray
                && y is JsonObject or JsonArray)
            {
                if (FuzzyMatchItem(x, indexX, y, indexY,
                    out var fuzzyResult, out deepEqual))
                {
                    return fuzzyResult;
                }
            }

            if (x.DeepEquals(y))
            {
                deepEqual = true;
                return true;
            }

            if (!_options.PreferFuzzyArrayItemMatch
                && x is JsonObject or JsonArray
                && y is JsonObject or JsonArray)
            {
                if (FuzzyMatchItem(x, indexX, y, indexY,
                    out var fuzzyResult, out deepEqual))
                {
                    return fuzzyResult;
                }
            }

            return false;
        }

        private bool FuzzyMatchItem(JsonNode? x, int indexX,
            JsonNode? y, int indexY,
            out bool result, out bool deepEqual)
        {
            result = false;
            deepEqual = false;

            var keyFinder = _options.ArrayObjectItemKeyFinder;
            if (keyFinder is not null)
            {
                var keyX = keyFinder(x, indexX);
                var keyY = keyFinder(y, indexY);

                if (keyX is null && keyY is null)
                {
                    // Use DeepEquals if both items are not keyed
                    return false;
                }

                result = Equals(keyX, keyY);
                return true;
            }

            if (_options.ArrayObjectItemMatchByPosition)
            {
                if (indexX == indexY)
                {
                    result = true;
                    return true;
                }

                // We don't return a result for objects at different position
                // so that we could still compare them using DeepEquals, or
                // return "not equal" if this method is called after.
            }

            return false;
        }
    }
}
