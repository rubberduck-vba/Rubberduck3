using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBSingleValue : VBTypedValue, IVBTypedValue<float>, INumericValue, INumericCoercion, IStringCoercion
{
    public VBSingleValue(TypedSymbol? declarationSymbol = null) 
        : base(VBSingleType.TypeInfo, declarationSymbol) { }

    public float Value { get; set; } = default;
    public float DefaultValue { get; } = default;

    public double AsDouble() => (double)Value;
    public double? AsCoercedNumeric(int depth = 0) => 0;
    public string? AsCoercedString(int depth = 0) => string.Empty;
    public VBTypedValue WithValue(double value) => this with { Value = (float)value };
}
