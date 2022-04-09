using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch.Comparison
{
    class JsonValueSemanticEqualityComparer : IEqualityComparer<JsonValue>
    {
        public bool Equals(JsonValue x, JsonValue y)
        {
            throw new NotImplementedException();
        }

        public int GetHashCode(JsonValue obj)
        {
            throw new NotImplementedException();
        }
    }
}

