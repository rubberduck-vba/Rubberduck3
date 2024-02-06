using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBVariantValue : VBTypedValue, IVBTypedValue<object?>, INumericCoercion, IStringCoercion
{
    public VBVariantValue(VBTypedValue typedValue, TypedSymbol declarationSymbol) 
        : base(VBVariantType.TypeInfo with { Subtype = typedValue.TypeInfo }, declarationSymbol) { }

    public VBTypedValue? TypedValue { get; init; } = default;
    public object? Value { get; init; } = default;
    public object? DefaultValue { get; } = default;

    public double? AsCoercedNumeric(int depth = 0) => 
        ((VBVariantType)TypeInfo).Subtype is INumericCoercion coercibleNumeric ? coercibleNumeric.AsCoercedNumeric(depth) : null;
    
    public string? AsCoercedString(int depth = 0) => 
        ((VBVariantType)TypeInfo).Subtype is IStringCoercion coercibleString ? coercibleString.AsCoercedString(depth) : null;

    public VBVariantValue WithValue(VBTypedValue value) => 
        this with 
        { 
            TypedValue = value, 
            Value = value, 
            TypeInfo = VBVariantType.TypeInfo with { Subtype = value.TypeInfo }
        };
}
