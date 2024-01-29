using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBErrorType : VBIntrinsicType<int>
{
    public const int ApplicationDefinedError = 1004;
    public static VBErrorType TypeInfo { get; } = new(ApplicationDefinedError);

    public VBErrorType(int errorNumber) : base($"Error {errorNumber}")
    {
        Size = 32;
    }

    public override VBType[] ConvertsSafelyToTypes { get; } = [VbVariantType];

    public override int DefaultValue => default;
}
