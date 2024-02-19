using Antlr4.Runtime.Tree;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Collections.Immutable;
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
        RunTransformBlock(ParseDocumentTextBlock, documentState, e => _parser.ParseDocument(e, Token));

    private BroadcastBlock<PipelineParseResult> BroadcastParseResultBlock { get; set; } = null!;
    private PipelineParseResult BroadcastParseResult(PipelineParseResult parseResult) =>
        RunTransformBlock(BroadcastParseResultBlock, parseResult, e => e);

    private ActionBlock<PipelineParseResult> SetDocumentStateFoldingsBlock { get; set; } = null!;
    private void SetDocumentStateFoldings(PipelineParseResult parseResult) =>
        RunActionBlock(SetDocumentStateFoldingsBlock, parseResult, e => State = (DocumentParserState)State.WithFoldings(e.Foldings) 
            ?? throw new InvalidOperationException("Document state was unexpectedly null."));

    private TransformBlock<PipelineParseResult, IParseTree> AcquireSyntaxTreeBlock { get; set; } = null!;
    private IParseTree AcquireSyntaxTree(PipelineParseResult input) =>
        RunTransformBlock(AcquireSyntaxTreeBlock, input, e => e.ParseResult.Tree);

    private BroadcastBlock<IParseTree> BroadcastSyntaxTreeBlock { get; set; } = null!;
    private IParseTree BroadcastSyntaxTree(IParseTree syntaxTree) =>
        RunTransformBlock(BroadcastSyntaxTreeBlock, syntaxTree, e => e);

    private ActionBlock<IParseTree> SetDocumentStateSyntaxTreeBlock { get; set; } = null!;
    private void SetDocumentStateSyntaxTree(IParseTree syntaxTree) =>
        RunActionBlock(SetDocumentStateSyntaxTreeBlock, syntaxTree, e => State = State.WithSyntaxTree(e));

    private TransformBlock<IParseTree, Symbol> AcquireMemberSymbolsBlock { get; set; } = null!;
    private Symbol AcquireMemberSymbols(IParseTree syntaxTree) =>
        RunTransformBlock(AcquireMemberSymbolsBlock, syntaxTree, e => _symbolsService.DiscoverMemberSymbols(syntaxTree, State.Uri));

    private ActionBlock<Symbol> SetDocumentStateMemberSymbolsBlock { get; set; } = null!;
    private void SetDocumentStateMemberSymbols(Symbol symbol) =>
        RunActionBlock(SetDocumentStateMemberSymbolsBlock, symbol, e => State = (DocumentParserState)State.WithSymbol(e));

    protected override (IEnumerable<IDataflowBlock>, Task) DefineSectionBlocks(ISourceBlock<DocumentParserState> source)
    {
        ParseDocumentTextBlock = new(ParseDocumentText, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(ParseDocumentTextBlock), ParseDocumentTextBlock);

        BroadcastParseResultBlock = new(BroadcastParseResult, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(BroadcastParseResultBlock), BroadcastParseResultBlock);

        SetDocumentStateFoldingsBlock = new(SetDocumentStateFoldings, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(SetDocumentStateFoldingsBlock), SetDocumentStateFoldingsBlock);

        AcquireSyntaxTreeBlock = new(AcquireSyntaxTree, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(AcquireSyntaxTreeBlock), AcquireSyntaxTreeBlock);

        BroadcastSyntaxTreeBlock = new(BroadcastSyntaxTree, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(BroadcastSyntaxTreeBlock), BroadcastSyntaxTreeBlock);

        SetDocumentStateSyntaxTreeBlock = new(SetDocumentStateSyntaxTree, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(SetDocumentStateSyntaxTreeBlock), SetDocumentStateSyntaxTreeBlock);

        AcquireMemberSymbolsBlock = new(AcquireMemberSymbols, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(AcquireMemberSymbolsBlock), AcquireMemberSymbolsBlock);

        SetDocumentStateMemberSymbolsBlock = new(SetDocumentStateMemberSymbols, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(SetDocumentStateMemberSymbolsBlock), SetDocumentStateMemberSymbolsBlock);

        Link(source, ParseDocumentTextBlock);
        Link(ParseDocumentTextBlock, BroadcastParseResultBlock);
        Link(BroadcastParseResultBlock, SetDocumentStateFoldingsBlock);
        Link(BroadcastParseResultBlock, AcquireSyntaxTreeBlock);

        Link(AcquireSyntaxTreeBlock, BroadcastSyntaxTreeBlock);
        Link(BroadcastSyntaxTreeBlock, AcquireMemberSymbolsBlock);
        Link(BroadcastSyntaxTreeBlock, SetDocumentStateSyntaxTreeBlock);

        Link(AcquireMemberSymbolsBlock, SetDocumentStateMemberSymbolsBlock);

        var completion = Task.WhenAll(DataflowBlocks.Select(e => e.Block.Completion).ToArray());

        return (new IDataflowBlock[]{
            ParseDocumentTextBlock,
            BroadcastParseResultBlock,
            SetDocumentStateFoldingsBlock,
            AcquireSyntaxTreeBlock,
            BroadcastSyntaxTreeBlock,
            SetDocumentStateMemberSymbolsBlock,
            AcquireMemberSymbolsBlock,
            SetDocumentStateSyntaxTreeBlock
        }, completion);
    }

    protected override ImmutableArray<(string Name, IDataflowBlock Block)> DataflowBlocks => new (string, IDataflowBlock)[]
    {
        (nameof(ParseDocumentTextBlock), ParseDocumentTextBlock),
        (nameof(BroadcastParseResultBlock), BroadcastParseResultBlock),
        (nameof(SetDocumentStateFoldingsBlock), SetDocumentStateFoldingsBlock),
        (nameof(AcquireSyntaxTreeBlock), AcquireSyntaxTreeBlock),
        (nameof(BroadcastSyntaxTreeBlock), BroadcastSyntaxTreeBlock),
        (nameof(SetDocumentStateMemberSymbolsBlock), SetDocumentStateMemberSymbolsBlock),
        (nameof(AcquireMemberSymbolsBlock), AcquireMemberSymbolsBlock),
        (nameof(SetDocumentStateSyntaxTreeBlock), SetDocumentStateSyntaxTreeBlock),
    }.ToImmutableArray();
}
