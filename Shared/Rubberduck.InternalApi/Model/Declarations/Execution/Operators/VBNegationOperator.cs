using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBNegationOperator : VBUnaryOperator
{
    public VBNegationOperator(string expression, TypedSymbol operand, Uri parentUri)
        : base(Tokens.NegationOp, expression, parentUri, operand) { }

    protected override VBTypedValue? EvaluateResult(ref VBExecutionScope context) => 
        SymbolOperation.EvaluateUnaryOpResult(ref context, this, (TypedSymbol)Children!.Single(), e => -e);
}
