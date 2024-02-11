using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;

public abstract record class VBBinaryOperator : VBOperator
{
    protected VBBinaryOperator(string token, Uri parentUri, string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null)
        : base(token, parentUri, new[] { lhs, rhs }.Where(e => e != null).OfType<TypedSymbol>().ToArray() ?? [])
    {
        LeftHandSideExpression = lhsExpression;
        RightHandSideExpression = rhsExpression;

        ResolvedLeftHandSideExpression = lhs;
        ResolvedRightHandSideExpression = rhs;
    }

    public string LeftHandSideExpression { get; init; }
    public TypedSymbol? ResolvedLeftHandSideExpression { get; init; }

    public string RightHandSideExpression { get; init; }
    public TypedSymbol? ResolvedRightHandSideExpression { get; init; }

    public virtual VBBinaryOperator WithOperands(TypedSymbol? lhs, TypedSymbol? rhs)
        => this with
        {
            ResolvedLeftHandSideExpression = lhs,
            ResolvedRightHandSideExpression = rhs,
            Children = new[] { lhs, rhs }.Where(e => e != null).OfType<TypedSymbol>().ToArray() ?? [],
        };

    protected sealed override VBTypedValue? EvaluateResult(ref VBExecutionScope context)
    {
        if (!CanExecute)
        {
            throw new InvalidOperationException("Symbol operand types must be resolved first.");
        }

        var lhs = context.GetTypedValue(ResolvedLeftHandSideExpression!);
        var rhs = context.GetTypedValue(ResolvedRightHandSideExpression!);
        return ExecuteBinaryOperator(ref context, lhs, rhs);
    }

    protected abstract VBTypedValue ExecuteBinaryOperator(ref VBExecutionScope context, VBTypedValue lhsValue, VBTypedValue rhsValue);

    protected bool CanConvertSafely(VBTypedValue lhsValue, VBTypedValue rhsValue)
        => lhsValue.TypeInfo.ConvertsSafelyToTypes.Contains(rhsValue.TypeInfo);
}
