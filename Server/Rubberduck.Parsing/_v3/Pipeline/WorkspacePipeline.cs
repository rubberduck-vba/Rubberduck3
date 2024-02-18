using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System;

namespace Rubberduck.Parsing._v3.Pipeline;

public class WorkspacePipeline : DataflowPipeline
{
    private readonly IWorkspaceStateManager _workspaces;

    public WorkspacePipeline(IWorkspaceStateManager workspaces, ParserPipelineSectionProvider sectionProvider,
        ILogger<WorkspacePipeline> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
        : base(logger, settingsProvider, performance)
    {
        _workspaces = workspaces;
        Orchestrator = new WorkspaceParserSection(this, _workspaces, sectionProvider, Logger, SettingsProvider, Performance);
    }

    private WorkspaceParserSection Orchestrator { get; set; } = default!;

    public async override Task StartAsync(object input, object? state, CancellationTokenSource? tokenSource)
    {
        var uri = (WorkspaceUri)input;
        Completion = Orchestrator.StartAsync(uri, state, tokenSource);
        await Completion;
        LogTrace($"{nameof(WorkspacePipeline)} completed.");
    }

    protected override void LogPipelineCompletionState()
    {
        throw new NotImplementedException();
    }
}