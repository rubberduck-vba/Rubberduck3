using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBDoubleType : VBIntrinsicType<double>, INumericType
{
    private VBDoubleType() : base(Tokens.Double) { }

    public static VBDoubleType TypeInfo { get; } = new();

    public override VBTypedValue DefaultValue { get; } = VBDoubleValue.Zero;
    public override VBType[] ConvertsSafelyToTypes { get; } = [VbStringType, VbVariantType];
}
