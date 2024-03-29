﻿using Antlr4.Runtime.Tree;
using Rubberduck.Parsing._v3.Pipeline.Services;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Settings.Model;

namespace Rubberduck.Parsing._v3.Pipeline;
public abstract class SyntaxTreeTraversalService
{
    private readonly RubberduckSettingsProvider _settingsProvider;

    protected SyntaxTreeTraversalService(ILogger<SyntaxTreeTraversalService> logger, RubberduckSettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
        Logger = logger;
    }

    protected ILogger Logger { get; }
    protected RubberduckSettings Settings => _settingsProvider.Settings;

    protected T TraverseTree<T>(IParseTree tree, IVBListener<T> listener)
    {
        ParseTreeWalker.Default.Walk(listener, tree);
        return listener.Result ?? throw new InvalidOperationException($"{listener.GetType().Name}.Result was unexpectedly null.");
    }
}

public class FoldingRangesParseTreeService : SyntaxTreeTraversalService
{
    public FoldingRangesParseTreeService(ILogger<SyntaxTreeTraversalService> logger, RubberduckSettingsProvider settingsProvider)
        : base(logger, settingsProvider)
    {
    }

    public IEnumerable<FoldingRange> DiscoverFoldingRanges(IParseTree tree, WorkspaceFileUri uri) => TraverseTree(tree, new VBFoldingListener(Settings.EditorSettings.CodeFoldingSettings));
}
public class PipelineParseTreeSymbolsService : SyntaxTreeTraversalService
{
    private readonly IResolverService _resolver;
    private readonly ILogger _logger;

    public PipelineParseTreeSymbolsService(ILogger<PipelineParseTreeSymbolsService> logger, RubberduckSettingsProvider settingsProvider, IResolverService resolver)
        : base(logger, settingsProvider)
    {
        _resolver = resolver;
        _logger = logger;
    }

    /// <summary>
    /// A first pass to discover all members (and their respective parameters, as applicable).
    /// </summary>
    /// <remarks>
    /// Ignores most symbols to produce a simple representation of a module that can be returned and sent to the client immediately.
    /// </remarks>
    /// <returns>
    /// Returns a module symbol with all its members, including parameters. Types are not resolved, unless implicit or intrinsic.
    /// </returns>
    public Symbol DiscoverMemberSymbols(IParseTree tree, WorkspaceFileUri uri) => TraverseTree(tree, new MemberSymbolsListener(_logger, uri));

    /// <summary>
    /// A second pass to discover all declaration symbols in the module, including inside member scopes.
    /// </summary>
    /// <remarks>
    /// These symbols are at the same abstraction level as Rubberduck v2.x <c>Declaration</c> objects.
    /// </remarks>
    /// <returns>
    /// Returns a copy of the provided <c>moduleSymbol</c> with all its members, including parameters. Types are not resolved, unless implicit or intrinsic.
    /// </returns>
    public Symbol DiscoverHierarchicalSymbols(IParseTree tree, WorkspaceFileUri uri) => TraverseTree(tree, new HierarchicalSymbolsListener(uri));

    /// <summary>
    /// Resolves a <c>VBType</c> for all symbols in the provided module.
    /// </summary>
    public Symbol RecursivelyResolveSymbols(Symbol module) => ResolveDataType(module);

    /// <summary>
    /// Resolves the data type of the provided <c>symbol</c>.
    /// </summary>
    /// <remarks>
    /// Recursively resolves the data type of each child item unless specified otherwise.
    /// </remarks>
    /// <returns>
    /// Returns a copy of the provided <c>symbol</c> with its data type resolved, or the symbol itself if it isn't typed.
    /// </returns>
    private Symbol ResolveDataType(Symbol symbol, bool recursive = true)
    {
        VBType? type = null;

        var typedSymbol = symbol as TypedSymbol;
        if (typedSymbol != null)
        {
            type = typedSymbol.ResolvedType ?? _resolver.Resolve(typedSymbol);
        }

        if (recursive)
        {
            var children = new List<Symbol>();
            foreach (var child in symbol.Children?.OfType<Symbol>() ?? [])
            {
                children.Add(ResolveDataType(child));
            }
            return symbol with { Children = children.ToArray() };
        }

        if (typedSymbol != null && type != null)
        {
            return typedSymbol with { ResolvedType = type };
        }

        return symbol;
    }
}