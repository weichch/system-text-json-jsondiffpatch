using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch.Diffs
{
    /// <summary>
    /// Context for matching array items.
    /// </summary>
    public struct ArrayItemMatchContext
    {
        /// <summary>
        /// Creates an instance of the context.
        /// </summary>
        /// <param name="left">The left item.</param>
        /// <param name="leftPos">The index of the left item.</param>
        /// <param name="right">The right item.</param>
        /// <param name="rightPos">The index of the right item.</param>
        public ArrayItemMatchContext(JsonNode? left, int leftPos, JsonNode? right, int rightPos)
        {
            Left = left;
            LeftPosition = leftPos;
            Right = right;
            RightPosition = rightPos;
            IsDeepEqual = false;
        }

        /// <summary>
        /// Gets the left item.
        /// </summary>
        public JsonNode? Left { get; }

        /// <summary>
        /// Gets the index of the left item.
        /// </summary>
        public int LeftPosition { get; }

        /// <summary>
        /// Gets the right item.
        /// </summary>
        public JsonNode? Right { get; }

        /// <summary>
        /// Gets the index of the right item.
        /// </summary>
        public int RightPosition { get; }

        /// <summary>
        /// Gets whether the result was the two items are deeply equal.
        /// </summary>
        public bool IsDeepEqual { get; private set; }

        /// <summary>
        /// Sets <see cref="IsDeepEqual"/> to <c>true</c>.
        /// </summary>
        public void DeepEqual()
        {
            IsDeepEqual = true;
        }
    }
}
