using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Services;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline.Abstract;

/// <summary>
/// A pipeline section that works with a <c>WorkspaceUri</c> input and orchestrates the processing of each file in that entire workspace.
/// </summary>
public abstract class WorkspaceOrchestratorSection : DataflowPipelineSection<WorkspaceUri, IWorkspaceState>
{
    private readonly IWorkspaceStateManager _workspaces;
    private readonly ParserPipelineSectionProvider _provider;

    protected WorkspaceOrchestratorSection(DataflowPipeline parent, IWorkspaceStateManager workspaces, ParserPipelineSectionProvider pipelineProvider,
        ILogger logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
        : base(parent, logger, settingsProvider, performance)
    {
        _workspaces = workspaces;
        _provider = pipelineProvider;
    }

    protected abstract WorkspaceDocumentSection StartDocumentPipeline(ParserPipelineSectionProvider provider, WorkspaceFileUri uri);

    private TransformBlock<WorkspaceUri, IWorkspaceState> AcquireWorkspaceBlock { get; set; } = null!;
    private IWorkspaceState AcquireWorkspaceState(WorkspaceUri uri) =>
        RunTransformBlock(AcquireWorkspaceBlock, uri, e => _workspaces.GetWorkspace(uri.WorkspaceRoot)
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

    private ActionBlock<WorkspaceDocumentSection> AcquireWorkspaceFilePipelineBlock { get; set; } = null!;

#pragma warning disable VSTHRD101 // Avoid unsupported async delegates
    // RunActionBlock captures and logs whatever exception the async delegate might throw.
    private void AquireWorkspaceFilePipeline(WorkspaceDocumentSection pipeline) =>
        RunActionBlock(AcquireWorkspaceFilePipelineBlock, pipeline, async e =>
        {
            if (pipeline.Completion is null)
            {
                throw new InvalidOperationException("Child pipeline completion task is unexpectely null.");
            }
            await pipeline.Completion;
        });
#pragma warning restore VSTHRD101 // Avoid unsupported async delegates

    private TransformBlock<WorkspaceFileUri, WorkspaceDocumentSection> CreateWorkspaceFilePipelineBlock { get; set; } = null!;
    private WorkspaceDocumentSection CreateWorkspaceFilePipeline(WorkspaceFileUri uri) =>
        RunTransformBlock(CreateWorkspaceFilePipelineBlock, uri, e => StartDocumentPipeline(_provider, uri));

    protected override (IEnumerable<IDataflowBlock>, Task) DefineSectionBlocks(CancellationTokenSource? tokenSource)
    {
        TokenSource = tokenSource;

        AcquireWorkspaceBlock = new(AcquireWorkspaceState, ConcurrentExecutionOptions(Token));
        PrioritizeFilesBlock = new(PrioritizeFiles, ConcurrentExecutionOptions(Token));
        CreateWorkspaceFilePipelineBlock = new(CreateWorkspaceFilePipeline, ConcurrentExecutionOptions(Token));
        AcquireWorkspaceFilePipelineBlock = new(AquireWorkspaceFilePipeline, ConcurrentExecutionOptions(Token));

        Link(AcquireWorkspaceBlock, PrioritizeFilesBlock);
        Link(PrioritizeFilesBlock, CreateWorkspaceFilePipelineBlock);
        Link(CreateWorkspaceFilePipelineBlock, AcquireWorkspaceFilePipelineBlock);

        Completion = Task.WhenAll(DataflowBlocks.Select(e => e.Block.Completion).ToArray())
            .ContinueWith(async t =>
            {
                await _provider.AwaitCompletionAsync();
                LogDebug("awaited pipeline provider completion tasks!");
            }, Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);

        return (new IDataflowBlock[]
        {
            AcquireWorkspaceBlock,
            PrioritizeFilesBlock,
            CreateWorkspaceFilePipelineBlock,
            AcquireWorkspaceFilePipelineBlock,
        }, Completion);
    }

    protected override ImmutableArray<(string Name, IDataflowBlock Block)> DataflowBlocks => new (string, IDataflowBlock)[]
    {
        (nameof(AcquireWorkspaceBlock), AcquireWorkspaceBlock),
        (nameof(PrioritizeFilesBlock), PrioritizeFilesBlock),
        (nameof(CreateWorkspaceFilePipelineBlock), CreateWorkspaceFilePipelineBlock),
        (nameof(AcquireWorkspaceFilePipelineBlock), AcquireWorkspaceFilePipelineBlock),
    }.ToImmutableArray();
}
