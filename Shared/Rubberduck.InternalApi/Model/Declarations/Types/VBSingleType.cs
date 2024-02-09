using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBSingleType : VBIntrinsicType<float>, INumericType
{
    private VBSingleType() : base(Tokens.Single) { }
    public static VBSingleType TypeInfo { get; } = new();

    public override VBTypedValue DefaultValue { get; } = VBSingleValue.Zero;
    public override VBType[] ConvertsSafelyToTypes { get; }
        = [VbDoubleType, VbStringType, VbVariantType];
}
