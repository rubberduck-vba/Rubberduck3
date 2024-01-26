using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types
{
    public record class VBBooleanType : VBIntrinsicType<bool>
    {
        private VBBooleanType() : base(Tokens.Boolean) { }
        public static VBBooleanType TypeInfo { get; } = new();

        public override bool DefaultValue { get; }
        public override VBType[] ConvertsSafelyToTypes { get; }
            = [VbByteType, VbIntegerType, VbLongType, VbLongLongType, VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
    }
}
