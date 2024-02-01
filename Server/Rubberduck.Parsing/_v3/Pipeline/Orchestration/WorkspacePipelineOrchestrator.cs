using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;

namespace Rubberduck.Parsing._v3.Pipeline.Orchestration
{
    public class WorkspacePipelineOrchestrator : ServiceBase
    {
        private readonly IParserPipelineProvider<WorkspaceUri> _workspacePipelineProvider;
        private readonly IParserPipelineProvider<WorkspaceFileUri> _filePipelineProvider;

        public WorkspacePipelineOrchestrator(ILogger logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance,
            IParserPipelineProvider<WorkspaceUri> workspacePipelineProvider,
            IParserPipelineProvider<WorkspaceFileUri> filePipelineProvider) 
            : base(logger, settingsProvider, performance)
        {
            _workspacePipelineProvider = workspacePipelineProvider;
            _filePipelineProvider = filePipelineProvider;
        }

        public Task ProcessWorkspaceAsync(WorkspaceUri uri) => _workspacePipelineProvider.StartNew(uri).Completion;

        public Task ProcessWorkspaceFileAsync(WorkspaceFileUri uri) => _filePipelineProvider.StartNew(uri).Completion;

        public void Cancel(WorkspaceUri uri) => _workspacePipelineProvider.GetCurrent(uri)?.Cancel();
        public void Cancel(WorkspaceFileUri uri) => _filePipelineProvider.GetCurrent(uri)?.Cancel();
    }
}
