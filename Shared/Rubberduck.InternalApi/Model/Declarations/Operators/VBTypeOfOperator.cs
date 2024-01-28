using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBTypeOfOperator : VBUnaryOperator
{
    public VBTypeOfOperator(string expression, TypedSymbol? operand = null, VBType? type = null)
        : base(Tokens.TypeOf, expression, operand, type)
    {
    }
}
