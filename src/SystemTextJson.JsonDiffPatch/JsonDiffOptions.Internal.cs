using System.Text.Json.Diffs;

namespace System.Text.Json
{
    /// <summary>
    /// Internal copy of <see cref="JsonDiffOptions"/> that can be passed via reference.
    /// </summary>
    internal readonly struct JsonDiffOptionsView
    {
        public JsonDiffOptionsView(JsonDiffOptions options)
        {
            SuppressDetectArrayMove = options.SuppressDetectArrayMove;
            IncludeValueOnMove = options.IncludeValueOnMove;
            ArrayItemMatcher = options.ArrayItemMatcher;
            TextDiffMinLength = options.TextDiffMinLength;
            TextMatcher = options.TextMatcher;
        }

        public bool SuppressDetectArrayMove { get; }
        public bool IncludeValueOnMove { get; }
        public ArrayItemMatch? ArrayItemMatcher { get; }
        public int TextDiffMinLength { get; }
        public TextMatch? TextMatcher { get; }
    }
}
