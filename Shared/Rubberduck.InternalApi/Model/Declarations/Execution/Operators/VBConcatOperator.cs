using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBConcatOperator : VBBinaryOperator
{
    public VBConcatOperator(string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null, VBType? type = null)
        : base(Tokens.ConcatOp, lhsExpression, rhsExpression, lhs, rhs, type)
    {
    }

    protected override VBTypedValue ExecuteBinaryOperator(VBExecutionContext context, VBTypedValue lhsValue, VBTypedValue rhsValue)
    {
        string? lhsString;
        if (lhsValue is VBStringValue stringValueLhs)
        {
            lhsString = stringValueLhs.Value;
        }
        else if (lhsValue is IStringCoercion stringCoercibleLhs)
        {
            // TODO issue a diagnostic for the string coercion
            lhsString = stringCoercibleLhs.AsCoercedString();
        }
        else
        {
            throw VBRuntimeErrorException.TypeMismatch;
        }

        string? rhsString;
        if (rhsValue is VBStringValue stringValueRhs)
        {
            rhsString = stringValueRhs.Value;
        }
        else if (rhsValue is IStringCoercion stringCoercibleRhs)
        {
            // TODO issue a diagnostic for the string coercion
            rhsString = stringCoercibleRhs.AsCoercedString();
        }
        else
        {
            throw VBRuntimeErrorException.TypeMismatch;
        }

        return new VBStringValue(this).WithValue($"{lhsString}{rhsString}");
    }
}
