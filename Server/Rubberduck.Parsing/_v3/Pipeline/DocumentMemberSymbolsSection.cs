using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System;
using System.Collections.Concurrent;
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

    private TransformBlock<DocumentState, Symbol> AcquireDocumentStateSymbolsBlock { get; set; } = null!;
    private Symbol AcquireDocumentStateSymbols(DocumentState state) =>
        RunTransformBlock(AcquireDocumentStateSymbolsBlock, state, 
            e => e.Symbol ?? throw new InvalidOperationException("Document.Symbol is unexpectedly null."), 
            nameof(AcquireDocumentStateSymbolsBlock), logPerformance: false);

    private TransformBlock<Symbol, Symbol> ResolveMemberSymbolsBlock { get; set; } = null!;
    private Symbol ResolveMemberSymbols(Symbol symbol) =>
        RunTransformBlock(ResolveMemberSymbolsBlock, symbol, 
            _symbolsService.RecursivelyResolveSymbols, 
            nameof(ResolveMemberSymbolsBlock), logPerformance: true);

    private ActionBlock<Symbol> SetDocumentStateMemberSymbolsBlock { get; set; } = null!;
    private void SetDocumentStateMemberSymbols(Symbol symbol) =>
        RunActionBlock(SetDocumentStateMemberSymbolsBlock, symbol, 
            e => State = (DocumentParserState)State.WithSymbol(e),
            nameof(SetDocumentStateMemberSymbolsBlock), logPerformance: false);

    protected override (IEnumerable<IDataflowBlock>, Task) DefineSectionBlocks(ISourceBlock<DocumentParserState> source)
    {
        AcquireDocumentStateSymbolsBlock = new(AcquireDocumentStateSymbols, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(AcquireDocumentStateSymbolsBlock), AcquireDocumentStateSymbolsBlock);

        ResolveMemberSymbolsBlock = new(ResolveMemberSymbols, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(ResolveMemberSymbols), ResolveMemberSymbolsBlock);

        var symbolBuffer = new BufferBlock<Symbol>(new DataflowBlockOptions { CancellationToken = Token });

        SetDocumentStateMemberSymbolsBlock = new(SetDocumentStateMemberSymbols, SingleMessageExecutionOptions(Token)); // NOTE: not thread-safe, keep single-threaded
        Completion = TraceBlockCompletionAsync(nameof(SetDocumentStateMemberSymbolsBlock), SetDocumentStateMemberSymbolsBlock);

        //Completion = SetDocumentStateMemberSymbolsBlock.Completion;

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

    protected override Dictionary<string, IDataflowBlock> DataflowBlocks => new()
    {
        [nameof(AcquireDocumentStateSymbolsBlock)] = AcquireDocumentStateSymbolsBlock,
        [nameof(ResolveMemberSymbolsBlock)] = ResolveMemberSymbolsBlock,
        [nameof(SetDocumentStateMemberSymbolsBlock)] = SetDocumentStateMemberSymbolsBlock,
    };

    protected override void LogAdditionalPipelineSectionCompletionInfo(StringBuilder builder, string name)
    {
        var uri = State?.Uri?.ToString();
        if (State != null && !string.IsNullOrWhiteSpace(uri))
        {
            builder.AppendLine($"\t📂 Uri: {uri} (⛔{State.SyntaxErrors.Count} errors; ⚠️{State.Diagnostics.Count} diagnostics; 🧩{State.Symbol?.Children?.Count() ?? 0} child symbols, {State.Foldings.Count} foldings)");
        }
    }
}
