using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public abstract record class VBNumericTypedValue : VBTypedValue, 
    INumericValue, 
    INumericCoercion, 
    IStringCoercion, 
    IComparable<VBNumericTypedValue>,
    IEquatable<VBNumericTypedValue>
{
    protected VBNumericTypedValue(VBType type, TypedSymbol? symbol = null)
        : base(type, symbol) { }

    public abstract double NumericValue { get; init; }

    public VBDoubleValue AsCoercedNumeric(int depth = 0) => AsDouble();
    public VBStringValue AsCoercedString(int depth = 0) => new VBStringValue(Symbol).WithValue(ToString());

    public VBBooleanValue AsBoolean() => new VBBooleanValue(Symbol).WithValue(NumericValue != 0);
    public VBByteValue AsByte() => new VBByteValue(Symbol).WithValue(NumericValue);
    public VBCurrencyValue AsCurrency() => new VBCurrencyValue(Symbol).WithValue(NumericValue);
    public VBDecimalValue AsDecimal() => new VBDecimalValue(Symbol).WithValue(NumericValue);
    public VBDoubleValue AsDouble() => new VBDoubleValue(Symbol).WithValue(NumericValue);
    public VBIntegerValue AsInteger() => new VBIntegerValue(Symbol).WithValue(NumericValue);
    public VBLongValue AsLong() => new VBLongValue(Symbol).WithValue(NumericValue);
    public VBLongLongValue AsLongLong() => new VBLongLongValue(Symbol).WithValue(NumericValue);
    public VBSingleValue AsSingle() => new VBSingleValue(Symbol).WithValue(NumericValue);

    public int CompareTo(VBNumericTypedValue? other) => other is null ? 1 : NumericValue.CompareTo(other.NumericValue);
    public override string ToString() => NumericValue.ToString();

    internal VBTypedValue WithValue(int value) => this with { NumericValue = value };

    public override int GetHashCode() => NumericValue.GetHashCode();
    public virtual bool Equals(VBNumericTypedValue? other)
    {
        return other != null && other.NumericValue == NumericValue;
    }
}
