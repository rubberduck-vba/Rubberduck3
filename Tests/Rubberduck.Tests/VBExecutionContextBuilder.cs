using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Rubberduck.Tests;

public class VBExecutionContextBuilder
{
    private VBExecutionContext _context;

    public VBExecutionContextBuilder(ILogger logger, RubberduckSettingsProvider settings, PerformanceRecordAggregator performance) 
    {
        _context = new(logger, settings, performance);
    }

    public VBExecutionContext Build() => _context;

    public VBExecutionContextBuilder WithSymbols(params TypedSymbol[] symbols)
    {
        foreach (var symbol in symbols)
        {
            _context.AddToSymbolTable(symbol);
        }
        return this;
    }

    public VBExecutionContextBuilder WithClassTypes(params VBClassType[] vbTypes)
    {
        foreach (var vbType in vbTypes)
        {
            _context.AddVBType(vbType);
        }
        return this;
    }
}
