using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;

public abstract record class VBUnaryOperator : VBOperator
{
    protected VBUnaryOperator(string token, string expression, TypedSymbol? operand = null, VBType? type = null)
        : base(token, operand is null ? null : [operand], type)
    {
        Expression = expression;
        ResolvedExpression = operand;
    }

    public string Expression { get; init; }
    public TypedSymbol? ResolvedExpression { get; init; }

    public VBUnaryOperator WithOperand(TypedSymbol operand) => this with { ResolvedExpression = operand, Children = new(operand) };

    protected override ExecutionContext ExecuteOperator(ExecutionContext context)
    {
        context.TryRunAction(() =>
        {
            if (ResolvedExpression is null)
            {
                throw new InvalidOperationException($"Unary expression symbol is not resolved.");
            }

            var value = context.ReadSymbolValue(ResolvedExpression, this);
            context.WriteSymbolValue(ResolvedExpression, ExecuteUnaryOperator(value), this);
        });
        return context;
    }

    protected abstract VBTypedValue ExecuteUnaryOperator(VBTypedValue value);
}
