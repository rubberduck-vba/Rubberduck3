using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBAddressOfOperator : VBUnaryOperator
{
    public VBAddressOfOperator(string expression, TypedSymbol? operand = null)
        : base(Tokens.AddressOf, expression, operand, VBType.VbLongPtrType)
    {
    }

    protected override VBTypedValue ExecuteUnaryOperator(VBTypedValue value) => new VBLongPtrValue(value.Symbol);
}
