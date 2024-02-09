using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBEmptyType : VBIntrinsicType<int?>
{
    private VBEmptyType() : base(Tokens.vbEmpty) { }
    public static VBEmptyType TypeInfo { get; } = new();

    public override VBEmptyValue DefaultValue { get; } = VBEmptyValue.Empty;
    public override VBType[] ConvertsSafelyToTypes { get; }
        = [VbBooleanType, VbByteType, VbIntegerType, VbLongType, VbLongLongType, VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
}
