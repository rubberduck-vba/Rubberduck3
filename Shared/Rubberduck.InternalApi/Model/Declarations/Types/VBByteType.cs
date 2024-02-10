using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBByteType : VBIntrinsicType<byte>, INumericType
{
    private static readonly VBByteType _type = new();

    private VBByteType() : base(Tokens.Byte) { }

    public static VBByteType TypeInfo => _type;
    public override VBByteValue DefaultValue { get; } = new();
}
