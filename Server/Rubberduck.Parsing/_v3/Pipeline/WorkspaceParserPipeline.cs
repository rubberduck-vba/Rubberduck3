using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;

namespace Rubberduck.Parsing._v3.Pipeline;

public class WorkspaceParserPipeline : WorkspaceOrchestratorPipeline
{
    private readonly ParserPipelineProvider _pipelineProvider;

    public WorkspaceParserPipeline(ILogger<WorkspaceParserPipeline> logger,
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance,
        IWorkspaceStateManager workspaceManager,
        ParserPipelineProvider pipelineProvider)
        : base(logger, settingsProvider, performance, workspaceManager)
    {
        _pipelineProvider = pipelineProvider;
    }

    protected override WorkspaceDocumentPipeline StartDocumentPipeline(WorkspaceFileUri uri) => _pipelineProvider.StartNew(uri, TokenSource);
}
