using Rubberduck.InternalApi.Model.Declarations.Symbols;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBCompareNotEqualOperator : VBComparisonOperator
{
    public VBCompareNotEqualOperator(string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null)
        : base(Tokens.CompareNotEqualOp, lhsExpression, rhsExpression, lhs, rhs)
    {
    }
}
