using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

public abstract class WorkspaceOrchestratorPipeline : ParserPipeline<WorkspaceUri, ParserPipelineState>
{
    private readonly IWorkspaceStateManager _workspaceManager;
    private readonly ConcurrentBag<WorkspaceDocumentPipeline> _filePipelines = [];

    protected WorkspaceOrchestratorPipeline(ILogger<WorkspaceParserPipeline> logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance,
        IWorkspaceStateManager workspaceManager) 
        : base(logger, settingsProvider, performance)
    {
        _workspaceManager = workspaceManager;
    }

    protected abstract WorkspaceDocumentPipeline StartDocumentPipeline(WorkspaceFileUri uri);

    private TransformBlock<WorkspaceUri, IWorkspaceState> AcquireWorkspaceBlock { get; set; } = null!;
    private IWorkspaceState AcquireWorkspaceState(WorkspaceUri uri) =>
        RunTransformBlock(AcquireWorkspaceBlock, uri, e => _workspaceManager.GetWorkspace(uri.WorkspaceRoot)
            ?? throw new InvalidOperationException($"Could not find workspace state for URI '{uri.WorkspaceRoot}'."));

    private TransformManyBlock<IWorkspaceState, WorkspaceFileUri> PrioritizeFilesBlock { get; set; } = null!;
    private WorkspaceFileUri[] PrioritizeFiles(IWorkspaceState state) =>
        RunTransformBlock(PrioritizeFilesBlock, state, e =>
        {
            var result = e.WorkspaceFiles
                .OrderByDescending(file => file.IsOpened) // opened files go first
                .Select(file => file.Uri)
                .ToArray();

            if (result.Length == 0)
            {
                throw new InvalidOperationException($"Workspace state has no files to process.");
            }

            return result;
        });

    private TransformBlock<WorkspaceFileUri, WorkspaceDocumentPipeline> CreateWorkspaceFilePipelineBlock { get; set; } = null!;
    private WorkspaceDocumentPipeline CreateWorkspaceFilePipeline(WorkspaceFileUri uri) =>
        RunTransformBlock(CreateWorkspaceFilePipelineBlock, uri, e => StartDocumentPipeline(uri));

    private ActionBlock<WorkspaceDocumentPipeline> AcquireWorkspaceFilePipelineBlock { get; set; } = null!;
    private void AquireWorkspaceFilePipeline(WorkspaceDocumentPipeline pipeline) =>
        RunActionBlock(AcquireWorkspaceFilePipelineBlock, pipeline, e => _filePipelines.Add(pipeline));

    protected override (ITargetBlock<WorkspaceUri> inputBlock, Task completion) DefinePipelineBlocks()
    {
        AcquireWorkspaceBlock = new(AcquireWorkspaceState, ConcurrentExecutionOptions);
        PrioritizeFilesBlock = new(PrioritizeFiles, ConcurrentExecutionOptions);
        CreateWorkspaceFilePipelineBlock = new(CreateWorkspaceFilePipeline, ConcurrentExecutionOptions);
        AcquireWorkspaceFilePipelineBlock = new(AquireWorkspaceFilePipeline, ConcurrentExecutionOptions);

        Link(AcquireWorkspaceBlock, PrioritizeFilesBlock, WithCompletionPropagation);
        Link(PrioritizeFilesBlock, CreateWorkspaceFilePipelineBlock, WithCompletionPropagation);
        Link(CreateWorkspaceFilePipelineBlock, AcquireWorkspaceFilePipelineBlock, WithCompletionPropagation);

        var completion = AcquireWorkspaceFilePipelineBlock.Completion
            .ContinueWith(t => Task.WhenAll(_filePipelines.Select(pipeline => pipeline.Completion).ToArray()), Token, TaskContinuationOptions.None, TaskScheduler.Default);

        return (AcquireWorkspaceBlock, completion);
    }
}

/// <summary>
/// A pipeline that broadcasts a <c>WorkspaceFileUri</c> for each file under a given workspace.
/// </summary>
/// <remarks>
/// Prioritizes files that are already opened in the client.
/// </remarks>
public class WorkspaceParserPipeline : WorkspaceOrchestratorPipeline
{
    private readonly ParserPipelineProvider _pipelineProvider;

    public WorkspaceParserPipeline(ILogger<WorkspaceParserPipeline> logger,
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance,
        IWorkspaceStateManager workspaceManager,
        ParserPipelineProvider pipelineProvider)
        : base(logger, settingsProvider, performance, workspaceManager)
    {
        _pipelineProvider = pipelineProvider;
    }

    protected override WorkspaceDocumentPipeline StartDocumentPipeline(WorkspaceFileUri uri) => _pipelineProvider.StartNew(uri, TokenSource);
}
