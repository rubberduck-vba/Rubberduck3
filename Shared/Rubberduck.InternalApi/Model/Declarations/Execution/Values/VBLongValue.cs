using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBLongValue : VBTypedValue, IVBTypedValue<int>
{
    public VBLongValue(TypedSymbol? declarationSymbol = null) 
        : base(VBLongType.TypeInfo, declarationSymbol) { }

    public int CurrentValue { get; } = default;
    public int DefaultValue { get; } = default;
}
