using Antlr4.Runtime.Tree;
using Microsoft.Extensions.Logging;
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
    private readonly PipelineParserService _service;

    public SymbolsPipeline(ILogger<WorkspaceParserPipeline> logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance,
        DocumentContentStore contentStore,
        PipelineParserService service) 
        : base(logger, settingsProvider, performance)
    {
        _contentStore = contentStore;
        _service = service;
    }

    private TransformBlock<PipelineParseResult, IParseTree> AcquireParseTreeBlock { get; set; } = null!;
    private TransformBlock<IParseTree, Symbol[]> AcquireSymbolsBlock { get; set; } = null!;

    protected override (ITargetBlock<PipelineParseResult> inputBlock, Task completion) DefinePipelineBlocks()
    {
        AcquireParseTreeBlock = new(AcquireParseTree, ExecutionOptions);
        AcquireSymbolsBlock = new(AcquireSymbols, ExecutionOptions);

        return (AcquireParseTreeBlock, AcquireParseTreeBlock.Completion);
    }

    protected override void SetInitialState(PipelineParseResult input) 
    {
        var uri = input.Uri;
        State = _contentStore.GetContent(uri);
    }

    private IParseTree AcquireParseTree(PipelineParseResult input) =>
        RunTransformBlock(AcquireParseTreeBlock, input, e => input.ParseResult.Tree);

    private Symbol[] AcquireSymbols(IParseTree syntaxTree) =>
        RunTransformBlock(AcquireSymbolsBlock, syntaxTree, e => Array.Empty<Symbol>());

}
