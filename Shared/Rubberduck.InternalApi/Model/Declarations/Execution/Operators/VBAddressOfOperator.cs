using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBAddressOfOperator : VBUnaryOperator
{
    public VBAddressOfOperator(string expression, TypedSymbol operand, Uri parentUri)
        : base(Tokens.AddressOf, expression, parentUri, operand)
    {
    }

    protected override VBTypedValue? EvaluateResult(ref ExecutionScope context) =>
        context.GetTypedValue((TypedSymbol)Children!.Single());
}
