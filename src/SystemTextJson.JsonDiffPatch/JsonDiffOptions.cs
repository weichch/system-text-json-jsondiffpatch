using System.Text.Json.Diffs;
using System.Text.Json.Nodes;

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
        /// Gets or sets whether to materialize any <see cref="JsonElement"/> backed <see cref="JsonValue"/>
        /// before generating the diff. This option will use more memory but will generate more accurate
        /// results backed by CLR types, for example, comparing two date strings using <see cref="DateTime"/>
        /// or <see cref="DateTimeOffset"/>.
        /// </summary>
        public bool MaterializeBeforeDiff { get; set; }

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
        /// Gets or sets the function to find key of a <see cref="JsonObject"/>
        /// or <see cref="JsonArray"/>. This is used when matching array items by
        /// their keys. If this function returns <c>null</c>, the items being
        /// compared are treated as "not keyed". When comparing two "not keyed"
        /// objects, their contents are compared. This function is only used when
        /// <see cref="ArrayItemMatcher"/> is set to <c>null</c>.
        /// </summary>
        public Func<JsonNode?, int, object?>? ArrayObjectItemKeyFinder { get; set; }

        /// <summary>
        /// Gets or sets whether two instances of JSON object types (object and array)
        /// are considered equal if their position is the same in their parent
        /// arrays regardless of their contents. This property is only used when
        /// <see cref="ArrayItemMatcher"/> is set to <c>null</c>. By settings this
        /// property to <c>true</c>, a diff could be returned faster but larger in
        /// size. Default value is <c>false</c>.
        /// </summary>
        public bool ArrayObjectItemMatchByPosition { get; set; }

        /// <summary>
        /// Gets or sets whether to prefer <see cref="ArrayObjectItemKeyFinder"/> and
        /// <see cref="ArrayObjectItemMatchByPosition"/> than using deep value comparison
        /// to match array object items. By settings this property to <c>true</c>,
        /// a diff could be returned faster but larger in size. Default value is <c>false</c>.
        /// </summary>
        public bool PreferFuzzyArrayItemMatch { get; set; }

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
