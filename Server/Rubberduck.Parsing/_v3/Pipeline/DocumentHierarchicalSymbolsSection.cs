using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

public class DocumentHierarchicalSymbolsSection : WorkspaceDocumentSection
{
    private readonly PipelineParseTreeSymbolsService _symbolsService;

    public DocumentHierarchicalSymbolsSection(DataflowPipeline parent, IAppWorkspacesService workspaces, PipelineParseTreeSymbolsService symbolsService,
        ILogger<WorkspaceDocumentParserOrchestrator> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
        : base(parent, workspaces, logger, settingsProvider, performance)
    {
        _symbolsService = symbolsService;
    }

    private TransformBlock<DocumentParserState, Symbol> AcquireCodeDocumentStateSymbolsBlock { get; set; } = null!;
    private Symbol AcquireCodeDocumentStateSymbols(DocumentParserState state) =>
        RunTransformBlock(AcquireCodeDocumentStateSymbolsBlock, state, 
            e => e.Symbol ?? throw new InvalidOperationException("Document.Symbol is unexpectedly null."),
            nameof(AcquireCodeDocumentStateSymbolsBlock), logPerformance: false);

    private TransformBlock<Symbol, Symbol> DiscoverHierarchicalSymbolsBlock { get; set; } = null!;
    private Symbol DiscoverHierarchicalSymbols(Symbol symbol) =>
        RunTransformBlock(DiscoverHierarchicalSymbolsBlock, symbol, 
            e => _symbolsService.DiscoverHierarchicalSymbols(State.SyntaxTree!, State.Uri),
            nameof(DiscoverHierarchicalSymbolsBlock), logPerformance: true);

    private ActionBlock<Symbol> SetCodeDocumentStateMemberSymbolsBlock { get; set; } = null!;
    private void SetCodeDocumentStateMemberSymbols(Symbol symbol) =>
        RunActionBlock(SetCodeDocumentStateMemberSymbolsBlock, symbol, 
            e => State = (DocumentParserState)State.WithSymbol(e), 
            nameof(SetCodeDocumentStateMemberSymbolsBlock), logPerformance: false);

    protected override (IEnumerable<IDataflowBlock>, Task) DefineSectionBlocks(ISourceBlock<DocumentParserState> source)
    {
        _ = source ?? throw new ArgumentNullException(nameof(source));

        AcquireCodeDocumentStateSymbolsBlock = new(AcquireCodeDocumentStateSymbols, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(AcquireCodeDocumentStateSymbolsBlock), AcquireCodeDocumentStateSymbolsBlock);

        DiscoverHierarchicalSymbolsBlock = new(DiscoverHierarchicalSymbols, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(DiscoverHierarchicalSymbolsBlock), DiscoverHierarchicalSymbolsBlock);

        var symbolsBuffer = new BufferBlock<Symbol>(new DataflowBlockOptions { CancellationToken = Token });

        SetCodeDocumentStateMemberSymbolsBlock = new(SetCodeDocumentStateMemberSymbols, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(SetCodeDocumentStateMemberSymbolsBlock), SetCodeDocumentStateMemberSymbolsBlock);

        Link(source, AcquireCodeDocumentStateSymbolsBlock);
        Link(AcquireCodeDocumentStateSymbolsBlock, DiscoverHierarchicalSymbolsBlock);
        Link(DiscoverHierarchicalSymbolsBlock, symbolsBuffer);
        Link(symbolsBuffer, SetCodeDocumentStateMemberSymbolsBlock);

        return (new IDataflowBlock[] 
        {
            AcquireCodeDocumentStateSymbolsBlock, 
            DiscoverHierarchicalSymbolsBlock,
            SetCodeDocumentStateMemberSymbolsBlock
        }, Completion);
    }

    protected override Dictionary<string, IDataflowBlock> DataflowBlocks => new()
    {
        [nameof(AcquireCodeDocumentStateSymbolsBlock)] = AcquireCodeDocumentStateSymbolsBlock,
        [nameof(DiscoverHierarchicalSymbolsBlock)] = DiscoverHierarchicalSymbolsBlock,
        [nameof(SetCodeDocumentStateMemberSymbolsBlock)] = SetCodeDocumentStateMemberSymbolsBlock,
    };

    protected override void LogAdditionalPipelineSectionCompletionInfo(StringBuilder builder, string name)
    {
        var uri = State?.Uri?.ToString();
        if (State != null && !string.IsNullOrWhiteSpace(uri))
        {
            builder.AppendLine($"\t📂 Uri: {uri} (⚠️{State.Diagnostics.Count} diagnostics; 🧩{State.Symbol?.Children?.Count() ?? 0} child symbols, {State.Foldings.Count} foldings)");
        }
    }
}
