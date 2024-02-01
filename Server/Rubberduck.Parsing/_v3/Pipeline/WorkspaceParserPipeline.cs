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
    private readonly IParserPipelineProvider<WorkspaceFileUri> _filePipelineProvider;

    public WorkspaceParserPipeline(ILogger<WorkspaceParserPipeline> logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance,
        IWorkspaceStateManager workspaceManager,
        IParserPipelineProvider<WorkspaceFileUri> filePipelineProvider)
        : base(logger, settingsProvider, performance)
    {
        _workspaceManager = workspaceManager;
        _filePipelineProvider = filePipelineProvider;
    }

    protected override (ITargetBlock<WorkspaceUri> inputBlock, Task completion) DefinePipelineBlocks()
    {
        AcquireWorkspaceBlock = new(AcquireWorkspaceState, ExecutionOptions);
        PrioritizeFilesBlock = new(PrioritizeFiles, ExecutionOptions);
        CreateWorkspaceFilePipelineBlock = new(CreateWorkspaceFilePipeline, ExecutionOptions);

        Link(AcquireWorkspaceBlock, PrioritizeFilesBlock, WithCompletionPropagation);
        Link(PrioritizeFilesBlock, CreateWorkspaceFilePipelineBlock, WithCompletionPropagation);

        return (AcquireWorkspaceBlock, CreateWorkspaceFilePipelineBlock.Completion);
    }

    protected override void SetInitialState(WorkspaceUri input) => State = new() { WorkspaceRootUri = input };

    public TransformBlock<WorkspaceUri, IWorkspaceState> AcquireWorkspaceBlock { get; private set; } = null!;
    public TransformManyBlock<IWorkspaceState, WorkspaceFileUri> PrioritizeFilesBlock { get; private set; } = null!;
    public TransformBlock<WorkspaceFileUri, IParserPipeline<WorkspaceFileUri>> CreateWorkspaceFilePipelineBlock { get; private set; } = null!;


    private IWorkspaceState AcquireWorkspaceState(WorkspaceUri uri) => 
        RunTransformBlock(AcquireWorkspaceBlock, uri, e => _workspaceManager.GetWorkspace(uri.WorkspaceRoot)
            ?? throw new InvalidOperationException($"Could not find workspace state for URI '{uri.WorkspaceRoot}'."));

    private WorkspaceFileUri[] PrioritizeFiles(IWorkspaceState state) =>
        RunTransformBlock(PrioritizeFilesBlock, state, e =>
        {
            var result = e.WorkspaceFiles
                .OrderByDescending(file => file.IsOpened) // opened files first
                .Select(file => file.Uri)
                .ToArray();

            if (result.Length == 0)
            {
                throw new InvalidOperationException($"Workspace state has no files to process.");
            }

            return result;
        });

    private IParserPipeline<WorkspaceFileUri> CreateWorkspaceFilePipeline(WorkspaceFileUri uri) =>
        RunTransformBlock(CreateWorkspaceFilePipelineBlock, uri, e => _filePipelineProvider.StartNew(uri, TokenSource));
}
