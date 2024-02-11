using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;

public abstract record class VBUnaryOperator : VBOperator
{
    protected VBUnaryOperator(string token, string expression, Uri parentUri, TypedSymbol? operand = null)
        : base(token, parentUri, operand is null ? null : [operand])
    {
        Expression = expression;
        ResolvedExpression = operand;
    }

    public string Expression { get; init; }
    public TypedSymbol? ResolvedExpression { get; init; }

    public VBUnaryOperator WithOperand(TypedSymbol operand) => this with { ResolvedExpression = operand, Children = new(operand) };
}
