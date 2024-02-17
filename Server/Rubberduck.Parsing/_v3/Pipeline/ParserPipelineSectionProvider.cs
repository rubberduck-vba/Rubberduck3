using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace Rubberduck.Parsing._v3.Pipeline;

public class ParserPipelineSectionProvider
{
    private readonly IServiceProvider _provider;
    private readonly ConcurrentDictionary<Uri, IParserPipeline> _pipelines = [];
    private readonly ConcurrentDictionary<Uri, Task> _tasks = [];

    public ParserPipelineSectionProvider(IServiceProvider provider)
    {
        _provider = provider;
    }

    public WorkspaceParserSection StartNew(DataflowPipeline parent, WorkspaceUri uri, CancellationTokenSource? tokenSource)
    {
        _ = uri ?? throw new ArgumentNullException(nameof(uri));

        if (_pipelines.TryGetValue(uri, out var pipeline))
        {
            try
            {
                pipeline.Cancel();
            }
            finally
            {
                _tasks.Remove(uri, out _);
            }
        }

        var workspaces = _provider.GetRequiredService<IWorkspaceStateManager>();
        var logger = _provider.GetRequiredService<ILogger<WorkspaceParserSection>>();
        var settings = _provider.GetRequiredService<RubberduckSettingsProvider>();
        var performance = _provider.GetRequiredService<PerformanceRecordAggregator>();

        var newPipeline = new WorkspaceParserSection(parent, workspaces, this, logger, settings, performance);
        var completion = newPipeline.StartAsync(uri, null, tokenSource);

        _tasks.TryAdd(uri, completion);
        _pipelines[uri] = newPipeline;

        return newPipeline;
    }

    public WorkspaceDocumentSection StartNew(DataflowPipeline parent, WorkspaceFileUri uri, CancellationTokenSource? tokenSource)
    {
        _ = uri ?? throw new ArgumentNullException(nameof(uri));

        if (_pipelines.TryGetValue(uri, out var pipeline))
        {
            try
            {
                pipeline.Cancel();
            }
            finally
            {
                _tasks.Remove(uri, out _);
            }
        }

        var workspaces = _provider.GetRequiredService<IWorkspaceService>();
        var parser = _provider.GetRequiredService<PipelineParserService>();
        var symbols = _provider.GetRequiredService<PipelineParseTreeSymbolsService>();
        var logger = _provider.GetRequiredService<ILogger<WorkspaceParserSection>>();
        var settings = _provider.GetRequiredService<RubberduckSettingsProvider>();
        var performance = _provider.GetRequiredService<PerformanceRecordAggregator>();

        var newPipeline = new WorkspaceFileSection(parent, workspaces, parser, symbols, logger, settings, performance);
        _ = newPipeline.StartAsync(uri, null, tokenSource);        

        return newPipeline;
    }

    public WorkspaceParserSection? GetCurrent(WorkspaceUri uri) =>
        _pipelines.TryGetValue(uri, out var current) && current is WorkspaceParserSection result ? result : null;

    public WorkspaceFileSection? GetCurrent(WorkspaceFileUri uri) =>
        _pipelines.TryGetValue(uri, out var current) && current is WorkspaceFileSection result ? result : null;
}
