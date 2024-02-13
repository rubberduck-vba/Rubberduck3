using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Threading.Tasks.Dataflow;


namespace Rubberduck.Parsing._v3.Pipeline;

public class WorkspaceSymbolsParserPipeline : ParserPipeline<WorkspaceUri, ParserPipelineState> // really <WorkspaceUri, ParserPipelineState>, but simpler for DI/IoC that way
{
    private readonly DocumentContentStore _contentStore;
    private readonly IWorkspaceStateManager _workspaceManager;
    private readonly PipelineParseTreeSymbolsService _symbolsService;

    public WorkspaceSymbolsParserPipeline(ILogger<WorkspaceParserPipeline> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance,
        DocumentContentStore contentStore,
        IWorkspaceStateManager workspaceManager,
        PipelineParseTreeSymbolsService symbolsService) 
        : base(logger, settingsProvider, performance)
    {
        _contentStore = contentStore;
        _workspaceManager = workspaceManager;
        _symbolsService = symbolsService;
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

    private TransformBlock<WorkspaceFileUri, DocumentParserState> AcquireWorkspaceDocumentsBlock { get; set; } = null!;
    private DocumentParserState AcquireWorkspaceDocuments(WorkspaceFileUri uri) =>
        RunTransformBlock(AcquireWorkspaceDocumentsBlock, uri, e => (DocumentParserState)_contentStore.GetDocument(e)
            ?? throw new InvalidOperationException("Document state was not found in the content store."));

    private ActionBlock<DocumentParserState> AcquireHierarchicalSymbolsBlock { get; set; } = null!;
    private void AcquireHierarchicalSymbols(DocumentParserState document) =>
        RunActionBlock(AcquireHierarchicalSymbolsBlock, document, e => 
        {
            var syntaxTree = e.SyntaxTree ?? throw new InvalidOperationException("Syntax tree was unexpectedly null.");
            ThrowIfCancellationRequested();

            var symbols = _symbolsService.DiscoverHierarchicalSymbols(syntaxTree, e.Uri);
            ThrowIfCancellationRequested();

            var resolved = _symbolsService.RecursivelyResolveSymbols(symbols);
            ThrowIfCancellationRequested();

            State.Documents[e.Uri] = e.WithSymbol(resolved);
        });

    protected override (ITargetBlock<WorkspaceUri> inputBlock, Task completion) DefinePipelineBlocks()
    {
        AcquireWorkspaceBlock = new(AcquireWorkspaceState, ConcurrentExecutionOptions);
        PrioritizeFilesBlock = new(PrioritizeFiles, ConcurrentExecutionOptions);
        AcquireWorkspaceDocumentsBlock = new(AcquireWorkspaceDocuments, ConcurrentExecutionOptions);
        AcquireHierarchicalSymbolsBlock = new(AcquireHierarchicalSymbols, ConcurrentExecutionOptions);

        Link(AcquireWorkspaceBlock, PrioritizeFilesBlock, WithCompletionPropagation);
        Link(PrioritizeFilesBlock, AcquireWorkspaceDocumentsBlock, WithCompletionPropagation);
        Link(AcquireWorkspaceDocumentsBlock, AcquireHierarchicalSymbolsBlock, WithCompletionPropagation);

        return (AcquireWorkspaceBlock, AcquireHierarchicalSymbolsBlock.Completion);
    }
}
