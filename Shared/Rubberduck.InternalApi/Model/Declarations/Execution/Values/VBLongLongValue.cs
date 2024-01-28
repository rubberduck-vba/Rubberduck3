using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBLongLongValue : VBTypedValue, IVBTypedValue<long>
{
    public VBLongLongValue(TypedSymbol? declarationSymbol = null) 
        : base(VBLongLongType.TypeInfo, declarationSymbol) { }

    public long CurrentValue { get; } = default;
    public long DefaultValue { get; } = default;
}
