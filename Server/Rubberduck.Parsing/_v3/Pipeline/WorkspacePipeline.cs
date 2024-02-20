using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using Rubberduck.Parsing._v3.Pipeline.Services;
using System;

namespace Rubberduck.Parsing._v3.Pipeline;

/// <summary>
/// A <c>DataflowPipeline</c> that works with a <c>WorkspaceUri</c> to orchestrate the processing of the entire workspace.
/// </summary>
public class WorkspacePipeline : DataflowPipeline
{
    private readonly IWorkspaceStateManager _workspaces;

    public WorkspacePipeline(IWorkspaceStateManager workspaces, ParserPipelineSectionProvider sectionProvider,
        ILogger<WorkspacePipeline> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
        : base(logger, settingsProvider, performance)
    {
        _workspaces = workspaces;

        SyntaxOrchestration = new WorkspaceDocumentParserOrchestrator(this, _workspaces, sectionProvider, logger, settingsProvider, performance); 
        MemberSymbolOrchestration = new WorkspaceMemberSymbolsOrchestrator(this, _workspaces, sectionProvider, logger, settingsProvider, performance);
        HierarchicalSymbolOrchestration = new WorkspaceHierarchicalSymbolsOrchestrator(this, _workspaces, sectionProvider, logger, settingsProvider, performance);

        Completion = MemberSymbolOrchestration.Completion;
    }

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

    public async override Task StartAsync(object input, CancellationTokenSource? tokenSource) =>
        await TryRunActionAsync(async () =>
        {
            var uri = (WorkspaceUri)input;
            
            // first step is to collect the syntax trees.
            await SyntaxOrchestration.StartAsync(uri, null, tokenSource);

            // then we can resolve member symbols...
            await MemberSymbolOrchestration.StartAsync(uri, null, tokenSource);

            // ...and only then we know enough to collect and resolve the rest of the symbols.
            await HierarchicalSymbolOrchestration.StartAsync(uri, null, tokenSource);

            LogTrace($"{nameof(WorkspacePipeline)} completed.");
        }, logPerformance: true);
}
