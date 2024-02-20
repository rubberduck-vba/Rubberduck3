using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

public class DocumentMemberSymbolsSection : WorkspaceDocumentSection
{
    private readonly PipelineParseTreeSymbolsService _symbolsService;

    public DocumentMemberSymbolsSection(DataflowPipeline parent, IWorkspaceService workspaces, PipelineParseTreeSymbolsService symbolsService,
        ILogger<WorkspaceDocumentParserOrchestrator> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
        : base(parent, workspaces, logger, settingsProvider, performance)
    {
        _symbolsService = symbolsService;
    }

    private TransformBlock<SourceFileDocumentState, Symbol> AcquireDocumentStateSymbolsBlock { get; set; } = null!;
    private Symbol AcquireDocumentStateSymbols(SourceFileDocumentState state) =>
        RunTransformBlock(AcquireDocumentStateSymbolsBlock, state, 
            e => e.Symbol ?? throw new InvalidOperationException("Document.Symbol is unexpectedly null."), 
            nameof(AcquireDocumentStateSymbols), logPerformance: false);

    private TransformBlock<Symbol, Symbol> ResolveMemberSymbolsBlock { get; set; } = null!;
    private Symbol ResolveMemberSymbols(Symbol symbol) =>
        RunTransformBlock(ResolveMemberSymbolsBlock, symbol, 
            e => _symbolsService.RecursivelyResolveSymbols(e), 
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

        ResolveMemberSymbolsBlock = new(ResolveMemberSymbols, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(ResolveMemberSymbols), ResolveMemberSymbolsBlock);

        var symbolBuffer = new BufferBlock<Symbol>(new DataflowBlockOptions { CancellationToken = Token });

        SetDocumentStateMemberSymbolsBlock = new(SetDocumentStateMemberSymbols, SingleMessageExecutionOptions(Token)); // NOTE: not thread-safe, keep single-threaded
        _ = TraceBlockCompletionAsync(nameof(SetDocumentStateMemberSymbolsBlock), SetDocumentStateMemberSymbolsBlock);

        Completion = SetDocumentStateMemberSymbolsBlock.Completion;

        Link(source, AcquireDocumentStateSymbolsBlock);
        Link(AcquireDocumentStateSymbolsBlock, ResolveMemberSymbolsBlock);
        Link(ResolveMemberSymbolsBlock, symbolBuffer);
        Link(symbolBuffer, SetDocumentStateMemberSymbolsBlock);

        return (new IDataflowBlock[] {
                AcquireDocumentStateSymbolsBlock,
                ResolveMemberSymbolsBlock,
                SetDocumentStateMemberSymbolsBlock
            }, Completion);
    }

    protected override ImmutableArray<(string, IDataflowBlock)> DataflowBlocks => new (string, IDataflowBlock)[]
    {
        (nameof(AcquireDocumentStateSymbolsBlock), AcquireDocumentStateSymbolsBlock),
        (nameof(ResolveMemberSymbolsBlock), ResolveMemberSymbolsBlock),
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
