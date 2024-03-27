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
using Rubberduck.Parsing._v3.Pipeline.Services;
using Rubberduck.Parsing.Abstract;
using System.Collections.Immutable;
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
        IAppWorkspacesService workspaces, 
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

    private TransformBlock<CodeDocumentState, PipelineParseResult> ParseDocumentTextBlock { get; set; } = null!;
    private PipelineParseResult ParseDocumentText(CodeDocumentState CodeDocumentState) =>
        RunTransformBlock(ParseDocumentTextBlock, CodeDocumentState, 
            e => _parser.ParseDocument(e, Token), 
            nameof(ParseDocumentTextBlock), logPerformance: true);

    private BroadcastBlock<PipelineParseResult> BroadcastParseResultBlock { get; set; } = null!;
    private PipelineParseResult BroadcastParseResult(PipelineParseResult parseResult) =>
        RunTransformBlock(BroadcastParseResultBlock, parseResult, 
            e => e, 
            nameof(BroadcastParseResultBlock), logPerformance: false);

    private ActionBlock<DocumentParserState> PublishDiagnosticsBlock { get; set; } = null!;
    private void PublishDiagnostics(DocumentParserState state) =>
        RunActionBlock(PublishDiagnosticsBlock, state,
            e => {
                if (e.Diagnostics.Any())
                {
                    LogInformation($"💡 Publishing {e.Diagnostics.Count} document diagnostics.", $"\n{string.Join("\n", e.Diagnostics.Select(diagnostic => $"\t[{diagnostic.Code!.Value.String}] {diagnostic.Message} ({diagnostic.Severity.ToString()!.ToUpperInvariant()})"))}");
                    _server.TextDocument.PublishDiagnostics(
                        new()
                        {
                            Uri = e.Id.Uri,
                            Version = e.Version,
                            Diagnostics = new(e.Diagnostics)
                        });
                }
            });

    private TransformBlock<PipelineParseResult, IEnumerable<FoldingRange>> AcquireFoldingRangesBlock { get; set; } = null!;
    private IEnumerable<FoldingRange> AcquireFoldingRanges(PipelineParseResult input) =>
        RunTransformBlock(AcquireFoldingRangesBlock, input,
            e => e.ParseResult.Listeners.OfType<VBFoldingListener>()
                    .SelectMany(e => e.Result)
                    .OrderBy(e => e.StartLine)
                    .ThenBy(e => e.StartCharacter)
                .ToImmutableSortedSet(),
            nameof(AcquireSyntaxTreeBlock), logPerformance: false);

    private TransformBlock<PipelineParseResult, IParseTree> AcquireSyntaxTreeBlock { get; set; } = null!;
    private IParseTree AcquireSyntaxTree(PipelineParseResult input) =>
        RunTransformBlock(AcquireSyntaxTreeBlock, input, 
            e => e.ParseResult.SyntaxTree, 
            nameof(AcquireSyntaxTreeBlock), logPerformance: false);

    private BroadcastBlock<IParseTree> BroadcastSyntaxTreeBlock { get; set; } = null!;
    private IParseTree BroadcastSyntaxTree(IParseTree syntaxTree) =>
        RunTransformBlock(BroadcastSyntaxTreeBlock, syntaxTree, e => e, 
            nameof(BroadcastSyntaxTreeBlock), logPerformance: false);

    private TransformBlock<IParseTree, IEnumerable<FoldingRange>> DiscoverFoldingRangesBlock { get; set; } = null!;
    private IEnumerable<FoldingRange> DiscoverFoldingRanges(IParseTree syntaxTree) =>
        RunTransformBlock(DiscoverFoldingRangesBlock, syntaxTree,
            e => _foldingRangesService.DiscoverFoldingRanges(e, State.Uri),
            nameof(DiscoverFoldingRangesBlock), logPerformance: true);

    private TransformBlock<IParseTree, Symbol> DiscoverMemberSymbolsBlock { get; set; } = null!;
    private Symbol DiscoverMemberSymbols(IParseTree syntaxTree) =>
        RunTransformBlock(DiscoverMemberSymbolsBlock, syntaxTree,
            e =>  _symbolsService.DiscoverMemberSymbols(syntaxTree, State.Uri),
            nameof(DiscoverMemberSymbolsBlock), logPerformance: true);

    private JoinBlock<PipelineParseResult, IEnumerable<FoldingRange>, Symbol> JoinCodeDocumentStateBlock { get; set; } = null!;

    private TransformBlock<Tuple<PipelineParseResult, IEnumerable<FoldingRange>, Symbol>, DocumentParserState> SetCodeDocumentStateBlock { get; set; } = null!;
    private DocumentParserState SetCodeDocumentState(Tuple<PipelineParseResult, IEnumerable<FoldingRange>, Symbol> joinedState) =>
        RunTransformBlock(SetCodeDocumentStateBlock, joinedState,
            e =>
            {
                var (pipelineParseResult, foldings, symbol) = e;
                var parseResult = pipelineParseResult.ParseResult;

                UpdateCodeDocumentState(State, state => state with
                {
                    SyntaxTree = parseResult.SyntaxTree,
                    Diagnostics = parseResult.Diagnostics.ToArray(),
                    Foldings = foldings.OrderBy(e => e.StartLine).ThenBy(e => e.StartCharacter).ToArray(),
                    Symbol = symbol,
                });

                return State;
            },
            nameof(SetCodeDocumentStateBlock), logPerformance: true);
    

    protected override (IEnumerable<IDataflowBlock>, Task) DefineSectionBlocks(ISourceBlock<DocumentParserState> source)
    {
        ParseDocumentTextBlock = new(ParseDocumentText, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(ParseDocumentTextBlock), ParseDocumentTextBlock);

        BroadcastParseResultBlock = new(BroadcastParseResult, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(BroadcastParseResultBlock), BroadcastParseResultBlock);

        AcquireSyntaxTreeBlock = new(AcquireSyntaxTree, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(AcquireSyntaxTreeBlock), AcquireSyntaxTreeBlock);

        BroadcastSyntaxTreeBlock = new(BroadcastSyntaxTree, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(BroadcastSyntaxTreeBlock), BroadcastSyntaxTreeBlock);

        DiscoverFoldingRangesBlock = new(DiscoverFoldingRanges, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(DiscoverFoldingRangesBlock), DiscoverFoldingRangesBlock);

        DiscoverMemberSymbolsBlock = new(DiscoverMemberSymbols, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(DiscoverMemberSymbolsBlock), DiscoverMemberSymbolsBlock);

        JoinCodeDocumentStateBlock = new(new() { Greedy = true });

        SetCodeDocumentStateBlock = new(SetCodeDocumentState, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(SetCodeDocumentStateBlock), SetCodeDocumentStateBlock);

        PublishDiagnosticsBlock = new(PublishDiagnostics, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(PublishDiagnosticsBlock), PublishDiagnosticsBlock);

        Link(source, ParseDocumentTextBlock);
        Link(ParseDocumentTextBlock, BroadcastParseResultBlock);
        Link(BroadcastParseResultBlock, AcquireSyntaxTreeBlock);
        
        Link(AcquireSyntaxTreeBlock, BroadcastSyntaxTreeBlock);
        Link(BroadcastSyntaxTreeBlock, DiscoverFoldingRangesBlock);
        Link(BroadcastSyntaxTreeBlock, DiscoverMemberSymbolsBlock);

        Link(BroadcastParseResultBlock, JoinCodeDocumentStateBlock.Target1);
        Link(DiscoverFoldingRangesBlock, JoinCodeDocumentStateBlock.Target2);
        Link(DiscoverMemberSymbolsBlock, JoinCodeDocumentStateBlock.Target3);

        Link(JoinCodeDocumentStateBlock, SetCodeDocumentStateBlock);
        Link(SetCodeDocumentStateBlock, PublishDiagnosticsBlock);

        var completion = PublishDiagnosticsBlock.Completion;

        return (new IDataflowBlock[]{
            ParseDocumentTextBlock,
            BroadcastParseResultBlock,
            AcquireSyntaxTreeBlock,
            BroadcastSyntaxTreeBlock,
            DiscoverFoldingRangesBlock,
            DiscoverMemberSymbolsBlock,
            JoinCodeDocumentStateBlock,
            SetCodeDocumentStateBlock,
            PublishDiagnosticsBlock
        }, completion);
    }

    protected override Dictionary<string, IDataflowBlock> DataflowBlocks => new()
    {
        [nameof(ParseDocumentTextBlock)] = ParseDocumentTextBlock,
        [nameof(BroadcastParseResultBlock)] = BroadcastParseResultBlock,
        [nameof(AcquireSyntaxTreeBlock)] = AcquireSyntaxTreeBlock,
        [nameof(BroadcastSyntaxTreeBlock)] = BroadcastSyntaxTreeBlock,
        [nameof(DiscoverFoldingRangesBlock)] = DiscoverFoldingRangesBlock,
        [nameof(DiscoverMemberSymbolsBlock)] = DiscoverMemberSymbolsBlock,
        [nameof(JoinCodeDocumentStateBlock)] = JoinCodeDocumentStateBlock,
        [nameof(SetCodeDocumentStateBlock)] = SetCodeDocumentStateBlock,
        [nameof(PublishDiagnosticsBlock)] = PublishDiagnosticsBlock,
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
