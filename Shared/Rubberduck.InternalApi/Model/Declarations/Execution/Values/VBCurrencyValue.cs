using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBCurrencyValue : VBNumericTypedValue,
    IVBTypedValue<VBCurrencyValue, decimal>,
    INumericValue<VBCurrencyValue>
{
    public VBCurrencyValue(TypedSymbol? symbol = null)
        : base(VBCurrencyType.TypeInfo, symbol) { }

    public static VBCurrencyValue MinValue { get; } = new VBCurrencyValue { NumericValue = (double)(long.MinValue * Math.Pow(10, -4)) };
    public static VBCurrencyValue MaxValue { get; } = new VBCurrencyValue { NumericValue = (double)(long.MaxValue * Math.Pow(10, -4)) };
    public static VBCurrencyValue Zero { get; } = new VBCurrencyValue { NumericValue = 0 };

    VBCurrencyValue INumericValue<VBCurrencyValue>.MinValue => MinValue;
    VBCurrencyValue INumericValue<VBCurrencyValue>.Zero => Zero;
    VBCurrencyValue INumericValue<VBCurrencyValue>.MaxValue => MaxValue;

    public decimal Value => (decimal)NumericValue;
    public override int Size { get; } = sizeof(long);
    public override double NumericValue { get; init; }

    public VBCurrencyValue WithValue(double value) => this with { NumericValue = (double)value };
}
