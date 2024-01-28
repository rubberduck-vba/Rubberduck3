using Rubberduck.InternalApi.Model.Declarations.Symbols;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBCompareLessThanOperator : VBComparisonOperator
{
    public VBCompareLessThanOperator(string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null)
        : base(Tokens.CompareLessThanOp, lhsExpression, rhsExpression, lhs, rhs)
    {
    }
}
