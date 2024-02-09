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

    public static VBCurrencyValue MinValue { get; } = new VBCurrencyValue().WithValue(long.MinValue * Math.Pow(10, -4));
    public static VBCurrencyValue MaxValue { get; } = new VBCurrencyValue().WithValue(long.MaxValue * Math.Pow(10, -4));
    public static VBCurrencyValue Zero { get; } = new VBCurrencyValue().WithValue(0);

    VBCurrencyValue INumericValue<VBCurrencyValue>.MinValue => MinValue;
    VBCurrencyValue INumericValue<VBCurrencyValue>.Zero => Zero;
    VBCurrencyValue INumericValue<VBCurrencyValue>.MaxValue => MaxValue;

    public decimal Value { get; init; } = default;
    public VBCurrencyValue DefaultValue { get; } = Zero;
    public decimal NominalValue => Value;

    public override int Size { get; } = sizeof(long);
    protected override double State => (double)Value;

    public VBCurrencyValue WithValue(double value) => this with { Value = (decimal)value };
}
