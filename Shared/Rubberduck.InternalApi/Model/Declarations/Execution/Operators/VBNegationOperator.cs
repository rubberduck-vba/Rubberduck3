using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBNegationOperator : VBUnaryOperator
{
    public VBNegationOperator(string expression, TypedSymbol operand, Uri parentUri)
        : base(Tokens.NegationOp, expression, parentUri, operand)
    {
    }

    public override VBTypedValue? Evaluate(ExecutionScope context)
    {
        var symbol = (TypedSymbol)Children!.Single();
        if (symbol.ResolvedType is VBStringType)
        {
            throw VBRuntimeErrorException.TypeMismatch;
        }
        if (symbol.ResolvedType is VBNullType)
        {
            return VBNullValue.Null;
        }
        if (symbol.ResolvedType is INumericType)
        {
            var value = ((INumericValue)context.GetTypedValue(symbol)).AsDouble() * -1;

            if (symbol.ResolvedType is VBByteType || symbol.ResolvedType is VBIntegerType)
            {
                // implicit widening OK
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
            throw VBRuntimeErrorException.ObjectDoesntSupportPropertyOrMethod;
        }
        else
        {
            throw VBRuntimeErrorException.TypeMismatch;
        }
    }
}
