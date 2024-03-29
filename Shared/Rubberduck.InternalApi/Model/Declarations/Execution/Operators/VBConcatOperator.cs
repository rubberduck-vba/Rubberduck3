﻿using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBConcatOperator : VBBinaryOperator
{
    public VBConcatOperator(WorkspaceUri parentUri, string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null)
        : base(Tokens.ConcatOp, parentUri, lhsExpression, rhsExpression, lhs, rhs)
    {
    }

    protected override VBTypedValue ExecuteBinaryOperator(ref VBExecutionScope context, VBTypedValue lhsValue, VBTypedValue rhsValue)
    {
        string? lhsString = null;
        if (lhsValue is VBStringValue stringValueLhs)
        {
            lhsString = stringValueLhs.Value;
        }
        else if (lhsValue is IStringCoercion stringCoercibleLhs)
        {
            context = context.WithDiagnostics([RubberduckDiagnostic.ImplicitStringCoercion(lhsValue.Symbol!)]);
            lhsString = stringCoercibleLhs.AsCoercedString()!.Value;
        }

        if (lhsString is null)
        {
            throw VBRuntimeErrorException.TypeMismatch(lhsValue.Symbol!, "LHS value must be coercible to `String`.");
        }

        string? rhsString = null;
        if (rhsValue is VBStringValue stringValueRhs)
        {
            rhsString = stringValueRhs.Value;
        }
        else if (rhsValue is IStringCoercion stringCoercibleRhs)
        {
            context = context.WithDiagnostics([RubberduckDiagnostic.ImplicitStringCoercion(rhsValue.Symbol!)]);
            rhsString = stringCoercibleRhs.AsCoercedString()!.Value;
        }

        if (rhsString is null)
        {
            throw VBRuntimeErrorException.TypeMismatch(rhsValue.Symbol!, "RHS value must be coercible to `String`.");
        }

        return new VBStringValue(this).WithValue($"{lhsString}{rhsString}");
    }
}
