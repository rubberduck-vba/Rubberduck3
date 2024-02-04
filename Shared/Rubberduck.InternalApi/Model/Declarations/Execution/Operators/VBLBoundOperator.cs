using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBLBoundOperator : VBUnaryOperator
{
    public VBLBoundOperator(string expression, TypedSymbol operand, Uri parentUri)
        : base(Tokens.LBound, expression, parentUri, operand)
    {
    }

    protected override VBTypedValue? EvaluateResult(ref ExecutionScope context)
    {
        var symbol = (TypedSymbol)Children!.First();
        var value = context.GetTypedValue(symbol);

        if (value.TypeInfo is VBArrayType arrayType)
        {
            return arrayType.DeclaredLowerBound.HasValue
                ? new VBLongValue(this).WithValue(arrayType.DeclaredLowerBound.Value)
                : new VBLongValue(this).WithValue(VBArrayType.ImplicitBoundary);
        }

        throw VBCompileErrorException.ExpectedArray(symbol, "Use the `LBound` operator to find the lower boundary of an array variable.");
    }
}
