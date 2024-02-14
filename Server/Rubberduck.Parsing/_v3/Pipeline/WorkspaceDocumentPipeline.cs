using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

public abstract class WorkspaceDocumentPipeline : ParserPipeline<WorkspaceFileUri, DocumentParserState>
{
    private readonly IWorkspaceService _workspaceService;
    
    protected WorkspaceDocumentPipeline(ILogger<WorkspaceParserPipeline> logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance,
        IWorkspaceService workspaceService) 
        : base(logger, settingsProvider, performance)
    {
        _workspaceService = workspaceService;
    }

    private TransformBlock<WorkspaceFileUri, DocumentParserState> AcquireDocumentStateBlock { get; set; } = null!;
    private DocumentParserState AcquireDocumentState(WorkspaceFileUri uri)
    {
        var workspace = _workspaceService.State.GetWorkspace(uri.WorkspaceRoot);
        if (workspace.TryGetWorkspaceFile(uri, out var state) && state != null)
        {
            State = new DocumentParserState((SourceFileDocumentState)state);
            return State;
        }

        throw new InvalidOperationException("Document state was not found in the content store.");
    }

    protected sealed override (ITargetBlock<WorkspaceFileUri> inputBlock, Task completion) DefinePipelineBlocks()
    {
        AcquireDocumentStateBlock = new TransformBlock<WorkspaceFileUri, DocumentParserState>(AcquireDocumentState, ConcurrentExecutionOptions);
        var (startBlock, completion) = DefinePipelineBlocks(AcquireDocumentStateBlock);

        Link(AcquireDocumentStateBlock, startBlock, WithCompletionPropagation);

        return (AcquireDocumentStateBlock, completion);
    }

    protected abstract (ITargetBlock<DocumentParserState>, Task) DefinePipelineBlocks(ISourceBlock<DocumentParserState> source);
}
