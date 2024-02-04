using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBLongValue : VBTypedValue, IVBTypedValue<int>, INumericValue, INumericCoercion, IStringCoercion
{
    public VBLongValue(TypedSymbol? declarationSymbol = null) 
        : base(VBLongType.TypeInfo, declarationSymbol) { }

    public int Value { get; init; } = default;
    public int DefaultValue { get; } = default;

    public double? AsCoercedNumeric(int depth = 0) => AsDouble();
    public string? AsCoercedString(int depth = 0) => Value.ToString();
    public double AsDouble() => (double)Value;
    public VBTypedValue WithValue(double value)
    {
        if (value > int.MaxValue || value < int.MinValue)
        {
            throw VBRuntimeErrorException.Overflow;
        }
        return this with { Value = (int)value };
    }
}
