using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBNewOperator : VBUnaryOperator
{
    public VBNewOperator(string expression, TypedSymbol operand, WorkspaceUri parentUri)
        : base(Tokens.New, expression, parentUri, operand)
    {
    }

    protected override VBTypedValue? EvaluateResult(ref VBExecutionScope context)
    {
        var symbol = (TypedSymbol)Children!.Single();
        if (symbol.ResolvedType is VBClassType classTypeInfo)
        {
            if (!classTypeInfo.IsCreatable)
            {
                throw VBRuntimeErrorException.AutomationNotSupported(symbol, "Instances of this class cannot be created this way.");
            }

            return new VBObjectValue(this)
            {
                TypeInfo = classTypeInfo,
                Value = Guid.NewGuid()
            };
        }

        throw VBCompileErrorException.ExpectedIdentifier(symbol, "A class type name is expected.");
    }
}
