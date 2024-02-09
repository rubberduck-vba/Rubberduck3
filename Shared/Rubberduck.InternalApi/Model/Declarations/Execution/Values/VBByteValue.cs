using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBByteValue : VBNumericTypedValue, 
    IVBTypedValue<VBByteValue, byte>, 
    INumericValue<VBByteValue>
{
    public VBByteValue(TypedSymbol? declarationSymbol = null)
        : base(VBByteType.TypeInfo, declarationSymbol) { }

    public static VBByteValue MinValue { get; } = new VBByteValue().WithValue(byte.MinValue);
    public static VBByteValue MaxValue { get; } = new VBByteValue().WithValue(byte.MaxValue);
    public static VBByteValue Zero { get; } = new VBByteValue().WithValue(0);

    VBByteValue INumericValue<VBByteValue>.MinValue => MinValue;
    VBByteValue INumericValue<VBByteValue>.Zero => Zero;
    VBByteValue INumericValue<VBByteValue>.MaxValue => MaxValue;

    public byte Value { get; init; } = default;
    public VBByteValue DefaultValue { get; } = Zero;
    public byte NominalValue => Value;

    public override int Size { get; } = 1;
    protected override double State => Value;

    public VBByteValue WithValue(double value)
    {
        if (value > MaxValue.Value || value < MinValue.Value)
        {
            throw VBRuntimeErrorException.Overflow(Symbol!, $"`{TypeInfo.Name}` values must be between **{MinValue.Value:N}** and **{MaxValue.Value:N}**.");
        }
        return this with { Value = (byte)value };
    }

    public VBByteValue WithValue(int value) => WithValue((double)value);
}
