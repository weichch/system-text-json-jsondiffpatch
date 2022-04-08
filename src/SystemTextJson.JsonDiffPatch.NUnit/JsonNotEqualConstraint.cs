using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch.Nunit
{
    class JsonNotEqualConstraint : JsonDiffConstraint
    {
        public JsonNotEqualConstraint(JsonNode? expected)
            : base(expected)
        {
        }

        public override Func<JsonNode?, JsonNode?, JsonNode, string>? OutputFormatter => null;
        protected override bool Test() => Delta is not null;
    }
}