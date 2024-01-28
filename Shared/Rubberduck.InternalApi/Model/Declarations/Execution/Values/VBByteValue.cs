using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBByteValue : VBTypedValue, IVBTypedValue<byte>
{
    public VBByteValue(TypedSymbol? declarationSymbol = null) 
        : base(VBByteType.TypeInfo, declarationSymbol) { }

    public byte CurrentValue { get; } = default;
    public byte DefaultValue { get; } = default;
}
