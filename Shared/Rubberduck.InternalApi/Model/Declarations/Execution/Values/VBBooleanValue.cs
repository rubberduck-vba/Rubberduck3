using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBBooleanValue : VBTypedValue, IVBTypedValue<bool>
{
    public VBBooleanValue(TypedSymbol? declarationSymbol = null)
        : base(VBBooleanType.TypeInfo, declarationSymbol) { }

    public bool CurrentValue { get; } = default;
    public bool DefaultValue { get; } = default;
}
