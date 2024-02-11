using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBMissingValue : VBTypedValue
{
    public VBMissingValue(TypedSymbol? symbol = null)
        : base(VBMissingType.TypeInfo, symbol) { }

    public static VBMissingValue Missing { get; } = new VBMissingValue();

    public override int Size => sizeof(int);
}