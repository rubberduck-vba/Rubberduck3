using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBVariantType : VBIntrinsicType<object?>
{
    private VBVariantType(VBType? subtype = null) : base(Tokens.Variant) 
    {
        Subtype = subtype ?? VbEmptyType;
    }

    public VBType Subtype { get; init; }
    public bool IsEmpty => Subtype == VbEmptyType;

    public static VBVariantType TypeInfo { get; } = new();

    public override bool RuntimeBinding { get; } = true;
    public override VBVariantValue DefaultValue { get; } = new VBVariantValue(VBEmptyType.TypeInfo.DefaultValue);
    public override VBType[] ConvertsSafelyToTypes { get; } = [];
}
