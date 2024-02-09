﻿using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public static class SymbolOperation
{
    public static VBTypedValue EvaluateUnaryOpResult(ref VBExecutionScope context, TypedSymbol symbol, Func<double, double> unaryOp)
    {
        if (symbol.ResolvedType is VBNullType)
        {
            context = context.WithDiagnostic(RubberduckDiagnostic.UnintendedConstantExpression(symbol));
            return new VBNullValue(symbol);
        }

        if (symbol.ResolvedType is INumericType)
        {
            var value = unaryOp.Invoke(((INumericValue)context.GetTypedValue(symbol)).AsDouble().Value);

            if (symbol.ResolvedType is VBByteType || symbol.ResolvedType is VBIntegerType)
            {
                context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitWideningConversion(symbol));
                return new VBIntegerValue(symbol) { Value = (short)value };
            }
            if (symbol.ResolvedType is VBLongType || (!context.Is64BitHost && symbol.ResolvedType is VBLongPtrType))
            {
                return new VBLongValue(symbol) { Value = (int)value };
            }
            if (symbol.ResolvedType is VBLongLongType || (context.Is64BitHost && symbol.ResolvedType is VBLongPtrType))
            {
                return new VBLongLongValue(symbol) { Value = (long)value };
            }
            if (symbol.ResolvedType is VBCurrencyType)
            {
                return new VBCurrencyValue(symbol) { Value = (decimal)value };
            }
            if (symbol.ResolvedType is VBDecimalType)
            {
                return new VBDecimalValue(symbol) { Value = (decimal)value };
            }
            if (symbol.ResolvedType is VBSingleType)
            {
                return new VBSingleValue(symbol) { Value = (float)value };
            }
            if (symbol.ResolvedType is VBDoubleType)
            {
                return new VBDoubleValue(symbol) { Value = (double)value };
            }

            throw VBRuntimeErrorException.TypeMismatch(symbol);
        }

        if (symbol.ResolvedType is INumericCoercion coercible)
        {
            context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitNumericCoercion(symbol));
            return new VBIntegerValue(symbol) { Value = (short)coercible.AsCoercedNumeric()!.Value };
        }

        if (symbol.ResolvedType is VBObjectType)
        {
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
                if (rhsValue.TypeInfo != VBType.VbStringType)
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
        var rhsType = rhsValue.TypeInfo!;

        if (lhsValue is VBStringValue lhsString)
        {
            return EvaluateStringCoercedNumericOp(ref context, opSymbol, lhsString, rhsValue, binaryOp);
        }

        if (lhsType is INumericType)
        {
            var lhsNumericValue = (VBNumericTypedValue)lhsValue;
            return EvaluateNumericOp(ref context, opSymbol, lhsNumericValue, rhsValue, binaryOp);
        }

        throw VBRuntimeErrorException.TypeMismatch(opSymbol, "The data types involved in this binary operation are not compatible.");
    }

    public static VBTypedValue EvaluateBinaryOpResult(ref VBExecutionScope context, TypedSymbol opSymbol, VBTypedValue lhsValue, VBTypedValue rhsValue, Func<int, int, int> binaryOp)
    {
        var lhsType = lhsValue.TypeInfo!;
        var rhsType = rhsValue.TypeInfo!;

        if (lhsValue is VBStringValue lhsString)
        {
            return EvaluateStringCoercedIntegerOp(ref context, opSymbol, lhsString, rhsValue, binaryOp);
        }

        if (lhsType is INumericType)
        {
            var lhsNumericValue = (VBNumericTypedValue)lhsValue;
            return EvaluateIntegerOp(ref context, opSymbol, lhsNumericValue, rhsValue, binaryOp);
        }

        throw VBRuntimeErrorException.TypeMismatch(opSymbol, "The data types involved in this binary operation are not compatible.");
    }

    #region TODO refactor
    private static VBTypedValue EvaluateStringCoercedNumericOp(ref VBExecutionScope context, TypedSymbol opSymbol, VBStringValue lhsString, VBTypedValue rhsValue, Func<double, double, double> binaryOp)
    {
        if (rhsValue is VBStringValue rhsString)
        {
            context = context.WithDiagnostic(RubberduckDiagnostic.PreferConcatOperatorForStringConcatenation(opSymbol));
            return lhsString.WithValue(lhsString.Value + rhsString.Value);
        }

        var rhsType = rhsValue.TypeInfo;
        if (rhsType is INumericCoercion coercible)
        {
            if (!double.TryParse(lhsString.Value, out var lhsNumberValue))
            {
                throw VBRuntimeErrorException.TypeMismatch(opSymbol, "This expression evaluates to a `Double`; LHS `String` value must have a numeric value. Consider explicitly validating and converting the values first.");
            }

            var rhsNumberValue = coercible.AsCoercedNumeric()?.Value ?? 0;
            context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitStringCoercion(rhsValue.Symbol!));

            return new VBDoubleValue { Value = binaryOp.Invoke(lhsNumberValue, rhsNumberValue), Symbol = opSymbol };
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

    private static VBTypedValue EvaluateStringCoercedIntegerOp(ref VBExecutionScope context, TypedSymbol opSymbol, VBStringValue lhsString, VBTypedValue rhsValue, Func<int, int, int> binaryOp)
    {
        if (rhsValue is VBStringValue rhsString)
        {
            context = context.WithDiagnostic(RubberduckDiagnostic.PreferConcatOperatorForStringConcatenation(opSymbol));
            return lhsString.WithValue(lhsString.Value + rhsString.Value);
        }

        var rhsType = rhsValue.TypeInfo;
        if (rhsType is INumericCoercion coercible)
        {
            if (!double.TryParse(lhsString.Value, out var lhsNumberValue))
            {
                throw VBRuntimeErrorException.TypeMismatch(opSymbol, "This expression evaluates to a `Double`; LHS `String` value must have a numeric value. Consider explicitly validating and converting the values first.");
            }

            var rhsNumberValue = coercible.AsCoercedNumeric()?.Value ?? 0;
            context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitStringCoercion(rhsValue.Symbol!));

            return new VBLongValue { Value = binaryOp.Invoke((int)lhsNumberValue, (int)rhsNumberValue), Symbol = opSymbol };
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
            return lhsNumericValue.WithValue(binaryOp.Invoke(lhsNumericValue.AsDouble().Value, rhsNumericValue.Value));
        }

        if (rhsType is VBDateType)
        {
            var rhsDateValue = (VBDateValue)rhsValue;
            context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitDateSerialConversion(rhsValue.Symbol!));

            return rhsDateValue.WithValue(binaryOp.Invoke(rhsDateValue.SerialValue, lhsNumericValue.AsDouble().Value));
        }

        if (rhsType is INumericCoercion coercible)
        {
            var rhsCoercedValue = coercible.AsCoercedNumeric();
            context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitNumericCoercion(opSymbol));

            if (lhsNumericValue.Size >= rhsValue.Size)
            {
                return lhsNumericValue.WithValue(binaryOp.Invoke(lhsNumericValue.AsDouble().Value, rhsCoercedValue?.Value ?? 0));
            }
            else
            {
                return ((VBNumericTypedValue)rhsValue).WithValue(binaryOp.Invoke(lhsNumericValue.AsDouble().Value, rhsCoercedValue?.Value ?? 0));
            }
        }

        throw VBRuntimeErrorException.TypeMismatch(opSymbol, $"Could not coerce RHS operand ({rhsType.Name}) into a numeric value.");
    }

    private static VBTypedValue EvaluateIntegerOp(ref VBExecutionScope context, TypedSymbol opSymbol, VBNumericTypedValue lhsNumericValue, VBTypedValue rhsValue, Func<int, int, int> binaryOp)
    {
        var rhsType = rhsValue.TypeInfo!;

        if (rhsType is INumericType)
        {
            var rhsNumericValue = (INumericValue)rhsValue;
            return lhsNumericValue.WithValue(binaryOp.Invoke((int)lhsNumericValue.AsDouble().Value, (int)rhsNumericValue.AsDouble().Value));
        }

        if (rhsType is VBDateType)
        {
            var rhsDateValue = (VBDateValue)rhsValue;
            context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitDateSerialConversion(rhsValue.Symbol!));

            return rhsDateValue.WithValue(binaryOp.Invoke((int)rhsDateValue.SerialValue, (int)lhsNumericValue.AsDouble().Value));
        }

        if (rhsType is INumericCoercion coercible)
        {
            var rhsCoercedValue = coercible.AsCoercedNumeric();
            context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitNumericCoercion(opSymbol));

            if (lhsNumericValue.Size >= rhsValue.Size)
            {
                return lhsNumericValue.WithValue(binaryOp.Invoke((int)lhsNumericValue.AsDouble().Value, (int)(rhsCoercedValue?.Value ?? 0)));
            }
            else
            {
                return ((VBNumericTypedValue)rhsValue).WithValue(binaryOp.Invoke((int)lhsNumericValue.AsDouble().Value, (int)(rhsCoercedValue?.Value ?? 0)));
            }
        }

        throw VBRuntimeErrorException.TypeMismatch(opSymbol, $"Could not coerce RHS operand ({rhsType.Name}) into a numeric value.");
    }
    #endregion
}