using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBCompareGreaterThanOrEqualOperator : VBComparisonOperator
{
    public VBCompareGreaterThanOrEqualOperator(Uri parentUri, string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null)
        : base(Tokens.CompareGreaterThanOrEqualOp, parentUri, lhsExpression, rhsExpression, lhs, rhs)
    {
    }

    protected override VBTypedValue ExecuteBinaryOperator(ref VBExecutionScope context, VBTypedValue lhsValue, VBTypedValue rhsValue)
    {
        if (lhsValue.TypeInfo is VBStringType)
        {
            return SymbolOperation.EvaluateCompareOpResult(ref context, this, lhsValue, rhsValue,
                (lhs, rhs, comparison) => string.Compare(lhs, rhs, comparison) >= 0);
        }
        else
        {
            if (lhsValue is INumericValue lhsNumeric)
            {
                return SymbolOperation.EvaluateCompareOpResult(ref context, this, lhsNumeric, rhsValue,
                    (lhs, rhs) => lhs.CompareTo(rhs) >= 0);
            }
        }
        throw VBRuntimeErrorException.TypeMismatch(this, $"Types {lhsValue.TypeInfo.Name} and {rhsValue.TypeInfo.Name} are not comparable.");
    }
}
