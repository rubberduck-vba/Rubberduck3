using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBVariantType : VBIntrinsicType<object?>
{
    private static readonly VBVariantType _type = new();

    private VBVariantType(VBType? subtype = null) : base(Tokens.Variant) 
    {
        Subtype = subtype ?? VBEmptyType.TypeInfo;
    }

    public VBType Subtype { get; init; }
    public bool IsEmpty => Subtype is VBEmptyType;

    public static VBVariantType TypeInfo => _type;

    public override bool RuntimeBinding { get; } = true;
    public override VBVariantValue DefaultValue => new(Subtype.DefaultValue);
    public override VBType[] ConvertsSafelyToTypes { get; } = [];
}
