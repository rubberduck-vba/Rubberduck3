using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBIntegerType : VBIntrinsicType<short>, INumericType
{
    private VBIntegerType() : base(Tokens.Integer) { }

    public static VBIntegerType TypeInfo { get; } = new();

    public override VBTypedValue DefaultValue { get; } = VBIntegerValue.Zero;
    public override VBType[] ConvertsSafelyToTypes { get; }
        = [VbLongType, VbLongLongType, VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
}
