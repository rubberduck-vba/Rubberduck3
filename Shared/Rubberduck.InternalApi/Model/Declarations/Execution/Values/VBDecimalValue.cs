using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBDecimalValue : VBNumericTypedValue, 
    IVBTypedValue<VBDecimalValue, decimal>,
    INumericValue<VBDecimalValue>
{
    public VBDecimalValue(TypedSymbol? declarationSymbol = null)
        : base(VBDecimalType.TypeInfo, declarationSymbol) { }

    public static VBDecimalValue MinValue { get; } = new VBDecimalValue().WithValue(long.MinValue * Math.Pow(10, -4));
    public static VBDecimalValue MaxValue { get; } = new VBDecimalValue().WithValue(long.MaxValue * Math.Pow(10, -4));
    public static VBDecimalValue Zero { get; } = new VBDecimalValue().WithValue(0);

    VBDecimalValue INumericValue<VBDecimalValue>.MinValue => MinValue;
    VBDecimalValue INumericValue<VBDecimalValue>.Zero => Zero;
    VBDecimalValue INumericValue<VBDecimalValue>.MaxValue => MaxValue;

    public decimal Value { get; init; } = default;
    public VBDecimalValue DefaultValue { get; } = Zero;
    public decimal NominalValue => Value;

    public override int Size => 14;
    protected override double State => (double)Value;

    public VBDecimalValue WithValue(double value) => this with { Value = (decimal)value };
}
