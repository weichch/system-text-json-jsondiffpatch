using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch.Diffs
{
    /// <summary>
    /// Represents JSON diff context.
    /// </summary>
    public class JsonDiffContext
    {
        private readonly JsonNode _leftNode;
        private readonly JsonNode _rightNode;

        internal JsonDiffContext(JsonNode left, JsonNode right)
        {
            _leftNode = left;
            _rightNode = right;
        }

        public T Left<T>()
        {
            if (typeof(T) == typeof(JsonNode))
            {
                return (T) (object) _leftNode;
            }

            throw new InvalidOperationException($"Type must be '{nameof(JsonNode)}'.");
        }

        public T Right<T>()
        {
            if (typeof(T) == typeof(JsonNode))
            {
                return (T) (object) _rightNode;
            }

            throw new InvalidOperationException($"Type must be '{nameof(JsonNode)}'.");
        }
    }
}
