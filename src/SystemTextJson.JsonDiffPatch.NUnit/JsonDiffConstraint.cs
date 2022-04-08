using System.Text.Json.Nodes;
using NUnit.Framework.Constraints;

namespace System.Text.Json.JsonDiffPatch.Nunit
{
    /// <summary>
    /// Represents a constraint that tests whether two JSON.
    /// </summary>
    public abstract class JsonDiffConstraint : Constraint
    {
        private JsonDiffOptions? _diffOptions;
        private Func<JsonNode?, JsonNode?, JsonNode, string>? _outputFormatter;
        
        protected JsonDiffConstraint(JsonNode? expected)
        {
            Expected = expected;
        }

        public virtual Func<JsonNode?, JsonNode?, JsonNode, string>? OutputFormatter => _outputFormatter;
        public JsonNode? Expected { get; }
        public JsonNode? Actual { get; private set; }
        public JsonNode? Delta { get; private set; }

        public JsonDiffConstraint WithDiffOptions(JsonDiffOptions diffOptions)
        {
            _diffOptions = diffOptions ?? throw new ArgumentNullException(nameof(diffOptions));
            return this;
        }
        
        public JsonDiffConstraint WithOutputFormatter(Func<JsonNode?, JsonNode?, JsonNode, string> outputFormatter)
        {
            _outputFormatter = outputFormatter ?? throw new ArgumentNullException(nameof(outputFormatter));
            return this;
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            Actual = (JsonNode?) (object?) actual;
            Delta = Expected.Diff(Actual, _diffOptions);
            return new JsonDiffConstraintResult(this, actual, Test());
        }

        protected abstract bool Test();
    }
}