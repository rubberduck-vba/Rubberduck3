using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBVariantType : VBIntrinsicType<object?>
{
    public static object? VBEmpty { get; } = null;

    private VBVariantType(VBType? subtype = null) : base(Tokens.Variant) 
    {
        Subtype = subtype ?? VbEmptyType;
    }

    public VBType Subtype { get; init; }
    public bool IsEmpty => Subtype == VbEmptyType;

    public static VBVariantType TypeInfo { get; } = new();

    public override bool RuntimeBinding { get; } = true;
    public override object? DefaultValue { get; } = VBEmpty;
    public override VBType[] ConvertsSafelyToTypes { get; } = [];
}

public record class VBErrorType : VBIntrinsicType<int>
{
    public const int ApplicationDefinedError = 1004;
    public static VBErrorType Default { get; } = new(ApplicationDefinedError);

    public VBErrorType(int errorNumber) : base($"Error {errorNumber}")
    {
    }

    public override VBType[] ConvertsSafelyToTypes { get; } = [VbVariantType];

    public override int DefaultValue => default;
}
