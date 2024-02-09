using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBNullType : VBIntrinsicType<object?>
{
    public VBNullType() : base(Tokens.Null) { }

    public static VBNullType TypeInfo { get; } = new();
    public override bool IsDeclarable => false;

    public override VBNullValue DefaultValue { get; } = VBNullValue.Null;
    public override VBType[] ConvertsSafelyToTypes { get; } = [VbVariantType];
}

/// <summary>
/// Represents the <c>Variant</c> subtype given to an optional <c>Variant</c> parameter that was not supplied.
/// </summary>
public record class VBMissingType : VBIntrinsicType<IntPtr>
{
    public VBMissingType() : base("<missing>") { }

    public static VBMissingType TypeInfo { get; } = new();
    public override bool IsDeclarable => false;

    public override VBMissingValue DefaultValue { get; } = new();
    public override VBType[] ConvertsSafelyToTypes { get; } = [VbVariantType];
}