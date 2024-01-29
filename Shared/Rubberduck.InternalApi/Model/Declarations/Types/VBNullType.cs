using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBNullType : VBIntrinsicType<object?>
{
    public VBNullType() : base(Tokens.Null)
    {
        Size = 0;
    }

    public static VBNullType TypeInfo { get; } = new();

    public override object? DefaultValue => null;
    public override VBType[] ConvertsSafelyToTypes { get; } = [VbVariantType, VbEmptyType];
}