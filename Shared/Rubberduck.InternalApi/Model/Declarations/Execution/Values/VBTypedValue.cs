using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public interface IVBTypedValue<TValue>
{
    TValue CurrentValue { get; }
    TValue DefaultValue { get; }
}

public record class VBTypedValue
{
    public VBTypedValue(VBType type, TypedSymbol? symbol = null)
    {
        TypeInfo = type;
        Symbol = symbol;
    }

    public TypedSymbol? Symbol { get; init; }
    public VBType TypeInfo { get; init; }
}

public record class VBObjectValue : VBTypedValue, IVBTypedValue<object?>
{
    public VBObjectValue(TypedSymbol? declarationSymbol = null)
        : base(VBObjectType.TypeInfo, declarationSymbol) { }

    public object? CurrentValue { get; } = default;
    public object? DefaultValue { get; } = default;
}

public record class VBVariantValue : VBTypedValue, IVBTypedValue<object?>
{
    public VBVariantValue(VBType subtype, TypedSymbol? declarationSymbol = null) 
        : base(VBVariantType.TypeInfo with { Subtype = subtype }, declarationSymbol) { }

    public object? CurrentValue { get; } = default;
    public object? DefaultValue { get; } = default;
}

public record class VBLongPtrValue : VBTypedValue, IVBTypedValue<int>
{
    public VBLongPtrValue(TypedSymbol? declarationSymbol = null) 
        : base(VBLongPtrType.TypeInfo, declarationSymbol) { }

    public int CurrentValue { get; } = default;
    public int DefaultValue { get; } = default;
}

public record class VBUserDefinedTypeValue : VBTypedValue, IVBTypedValue<object>
{
    public VBUserDefinedTypeValue(VBUserDefinedType type, TypedSymbol? declarationSymbol = null) 
        : base(type, declarationSymbol)
    {
        DefaultValue = new object();
        CurrentValue = DefaultValue;
    }

    public object CurrentValue { get; }
    public object DefaultValue { get; }
}

public record class VBEnumValue : VBTypedValue, IVBTypedValue<VBLongValue?>
{
    public VBEnumValue(VBLongValue value, EnumMemberSymbol declarationSymbol) 
        : base(value.TypeInfo, declarationSymbol) 
    {
    }

    public VBLongValue? CurrentValue { get; }
    public VBLongValue? DefaultValue { get; }
}