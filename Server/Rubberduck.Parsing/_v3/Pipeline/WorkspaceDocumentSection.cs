using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

public abstract class WorkspaceDocumentSection : DataflowPipelineSection<WorkspaceFileUri, DocumentParserState>
{
    private readonly IWorkspaceService _workspaceService;
    
    protected WorkspaceDocumentSection(DataflowPipeline parent, IWorkspaceService workspaceService, 
        ILogger<WorkspaceParserSection> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
        : base(parent, logger, settingsProvider, performance)
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

    protected sealed override (IEnumerable<IDataflowBlock> blocks, Task completion) DefineSectionBlocks(CancellationTokenSource? tokenSource)
    {
        TokenSource = tokenSource;

        AcquireDocumentStateBlock = new TransformBlock<WorkspaceFileUri, DocumentParserState>(AcquireDocumentState, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(AcquireDocumentStateBlock), AcquireDocumentStateBlock);

        var (blocks, completion) = DefinePipelineBlocks(AcquireDocumentStateBlock);
        Completion = completion;

        return (new[] { AcquireDocumentStateBlock }.Concat(blocks), Completion);
    }

    protected abstract (IEnumerable<IDataflowBlock> blocks, Task completion) DefinePipelineBlocks(ISourceBlock<DocumentParserState> source);
}
