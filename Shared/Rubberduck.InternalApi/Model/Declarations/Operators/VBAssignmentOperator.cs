using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBAssignmentOperator : VBBinaryOperator
{
    public VBAssignmentOperator(string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null)
        : base(Tokens.CompareEqualOp, lhsExpression, rhsExpression, lhs, rhs, lhs?.ResolvedType)
    {
    }

    public override VBBinaryOperator WithOperands(TypedSymbol? lhs, TypedSymbol? rhs)
        => this with
        {
            ResolvedLeftHandSideExpression = lhs,
            ResolvedRightHandSideExpression = rhs,
            Children = new[] { lhs, rhs }.Where(e => e != null).OfType<TypedSymbol>().ToArray() ?? [],
            ResolvedType = lhs?.ResolvedType,
        };
}
