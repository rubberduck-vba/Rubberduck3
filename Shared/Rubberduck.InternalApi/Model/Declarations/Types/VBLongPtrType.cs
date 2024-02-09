using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBLongPtrType : VBIntrinsicType<int>, INumericType
{
    public VBLongPtrType() : base(Tokens.LongPtr) { }
    public static VBLongPtrType TypeInfo { get; } = new();

    public override VBTypedValue DefaultValue { get; } = VBLongPtrValue.Zero;
    public override VBType[] ConvertsSafelyToTypes { get; } = [VbLongType, VbLongLongType];
}
