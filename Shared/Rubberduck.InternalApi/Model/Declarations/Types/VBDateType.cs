using System;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBDateType : VBIntrinsicType<DateTime>
{
    private VBDateType() : base(Tokens.Date) { }
    public static VBDateType TypeInfo { get; } = new();

    public override VBTypedValue DefaultValue { get; } = VBDateValue.Zero;
    public override VBType[] ConvertsSafelyToTypes { get; } = [VbStringType, VbVariantType];
}
