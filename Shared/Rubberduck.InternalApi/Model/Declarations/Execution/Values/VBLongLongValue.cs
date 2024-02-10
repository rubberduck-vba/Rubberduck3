using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBLongLongValue : VBNumericTypedValue,
    IVBTypedValue<VBLongLongValue, long>,
    INumericValue<VBLongLongValue>
{
    public VBLongLongValue(TypedSymbol? declarationSymbol = null)
        : base(VBLongLongType.TypeInfo, declarationSymbol) { }

    public static VBLongLongValue MinValue { get; } = new VBLongLongValue { NumericValue = long.MinValue };
    public static VBLongLongValue MaxValue { get; } = new VBLongLongValue { NumericValue = long.MaxValue };
    public static VBLongLongValue Zero { get; } = new VBLongLongValue { NumericValue = 0 };

    VBLongLongValue INumericValue<VBLongLongValue>.MinValue => MinValue;
    VBLongLongValue INumericValue<VBLongLongValue>.Zero => Zero;
    VBLongLongValue INumericValue<VBLongLongValue>.MaxValue => MaxValue;

    public long Value => (long)NumericValue;    
    public override int Size => sizeof(long);
    public override double NumericValue { get; init; }

    public VBLongLongValue WithValue(double value) => this with { NumericValue = (long)value };
    public new VBLongLongValue WithValue(int value) => this with { NumericValue = (long)value };
}
