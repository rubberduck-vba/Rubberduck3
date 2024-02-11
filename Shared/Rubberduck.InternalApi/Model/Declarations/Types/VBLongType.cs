using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBLongType : VBIntrinsicType<int>, INumericType
{
    private static readonly VBLongType _type = new();

    private VBLongType() : base(Tokens.Long) { }
    public static VBLongType TypeInfo => _type;

    public override VBTypedValue DefaultValue { get; } = VBLongValue.Zero;
}
