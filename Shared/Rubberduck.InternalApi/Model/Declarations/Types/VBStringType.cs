using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBStringType : VBIntrinsicType<string?>
{
    private static readonly VBStringType _type = new();

    protected VBStringType() : base(Tokens.String) { }
    public static VBStringType TypeInfo => _type;

    public override VBTypedValue DefaultValue => VBStringValue.VBNullString;
    public override VBType[] ConvertsSafelyToTypes { get; } = [VBVariantType.TypeInfo];
}

public record class VBFixedStringType : VBStringType { }