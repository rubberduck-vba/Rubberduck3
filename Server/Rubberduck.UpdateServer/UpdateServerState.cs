using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.LanguagePlatform;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider.Model;
using System.Collections.Generic;
using System.Threading;

namespace Rubberduck.UpdateServer
{
    public enum ServerState
    {
        PreInit,
        Initialized,
        CheckingVersion,
        Downloading,
        DisconnectingClient,
        Updating,
        ReconnectingClient,
        Completed
    }

    public class UpdateServerState : ServerState<UpdateServerSettingsGroup, UpdateServerStartupSettings>
    {
        private readonly IExitHandler _exitHandler;

        public UpdateServerState(
            ILogger<ServerState<UpdateServerSettingsGroup, UpdateServerStartupSettings>> logger, 
            //ServerStartupOptions startupOptions, 
            IHealthCheckService<UpdateServerStartupSettings> healthCheck,
            IExitHandler exitHandler)
            : base(logger, healthCheck)
        {
            _exitHandler = exitHandler;
        }

        public ServerState ServerState { get; set; }

        protected override void OnClientProcessExited()
        {
            var normalClientOfflineStates = new HashSet<ServerState>(new[]
            {
                ServerState.DisconnectingClient,
                ServerState.Updating,
                ServerState.ReconnectingClient,
                ServerState.Completed
            });

            if (!normalClientOfflineStates.Contains(ServerState))
            {
                _exitHandler.Handle(new ExitParams(), CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }
    }
}