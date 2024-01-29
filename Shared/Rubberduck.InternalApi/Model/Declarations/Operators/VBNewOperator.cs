using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBNewOperator : VBUnaryOperator
{
    public VBNewOperator(string expression, TypedSymbol? operand = null, VBType? type = null)
        : base(Tokens.New, expression, operand, type)
    {
    }

    protected override VBTypedValue ExecuteUnaryOperator(VBTypedValue value)
    {
        throw new System.NotImplementedException();
    }
}
