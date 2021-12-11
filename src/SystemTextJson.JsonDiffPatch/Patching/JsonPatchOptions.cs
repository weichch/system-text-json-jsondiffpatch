using System.Text.Json.JsonDiffPatch.Patching;

namespace System.Text.Json.JsonDiffPatch
{
    /// <summary>
    /// Represents options for patching JSON object.
    /// </summary>
    public struct JsonPatchOptions
    {
        /// <summary>
        /// Gets or sets the function to patch long text.
        /// </summary>
        public TextPatch? TextPatchProvider { get; set; }
    }
}
