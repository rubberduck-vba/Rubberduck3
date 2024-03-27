using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Services;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline.Abstract;

/// <summary>
/// A pipeline section that works with a <c>WorkspaceUri</c> input and orchestrates the processing of each file in that entire workspace.
/// </summary>
public abstract class WorkspaceOrchestratorSection : DataflowPipelineSection<WorkspaceUri, IWorkspaceState>
{
    private readonly ILanguageServer _server;
    private readonly IAppWorkspacesStateManager _workspaces;
    private readonly ParserPipelineSectionProvider _provider;

    protected WorkspaceOrchestratorSection(DataflowPipeline parent, 
        IAppWorkspacesStateManager workspaces, 
        ParserPipelineSectionProvider pipelineProvider,
        ILanguageServer server,
        ILogger logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
        : base(parent, logger, settingsProvider, performance)
    {
        _workspaces = workspaces;
        _provider = pipelineProvider;
        _server = server;
    }

    protected abstract WorkspaceDocumentSection StartDocumentPipeline(ParserPipelineSectionProvider provider, WorkspaceFileUri uri);

    private TransformBlock<WorkspaceUri, IWorkspaceState> AcquireWorkspaceStateBlock { get; set; } = null!;
    private IWorkspaceState AcquireWorkspaceState(WorkspaceUri uri) =>
        RunTransformBlock(AcquireWorkspaceStateBlock, uri, 
            e => State = _workspaces.GetWorkspace(uri.WorkspaceRoot) ?? throw new InvalidOperationException($"Could not find workspace state for URI '{uri}'."),
            nameof(AcquireWorkspaceStateBlock), logPerformance: false);

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
        }, nameof(PrioritizeFilesBlock), logPerformance: true);

    private ActionBlock<WorkspaceDocumentSection> AcquireWorkspaceFilePipelineBlock { get; set; } = null!;

    private async Task AcquireWorkspaceFilePipelineAsync(WorkspaceDocumentSection pipeline) => await
        RunTransformBlock(AcquireWorkspaceFilePipelineBlock, pipeline, e =>
        {
            if (pipeline.Completion is null)
            {
                throw new InvalidOperationException("Child pipeline completion task is unexpectely null.");
            }
            return pipeline.Completion;
        }, nameof(AcquireWorkspaceFilePipelineBlock), logPerformance: false);

    private TransformBlock<WorkspaceFileUri, WorkspaceDocumentSection> CreateWorkspaceFilePipelineBlock { get; set; } = null!;
    private WorkspaceDocumentSection CreateWorkspaceFilePipeline(WorkspaceFileUri uri) =>
        RunTransformBlock(CreateWorkspaceFilePipelineBlock, uri, e => StartDocumentPipeline(_provider, uri), 
            nameof(CreateWorkspaceFilePipelineBlock), logPerformance: false);

    protected override (IEnumerable<IDataflowBlock>, Task) DefineSectionBlocks(CancellationTokenSource? tokenSource)
    {
        TokenSource = tokenSource;

        AcquireWorkspaceStateBlock = new(AcquireWorkspaceState, ConcurrentExecutionOptions(Token));
        PrioritizeFilesBlock = new(PrioritizeFiles, ConcurrentExecutionOptions(Token));
        CreateWorkspaceFilePipelineBlock = new(CreateWorkspaceFilePipeline, ConcurrentExecutionOptions(Token));
        AcquireWorkspaceFilePipelineBlock = new(AcquireWorkspaceFilePipelineAsync, ConcurrentExecutionOptions(Token));

        Link(AcquireWorkspaceStateBlock, PrioritizeFilesBlock);
        Link(PrioritizeFilesBlock, CreateWorkspaceFilePipelineBlock);
        Link(CreateWorkspaceFilePipelineBlock, AcquireWorkspaceFilePipelineBlock);

        Completion = AcquireWorkspaceFilePipelineBlock.Completion;

        return (new IDataflowBlock[]
        {
            AcquireWorkspaceStateBlock,
            PrioritizeFilesBlock,
            CreateWorkspaceFilePipelineBlock,
            AcquireWorkspaceFilePipelineBlock,
        }, Completion);
    }

    protected override Dictionary<string, IDataflowBlock> DataflowBlocks => new()
    {
        [nameof(AcquireWorkspaceStateBlock)] = AcquireWorkspaceStateBlock,
        [nameof(PrioritizeFilesBlock)] = PrioritizeFilesBlock,
        [nameof(CreateWorkspaceFilePipelineBlock)] = CreateWorkspaceFilePipelineBlock,
        [nameof(AcquireWorkspaceFilePipelineBlock)] = AcquireWorkspaceFilePipelineBlock,
    };

    protected override void LogAdditionalPipelineSectionCompletionInfo(StringBuilder builder, string name)
    {
        var info = State?.WorkspaceRoot?.ToString();
        if (!string.IsNullOrWhiteSpace(info))
        {
            builder.AppendLine($"\t📁 {info}");
        }
    }
}
