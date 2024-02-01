using Antlr4.Runtime.Tree;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using Rubberduck.Parsing.VBA.Parsing;
using System;
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

    public TransformBlock<WorkspaceFileUri, DocumentState> AcquireDocumentStateBlock { get; private set; } = null!;
    public TransformBlock<DocumentState, PipelineParseResult> ParseDocumentTextBlock { get; private set; } = null!;
    public BroadcastBlock<PipelineParseResult> BroadcastParseResultBlock { get; private set; } = null!;
    public ActionBlock<PipelineParseResult> SetDocumentStateFoldingsBlock { get; private set; } = null!;
    public TransformBlock<PipelineParseResult, IParseTree> AcquireSyntaxTreeBlock { get; private set; } = null!;
    public BroadcastBlock<IParseTree> BroadcastSyntaxTreeBlock { get; private set; } = null!;
    public ActionBlock<IParseTree> SetDocumentStateSyntaxTreeBlock { get; private set; } = null!;
    public TransformBlock<IParseTree, Symbol> AcquireMemberSymbolsBlock { get; private set; } = null!;
    public TransformBlock<Symbol, Symbol> ResolveMemberSymbolsBlock { get; private set; } = null!;
    public BroadcastBlock<Symbol> BroadcastMemberSymbolsBlock { get; private set; } = null!;
    public ActionBlock<Symbol> SetDocumentStateSymbolsBlock { get; private set; } = null!;
    
    public JoinBlock<IParseTree, Symbol> JoinMemberSymbolsBlock { get; private set; } = null!;
    public TransformBlock<Tuple<IParseTree, Symbol>, Symbol> AcquireHierarchicalSymbolsBlock { get; private set; } = null!;
    public TransformBlock<Symbol, Symbol> ResolveHierarchicalSymbolsBlock { get; private set; } = null!;
    public ActionBlock<Symbol> SetDocumentStateHierarchicalSymbolsBlock { get; private set; } = null!;

    protected override void SetInitialState(WorkspaceFileUri input)
    {
        State = new DocumentParserState(_contentStore.GetContent(input));
    }

    private DocumentParserState DocumentParserState => (DocumentParserState)State;

    protected override (ITargetBlock<WorkspaceFileUri> inputBlock, Task completion) DefinePipelineBlocks()
    {
        AcquireDocumentStateBlock = new(AcquireDocumentState, ExecutionOptions);
        ParseDocumentTextBlock = new(ParseDocumentText, ExecutionOptions);
        SetDocumentStateFoldingsBlock = new(SetDocumentStateFoldings, ExecutionOptions);
        BroadcastParseResultBlock = new(BroadcastParseResult, ExecutionOptions);
        AcquireSyntaxTreeBlock = new(AcquireSyntaxTree, ExecutionOptions);
        BroadcastSyntaxTreeBlock = new(BroadcastSyntaxTree, ExecutionOptions);
        SetDocumentStateSyntaxTreeBlock = new(SetDocumentStateSyntaxTree, ExecutionOptions);
        ResolveMemberSymbolsBlock = new(ResolveMemberSymbols, ExecutionOptions);
        BroadcastMemberSymbolsBlock = new(BroadcastMemberSymbols, ExecutionOptions);
        SetDocumentStateSymbolsBlock = new(SetDocumentStateSymbols, ExecutionOptions);
        JoinMemberSymbolsBlock = new(GreedyJoinExecutionOptions);
        AcquireHierarchicalSymbolsBlock = new(AcquireHierarchicalSymbols, ExecutionOptions);
        ResolveHierarchicalSymbolsBlock = new(ResolveHierarchicalSymbols, ExecutionOptions);
        SetDocumentStateHierarchicalSymbolsBlock = new(SetDocumentStateHierarchicalSymbols, ExecutionOptions);

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

    private DocumentState AcquireDocumentState(WorkspaceFileUri uri) => 
        State = RunTransformBlock(AcquireDocumentStateBlock, uri, param =>
            _contentStore.GetContent(param) ?? throw new InvalidOperationException("Document state was not found in the content store."));

    private PipelineParseResult ParseDocumentText(DocumentState documentState) =>
        RunTransformBlock(ParseDocumentTextBlock, documentState, e => _parser.ParseDocument(e, Token));

    private PipelineParseResult BroadcastParseResult(PipelineParseResult parseResult) =>
        RunTransformBlock(BroadcastParseResultBlock, parseResult, e => e);

    private void SetDocumentStateFoldings(PipelineParseResult parseResult) =>
        RunActionBlock(SetDocumentStateFoldingsBlock, parseResult, e => 
            State = State?.WithFoldings(e.Foldings) ?? throw new InvalidOperationException("Document state was unexpectedly null."));

    private IParseTree AcquireSyntaxTree(PipelineParseResult input) =>
        RunTransformBlock(AcquireSyntaxTreeBlock, input, e => e.ParseResult.Tree);

    private IParseTree BroadcastSyntaxTree(IParseTree syntaxTree) =>
        RunTransformBlock(BroadcastSyntaxTreeBlock, syntaxTree, e => e);

    private void SetDocumentStateSyntaxTree(IParseTree syntaxTree) =>
        RunActionBlock(SetDocumentStateSyntaxTreeBlock, syntaxTree, syntaxTree => State = DocumentParserState.WithParseTree(syntaxTree));

    private Symbol ResolveMemberSymbols(Symbol symbol) =>
        RunTransformBlock(ResolveMemberSymbolsBlock, symbol, e => _symbolsService.RecursivelyResolveSymbols(e));

    private Symbol BroadcastMemberSymbols(Symbol symbol) =>
        RunTransformBlock(BroadcastMemberSymbolsBlock, symbol, e => e);

    private void SetDocumentStateSymbols(Symbol symbol) =>
        RunActionBlock(SetDocumentStateSymbolsBlock, symbol, e => State = DocumentParserState.WithHierarchicalSymbols(e));

    private Symbol AcquireHierarchicalSymbols(Tuple<IParseTree, Symbol> input) =>
        RunTransformBlock(AcquireHierarchicalSymbolsBlock, input.Item1, e => _symbolsService.DiscoverHierarchicalSymbols(e, State.Uri));

    private Symbol ResolveHierarchicalSymbols(Symbol symbol) =>
        RunTransformBlock(ResolveHierarchicalSymbolsBlock, symbol, e => _symbolsService.RecursivelyResolveSymbols(e));

    private void SetDocumentStateHierarchicalSymbols(Symbol symbol) =>
        RunActionBlock(SetDocumentStateSymbolsBlock, symbol, e => State = DocumentParserState.WithHierarchicalSymbols(e));
}
