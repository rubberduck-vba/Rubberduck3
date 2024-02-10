using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBVariantValue : VBTypedValue, IVBTypedValue<VBVariantValue, object?>, INumericCoercion, IStringCoercion
{
    public VBVariantValue(VBTypedValue typedValue, TypedSymbol? symbol = null) 
        : base(VBVariantType.TypeInfo with { Subtype = typedValue.TypeInfo }, symbol) { }

    public VBTypedValue? TypedValue { get; init; } = default;
    public object? Value { get; init; } = default;
    public override int Size => IntPtr.Size;

    public VBDoubleValue? AsCoercedNumeric(int depth = 0) => 
        ((VBVariantType)TypeInfo).Subtype is INumericCoercion coercibleNumeric ? coercibleNumeric.AsCoercedNumeric(depth) : null!;
    
    public VBStringValue? AsCoercedString(int depth = 0) => 
        ((VBVariantType)TypeInfo).Subtype is IStringCoercion coercibleString ? coercibleString.AsCoercedString(depth) : null!;

    public VBVariantValue WithValue(VBTypedValue value) => 
        this with 
        { 
            TypedValue = value, 
            Value = value, 
            TypeInfo = VBVariantType.TypeInfo with { Subtype = value.TypeInfo }
        };
}
