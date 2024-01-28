using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBCurrencyValue : VBTypedValue, IVBTypedValue<decimal>
{
    public VBCurrencyValue(TypedSymbol? declarationSymbol = null) 
        : base(VBCurrencyType.TypeInfo, declarationSymbol) { }

    public decimal CurrentValue { get; } = default;
    public decimal DefaultValue { get; } = default;
}
