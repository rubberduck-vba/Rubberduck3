using Newtonsoft.Json.Linq;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.StandardLibrary
{
    public static class Conversion
    {
        public static VBBooleanValue CBool(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value)
        {
            if (value is VBBooleanValue nop)
            {
                return nop;
            }

            if (value is VBNullValue)
            {
                throw VBRuntimeErrorException.InvalidUseOfNull(value.Symbol!, "There is no possible type conversion from `Null`.");
            }

            if (value is VBErrorValue error)
            {
                return new VBBooleanValue(symbol).WithValue(error.Value != 0);
            }

            return SymbolOperation.EvaluateCompareOpResult(ref context, symbol, VBIntegerValue.Zero, value, (lhs, rhs) => lhs != rhs);
        }

        public static VBByteValue CByte(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value)
        {
            if (value is VBByteValue nop)
            {
                return nop;
            }

            if (value is VBNullValue)
            {
                throw VBRuntimeErrorException.InvalidUseOfNull(value.Symbol!, "There is no possible type conversion from `Null`.");
            }

            if (value is VBErrorValue error)
            {
                return new VBByteValue(symbol).WithValue(error.Value ?? 0);
            }

            if (value is INumericValue numeric)
            {
                return (VBByteValue)new VBByteValue(symbol).WithValue(numeric.AsDouble());
            }

            if (value is INumericCoercion coercible)
            {
                var coerced = coercible.AsCoercedNumeric();
                context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitNumericCoercion(value.Symbol!));

                return (VBByteValue)new VBByteValue(symbol).WithValue(coerced ?? 0);
            }

            throw VBRuntimeErrorException.TypeMismatch(value.Symbol!);
        }

        public static VBCurrencyValue CCur(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value)
        {
            if (value is VBCurrencyValue nop)
            {
                return nop;
            }

            if (value is VBNullValue)
            {
                throw VBRuntimeErrorException.InvalidUseOfNull(value.Symbol!, "There is no possible type conversion from `Null`.");
            }

            if (value is VBErrorValue error)
            {
                return (VBCurrencyValue)new VBCurrencyValue(symbol).WithValue(error.Value ?? 0);
            }

            if (value is INumericValue numeric)
            {
                return (VBCurrencyValue)new VBCurrencyValue(symbol).WithValue(numeric.AsDouble());
            }

            if (value is INumericCoercion coercible)
            {
                var coerced = coercible.AsCoercedNumeric();
                context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitNumericCoercion(value.Symbol!));

                return (VBCurrencyValue)new VBCurrencyValue(symbol).WithValue(coerced ?? 0);
            }

            throw VBRuntimeErrorException.TypeMismatch(value.Symbol!);
        }

        public static VBDateValue CDate(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value)
        {
            if (value is VBDateValue nop)
            {
                return nop;
            }

            if (value is VBNullValue)
            {
                throw VBRuntimeErrorException.InvalidUseOfNull(value.Symbol!, "There is no possible type conversion from `Null`.");
            }

            if (value is INumericValue numeric)
            {
                return (VBDateValue)new VBDateValue(symbol).WithValue(numeric.AsDouble());
            }

            if (value is INumericCoercion coercible)
            {
                var coerced = coercible.AsCoercedNumeric();
                context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitNumericCoercion(value.Symbol!));

                return (VBDateValue)new VBDateValue(symbol).WithValue(coerced ?? 0);
            }

            if (value is VBStringValue stringValue)
            {
                // this is probably excluding a bunch of weird valid date literals
                if (DateTime.TryParse(stringValue.Value, out var dtValue))
                {
                    return new VBDateValue(symbol).WithValue(dtValue);
                }
            }

            throw VBRuntimeErrorException.TypeMismatch(value.Symbol!, $"Type `{value.TypeInfo.Name}` cannot be converted directly to a `Date`.");
        }

        public static VBDoubleValue CDbl(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value)
        {
            if (value is VBDoubleValue nop)
            {
                return nop;
            }

            if (value is VBNullValue)
            {
                throw VBRuntimeErrorException.InvalidUseOfNull(value.Symbol!, "There is no possible type conversion from `Null`.");
            }

            if (value is VBErrorValue error)
            {
                return (VBDoubleValue)new VBDoubleValue(symbol).WithValue(error.Value ?? 0);
            }

            if (value is INumericValue numeric)
            {
                return (VBDoubleValue)new VBDoubleValue(symbol).WithValue(numeric.AsDouble());
            }

            if (value is INumericCoercion coercible)
            {
                var coerced = coercible.AsCoercedNumeric();
                context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitNumericCoercion(value.Symbol!));

                return (VBDoubleValue)new VBDoubleValue(symbol).WithValue(coerced ?? 0);
            }

            throw VBRuntimeErrorException.TypeMismatch(value.Symbol!);
        }

        public static VBDecimalValue CDec(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value)
        {
            if (value is VBDecimalValue nop)
            {
                return nop;
            }

            if (value is VBNullValue)
            {
                throw VBRuntimeErrorException.InvalidUseOfNull(value.Symbol!, "There is no possible type conversion from `Null`.");
            }

            if (value is VBErrorValue error)
            {
                return new VBDecimalValue(symbol).WithValue((decimal)(error.Value ?? 0));
            }

            if (value is INumericValue numeric)
            {
                return new VBDecimalValue(symbol).WithValue((decimal)numeric.AsDouble());
            }

            if (value is INumericCoercion coercible)
            {
                var coerced = coercible.AsCoercedNumeric()! ?? 0;
                context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitNumericCoercion(value.Symbol!));

                return new VBDecimalValue(symbol).WithValue((decimal)coerced);
            }

            throw VBRuntimeErrorException.TypeMismatch(value.Symbol!);
        }

        public static VBIntegerValue CInt(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value)
        {
            if (value is VBIntegerValue nop)
            {
                return nop;
            }

            if (value is VBNullValue)
            {
                throw VBRuntimeErrorException.InvalidUseOfNull(value.Symbol!, "There is no possible type conversion from `Null`.");
            }

            if (value is VBErrorValue error)
            {
                return (VBIntegerValue)new VBIntegerValue(symbol).WithValue((error.Value ?? 0));
            }

            if (value is INumericValue numeric)
            {
                return (VBIntegerValue)new VBIntegerValue(symbol).WithValue(numeric.AsDouble());
            }

            if (value is INumericCoercion coercible)
            {
                var coerced = coercible.AsCoercedNumeric()! ?? 0;
                context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitNumericCoercion(value.Symbol!));

                return (VBIntegerValue)new VBIntegerValue(symbol).WithValue(coerced);
            }

            throw VBRuntimeErrorException.TypeMismatch(value.Symbol!);
        }

        public static VBLongValue CLng(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value)
        {
            if (value is VBLongValue nop)
            {
                return nop;
            }

            if (value is VBNullValue)
            {
                throw VBRuntimeErrorException.InvalidUseOfNull(value.Symbol!, "There is no possible type conversion from `Null`.");
            }

            if (value is VBErrorValue error)
            {
                return (VBLongValue)new VBLongValue(symbol).WithValue((error.Value ?? 0));
            }

            if (value is INumericValue numeric)
            {
                return (VBLongValue)new VBLongValue(symbol).WithValue(numeric.AsDouble());
            }

            if (value is INumericCoercion coercible)
            {
                var coerced = coercible.AsCoercedNumeric()! ?? 0;
                context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitNumericCoercion(value.Symbol!));

                return (VBLongValue)new VBLongValue(symbol).WithValue(coerced);
            }

            throw VBRuntimeErrorException.TypeMismatch(value.Symbol!);
        }

        public static VBLongLongValue CLngLng(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value)
        {
            if (value is VBLongLongValue nop)
            {
                return nop;
            }

            if (value is VBNullValue)
            {
                throw VBRuntimeErrorException.InvalidUseOfNull(value.Symbol!, "There is no possible type conversion from `Null`.");
            }

            if (value is VBErrorValue error)
            {
                return (VBLongLongValue)new VBLongLongValue(symbol).WithValue((error.Value ?? 0));
            }

            if (value is INumericValue numeric)
            {
                return (VBLongLongValue)new VBLongLongValue(symbol).WithValue(numeric.AsDouble());
            }

            if (value is INumericCoercion coercible)
            {
                var coerced = coercible.AsCoercedNumeric()! ?? 0;
                context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitNumericCoercion(value.Symbol!));

                return (VBLongLongValue)new VBLongLongValue(symbol).WithValue(coerced);
            }

            throw VBRuntimeErrorException.TypeMismatch(value.Symbol!);
        }

        public static VBLongPtrValue CLngPtr(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value)
        {
            VBType ptrSize = context.Is64BitHost
                ? VBLongLongType.TypeInfo
                : VBLongType.TypeInfo;

            if (value is VBLongPtrValue nop)
            {
                return nop;
            }

            if (value is VBNullValue)
            {
                throw VBRuntimeErrorException.InvalidUseOfNull(value.Symbol!, "There is no possible type conversion from `Null`.");
            }

            if (value is VBErrorValue error)
            {
                return (VBLongPtrValue)new VBLongPtrValue(symbol).WithValue((error.Value ?? 0), ptrSize);
            }

            if (value is INumericValue numeric)
            {
                return (VBLongPtrValue)new VBLongPtrValue(symbol).WithValue(numeric.AsDouble(), ptrSize);
            }

            if (value is INumericCoercion coercible)
            {
                var coerced = coercible.AsCoercedNumeric()! ?? 0;
                context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitNumericCoercion(value.Symbol!));

                return (VBLongPtrValue)new VBLongPtrValue(symbol).WithValue(coerced, ptrSize);
            }

            throw VBRuntimeErrorException.TypeMismatch(value.Symbol!);
        }

        public static VBSingleValue CSng(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value)
        {
            if (value is VBSingleValue nop)
            {
                return nop;
            }

            if (value is VBNullValue)
            {
                throw VBRuntimeErrorException.InvalidUseOfNull(value.Symbol!, "There is no possible type conversion from `Null`.");
            }

            if (value is VBErrorValue error)
            {
                return (VBSingleValue)new VBSingleValue(symbol).WithValue(error.Value ?? 0);
            }

            if (value is INumericValue numeric)
            {
                return (VBSingleValue)new VBSingleValue(symbol).WithValue(numeric.AsDouble());
            }

            if (value is INumericCoercion coercible)
            {
                var coerced = coercible.AsCoercedNumeric();
                context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitNumericCoercion(value.Symbol!));

                return (VBSingleValue)new VBSingleValue(symbol).WithValue(coerced ?? 0);
            }

            throw VBRuntimeErrorException.TypeMismatch(value.Symbol!);
        }

        public static VBStringValue CStr(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value)
        {
            if (value is VBStringValue nop)
            {
                return nop;
            }

            if (value is VBNullValue)
            {
                throw VBRuntimeErrorException.InvalidUseOfNull(value.Symbol!, "There is no possible type conversion from `Null`.");
            }

            if (value is IStringCoercion coercible)
            {
                var coerced = coercible.AsCoercedString();
                // NOTE: diagnostics here would be noisy
                return (VBStringValue)new VBStringValue(symbol).WithValue(coerced);
            }

            throw VBRuntimeErrorException.TypeMismatch(value.Symbol!);
        }

        public static VBVariantValue CVar(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value)
        {
            if (value is VBVariantValue nop)
            {
                return nop;
            }

            return new VBVariantValue(value.TypeInfo, value.Symbol!);
        }

        public static VBVariantValue CVDate(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value)
        {
            var dtValue = CDate(ref context, symbol, value);
            return new VBVariantValue(dtValue.TypeInfo, dtValue.Symbol!);
        }

        public static VBErrorValue CVErr(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value)
        {
            if (value is INumericValue numeric)
            {
                return new VBErrorValue(symbol) { Value = (int)numeric.AsDouble() };
            }

            if (value is INumericCoercion coercible)
            {
                var coerced = coercible.AsCoercedNumeric() ?? 0;
                return new VBErrorValue(symbol) { Value = (int)coerced };
            }

            throw VBRuntimeErrorException.TypeMismatch(value.Symbol!);
        }

        public static VBVariantValue Error(ref VBExecutionScope context, TypedSymbol symbol, TypedSymbol? target, VBLongValue value)
        {
            var message = VBRuntimeErrorException.GetErrorString(value.Value);

            if (target is null)
            {
                context = context.WithDiagnostic(RubberduckDiagnostic.PreferErrRaiseOverErrorStatement(symbol));
                throw new VBRuntimeErrorException(symbol, value.Value, message);
            }

            return new VBVariantValue(VBStringType.TypeInfo, symbol) { Value = message };
        }

        public static VBStringValue ErrorS(ref VBExecutionScope context, TypedSymbol symbol, TypedSymbol? target, VBLongValue value) =>
            new VBStringValue(symbol).WithValue(VBRuntimeErrorException.GetErrorString(value.Value));
    }
}
