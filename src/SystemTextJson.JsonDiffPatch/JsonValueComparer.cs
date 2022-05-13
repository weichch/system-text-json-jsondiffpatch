using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    /// <summary>
    /// Comparer for <see cref="JsonValue"/>.
    /// </summary>
    public static partial class JsonValueComparer
    {
        /// <summary>
        /// Compares two <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="x">The left value.</param>
        /// <param name="y">The right value.</param>
        public static int Compare(JsonValue? x, JsonValue? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is null)
            {
                return -1;
            }

            if (y is null)
            {
                return 1;
            }

            var wrapperX = new JsonValueWrapper(x);
            var wrapperY = new JsonValueWrapper(y);

            return wrapperX.CompareTo(ref wrapperY);
        }
    }
}

