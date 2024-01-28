using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBNegationOperator : VBUnaryOperator
{
    public VBNegationOperator(string expression, TypedSymbol? operand = null, VBType? type = null)
        : base(Tokens.NegationOp, expression, operand, type)
    {
    }
}
