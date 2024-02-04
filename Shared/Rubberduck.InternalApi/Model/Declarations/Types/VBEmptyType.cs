using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBEmptyType : VBIntrinsicType<int?>
{
    private VBEmptyType() : base(Tokens.vbEmpty) 
    {
        Size = 0;
    }
    public static VBEmptyType TypeInfo { get; } = new();

    public override int? DefaultValue { get; }
    public override VBType[] ConvertsSafelyToTypes { get; }
        = [VbBooleanType, VbByteType, VbIntegerType, VbLongType, VbLongLongType, VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
}
