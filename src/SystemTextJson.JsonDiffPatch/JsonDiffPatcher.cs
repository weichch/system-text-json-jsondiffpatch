using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    /// <summary>
    /// Provides methods to diff and patch JSON objects.
    /// </summary>
    public static partial class JsonDiffPatcher
    {
        /// <summary>
        /// Gets or sets the default diff options.
        /// </summary>
        public static Func<JsonDiffOptions>? DefaultOptions { get; set; }

        /// <summary>
        /// Gets or sets the default comparison mode used when determine <see cref="JsonNode"/> deep equality.
        /// </summary>
        public static JsonElementComparison DefaultDeepEqualsComparison { get; set; } = JsonElementComparison.RawText;
    }
}
