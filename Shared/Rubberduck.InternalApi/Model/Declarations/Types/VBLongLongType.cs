using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBLongLongType : VBIntrinsicType<long>, INumericType
{
    private VBLongLongType() : base(Tokens.LongLong) { }

    public static VBLongLongType TypeInfo { get; } = new();

    public override VBTypedValue DefaultValue { get; } = VBLongLongValue.Zero;
    public override VBType[] ConvertsSafelyToTypes { get; }
        = [VbDecimalType, VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
}
