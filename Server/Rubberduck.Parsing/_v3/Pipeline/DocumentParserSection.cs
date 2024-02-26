using Antlr4.Runtime.Tree;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

public class DocumentParserSection : WorkspaceDocumentSection
{
    private readonly PipelineParserService _parser;
    private readonly PipelineParseTreeSymbolsService _symbolsService;

    public DocumentParserSection(DataflowPipeline parent, IWorkspaceService workspaces, PipelineParserService parser, PipelineParseTreeSymbolsService symbolsService,
        ILogger<WorkspaceDocumentParserOrchestrator> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
        : base(parent, workspaces, logger, settingsProvider, performance)
    {
        _parser = parser;
        _symbolsService = symbolsService;
    }

    private TransformBlock<DocumentState, PipelineParseResult> ParseDocumentTextBlock { get; set; } = null!;
    private PipelineParseResult ParseDocumentText(DocumentState documentState) =>
        RunTransformBlock(ParseDocumentTextBlock, documentState, 
            e => _parser.ParseDocument(e, Token), 
            nameof(ParseDocumentTextBlock), logPerformance: true);

    private BroadcastBlock<PipelineParseResult> BroadcastParseResultBlock { get; set; } = null!;
    private PipelineParseResult BroadcastParseResult(PipelineParseResult parseResult) =>
        RunTransformBlock(BroadcastParseResultBlock, parseResult, 
            e => e, 
            nameof(BroadcastParseResultBlock), logPerformance: false);

    private ActionBlock<PipelineParseResult> SetDocumentStateFoldingsBlock { get; set; } = null!;
    private void SetDocumentStateFoldings(PipelineParseResult parseResult) =>
        RunActionBlock(SetDocumentStateFoldingsBlock, parseResult,
            e =>
            {
                State = (DocumentParserState)State.WithFoldings(e.Foldings) ?? throw new InvalidOperationException("Document state was unexpectedly null.");
                UpdateDocumentState(State);
            },
            nameof(SetDocumentStateFoldingsBlock), logPerformance: false);

    private TransformBlock<PipelineParseResult, IParseTree> AcquireSyntaxTreeBlock { get; set; } = null!;
    private IParseTree AcquireSyntaxTree(PipelineParseResult input) =>
        RunTransformBlock(AcquireSyntaxTreeBlock, input, 
            e => e.ParseResult.Tree, 
            nameof(AcquireSyntaxTreeBlock), logPerformance: false);

    private BroadcastBlock<IParseTree> BroadcastSyntaxTreeBlock { get; set; } = null!;
    private IParseTree BroadcastSyntaxTree(IParseTree syntaxTree) =>
        RunTransformBlock(BroadcastSyntaxTreeBlock, syntaxTree, 
            e => e, 
            nameof(BroadcastSyntaxTreeBlock), logPerformance: false);

    private ActionBlock<IParseTree> SetDocumentStateSyntaxTreeBlock { get; set; } = null!;
    private void SetDocumentStateSyntaxTree(IParseTree syntaxTree) =>
        RunActionBlock(SetDocumentStateSyntaxTreeBlock, syntaxTree,
            e =>
            {
                State = State.WithSyntaxTree(e);
                UpdateDocumentState(State);
            }, 
            nameof(SetDocumentStateSyntaxTreeBlock), logPerformance: false);

    private TransformBlock<IParseTree, Symbol> DiscoverMemberSymbolsBlock { get; set; } = null!;
    private Symbol DiscoverMemberSymbols(IParseTree syntaxTree) =>
        RunTransformBlock(DiscoverMemberSymbolsBlock, syntaxTree,
            e =>  _symbolsService.DiscoverMemberSymbols(syntaxTree, State.Uri),
            nameof(DiscoverMemberSymbolsBlock), logPerformance: true);

    private ActionBlock<Symbol> SetDocumentStateMemberSymbolsBlock { get; set; } = null!;
    private void SetDocumentStateMemberSymbols(Symbol symbol) =>
        RunActionBlock(SetDocumentStateMemberSymbolsBlock, symbol,
            e =>
            {
                State = (DocumentParserState)State.WithSymbol(e);
                UpdateDocumentState(State);
            },
            nameof(SetDocumentStateMemberSymbolsBlock), logPerformance: false);

    protected override (IEnumerable<IDataflowBlock>, Task) DefineSectionBlocks(ISourceBlock<DocumentParserState> source)
    {
        ParseDocumentTextBlock = new(ParseDocumentText, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(ParseDocumentTextBlock), ParseDocumentTextBlock);

        BroadcastParseResultBlock = new(BroadcastParseResult, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(BroadcastParseResultBlock), BroadcastParseResultBlock);

        var parseResultBufferBlock = new BufferBlock<PipelineParseResult>(new DataflowBlockOptions { CancellationToken = Token });

        SetDocumentStateFoldingsBlock = new(SetDocumentStateFoldings, SingleMessageExecutionOptions(Token)); // NOTE: not thread-safe, keep single-threaded
        _ = TraceBlockCompletionAsync(nameof(SetDocumentStateFoldingsBlock), SetDocumentStateFoldingsBlock);

        AcquireSyntaxTreeBlock = new(AcquireSyntaxTree, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(AcquireSyntaxTreeBlock), AcquireSyntaxTreeBlock);

        BroadcastSyntaxTreeBlock = new(BroadcastSyntaxTree, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(BroadcastSyntaxTreeBlock), BroadcastSyntaxTreeBlock);

        var syntaxTreeBufferBlock = new BufferBlock<IParseTree>(new DataflowBlockOptions { CancellationToken = Token });

        SetDocumentStateSyntaxTreeBlock = new(SetDocumentStateSyntaxTree, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(SetDocumentStateSyntaxTreeBlock), SetDocumentStateSyntaxTreeBlock);

        DiscoverMemberSymbolsBlock = new(DiscoverMemberSymbols, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(DiscoverMemberSymbolsBlock), DiscoverMemberSymbolsBlock);

        var SymbolsBufferBlock = new BufferBlock<Symbol>(new DataflowBlockOptions { CancellationToken = Token });
        _ = TraceBlockCompletionAsync(nameof(SymbolsBufferBlock), SymbolsBufferBlock);

        SetDocumentStateMemberSymbolsBlock = new(SetDocumentStateMemberSymbols, SingleMessageExecutionOptions(Token)); // NOTE: not thread-safe, keep single-threaded
        _ = TraceBlockCompletionAsync(nameof(SetDocumentStateMemberSymbolsBlock), SetDocumentStateMemberSymbolsBlock);

        Link(source, ParseDocumentTextBlock);
        Link(ParseDocumentTextBlock, BroadcastParseResultBlock);
        Link(BroadcastParseResultBlock, parseResultBufferBlock);
        Link(BroadcastParseResultBlock, AcquireSyntaxTreeBlock);

        Link(parseResultBufferBlock, SetDocumentStateFoldingsBlock);

        Link(AcquireSyntaxTreeBlock, BroadcastSyntaxTreeBlock);
        Link(BroadcastSyntaxTreeBlock, DiscoverMemberSymbolsBlock);
        Link(BroadcastSyntaxTreeBlock, syntaxTreeBufferBlock);

        Link(syntaxTreeBufferBlock, SetDocumentStateSyntaxTreeBlock);

        Link(DiscoverMemberSymbolsBlock, SymbolsBufferBlock);
        Link(SymbolsBufferBlock, SetDocumentStateMemberSymbolsBlock);

        var completion = Task.WhenAll(DataflowBlocks.Values.Select(e => e.Completion).ToArray());

        return (new IDataflowBlock[]{
            ParseDocumentTextBlock,
            BroadcastParseResultBlock,
            SetDocumentStateFoldingsBlock,
            AcquireSyntaxTreeBlock,
            BroadcastSyntaxTreeBlock,
            SetDocumentStateMemberSymbolsBlock,
            DiscoverMemberSymbolsBlock,
            SetDocumentStateSyntaxTreeBlock
        }, completion);
    }

    protected override Dictionary<string, IDataflowBlock> DataflowBlocks => new()
    {
        [nameof(ParseDocumentTextBlock)] = ParseDocumentTextBlock,
        [nameof(BroadcastParseResultBlock)] = BroadcastParseResultBlock,
        [nameof(SetDocumentStateFoldingsBlock)] = SetDocumentStateFoldingsBlock,
        [nameof(AcquireSyntaxTreeBlock)] = AcquireSyntaxTreeBlock,
        [nameof(BroadcastSyntaxTreeBlock)] = BroadcastSyntaxTreeBlock,
        [nameof(SetDocumentStateMemberSymbolsBlock)] = SetDocumentStateMemberSymbolsBlock,
        [nameof(DiscoverMemberSymbolsBlock)] = DiscoverMemberSymbolsBlock,
        [nameof(SetDocumentStateSyntaxTreeBlock)] = SetDocumentStateSyntaxTreeBlock,
    };

    protected override void LogAdditionalPipelineSectionCompletionInfo(StringBuilder builder, string name)
    {
        var uri = State?.Uri;
        if (State != null && !string.IsNullOrWhiteSpace(uri?.ToString()))
        {
            builder.AppendLine($"\t📂 Uri: {uri} (⛔{State.SyntaxErrors.Count} errors; ⚠️{State.Diagnostics.Count} diagnostics; 🧩{State.Symbol?.Children?.Count() ?? 0} child symbols)");
        }
    }
}
