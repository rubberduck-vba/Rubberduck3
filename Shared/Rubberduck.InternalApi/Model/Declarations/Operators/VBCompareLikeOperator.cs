using Rubberduck.InternalApi.Model.Declarations.Symbols;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBCompareLikeOperator : VBComparisonOperator
{
    public VBCompareLikeOperator(string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null)
        : base(Tokens.CompareLikeOp, lhsExpression, rhsExpression, lhs, rhs)
    {
    }
}
