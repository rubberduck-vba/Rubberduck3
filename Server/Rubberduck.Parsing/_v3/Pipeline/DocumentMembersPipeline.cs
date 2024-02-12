using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

public class DocumentMembersPipeline : WorkspaceDocumentPipeline
{
    private readonly PipelineParseTreeSymbolsService _symbolsService;

    public DocumentMembersPipeline(ILogger<WorkspaceParserPipeline> logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance, 
        DocumentContentStore contentStore,
        PipelineParseTreeSymbolsService symbolsService)
        : base(logger, settingsProvider, performance, contentStore)
    {
        _symbolsService = symbolsService;
    }

    private TransformBlock<DocumentState, Symbol> AcquireDocumentStateSymbolsBlock { get; set; } = null!;
    private Symbol AcquireDocumentStateSymbols(DocumentState state) =>
        RunTransformBlock(AcquireDocumentStateSymbolsBlock, state, e => e.Symbols ?? throw new InvalidOperationException("Document.Symbols is unexpectedly null."));

    private TransformBlock<Symbol, Symbol> ResolveMemberSymbolsBlock { get; set; } = null!;
    private Symbol ResolveMemberSymbols(Symbol symbol) =>
        RunTransformBlock(ResolveMemberSymbolsBlock, symbol, e => _symbolsService.RecursivelyResolveSymbols(e));

    private ActionBlock<Symbol> SetDocumentStateMemberSymbolsBlock { get; set; } = null!;
    private void SetDocumentStateMemberSymbols(Symbol symbol) =>
        RunActionBlock(SetDocumentStateMemberSymbolsBlock, symbol, e => State = (DocumentParserState)State.WithSymbols(e));

    protected override (ITargetBlock<DocumentParserState>, Task) DefinePipelineBlocks(ISourceBlock<DocumentParserState> source)
    {
        AcquireDocumentStateSymbolsBlock = new(AcquireDocumentStateSymbols, ConcurrentExecutionOptions);
        ResolveMemberSymbolsBlock = new(ResolveMemberSymbols, ConcurrentExecutionOptions);
        SetDocumentStateMemberSymbolsBlock = new(SetDocumentStateMemberSymbols, ConcurrentExecutionOptions);

        Link(source, AcquireDocumentStateSymbolsBlock, WithCompletionPropagation);
        Link(AcquireDocumentStateSymbolsBlock, ResolveMemberSymbolsBlock, WithCompletionPropagation);
        Link(ResolveMemberSymbolsBlock, SetDocumentStateMemberSymbolsBlock, WithCompletionPropagation);

        return (AcquireDocumentStateSymbolsBlock, SetDocumentStateMemberSymbolsBlock.Completion);
    }
}
