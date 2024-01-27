using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBNullType : VBIntrinsicType<object?>
{
    public VBNullType() : base(Tokens.Null)
    {
    }

    public override object? DefaultValue => null;
    public override VBType[] ConvertsSafelyToTypes { get; } = [VbVariantType, VbEmptyType];
}