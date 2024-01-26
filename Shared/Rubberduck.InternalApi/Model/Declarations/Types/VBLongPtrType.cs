using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types
{
    public record class VBLongPtrType : VBIntrinsicType<int>
    {
        public static int Size { get; } = sizeof(int); // FIXME use bitness from client info

        public VBLongPtrType() : base(Tokens.LongPtr) { }
        public static VBLongPtrType TypeInfo { get; } = new();

        public override int DefaultValue { get; }
        public override VBType[] ConvertsSafelyToTypes { get; } = [VbLongType, VbLongLongType];
    }
}
