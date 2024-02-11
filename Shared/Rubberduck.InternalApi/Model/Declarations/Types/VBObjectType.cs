using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBObjectType : VBIntrinsicType<Guid>
{
    private static readonly VBObjectType _type = new();

    private VBObjectType() : base(Tokens.Object) { }
    public static VBObjectType TypeInfo => _type;

    public override bool RuntimeBinding { get; } = true;
    public override VBTypedValue DefaultValue { get; } = VBObjectValue.Nothing;
    public override VBType[] ConvertsSafelyToTypes { get; } = [VBVariantType.TypeInfo];
}
