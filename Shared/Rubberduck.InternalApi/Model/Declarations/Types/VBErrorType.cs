using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBErrorType : VBIntrinsicType<int>
{
    private static readonly VBErrorType _type;
    static VBErrorType()
    {
        _type = new();
    }

    public VBErrorType() : base(Tokens.Error) { }

    public static VBErrorType TypeInfo => _type;

    public override VBTypedValue DefaultValue => VBErrorValue.None;
    public override VBType[] ConvertsSafelyToTypes => [VBVariantType.TypeInfo];
}
