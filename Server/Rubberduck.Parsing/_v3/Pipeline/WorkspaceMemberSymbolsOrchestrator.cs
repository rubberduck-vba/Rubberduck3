using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using Rubberduck.Parsing._v3.Pipeline.Services;
using System.Text;

namespace Rubberduck.Parsing._v3.Pipeline;

public class WorkspaceMemberSymbolsOrchestrator : WorkspaceOrchestratorSection
{
    public WorkspaceMemberSymbolsOrchestrator(DataflowPipeline parent, IAppWorkspacesStateManager workspaces, ParserPipelineSectionProvider pipelineProvider,
        ILanguageServer server, ILogger logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
        : base(parent, workspaces, pipelineProvider, server, logger, settingsProvider, performance)
    {
    }

    protected override WorkspaceDocumentSection StartDocumentPipeline(ParserPipelineSectionProvider provider, WorkspaceFileUri uri) => 
        provider.StartWorkspaceFileDocumentMemberResolverSection(LanguageServer, this, uri, TokenSource);
}
