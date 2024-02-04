using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public static class NumericSymbolOperation
{
    public static VBTypedValue EvaluateUnaryOpResult(ref VBExecutionScope context, TypedSymbol symbol, Func<double, double> unaryOp)
    {
        if (symbol.ResolvedType is VBNullType)
        {
            context = context.WithDiagnostics([RubberduckDiagnostic.UnintendedConstantExpression(symbol)]);
            return VBNullValue.Null;
        }
        if (symbol.ResolvedType is INumericType)
        {
            var value = unaryOp.Invoke(((INumericValue)context.GetTypedValue(symbol)).AsDouble());

            if (symbol.ResolvedType is VBByteType || symbol.ResolvedType is VBIntegerType)
            {
                context = context.WithDiagnostics([RubberduckDiagnostic.ImplicitWideningConversion(symbol)]);
                return new VBIntegerValue { Value = (short)value };
            }
            if (symbol.ResolvedType is VBLongType || (!context.Is64BitHost && symbol.ResolvedType is VBLongPtrType))
            {
                return new VBLongValue { Value = (int)value };
            }
            if (symbol.ResolvedType is VBLongLongType || (context.Is64BitHost && symbol.ResolvedType is VBLongPtrType))
            {
                return new VBLongLongValue { Value = (long)value };
            }
            if (symbol.ResolvedType is VBCurrencyType)
            {
                return new VBCurrencyValue { Value = (decimal)value };
            }
            if (symbol.ResolvedType is VBDecimalType)
            {
                return new VBDecimalValue { Value = (decimal)value };
            }
            if (symbol.ResolvedType is VBSingleType)
            {
                return new VBSingleValue { Value = (float)value };
            }
            if (symbol.ResolvedType is VBDoubleType)
            {
                return new VBDoubleValue { Value = (double)value };
            }
        }
        if (symbol.ResolvedType is INumericCoercion coercible)
        {
            // implicit numeric coercion, should issue diagnostic
            return new VBIntegerValue { Value = (short)coercible.AsCoercedNumeric()! };
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

    public static VBTypedValue EvaluateBinaryOpResult(ref VBExecutionScope context, TypedSymbol opSymbol, VBTypedValue lhsValue, VBTypedValue rhsValue, Func<double, double, double> binaryOp)
    {
        var lhsType = lhsValue.TypeInfo!;
        var rhsType = rhsValue.TypeInfo!;

        if (lhsValue is VBStringValue lhsString)
        {
            if (rhsValue is VBStringValue rhsString)
            {
                context = context.WithDiagnostics([RubberduckDiagnostic.PreferConcatOperatorForStringConcatenation(opSymbol)]);
                return lhsString.WithValue(lhsString.Value + rhsString.Value);
            }
            if (rhsType is INumericCoercion coercible)
            {
                if (!double.TryParse(lhsString.Value, out var lhsNumberValue))
                {
                    throw VBRuntimeErrorException.TypeMismatch(opSymbol, "This expression evaluates to a `Double`; LHS `String` value must have a numeric value.");
                }

                var rhsNumberValue = coercible.AsCoercedNumeric() ?? 0;
                context = context.WithDiagnostics([RubberduckDiagnostic.ImplicitStringCoercion(rhsValue.Symbol!)]);

                return new VBDoubleValue { Value = binaryOp.Invoke(lhsNumberValue, rhsNumberValue), Symbol = opSymbol };
            }
            if (rhsType is VBNullType)
            {
                return VBNullValue.Null;
            }
            if (rhsType is VBEmptyType)
            {
                return lhsValue;
            }
            if (rhsType is VBObjectType && context.GetTypedValue(rhsValue.Symbol!) is null)
            {
                throw VBCompileErrorException.InvalidUseOfObject(rhsValue.Symbol!, "Object could not be let-coerced into a `String`. Is a member call missing?");
            }

            throw VBRuntimeErrorException.TypeMismatch(rhsValue.Symbol!, "Could not coerce RHS operand into a `String`.");
        }

        if (lhsType is INumericType)
        {
            var lhsNumericValue = (INumericValue)lhsValue;
            if (rhsType is INumericType)
            {
                var rhsNumericValue = (INumericValue)rhsValue;
                return lhsNumericValue.WithValue(binaryOp.Invoke(lhsNumericValue.AsDouble(), rhsNumericValue.AsDouble()));
            }
            if (rhsType is VBDateType)
            {
                var rhsDateValue = (VBDateValue)rhsValue;
                context = context.WithDiagnostics([RubberduckDiagnostic.ImplicitDateSerialConversion(rhsValue.Symbol!)]);

                return rhsDateValue.WithValue(binaryOp.Invoke(rhsDateValue.SerialValue, lhsNumericValue.AsDouble()));
            }
            if (rhsType is INumericCoercion coercible)
            {
                var rhsCoercedValue = coercible.AsCoercedNumeric();
                context = context.WithDiagnostics([RubberduckDiagnostic.ImplicitNumericCoercion(opSymbol)]);

                return lhsNumericValue.WithValue(binaryOp.Invoke(lhsNumericValue.AsDouble(), rhsCoercedValue ?? 0));
            }
        }

        throw VBRuntimeErrorException.TypeMismatch(opSymbol, "The data types involved in this binary operation are not compatible.");
    }
}