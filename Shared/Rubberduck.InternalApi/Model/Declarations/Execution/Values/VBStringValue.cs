using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBStringValue : VBTypedValue, IVBTypedValue<string?>
{
    public static VBStringValue VBNullString { get; } = new VBStringValue();

    public VBStringValue(TypedSymbol? declarationSymbol = null) 
        : base(VBStringType.TypeInfo, declarationSymbol) { }

    public string? CurrentValue { get; } = default;
    public string? DefaultValue { get; } = default;
}
