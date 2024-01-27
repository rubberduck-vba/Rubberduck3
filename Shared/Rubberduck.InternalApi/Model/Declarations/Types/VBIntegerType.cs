using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBIntegerType : VBIntrinsicType<short>
{
    private VBIntegerType() : base(Tokens.Integer) { }
    public static VBIntegerType TypeInfo { get; } = new();

    public override short DefaultValue { get; }
    public override VBType[] ConvertsSafelyToTypes { get; }
        = [VbLongType, VbLongLongType, VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
}
