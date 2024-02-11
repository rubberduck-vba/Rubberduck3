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

    public static VBDoubleValue MinValue { get; } = new VBDoubleValue { NumericValue = double.MinValue * Math.Pow(10, -4) };
    public static VBDoubleValue MaxValue { get; } = new VBDoubleValue { NumericValue = double.MaxValue * Math.Pow(10, -4) };
    public static VBDoubleValue Zero { get; } = new VBDoubleValue { NumericValue = 0 };

    VBDoubleValue INumericValue<VBDoubleValue>.MinValue => MinValue;
    VBDoubleValue INumericValue<VBDoubleValue>.Zero => Zero;
    VBDoubleValue INumericValue<VBDoubleValue>.MaxValue => MaxValue;

    public double Value => NumericValue;
    public override int Size => 8;
    public override double NumericValue { get; init; }

    public VBDoubleValue WithValue(double value) => this with { NumericValue = value };
}
