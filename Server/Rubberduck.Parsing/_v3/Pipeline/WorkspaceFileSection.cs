using Antlr4.Runtime.Tree;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

public class WorkspaceFileSection : WorkspaceDocumentSection
{
    private readonly PipelineParserService _parser;
    private readonly PipelineParseTreeSymbolsService _symbolsService;

    public WorkspaceFileSection(DataflowPipeline parent, IWorkspaceService workspaces, PipelineParserService parser, PipelineParseTreeSymbolsService symbolsService,
        ILogger<WorkspaceParserPipeline> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
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

    protected override (IEnumerable<IDataflowBlock>, Task) DefinePipelineBlocks(ISourceBlock<DocumentParserState> source)
    {
        ParseDocumentTextBlock = new(ParseDocumentText, ConcurrentExecutionOptions(Token));
        TraceBlockCompletion(nameof(ParseDocumentTextBlock), ParseDocumentTextBlock);

        BroadcastParseResultBlock = new(BroadcastParseResult, ConcurrentExecutionOptions(Token));
        TraceBlockCompletion(nameof(BroadcastParseResultBlock), BroadcastSyntaxTreeBlock);

        SetDocumentStateFoldingsBlock = new(SetDocumentStateFoldings, ConcurrentExecutionOptions(Token));
        TraceBlockCompletion(nameof(SetDocumentStateFoldingsBlock), SetDocumentStateFoldingsBlock);

        AcquireSyntaxTreeBlock = new(AcquireSyntaxTree, ConcurrentExecutionOptions(Token));
        TraceBlockCompletion(nameof(AcquireSyntaxTreeBlock), AcquireSyntaxTreeBlock);

        BroadcastSyntaxTreeBlock = new(BroadcastSyntaxTree, ConcurrentExecutionOptions(Token));
        TraceBlockCompletion(nameof(BroadcastSyntaxTreeBlock), BroadcastSyntaxTreeBlock);

        SetDocumentStateSyntaxTreeBlock = new(SetDocumentStateSyntaxTree, ConcurrentExecutionOptions(Token));
        TraceBlockCompletion(nameof(SetDocumentStateSyntaxTreeBlock), SetDocumentStateSyntaxTreeBlock);

        AcquireMemberSymbolsBlock = new(AcquireMemberSymbols, ConcurrentExecutionOptions(Token));
        TraceBlockCompletion(nameof(AcquireMemberSymbolsBlock), AcquireMemberSymbolsBlock);

        SetDocumentStateMemberSymbolsBlock = new(SetDocumentStateMemberSymbols, ConcurrentExecutionOptions(Token));
        TraceBlockCompletion(nameof(SetDocumentStateMemberSymbolsBlock), SetDocumentStateMemberSymbolsBlock);

        Link(ParseDocumentTextBlock, BroadcastParseResultBlock, WithCompletionPropagation);
        Link(BroadcastParseResultBlock, SetDocumentStateFoldingsBlock, WithCompletionPropagation);
        Link(BroadcastParseResultBlock, AcquireSyntaxTreeBlock, WithCompletionPropagation);
        Link(AcquireSyntaxTreeBlock, BroadcastSyntaxTreeBlock, WithCompletionPropagation);
        Link(BroadcastSyntaxTreeBlock, AcquireMemberSymbolsBlock, WithCompletionPropagation);
        Link(BroadcastSyntaxTreeBlock, SetDocumentStateSyntaxTreeBlock, WithCompletionPropagation);
        Link(AcquireMemberSymbolsBlock, SetDocumentStateMemberSymbolsBlock, WithCompletionPropagation);

        return (new IDataflowBlock[]{
            ParseDocumentTextBlock,
            BroadcastParseResultBlock,
            SetDocumentStateFoldingsBlock,
            AcquireSyntaxTreeBlock,
            BroadcastSyntaxTreeBlock,
            SetDocumentStateMemberSymbolsBlock,
            AcquireMemberSymbolsBlock,
            SetDocumentStateSyntaxTreeBlock
        }, SetDocumentStateMemberSymbolsBlock.Completion);
    }
}
