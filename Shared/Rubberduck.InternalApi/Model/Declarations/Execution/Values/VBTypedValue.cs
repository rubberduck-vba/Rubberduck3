using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System.Collections.Immutable;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public interface IVBTypedValue<TValue>
{
    TValue Value { get; }
    TValue DefaultValue { get; }
}

public record class VBTypedValue
{
    public VBTypedValue(VBType type, TypedSymbol? symbol = null)
    {
        TypeInfo = type;
        Symbol = symbol;
    }

    public IExecutableSymbol[] Writes { get; init; } = [];
    public IExecutableSymbol[] Reads { get; init; } = [];

    public TypedSymbol? Symbol { get; init; }
    public VBType TypeInfo { get; init; }

    public VBTypedValue WithWriteSite(IExecutableSymbol site) => this with { Writes = [.. Writes, site] };
    public VBTypedValue WithReadSite(IExecutableSymbol site) => this with { Reads = [.. Reads, site] };
}

public record class VBTypeDescValue : VBTypedValue
{
    public VBTypeDescValue(TypedSymbol? symbol = null) 
        : base(VBTypeDesc.TypeInfo, symbol)
    {
    }
}