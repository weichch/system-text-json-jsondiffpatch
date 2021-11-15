using System.Text.Json.Diffs;

namespace System.Text.Json
{
    /// <summary>
    /// Represents options for making JSON diff.
    /// </summary>
    public struct JsonDiffOptions
    {
        // NOTE
        // If add any settings here, also copy to JsonDiffOptionsView

        /// <summary>
        /// Specifies whether to suppress detect array move. Default value is <c>false</c>.
        /// </summary>
        public bool SuppressDetectArrayMove { get; set; }

        /// <summary>
        /// Specifies whether to include moved item value. See
        /// <see link="https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#array-moves"/>.
        /// Default value is <c>false</c>.
        /// </summary>
        public bool IncludeValueOnMove { get; set; }

        /// <summary>
        /// Gets or sets the function to match array items.
        /// </summary>
        public ArrayItemMatch? ArrayItemMatcher { get; set; }

        /// <summary>
        /// Gets or sets the minimum length for diffing texts using <see cref="TextMatcher"/>
        /// or default text diffing algorithm, aka Google's diff-match-patch algorithm. When text
        /// diffing algorithm is not used, text diffing is fallback to value replacement. If this
        /// property is set to <c>0</c>, diffing algorithm is disabled. Default value is <c>0</c>.
        /// </summary>
        public int TextDiffMinLength { get; set; }

        /// <summary>
        /// Gets or sets the function to match long texts.
        /// </summary>
        public TextMatch? TextMatcher { get; set; }
    }
}
