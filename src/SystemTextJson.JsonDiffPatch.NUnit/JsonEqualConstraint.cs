using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch.Nunit
{
    class JsonEqualConstraint : JsonDiffConstraint
    {
        public JsonEqualConstraint(JsonNode? expected)
            : base(expected)
        {
        }

        protected override bool Test() => Delta is null;
    }
}