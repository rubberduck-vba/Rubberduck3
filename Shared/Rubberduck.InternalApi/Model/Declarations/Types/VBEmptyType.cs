using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBEmptyType : VBIntrinsicType<int?>
{
    private static readonly VBEmptyType _type = new();

    private VBEmptyType() : base(Tokens.vbEmpty) { }
    public static VBEmptyType TypeInfo => _type;

    public override VBEmptyValue DefaultValue { get; } = VBEmptyValue.Empty;
    public override VBType[] ConvertsSafelyToTypes => [VBVariantType.TypeInfo];
}
