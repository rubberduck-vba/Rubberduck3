using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBCompareEqualOperator : VBComparisonOperator
{
    public VBCompareEqualOperator(string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null)
        : base(Tokens.CompareEqualOp, lhsExpression, rhsExpression, lhs, rhs)
    {
    }

    protected override VBTypedValue ExecuteBinaryOperator(ref VBExecutionScope context, VBTypedValue lhsValue, VBTypedValue rhsValue)
    {
        throw new System.NotImplementedException();
    }
}
