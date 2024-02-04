using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBByteValue : VBTypedValue, IVBTypedValue<byte>, INumericValue, INumericCoercion, IStringCoercion
{
    public VBByteValue(TypedSymbol? declarationSymbol = null) 
        : base(VBByteType.TypeInfo, declarationSymbol) { }

    public byte Value { get; init; } = default;
    public byte DefaultValue { get; } = default;

    public double? AsCoercedNumeric(int depth = 0) => AsDouble();
    public string? AsCoercedString(int depth = 0) => Value.ToString();

    public double AsDouble() => (double)Value;
    public VBTypedValue WithValue(double value)
    {
        if (value > byte.MaxValue || value < byte.MinValue)
        {
            throw VBRuntimeErrorException.Overflow;
        }
        return this with { Value = (byte)value };
    }
}
