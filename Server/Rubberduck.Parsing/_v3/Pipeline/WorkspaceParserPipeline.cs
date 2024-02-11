using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

/// <summary>
/// A pipeline that broadcasts a <c>WorkspaceFileUri</c> for each file under a given workspace.
/// </summary>
/// <remarks>
/// Prioritizes files that are already opened in the client.
/// </remarks>
public class WorkspaceParserPipeline : ParserPipeline<WorkspaceUri, ParserPipelineState>
{
    private readonly IWorkspaceStateManager _workspaceManager;
    private readonly ParserPipelineProvider _pipelineProvider;

    private readonly ConcurrentBag<IParserPipeline> _filePipelines = new();

    public WorkspaceParserPipeline(ILogger<WorkspaceParserPipeline> logger,
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance,
        IWorkspaceStateManager workspaceManager,
        ParserPipelineProvider pipelineProvider)
        : base(logger, settingsProvider, performance)
    {
        _workspaceManager = workspaceManager;
        _pipelineProvider = pipelineProvider;
    }

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

    private TransformBlock<WorkspaceFileUri, WorkspaceFileParserPipeline> CreateWorkspaceFilePipelineBlock { get; set; } = null!;
    private WorkspaceFileParserPipeline CreateWorkspaceFilePipeline(WorkspaceFileUri uri) =>
        RunTransformBlock(CreateWorkspaceFilePipelineBlock, uri, e => _pipelineProvider.StartNew(uri, TokenSource));

    private ActionBlock<WorkspaceFileParserPipeline> AcquireWorkspaceFilePipelineBlock { get; set; } = null!;
    private void AquireWorkspaceFilePipeline(WorkspaceFileParserPipeline pipeline) =>
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

        return (AcquireWorkspaceBlock, AcquireWorkspaceFilePipelineBlock.Completion);
    }
}
