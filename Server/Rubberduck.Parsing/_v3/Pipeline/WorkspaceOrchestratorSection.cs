using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

public abstract class WorkspaceOrchestratorSection : DataflowPipelineSection<WorkspaceUri, IWorkspaceState>
{
    private readonly IWorkspaceStateManager _workspaceManager;
    private readonly ConcurrentBag<WorkspaceDocumentSection> _filePipelines = [];
    private readonly ConcurrentBag<Task> _completionTasks = [];

    protected WorkspaceOrchestratorSection(DataflowPipeline parent, IWorkspaceStateManager workspaceManager,
        ILogger logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
        : base(parent, logger, settingsProvider, performance)
    {
        _workspaceManager = workspaceManager;
    }

    protected abstract WorkspaceDocumentSection StartDocumentPipeline(WorkspaceFileUri uri);

    private TransformBlock<WorkspaceUri, IWorkspaceState> AcquireWorkspaceBlock { get; set; } = null!;
    private IWorkspaceState AcquireWorkspaceState(WorkspaceUri uri) =>
        RunTransformBlock(AcquireWorkspaceBlock, uri, e => _workspaceManager.GetWorkspace(uri.WorkspaceRoot)
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

    private ActionBlock<WorkspaceDocumentSection> AcquireWorkspaceFilePipelineBlock { get; set; } = null!;
    private void AquireWorkspaceFilePipeline(WorkspaceDocumentSection pipeline) =>
        RunActionBlock(AcquireWorkspaceFilePipelineBlock, pipeline, e =>
        {
            if (pipeline.Completion is null)
            {
                throw new InvalidOperationException("Child pipeline completion task is unexpectely null.");
            }
            _filePipelines.Add(pipeline);
        });

    protected override (IEnumerable<IDataflowBlock>, Task) DefineSectionBlocks(CancellationTokenSource? tokenSource)
    {
        TokenSource = tokenSource;

        AcquireWorkspaceBlock = new(AcquireWorkspaceState, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(AcquireWorkspaceBlock), AcquireWorkspaceBlock);

        PrioritizeFilesBlock = new(PrioritizeFiles, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(PrioritizeFilesBlock), PrioritizeFilesBlock);

        CreateWorkspaceFilePipelineBlock = new(CreateWorkspaceFilePipeline, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(CreateWorkspaceFilePipelineBlock), CreateWorkspaceFilePipelineBlock);

        AcquireWorkspaceFilePipelineBlock = new(AquireWorkspaceFilePipeline, ConcurrentExecutionOptions(Token));
        var childCompletion = TraceBlockCompletionAsync(nameof(AcquireWorkspaceFilePipelineBlock), AcquireWorkspaceFilePipelineBlock);

        Link(AcquireWorkspaceBlock, PrioritizeFilesBlock);
        Link(PrioritizeFilesBlock, CreateWorkspaceFilePipelineBlock);
        Link(CreateWorkspaceFilePipelineBlock, AcquireWorkspaceFilePipelineBlock);

        Completion = childCompletion
            .ContinueWith(async t => await Task.WhenAll(_filePipelines.Select(e => e.Completion).ToArray()), Token, TaskContinuationOptions.None, TaskScheduler.Default);

        return (new IDataflowBlock[] 
        { 
            AcquireWorkspaceBlock,
            PrioritizeFilesBlock,
            CreateWorkspaceFilePipelineBlock,
            AcquireWorkspaceFilePipelineBlock,
        }, Completion);
    }

    protected override ImmutableArray<(string, IDataflowBlock)> DataflowBlocks => new (string, IDataflowBlock)[]
    {
        (nameof(AcquireWorkspaceBlock), AcquireWorkspaceBlock),
        (nameof(PrioritizeFilesBlock), PrioritizeFilesBlock),
        (nameof(CreateWorkspaceFilePipelineBlock), CreateWorkspaceFilePipelineBlock),
        (nameof(AcquireWorkspaceFilePipelineBlock), AcquireWorkspaceFilePipelineBlock),
    }.ToImmutableArray();
}
