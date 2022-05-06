using System.Collections.Generic;
using System.Text.Json.JsonDiffPatch.Diffs;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    /// <summary>
    /// Represents options for making JSON diff.
    /// </summary>
    public class JsonDiffOptions
    {
        internal static readonly JsonDiffOptions Default = new();
        private JsonComparerOptions _comparerOptions;

        /// <summary>
        /// Specifies whether to suppress detect array move. Default value is <c>false</c>.
        /// </summary>
        public bool SuppressDetectArrayMove { get; set; }

        /// <summary>
        /// Gets or sets custom function to match array items when array items are found not equal using default match
        /// algorithm.
        /// </summary>
        public ArrayItemMatch? ArrayItemMatcher { get; set; }

        /// <summary>
        /// Gets or sets the function to find key of a <see cref="JsonObject"/> or <see cref="JsonArray"/>.
        /// </summary>
        public Func<JsonNode?, int, object?>? ArrayObjectItemKeyFinder { get; set; }

        /// <summary>
        /// Gets or sets whether two JSON objects (object or array) should be considered equal if they are at the same
        /// index inside their parent arrays.
        /// </summary>
        public bool ArrayObjectItemMatchByPosition { get; set; }

        /// <summary>
        /// Gets or sets the minimum length of texts that should be compared using long text comparison algorithm, e.g.
        /// <c>diff_match_patch</c> from Google. If this property is set to <c>0</c>, long text comparison algorithm is
        /// disabled. Default value is <c>0</c>.
        /// </summary>
        public int TextDiffMinLength { get; set; }

        /// <summary>
        /// Gets or sets the function to diff long texts.
        /// </summary>
        public Func<string, string, string?>? TextDiffProvider { get; set; }

        /// <summary>
        /// Gets or sets the mode to compare two <see cref="JsonElement"/> instances.
        /// </summary>
        public JsonElementComparison JsonElementComparison { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="JsonValue"/> comparer.
        /// </summary>
        public IEqualityComparer<JsonValue>? ValueComparer { get; set; }

        internal ref JsonComparerOptions CreateComparerOptions()
        {
            if (JsonElementComparison != _comparerOptions.JsonElementComparison
                || !ReferenceEquals(ValueComparer, _comparerOptions.ValueComparer))
            {
                _comparerOptions = new JsonComparerOptions(JsonElementComparison, ValueComparer);
            }

            return ref _comparerOptions;
        }
    }
}
