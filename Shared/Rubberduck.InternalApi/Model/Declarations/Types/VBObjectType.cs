using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types
{
    public record class VBObjectType : VBIntrinsicType<object?>
    {
        public static object? Nothing { get; } = null;

        private VBObjectType() : base(Tokens.Object) { }
        public static VBObjectType TypeInfo { get; } = new();

        public override bool RuntimeBinding { get; } = true;
        public override object? DefaultValue { get; } = Nothing;
        public override VBType[] ConvertsSafelyToTypes { get; } = [VbVariantType];
    }
}
