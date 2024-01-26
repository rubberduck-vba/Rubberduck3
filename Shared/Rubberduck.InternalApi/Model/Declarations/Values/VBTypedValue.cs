using System;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Values;

public interface IVBTypedValue<TValue>
{
    TValue CurrentValue { get; }
    TValue DefaultValue { get; }
}

public record class VBTypedValue
{
    public VBTypedValue(VBType type)
    {
        TypeInfo = type;
    }

    public VBType TypeInfo { get; init; }
}

public record class VBBooleanValue : VBTypedValue, IVBTypedValue<bool>
{
    public VBBooleanValue() : base(VBBooleanType.TypeInfo) { }

    public bool CurrentValue { get; } = default;
    public bool DefaultValue { get; } = default;
}

public record class VBByteValue : VBTypedValue, IVBTypedValue<byte>
{
    public VBByteValue(): base(VBByteType.TypeInfo) { }

    public byte CurrentValue { get; } = default;
    public byte DefaultValue { get; } = default;
}

public record class VBIntegerValue : VBTypedValue, IVBTypedValue<short>
{
    public VBIntegerValue() : base(VBIntegerType.TypeInfo) { }

    public short CurrentValue { get; } = default;
    public short DefaultValue { get; } = default;
}

public record class VBLongValue : VBTypedValue, IVBTypedValue<int>
{
    public VBLongValue() : base(VBLongType.TypeInfo) { }

    public int CurrentValue { get; } = default;
    public int DefaultValue { get; } = default;
}

public record class VBLongLongValue : VBTypedValue, IVBTypedValue<long>
{
    public VBLongLongValue() : base(VBLongLongType.TypeInfo) { }

    public long CurrentValue { get; } = default;
    public long DefaultValue { get; } = default;
}

public record class VBCurrencyValue : VBTypedValue, IVBTypedValue<decimal>
{
    public VBCurrencyValue() : base(VBCurrencyType.TypeInfo) { }

    public decimal CurrentValue { get; } = default;
    public decimal DefaultValue { get; } = default;
}

public record class VBSingleValue : VBTypedValue, IVBTypedValue<float>
{
    public VBSingleValue() : base(VBSingleType.TypeInfo) { }

    public float CurrentValue { get; } = default;
    public float DefaultValue { get; } = default;
}

public record class VBDoubleValue : VBTypedValue, IVBTypedValue<double>
{
    public VBDoubleValue() : base(VBDoubleType.TypeInfo) { }

    public double CurrentValue { get; } = default;
    public double DefaultValue { get; } = default;
}

public record class VBDateValue : VBTypedValue, IVBTypedValue<DateTime>
{
    public VBDateValue() : base(VBDateType.TypeInfo) { }

    public DateTime CurrentValue { get; } = default;
    public DateTime DefaultValue { get; } = default;
}

public record class VBStringValue : VBTypedValue, IVBTypedValue<string?>
{
    public static VBStringValue VBNullString { get; } = new VBStringValue();

    public VBStringValue() : base(VBStringType.TypeInfo) { }

    public string? CurrentValue { get; } = default;
    public string? DefaultValue { get; } = default;
}

public record class VBObjectValue : VBTypedValue, IVBTypedValue<object?>
{
    public VBObjectValue() : base(VBObjectType.TypeInfo) { }

    public object? CurrentValue { get; } = default;
    public object? DefaultValue { get; } = default;
}

public record class VBVariantValue : VBTypedValue, IVBTypedValue<object?>
{
    public VBVariantValue(VBType subtype) : base(VBVariantType.TypeInfo with { Subtype = subtype }) { }

    public object? CurrentValue { get; } = default;
    public object? DefaultValue { get; } = default;
}

public record class VBLongPtrValue : VBTypedValue, IVBTypedValue<int>
{
    public VBLongPtrValue() : base(VBLongPtrType.TypeInfo) { }

    public int CurrentValue { get; } = default;
    public int DefaultValue { get; } = default;
}

public record class VBClassReferenceValue : VBTypedValue, IVBTypedValue<int?>
{
    public VBClassReferenceValue(VBClassType type) : base(type) { }

    public int? CurrentValue { get; } = default;
    public int? DefaultValue { get; } = default;
}

public record class VBUserDefinedTypeValue : VBTypedValue, IVBTypedValue<object>
{
    public VBUserDefinedTypeValue(VBUserDefinedType type) : base(type) 
    {
        DefaultValue = new object();
        CurrentValue = DefaultValue;
    }

    public object CurrentValue { get; }
    public object DefaultValue { get; }
}

public record class VBEnumValue : VBTypedValue, IVBTypedValue<int>
{
    public VBEnumValue(VBEnumType type) : base(type) { }

    public int CurrentValue { get; } = default;
    public int DefaultValue { get; } = default;
}