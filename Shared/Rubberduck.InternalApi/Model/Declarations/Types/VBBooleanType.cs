using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBBooleanType : VBIntrinsicType<bool>
{
    private VBBooleanType() : base(Tokens.Boolean) { }
    public static VBBooleanType TypeInfo { get; } = new();

    public override VBBooleanValue DefaultValue { get; } = new VBBooleanValue() { Value = false };
    public override VBType[] ConvertsSafelyToTypes { get; }
        = [VbByteType, VbIntegerType, VbLongType, VbLongLongType, VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
}
