using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBAdditionOperator : VBBinaryOperator
{
    public VBAdditionOperator(string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null, VBType? type = null)
        : base(Tokens.AdditionOp, lhsExpression, rhsExpression, lhs, rhs, type)
    {
    }

    public override VBTypedValue? Evaluate(ExecutionContext context)
    {
        var lhsType = ResolvedLeftHandSideExpression?.ResolvedType;
        var rhsType = ResolvedRightHandSideExpression?.ResolvedType;

        if (lhsType == rhsType)
        {
            // how would we return a VBTypedValue of the known VBType here?
            // in any case we're going to need lhsValue and rhsValue.
        }

        if (rhsType?.ConvertsSafelyToTypes.Contains(lhsType) ?? false)
        {
            // implicit type conversion is safe, even in @OptionStrict mode
        }

        // TODO 
        return null;
    }

    public override ExecutionContext Execute(ExecutionContext context)
    {
        var lhsType = ResolvedLeftHandSideExpression?.ResolvedType;
        var rhsType = ResolvedRightHandSideExpression?.ResolvedType;

        // TODO
        return context;
    }
}
