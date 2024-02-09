using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBByteType : VBIntrinsicType<byte>, INumericType
{
    private VBByteType() : base(Tokens.Byte) { }

    public static VBByteType TypeInfo { get; } = new();

    public override VBByteValue DefaultValue { get; } = new();
    public override VBType[] ConvertsSafelyToTypes { get; }
        = [VbIntegerType, VbLongType, VbLongLongType, VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
}
