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

        /// <summary>
        /// Gets the left value in comparison.
        /// </summary>
        /// <typeparam name="T">The type of left value.</typeparam>
        public T Left<T>()
        {
            if (_leftNode is T leftValue)
            {
                return leftValue;
            }

            throw new InvalidOperationException($"Type must be '{nameof(JsonNode)}' or derived type.");
        }

        /// <summary>
        /// Gets the right value in comparison.
        /// </summary>
        /// <typeparam name="T">The type of right value.</typeparam>
        public T Right<T>()
        {
            if (_rightNode is T rightValue)
            {
                return rightValue;
            }

            throw new InvalidOperationException($"Type must be '{nameof(JsonNode)}' or derived type.");
        }
    }
}
