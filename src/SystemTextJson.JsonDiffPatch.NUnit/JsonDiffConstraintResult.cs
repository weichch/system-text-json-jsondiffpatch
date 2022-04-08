using NUnit.Framework.Constraints;

namespace System.Text.Json.JsonDiffPatch.Nunit
{
    class JsonDiffConstraintResult : ConstraintResult
    {
        private readonly JsonDiffConstraint _constraint;

        public JsonDiffConstraintResult(JsonDiffConstraint constraint, object actual, bool isSuccess)
            : base(constraint, actual, isSuccess)
        {
            _constraint = constraint;
        }

        public override void WriteMessageTo(MessageWriter writer)
        {
            if (_constraint.OutputFormatter is null || _constraint.Delta is null)
            {
                return;
            }

            writer.Write(_constraint.OutputFormatter(_constraint.Expected, _constraint.Actual, _constraint.Delta));
        }
    }
}