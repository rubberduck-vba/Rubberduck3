using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBNewOperator : VBUnaryOperator
{
    public VBNewOperator(string expression, TypedSymbol operand, Uri parentUri)
        : base(Tokens.New, expression, parentUri, operand)
    {
    }

    public override VBTypedValue? Evaluate(ExecutionScope context)
    {
        var type = (TypedSymbol)Children!.Single();
        return new VBObjectValue 
        { 
            Symbol = this, 
            TypeInfo = type.ResolvedType!, 
            Value = new object() 
        };
    }
}
