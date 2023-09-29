using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Threading;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Common;
using Rubberduck.SettingsProvider;
using System.Linq;

namespace Rubberduck.Client.Handlers
{
    public class WorkspaceFoldersHandler : WorkspaceFoldersHandlerBase
    {
        private readonly ILogger<WorkspaceFoldersHandler> _logger;
        private readonly ISettingsProvider<LanguageServerSettings> _settingsProvider;

        TraceLevel TraceLevel => _settingsProvider.Value.Settings.TraceLevel.ToTraceLevel();

        public WorkspaceFoldersHandler(ILogger<WorkspaceFoldersHandler> logger, ISettingsProvider<LanguageServerSettings> settingsProvider)
        {
            _logger = logger;
            _settingsProvider = settingsProvider;
        }

        public override async Task<Container<WorkspaceFolder>?> Handle(WorkspaceFolderParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Container<WorkspaceFolder> response = null!;

            try
            {
                var elapsed = TimedAction.Run(() =>
                {
                    var workspaceFolders = Enumerable.Empty<WorkspaceFolder>();
                    response = new Container<WorkspaceFolder>(workspaceFolders);
                });

                _logger.LogPerformance("Handled WorkspaceFolders request.", elapsed, TraceLevel);

                return await Task.FromResult(response);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, TraceLevel);
                throw;
            }
        }
    }
}
