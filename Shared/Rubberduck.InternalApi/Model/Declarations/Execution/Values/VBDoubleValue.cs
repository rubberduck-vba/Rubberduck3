using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBDoubleValue : VBNumericTypedValue, 
    IVBTypedValue<VBDoubleValue, double>,
    INumericValue<VBDoubleValue>
{
    public VBDoubleValue(TypedSymbol? symbol = null)
        : base(VBDoubleType.TypeInfo, symbol) { }

    public static VBDoubleValue MinValue { get; } = new VBDoubleValue().WithValue(double.MinValue * Math.Pow(10, -4));
    public static VBDoubleValue MaxValue { get; } = new VBDoubleValue().WithValue(double.MaxValue * Math.Pow(10, -4));
    public static VBDoubleValue Zero { get; } = new VBDoubleValue().WithValue(0);

    VBDoubleValue INumericValue<VBDoubleValue>.MinValue => MinValue;
    VBDoubleValue INumericValue<VBDoubleValue>.Zero => Zero;
    VBDoubleValue INumericValue<VBDoubleValue>.MaxValue => MaxValue;

    public double Value { get; init; } = default;
    public VBDoubleValue DefaultValue { get; } = Zero;
    public double NominalValue => Value;

    public override int Size => 8;
    protected override double State => (double)Value;

    public VBDoubleValue WithValue(double value) => this with { Value = value };
}
