using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBIntegerValue : VBNumericTypedValue, 
    IVBTypedValue<VBIntegerValue, short>,
    INumericValue<VBIntegerValue>
{
    public VBIntegerValue(TypedSymbol? declarationSymbol = null)
        : base(VBIntegerType.TypeInfo, declarationSymbol) { }

    public static VBIntegerValue MinValue { get; } = new VBIntegerValue { NumericValue = short.MinValue };
    public static VBIntegerValue MaxValue { get; } = new VBIntegerValue { NumericValue = short.MaxValue };
    public static VBIntegerValue Zero { get; } = new VBIntegerValue { NumericValue = 0 };

    VBIntegerValue INumericValue<VBIntegerValue>.MinValue => MinValue;
    VBIntegerValue INumericValue<VBIntegerValue>.Zero => Zero;
    VBIntegerValue INumericValue<VBIntegerValue>.MaxValue => MaxValue;

    public short Value => (short)NumericValue;
    public override int Size { get; } = sizeof(short);
    public override double NumericValue { get; init; }

    public VBIntegerValue WithValue(double value)
    {
        if (value > MaxValue.Value || value < MinValue.Value)
        {
            throw VBRuntimeErrorException.Overflow(Symbol!, $"`{TypeInfo.Name}` values must be between **{MinValue.Value:N}** and **{MaxValue.Value:N}**.");
        }
        return this with { NumericValue = (short)value };
    }

    public new VBIntegerValue WithValue(int value) => WithValue((double)value);
}
