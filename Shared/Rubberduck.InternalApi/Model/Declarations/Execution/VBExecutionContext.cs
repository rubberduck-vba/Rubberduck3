﻿using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Execution;

public record class ExecutionScope : IDiagnosticSource
{
    private readonly Stack<ExecutionScope> _callStack;
    private readonly Dictionary<Symbol, VBTypedValue> _symbols;
    
    public ExecutionScope(Stack<ExecutionScope> callStack, Dictionary<Symbol, VBTypedValue> symbolTable, VBTypeMember member, VBRuntimeErrorException? error = null, Diagnostic[]? diagnostics = null)
    {
        _callStack = callStack;
        _symbols = symbolTable;

        MemberInfo = member;
        Error = error;

        Names = symbolTable.Select(e => e.Key.Name).ToImmutableHashSet();
    }

    public bool Is64BitHost { get; init; }

    public VBTypedValue GetTypedValue(Symbol symbol) => _symbols[symbol];
    public void SetTypedValue(Symbol symbol, VBTypedValue value) => _symbols[symbol] = value;

    public ImmutableHashSet<string> Names { get; }
    public VBTypeMember MemberInfo { get; init; }

    public VBRuntimeErrorException? Error { get; init; }
    public bool ActiveOnErrorResumeNext { get; init; }
    public Symbol? ActiveOnErrorGoTo { get; init; }

    public ImmutableArray<Diagnostic> Diagnostics { get; init; }

    public ExecutionScope WithError(VBRuntimeErrorException error) => WithDiagnostics(error.Diagnostics) with { Error = error };
    public ExecutionScope WithDiagnostics(IEnumerable<Diagnostic> diagnostics) => this with { Diagnostics = Diagnostics.AddRange(diagnostics) };
}

public class VBExecutionContext : ServiceBase, IDiagnosticSource
{
    private readonly Stack<ExecutionScope> _callStack = new();
    private readonly ConcurrentDictionary<TypedSymbol, VBTypedValue> _symbols = new();

    public VBExecutionContext(ILogger logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance) 
        : base(logger, settingsProvider, performance)
    {
    }

    public void AddTypedSymbol(TypedSymbol symbol) => _symbols.TryAdd(symbol, null!);

    public ExecutionScope EnterScope(VBTypeMember member)
    {
        var scope = new ExecutionScope(_callStack, _symbols.Select(e => (e.Key, e.Value)).ToDictionary(e => (Symbol)e.Key, e => (VBTypedValue)e.Value), member);
        _callStack.Push(scope);
        return scope;
    }

    public ExecutionScope CurrentScope => _callStack.Peek();

    public void End() => _callStack.Clear();

    public ExecutionScope? ExitScope(VBRuntimeErrorException? error = null)
    {
        return _callStack.Pop();
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

    public ImmutableArray<Diagnostic> Diagnostics { get; init; } = [];

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
