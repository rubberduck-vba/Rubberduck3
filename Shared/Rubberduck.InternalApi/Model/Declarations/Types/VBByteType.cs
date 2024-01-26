using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types
{
    public record class VBByteType : VBIntrinsicType<byte>
    {
        private VBByteType() : base(Tokens.Byte) { }
        public static VBByteType TypeInfo { get; } = new();

        public override byte DefaultValue { get; }
        public override VBType[] ConvertsSafelyToTypes { get; }
            = [VbIntegerType, VbLongType, VbLongLongType, VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
    }
}
