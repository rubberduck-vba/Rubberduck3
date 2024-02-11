using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public static class SymbolOperation
{
    public static VBTypedValue EvaluateUnaryOpResult(ref VBExecutionScope context, VBUnaryOperator opSymbol, TypedSymbol symbol, Func<double, double> unaryOp)
    {
        var type = symbol.ResolvedType;
        if (type is VBVariantType variant)
        {
            type = context.GetTypedValue(symbol).TypeInfo ?? variant.Subtype;
        }

        if (type is VBNullType)
        {
            context = context.WithDiagnostic(RubberduckDiagnostic.UnintendedConstantExpression(symbol));
            return new VBNullValue(symbol);
        }

        if (opSymbol is VBNotOperator && type is VBBooleanType)
        {
            var value = (VBBooleanValue)context.GetTypedValue(symbol);
            return new VBBooleanValue(symbol).WithValue((int)unaryOp.Invoke(value.AsCoercedNumeric().NumericValue));
        }

        if (type is VBEmptyType)
        {
            context = context.WithDiagnostic(RubberduckDiagnostic.UnintendedConstantExpression(symbol));
            return new VBLongValue(symbol).WithValue(0);
        }

        if (type is INumericType)
        {
            var value = unaryOp.Invoke(((INumericValue)context.GetTypedValue(symbol)).AsDouble().Value);
            if (opSymbol is VBNotOperator)
            {
                return new VBLongValue(symbol).WithValue(value);
            }

            if (type is VBByteType || type is VBIntegerType)
            {
                if (type is VBByteType)
                {
                    context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitWideningConversion(symbol));
                }
                return new VBIntegerValue(symbol) { NumericValue = (short)value };
            }
            if (type is VBLongType || (!context.Is64BitHost && type is VBLongPtrType))
            {
                return new VBLongValue(symbol) { NumericValue = (int)value };
            }
            if (type is VBLongLongType || (context.Is64BitHost && type is VBLongPtrType))
            {
                return new VBLongLongValue(symbol) { NumericValue = (long)value };
            }
            if (type is VBCurrencyType)
            {
                return new VBCurrencyValue(symbol) { NumericValue = value };
            }
            if (type is VBDecimalType)
            {
                return new VBDecimalValue(symbol) { NumericValue = value };
            }
            if (type is VBSingleType)
            {
                return new VBSingleValue(symbol) { NumericValue = (float)value };
            }
            if (type is VBDoubleType)
            {
                return new VBDoubleValue(symbol) { NumericValue = (double)value };
            }

            throw VBRuntimeErrorException.TypeMismatch(symbol);
        }

        if (context.GetTypedValue(symbol) is INumericCoercion coercible)
        {
            var value = unaryOp.Invoke(coercible.AsCoercedNumeric()!.Value);
            if (type is VBDateType)
            {
                context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitDateSerialConversion(symbol));
                return VBDateValue.FromSerial(value);
            }
            return new VBIntegerValue(symbol) { NumericValue = value };
        }

        if (type is VBObjectType)
        {
            var value = context.GetTypedValue(symbol);
            if (value is VBNothingValue)
            {
                throw VBRuntimeErrorException.ObjectVariableNotSet(symbol);
            }

            throw VBRuntimeErrorException.ObjectDoesntSupportPropertyOrMethod(symbol);
        }
        else
        {
            throw VBRuntimeErrorException.TypeMismatch(symbol);
        }
    }

    public static VBBooleanValue EvaluateCompareOpResult(ref VBExecutionScope context, TypedSymbol opSymbol, VBTypedValue lhsValue, VBTypedValue rhsValue, Func<string, string, StringComparison, bool> compareOp)
    {
        if (lhsValue is VBStringValue lhsString)
        {
            if (rhsValue is IStringCoercion coercible)
            {
                var rhsString = coercible.AsCoercedString()?.Value;
                if (rhsValue.TypeInfo != VBErrorType.TypeInfo)
                {
                    context.WithDiagnostic(RubberduckDiagnostic.ImplicitStringCoercion(rhsValue.Symbol!));
                }

                // TODO handle VBOptionCompare.Database... somehow
                var stringComparison = context.OptionCompare == VBOptionCompare.Binary
                    ? StringComparison.Ordinal : StringComparison.InvariantCultureIgnoreCase;

                var result = compareOp.Invoke(lhsString.Value!, rhsString!, stringComparison);
                return new VBBooleanValue(opSymbol) { Value = result };
            }
        }

        throw VBRuntimeErrorException.TypeMismatch(opSymbol, "The data types involved in this comparison operation are not compatible.");
    }

    public static VBBooleanValue EvaluateCompareOpResult(ref VBExecutionScope context, TypedSymbol opSymbol, VBNumericTypedValue lhsValue, VBTypedValue rhsValue, Func<double, double, bool> compareOp)
    {
        var lhsNumeric = lhsValue.AsDouble().Value;
        if (lhsValue.TypeInfo == rhsValue.TypeInfo)
        {
            return new VBBooleanValue(opSymbol) { Value = compareOp.Invoke(lhsNumeric, ((INumericValue)rhsValue).AsDouble().Value) };
        }

        if (rhsValue is INumericValue rhsNumeric)
        {
            if (!rhsValue.TypeInfo.ConvertsSafelyToType(rhsValue.TypeInfo))
            {
                if (context.OptionStrict)
                {
                    throw VBCompileErrorException.OptionStrictForbidden(opSymbol, $"Narrowing conversion from `{rhsValue.TypeInfo.Name}` to `{lhsValue.TypeInfo.Name}` is not permitted with `@OptionStrict`.");
                }
                context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitNarrowingConversion(rhsValue.Symbol!));
            }

            return new VBBooleanValue(opSymbol) { Value = compareOp.Invoke(lhsNumeric, rhsNumeric.AsDouble().Value) };
        }

        if (rhsValue is INumericCoercion coercible)
        {
            var rhsCoerced = coercible.AsCoercedNumeric()?.Value ?? 0;
            context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitNumericCoercion(rhsValue.Symbol!));

            return new VBBooleanValue(opSymbol) { Value = compareOp.Invoke(lhsNumeric, rhsCoerced) };
        }

        throw VBRuntimeErrorException.TypeMismatch(opSymbol, "The data types involved in this comparison operation are not compatible.");
    }

    public static VBTypedValue EvaluateBinaryOpResult(ref VBExecutionScope context, TypedSymbol opSymbol, VBTypedValue lhsValue, VBTypedValue rhsValue, Func<double, double, double> binaryOp)
    {
        var lhsType = lhsValue.TypeInfo!;
        if (lhsType is VBVariantType lhsVariant)
        {
            lhsType = lhsVariant.Subtype;
        }

        var rhsType = rhsValue.TypeInfo!;
        if (rhsType is VBVariantType rhsVariant)
        {
            rhsType = rhsVariant.Subtype;
        }

        if (lhsType is VBNullType || rhsType is VBNullType)
        {
            return VBNullValue.Null;
        }

        if (lhsValue is VBStringValue lhsString)
        {
            return EvaluateStringCoercedNumericOp(ref context, opSymbol, lhsString, rhsValue, binaryOp);
        }

        if (lhsValue is VBNumericTypedValue lhsNumericValue)
        {
            return EvaluateNumericOp(ref context, opSymbol, lhsNumericValue, rhsValue, binaryOp);
        }

        throw VBRuntimeErrorException.TypeMismatch(opSymbol, "The data types involved in this binary operation are not compatible.");
    }

    public static VBTypedValue EvaluateBinaryOpResult(ref VBExecutionScope context, TypedSymbol opSymbol, VBTypedValue lhsValue, VBTypedValue rhsValue, Func<int, int, int> binaryOp)
    {
        var lhsType = lhsValue.TypeInfo!;
        if (lhsType is VBVariantType lhsVariant)
        {
            lhsType = lhsVariant.Subtype;
        }

        var rhsType = rhsValue.TypeInfo!;
        if (rhsType is VBVariantType rhsVariant)
        {
            rhsType = rhsVariant.Subtype;
        }

        if (lhsType is VBNullType || rhsType is VBNullType)
        {
            context = context.WithDiagnostic(RubberduckDiagnostic.UnintendedConstantExpression(opSymbol));
            return VBNullValue.Null;
        }

        if (opSymbol is VBBitwiseOperator)
        {
            if (lhsType is VBEmptyType)
            {
                context = context.WithDiagnostic(RubberduckDiagnostic.UnintendedConstantExpression(lhsValue.Symbol!));
                lhsValue = VBLongValue.Zero;
            }
            if (rhsType is VBEmptyType)
            {
                context = context.WithDiagnostic(RubberduckDiagnostic.UnintendedConstantExpression(rhsValue.Symbol!));
                rhsValue = VBLongValue.Zero;
            }
        }

        if (lhsValue is VBStringValue lhsString)
        {
            return EvaluateStringCoercedIntegerOp(ref context, opSymbol, lhsString, rhsValue, (lhs, rhs) => binaryOp(lhs, rhs));
        }

        if (lhsType is VBDateType)
        {
            var lhsSerialDateValue = new VBDoubleValue(lhsValue.Symbol) { NumericValue = ((VBDateValue)lhsValue).SerialValue };
            context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitDateSerialConversion(lhsValue.Symbol!));
            var result = ((VBDoubleValue)EvaluateIntegerOp(ref context, opSymbol, lhsSerialDateValue, rhsValue, binaryOp)).NumericValue;

            if (opSymbol is VBAdditionOperator || opSymbol is VBSubtractionOperator)
            {
                return VBDateValue.FromSerial(result);
            }

            return new VBDoubleValue(opSymbol) { NumericValue = result };
        }

        if (lhsValue is VBNumericTypedValue lhsNumericValue)
        {
            var result = EvaluateIntegerOp(ref context, opSymbol, lhsNumericValue, rhsValue, binaryOp);
            if (opSymbol is VBAdditionOperator || opSymbol is VBSubtractionOperator || opSymbol is VBMultiplicationOperator)
            {
                return result;
            }
            return ((INumericValue)result).AsDouble();
        }

        if (lhsValue is VBBooleanValue lhsBool && rhsValue is VBBooleanValue rhsBool)
        {
            var lhsNumeric = lhsBool.AsCoercedNumeric().AsLong().Value;
            var rhsNumeric = rhsBool.AsCoercedNumeric().AsLong().Value;
            return new VBBooleanValue(opSymbol).WithValue(binaryOp.Invoke(lhsNumeric, rhsNumeric));
        }

        throw VBRuntimeErrorException.TypeMismatch(opSymbol, "The data types involved in this binary operation are not compatible.");
    }

    public static VBTypedValue EvaluateBinaryOpResult(ref VBExecutionScope context, TypedSymbol opSymbol, VBTypedValue lhsValue, VBTypedValue rhsValue, Func<int, int, double> binaryOp)
    {
        var lhsType = lhsValue.TypeInfo!;
        if (lhsType is VBVariantType lhsVariant)
        {
            lhsType = lhsVariant.Subtype;
        }

        var rhsType = rhsValue.TypeInfo!;
        if (rhsType is VBVariantType rhsVariant)
        {
            rhsType = rhsVariant.Subtype;
        }

        if (lhsType is VBNullType || rhsType is VBNullType)
        {
            context = context.WithDiagnostic(RubberduckDiagnostic.UnintendedConstantExpression(opSymbol));
            return VBNullValue.Null;
        }

        if (lhsValue is VBStringValue lhsString)
        {
            return EvaluateStringCoercedIntegerOp(ref context, opSymbol, lhsString, rhsValue, (lhs, rhs) => binaryOp(lhs, rhs));
        }

        if (lhsType is VBDateType)
        {
            var lhsNumericValue = new VBDoubleValue(lhsValue.Symbol) { NumericValue = ((VBDateValue)lhsValue).SerialValue };
            context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitDateSerialConversion(lhsValue.Symbol!));
            var result = ((VBDoubleValue)EvaluateIntegerOp(ref context, opSymbol, lhsNumericValue, rhsValue, (lhs, rhs) => (int)binaryOp(lhs, rhs))).NumericValue;

            if (opSymbol is VBAdditionOperator || opSymbol is VBSubtractionOperator)
            {
                return VBDateValue.FromSerial(result);
            }

            return new VBDoubleValue(opSymbol) { NumericValue = result };
        }

        if (lhsType is INumericType)
        {
            var lhsNumericValue = (VBNumericTypedValue)lhsValue;
            var result = EvaluateIntegerOp(ref context, opSymbol, lhsNumericValue, rhsValue, (lhs, rhs) => (int)binaryOp(lhs, rhs));
            if (opSymbol is VBAdditionOperator || opSymbol is VBSubtractionOperator)
            {
                return result;
            }
            return ((INumericValue)result).AsDouble();
        }

        throw VBRuntimeErrorException.TypeMismatch(opSymbol, "The data types involved in this binary operation are not compatible.");
    }

    #region TODO refactor
    private static VBTypedValue EvaluateStringCoercedNumericOp(ref VBExecutionScope context, TypedSymbol opSymbol, VBStringValue lhsString, VBTypedValue rhsValue, Func<double, double, double> binaryOp)
    {
        if (rhsValue is VBStringValue rhsString && opSymbol is VBAdditionOperator)
        {
            context = context.WithDiagnostic(RubberduckDiagnostic.PreferConcatOperatorForStringConcatenation(opSymbol));
            return lhsString.WithValue(lhsString.Value + rhsString.Value);
        }

        var rhsType = rhsValue.TypeInfo;
        if (rhsValue is VBNumericTypedValue numeric)
        {
            double lhsNumberValue;
            if (lhsString.Value.StartsWith("&H"))
            {
                try
                {
                    lhsNumberValue = Convert.ToInt32(lhsString.Value.Replace("&H", "0x"), 16);
                }
                catch
                {
                    throw VBRuntimeErrorException.TypeMismatch(opSymbol, "This expression evaluates to a `Double`; LHS `String` value must have a numeric value. Consider explicitly validating and converting the values first.");
                }
            }
            else if (!double.TryParse(lhsString.Value, out lhsNumberValue) || !double.IsRealNumber(lhsNumberValue))
            {
                throw VBRuntimeErrorException.TypeMismatch(opSymbol, "This expression evaluates to a `Double`; LHS `String` value must have a numeric value. Consider explicitly validating and converting the values first.");
            }

            var rhsNumberValue = numeric.NumericValue;
            context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitStringCoercion(rhsValue.Symbol!));

            return new VBDoubleValue { NumericValue = binaryOp.Invoke((int)lhsNumberValue, (int)rhsNumberValue), Symbol = opSymbol };
        }

        if (rhsType is INumericCoercion coercible)
        {
            if (!double.TryParse(lhsString.Value, out var lhsNumberValue))
            {
                throw VBRuntimeErrorException.TypeMismatch(opSymbol, "This expression evaluates to a `Double`; LHS `String` value must have a numeric value. Consider explicitly validating and converting the values first.");
            }

            var rhsNumberValue = coercible.AsCoercedNumeric()?.Value ?? 0;
            context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitStringCoercion(rhsValue.Symbol!));

            return new VBDoubleValue { NumericValue = binaryOp.Invoke(lhsNumberValue, rhsNumberValue), Symbol = opSymbol };
        }
        if (rhsType is VBNullType)
        {
            context = context.WithDiagnostic(RubberduckDiagnostic.UnintendedConstantExpression(opSymbol));
            return VBNullValue.Null;
        }
        if (rhsType is VBEmptyType)
        {
            return lhsString;
        }
        if (rhsType is VBObjectType && context.GetTypedValue(rhsValue.Symbol!) is null)
        {
            throw VBCompileErrorException.InvalidUseOfObject(rhsValue.Symbol!, "Object could not be let-coerced into a `String`.");
        }

        throw VBRuntimeErrorException.TypeMismatch(rhsValue.Symbol!, $"Could not coerce RHS operand ({rhsType.Name}) into a `String`.");
    }

    private static VBTypedValue EvaluateStringCoercedIntegerOp(ref VBExecutionScope context, TypedSymbol opSymbol, VBStringValue lhsString, VBTypedValue rhsValue, Func<int, int, double> binaryOp)
    {
        if (rhsValue is VBStringValue rhsString && opSymbol is VBAdditionOperator)
        {
            context = context.WithDiagnostic(RubberduckDiagnostic.PreferConcatOperatorForStringConcatenation(opSymbol));
            return lhsString.WithValue(lhsString.Value + rhsString.Value);
        }

        var rhsType = rhsValue.TypeInfo;
        if (rhsValue is VBNumericTypedValue numeric) 
        {
            double lhsNumberValue;
            if (lhsString.Value.StartsWith("&H"))
            {
                try
                {
                    lhsNumberValue = Convert.ToInt32(lhsString.Value.Replace("&H", "0x"), 16);
                }
                catch 
                {
                    throw VBRuntimeErrorException.TypeMismatch(opSymbol, "This expression evaluates to a `Double`; LHS `String` value must have a numeric value. Consider explicitly validating and converting the values first.");
                }
            }
            else if (!double.TryParse(lhsString.Value, out lhsNumberValue) || !double.IsRealNumber(lhsNumberValue))
            {
                throw VBRuntimeErrorException.TypeMismatch(opSymbol, "This expression evaluates to a `Double`; LHS `String` value must have a numeric value. Consider explicitly validating and converting the values first.");
            }

            var rhsNumberValue = numeric.NumericValue;
            context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitStringCoercion(rhsValue.Symbol!));

            return new VBDoubleValue { NumericValue = binaryOp.Invoke((int)lhsNumberValue, (int)rhsNumberValue), Symbol = opSymbol };
        }

        if (rhsType is INumericCoercion coercible)
        {
            if (!double.TryParse(lhsString.Value, out var lhsNumberValue))
            {
                throw VBRuntimeErrorException.TypeMismatch(opSymbol, "This expression evaluates to a `Double`; LHS `String` value must have a numeric value. Consider explicitly validating and converting the values first.");
            }

            var rhsNumberValue = coercible.AsCoercedNumeric()?.Value ?? 0;
            context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitStringCoercion(rhsValue.Symbol!));

            return new VBDoubleValue { NumericValue = binaryOp.Invoke((int)lhsNumberValue, (int)rhsNumberValue), Symbol = opSymbol };
        }
        if (rhsType is VBNullType)
        {
            return VBNullValue.Null;
        }
        if (rhsType is VBEmptyType)
        {
            return lhsString;
        }
        if (rhsType is VBObjectType && context.GetTypedValue(rhsValue.Symbol!) is null)
        {
            throw VBCompileErrorException.InvalidUseOfObject(rhsValue.Symbol!, "Object could not be let-coerced into a `String`.");
        }

        throw VBRuntimeErrorException.TypeMismatch(rhsValue.Symbol!, $"Could not coerce RHS operand ({rhsType.Name}) into a `String`.");
    }
    #endregion

    #region TODO refactor
    private static VBTypedValue EvaluateNumericOp(ref VBExecutionScope context, TypedSymbol opSymbol, VBNumericTypedValue lhsNumericValue, VBTypedValue rhsValue, Func<double, double, double> binaryOp)
    {
        var rhsType = rhsValue.TypeInfo!;

        if (rhsType is INumericType)
        {
            var rhsNumericValue = ((VBNumericTypedValue)rhsValue).AsDouble();
            return lhsNumericValue.AsDouble().WithValue(binaryOp.Invoke(lhsNumericValue.AsDouble().Value, rhsNumericValue.Value));
        }

        if (rhsType is VBDateType)
        {
            var rhsDateValue = (VBDateValue)rhsValue;
            context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitDateSerialConversion(rhsValue.Symbol!));

            return new VBDoubleValue(opSymbol).WithValue(binaryOp.Invoke(lhsNumericValue.AsDouble().Value, rhsDateValue.SerialValue));
        }

        if (rhsType is INumericCoercion coercible)
        {
            var rhsCoercedValue = coercible.AsCoercedNumeric();
            context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitNumericCoercion(opSymbol));

            if (lhsNumericValue.Size >= rhsValue.Size)
            {
                return lhsNumericValue.AsDouble().WithValue(binaryOp.Invoke(lhsNumericValue.AsDouble().Value, rhsCoercedValue?.Value ?? 0));
            }
            else
            {
                return ((VBNumericTypedValue)rhsValue).AsDouble().WithValue(binaryOp.Invoke(lhsNumericValue.AsDouble().Value, rhsCoercedValue?.Value ?? 0));
            }
        }

        throw VBRuntimeErrorException.TypeMismatch(opSymbol, $"Could not coerce RHS operand ({rhsType.Name}) into a numeric value.");
    }

    private static VBTypedValue EvaluateIntegerOp(ref VBExecutionScope context, TypedSymbol opSymbol, VBNumericTypedValue lhsNumericValue, VBTypedValue rhsValue, Func<int, int, int> binaryOp)
    {
        var rhsType = rhsValue.TypeInfo!;

        if (rhsValue is VBNumericTypedValue rhsNumericValue)
        {
            if (lhsNumericValue.Size >= rhsNumericValue.Size)
            {
                return lhsNumericValue.WithValue(binaryOp.Invoke((int)lhsNumericValue.AsDouble().Value, (int)rhsNumericValue.AsDouble().Value));
            }
            else
            {
                return rhsNumericValue.WithValue(binaryOp.Invoke((int)lhsNumericValue.AsDouble().Value, (int)rhsNumericValue.AsDouble().Value));
            }
        }

        if (rhsType is VBDateType)
        {
            var rhsDateValue = (VBDateValue)rhsValue;
            context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitDateSerialConversion(rhsValue.Symbol!));

            return rhsDateValue.WithValue(binaryOp.Invoke((int)rhsDateValue.SerialValue, (int)lhsNumericValue.AsDouble().Value));
        }

        if (rhsValue is INumericCoercion coercible)
        {
            var rhsCoercedValue = coercible.AsCoercedNumeric();
            context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitNumericCoercion(opSymbol));

            if (lhsNumericValue.Size >= rhsValue.Size)
            {
                return lhsNumericValue.WithValue(binaryOp.Invoke((int)lhsNumericValue.AsDouble().Value, (int)(rhsCoercedValue?.Value ?? 0)));
            }
            else
            {
                return rhsCoercedValue!.AsDouble().WithValue(binaryOp.Invoke((int)lhsNumericValue.AsDouble().Value, (int)(rhsCoercedValue?.Value ?? 0)));
            }
        }

        throw VBRuntimeErrorException.TypeMismatch(opSymbol, $"Could not coerce RHS operand ({rhsType.Name}) into a numeric value.");
    }
    #endregion
}