using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

public class DocumentMembersPipeline : WorkspaceDocumentSection
{
    private readonly PipelineParseTreeSymbolsService _symbolsService;

    public DocumentMembersPipeline(DataflowPipeline parent, IWorkspaceService workspaces, PipelineParseTreeSymbolsService symbolsService,
        ILogger<WorkspaceParserPipeline> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
        : base(parent, workspaces, logger, settingsProvider, performance)
    {
        _symbolsService = symbolsService;
    }

    private TransformBlock<SourceFileDocumentState, Symbol> AcquireDocumentStateSymbolsBlock { get; set; } = null!;
    private Symbol AcquireDocumentStateSymbols(SourceFileDocumentState state) =>
        RunTransformBlock(AcquireDocumentStateSymbolsBlock, state, e => e.Symbol ?? throw new InvalidOperationException("Document.Symbol is unexpectedly null."));

    private TransformBlock<Symbol, Symbol> ResolveMemberSymbolsBlock { get; set; } = null!;
    private Symbol ResolveMemberSymbols(Symbol symbol) =>
        RunTransformBlock(ResolveMemberSymbolsBlock, symbol, e => _symbolsService.RecursivelyResolveSymbols(e));

    private ActionBlock<Symbol> SetDocumentStateMemberSymbolsBlock { get; set; } = null!;
    private void SetDocumentStateMemberSymbols(Symbol symbol) =>
        RunActionBlock(SetDocumentStateMemberSymbolsBlock, symbol, e => State = (DocumentParserState)State.WithSymbol(e));

    protected override (IEnumerable<IDataflowBlock>, Task) DefinePipelineBlocks(ISourceBlock<DocumentParserState> source)
    {
        AcquireDocumentStateSymbolsBlock = new(AcquireDocumentStateSymbols, ConcurrentExecutionOptions(Token));
        TraceBlockCompletion(nameof(AcquireDocumentStateSymbolsBlock), AcquireDocumentStateSymbolsBlock);

        ResolveMemberSymbolsBlock = new(ResolveMemberSymbols, ConcurrentExecutionOptions(Token));
        TraceBlockCompletion(nameof(ResolveMemberSymbols), ResolveMemberSymbolsBlock);

        SetDocumentStateMemberSymbolsBlock = new(SetDocumentStateMemberSymbols, ConcurrentExecutionOptions(Token));
        TraceBlockCompletion(nameof(SetDocumentStateMemberSymbolsBlock), SetDocumentStateMemberSymbolsBlock);

        Link(source, AcquireDocumentStateSymbolsBlock);
        Link(AcquireDocumentStateSymbolsBlock, ResolveMemberSymbolsBlock);
        Link(ResolveMemberSymbolsBlock, SetDocumentStateMemberSymbolsBlock);

        return (new IDataflowBlock[] {
                AcquireDocumentStateSymbolsBlock,
                ResolveMemberSymbolsBlock,
                SetDocumentStateMemberSymbolsBlock
            }, SetDocumentStateMemberSymbolsBlock.Completion);
    }
}
