using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBLongType : VBIntrinsicType<int>, INumericType
{
    private VBLongType() : base(Tokens.Long) { }
    public static VBLongType TypeInfo { get; } = new();

    public override VBTypedValue DefaultValue { get; } = VBLongValue.Zero;
    public override VBType[] ConvertsSafelyToTypes { get; }
        = [VbLongLongType, VbDecimalType, VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
}
