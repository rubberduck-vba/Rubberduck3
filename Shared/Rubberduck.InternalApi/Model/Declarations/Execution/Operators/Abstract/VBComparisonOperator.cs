using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;

public abstract record class VBComparisonOperator : VBBinaryOperator
{
    protected VBComparisonOperator(string token, Uri parentUri, string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null)
        : base(token, parentUri, lhsExpression, rhsExpression, lhs, rhs)
    {
        ResolvedType = VBType.VbBooleanType;
    }
}
