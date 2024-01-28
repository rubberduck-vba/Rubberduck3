using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;

public abstract record class VBBinaryOperator : VBOperator
{
    protected VBBinaryOperator(string token, string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null, VBType? type = null)
        : base(token, new[] { lhs, rhs }.Where(e => e != null).OfType<TypedSymbol>().ToArray() ?? [], type)
    {
        LeftHandSideExpression = lhsExpression;
        RightHandSideExpression = rhsExpression;
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
}
