using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
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

    public WorkspaceFileParserPipeline(ILogger<WorkspaceParserPipeline> logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance,
        DocumentContentStore contentStore,
        PipelineParserService parser) 
        : base(logger, settingsProvider, performance)
    {
        _contentStore = contentStore;
        _parser = parser;
    }

    private TransformBlock<WorkspaceUri, DocumentState> AcquireDocumentStateBlock { get; set; } = null!;
    private TransformBlock<DocumentState, PipelineParseResult> ParseDocumentTextBlock { get; set; } = null!;
    private BroadcastBlock<PipelineParseResult> BroadcastParseResultsBlock { get; set; } = null!;
    private ActionBlock<PipelineParseResult> UpdateDocumentStateBlock { get; set; } = null!;

    protected override (ITargetBlock<WorkspaceFileUri> inputBlock, Task completion) DefinePipelineBlocks()
    {
        AcquireDocumentStateBlock = new(AcquireDocumentState, ExecutionOptions);
        ParseDocumentTextBlock = new(ParseDocumentText, ExecutionOptions);
        UpdateDocumentStateBlock = new(UpdateDocumentState, ExecutionOptions);
        BroadcastParseResultsBlock = new(BroadcastParserResult, ExecutionOptions);

        Link(AcquireDocumentStateBlock, ParseDocumentTextBlock, WithCompletionPropagation);
        Link(ParseDocumentTextBlock, UpdateDocumentStateBlock, WithCompletionPropagation);
        Link(BroadcastParseResultsBlock, UpdateDocumentStateBlock, WithCompletionPropagation);
        
        return (AcquireDocumentStateBlock, UpdateDocumentStateBlock.Completion);
    }

    protected override void SetInitialState(WorkspaceFileUri input) { }

    private DocumentState AcquireDocumentState(WorkspaceUri uri)
    {
        uri = uri as WorkspaceFileUri ?? throw new ArgumentException($"Expected a {nameof(WorkspaceFileUri)}.");

        return RunTransformBlock(AcquireDocumentStateBlock,uri, param =>
            _contentStore.GetContent(param) ?? throw new InvalidOperationException("Document state was not found in the content store."));
    }

    private PipelineParseResult ParseDocumentText(DocumentState documentState) =>
        RunTransformBlock(ParseDocumentTextBlock, documentState, e => _parser.ParseDocument(e, Token));

    private void UpdateDocumentState(PipelineParseResult parserResult) =>
        State = RunTransformBlock(UpdateDocumentStateBlock, parserResult, e => State?.WithFoldings(e.Foldings)
            ?? throw new InvalidOperationException("Document state was unexpectedly null."));

    private PipelineParseResult BroadcastParserResult(PipelineParseResult parserResult) =>
        RunTransformBlock(BroadcastParseResultsBlock, parserResult, e => e);
}
