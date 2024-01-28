using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBSubtractionOperator : VBBinaryOperator
{
    public VBSubtractionOperator(string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null, VBType? type = null)
        : base(Tokens.SubtractionOp, lhsExpression, rhsExpression, lhs, rhs, type)
    {
    }
}
