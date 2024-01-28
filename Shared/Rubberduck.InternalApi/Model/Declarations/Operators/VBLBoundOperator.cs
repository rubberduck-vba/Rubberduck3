using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBLBoundOperator : VBUnaryOperator
{
    public VBLBoundOperator(string expression, TypedSymbol? operand = null)
        : base(Tokens.LBound, expression, operand, VBType.VbLongType)
    {
    }
}
