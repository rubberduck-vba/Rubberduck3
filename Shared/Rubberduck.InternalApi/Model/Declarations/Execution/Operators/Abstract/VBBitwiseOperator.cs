using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;

public abstract record class VBBitwiseOperator : VBBinaryOperator
{
    protected VBBitwiseOperator(string token, Uri parentUri, string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null) 
        : base(token, parentUri, lhsExpression, rhsExpression, lhs, rhs)
    {
        ResolvedType = VBLongType.TypeInfo;
    }

    public VBBooleanValue ExecuteAsLogicalOp(ref VBExecutionScope context, VBTypedValue lhsValue, VBTypedValue rhsValue) =>
        new(this) { Value = ((VBLongValue)ExecuteBinaryOperator(ref context, lhsValue, rhsValue)).Value != 0 };

    protected abstract Func<int, int, int> BitwiseOp { get; }

    protected sealed override VBTypedValue ExecuteBinaryOperator(ref VBExecutionScope context, VBTypedValue lhsValue, VBTypedValue rhsValue)
    {
        var result = SymbolOperation.EvaluateBinaryOpResult(ref context, this, lhsValue, rhsValue, BitwiseOp);
        if (lhsValue.TypeInfo is VBBooleanType && rhsValue.TypeInfo is VBBooleanType)
        {
            return result;
        }

        context = context.WithDiagnostic(RubberduckDiagnostic.BitwiseOperator(this));
        return ((INumericValue)result).AsLong();
    }
}
