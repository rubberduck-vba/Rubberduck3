using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBUBoundOperator : VBUnaryOperator
{
    public VBUBoundOperator(string expression, TypedSymbol? operand = null)
        : base(Tokens.UBound, expression, operand, VBType.VbLongType)
    {
    }
}
