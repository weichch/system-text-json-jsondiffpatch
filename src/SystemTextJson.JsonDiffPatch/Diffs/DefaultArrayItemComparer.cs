using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch.Diffs
{
    internal readonly ref struct DefaultArrayItemComparer
    {
        private readonly JsonDiffOptions _options;

        public DefaultArrayItemComparer(JsonDiffOptions options)
        {
            _options = options;
        }

        public bool MatchArrayItem(ref ArrayItemMatchContext context)
        {
            // Returns if one JSON node matches another. Nodes are considered match if:
            // - they are deeply equal, OR
            // - they are of JavaScript object type JsonObject and JsonArray and their
            //   positions in corresponding arrays are equal
            // See: https://github.com/benjamine/jsondiffpatch/blob/a8cde4c666a8a25d09d8f216c7f19397f2e1b569/src/filters/arrays.js#L43

            if (_options.PreferFuzzyArrayItemMatch
                && context.Left is JsonObject or JsonArray
                && context.Right is JsonObject or JsonArray)
            {
                if (FuzzyMatchItem(ref context, out var fuzzyResult))
                {
                    return fuzzyResult;
                }
            }

            if (context.ComparerOptions.JsonElementComparison == JsonElementComparison.Semantic)
            {
                if (context.TryCompareValue(out var valueCompareResult))
                {
                    if (valueCompareResult)
                    {
                        context.DeepEqual();
                    }

                    return valueCompareResult;
                }
            }

            if (context.Left.DeepEquals(context.Right, context.ComparerOptions))
            {
                context.DeepEqual();
                return true;
            }

            if (!_options.PreferFuzzyArrayItemMatch
                && context.Left is JsonObject or JsonArray
                && context.Right is JsonObject or JsonArray)
            {
                if (FuzzyMatchItem(ref context, out var fuzzyResult))
                {
                    return fuzzyResult;
                }
            }

            return false;
        }

        private bool FuzzyMatchItem(ref ArrayItemMatchContext context, out bool result)
        {
            result = false;

            var keyFinder = _options.ArrayObjectItemKeyFinder;
            if (keyFinder is not null)
            {
                var keyX = keyFinder(context.Left, context.LeftPosition);
                var keyY = keyFinder(context.Right, context.RightPosition);

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
                if (context.LeftPosition == context.RightPosition)
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
