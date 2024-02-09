using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBErrorType : VBIntrinsicType<int>
{
    public VBErrorType() : base(Tokens.Error) { }

    public static VBErrorType TypeInfo { get; } = new();

    public override VBType[] ConvertsSafelyToTypes { get; } = [VbVariantType];

    public override VBTypedValue DefaultValue { get; } = VBErrorValue.None;
}
