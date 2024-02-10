using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBLongPtrType : VBIntrinsicType<int>, INumericType
{
    private static readonly VBLongPtrType _type = new();

    public VBLongPtrType() : base(Tokens.LongPtr) { }
    public static VBLongPtrType TypeInfo => _type;

    public override VBTypedValue DefaultValue { get; } = VBLongPtrValue.Zero;
    public override VBType[] ConvertsSafelyToTypes { get; } = [VBLongType.TypeInfo, VBLongLongType.TypeInfo, VBVariantType.TypeInfo];
}
