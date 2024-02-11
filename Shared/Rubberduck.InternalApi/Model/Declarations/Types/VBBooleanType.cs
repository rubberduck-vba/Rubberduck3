using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBBooleanType : VBIntrinsicType<bool>
{
    private static readonly VBBooleanType _type = new();

    private VBBooleanType() : base(Tokens.Boolean) { }
    public static VBBooleanType TypeInfo => _type;

    public override VBBooleanValue DefaultValue { get; } = new VBBooleanValue() { Value = false };
    public override VBType[] ConvertsSafelyToTypes =>
        [
            VBByteType.TypeInfo,
            VBIntegerType.TypeInfo,
            VBLongType.TypeInfo,
            VBLongLongType.TypeInfo,
            VBDecimalType.TypeInfo,
            VBCurrencyType.TypeInfo,
            VBSingleType.TypeInfo,
            VBDoubleType.TypeInfo,
            VBStringType.TypeInfo,
            VBVariantType.TypeInfo
        ];
}
