using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;

namespace Rubberduck.Parsing._v3.Pipeline;

public class WorkspaceParserSection : WorkspaceOrchestratorSection
{
    private readonly ParserPipelineSectionProvider _pipelineProvider;

    public WorkspaceParserSection(DataflowPipeline parent, IWorkspaceStateManager workspaces, ParserPipelineSectionProvider pipelineProvider,
        ILogger logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
        : base(parent, workspaces, logger, settingsProvider, performance)
    {
        _pipelineProvider = pipelineProvider;
    }

    protected override WorkspaceDocumentSection StartDocumentPipeline(WorkspaceFileUri uri) => _pipelineProvider.StartNew(this, uri, TokenSource);
}
