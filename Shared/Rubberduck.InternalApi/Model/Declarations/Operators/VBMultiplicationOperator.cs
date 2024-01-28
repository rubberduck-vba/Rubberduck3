﻿using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBMultiplicationOperator : VBBinaryOperator
{
    public VBMultiplicationOperator(string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null, VBType? type = null)
        : base(Tokens.MultiplicationOp, lhsExpression, rhsExpression, lhs, rhs, type)
    {
    }
}
