using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    /// <summary>
    /// Represents the options for internal JSON comparer.
    /// </summary>
    public readonly struct JsonComparerOptions
    {
        /// <summary>
        /// Creates an instance of the options.
        /// </summary>
        public JsonComparerOptions(JsonElementComparison jsonElementComparison)
        {
            JsonElementComparison = jsonElementComparison;
            ValueComparer = null;
        }
        
        /// <summary>
        /// Creates an instance of the options.
        /// </summary>
        public JsonComparerOptions(IEqualityComparer<JsonValue> valueComparer)
        {
            JsonElementComparison = JsonElementComparison.RawText;
            ValueComparer = valueComparer;
        }

        /// <summary>
        /// Gets the mode to compare two <see cref="JsonElement"/> instances.
        /// </summary>
        public JsonElementComparison JsonElementComparison { get; }
        
        /// <summary>
        /// Gets the value comparer.
        /// </summary>
        public IEqualityComparer<JsonValue>? ValueComparer { get; }
    }
}