using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public abstract record class VBNumericTypedValue : VBTypedValue, 
    INumericValue, 
    INumericCoercion, 
    IStringCoercion, 
    IComparable<VBNumericTypedValue>
{
    protected VBNumericTypedValue(VBType type, TypedSymbol? symbol = null)
        : base(type, symbol) { }

    protected abstract double State { get; }

    public VBDoubleValue WithValue(double value) => new VBDoubleValue(Symbol).WithValue(value);

    public VBDoubleValue AsCoercedNumeric(int depth = 0) => AsDouble();
    public VBStringValue AsCoercedString(int depth = 0) => new VBStringValue(Symbol).WithValue(ToString());

    public VBBooleanValue AsBoolean() => new VBBooleanValue(Symbol).WithValue(State != 0);
    public VBByteValue AsByte() => new VBByteValue(Symbol).WithValue(State);
    public VBCurrencyValue AsCurrency() => new VBCurrencyValue(Symbol).WithValue(State);
    public VBDecimalValue AsDecimal() => new VBDecimalValue(Symbol).WithValue(State);
    public VBDoubleValue AsDouble() => new VBDoubleValue(Symbol).WithValue(State);
    public VBIntegerValue AsInteger() => new VBIntegerValue(Symbol).WithValue(State);
    public VBLongValue AsLong() => new VBLongValue(Symbol).WithValue(State);
    public VBLongLongValue AsLongLong() => new VBLongLongValue(Symbol).WithValue(State);
    public VBSingleValue AsSingle() => new VBSingleValue(Symbol).WithValue(State);

    public int CompareTo(VBNumericTypedValue? other) => other is null ? 1 : State.CompareTo(other.State);
    public override string ToString() => State.ToString();
}
