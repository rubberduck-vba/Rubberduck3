﻿using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Execution;

public class VBExecutionContext : ServiceBase, IDiagnosticSource
{
    private readonly Stack<VBExecutionScope> _callStack = new();

    private readonly ConcurrentDictionary<Uri, ProjectSymbol> _referencedLibraries = new();
    private readonly ConcurrentDictionary<TypedSymbol, VBTypedValue> _symbolTable = new();

    /// <summary>
    /// Last-in, first-out concurrent data structure mapping type names to possible <c>VBType</c> values to resolve class types.
    /// </summary>
    private readonly ConcurrentDictionary<string, ConcurrentStack<VBType>> _typeNames = new();

    public VBExecutionContext(ILogger logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance) 
        : base(logger, settingsProvider, performance)
    {
    }

    public int LanguageVersion { get; set; } = 7;
    public bool Is64BitHost { get; set; }

    public void AddToSymbolTable(TypedSymbol symbol)
    {
        _symbolTable.TryAdd(symbol, symbol.ResolvedType!.DefaultValue);
        if (symbol is ClassModuleSymbol classModule && classModule.ResolvedType is VBClassType vbClassType)
        {
            AddVBType(vbClassType);
        }
    }

    public VBType ResolveType(string name, WorkspaceUri scopeUri)
    {
        var workspace = scopeUri.WorkspaceRoot;
        if (_typeNames.TryGetValue(name, out var candidates))
        {
            if (candidates.Count == 1)
            {
                return candidates.Single();
            }
            else
            {
                // TODO not this
                return candidates.First();
            }
        }
        else
        {
            return VBObjectType.TypeInfo;
        }
    }

    public void AddDiagnostic(Diagnostic diagnostic) => Diagnostics.Add(diagnostic);

    public void LoadReferencedLibrarySymbols(ProjectSymbol symbol)
    {
        _referencedLibraries[symbol.Uri] = symbol;
        foreach (var vbType in symbol.Children?.OfType<ClassModuleSymbol>().Select(e => e.ResolvedType).OfType<VBClassType>() ?? [])
        {
            AddVBType(vbType);
        }
    }

    public void UnloadReferencedLibrarySymbols(WorkspaceUri uri) => _referencedLibraries.TryRemove(uri, out _);

    public void AddVBType(VBType vbType)
    {
        var key = vbType.Name;
        if (!_typeNames.ContainsKey(key))
        {
            _typeNames[key] = new ConcurrentStack<VBType>();
        }

        _typeNames[key].Push(vbType);
    }



    public VBExecutionScope EnterScope(VBTypeMember member)
    {
        var scope = new VBExecutionScope(_callStack, _symbolTable.Select(e => (e.Key, e.Value)).ToDictionary(e => (Symbol)e.Key, e => e.Value), member)
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
        _symbolTable.Keys
            .Where(e => e is ClassModuleSymbol || e is StandardModuleSymbol)
            .SelectMany(e => ((VBMemberOwnerType)e.ResolvedType!).Members)
            .SingleOrDefault(e => e.Declaration == symbol);

    /// <summary>
    /// Gets all resolved symbols in the context.
    /// </summary>
    public ImmutableHashSet<TypedSymbol> ResolvedSymbols => _symbolTable.Keys.Where(e => e.ResolvedType != null).ToImmutableHashSet();

    /// <summary>
    /// Gets all unresolved symbols in the context.
    /// </summary>
    public ImmutableHashSet<TypedSymbol> UnresolvedSymbols => _symbolTable.Keys.Where(e => e.ResolvedType is null).ToImmutableHashSet();

    /// <summary>
    /// Gets all <c>VBMemberOwnerType</c> data types in the context.
    /// </summary>
    public ImmutableHashSet<TypedSymbol> MemberOwnerTypes => _symbolTable.Keys.Where(e => e.ResolvedType is VBMemberOwnerType).ToImmutableHashSet();

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
    public VBTypedValue GetSymbolValue(TypedSymbol symbol) => _symbolTable[symbol];
    /// <summary>
    /// Sets the currently held value of the specified symbol.
    /// </summary>
    public void SetSymbolValue(TypedSymbol symbol, VBTypedValue value) => _symbolTable[symbol] = value;
}
