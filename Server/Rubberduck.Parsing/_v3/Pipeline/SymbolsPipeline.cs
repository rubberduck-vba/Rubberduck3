using Antlr4.Runtime.Tree;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Bson;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

/// <summary>
/// A pipeline that produces and broadcasts all symbols in a given <c>ParserResult</c>.
/// </summary>
public class SymbolsPipeline : ParserPipeline<PipelineParseResult, DocumentState>
{
    private readonly DocumentContentStore _contentStore;
    private readonly PipelineParseTreeSymbolsService _service;

    public SymbolsPipeline(ILogger<WorkspaceParserPipeline> logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance,
        DocumentContentStore contentStore,
        PipelineParseTreeSymbolsService service) 
        : base(logger, settingsProvider, performance)
    {
        _contentStore = contentStore;
        _service = service;
    }

    private TransformBlock<PipelineParseResult, IParseTree> AcquireParseTreeBlock { get; set; } = null!;
    private TransformBlock<IParseTree, Symbol> AcquireSymbolsBlock { get; set; } = null!;
    private BroadcastBlock<Symbol> BroadcastDeclarationSymbolsBlock { get; set; } = null!;
    private ActionBlock<Symbol> UpdateDocumentStateBlock { get; set; } = null!;

    protected override (ITargetBlock<PipelineParseResult> inputBlock, Task completion) DefinePipelineBlocks()
    {
        AcquireParseTreeBlock = new(AcquireParseTree, ExecutionOptions);
        AcquireSymbolsBlock = new(AcquireSymbols, ExecutionOptions);
        BroadcastDeclarationSymbolsBlock = new(BroadcastDeclarationSymbols, ExecutionOptions);
        UpdateDocumentStateBlock = new(UpdateDocumentState, ExecutionOptions);

        Link(AcquireParseTreeBlock, AcquireSymbolsBlock, WithCompletionPropagation);
        Link(AcquireSymbolsBlock, BroadcastDeclarationSymbolsBlock, WithCompletionPropagation);
        Link(BroadcastDeclarationSymbolsBlock, UpdateDocumentStateBlock, WithCompletionPropagation);

        return (AcquireParseTreeBlock, AcquireParseTreeBlock.Completion);
    }

    protected override void SetInitialState(PipelineParseResult input) 
    {
        var uri = input.Uri;
        State = _contentStore.GetContent(uri);
    }

    private IParseTree AcquireParseTree(PipelineParseResult input) => 
        RunTransformBlock(AcquireParseTreeBlock, input, e => input.ParseResult.Tree);

    private Symbol AcquireSymbols(IParseTree syntaxTree) => 
        RunTransformBlock(AcquireSymbolsBlock, syntaxTree, e => _service.DiscoverDeclarationSymbols(e, State!.Uri));

    private Symbol BroadcastDeclarationSymbols(Symbol symbol) => 
        RunTransformBlock(BroadcastDeclarationSymbolsBlock, symbol, symbol => symbol);

    private void UpdateDocumentState(Symbol symbol) =>
        RunActionBlock(UpdateDocumentStateBlock, symbol, symbol => State = State!.WithSymbols(symbol));
}
