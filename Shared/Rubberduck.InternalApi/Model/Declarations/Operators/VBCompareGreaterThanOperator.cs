using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBCompareGreaterThanOperator : VBComparisonOperator
{
    public VBCompareGreaterThanOperator(string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null)
        : base(Tokens.CompareGreaterThanOp, lhsExpression, rhsExpression, lhs, rhs)
    {
    }
}
