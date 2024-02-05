using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBByteValue : VBTypedValue, IVBTypedValue<byte>, INumericValue, INumericCoercion, IStringCoercion
{
    public static byte MinValue { get; } = byte.MinValue;
    public static byte MaxValue { get; } = byte.MaxValue;

    public VBByteValue(TypedSymbol? declarationSymbol = null) 
        : base(VBByteType.TypeInfo, declarationSymbol) { }

    public byte Value { get; init; } = default;
    public byte DefaultValue { get; } = default;

    public double? AsCoercedNumeric(int depth = 0) => AsDouble();
    public string? AsCoercedString(int depth = 0) => Value.ToString();

    public double AsDouble() => (double)Value;
    public VBTypedValue WithValue(double value)
    {
        if (value > MaxValue || value < MinValue)
        {
            throw VBRuntimeErrorException.Overflow(Symbol!, $"`{TypeInfo.Name}` values must be between **{MinValue:N}** and **{MaxValue:N}**.");
        }
        return this with { Value = (byte)value };
    }

    public VBByteValue WithValue(int value) => (VBByteValue)WithValue((double)value);
}
