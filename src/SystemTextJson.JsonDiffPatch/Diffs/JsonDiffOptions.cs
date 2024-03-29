﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.JsonDiffPatch.Diffs;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    /// <summary>
    /// Represents options for making JSON diff.
    /// </summary>
    public class JsonDiffOptions
    {
        private JsonElementComparison? _jsonElementComparison;

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
        public JsonElementComparison JsonElementComparison
        {
            get => _jsonElementComparison ?? JsonDiffPatcher.DefaultComparison;
            set => _jsonElementComparison = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="JsonValue"/> comparer.
        /// </summary>
        public IEqualityComparer<JsonValue>? ValueComparer { get; set; }

        /// <summary>
        /// Gets or sets the filter function to ignore JSON property. To ignore a property,
        /// implement this function and return <c>false</c>.
        /// </summary>
        public Func<string, JsonDiffContext, bool>? PropertyFilter { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JsonComparerOptions CreateComparerOptions() => new(JsonElementComparison, ValueComparer);
    }
}
