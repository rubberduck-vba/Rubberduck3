using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBCurrencyType : VBIntrinsicType<decimal>, INumericType
{
    private VBCurrencyType() : base(Tokens.Currency) { }

    public static VBCurrencyType TypeInfo { get; } = new();

    public override VBTypedValue DefaultValue { get; } = VBCurrencyValue.Zero;
    public override VBType[] ConvertsSafelyToTypes { get; }
        = [VbDecimalType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
}
