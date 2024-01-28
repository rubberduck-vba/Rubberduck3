using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBAdditionOperator : VBBinaryOperator
{
    public VBAdditionOperator(string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null, VBType? type = null)
        : base(Tokens.AdditionOp, lhsExpression, rhsExpression, lhs, rhs, type)
    {
    }
}
