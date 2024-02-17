using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;

namespace Rubberduck.Parsing._v3.Pipeline;

public class WorkspaceParserPipeline : DataflowPipeline
{
    private readonly IWorkspaceStateManager _workspaces;
    private readonly ParserPipelineSectionProvider _sectionProvider;

    public WorkspaceParserPipeline(IWorkspaceStateManager workspaces, ParserPipelineSectionProvider sectionProvider,
        ILogger<WorkspaceParserPipeline> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
        : base(logger, settingsProvider, performance)
    {
        _workspaces = workspaces;
        _sectionProvider = sectionProvider;

        Orchestrator = new WorkspaceParserSection(this, _workspaces, _sectionProvider, Logger, SettingsProvider, Performance);
    }

    private WorkspaceParserSection Orchestrator { get; set; } = default!;

    public override async Task RunPipelineAsync(WorkspaceUri uri, CancellationTokenSource? tokenSource)
    {
        await Orchestrator.StartAsync(uri, tokenSource);
        
        await Orchestrator.Completion;
        LogTrace("WorkspaceParserPipeline completed.");
    }

    protected override void LogPipelineCompletionState()
    {
        
    }
}