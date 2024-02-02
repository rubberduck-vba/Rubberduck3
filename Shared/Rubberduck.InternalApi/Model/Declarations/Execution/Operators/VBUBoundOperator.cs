using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBUBoundOperator : VBUnaryOperator
{
    public VBUBoundOperator(string expression, TypedSymbol? operand = null)
        : base(Tokens.UBound, expression, operand, VBType.VbLongType)
    {
    }

    protected override VBTypedValue ExecuteUnaryOperator(VBTypedValue value)
    {
        if (value.TypeInfo is VBArrayType arrayType && arrayType.DeclaredUpperBound.HasValue)
        {
            return new VBLongValue(this).WithValue(arrayType.DeclaredUpperBound.Value);
        }

        return new VBLongValue(this).WithValue(VBArrayType.ImplicitBoundary);
    }
}
