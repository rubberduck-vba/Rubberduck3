using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.LanguagePlatform;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Rubberduck.LanguageServer
{
    public interface ILanguageServerState : IServerStateWriter
    {
        void AddWorkspaceFolders(IEnumerable<WorkspaceFolder> workspaceFolders);
    }

    public class LanguageServerState : ServerState<LanguageServerSettings>, ILanguageServerState
    {
        private readonly IExitHandler _exitHandler;

        public LanguageServerState(ILogger<LanguageServerState> logger, 
            ServerStartupOptions startupOptions,
            IHealthCheckService<LanguageServerSettings> healthCheck,
            IExitHandler exitHandler)
            : base(logger, startupOptions, healthCheck)
        {
            _exitHandler = exitHandler;
        }

        private Container<WorkspaceFolder>? _workspaceFolders;
        public IEnumerable<WorkspaceFolder> Workspacefolders => _workspaceFolders ?? throw new ServerStateNotInitializedException();

        public void AddWorkspaceFolders(IEnumerable<WorkspaceFolder> workspaceFolders)
        {
            _workspaceFolders = _workspaceFolders?.Concat(workspaceFolders).ToContainer() ?? throw new ServerStateNotInitializedException();
        }

        protected override void OnInitialize(InitializeParams param)
        {
            _workspaceFolders = param.WorkspaceFolders!;
        }

        protected override void OnClientProcessExited()
        {
            _exitHandler.Handle(new ExitParams(), CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}