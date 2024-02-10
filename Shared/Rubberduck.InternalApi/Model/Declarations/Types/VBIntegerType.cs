using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBIntegerType : VBIntrinsicType<short>, INumericType
{
    private static readonly VBIntegerType _type = new();

    private VBIntegerType() : base(Tokens.Integer) { }

    public static VBIntegerType TypeInfo => _type;

    public override VBTypedValue DefaultValue { get; } = VBIntegerValue.Zero;
}
