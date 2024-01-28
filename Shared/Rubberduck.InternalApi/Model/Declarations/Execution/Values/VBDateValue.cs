using System;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBDateValue : VBTypedValue, IVBTypedValue<DateTime>
{
    public VBDateValue(TypedSymbol? declarationSymbol = null) 
        : base(VBDateType.TypeInfo, declarationSymbol) { }

    public DateTime CurrentValue { get; } = default;
    public DateTime DefaultValue { get; } = default;
}
