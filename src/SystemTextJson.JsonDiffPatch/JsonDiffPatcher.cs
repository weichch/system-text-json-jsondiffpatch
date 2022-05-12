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
        /// Gets or sets the default comparison mode used by <c>DeepEquals</c> to compare <see cref="JsonElement"/>.
        /// </summary>
        public static JsonElementComparison DefaultComparison { get; set; } = JsonElementComparison.RawText;
    }
}
