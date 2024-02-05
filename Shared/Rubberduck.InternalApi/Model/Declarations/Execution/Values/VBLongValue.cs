using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBLongValue : VBTypedValue, IVBTypedValue<int>, INumericValue, INumericCoercion, IStringCoercion
{
    public static int MinValue { get; } = int.MinValue;
    public static int MaxValue { get; } = int.MaxValue;

    public VBLongValue(TypedSymbol? declarationSymbol = null) 
        : base(VBLongType.TypeInfo, declarationSymbol) { }

    public int Value { get; init; } = default;
    public int DefaultValue { get; } = default;

    public double? AsCoercedNumeric(int depth = 0) => AsDouble();
    public string? AsCoercedString(int depth = 0) => Value.ToString();
    public double AsDouble() => (double)Value;
    public VBTypedValue WithValue(double value)
    {
        if (value > MaxValue || value < MinValue)
        {
            throw VBRuntimeErrorException.Overflow(Symbol!, $"`{TypeInfo.Name}` values must be between **{MinValue:N}** and **{MaxValue:N}**.");
        }
        return this with { Value = (int)value };
    }
}
