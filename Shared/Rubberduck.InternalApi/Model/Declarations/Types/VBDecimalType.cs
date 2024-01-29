using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBDecimalType : VBIntrinsicType<decimal>, INumericType
{
    private VBDecimalType() : base(Tokens.Decimal) 
    {
        Size = 64;
    }
    public static VBDecimalType TypeInfo { get; } = new();

    public override decimal DefaultValue { get; }

    public override bool IsDeclarable { get; } = false; // indeed, "As Decimal" is illegal, ..but CDec() returns a Decimal.

    public override VBType[] ConvertsSafelyToTypes { get; }
        = [VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
}
