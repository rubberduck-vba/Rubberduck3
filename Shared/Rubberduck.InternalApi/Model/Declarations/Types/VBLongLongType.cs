using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBLongLongType : VBIntrinsicType<long>, INumericType
{
    private VBLongLongType() : base(Tokens.LongLong) 
    {
        Size = 64;
    }
    public static VBLongLongType TypeInfo { get; } = new();

    public override long DefaultValue { get; }
    public override VBType[] ConvertsSafelyToTypes { get; }
        = [VbDecimalType, VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
}
