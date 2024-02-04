﻿using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBMultiplicationOperator : VBBinaryOperator
{
    public VBMultiplicationOperator(Uri parentUri, string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null)
        : base(Tokens.MultiplicationOp, parentUri, lhsExpression, rhsExpression, lhs, rhs)
    {
    }

    protected override VBTypedValue ExecuteBinaryOperator(ref ExecutionScope context, VBTypedValue lhsValue, VBTypedValue rhsValue) =>
        NumericSymbolOperation.EvaluateBinaryOpResult(ref context, this, lhsValue, rhsValue, (lhs, rhs) => lhs * rhs);
}
