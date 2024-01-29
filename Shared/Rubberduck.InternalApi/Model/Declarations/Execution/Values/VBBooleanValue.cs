using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBBooleanValue : VBTypedValue, IVBTypedValue<bool>, INumericCoercion, IStringCoercion
{
    public VBBooleanValue(TypedSymbol? declarationSymbol = null)
        : base(VBBooleanType.TypeInfo, declarationSymbol) { }

    public bool Value { get; init; } = default;
    public bool DefaultValue { get; } = default;

    public double? AsCoercedNumeric(int depth = 0) => Value ? -1 : 0;
    public string? AsCoercedString(int depth = 0) => Value ? Tokens.True : Tokens.False;
}
