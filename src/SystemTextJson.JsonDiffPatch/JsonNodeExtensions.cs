using System.Linq;
using System.Text.Json.Diffs;
using System.Text.Json.Nodes;

namespace System.Text.Json
{
    /// <summary>
    /// Provides extensions to <see cref="JsonNode"/>.
    /// </summary>
    public static class JsonNodeExtensions
    {
        /// <summary>
        /// Creates a clone of the <see cref="JsonNode"/>.
        /// </summary>
        public static T? Clone<T>(this T? node)
            where T : JsonNode
        {
            return (T?) (node switch
            {
                null => (JsonNode?) null,
                JsonObject obj => new JsonObject(obj, obj.Options),
                JsonArray array => array.Options is null
                    ? new JsonArray(array.Select(Clone).ToArray())
                    : new JsonArray(array.Options.Value, array.Select(Clone).ToArray()),
                JsonValue value => JsonValue.Create(value, value.Options),
                _ => throw new NotSupportedException(
                    $"JsonNode of type '{node.GetType().Name}' is not supported.")
            });
        }

        /// <summary>
        /// Determines whether two <see cref="JsonNode"/> objects are deeply equal.
        /// </summary>
        public static bool DeepEquals(this JsonNode? node0, JsonNode? node)
        {
            return JsonNodeComparer.AreEqual(node0, node);
        }
    }
}
