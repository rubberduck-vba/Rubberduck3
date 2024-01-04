using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Rubberduck.Editor.RPC.EditorServer
{
    public interface ILanguageServerState : IServerStateWriter
    {
        void AddWorkspaceFolders(IEnumerable<WorkspaceFolder> workspaceFolders);
    }

    public class EditorServerState : ServerState<LanguageClientSettings, LanguageClientStartupSettings>, ILanguageServerState
    {
        private readonly IExitHandler _exitHandler;

        private readonly HashSet<WorkspaceFolder> _workspaceFolders = new();

        public EditorServerState(ILogger<EditorServerState> logger,
            IHealthCheckService<LanguageClientStartupSettings> healthCheck,
            IExitHandler exitHandler)
            : base(logger, healthCheck)
        {
            _exitHandler = exitHandler;
        }

        public IEnumerable<WorkspaceFolder> WorkspaceFolders { get; set; }

        public DocumentUri RootUri { get; set; }

        public void AddWorkspaceFolders(IEnumerable<WorkspaceFolder> workspaceFolders)
        {
            _workspaceFolders.UnionWith(workspaceFolders);
        }

        protected override void OnInitialize(InitializeParams param)
        {
            var folders = param.WorkspaceFolders ?? throw new ArgumentException("Expected non-empty WorkspaceFolders collection.");
            var root = param.RootUri?.ToUri().LocalPath ?? param.RootPath ?? throw new ArgumentException("Expected non-null root URI or root path.");

            _workspaceFolders.Clear();
            _workspaceFolders.UnionWith(folders);
            RootUri = new Uri(root);
        }

        protected override void OnClientProcessExited()
        {
            _exitHandler.Handle(new ExitParams(), CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}