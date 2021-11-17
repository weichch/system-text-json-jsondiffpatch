using System.Text.Json.Diffs;
using System.Text.Json.Nodes;

namespace System.Text.Json
{
    internal readonly struct JsonDiffOptionsView
    {
        public JsonDiffOptionsView(JsonDiffOptions options, bool copyJsonElement)
        {
            SuppressDetectArrayMove = options.SuppressDetectArrayMove;
            IncludeValueOnMove = options.IncludeValueOnMove;
            ArrayItemMatcher = options.ArrayItemMatcher;
            ArrayObjectItemKeyFinder = options.ArrayObjectItemKeyFinder;
            ArrayObjectItemMatchByPosition = options.ArrayObjectItemMatchByPosition;
            PreferFuzzyArrayItemMatch = options.PreferFuzzyArrayItemMatch;
            TextDiffMinLength = options.TextDiffMinLength;
            TextMatcher = options.TextMatcher;
            CopyJsonElement = copyJsonElement;
        }

        public bool SuppressDetectArrayMove { get; }
        public bool IncludeValueOnMove { get; }
        public ArrayItemMatch? ArrayItemMatcher { get; }
        public Func<JsonNode?, int, object?>? ArrayObjectItemKeyFinder { get; }
        public bool ArrayObjectItemMatchByPosition { get; }
        public bool PreferFuzzyArrayItemMatch { get; }
        public int TextDiffMinLength { get; }
        public TextMatch? TextMatcher { get; }
        public bool CopyJsonElement { get; }
    }
}
