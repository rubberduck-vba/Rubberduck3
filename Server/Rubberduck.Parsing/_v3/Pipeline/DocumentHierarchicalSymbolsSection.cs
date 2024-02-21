using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

public class DocumentHierarchicalSymbolsSection : WorkspaceDocumentSection
{
    private readonly PipelineParseTreeSymbolsService _symbolsService;

    public DocumentHierarchicalSymbolsSection(DataflowPipeline parent, IWorkspaceService workspaces, PipelineParseTreeSymbolsService symbolsService,
        ILogger<WorkspaceDocumentParserOrchestrator> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
        : base(parent, workspaces, logger, settingsProvider, performance)
    {
        _symbolsService = symbolsService;
    }

    private TransformBlock<DocumentParserState, Symbol> AcquireDocumentStateSymbolsBlock { get; set; } = null!;
    private Symbol AcquireDocumentStateSymbols(DocumentParserState state) =>
        RunTransformBlock(AcquireDocumentStateSymbolsBlock, state, 
            e => e.Symbol ?? throw new InvalidOperationException("Document.Symbol is unexpectedly null."),
            nameof(AcquireDocumentStateSymbols), logPerformance: false);

    private TransformBlock<Symbol, Symbol> DiscoverHierarchicalSymbolsBlock { get; set; } = null!;
    private Symbol ResolveMemberSymbols(Symbol symbol) =>
        RunTransformBlock(DiscoverHierarchicalSymbolsBlock, symbol, 
            e => _symbolsService.DiscoverHierarchicalSymbols(State.SyntaxTree!, State.Uri),
            nameof(ResolveMemberSymbols), logPerformance: true);

    private ActionBlock<Symbol> SetDocumentStateMemberSymbolsBlock { get; set; } = null!;
    private void SetDocumentStateMemberSymbols(Symbol symbol) =>
        RunActionBlock(SetDocumentStateMemberSymbolsBlock, symbol, 
            e => State = (DocumentParserState)State.WithSymbol(e), 
            nameof(SetDocumentStateMemberSymbols), logPerformance: false);

    protected override (IEnumerable<IDataflowBlock>, Task) DefineSectionBlocks(ISourceBlock<DocumentParserState> source)
    {
        AcquireDocumentStateSymbolsBlock = new(AcquireDocumentStateSymbols, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(AcquireDocumentStateSymbolsBlock), AcquireDocumentStateSymbolsBlock);

        DiscoverHierarchicalSymbolsBlock = new(ResolveMemberSymbols, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(DiscoverHierarchicalSymbolsBlock), DiscoverHierarchicalSymbolsBlock);

        var symbolsBuffer = new BufferBlock<Symbol>(new DataflowBlockOptions { CancellationToken = Token });

        SetDocumentStateMemberSymbolsBlock = new(SetDocumentStateMemberSymbols, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(SetDocumentStateMemberSymbolsBlock), SetDocumentStateMemberSymbolsBlock);

        Link(source, AcquireDocumentStateSymbolsBlock);
        Link(AcquireDocumentStateSymbolsBlock, DiscoverHierarchicalSymbolsBlock);
        Link(DiscoverHierarchicalSymbolsBlock, symbolsBuffer);
        Link(symbolsBuffer, SetDocumentStateMemberSymbolsBlock);

        return (new IDataflowBlock[] 
        {
            AcquireDocumentStateSymbolsBlock, 
            DiscoverHierarchicalSymbolsBlock,
            SetDocumentStateMemberSymbolsBlock
        }, Completion);
    }

    protected override ImmutableArray<(string, IDataflowBlock)> DataflowBlocks => new (string, IDataflowBlock)[]
    {
        (nameof(AcquireDocumentStateSymbolsBlock), AcquireDocumentStateSymbolsBlock),
        (nameof(DiscoverHierarchicalSymbolsBlock), DiscoverHierarchicalSymbolsBlock),
        (nameof(SetDocumentStateMemberSymbolsBlock), SetDocumentStateMemberSymbolsBlock),
    }.ToImmutableArray();

    public override void LogPipelineCompletionState()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"Pipeline ({GetType().Name}) completion status");
        builder.AppendLine($"\tℹ️ {(State?.Uri.ToString() ?? ("(no info)"))}");

        foreach (var (name, block) in DataflowBlocks)
        {
            builder.AppendLine($"\t{(block.Completion.IsCompletedSuccessfully ? "✔️" : block.Completion.IsFaulted ? "💀" : block.Completion.IsCanceled ? "⚠️" : "◼️")}[{name}] status: {block.Completion.Status}");
        }
        LogDebug(builder.ToString());
    }
}
