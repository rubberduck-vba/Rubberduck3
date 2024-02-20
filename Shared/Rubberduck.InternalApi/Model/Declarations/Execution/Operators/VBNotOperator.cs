using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBNotOperator : VBUnaryOperator
{
    public VBNotOperator(string expression, WorkspaceUri parentUri, TypedSymbol? operand = null) 
        : base(Tokens.Not, expression, parentUri, operand) { }

    protected override VBTypedValue? EvaluateResult(ref VBExecutionScope context)
    {
        var operand = (TypedSymbol)Children!.Single();
        if (operand.ResolvedType != VBBooleanType.TypeInfo)
        {
            context = context.WithDiagnostic(RubberduckDiagnostic.BitwiseOperator(this));
        }
        return SymbolOperation.EvaluateUnaryOpResult(ref context, this, operand, e => ~(int)e);
    }
}