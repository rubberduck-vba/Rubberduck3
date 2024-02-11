using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBArrayType : VBIntrinsicType<object[]>, IEnumerableType
{
    private static readonly VBArrayType _type = new();

    public VBArrayType(VBType? subtype = null) : base("Array")
    {
        Subtype = subtype ?? VBVariantType.TypeInfo;
    }

    public static VBArrayType TypeInfo => _type;
    public VBType Subtype { get; init; }

    public bool IsArray { get; } = true;

    public override VBTypedValue DefaultValue => new VBResizableArrayValue([]);
    public override bool CanPassByValue { get; } = false;

    public override VBType[] ConvertsSafelyToTypes => [VBVariantType.TypeInfo];
}
