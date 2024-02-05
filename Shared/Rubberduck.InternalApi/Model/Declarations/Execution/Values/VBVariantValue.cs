using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBVariantValue : VBTypedValue, IVBTypedValue<object?>, INumericCoercion, IStringCoercion
{
    public VBVariantValue(VBType subtype, TypedSymbol declarationSymbol) 
        : base(VBVariantType.TypeInfo with { Subtype = subtype }, declarationSymbol) { }

    public object? Value { get; init; } = default;
    public object? DefaultValue { get; } = default;

    public double? AsCoercedNumeric(int depth = 0) => ((VBVariantType)TypeInfo).Subtype is INumericCoercion coercibleNumeric ? coercibleNumeric.AsCoercedNumeric(depth) : null;
    public string? AsCoercedString(int depth = 0) => ((VBVariantType)TypeInfo).Subtype is IStringCoercion coercibleString ? coercibleString.AsCoercedString(depth) : null;
}
