using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBSingleValue : VBNumericTypedValue, 
    IVBTypedValue<VBSingleValue, float>, 
    INumericValue<VBSingleValue>
{
    public VBSingleValue(TypedSymbol? declarationSymbol = null)
        : base(VBSingleType.TypeInfo, declarationSymbol) { }

    public static VBSingleValue MinValue { get; } = new VBSingleValue().WithValue(float.MinValue);
    public static VBSingleValue MaxValue { get; } = new VBSingleValue().WithValue(float.MaxValue);
    public static VBSingleValue Zero { get; } = new VBSingleValue().WithValue(0);

    VBSingleValue INumericValue<VBSingleValue>.MinValue => MinValue;
    VBSingleValue INumericValue<VBSingleValue>.Zero => Zero;
    VBSingleValue INumericValue<VBSingleValue>.MaxValue => MaxValue;

    public float Value { get; set; } = default;
    public VBSingleValue DefaultValue { get; } = Zero;
    public float NominalValue => Value;

    public override int Size => sizeof(float);
    protected override double State => Value;

    public VBSingleValue WithValue(double value) => this with { Value = (float)value };
}
