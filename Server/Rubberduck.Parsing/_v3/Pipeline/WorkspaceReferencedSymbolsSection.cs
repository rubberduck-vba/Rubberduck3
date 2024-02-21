using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Collections.Immutable;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

public class WorkspaceReferencedSymbolsSection : DataflowPipelineSection<WorkspaceUri, IWorkspaceState>
{
    private IWorkspaceStateManager _workspaces;

    public WorkspaceReferencedSymbolsSection(DataflowPipeline parent, IWorkspaceStateManager workspaces,
        ILogger logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
        : base(parent, logger, settingsProvider, performance)
    {
        _workspaces = workspaces;
    }

    private TransformBlock<WorkspaceUri, IWorkspaceState> AcquireWorkspaceStateBlock { get; set; } = default!;
    private IWorkspaceState AcquireWorkspaceState(WorkspaceUri uri) =>
        RunTransformBlock(AcquireWorkspaceStateBlock, uri,
            e => _workspaces.GetWorkspace(uri),
            nameof(AcquireWorkspaceState), logPerformance: false);

    

    protected override ImmutableArray<(string Name, IDataflowBlock Block)> DataflowBlocks =>
        [
            (nameof(AcquireWorkspaceState), AcquireWorkspaceStateBlock)
        ];

    protected override (IEnumerable<IDataflowBlock> blocks, Task completion) DefineSectionBlocks(CancellationTokenSource? tokenSource)
    {
        throw new NotImplementedException();
    }
}
