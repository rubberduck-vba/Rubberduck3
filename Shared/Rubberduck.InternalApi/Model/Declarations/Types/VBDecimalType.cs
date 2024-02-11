using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBDecimalType : VBIntrinsicType<decimal>, INumericType
{
    private static readonly VBDecimalType _type = new();

    private VBDecimalType() : base(Tokens.Decimal) { }

    public static VBDecimalType TypeInfo => _type;

    public override VBTypedValue DefaultValue { get; } = VBDecimalValue.Zero;

    public override bool IsDeclarable { get; } = false; // "As Decimal" is explicitly specified as illegal.
}
