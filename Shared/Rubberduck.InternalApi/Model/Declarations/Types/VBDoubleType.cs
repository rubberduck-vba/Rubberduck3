using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBDoubleType : VBIntrinsicType<double>, INumericType
{
    private static readonly VBDoubleType _type = new();

    private VBDoubleType() : base(Tokens.Double) { }

    public static VBDoubleType TypeInfo => _type;

    public override VBTypedValue DefaultValue { get; } = VBDoubleValue.Zero;
}
