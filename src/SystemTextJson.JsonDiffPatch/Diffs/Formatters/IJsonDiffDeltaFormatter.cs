using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch.Diffs.Formatters
{
    /// <summary>
    /// Defines <see cref="JsonDiffDelta"/> formatting.
    /// </summary>
    public interface IJsonDiffDeltaFormatter
    {
        /// <summary>
        /// Creates a new JSON diff document from the <see cref="JsonDiffDelta"/>.
        /// </summary>
        /// <param name="delta">The JSON diff delta.</param>
        JsonNode? Format(ref JsonDiffDelta delta);
    }
}

