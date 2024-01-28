using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBIntegerValue : VBTypedValue, IVBTypedValue<short>
{
    public VBIntegerValue(TypedSymbol? declarationSymbol = null) 
        : base(VBIntegerType.TypeInfo, declarationSymbol) { }

    public short CurrentValue { get; } = default;
    public short DefaultValue { get; } = default;
}
