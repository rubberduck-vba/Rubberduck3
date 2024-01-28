using Rubberduck.InternalApi.Model.Declarations.Symbols;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBCompareGreaterThanOrEqualOperator : VBComparisonOperator
{
    public VBCompareGreaterThanOrEqualOperator(string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null)
        : base(Tokens.CompareGreaterThanOrEqualOp, lhsExpression, rhsExpression, lhs, rhs)
    {
    }
}
