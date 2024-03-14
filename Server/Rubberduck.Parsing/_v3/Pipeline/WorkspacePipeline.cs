using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using Rubberduck.Parsing._v3.Pipeline.Services;

namespace Rubberduck.Parsing._v3.Pipeline;

/// <summary>
/// A <c>DataflowPipeline</c> that works with a <c>WorkspaceUri</c> to orchestrate the processing of the entire workspace.
/// </summary>
public class WorkspacePipeline : DataflowPipeline
{
    private readonly ILogger _logger;
    private readonly IWorkspaceStateManager _workspaces;
    private readonly ParserPipelineSectionProvider _sectionProvider;
    private readonly LibrarySymbolsService _librarySymbols;

    public WorkspacePipeline(IWorkspaceStateManager workspaces, ParserPipelineSectionProvider sectionProvider, LibrarySymbolsService librarySymbols,
        ILogger<WorkspacePipeline> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
        : base(logger, settingsProvider, performance)
    {
        _logger = logger;
        _workspaces = workspaces;
        _sectionProvider = sectionProvider;
        _librarySymbols = librarySymbols;
    }

    public IWorkspaceState? State => _workspaces.ActiveWorkspace;

    private WorkspaceReferencedSymbolsSection ReferencedSymbolsSection { get; set; } = default!;

    /// <summary>
    /// Creates a <c>DocumentParserSection</c> for each workspace file, to produce syntax trees and discover member symbols for each document in the workspace.
    /// </summary>
    private WorkspaceDocumentParserOrchestrator SyntaxOrchestration { get; set; } = default!;
    /// <summary>
    /// Creates a <c>DocumentMemberSymbolsSection</c> for each workspace file, to resolve all declaration symbols in the workspace.
    /// </summary>
    private WorkspaceMemberSymbolsOrchestrator MemberSymbolOrchestration { get; set; } = default!;
    /// <summary>
    /// Creates a <c>HierarchicalSymbolsSection</c> for each workspace file, to discover and resolve all hierarchical symbols and semantic tokens in the workspace.
    /// </summary>
    private WorkspaceHierarchicalSymbolsOrchestrator HierarchicalSymbolOrchestration { get; set; } = default!;

    public ILanguageServer Server { get; private set; }

    public async override Task StartAsync(ILanguageServer server, object input, CancellationTokenSource? tokenSource)
    {
        Server = server;
        DefinePipelineSections(server);

        await TryRunActionAsync(async () =>
        {
            var uri = (WorkspaceUri)input;
            await ProcessAsync(server, uri, tokenSource);

            LogTrace($"{nameof(WorkspacePipeline)} completed.");
        }, logPerformance: true);
    }

    private void DefinePipelineSections(ILanguageServer server)
    {
        ReferencedSymbolsSection = new WorkspaceReferencedSymbolsSection(this, _workspaces, _librarySymbols, server, _logger, SettingsProvider, Performance);
        SyntaxOrchestration = new WorkspaceDocumentParserOrchestrator(this, _workspaces, _sectionProvider, server, _logger, SettingsProvider, Performance);
        MemberSymbolOrchestration = new WorkspaceMemberSymbolsOrchestrator(this, _workspaces, _sectionProvider, server, _logger, SettingsProvider, Performance);
        HierarchicalSymbolOrchestration = new WorkspaceHierarchicalSymbolsOrchestrator(this, _workspaces, _sectionProvider, server, _logger, SettingsProvider, Performance);
        Completion = MemberSymbolOrchestration.Completion;
    }

    private async Task ProcessAsync(ILanguageServer server, WorkspaceUri uri, CancellationTokenSource? tokenSource)
    {
        // first collect the symbols from referenced libraries
        var referencedSymbols = ReferencedSymbolsSection.StartAsync(server, uri, tokenSource);

        // we collect the syntax trees at the same time (but we're probably already done with the libraries by now).
        var syntaxOrchestration = SyntaxOrchestration.StartAsync(server, uri, null, tokenSource);

        // must await completion of referenced symbols and syntax trees before we can resolve symbol types
        await Task.WhenAll(referencedSymbols, syntaxOrchestration);

        // then we can resolve member symbols...
        await MemberSymbolOrchestration.StartAsync(server, uri, null, tokenSource);

        //// ...and only then we know enough to collect and resolve the rest of the symbols.
        //await HierarchicalSymbolOrchestration.StartAsync(...);
    }
}
