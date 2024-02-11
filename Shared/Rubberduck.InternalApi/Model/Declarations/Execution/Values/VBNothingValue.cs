using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBNothingValue : VBObjectValue,
    IVBTypedValue<VBObjectValue, Guid>
{
    public VBNothingValue(TypedSymbol? symbol = null) 
        : base(symbol) 
    {
        Value = Guid.Empty;
    }
}