using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBCompareLessThanOrEqualOperator : VBComparisonOperator
{
    public VBCompareLessThanOrEqualOperator(string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null)
        : base(Tokens.CompareLessThanOrEqualOp, lhsExpression, rhsExpression, lhs, rhs)
    {
    }
}
