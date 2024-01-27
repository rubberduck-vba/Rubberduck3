using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBLongType : VBIntrinsicType<int>
{
    private VBLongType() : base(Tokens.Long) { }
    public static VBLongType TypeInfo { get; } = new();

    public override int DefaultValue { get; }
    public override VBType[] ConvertsSafelyToTypes { get; }
        = [VbLongLongType, VbDecimalType, VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
}
