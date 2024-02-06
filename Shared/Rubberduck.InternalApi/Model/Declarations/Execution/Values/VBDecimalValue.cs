using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBDecimalValue : VBTypedValue, IVBTypedValue<decimal>, INumericValue, INumericCoercion, IStringCoercion
{
    public VBDecimalValue(TypedSymbol? declarationSymbol = null)
        : base(VBDecimalType.TypeInfo, declarationSymbol) { }

    public decimal Value { get; init; } = default;
    public decimal DefaultValue { get; } = default;

    public double? AsCoercedNumeric(int depth = 0) => AsDouble();
    public string? AsCoercedString(int depth = 0) => Value.ToString();
    public double AsDouble() => (double)Value;
    public int AsLong() => (int)Value;
    public short AsInteger() => (short)Value;
    public VBTypedValue WithValue(double value) => this with { Value = (decimal)value };

    public VBDecimalValue WithValue(decimal value) => this with { Value = value };
}
