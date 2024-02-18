using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Collections.Immutable;
using System.Threading.Tasks.Dataflow;


namespace Rubberduck.Parsing._v3.Pipeline;

public class WorkspaceSymbolsParserPipeline : DataflowPipelineSection<WorkspaceUri, ParserPipelineState>
{
    private readonly DocumentContentStore _contentStore;
    private readonly IWorkspaceStateManager _workspaceManager;
    private readonly PipelineParseTreeSymbolsService _symbolsService;

    public WorkspaceSymbolsParserPipeline(DataflowPipeline parent, DocumentContentStore contentStore, IWorkspaceStateManager workspaceManager, PipelineParseTreeSymbolsService symbolsService,
        ILogger<WorkspaceParserSection> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
        : base(parent, logger, settingsProvider, performance)
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

    protected override (IEnumerable<IDataflowBlock>, Task) DefineSectionBlocks(CancellationTokenSource? tokenSource)
    {
        var items = new List<IDataflowBlock>();
        TokenSource = tokenSource ?? TokenSource;

        AcquireWorkspaceBlock = new(AcquireWorkspaceState, ConcurrentExecutionOptions(Token));
        TraceBlockCompletionAsync(nameof(AcquireWorkspaceBlock), AcquireWorkspaceBlock);
        items.Add(AcquireWorkspaceBlock);

        PrioritizeFilesBlock = new(PrioritizeFiles, ConcurrentExecutionOptions(Token));
        TraceBlockCompletionAsync(nameof(PrioritizeFilesBlock), PrioritizeFilesBlock);
        items.Add(PrioritizeFilesBlock);

        AcquireWorkspaceDocumentsBlock = new(AcquireWorkspaceDocuments, ConcurrentExecutionOptions(Token));
        TraceBlockCompletionAsync(nameof(AcquireWorkspaceDocumentsBlock), AcquireWorkspaceDocumentsBlock);
        items.Add(AcquireWorkspaceDocumentsBlock);

        AcquireHierarchicalSymbolsBlock = new(AcquireHierarchicalSymbols, ConcurrentExecutionOptions(Token));
        TraceBlockCompletionAsync(nameof(AcquireHierarchicalSymbolsBlock), AcquireHierarchicalSymbolsBlock);
        items.Add(AcquireHierarchicalSymbolsBlock);

        Link(AcquireWorkspaceBlock, PrioritizeFilesBlock);
        Link(PrioritizeFilesBlock, AcquireWorkspaceDocumentsBlock);
        Link(AcquireWorkspaceDocumentsBlock, AcquireHierarchicalSymbolsBlock);

        return (
            [
                AcquireWorkspaceBlock, 
                PrioritizeFilesBlock, 
                AcquireWorkspaceDocumentsBlock, 
                AcquireHierarchicalSymbolsBlock
            ], 
            AcquireHierarchicalSymbolsBlock.Completion);
    }

    protected override ImmutableArray<(string, IDataflowBlock)> DataflowBlocks => new (string, IDataflowBlock)[]
    {
        (nameof(AcquireWorkspaceBlock), AcquireWorkspaceBlock),
        (nameof(PrioritizeFilesBlock), PrioritizeFilesBlock),
        (nameof(AcquireWorkspaceDocumentsBlock), AcquireWorkspaceDocumentsBlock),
        (nameof(AcquireHierarchicalSymbolsBlock), AcquireHierarchicalSymbolsBlock),
    }.ToImmutableArray();
}
