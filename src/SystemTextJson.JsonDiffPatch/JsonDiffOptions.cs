using System.Text.Json.Diffs;

namespace System.Text.Json
{
    /// <summary>
    /// Represents options for making JSON diff.
    /// </summary>
    public struct JsonDiffOptions
    {
        /// <summary>
        /// Specifies whether to detect array move. Default value is <c>false</c>.
        /// </summary>
        public bool DetectArrayMove { get; set; }

        /// <summary>
        /// Specifies whether to include moved item value. See
        /// <see link="https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#array-moves"/>.
        /// Default value is <c>false</c>.
        /// </summary>
        public bool IncludeValueOnMove { get; set; }

        /// <summary>
        /// Gets or sets the delegate used to match array items.
        /// </summary>
        public ArrayItemMatch? ArrayItemMatcher { get; set; }
    }
}
