using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline.Abstract;

/// <summary>
/// A pipeline section that works with a <c>WorkspaceFileUri</c> input to process a single file.
/// </summary>
/// <remarks>
/// The class defines an <c>AcquireDocumentStateBlock</c> that is provided as a <c>source</c> for implementing/derived classes,
/// so implementations work with a <c>DocumentParserState</c> input.
/// </remarks>
public abstract class WorkspaceDocumentSection : DataflowPipelineSection<WorkspaceFileUri, DocumentParserState>
{
    private readonly IWorkspaceService _workspaceService;
    private IWorkspaceState _workspace = null!;

    protected WorkspaceDocumentSection(DataflowPipeline parent, IWorkspaceService workspaceService,
        ILogger<WorkspaceDocumentParserOrchestrator> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
        : base(parent, logger, settingsProvider, performance)
    {
        _workspaceService = workspaceService;
    }

    protected void UpdateDocumentState(DocumentParserState state, Func<DocumentParserState, DocumentParserState> update)
    {
        State = update(state);
        _workspace.LoadDocumentState(State);
    }

    private TransformBlock<WorkspaceFileUri, DocumentParserState> AcquireDocumentStateBlock { get; set; } = null!;
    private DocumentParserState AcquireDocumentState(WorkspaceFileUri uri)
    {
        var workspace = _workspace = _workspaceService.State.GetWorkspace(uri);
        if (workspace.TryGetWorkspaceFile(uri, out var state) && state != null)
        {
            if (state is DocumentParserState parserState)
            {
                State = parserState;
            }
            else
            {
                State = new DocumentParserState(state);
            }
            
            return State;
        }

        throw new InvalidOperationException("Document state was not found in the content store.");
    }

    protected sealed override (IEnumerable<IDataflowBlock> blocks, Task completion) DefineSectionBlocks(CancellationTokenSource? tokenSource)
    {
        TokenSource = tokenSource;

        AcquireDocumentStateBlock = new TransformBlock<WorkspaceFileUri, DocumentParserState>(AcquireDocumentState, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(AcquireDocumentStateBlock), AcquireDocumentStateBlock);

        var (blocks, completion) = DefineSectionBlocks(AcquireDocumentStateBlock);
        Completion = completion;

        return (new[] { AcquireDocumentStateBlock }.Concat(blocks), Completion);
    }

    protected abstract (IEnumerable<IDataflowBlock> blocks, Task completion) DefineSectionBlocks(ISourceBlock<DocumentParserState> source);
}
