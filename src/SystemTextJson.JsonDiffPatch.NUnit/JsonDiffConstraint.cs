using System.Text.Json.Nodes;
using NUnit.Framework.Constraints;

namespace System.Text.Json.JsonDiffPatch.Nunit
{
    /// <summary>
    /// Represents a constraint that tests whether two JSON.
    /// </summary>
    public abstract class JsonDiffConstraint : Constraint
    {
        private readonly JsonNode? _expected;
        private JsonDiffOptions? _diffOptions;
        private Func<JsonNode, string>? _outputFormatter;
        
        protected JsonDiffConstraint(JsonNode? expected)
        {
            _expected = expected;
        }

        public virtual Func<JsonNode, string>? OutputFormatter => _outputFormatter;
        public JsonNode? Delta { get; private set; }

        public JsonDiffConstraint WithDiffOptions(JsonDiffOptions diffOptions)
        {
            _diffOptions = diffOptions ?? throw new ArgumentNullException(nameof(diffOptions));
            return this;
        }
        
        public JsonDiffConstraint WithOutputFormatter(Func<JsonNode, string> outputFormatter)
        {
            _outputFormatter = outputFormatter ?? throw new ArgumentNullException(nameof(outputFormatter));
            return this;
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            Delta = _expected.Diff((JsonNode?) (object?) actual, _diffOptions);
            return new JsonDiffConstraintResult(this, actual, Test());
        }

        protected abstract bool Test();
    }
}