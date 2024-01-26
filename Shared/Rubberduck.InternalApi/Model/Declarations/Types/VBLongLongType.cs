using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types
{
    public record class VBLongLongType : VBIntrinsicType<long>
    {
        private VBLongLongType() : base(Tokens.LongLong) { }
        public static VBLongLongType TypeInfo { get; } = new();

        public override long DefaultValue { get; }
        public override VBType[] ConvertsSafelyToTypes { get; }
            = [VbDecimalType, VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
    }
}
