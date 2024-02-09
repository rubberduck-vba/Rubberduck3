using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBStringType : VBIntrinsicType<string?>
{
    protected VBStringType() : base(Tokens.String) { }
    public static VBStringType TypeInfo { get; } = new();

    public override VBType[] ConvertsSafelyToTypes { get; } = [VbVariantType];
    public override VBTypedValue DefaultValue => VBStringValue.VBNullString;
}

public record class VBFixedStringType : VBStringType { }