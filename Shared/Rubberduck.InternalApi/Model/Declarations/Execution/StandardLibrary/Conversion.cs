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
        public static VBBooleanValue CBool(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value) =>
            CheckNullError(value, e => new VBBooleanValue(symbol).WithValue(e.Value != 0), out var nop)
                ? nop : SymbolOperation.EvaluateCompareOpResult(ref context, symbol, VBIntegerValue.Zero, value, (lhs, rhs) => lhs != rhs);

        public static VBByteValue CByte(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value) =>
            CheckNullError(value, e => new VBByteValue(symbol).WithValue(e.Value ?? 0), out var nop)
                ? nop : (VBByteValue)new VBByteValue(symbol).WithValue(GetNumericValueOrThrow(ref context, value));

        public static VBCurrencyValue CCur(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value) =>
            CheckNullError(value, e => (VBCurrencyValue)new VBCurrencyValue(symbol).WithValue(e.Value ?? 0), out var nop)
                ? nop : (VBCurrencyValue)new VBCurrencyValue(symbol).WithValue(GetNumericValueOrThrow(ref context, value));

        public static VBDateValue CDate(ref VBExecutionScope context, VBTypedValue value)
        {
            if (CheckNullError(value, null as Func<VBTypedValue, VBDateValue>, out var nop))
            {
                // already a date
                return nop;
            }

            if (TryConvertNumericValue(ref context, value, out var numeric))
            {
                // from serial
                return (VBDateValue)new VBDateValue(value.Symbol!).WithValue(numeric);
            }

            // this is probably excluding a bunch of weird valid date literals
            if (DateTime.TryParse(GetStringValueOrThrow(value), out var dtValue))
            {
                return new VBDateValue(value.Symbol!).WithValue(dtValue);
            }

            throw VBRuntimeErrorException.TypeMismatch(value.Symbol!, $"Type `{value.TypeInfo.Name}` cannot be converted directly to a `Date`.");
        }

        public static VBDoubleValue CDbl(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value) =>
            CheckNullError(value, e => (VBDoubleValue)new VBDoubleValue(symbol).WithValue(e.Value ?? 0), out var nop)
                ? nop : (VBDoubleValue)new VBDoubleValue(symbol).WithValue(GetNumericValueOrThrow(ref context, value));

        public static VBDecimalValue CDec(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value) =>
            CheckNullError(value, e => new VBDecimalValue(symbol).WithValue((decimal)(e.Value ?? 0)), out var nop)
                ? nop : (VBDecimalValue)new VBDecimalValue(symbol).WithValue(GetNumericValueOrThrow(ref context, value));

        public static VBIntegerValue CInt(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value) =>
            CheckNullError(value, e => (VBIntegerValue)new VBIntegerValue(symbol).WithValue(e.Value ?? 0), out var nop)
                ? nop : (VBIntegerValue)new VBIntegerValue(symbol).WithValue(GetNumericValueOrThrow(ref context, value));

        public static VBLongValue CLng(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value) =>
            CheckNullError(value, e => (VBLongValue)new VBLongValue(symbol).WithValue(e.Value ?? 0), out var nop)
                ? nop : (VBLongValue)new VBLongValue(symbol).WithValue(GetNumericValueOrThrow(ref context, value));

        public static VBLongLongValue CLngLng(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value) =>
            CheckNullError(value, e => (VBLongLongValue)new VBLongLongValue(symbol).WithValue(e.Value ?? 0), out var nop)
                ? nop : (VBLongLongValue) new VBLongLongValue(symbol).WithValue(GetNumericValueOrThrow(ref context, value));

        public static VBLongPtrValue CLngPtr(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value)
        {
            VBType ptrSize = context.Is64BitHost ? VBLongLongType.TypeInfo : VBLongType.TypeInfo;
            return CheckNullError(value, e => (VBLongPtrValue)new VBLongPtrValue(symbol).WithValue(e.Value ?? 0, ptrSize), out var nop)
                ? nop : (VBLongPtrValue)new VBLongPtrValue(symbol).WithValue(GetNumericValueOrThrow(ref context, value), ptrSize);
        }

        public static VBSingleValue CSng(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value) =>
            CheckNullError(value, e => (VBSingleValue)new VBSingleValue(symbol).WithValue(e.Value ?? 0), out var nop)
                ? nop : (VBSingleValue)new VBSingleValue(symbol).WithValue(GetNumericValueOrThrow(ref context, value));

        public static VBStringValue CStr(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value) =>
            CheckNullError(value, null as Func<VBTypedValue, VBStringValue>, out var nop)
                ? nop : (VBStringValue)new VBStringValue(symbol).WithValue(GetStringValueOrThrow(value));

        public static VBVariantValue CVar(VBTypedValue value) => value is VBVariantValue nop ? nop : new VBVariantValue(value, value.Symbol!);
        public static VBVariantValue CVDate(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value) => new(CDate(ref context, value), symbol!);


        public static VBVariantValue Error(ref VBExecutionScope context, TypedSymbol symbol, TypedSymbol? lhs, VBTypedValue value)
        {
            var number = (int)GetNumericValueOrThrow(ref context, value);
            var message = VBRuntimeErrorException.GetErrorString(number);

            if (lhs is null) // function is being used as a statement
            {
                context = context.WithDiagnostic(RubberduckDiagnostic.PreferErrRaiseOverErrorStatement(symbol));
                throw new VBRuntimeErrorException(symbol, number, message);
            }

            return new VBVariantValue(new VBStringValue(symbol).WithValue(message), symbol);
        }

        public static VBStringValue ErrorS(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value) =>
            new VBStringValue(symbol).WithValue(VBRuntimeErrorException.GetErrorString((int)GetNumericValueOrThrow(ref context, value)));

        public static VBStringValue HexS(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value) =>
            ConvertNumericString(ref context, symbol, value, n => n.ToString("X"));
        public static VBVariantValue CVErr(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value) =>
            Convert(ref context, symbol, value, n => new VBErrorValue(symbol, (int)n));
        public static VBVariantValue Fix(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value) =>
            Convert(ref context, symbol, value, n => Math.Sign(n) * Math.Truncate(Math.Abs(n)));
        public static VBVariantValue Hex(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value) =>
            Convert(ref context, symbol, value, n => n.ToString("X"));
        public static VBVariantValue Int(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value) =>
            Convert(ref context, symbol, value, n => Math.Truncate(n));
        public static VBVariantValue Oct(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value) =>
            Convert(ref context, symbol, value, n => System.Convert.ToString((long)n, toBase: 8));

        private static VBVariantValue Convert<T>(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value, Func<double, T> op) =>
            new VBVariantValue(value, symbol) { Value = op(GetNumericValueOrThrow(ref context, value)) };

        private static VBStringValue ConvertNumericString(ref VBExecutionScope context, TypedSymbol symbol, VBTypedValue value, Func<double, string> op) =>
            new VBStringValue(symbol).WithValue(op(GetNumericValueOrThrow(ref context, value)));

        /// <summary>
        /// Throws a <c>VBRuntimeErrorException.TypeMismatch</c> if it isn't a string, or can't be coerced into one.
        /// </summary>
        /// <exception cref="VBRuntimeErrorException" />
        private static string GetStringValueOrThrow(VBTypedValue value) => value is IStringCoercion coercible
            ? coercible.AsCoercedString() // NOTE: diagnostics here would be noisy.
            : throw VBRuntimeErrorException.TypeMismatch(value.Symbol!);

        /// <summary>
        /// Throws a <c>VBRuntimeErrorException.TypeMismatch</c> if it isn't a number, or can't be coerced into one.
        /// </summary>
        /// <exception cref="VBRuntimeErrorException" />
        private static double GetNumericValueOrThrow(ref VBExecutionScope context, VBTypedValue value) => 
            TryConvertNumericValue(ref context, value, out var numericResult)
                ? numericResult
                : throw VBRuntimeErrorException.TypeMismatch(value.Symbol!);

        private static bool TryConvertNumericValue(ref VBExecutionScope context, VBTypedValue value, out double numericResult)
        {
            if (value is INumericValue numeric)
            {
                numericResult = numeric.AsDouble();
                return true;
            }

            if (value is INumericCoercion coercible)
            {
                var coerced = coercible.AsCoercedNumeric() ?? 0;
                context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitNumericCoercion(value.Symbol!));

                numericResult = coerced;
                return true;
            }

            numericResult = double.NaN;
            return false;
        }

        /// <summary>
        /// Validates the value against <c>VBNullValue</c> and <c>VBErrorValue</c>.
        /// </summary>
        /// <remarks>
        /// <c>VBRuntimeErrorException.TypeMismatch</c> is thrown when the <c>value</c> is a <c>VBErrorValue</c> and cannot be converted;  
        /// <c>VBRuntimeErrorException.InvalidUseOfNull</c> is thrown whenever the <c>value</c> is a <c>VBNullValue</c>.
        /// </remarks>
        /// <typeparam name="T">The target VBType</typeparam>
        /// <param name="value">The typed value to validate</param>
        /// <param name="convertErrorValue">A function to convert <c>VBErrorValue</c> values. If <c>null</c>, a <c>VBRuntimeException.TypeMismatch</c> is added to the execution context.</param>
        /// <param name="typedResult">The typed result, if successfully converted from an <c>VBErrorValue</c> or if the value was already of the correct type.</param>
        /// <returns>
        /// <c>true</c> if a typed result was successfully converted (or if the result is excatly the value that was given), <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="VBRuntimeErrorException" />
        private static bool CheckNullError<T>(VBTypedValue value, Func<VBErrorValue, T>? convertErrorValue, out T typedResult) where T : VBTypedValue
        {
            if (value is T nop)
            {
                typedResult = nop;
                return true;
            }

            if (value is VBErrorValue error)
            {
                if (convertErrorValue is null)
                {
                    throw VBRuntimeErrorException.TypeMismatch(value.Symbol!, $"Type `{value.TypeInfo.Name}` cannot be converted to {typeof(T).Name}.");
                }

                typedResult = convertErrorValue(error);
                return true;
            }

            if (value is VBNullValue)
            {
                throw VBRuntimeErrorException.InvalidUseOfNull(value.Symbol!, "There is no possible type conversion from `Null`.");
            }

            typedResult = null!;
            return false;
        }
    }
}
