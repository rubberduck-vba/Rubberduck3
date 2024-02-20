using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using Rubberduck.Parsing._v3.Pipeline.Services;

namespace Rubberduck.Parsing._v3.Pipeline;

public class WorkspaceHierarchicalSymbolsOrchestrator : WorkspaceOrchestratorSection
{
    public WorkspaceHierarchicalSymbolsOrchestrator(DataflowPipeline parent, IWorkspaceStateManager workspaces, ParserPipelineSectionProvider pipelineProvider,
        ILogger logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
        : base(parent, workspaces, pipelineProvider, logger, settingsProvider, performance)
    {
    }

    protected override WorkspaceDocumentSection StartDocumentPipeline(ParserPipelineSectionProvider provider, WorkspaceFileUri uri) =>
        provider.StartWorkspaceFileHierarchicalSymbolsSection(this, uri, TokenSource);
}