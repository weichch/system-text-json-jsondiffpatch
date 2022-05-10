using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    internal readonly struct JsonComparerOptions
    {
        private readonly JsonElementComparison? _jsonElementComparison;

        public JsonComparerOptions(JsonElementComparison? jsonElementComparison,
            IEqualityComparer<JsonValue>? valueComparer)
        {
            _jsonElementComparison = jsonElementComparison;
            ValueComparer = valueComparer;
        }

        public JsonElementComparison JsonElementComparison =>
            _jsonElementComparison ?? JsonDiffPatcher.DefaultDeepEqualsComparison;

        public IEqualityComparer<JsonValue>? ValueComparer { get; }
    }
}