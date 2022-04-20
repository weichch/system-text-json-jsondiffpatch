namespace System.Text.Json.JsonDiffPatch
{
    /// <summary>
    /// Represents <see cref="JsonElement"/> comparison modes.
    /// </summary>
    public enum JsonElementComparison
    {
        /// <summary>
        /// Only compares raw text of two <see cref="JsonElement"/> instances.
        /// </summary>
        RawText,
        
        /// <summary>
        /// Deserializes both <see cref="JsonElement"/> instances into value object of the most significant type
        /// and compares the value objects.
        /// </summary>
        Semantic
    }
}