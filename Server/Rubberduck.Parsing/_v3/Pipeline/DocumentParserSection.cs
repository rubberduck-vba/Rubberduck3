using Antlr4.Runtime.Tree;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

public class DocumentParserSection : WorkspaceDocumentSection
{
    private readonly PipelineParserService _parser;
    private readonly FoldingRangesParseTreeService _foldingRangesService;
    private readonly PipelineParseTreeSymbolsService _symbolsService;
    private readonly ILanguageServer _server;

    public DocumentParserSection(DataflowPipeline parent, 
        IWorkspaceService workspaces, 
        PipelineParserService parser,
        FoldingRangesParseTreeService foldingRangesService,
        PipelineParseTreeSymbolsService symbolsService,
        ILanguageServer server,
        ILogger<WorkspaceDocumentParserOrchestrator> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
        : base(parent, workspaces, logger, settingsProvider, performance)
    {
        _parser = parser;
        _foldingRangesService = foldingRangesService;
        _symbolsService = symbolsService;
        _server = server;
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

    private ActionBlock<PipelineParseResult> SetDocumentStateBlock { get; set; } = null!;
    private void SetDocumentState(PipelineParseResult parseResult) =>
        RunActionBlock(SetDocumentStateBlock, parseResult,
            e =>
            {
                UpdateDocumentState(State, state => (DocumentParserState)state
                    .WithSyntaxErrors(parseResult.ParseResult.SyntaxErrors));
                PublishDiagnostics(State);
            },
            nameof(SetDocumentStateBlock), logPerformance: false);

    private void PublishDiagnostics(DocumentParserState state)
    {
        if (state.Diagnostics.Count > 0)
        {
            LogInformation($"💡 Publishing {state.Diagnostics.Count} document diagnostics.", $"\n{string.Join("\n", state.Diagnostics.Select(e => $"\t[{e.Code!.Value.String}] {e.Message} ({e.Severity.ToString()!.ToUpperInvariant()})"))}");
            _server?.TextDocument.PublishDiagnostics(
                new()
                {
                    Uri = state.Uri.AbsoluteLocation.LocalPath,
                    Version = state.Version,
                    Diagnostics = new(state.Diagnostics)
                });
        }
    }

    private TransformBlock<PipelineParseResult, IParseTree> AcquireSyntaxTreeBlock { get; set; } = null!;
    private IParseTree AcquireSyntaxTree(PipelineParseResult input) =>
        RunTransformBlock(AcquireSyntaxTreeBlock, input, 
            e => e.ParseResult.SyntaxTree, 
            nameof(AcquireSyntaxTreeBlock), logPerformance: false);

    private BroadcastBlock<IParseTree> BroadcastSyntaxTreeBlock { get; set; } = null!;
    private IParseTree BroadcastSyntaxTree(IParseTree syntaxTree) =>
        RunTransformBlock(BroadcastSyntaxTreeBlock, syntaxTree, e => e, 
            nameof(BroadcastSyntaxTreeBlock), logPerformance: false);

    private ActionBlock<IParseTree> SetDocumentStateSyntaxTreeBlock { get; set; } = null!;
    private void SetDocumentStateSyntaxTree(IParseTree syntaxTree) =>
        RunActionBlock(SetDocumentStateSyntaxTreeBlock, syntaxTree,
            e => UpdateDocumentState(State, state => state.WithSyntaxTree(e)), 
            nameof(SetDocumentStateSyntaxTreeBlock), logPerformance: false);

    private TransformBlock<IParseTree, IEnumerable<FoldingRange>> DiscoverFoldingRangesBlock { get; set; } = null!;
    private IEnumerable<FoldingRange> DiscoverFoldingRanges(IParseTree syntaxTree) =>
        RunTransformBlock(DiscoverFoldingRangesBlock, syntaxTree,
            e => _foldingRangesService.DiscoverFoldingRanges(e, State.Uri),
            nameof(DiscoverFoldingRangesBlock), logPerformance: true);

    private ActionBlock<IEnumerable<FoldingRange>> SetDocumentStateFoldingsBlock { get; set; } = null!;
    private void SetDocumentStateFoldings(IEnumerable<FoldingRange> parseResult) =>
        RunActionBlock(SetDocumentStateFoldingsBlock, parseResult,
            e => UpdateDocumentState(State, state => (DocumentParserState)state.WithFoldings(e)
                ?? throw new InvalidOperationException("Document state was unexpectedly null.")),
            nameof(SetDocumentStateFoldingsBlock), logPerformance: false);

    private TransformBlock<IParseTree, Symbol> DiscoverMemberSymbolsBlock { get; set; } = null!;
    private Symbol DiscoverMemberSymbols(IParseTree syntaxTree) =>
        RunTransformBlock(DiscoverMemberSymbolsBlock, syntaxTree,
            e =>  _symbolsService.DiscoverMemberSymbols(syntaxTree, State.Uri),
            nameof(DiscoverMemberSymbolsBlock), logPerformance: true);

    private ActionBlock<Symbol> SetDocumentStateMemberSymbolsBlock { get; set; } = null!;
    private void SetDocumentStateMemberSymbols(Symbol symbol) =>
        RunActionBlock(SetDocumentStateMemberSymbolsBlock, symbol,
            e => UpdateDocumentState(State, state => (DocumentParserState)state.WithSymbol(e)),
            nameof(SetDocumentStateMemberSymbolsBlock), logPerformance: false);

    protected override (IEnumerable<IDataflowBlock>, Task) DefineSectionBlocks(ISourceBlock<DocumentParserState> source)
    {
        ParseDocumentTextBlock = new(ParseDocumentText, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(ParseDocumentTextBlock), ParseDocumentTextBlock);

        BroadcastParseResultBlock = new(BroadcastParseResult, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(BroadcastParseResultBlock), BroadcastParseResultBlock);

        SetDocumentStateBlock = new(SetDocumentState, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(SetDocumentStateBlock), SetDocumentStateBlock);

        var parseResultBufferBlock = new BufferBlock<PipelineParseResult>(new DataflowBlockOptions { CancellationToken = Token });

        SetDocumentStateFoldingsBlock = new(SetDocumentStateFoldings, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(SetDocumentStateFoldingsBlock), SetDocumentStateFoldingsBlock);

        AcquireSyntaxTreeBlock = new(AcquireSyntaxTree, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(AcquireSyntaxTreeBlock), AcquireSyntaxTreeBlock);

        BroadcastSyntaxTreeBlock = new(BroadcastSyntaxTree, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(BroadcastSyntaxTreeBlock), BroadcastSyntaxTreeBlock);

        var syntaxTreeBufferBlock = new BufferBlock<IParseTree>(new DataflowBlockOptions { CancellationToken = Token });

        DiscoverFoldingRangesBlock = new(DiscoverFoldingRanges, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(DiscoverFoldingRangesBlock), DiscoverFoldingRangesBlock);

        SetDocumentStateSyntaxTreeBlock = new(SetDocumentStateSyntaxTree, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(SetDocumentStateSyntaxTreeBlock), SetDocumentStateSyntaxTreeBlock);

        DiscoverMemberSymbolsBlock = new(DiscoverMemberSymbols, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(DiscoverMemberSymbolsBlock), DiscoverMemberSymbolsBlock);

        var SymbolsBufferBlock = new BufferBlock<Symbol>(new DataflowBlockOptions { CancellationToken = Token });
        _ = TraceBlockCompletionAsync(nameof(SymbolsBufferBlock), SymbolsBufferBlock);

        SetDocumentStateMemberSymbolsBlock = new(SetDocumentStateMemberSymbols, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(SetDocumentStateMemberSymbolsBlock), SetDocumentStateMemberSymbolsBlock);

        Link(source, ParseDocumentTextBlock);
        Link(ParseDocumentTextBlock, BroadcastParseResultBlock);
        Link(BroadcastParseResultBlock, parseResultBufferBlock);
        Link(BroadcastParseResultBlock, AcquireSyntaxTreeBlock);
        
        Link(AcquireSyntaxTreeBlock, BroadcastSyntaxTreeBlock);
        Link(BroadcastParseResultBlock, SetDocumentStateBlock);
        Link(BroadcastSyntaxTreeBlock, DiscoverFoldingRangesBlock);
        Link(BroadcastSyntaxTreeBlock, DiscoverMemberSymbolsBlock);
        Link(BroadcastSyntaxTreeBlock, syntaxTreeBufferBlock);

        Link(syntaxTreeBufferBlock, SetDocumentStateSyntaxTreeBlock);

        Link(DiscoverFoldingRangesBlock, SetDocumentStateFoldingsBlock);
        Link(DiscoverMemberSymbolsBlock, SymbolsBufferBlock);
        Link(SymbolsBufferBlock, SetDocumentStateMemberSymbolsBlock);

        var completion = Task.WhenAll(DataflowBlocks.Values.Select(e => e.Completion).ToArray());

        return (new IDataflowBlock[]{
            ParseDocumentTextBlock,
            BroadcastParseResultBlock,
            SetDocumentStateFoldingsBlock,
            AcquireSyntaxTreeBlock,
            BroadcastSyntaxTreeBlock,
            DiscoverMemberSymbolsBlock,
            DiscoverFoldingRangesBlock,
            SetDocumentStateMemberSymbolsBlock,
            SetDocumentStateFoldingsBlock,
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
            builder.AppendLine($"\t📂 Uri: {uri} (⚠️{State.Diagnostics.Count} diagnostics; 🧩{State.Symbol?.Children?.Count() ?? 0} child symbols, {State.Foldings.Count} foldings)");
        }
    }
}
