using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBSingleType : VBIntrinsicType<float>, INumericType
{
    private static readonly VBSingleType _type = new();

    private VBSingleType() : base(Tokens.Single) { }
    public static VBSingleType TypeInfo => _type;

    public override VBTypedValue DefaultValue { get; } = VBSingleValue.Zero;
}
