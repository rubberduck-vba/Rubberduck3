using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

public class HierarchicalSymbolsPipeline : WorkspaceDocumentSection
{
    private readonly PipelineParseTreeSymbolsService _symbolsService;

    public HierarchicalSymbolsPipeline(DataflowPipeline parent, IWorkspaceService workspaces, PipelineParseTreeSymbolsService symbolsService,
        ILogger<WorkspaceParserPipeline> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
        : base(parent, workspaces, logger, settingsProvider, performance)
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

    protected override (IEnumerable<IDataflowBlock>, Task) DefinePipelineBlocks(ISourceBlock<DocumentParserState> source)
    {
        AcquireDocumentStateSymbolsBlock = new(AcquireDocumentStateSymbols, ConcurrentExecutionOptions(Token));
        DiscoverHierarchicalSymbolsBlock = new(ResolveMemberSymbols, ConcurrentExecutionOptions(Token));
        SetDocumentStateMemberSymbolsBlock = new(SetDocumentStateMemberSymbols, ConcurrentExecutionOptions(Token));

        Link(source, AcquireDocumentStateSymbolsBlock, WithCompletionPropagation);
        Link(AcquireDocumentStateSymbolsBlock, DiscoverHierarchicalSymbolsBlock, WithCompletionPropagation);
        Link(DiscoverHierarchicalSymbolsBlock, SetDocumentStateMemberSymbolsBlock, WithCompletionPropagation);

        return (new IDataflowBlock[] {AcquireDocumentStateSymbolsBlock, DiscoverHierarchicalSymbolsBlock}, SetDocumentStateMemberSymbolsBlock.Completion);
    }
}
