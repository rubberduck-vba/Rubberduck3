using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
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

    public VBAssignmentOperator(AssignmentKind kind, string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null)
        : base(Tokens.CompareEqualOp, lhsExpression, rhsExpression, lhs, rhs, lhs?.ResolvedType)
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

    protected override VBTypedValue ExecuteBinaryOperator(VBExecutionContext context, VBTypedValue lhsValue, VBTypedValue rhsValue)
    {
        var assignmentTarget = lhsValue;
        if (lhsValue is VBObjectValue lhsObject && _kind == AssignmentKind.ValueAssignment)
        {
            assignmentTarget = lhsObject.LetCoerce();
            if (assignmentTarget is null)
            {
                throw VBRuntimeErrorException.TypeMismatch; // TODO get the real error, this isn't it (memory fails right now)
            }
        }

        if (assignmentTarget.TypeInfo != rhsValue.TypeInfo)
        {
            if (!assignmentTarget.TypeInfo.ConvertsSafelyToTypes.Contains(rhsValue.TypeInfo))
            {
                if (rhsValue is VBObjectValue rhsObject && _kind == AssignmentKind.ValueAssignment)
                {
                    // TODO issue diagnostic for implicit let coercion
                    var letCoercedValue = rhsObject.LetCoerce();
                    if (letCoercedValue != null)
                    {
                        return ExecuteBinaryOperator(context, assignmentTarget, letCoercedValue);
                    }
                }

                throw VBRuntimeErrorException.TypeMismatch;
            }
            else if (_kind == AssignmentKind.ValueAssignment)
            {
                // TODO issue diagnostic for implicit value conversion
            }
        }

        context.WriteSymbolValue(assignmentTarget.Symbol!, rhsValue, this);
        return rhsValue;
    }
}
