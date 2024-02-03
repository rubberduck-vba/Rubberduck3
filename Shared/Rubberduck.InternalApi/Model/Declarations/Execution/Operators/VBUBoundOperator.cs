using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System.Linq;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBUBoundOperator : VBUnaryOperator
{
    public VBUBoundOperator(string expression, TypedSymbol operand, Uri parentUri)
        : base(Tokens.UBound, expression, parentUri, operand)
    {
    }

    public override VBTypedValue? Evaluate(ExecutionScope context)
    {
        var symbol = (TypedSymbol)Children!.Single();
        var value = context.GetTypedValue(symbol);

        if (value.TypeInfo is VBArrayType arrayType)
        {
            return arrayType.DeclaredUpperBound.HasValue
                ? new VBLongValue(this).WithValue(arrayType.DeclaredUpperBound.Value)
                : new VBLongValue(this).WithValue(VBArrayType.ImplicitBoundary);
        }

        throw new InvalidOperationException("VBCompileError: Expected array");
    }
}
