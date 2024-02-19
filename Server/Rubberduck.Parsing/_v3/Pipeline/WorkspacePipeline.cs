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
        SyntaxOrchestration = new WorkspaceDocumentParserOrchestrator(this, _workspaces, sectionProvider, Logger, SettingsProvider, Performance);
        MemberSymbolOrchestration = new WorkspaceMemberSymbolsOrchestrator(this, _workspaces, sectionProvider, Logger, SettingsProvider, Performance);
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

    public async override Task StartAsync(object input, CancellationTokenSource? tokenSource)
    {
        var uri = (WorkspaceUri)input;

        await SyntaxOrchestration.StartAsync(uri, null, tokenSource);

            //.ContinueWith(t => MemberSymbolOrchestration.StartAsync(uri, null, tokenSource),
            //    Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default)

            //.ContinueWith(t => HierarchicalSymbolOrchestration.StartAsync(uri, null, tokenSource),
            //    Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default)
            ;

        LogTrace($"{nameof(WorkspacePipeline)} completed."); // BUG: this is a lie.
    }
}