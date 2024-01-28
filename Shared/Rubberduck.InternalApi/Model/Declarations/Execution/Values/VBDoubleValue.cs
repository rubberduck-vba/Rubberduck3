using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBDoubleValue : VBTypedValue, IVBTypedValue<double>
{
    public VBDoubleValue(TypedSymbol? declarationSymbol = null) 
        : base(VBDoubleType.TypeInfo, declarationSymbol) { }

    public double CurrentValue { get; } = default;
    public double DefaultValue { get; } = default;
}
