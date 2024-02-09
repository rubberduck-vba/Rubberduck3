using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBLongLongValue : VBNumericTypedValue,
    IVBTypedValue<VBLongLongValue, long>,
    INumericValue<VBLongLongValue>
{
    public VBLongLongValue(TypedSymbol? declarationSymbol = null)
        : base(VBLongLongType.TypeInfo, declarationSymbol) { }

    public static VBLongLongValue MinValue { get; } = new VBLongLongValue().WithValue(long.MinValue);
    public static VBLongLongValue MaxValue { get; } = new VBLongLongValue().WithValue(long.MaxValue);
    public static VBLongLongValue Zero { get; } = new VBLongLongValue().WithValue(0);

    VBLongLongValue INumericValue<VBLongLongValue>.MinValue => MinValue;
    VBLongLongValue INumericValue<VBLongLongValue>.Zero => Zero;
    VBLongLongValue INumericValue<VBLongLongValue>.MaxValue => MaxValue;

    public long Value { get; init; } = default;
    public VBLongLongValue DefaultValue { get; } = Zero;
    public long NominalValue => Value;

    public override int Size => sizeof(long);
    protected override double State => Value;

    public VBLongLongValue WithValue(double value) => this with { Value = (long)value };
    public VBLongLongValue WithValue(int value) => this with { Value = (long)value };
}
