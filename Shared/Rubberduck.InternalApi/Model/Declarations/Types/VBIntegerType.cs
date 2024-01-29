using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBIntegerType : VBIntrinsicType<short>, INumericType
{
    private VBIntegerType() : base(Tokens.Integer) 
    {
        Size = 16;
    }
    public static VBIntegerType TypeInfo { get; } = new();

    public override short DefaultValue { get; }
    public override VBType[] ConvertsSafelyToTypes { get; }
        = [VbLongType, VbLongLongType, VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
}
