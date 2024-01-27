using System;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBDateType : VBIntrinsicType<DateTime>
{
    private VBDateType() : base(Tokens.Date) { }
    public static VBDateType TypeInfo { get; } = new();

    public override DateTime DefaultValue { get; } = new DateTime(1899, 12, 30);
    public override VBType[] ConvertsSafelyToTypes { get; } = [VbStringType, VbVariantType];
}
