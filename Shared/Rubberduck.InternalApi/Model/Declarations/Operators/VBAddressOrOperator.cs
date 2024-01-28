using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBAddressOrOperator : VBUnaryOperator
{
    public VBAddressOrOperator(string expression, TypedSymbol? operand = null)
        : base(Tokens.AddressOf, expression, operand, VBType.VbLongPtrType)
    {
    }
}
