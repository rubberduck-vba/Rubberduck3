using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBLongPtrType : VBIntrinsicType<int>, INumericType
{
    public VBLongPtrType() : base(Tokens.LongPtr) 
    {
        Size = 32; // FIXME needs to be the LongPtr size of the host application process.
    }
    public static VBLongPtrType TypeInfo { get; } = new();

    public override int DefaultValue { get; }
    public override VBType[] ConvertsSafelyToTypes { get; } = [VbLongType, VbLongLongType];
}
