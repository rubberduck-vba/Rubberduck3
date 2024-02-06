using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBIntegerValue : VBTypedValue, IVBTypedValue<short>, INumericValue, INumericCoercion, IStringCoercion
{
    public static short MinValue { get; } = short.MinValue;
    public static short MaxValue { get; } = short.MaxValue;
    public static VBIntegerValue Zero { get; } = new VBIntegerValue { Value = 0 };

    public VBIntegerValue(TypedSymbol? declarationSymbol = null) 
        : base(VBIntegerType.TypeInfo, declarationSymbol) { }

    public short Value { get; init; } = default;
    public short DefaultValue { get; } = default;

    public double? AsCoercedNumeric(int depth = 0) => AsDouble();
    public string? AsCoercedString(int depth = 0) => Value.ToString();
    public double AsDouble() => (double)Value;
    public int AsLong() => (int)Value;
    public short AsInteger() => (short)Value;

    public VBTypedValue WithValue(double value)
    {
        if (value > MaxValue || value < MinValue)
        {
            throw VBRuntimeErrorException.Overflow(Symbol!, $"`{TypeInfo.Name}` values must be between **{MinValue:N}** and **{MaxValue:N}**.");
        }
        return this with { Value = (short)value };
    }
}
