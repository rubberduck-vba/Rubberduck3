using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Collections.Concurrent;

namespace Rubberduck.Parsing._v3.Pipeline.Services;

public class ParserPipelineSectionProvider
{
    private readonly IServiceProvider _provider;
    private readonly ConcurrentDictionary<Uri, DataflowPipeline> _pipelines = [];
    private readonly ConcurrentDictionary<Uri, Task> _tasks = [];

    public ParserPipelineSectionProvider(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task AwaitCompletionAsync() => await Task.WhenAll(_tasks.Values);

    /// <summary>
    /// Starts a new <c>WorkspaceFileParserSection</c> pipeline for the specified <c>WorkspaceFileUri</c>.
    /// </summary>
    /// <remarks>
    /// This pipeline creates a syntax tree for a document, then traverses it to discover (but not resolve) all member definition symbols.
    /// </remarks>
    public WorkspaceDocumentSection StartWorkspaceFileParserSection(ILanguageServer server, DataflowPipeline parent, WorkspaceFileUri uri, CancellationTokenSource? tokenSource)
    {
        _ = uri ?? throw new ArgumentNullException(nameof(uri));

        if (_pipelines.TryGetValue(uri, out var pipeline))
        {
            try
            {
                pipeline.Cancel();
            }
            catch(TaskCanceledException) { }
            finally
            {
                _tasks.Remove(uri, out _);
            }
        }

        var workspaces = _provider.GetRequiredService<IWorkspaceService>();
        var parser = _provider.GetRequiredService<PipelineParserService>();
        var foldings = _provider.GetRequiredService<FoldingRangesParseTreeService>();
        var symbols = _provider.GetRequiredService<PipelineParseTreeSymbolsService>();
        var logger = _provider.GetRequiredService<ILogger<WorkspaceDocumentParserOrchestrator>>();
        var settings = _provider.GetRequiredService<RubberduckSettingsProvider>();
        var performance = _provider.GetRequiredService<PerformanceRecordAggregator>();

        var newPipeline = new DocumentParserSection(parent, workspaces, parser, foldings, symbols, server, logger, settings, performance);
        var completion = newPipeline.StartAsync(server, uri, null, tokenSource);

        _tasks.TryAdd(uri, completion);
        _pipelines[uri] = newPipeline;

        return newPipeline;
    }

    /// <summary>
    /// Starts a new <c>DocumentMembersSection</c> pipeline for the specified <c>WorkspaceFileUri</c>.
    /// </summary>
    /// <remarks>
    /// This pipeline acquires the member symbols of a document and resolves a type for each one.
    /// </remarks>
    public WorkspaceDocumentSection StartWorkspaceFileDocumentMemberResolverSection(ILanguageServer server, DataflowPipeline parent, WorkspaceFileUri uri, CancellationTokenSource? tokenSource)
    {
        _ = uri ?? throw new ArgumentNullException(nameof(uri));

        var workspaces = _provider.GetRequiredService<IWorkspaceService>();
        var symbols = _provider.GetRequiredService<PipelineParseTreeSymbolsService>();
        var logger = _provider.GetRequiredService<ILogger<WorkspaceDocumentParserOrchestrator>>();
        var settings = _provider.GetRequiredService<RubberduckSettingsProvider>();
        var performance = _provider.GetRequiredService<PerformanceRecordAggregator>();

        var newPipeline = new DocumentMemberSymbolsSection(parent, workspaces, symbols, logger, settings, performance);
        var completion = newPipeline.StartAsync(server, uri, null, tokenSource);

        _tasks.TryAdd(uri, completion);
        _pipelines[uri] = newPipeline;

        return newPipeline;
    }

    /// <summary>
    /// Starts a new <c>HierarchicalSymbolsSection</c> pipeline for the specified <c>WorkspaceFileUri</c>.
    /// </summary>
    /// <remarks>
    /// This pipeline traverses the syntax tree of a document, discovers all symbols, and resolves a type for each one.
    /// </remarks>
    public WorkspaceDocumentSection StartWorkspaceFileHierarchicalSymbolsSection(ILanguageServer server, DataflowPipeline parent, WorkspaceFileUri uri, CancellationTokenSource? tokenSource)
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
        var symbols = _provider.GetRequiredService<PipelineParseTreeSymbolsService>();
        var logger = _provider.GetRequiredService<ILogger<WorkspaceDocumentParserOrchestrator>>();
        var settings = _provider.GetRequiredService<RubberduckSettingsProvider>();
        var performance = _provider.GetRequiredService<PerformanceRecordAggregator>();

        var newPipeline = new DocumentHierarchicalSymbolsSection(parent, workspaces, symbols, logger, settings, performance);
        var completion = newPipeline.StartAsync(server, uri, null, tokenSource);

        _tasks.TryAdd(uri, completion);
        _pipelines[uri] = newPipeline;

        return newPipeline;
    }

    public WorkspaceDocumentParserOrchestrator? GetCurrent(WorkspaceUri uri) =>
        _pipelines.TryGetValue(uri, out var current) && current is WorkspaceDocumentParserOrchestrator result ? result : null;

    public DocumentParserSection? GetCurrent(WorkspaceFileUri uri) =>
        _pipelines.TryGetValue(uri, out var current) && current is DocumentParserSection result ? result : null;
}
