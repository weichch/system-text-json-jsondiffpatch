using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    /// <summary>
    /// Represents the options for internal JSON comparer.
    /// </summary>
    public readonly struct JsonComparerOptions
    {
        private readonly JsonElementComparison? _jsonElementComparison;

        /// <summary>
        /// Creates an instance of the options.
        /// </summary>
        public JsonComparerOptions(JsonElementComparison? jsonElementComparison,
            IEqualityComparer<JsonValue>? valueComparer)
        {
            _jsonElementComparison = jsonElementComparison;
            ValueComparer = valueComparer;
        }

        /// <summary>
        /// Gets the mode to compare two <see cref="JsonElement"/> instances.
        /// </summary>
        public JsonElementComparison JsonElementComparison =>
            _jsonElementComparison ?? JsonDiffPatcher.DefaultDeepEqualsComparison;

        /// <summary>
        /// Gets the value comparer.
        /// </summary>
        public IEqualityComparer<JsonValue>? ValueComparer { get; }
    }
}