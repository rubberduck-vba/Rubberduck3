using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;

public abstract record class VBComparisonOperator : VBBinaryOperator
{
    protected VBComparisonOperator(string token, string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null)
        : base(token, lhsExpression, rhsExpression, lhs, rhs, VBType.VbBooleanType)
    {
    }
}
