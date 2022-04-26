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
            : this(left, leftPos, default, null, right, rightPos, default, null,
                default)
        {
        }

        /// <summary>
        /// Creates an instance of the context.
        /// </summary>
        /// <param name="left">The left item.</param>
        /// <param name="leftPos">The index of the left item.</param>
        /// <param name="right">The right item.</param>
        /// <param name="rightPos">The index of the right item.</param>
        /// <param name="comparerOptions">The comparer options.</param>
        public ArrayItemMatchContext(JsonNode? left, int leftPos, JsonNode? right, int rightPos,
            in JsonComparerOptions comparerOptions)
            : this(left, leftPos, default, null, right, rightPos, default, null,
                comparerOptions)
        {
        }

        /// <summary>
        /// Creates an instance of the context.
        /// </summary>
        /// <param name="left">The left item.</param>
        /// <param name="leftPos">The index of the left item.</param>
        /// <param name="leftValueKind">The value kind for left item.</param>
        /// <param name="leftValue">The value of the left item.</param>
        /// <param name="right">The right item.</param>
        /// <param name="rightPos">The index of the right item.</param>
        /// <param name="rightValueKind">The value kind for right item.</param>
        /// <param name="rightValue">The value of the right item.</param>
        /// <param name="comparerOptions">The comparer options.</param>
        public ArrayItemMatchContext(JsonNode? left, int leftPos, JsonValueKind leftValueKind, object? leftValue,
            JsonNode? right, int rightPos, JsonValueKind rightValueKind, object? rightValue,
            in JsonComparerOptions comparerOptions)
        {
            Left = left;
            LeftPosition = leftPos;
            LeftValueKind = leftValueKind;
            LeftValue = leftValue;
            Right = right;
            RightPosition = rightPos;
            RightValueKind = rightValueKind;
            RightValue = rightValue;
            ComparerOptions = comparerOptions;
            IsDeepEqual = false;
        }

        internal ArrayItemMatchContext(JsonNode? left, int leftPos, in Lcs.LcsValueCacheEntry cachedLeftValue,
            JsonNode? right, int rightPos, in Lcs.LcsValueCacheEntry cachedRightValue,
            in JsonComparerOptions comparerOptions)
            : this(left, leftPos, cachedLeftValue.ValueKind, cachedLeftValue.Value,
                right, rightPos, cachedRightValue.ValueKind, cachedRightValue.Value, comparerOptions)
        {
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
        /// Gets the value kind for left item.
        /// </summary>
        public JsonValueKind LeftValueKind { get; }
        
        /// <summary>
        /// Gets the value of the left item.
        /// </summary>
        public object? LeftValue { get; }

        /// <summary>
        /// Gets the right item.
        /// </summary>
        public JsonNode? Right { get; }

        /// <summary>
        /// Gets the index of the right item.
        /// </summary>
        public int RightPosition { get; }
        
        /// <summary>
        /// Gets the value kind for right item.
        /// </summary>
        public JsonValueKind RightValueKind { get; }
        
        /// <summary>
        /// Gets the value of the right item.
        /// </summary>
        public object? RightValue { get; }

        /// <summary>
        /// Gets the comparer options.
        /// </summary>
        public JsonComparerOptions ComparerOptions { get; }

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

        /// <summary>
        /// Attempts to determine whether two value objects are equal.
        /// </summary>
        /// <param name="result">The comparison result.</param>
        public bool TryCompareValue(out bool result)
        {
            if (ComparerOptions.JsonElementComparison == JsonElementComparison.Semantic)
            {
                if (LeftValueKind is JsonValueKind.Number or JsonValueKind.String &&
                    RightValueKind is JsonValueKind.Number or JsonValueKind.String)
                {
                    result = LeftValueKind == RightValueKind &&
                             JsonValueComparer.CompareValue(LeftValueKind, LeftValue, RightValue) == 0;
                    return true;
                }
            }

            result = false;
            return false;
        }
    }
}
