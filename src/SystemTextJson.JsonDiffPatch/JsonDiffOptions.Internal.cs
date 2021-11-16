using System.Text.Json.Diffs;
using System.Text.Json.Nodes;

namespace System.Text.Json
{
    /// <summary>
    /// Internal copy of <see cref="JsonDiffOptions"/> that can be passed via reference.
    /// </summary>
    internal readonly struct JsonDiffOptionsView
    {
        public JsonDiffOptionsView(JsonDiffOptions options)
        {
            MaterializeBeforeDiff = options.MaterializeBeforeDiff;
            SuppressDetectArrayMove = options.SuppressDetectArrayMove;
            IncludeValueOnMove = options.IncludeValueOnMove;
            ArrayItemMatcher = options.ArrayItemMatcher;
            ArrayObjectItemKeyFinder = options.ArrayObjectItemKeyFinder;
            ArrayObjectItemMatchByPosition = options.ArrayObjectItemMatchByPosition;
            PreferFuzzyArrayItemMatch = options.PreferFuzzyArrayItemMatch;
            TextDiffMinLength = options.TextDiffMinLength;
            TextMatcher = options.TextMatcher;
        }

        public bool MaterializeBeforeDiff { get; }
        public bool SuppressDetectArrayMove { get; }
        public bool IncludeValueOnMove { get; }
        public ArrayItemMatch? ArrayItemMatcher { get; }
        public Func<JsonNode?, int, object?>? ArrayObjectItemKeyFinder { get; }
        public bool ArrayObjectItemMatchByPosition { get; }
        public bool PreferFuzzyArrayItemMatch { get; }
        public int TextDiffMinLength { get; }
        public TextMatch? TextMatcher { get; }
    }
}
