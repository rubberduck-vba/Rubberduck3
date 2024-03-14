using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using Rubberduck.Parsing._v3.Pipeline.Services;

namespace Rubberduck.Parsing._v3.Pipeline;

/// <summary>
/// A <c>DataflowPipeline</c> that works with a <c>WorkspaceFileUri</c> to process the active document.
/// </summary>
public class DocumentPipeline : DataflowPipeline
{
    private readonly ILogger _logger;
    private readonly IWorkspaceStateManager _workspaces;
    private readonly ParserPipelineSectionProvider _sectionProvider;

    public DocumentPipeline(IWorkspaceStateManager workspaces, ParserPipelineSectionProvider sectionProvider,
        ILogger<WorkspacePipeline> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
        : base(logger, settingsProvider, performance)
    {
        _logger = logger;
        _workspaces = workspaces;
        _sectionProvider = sectionProvider;
    }

    public IWorkspaceState? State => _workspaces.ActiveWorkspace;

    public async override Task StartAsync(ILanguageServer server, object input, CancellationTokenSource? tokenSource)
    {
        var uri = (WorkspaceFileUri)input;
        CancelCurrent(uri);

        await TryRunActionAsync(async () =>
        {
            await _sectionProvider.StartWorkspaceFileParserSection(server, this, uri, tokenSource).Completion;
            await _sectionProvider.StartWorkspaceFileDocumentMemberResolverSection(server, this, uri, tokenSource).Completion;
            //await _sectionProvider.StartWorkspaceFileHierarchicalSymbolsSection(server, this, uri, tokenSource).Completion;

            // TODO orchestrate (from caller) affected URIs to re-resolve

            Completion = Task.CompletedTask;

            LogTrace($"{nameof(WorkspacePipeline)} completed.");
        }, logPerformance: true);
    }

    private void CancelCurrent(WorkspaceFileUri uri)
    {
        var current = _sectionProvider.GetCurrent(uri);
        if (current != null)
        {
            LogTrace($"Cancelling current document pipeline", $"WorkspaceFileUri: {uri}");
            try
            {
                current.Cancel();
            }
            catch (Exception exception)
            {
                LogTrace($"Caught {exception.GetType()} exception");
            }
        }
    }
}
