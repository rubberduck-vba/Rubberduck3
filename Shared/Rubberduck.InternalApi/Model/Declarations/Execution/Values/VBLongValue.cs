using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBLongValue : VBNumericTypedValue, 
    IVBTypedValue<VBLongValue, int>, 
    INumericValue<VBLongValue>
{
    public VBLongValue(TypedSymbol? declarationSymbol = null)
        : base(VBLongType.TypeInfo, declarationSymbol) { }

    public static VBLongValue MinValue { get; } = new VBLongValue().WithValue(int.MinValue);
    public static VBLongValue MaxValue { get; } = new VBLongValue().WithValue(int.MaxValue);
    public static VBLongValue Zero { get; } = new VBLongValue().WithValue(0);

    VBLongValue INumericValue<VBLongValue>.MinValue => MinValue;
    VBLongValue INumericValue<VBLongValue>.Zero => Zero;
    VBLongValue INumericValue<VBLongValue>.MaxValue => MaxValue;

    public int Value { get; init; } = default;
    public VBLongValue DefaultValue { get; } = Zero;
    public int NominalValue => Value;

    public override int Size => sizeof(int);
    protected override double State => Value;

    public VBLongValue WithValue(double value)
    {
        if (value > MaxValue.Value || value < MinValue.Value)
        {
            throw VBRuntimeErrorException.Overflow(Symbol!, $"`{TypeInfo.Name}` values must be between **{MinValue.Value:N}** and **{MaxValue.Value:N}**.");
        }
        return this with { Value = (int)value };
    }

    public VBLongValue WithValue(int value) => WithValue((double)value);
}
