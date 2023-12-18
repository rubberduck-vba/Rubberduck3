using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.ServerPlatform;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Editor.RPC.EditorServer.Handlers.Lifecycle
{
    public class ExitHandler : ExitHandlerBase
    {
        /* LSP 3.17 Exit Notification
         * A notification to ask the server to exit its process. 
         * The server should exit with success code 0 if the shutdown request has been received before; otherwise with error code 1.
        */ 

        private readonly ServerPlatformServiceHelper _service;
        private readonly Func<EditorServerState> _state;

        public ExitHandler(ServerPlatformServiceHelper service, Func<EditorServerState> state)
        {
            _service = service;
            _state = state;
        }

        public async override Task<Unit> Handle(ExitParams request, CancellationToken cancellationToken)
        {
            _service.LogTrace("Received Exit notification.");
            cancellationToken.ThrowIfCancellationRequested();
            _service.RunAction(() =>
            {
                if (_state().IsCleanExit)
                {
                    _service.LogInformation("Exiting process...", $"ExitCode: {0}");
                    Environment.Exit(0);
                }
                else
                {
                    _service.LogWarning("Exiting process...", $"ExitCode: {1}");
                    Environment.Exit(1);
                }
            });
            return await Task.FromResult(Unit.Value);
        }
    }
}