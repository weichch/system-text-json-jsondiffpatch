using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace System.Text.Json.Diffs
{
    /// <summary>
    /// Implements JSON array diff:
    /// <see link="https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#array-with-inner-changes"/>
    /// </summary>
    internal readonly struct ArrayDiff
    {
        private const int ArrayMoved = 3;

        private readonly JsonArray _left;
        private readonly JsonArray _right;

        public ArrayDiff(JsonArray left, JsonArray right)
        {
            _left = left;
            _right = right;
        }

        public JsonObject? GetDelta()
        {
            Span<JsonNode?> trimmedLeft = _left.ToArray();
            Span<JsonNode?> trimmedRight = _right.ToArray();

            var lcs = Lcs.Get(trimmedLeft, trimmedRight);

            return null;
        }
    }
}
