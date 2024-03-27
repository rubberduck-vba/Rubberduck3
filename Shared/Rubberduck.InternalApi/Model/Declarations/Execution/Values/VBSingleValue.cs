using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBSingleValue : VBNumericTypedValue, 
    IVBTypedValue<VBSingleValue, float>, 
    INumericValue<VBSingleValue>
{
    public VBSingleValue(TypedSymbol? declarationSymbol = null)
        : base(VBSingleType.TypeInfo, declarationSymbol) { }

    public static VBSingleValue MinValue { get; } = new VBSingleValue { NumericValue = float.MinValue };
    public static VBSingleValue MaxValue { get; } = new VBSingleValue { NumericValue = float.MaxValue };
    public static VBSingleValue Zero { get; } = new VBSingleValue { NumericValue = 0 };

    VBSingleValue INumericValue<VBSingleValue>.MinValue => MinValue;
    VBSingleValue INumericValue<VBSingleValue>.Zero => Zero;
    VBSingleValue INumericValue<VBSingleValue>.MaxValue => MaxValue;

    public float Value => (float)NumericValue;
    public override int Size => sizeof(float);
    public override double NumericValue { get; init; }

    public new VBSingleValue WithValue(double value) => this with { NumericValue = (float)value };
}
