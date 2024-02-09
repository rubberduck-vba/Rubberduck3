using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBArrayType : VBIntrinsicType<object[]>, IEnumerableType
{
    public VBArrayType(VBType? subtype = null) : base("Array")
    {
        Subtype = subtype ?? VbVariantType;
    }

    public static VBArrayType TypeInfo { get; } = new();
    public VBType Subtype { get; init; }

    public bool IsArray { get; } = true;

    public override VBTypedValue DefaultValue => new VBResizableArrayValue([]);
    public override bool CanPassByValue { get; } = false;
}
