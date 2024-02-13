using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

public class HierarchicalSymbolsPipeline : WorkspaceDocumentPipeline
{
    private readonly PipelineParseTreeSymbolsService _symbolsService;

    public HierarchicalSymbolsPipeline(ILogger<WorkspaceParserPipeline> logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance,
        DocumentContentStore contentStore,
        PipelineParseTreeSymbolsService symbolsService)
        : base(logger, settingsProvider, performance, contentStore)
    {
        _symbolsService = symbolsService;
    }

    private TransformBlock<DocumentParserState, Symbol> AcquireDocumentStateSymbolsBlock { get; set; } = null!;
    private Symbol AcquireDocumentStateSymbols(DocumentParserState state) =>
        RunTransformBlock(AcquireDocumentStateSymbolsBlock, state, e => e.Symbol ?? throw new InvalidOperationException("Document.Symbol is unexpectedly null."));

    private TransformBlock<Symbol, Symbol> DiscoverHierarchicalSymbolsBlock { get; set; } = null!;
    private Symbol ResolveMemberSymbols(Symbol symbol) =>
        RunTransformBlock(DiscoverHierarchicalSymbolsBlock, symbol, e => _symbolsService.DiscoverHierarchicalSymbols(State.SyntaxTree!, State.Uri));

    private ActionBlock<Symbol> SetDocumentStateMemberSymbolsBlock { get; set; } = null!;
    private void SetDocumentStateMemberSymbols(Symbol symbol) =>
        RunActionBlock(SetDocumentStateMemberSymbolsBlock, symbol, e => State = (DocumentParserState)State.WithSymbol(e));

    protected override (ITargetBlock<DocumentParserState>, Task) DefinePipelineBlocks(ISourceBlock<DocumentParserState> source)
    {
        AcquireDocumentStateSymbolsBlock = new(AcquireDocumentStateSymbols, ConcurrentExecutionOptions);
        DiscoverHierarchicalSymbolsBlock = new(ResolveMemberSymbols, ConcurrentExecutionOptions);
        SetDocumentStateMemberSymbolsBlock = new(SetDocumentStateMemberSymbols, ConcurrentExecutionOptions);

        Link(source, AcquireDocumentStateSymbolsBlock, WithCompletionPropagation);
        Link(AcquireDocumentStateSymbolsBlock, DiscoverHierarchicalSymbolsBlock, WithCompletionPropagation);
        Link(DiscoverHierarchicalSymbolsBlock, SetDocumentStateMemberSymbolsBlock, WithCompletionPropagation);

        return (AcquireDocumentStateSymbolsBlock, SetDocumentStateMemberSymbolsBlock.Completion);
    }
}
