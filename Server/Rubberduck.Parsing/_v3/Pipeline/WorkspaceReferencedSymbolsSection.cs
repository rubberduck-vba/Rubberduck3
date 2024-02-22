using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Collections.Immutable;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

public class WorkspaceReferencedSymbolsSection : DataflowPipelineSection<WorkspaceUri, IWorkspaceState>
{
    private IWorkspaceStateManager _workspaces;
    private readonly LibrarySymbolsService _librarySymbolsService;

    public WorkspaceReferencedSymbolsSection(DataflowPipeline parent, IWorkspaceStateManager workspaces, LibrarySymbolsService librarySymbolsService,
        ILogger logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
        : base(parent, logger, settingsProvider, performance)
    {
        _workspaces = workspaces;
        _librarySymbolsService = librarySymbolsService;
    }

    private TransformBlock<WorkspaceUri, IWorkspaceState> AcquireWorkspaceStateBlock { get; set; } = default!;
    private IWorkspaceState AcquireWorkspaceState(WorkspaceUri uri) =>
        RunTransformBlock(AcquireWorkspaceStateBlock, uri,
            e => _workspaces.GetWorkspace(uri),
            nameof(AcquireWorkspaceState), logPerformance: false);

    private TransformManyBlock<IWorkspaceState, Reference> AcquireLibraryReferencesBlock { get; set; } = default!;
    private IEnumerable<Reference> AcquireLibraryReferences(IWorkspaceState state) =>
        RunTransformBlock(AcquireLibraryReferencesBlock, state,
            e => state.References, 
            nameof(AcquireLibraryReferences), logPerformance: false);

    private TransformBlock<Reference, ProjectSymbol> LoadLibrarySymbolsBlock { get; set; } = default!;
    private ProjectSymbol LoadLibrarySymbols(Reference reference) =>
        RunTransformBlock(LoadLibrarySymbolsBlock, reference,
            e => _librarySymbolsService.LoadSymbolsFromTypeLibrary(reference),
            nameof(LoadLibrarySymbols), logPerformance: true);

    private ActionBlock<ProjectSymbol> SetLibrarySymbolStateBlock { get; set; } = default!;
    private void SetLibrarySymbolState(ProjectSymbol symbol) =>
        RunActionBlock(SetLibrarySymbolStateBlock, symbol,
            e => State.ExecutionContext.LoadReferencedLibrarySymbols(symbol),
            nameof(SetLibrarySymbolState), logPerformance: false);


    protected override ImmutableArray<(string Name, IDataflowBlock Block)> DataflowBlocks =>
        [
            (nameof(AcquireWorkspaceState), AcquireWorkspaceStateBlock),
            (nameof(AcquireLibraryReferences), AcquireLibraryReferencesBlock),
            (nameof(LoadLibrarySymbols), LoadLibrarySymbolsBlock),
            (nameof(SetLibrarySymbolState), SetLibrarySymbolStateBlock),
        ];

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
