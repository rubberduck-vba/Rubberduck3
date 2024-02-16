using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

public abstract class WorkspaceOrchestratorPipeline : ParserPipelineSection<Uri, IWorkspaceState>
{
    private readonly IWorkspaceStateManager _workspaceManager;
    private readonly ConcurrentBag<WorkspaceDocumentSection> _filePipelines = [];
    private readonly ConcurrentBag<Task> _completionTasks = [];

    protected WorkspaceOrchestratorPipeline(DataflowPipeline parent, IWorkspaceStateManager workspaceManager,
        ILogger<WorkspaceParserPipeline> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
        : base(parent, logger, settingsProvider, performance)
    {
        _workspaceManager = workspaceManager;
    }

    protected abstract WorkspaceDocumentSection StartDocumentPipeline(WorkspaceFileUri uri);

    private TransformBlock<Uri, IWorkspaceState> AcquireWorkspaceBlock { get; set; } = null!;
    private IWorkspaceState AcquireWorkspaceState(Uri uri) =>
        RunTransformBlock(AcquireWorkspaceBlock, uri, e => _workspaceManager.GetWorkspace(uri)
            ?? throw new InvalidOperationException($"Could not find workspace state for URI '{uri}'."));

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

    private TransformBlock<WorkspaceFileUri, WorkspaceDocumentSection> CreateWorkspaceFilePipelineBlock { get; set; } = null!;
    private WorkspaceDocumentSection CreateWorkspaceFilePipeline(WorkspaceFileUri uri) =>
        RunTransformBlock(CreateWorkspaceFilePipelineBlock, uri, e => StartDocumentPipeline(uri));

    private TransformBlock<WorkspaceDocumentSection, Task> AcquireWorkspaceFilePipelineBlock { get; set; } = null!;
    private async Task AquireWorkspaceFilePipelineAsync(WorkspaceDocumentSection pipeline) =>
        await RunTransformBlock(AcquireWorkspaceFilePipelineBlock, pipeline, e =>
        {
            _filePipelines.Add(pipeline);
            return pipeline.Completion!;
        });

    private BufferBlock<Task> CompletionTasksBufferBlock { get; set; } = null!;

    protected override (IEnumerable<IDataflowBlock>, Task) DefinePipelineBlocks(CancellationTokenSource? tokenSource)
    {
        TokenSource = tokenSource;

        AcquireWorkspaceBlock = new(AcquireWorkspaceState, ConcurrentExecutionOptions(Token));
        TraceBlockCompletion(nameof(AcquireWorkspaceBlock), AcquireWorkspaceFilePipelineBlock);

        PrioritizeFilesBlock = new(PrioritizeFiles, ConcurrentExecutionOptions(Token));
        TraceBlockCompletion(nameof(PrioritizeFilesBlock), PrioritizeFilesBlock);

        CreateWorkspaceFilePipelineBlock = new(CreateWorkspaceFilePipeline, ConcurrentExecutionOptions(Token));
        TraceBlockCompletion(nameof(CreateWorkspaceFilePipelineBlock), CreateWorkspaceFilePipelineBlock);

        AcquireWorkspaceFilePipelineBlock = new(AquireWorkspaceFilePipelineAsync, ConcurrentExecutionOptions(Token));
        TraceBlockCompletion(nameof(AcquireWorkspaceFilePipelineBlock), AcquireWorkspaceFilePipelineBlock);

        CompletionTasksBufferBlock = new BufferBlock<Task>();
        TraceBlockCompletion(nameof(CompletionTasksBufferBlock), CompletionTasksBufferBlock);

        Link(AcquireWorkspaceBlock, PrioritizeFilesBlock);
        Link(PrioritizeFilesBlock, CreateWorkspaceFilePipelineBlock);
        Link(CreateWorkspaceFilePipelineBlock, AcquireWorkspaceFilePipelineBlock);
        Link(AcquireWorkspaceFilePipelineBlock, CompletionTasksBufferBlock);

        return (new IDataflowBlock[] 
        { 
            AcquireWorkspaceBlock,
            PrioritizeFilesBlock,
            CreateWorkspaceFilePipelineBlock,
            AcquireWorkspaceFilePipelineBlock,
            CompletionTasksBufferBlock
        }, CompletionTasksBufferBlock.Completion);
    }

    
}
