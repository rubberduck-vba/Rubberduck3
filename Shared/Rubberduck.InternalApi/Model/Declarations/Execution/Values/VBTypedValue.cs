using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public interface IVBTypedValue<VBTValue, TValue, TNominal> where VBTValue : VBTypedValue
{
    VBTValue DefaultValue { get; }
    TValue Value { get; }
    TNominal NominalValue { get; }
}

public interface IVBTypedValue<VBTValue, TValue> : IVBTypedValue<VBTValue, TValue, TValue> where VBTValue : VBTypedValue { }

public abstract record class VBTypedValue
{
    public VBTypedValue(VBType type, TypedSymbol? symbol = null)
    {
        TypeInfo = type;
        Symbol = symbol;
    }

    public bool IsArray() => this is VBArrayValue || TypeInfo is VBVariantType variant && variant.Subtype is VBArrayType;
    public bool IsObject() => this is VBObjectValue || TypeInfo is VBVariantType variant && variant.Subtype is VBObjectType;
    public bool IsNull() => this is VBNullValue || TypeInfo is VBVariantType variant && variant.Subtype is VBNullType;
    public bool IsEmpty() => this is VBEmptyValue || TypeInfo is VBVariantType variant && variant.Subtype is VBEmptyType;
    public bool IsMissing() => this is VBMissingValue || TypeInfo is VBVariantType variant && variant.Subtype is VBMissingType;
    public bool IsError() => this is VBErrorValue || TypeInfo is VBVariantType variant && variant.Subtype is VBErrorType;
    public bool IsNumeric() => this is INumericValue;

    public bool IsWithBlockVariable { get; init; }

    public VBVariantValue AsVariant() => new(this, Symbol);

    public TypedSymbol? Symbol { get; init; }
    public VBType TypeInfo { get; init; }

    public abstract int Size { get; }

    public abstract override string ToString();
}
