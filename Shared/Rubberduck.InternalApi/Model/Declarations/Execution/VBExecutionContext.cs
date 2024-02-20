using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Execution;

public class VBExecutionContext : ServiceBase, IDiagnosticSource
{
    private readonly Stack<VBExecutionScope> _callStack = new();
    private readonly ConcurrentDictionary<TypedSymbol, VBTypedValue> _symbols = new();

    public VBExecutionContext(ILogger logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance) 
        : base(logger, settingsProvider, performance)
    {
    }

    public int LanguageVersion { get; set; } = 7;
    public bool Is64BitHost { get; set; }

    public void AddTypedSymbol(TypedSymbol symbol) => _symbols.TryAdd(symbol, null!);
    public void AddDiagnostic(Diagnostic diagnostic) => Diagnostics.Add(diagnostic);

    public VBExecutionScope EnterScope(VBTypeMember member)
    {
        var scope = new VBExecutionScope(_callStack, _symbols.Select(e => (e.Key, e.Value)).ToDictionary(e => (Symbol)e.Key, e => (VBTypedValue)e.Value), member)
        {
            Is64BitHost = Is64BitHost,
        };
        _callStack.Push(scope);
        return scope;
    }

    public VBExecutionScope CurrentScope => _callStack.Peek();

    public void End() => _callStack.Clear();

    public VBExecutionScope? ExitScope(VBRuntimeErrorException? error = null)
    {
        _callStack.Pop();
        if (!_callStack.Any() && error != null)
        {
            // issue unhandled error diagnostic?
            End();
            return null;
        }

        return CurrentScope;
    }

    public VBTypeMember? GetModuleMember(Symbol symbol) => 
        _symbols.Keys
            .Where(e => e is ClassModuleSymbol || e is StandardModuleSymbol)
            .SelectMany(e => ((VBMemberOwnerType)e.ResolvedType!).Members)
            .SingleOrDefault(e => e.Declaration == symbol);

    /// <summary>
    /// Gets all resolved symbols in the context.
    /// </summary>
    public ImmutableHashSet<TypedSymbol> ResolvedSymbols => _symbols.Keys.Where(e => e.ResolvedType != null).ToImmutableHashSet();

    /// <summary>
    /// Gets all <c>VBMemberOwnerType</c> data types in the context.
    /// </summary>
    public ImmutableHashSet<TypedSymbol> MemberOwnerTypes => _symbols.Keys.Where(e => e.ResolvedType is VBMemberOwnerType).ToImmutableHashSet();

    public ConcurrentBag<Diagnostic> Diagnostics { get; init; } = [];

    IEnumerable<Diagnostic> IDiagnosticSource.Diagnostics => Diagnostics.ToArray();

    /// <summary>
    /// Associates the specified value to a symbol, registering the provided executable symbol as a write site.
    /// </summary>
    /// <remarks>
    /// Local symbols without write sites should be considered unassigned.
    /// </remarks>
    //public void WriteSymbolValue(TypedSymbol symbol, VBTypedValue value, IExecutable site) => _symbols[symbol] = value.WithWriteSite(site);
    /// <summary>
    /// Returns the currently held value of the specified symbol, registering the provided executable symbol as a read site.
    /// </summary>
    /// <remarks>
    /// Local symbols without read sites should be considered unused.
    /// </remarks>
    //public VBTypedValue ReadSymbolValue(TypedSymbol symbol, IExecutable site) => _symbols[symbol] = _symbols[symbol].WithReadSite(site);

    /// <summary>
    /// Gets the currently held value of the specified symbol.
    /// </summary>
    public VBTypedValue GetSymbolValue(TypedSymbol symbol) => _symbols[symbol];
    /// <summary>
    /// Sets the currently held value of the specified symbol.
    /// </summary>
    public void SetSymbolValue(TypedSymbol symbol, VBTypedValue value) => _symbols[symbol] = value;
}
