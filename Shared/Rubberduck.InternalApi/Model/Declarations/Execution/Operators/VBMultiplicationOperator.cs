﻿using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBMultiplicationOperator : VBBinaryOperator
{
    public VBMultiplicationOperator(WorkspaceUri parentUri, string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null)
        : base(Tokens.MultiplicationOp, parentUri, lhsExpression, rhsExpression, lhs, rhs)
    {
    }
    
    protected override VBTypedValue ExecuteBinaryOperator(ref VBExecutionScope context, VBTypedValue lhsValue, VBTypedValue rhsValue) =>
        rhsValue is VBDateValue rhsDateValue
        ? (SymbolOperation.EvaluateBinaryOpResult(ref context, this, lhsValue, rhsValue, (double lhs, double rhs) => lhs * rhsDateValue.SerialValue))
        : (SymbolOperation.EvaluateBinaryOpResult(ref context, this, lhsValue, rhsValue, (int lhs, int rhs) => lhs * rhs));
}
