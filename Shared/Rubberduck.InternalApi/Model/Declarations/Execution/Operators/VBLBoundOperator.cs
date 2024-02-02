using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBLBoundOperator : VBUnaryOperator
{
    public VBLBoundOperator(string expression, TypedSymbol? operand = null)
        : base(Tokens.LBound, expression, operand, VBType.VbLongType)
    {
    }

    protected override VBTypedValue ExecuteUnaryOperator(VBTypedValue value)
    {
        if (value.TypeInfo is VBArrayType arrayType && arrayType.DeclaredLowerBound.HasValue)
        {
            return new VBLongValue(this).WithValue(arrayType.DeclaredLowerBound.Value);
        }

        return new VBLongValue(this).WithValue(VBArrayType.ImplicitBoundary);
    }
}
