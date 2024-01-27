using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

/// <summary>
/// A pipeline that broadcasts a <c>WorkspaceFileUri</c> for each file under a given workspace.
/// </summary>
/// <remarks>
/// The output prioritizes files that are already opened in the client.
/// </remarks>
public class WorkspaceParserPipeline : ParserPipeline<WorkspaceUri, ParserPipelineState>
{
    private readonly IWorkspaceStateManager _workspaceManager;

    public WorkspaceParserPipeline(ILogger<WorkspaceParserPipeline> logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance,
        IWorkspaceStateManager workspaceManager)
        : base(logger, settingsProvider, performance)
    {
        _workspaceManager = workspaceManager;
    }

    protected override (ITargetBlock<WorkspaceUri> inputBlock, Task completion) DefinePipelineBlocks()
    {
        AcquireWorkspaceBlock = new(AcquireWorkspaceState, ExecutionOptions);
        PrioritizeFilesBlock = new(PrioritizeFiles, ExecutionOptions);
        WorkspaceFileUriBufferBlock = new(ExecutionOptions);

        Link(AcquireWorkspaceBlock, PrioritizeFilesBlock, WithCompletionPropagation);
        Link(PrioritizeFilesBlock, WorkspaceFileUriBufferBlock, WithCompletionPropagation);

        return (AcquireWorkspaceBlock, WorkspaceFileUriBufferBlock.Completion);
    }

    protected override void SetInitialState(WorkspaceUri input) => State = new() { WorkspaceRootUri = input };

    private TransformBlock<WorkspaceUri, IWorkspaceState> AcquireWorkspaceBlock { get; set; } = null!;
    private TransformManyBlock<IWorkspaceState, WorkspaceFileUri> PrioritizeFilesBlock { get; set; } = null!;
    private BufferBlock<WorkspaceFileUri> WorkspaceFileUriBufferBlock { get; set; } = null!;


    private IWorkspaceState AcquireWorkspaceState(WorkspaceUri uri) => 
        RunTransformBlock(AcquireWorkspaceBlock, uri, e => _workspaceManager.GetWorkspace(uri.WorkspaceRoot)
            ?? throw new InvalidOperationException($"Could not find workspace state for URI '{uri.WorkspaceRoot}'."));

    private WorkspaceFileUri[] PrioritizeFiles(IWorkspaceState state)
    {
        var result = RunTransformBlock(PrioritizeFilesBlock, state, 
            e => e.WorkspaceFiles
                .OrderByDescending(file => file.IsOpened)
                .ThenBy(file => file.Name)
                .Select(file => file.Uri).ToArray());

        if (result.Length == 0)
        {
            throw new InvalidOperationException($"Workspace state has no files to process.");
        }

        return result;
    }
}
