using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBCurrencyValue : VBTypedValue, IVBTypedValue<decimal>, INumericValue, INumericCoercion, IStringCoercion
{
    public VBCurrencyValue(TypedSymbol? declarationSymbol = null) 
        : base(VBCurrencyType.TypeInfo, declarationSymbol) { }

    public decimal Value { get; init; } = default;
    public decimal DefaultValue { get; } = default;

    public double? AsCoercedNumeric(int depth = 0) => AsDouble();
    public string? AsCoercedString(int depth = 0) => Value.ToString();
    public double AsDouble() => (double)Value;
    public VBTypedValue WithValue(double value) => this with { Value = (decimal)value };
}
