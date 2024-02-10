using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBLongLongType : VBIntrinsicType<long>, INumericType
{
    private static readonly VBLongLongType _type = new();

    private VBLongLongType() : base(Tokens.LongLong) { }

    public static VBLongLongType TypeInfo => _type;

    public override VBTypedValue DefaultValue { get; } = VBLongLongValue.Zero;
}
