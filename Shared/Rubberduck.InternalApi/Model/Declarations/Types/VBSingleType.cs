using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBSingleType : VBIntrinsicType<float>
{
    private VBSingleType() : base(Tokens.Single) { }
    public static VBSingleType TypeInfo { get; } = new();

    public override float DefaultValue { get; }
    public override VBType[] ConvertsSafelyToTypes { get; }
        = [VbDoubleType, VbStringType, VbVariantType];
}
