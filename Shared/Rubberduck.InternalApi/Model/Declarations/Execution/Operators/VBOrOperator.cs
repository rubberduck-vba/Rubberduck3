using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBOrOperator : VBBitwiseOperator
{
    public VBOrOperator(string token, WorkspaceUri parentUri, string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null)
        : base(token, parentUri, lhsExpression, rhsExpression, lhs, rhs)
    {
    }

    protected override Func<int, int, int> BitwiseOp { get; } = (lhs, rhs) => lhs | rhs;
}
