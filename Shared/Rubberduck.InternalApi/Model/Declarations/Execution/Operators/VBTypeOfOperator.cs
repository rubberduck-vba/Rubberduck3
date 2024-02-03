using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBTypeOfOperator : VBUnaryOperator
{
    public VBTypeOfOperator(string expression, TypedSymbol operand, Uri parentUri)
        : base(Tokens.TypeOf, expression, parentUri, operand)
    {
    }

    public override VBTypedValue? Evaluate(ExecutionScope context)
    {
        var symbol = (TypedSymbol)Children!.Single();
        return new VBTypeDescValue(symbol);
    }
}
