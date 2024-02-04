using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public enum AssignmentKind
{
    ValueAssignment,
    ReferenceAssignment
}

public record class VBAssignmentOperator : VBBinaryOperator
{
    private readonly AssignmentKind _kind;

    public VBAssignmentOperator(AssignmentKind kind, Uri parentUri, string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null)
        : base(Tokens.CompareEqualOp, parentUri, lhsExpression, rhsExpression, lhs, rhs)
    {
        _kind = kind;
    }

    public override VBBinaryOperator WithOperands(TypedSymbol? lhs, TypedSymbol? rhs)
        => this with
        {
            ResolvedLeftHandSideExpression = lhs,
            ResolvedRightHandSideExpression = rhs,
            Children = new[] { lhs, rhs }.Where(e => e != null).OfType<TypedSymbol>().ToArray() ?? [],
            ResolvedType = lhs?.ResolvedType,
        };

    protected override VBTypedValue ExecuteBinaryOperator(ref ExecutionScope context, VBTypedValue lhsValue, VBTypedValue rhsValue)
    {
        var assignmentTarget = lhsValue;

        if (lhsValue is VBObjectValue lhsObject)
        {
            if (_kind == AssignmentKind.ReferenceAssignment && assignmentTarget.TypeInfo != rhsValue.TypeInfo)
            {
                if (assignmentTarget.TypeInfo.ConvertsSafelyToTypes.Contains(rhsValue.TypeInfo))
                {
                    context = context.WithDiagnostics([RubberduckDiagnostic.TypeCastConversion(this)]);
                }
                else
                {
                    throw VBRuntimeErrorException.TypeMismatch(this, "This reference assignment is referring to incompatible object types.");
                }
            }
            else if (_kind == AssignmentKind.ValueAssignment)
            {
                assignmentTarget = lhsObject.LetCoerce();
                context = context.WithDiagnostics([RubberduckDiagnostic.ImplicitLetCoercion(lhsObject.Symbol!)]);

                if (rhsValue is VBObjectValue rhsObject)
                {
                    context = context.WithDiagnostics([RubberduckDiagnostic.SuspiciousValueAssignment(this)]);
                }
            }
        }

        if (assignmentTarget.TypeInfo != rhsValue.TypeInfo)
        {
            if (!assignmentTarget.TypeInfo.ConvertsSafelyToTypes.Contains(rhsValue.TypeInfo))
            {
                if (rhsValue is VBObjectValue rhsObject && _kind == AssignmentKind.ValueAssignment)
                {
                    var letCoercedValue = rhsObject.LetCoerce();
                    context = context.WithDiagnostics([RubberduckDiagnostic.ImplicitLetCoercion(rhsObject.Symbol!)]);
                    context.SetTypedValue(lhsValue.Symbol!, letCoercedValue);
                    return letCoercedValue;
                }

                throw VBRuntimeErrorException.TypeMismatch(this, "This value assignment is referring to incompatible data types.");
            }
            else if (_kind == AssignmentKind.ValueAssignment)
            {
                if (assignmentTarget.TypeInfo.ConvertsSafelyToTypes.Contains(rhsValue.TypeInfo))
                {
                    context = context.WithDiagnostics([RubberduckDiagnostic.ImplicitWideningConversion(this)]);
                }
                else
                {
                    throw VBRuntimeErrorException.TypeMismatch(this, "This value assignment is referring to conflicting data types.");
                }
            }
        }

        context.SetTypedValue(assignmentTarget.Symbol!, rhsValue);
        return rhsValue;
    }
}
