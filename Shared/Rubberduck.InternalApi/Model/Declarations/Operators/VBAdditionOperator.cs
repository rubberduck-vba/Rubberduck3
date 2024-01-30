using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBAdditionOperator : VBBinaryOperator
{
    public VBAdditionOperator(string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null, VBType? type = null)
        : base(Tokens.AdditionOp, lhsExpression, rhsExpression, lhs, rhs, type)
    {
    }

    protected override VBTypedValue ExecuteBinaryOperator(ExecutionContext context, VBTypedValue lhs, VBTypedValue rhs)
    {
        if (!CanConvertSafely(lhs, rhs))
        {
            // TODO possible diagnostic here
        }

        if (lhs is INumericValue lhsNumeric && rhs is INumericValue rhsNumeric)
        {
            // return type is the widest of the supplied types.
            return Addition(lhsNumeric, rhsNumeric);
        }
        if (lhs is INumericValue lhsNumber && rhs is VBStringValue rhsNumericString)
        {
            if (double.TryParse(rhsNumericString.Value, out var rhsDouble))
            {
                return Addition(lhsNumber, new VBDoubleValue() { Value = rhsDouble });
            }
        }

        if (lhs is VBStringValue lhsString && rhs is VBStringValue rhsString)
        {
            return Addition(lhsString, rhsString);
        }
        if (lhs is VBNullValue lhsNull && rhs is VBStringValue rhsStringValue)
        {
            return Addition(lhsNull, rhsStringValue);
        }

        if (lhs is VBDateValue lhsDate && rhs is INumericValue rhsNumericDate)
        {
            return Addition(lhsDate, rhsNumericDate);
        }

        if (lhs is VBBooleanValue lhsBoolean && rhs is VBBooleanValue rhsBoolean)
        {
            return Addition(lhsBoolean, rhsBoolean);
        }

        // TODO issue diagnostic for invalid operands
        throw VBRuntimeErrorException.TypeMismatch;
    }

    private static VBTypedValue Addition(VBBooleanValue lhs, VBBooleanValue rhs)
    {
        // NOTE VBA actually converts to integer first
        // TODO issue diagnostic for implicit type conversion
        // TODO suggest replacing with VBLogicalAndOperator

        return new VBBooleanValue { Value = lhs.Value && rhs.Value };
    }

    private static VBTypedValue Addition(INumericValue lhs, INumericValue rhs)
    {
        var lhsType = ((VBTypedValue)lhs).TypeInfo;
        var rhsType = ((VBTypedValue)rhs).TypeInfo;
        
        // the largest size argument is the type we return
        var returnValue = rhsType.Size > lhsType.Size ? rhs : lhs;
        return returnValue.WithValue(lhs.AsDouble() + rhs.AsDouble());
    }

    private static VBTypedValue Addition(VBDateValue lhs, INumericValue rhs)
    {
        // NOTE VBA actually converts to double first
        // TODO issue diagnostic for dubious date arithmetic
        // TODO issue diagnostic for implicit type conversion
        // TODO suggest replacing with DateAdd function
        return VBDateValue.FromSerial(lhs.SerialValue + rhs.AsDouble());
    }

    private VBTypedValue Addition(VBStringValue lhs, VBStringValue rhs)
    {
        // TODO issue diagnostic / suggest replacing with string concatenation operator
        return new VBStringValue(this).WithValue($"{lhs.Value}{rhs.Value}");
    }

    private VBTypedValue Addition(VBNullValue _, VBStringValue rhs) => rhs;
}
