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

    public static VBDecimalValue MinValue { get; } = new VBDecimalValue { NumericValue = (double)(long.MinValue * Math.Pow(10, -4)) };
    public static VBDecimalValue MaxValue { get; } = new VBDecimalValue { NumericValue = (double)(long.MaxValue * Math.Pow(10, -4)) };
    public static VBDecimalValue Zero { get; } = new VBDecimalValue { NumericValue = 0 };

    VBDecimalValue INumericValue<VBDecimalValue>.MinValue => MinValue;
    VBDecimalValue INumericValue<VBDecimalValue>.Zero => Zero;
    VBDecimalValue INumericValue<VBDecimalValue>.MaxValue => MaxValue;

    public decimal Value => (decimal)NumericValue;    
    public override int Size => 14;
    public override double NumericValue { get; init; }

    public new VBDecimalValue WithValue(double value) => this with { NumericValue = (double)value };
}
