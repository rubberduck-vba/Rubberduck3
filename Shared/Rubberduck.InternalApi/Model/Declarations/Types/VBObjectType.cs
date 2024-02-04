using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBObjectType : VBIntrinsicType<object?>
{
    private VBObjectType() : base(Tokens.Object) 
    {
        Size = 32;
    }
    public static VBObjectType TypeInfo { get; } = new();

    public override bool RuntimeBinding { get; } = true;
    public override object? DefaultValue { get; } = VBObjectValue.Nothing;
    public override VBType[] ConvertsSafelyToTypes { get; } = [VbVariantType];
}
