using Antlr4.Runtime.Tree;
using Rubberduck.Parsing._v3.Pipeline.Services;
using Rubberduck.Parsing.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.Parsing._v3.Pipeline;

public interface IResolverService
{
    VBType Resolve(TypedSymbol symbol);
}

public class PipelineParserService
{
    private readonly RubberduckSettingsProvider _settingsProvider;
    private readonly IParser<string> _parser;
    private readonly IResolverService _resolver;

    public PipelineParserService(RubberduckSettingsProvider settingsProvider, IParser<string> parser, IResolverService resolver)
    {
        _settingsProvider = settingsProvider;
        _parser = parser;
        _resolver = resolver;
    }

    public PipelineParseResult ParseDocument(DocumentState state, CancellationToken token)
    {
        var settings = _settingsProvider.Settings.EditorSettings.CodeFoldingSettings;
        var foldingsListener = new VBFoldingListener(settings);
        
        token.ThrowIfCancellationRequested();

        var result = _parser.Parse(state.Uri, state.Text, token, parseListeners: [foldingsListener])
            ?? throw new InvalidOperationException("ParserResult was unexpectedly null.");

        return new()
        {
            Foldings = foldingsListener.Foldings,
            ParseResult = result,
            Uri = state.Uri
        };
    }
}

public class PipelineParseTreeSymbolsService
{
    private readonly RubberduckSettingsProvider _settingsProvider;
    private readonly IResolverService _resolver;

    public PipelineParseTreeSymbolsService(RubberduckSettingsProvider settingsProvider, IResolverService resolver)
    {
        _settingsProvider = settingsProvider;
        _resolver = resolver;
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
    public Symbol DiscoverMemberSymbols(IParseTree tree, WorkspaceFileUri uri) => TraverseTree(tree, new MemberSymbolsListener(uri));

    /// <summary>
    /// A second pass to discover all declaration symbols in the module, including inside member scopes.
    /// </summary>
    /// <remarks>
    /// These symbols are at the same abstraction level as Rubberduck v2.x <c>Declaration</c> objects.
    /// </remarks>
    /// <returns>
    /// Returns a copy of the provided <c>moduleSymbol</c> with all its members, including parameters. Types are not resolved, unless implicit or intrinsic.
    /// </returns>
    public Symbol DiscoverDeclarationSymbols(IParseTree tree, WorkspaceFileUri uri) => TraverseTree(tree, new DeclarationSymbolsListener(uri));

    /// <summary>
    /// Resolves a <c>VBType</c> for the given module symbol.
    /// </summary>
    /// <remarks>
    /// Recursively resolves child symbols' types.
    /// </remarks>
    public TypedSymbol ResolveMemberSymbols(TypedSymbol module) => ResolveDataType(module);

    private Symbol TraverseTree(IParseTree tree, IVBListener<Symbol> listener)
    {
        ParseTreeWalker.Default.Walk(listener, tree);
        return listener.Result ?? throw new InvalidOperationException($"{listener.GetType().Name}.Result was unexpectedly null.");
    }

    private SemanticToken[] TraverseTree(IParseTree tree, IVBListener<SemanticToken[]> listener)
    {
        ParseTreeWalker.Default.Walk(listener, tree);
        return listener.Result ?? throw new InvalidOperationException($"{listener.GetType().Name}.Result was unexpectedly null.");
    }

    /// <summary>
    /// Resolves the data type of the provided <c>symbol</c>.
    /// </summary>
    /// <remarks>
    /// Recursively resolves the data type of each child item unless specified otherwise.
    /// </remarks>
    /// <returns>
    /// Returns a copy of the provided <c>symbol</c> with its data type resolved.
    /// </returns>
    private TypedSymbol ResolveDataType(TypedSymbol symbol, bool recursive = true)
    {
        var type = symbol.ResolvedType ?? _resolver.Resolve(symbol);

        if (recursive)
        {
            var children = new List<Symbol>();
            foreach (var child in symbol.Children?.OfType<TypedSymbol>() ?? [])
            {
                children.Add(ResolveDataType(child));
            }
            return symbol with { Children = children.ToArray() };
        }

        return symbol with { ResolvedType = type };
    }
}