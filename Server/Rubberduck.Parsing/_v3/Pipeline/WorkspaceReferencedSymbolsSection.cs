using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

public class WorkspaceReferencedSymbolsSection : DataflowPipelineSection<WorkspaceUri, IWorkspaceState>
{
    private IAppWorkspacesStateManager _workspaces;
    private readonly LibrarySymbolsService _librarySymbolsService;
    private readonly ILanguageServer _server;

    public WorkspaceReferencedSymbolsSection(DataflowPipeline parent, IAppWorkspacesStateManager workspaces, LibrarySymbolsService librarySymbolsService,
        ILanguageServer server, ILogger logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
        : base(parent, logger, settingsProvider, performance)
    {
        _workspaces = workspaces;
        _librarySymbolsService = librarySymbolsService;
        _server = server;
    }

    private TransformBlock<WorkspaceUri, IWorkspaceState> AcquireWorkspaceStateBlock { get; set; } = default!;
    private IWorkspaceState AcquireWorkspaceState(WorkspaceUri uri) =>
        RunTransformBlock(AcquireWorkspaceStateBlock, uri,
            e => State = _workspaces.GetWorkspace(uri),
            nameof(AcquireWorkspaceStateBlock), logPerformance: false);

    private TransformManyBlock<IWorkspaceState, Reference> AcquireLibraryReferencesBlock { get; set; } = default!;
    private IEnumerable<Reference> AcquireLibraryReferences(IWorkspaceState state) =>
        RunTransformBlock(AcquireLibraryReferencesBlock, state,
            e => state.References, 
            nameof(AcquireLibraryReferencesBlock), logPerformance: false);

    private TransformBlock<Reference, ProjectSymbol> LoadLibrarySymbolsBlock { get; set; } = default!;
    private ProjectSymbol LoadLibrarySymbols(Reference reference) =>
        RunTransformBlock(LoadLibrarySymbolsBlock, reference,
            e => _librarySymbolsService.LoadSymbolsFromTypeLibrary(reference),
            nameof(LoadLibrarySymbolsBlock), logPerformance: true);

    private ActionBlock<ProjectSymbol> SetLibrarySymbolStateBlock { get; set; } = default!;
    private void SetLibrarySymbolState(ProjectSymbol symbol) =>
        RunActionBlock(SetLibrarySymbolStateBlock, symbol,
            e =>
            {
                State.ExecutionContext.LoadReferencedLibrarySymbols(symbol);
            },
            nameof(SetLibrarySymbolStateBlock), logPerformance: false);


    protected override Dictionary<string, IDataflowBlock> DataflowBlocks => new()
    {
        [nameof(AcquireWorkspaceStateBlock)] = AcquireWorkspaceStateBlock,
        [nameof(AcquireLibraryReferencesBlock)] = AcquireLibraryReferencesBlock,
        [nameof(LoadLibrarySymbolsBlock)] = LoadLibrarySymbolsBlock,
        [nameof(SetLibrarySymbolStateBlock)] = SetLibrarySymbolStateBlock,
    };

    protected override (IEnumerable<IDataflowBlock> blocks, Task completion) DefineSectionBlocks(CancellationTokenSource? tokenSource)
    {
        AcquireWorkspaceStateBlock = new(AcquireWorkspaceState, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(AcquireWorkspaceStateBlock), AcquireWorkspaceStateBlock);

        AcquireLibraryReferencesBlock = new(AcquireLibraryReferences, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(AcquireLibraryReferencesBlock), AcquireLibraryReferencesBlock);

        LoadLibrarySymbolsBlock = new(LoadLibrarySymbols, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(LoadLibrarySymbolsBlock), LoadLibrarySymbolsBlock);

        SetLibrarySymbolStateBlock = new(SetLibrarySymbolState, ConcurrentExecutionOptions(Token));
        _ = TraceBlockCompletionAsync(nameof(SetLibrarySymbolStateBlock), SetLibrarySymbolStateBlock);

        Link(AcquireWorkspaceStateBlock, AcquireLibraryReferencesBlock);
        Link(AcquireLibraryReferencesBlock, LoadLibrarySymbolsBlock);
        Link(LoadLibrarySymbolsBlock, SetLibrarySymbolStateBlock);

        return (
            [
                AcquireWorkspaceStateBlock,
                AcquireLibraryReferencesBlock,
                LoadLibrarySymbolsBlock,
                SetLibrarySymbolStateBlock,
            ], SetLibrarySymbolStateBlock.Completion);
    }
}
