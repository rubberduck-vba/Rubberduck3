using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Execution;

public class ExecutionContext : ServiceBase
{
    private readonly Stack<ExecutionContext> _callStack;
    private readonly ImmutableArray<Symbol> _locals;
    private readonly Dictionary<TypedSymbol, VBTypedValue> _symbols = [];

    public ExecutionContext(ILogger logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance,
        Stack<ExecutionContext> callStack, 
        IEnumerable<VBTypedValue> symbolTable) 
        : base(logger, settingsProvider, performance)
    {
        _callStack = callStack;
        _symbols = symbolTable
            .Select(e => (Symbol: e.Symbol!, Value: e))
            .ToDictionary(e => e.Symbol, e => e.Value);
    }

    /// <summary>
    /// Associates the specified value to a symbol, registering the provided executable symbol as a write site.
    /// </summary>
    /// <remarks>
    /// Local symbols without write sites should be considered unassigned.
    /// </remarks>
    public void WriteSymbolValue(TypedSymbol symbol, VBTypedValue value, IExecutableSymbol site) => _symbols[symbol] = value.WithWriteSite(site);
    /// <summary>
    /// Returns the currently held value of the specified symbol, registering the provided executable symbol as a read site.
    /// </summary>
    /// <remarks>
    /// Local symbols without read sites should be considered unused.
    /// </remarks>
    public VBTypedValue ReadSymbolValue(TypedSymbol symbol, IExecutableSymbol site) => _symbols[symbol] = _symbols[symbol].WithReadSite(site);

    /// <summary>
    /// Gets the currently held value of the specified symbol.
    /// </summary>
    public VBTypedValue GetSymbolValue(TypedSymbol symbol) => _symbols[symbol];
    /// <summary>
    /// Sets the currently held value of the specified symbol.
    /// </summary>
    public void SetSymbolValue(TypedSymbol symbol, VBTypedValue value) => _symbols[symbol] = value;
}
