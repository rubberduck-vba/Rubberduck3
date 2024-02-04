using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public interface IVBTypedValue<TValue>
{
    TValue Value { get; }
    TValue DefaultValue { get; }
}

public record class VBTypedValue
{
    public VBTypedValue(VBType type, TypedSymbol? symbol)
    {
        TypeInfo = type;
        Symbol = symbol;
    }

    public IExecutable[] Writes { get; init; } = [];
    public IExecutable[] Reads { get; init; } = [];

    public TypedSymbol? Symbol { get; init; }
    public VBType TypeInfo { get; init; }

    public VBTypedValue WithWriteSite(IExecutable site) => this with { Writes = [.. Writes, site] };
    public VBTypedValue WithReadSite(IExecutable site) => this with { Reads = [.. Reads, site] };
}

public record class VBTypeDescValue : VBTypedValue
{
    public VBTypeDescValue(TypedSymbol symbol) : base(symbol.ResolvedType!, symbol)
    {
    }
}