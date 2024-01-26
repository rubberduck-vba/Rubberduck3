using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types
{
    public record class VBStringType : VBIntrinsicType<string?>
    {
        public static string? VBNullString { get; } = null;

        private VBStringType() : base(Tokens.String) { }
        public static VBStringType TypeInfo { get; } = new();

        public override string? DefaultValue { get; } = VBNullString;
        public override VBType[] ConvertsSafelyToTypes { get; } = [VbVariantType];
    }
}
