using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBDoubleType : VBIntrinsicType<double>, INumericType
{
    private VBDoubleType() : base(Tokens.Double) 
    {
        Size = 128;
    }
    public static VBDoubleType TypeInfo { get; } = new();

    public override double DefaultValue { get; }
    public override VBType[] ConvertsSafelyToTypes { get; } = [VbStringType, VbVariantType];
}
