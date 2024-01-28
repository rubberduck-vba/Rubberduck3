using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

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
}
