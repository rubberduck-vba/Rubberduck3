using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBModuloOperator : VBBinaryOperator
{
    public VBModuloOperator(Uri parentUri, string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null)
        : base(Tokens.ModuloOp, parentUri, lhsExpression, rhsExpression, lhs, rhs)
    {
    }

    protected override VBTypedValue ExecuteBinaryOperator(ref VBExecutionScope context, VBTypedValue lhsValue, VBTypedValue rhsValue) =>
        NumericSymbolOperation.EvaluateBinaryOpResult(ref context, this, lhsValue, rhsValue, (lhs, rhs) =>
        {
            if (rhs == 0)
            {
                throw VBRuntimeErrorException.DivisionByZero(this, "RHS/divisor operand must be non-zero. Consider validating the expression before using it as a divisor operand.");
            }
            return lhs % rhs;
        });
}
