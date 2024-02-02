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

/// <summary>
/// A pipeline that broadcasts parse results for a given <c>DocumentState</c>.
/// </summary>
public class WorkspaceFileParserPipeline : ParserPipeline<WorkspaceFileUri, DocumentState>
{
    private readonly DocumentContentStore _contentStore;
    private readonly PipelineParserService _parser;
    private readonly PipelineParseTreeSymbolsService _symbolsService;

    public WorkspaceFileParserPipeline(ILogger<WorkspaceParserPipeline> logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance,
        DocumentContentStore contentStore,
        PipelineParserService parser,
        PipelineParseTreeSymbolsService symbolsService) 
        : base(logger, settingsProvider, performance)
    {
        _contentStore = contentStore;
        _parser = parser;
        _symbolsService = symbolsService;
    }

    private TransformBlock<WorkspaceFileUri, DocumentState> AcquireDocumentStateBlock { get; set; } = null!;
    private DocumentState AcquireDocumentState(WorkspaceFileUri uri) =>
        State = RunTransformBlock(AcquireDocumentStateBlock, uri, e => _contentStore.GetContent(e) 
            ?? throw new InvalidOperationException("Document state was not found in the content store."));

    private TransformBlock<DocumentState, PipelineParseResult> ParseDocumentTextBlock { get; set; } = null!;
    private PipelineParseResult ParseDocumentText(DocumentState documentState) =>
        RunTransformBlock(ParseDocumentTextBlock, documentState, e => _parser.ParseDocument(e, Token));

    private BroadcastBlock<PipelineParseResult> BroadcastParseResultBlock { get; set; } = null!;
    private PipelineParseResult BroadcastParseResult(PipelineParseResult parseResult) =>
        RunTransformBlock(BroadcastParseResultBlock, parseResult, e => e);

    private ActionBlock<PipelineParseResult> SetDocumentStateFoldingsBlock { get; set; } = null!;
    private void SetDocumentStateFoldings(PipelineParseResult parseResult) =>
        RunActionBlock(SetDocumentStateFoldingsBlock, parseResult, e => State = State?.WithFoldings(e.Foldings) 
            ?? throw new InvalidOperationException("Document state was unexpectedly null."));

    private TransformBlock<PipelineParseResult, IParseTree> AcquireSyntaxTreeBlock { get; set; } = null!;
    private IParseTree AcquireSyntaxTree(PipelineParseResult input) =>
        RunTransformBlock(AcquireSyntaxTreeBlock, input, e => e.ParseResult.Tree);

    private BroadcastBlock<IParseTree> BroadcastSyntaxTreeBlock { get; set; } = null!;
    private IParseTree BroadcastSyntaxTree(IParseTree syntaxTree) =>
        RunTransformBlock(BroadcastSyntaxTreeBlock, syntaxTree, e => e);

    private ActionBlock<IParseTree> SetDocumentStateSyntaxTreeBlock { get; set; } = null!;
    private void SetDocumentStateSyntaxTree(IParseTree syntaxTree) =>
        RunActionBlock(SetDocumentStateSyntaxTreeBlock, syntaxTree, e => State = DocumentParserState.WithSyntaxTree(e));

    private TransformBlock<IParseTree, Symbol> AcquireMemberSymbolsBlock { get; set; } = null!;
    private Symbol AcquireMemberSymbols(IParseTree syntaxTree) =>
        RunTransformBlock(AcquireMemberSymbolsBlock, syntaxTree, e => _symbolsService.DiscoverMemberSymbols(syntaxTree, State.Uri));

    private TransformBlock<Symbol, Symbol> ResolveMemberSymbolsBlock { get; set; } = null!;
    private Symbol ResolveMemberSymbols(Symbol symbol) =>
        RunTransformBlock(ResolveMemberSymbolsBlock, symbol, e => _symbolsService.RecursivelyResolveSymbols(e));

    private BroadcastBlock<Symbol> BroadcastMemberSymbolsBlock { get; set; } = null!;
    private Symbol BroadcastMemberSymbols(Symbol symbol) =>
        RunTransformBlock(BroadcastMemberSymbolsBlock, symbol, e => e);

    private ActionBlock<Symbol> SetDocumentStateSymbolsBlock { get; set; } = null!;
    private void SetDocumentStateSymbols(Symbol symbol) =>
        RunActionBlock(SetDocumentStateSymbolsBlock, symbol, e => State = DocumentParserState.WithSymbols(e));


