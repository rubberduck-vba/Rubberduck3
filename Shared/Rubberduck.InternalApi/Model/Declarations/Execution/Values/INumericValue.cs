using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public interface INumericValue 
{
    VBBooleanValue AsBoolean();
    VBByteValue AsByte();
    VBIntegerValue AsInteger();
    VBLongValue AsLong();
    VBLongLongValue AsLongLong();
    VBSingleValue AsSingle();
    VBDoubleValue AsDouble();
    VBCurrencyValue AsCurrency();
    VBDecimalValue AsDecimal();
}

public interface INumericValue<VBTValue> : INumericValue
    where VBTValue : VBTypedValue
{
    VBType TypeInfo { get; }

    VBTValue MinValue { get; }
    VBTValue MaxValue { get; }
    VBTValue Zero { get; }
}

public interface INumericCoercion
{
    VBDoubleValue? AsCoercedNumeric(int depth = 0);
}

public interface IStringCoercion
{
    VBStringValue? AsCoercedString(int depth = 0);
}
