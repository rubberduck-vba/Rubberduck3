using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBSingleValue : VBTypedValue, IVBTypedValue<float>
{
    public VBSingleValue(TypedSymbol? declarationSymbol = null) 
        : base(VBSingleType.TypeInfo, declarationSymbol) { }

    public float CurrentValue { get; } = default;
    public float DefaultValue { get; } = default;
}
