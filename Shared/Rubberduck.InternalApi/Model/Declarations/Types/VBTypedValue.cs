using System;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBTypedValue<T>
{
    public VBTypedValue(VBType<T> type, T value)
    {
        TypeInfo = type;
        Value = value;
    }

    public VBType<T> TypeInfo { get; init; }
    public T Value { get; init; }

    public VBTypedValue<T> SetValue(T value) => this with { Value = value };
}

public record class VBBooleanValue : VBTypedValue<bool>
{
    public VBBooleanValue(bool value) 
        : base(VBBooleanType.TypeInfo, value)
    {
    }
}

public record class VBByteValue : VBTypedValue<byte>
{
    public VBByteValue(byte value)
        : base(VBByteType.TypeInfo, value)
    {
    }
}

public record class VBIntegerValue : VBTypedValue<short>
{
    public VBIntegerValue(short value)
        : base(VBIntegerType.TypeInfo, value)
    {
    }
}

public record class VBLongValue : VBTypedValue<int>
{
    public VBLongValue(int value)
        : base(VBLongType.TypeInfo, value)
    {
    }
}

public record class VBLongLongValue : VBTypedValue<long>
{
    public VBLongLongValue(long value)
        : base(VBLongLongType.TypeInfo, value)
    {
    }
}

public record class VBCurrencyValue : VBTypedValue<decimal>
{ 
    public VBCurrencyValue(decimal value)
        : base(VBCurrencyType.TypeInfo, value)
    {
    }
}

public record class VBSingleValue : VBTypedValue<float>
{
    public VBSingleValue(float value)
        : base(VBSingleType.TypeInfo, value)
    {
    }
}

public record class VBDoubleValue : VBTypedValue<double>
{
    public VBDoubleValue(double value)
        : base(VBDoubleType.TypeInfo, value)
    {
    }
}

public record class VBDateValue : VBTypedValue<DateTime>
{
    public VBDateValue(DateTime value)
        : base(VBDateType.TypeInfo, value)
    {
    }
}

public record class VBStringValue : VBTypedValue<string?>
{
    public VBStringValue(string value)
        : base(VBStringType.TypeInfo, value)
    {
    }
}

public record class VBObjectValue : VBTypedValue<object?>
{
    public VBObjectValue(object? value)
        : base(VBObjectType.TypeInfo, value)
    {
    }
}

public record class VBVariantValue : VBTypedValue<object?>
{
    public VBVariantValue(VBType subtype, object? value)
        : base(VBVariantType.TypeInfo with { Subtype = subtype }, value)
    {
    }
}

public record class VBLongPtrValue : VBTypedValue<int>
{
    public VBLongPtrValue(int value)
        : base(VBLongPtrType.TypeInfo, value)
    {
    }
}

public record class VBClassReferenceValue : VBTypedValue<object?>
{
    public VBClassReferenceValue(VBClassType type, object? value)
        : base(type, value)
    {
    }
}

public record class VBUserDefinedTypeValue : VBTypedValue<object>
{
    public VBUserDefinedTypeValue(VBUserDefinedType type)
        : base(type, type.DefaultValue)
    {
    }
}

public record class VBEnumValue : VBTypedValue<int>
{
    public VBEnumValue(VBEnumType type, int value)
        : base(type, value)
    {
    }
}