    private JoinBlock<IParseTree, Symbol> JoinMemberSymbolsBlock { get; set; } = null!;
    private Symbol AcquireHierarchicalSymbols(Tuple<IParseTree, Symbol> input) =>
        RunTransformBlock(AcquireHierarchicalSymbolsBlock, input.Item1, e => _symbolsService.DiscoverHierarchicalSymbols(e, State.Uri));

    private TransformBlock<Tuple<IParseTree, Symbol>, Symbol> AcquireHierarchicalSymbolsBlock { get; set; } = null!;
    private Symbol ResolveHierarchicalSymbols(Symbol symbol) =>
        RunTransformBlock(ResolveHierarchicalSymbolsBlock, symbol, e => _symbolsService.RecursivelyResolveSymbols(e));

    private TransformBlock<Symbol, Symbol> ResolveHierarchicalSymbolsBlock { get; set; } = null!;
    private ActionBlock<Symbol> SetDocumentStateHierarchicalSymbolsBlock { get; set; } = null!;

    private DocumentParserState DocumentParserState => (DocumentParserState)State;
    private void SetDocumentStateHierarchicalSymbols(Symbol symbol) =>
        RunActionBlock(SetDocumentStateSymbolsBlock, symbol, e => State = DocumentParserState.WithSymbols(e));

    protected override (ITargetBlock<WorkspaceFileUri> inputBlock, Task completion) DefinePipelineBlocks()
    {
        AcquireDocumentStateBlock = new(AcquireDocumentState, ConcurrentExecutionOptions);
        ParseDocumentTextBlock = new(ParseDocumentText, ConcurrentExecutionOptions);
        SetDocumentStateFoldingsBlock = new(SetDocumentStateFoldings, ConcurrentExecutionOptions);
        BroadcastParseResultBlock = new(BroadcastParseResult, ConcurrentExecutionOptions);
        AcquireSyntaxTreeBlock = new(AcquireSyntaxTree, ConcurrentExecutionOptions);
        BroadcastSyntaxTreeBlock = new(BroadcastSyntaxTree, ConcurrentExecutionOptions);
        SetDocumentStateSyntaxTreeBlock = new(SetDocumentStateSyntaxTree, ConcurrentExecutionOptions);
        AcquireMemberSymbolsBlock = new(AcquireMemberSymbols, ConcurrentExecutionOptions);
        ResolveMemberSymbolsBlock = new(ResolveMemberSymbols, ConcurrentExecutionOptions);
        BroadcastMemberSymbolsBlock = new(BroadcastMemberSymbols, ConcurrentExecutionOptions);
        SetDocumentStateSymbolsBlock = new(SetDocumentStateSymbols, ConcurrentExecutionOptions);
        JoinMemberSymbolsBlock = new(GreedyJoinExecutionOptions);
        AcquireHierarchicalSymbolsBlock = new(AcquireHierarchicalSymbols, ConcurrentExecutionOptions);
        ResolveHierarchicalSymbolsBlock = new(ResolveHierarchicalSymbols, ConcurrentExecutionOptions);
        SetDocumentStateHierarchicalSymbolsBlock = new(SetDocumentStateHierarchicalSymbols, ConcurrentExecutionOptions);

        Link(AcquireDocumentStateBlock, ParseDocumentTextBlock, WithCompletionPropagation);
        Link(ParseDocumentTextBlock, BroadcastParseResultBlock, WithCompletionPropagation);
        Link(BroadcastParseResultBlock, SetDocumentStateFoldingsBlock, WithCompletionPropagation);
        Link(BroadcastParseResultBlock, AcquireSyntaxTreeBlock, WithoutCompletionPropagation);
        Link(AcquireSyntaxTreeBlock, AcquireMemberSymbolsBlock, WithCompletionPropagation);
        Link(AcquireMemberSymbolsBlock, ResolveMemberSymbolsBlock, WithCompletionPropagation);
        Link(ResolveMemberSymbolsBlock, SetDocumentStateSymbolsBlock, WithCompletionPropagation);

        Link(BroadcastSyntaxTreeBlock, JoinMemberSymbolsBlock.Target1, WithoutCompletionPropagation);
        Link(BroadcastMemberSymbolsBlock, JoinMemberSymbolsBlock.Target2, WithCompletionPropagation);

        Link(JoinMemberSymbolsBlock, AcquireHierarchicalSymbolsBlock, WithCompletionPropagation);
        Link(AcquireHierarchicalSymbolsBlock, ResolveHierarchicalSymbolsBlock, WithCompletionPropagation);
        Link(ResolveHierarchicalSymbolsBlock, SetDocumentStateHierarchicalSymbolsBlock, WithCompletionPropagation);

        return (AcquireDocumentStateBlock, SetDocumentStateHierarchicalSymbolsBlock.Completion);
    }
}
