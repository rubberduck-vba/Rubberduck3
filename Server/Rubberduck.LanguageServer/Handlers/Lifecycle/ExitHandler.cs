using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.ServerPlatform;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers.Lifecycle
{
    public class ExitHandler : ExitHandlerBase
    {
        /* LSP 3.17 Exit Notification
         * A notification to ask the server to exit its process. 
         * The server should exit with success code 0 if the shutdown request has been received before; otherwise with error code 1.
        */

        private readonly ServerPlatformServiceHelper _service;
        private readonly Func<LanguageServerState> _state;

        public ExitHandler(ServerPlatformServiceHelper service, Func<LanguageServerState> state)
        {
            _service = service;
            _state = state;
        }

        public async override Task<Unit> Handle(ExitParams request, CancellationToken cancellationToken)
        {
            var service = _service;
            service.LogTrace("Received Exit notification.");

            cancellationToken.ThrowIfCancellationRequested();
            var state = _state();

            service.TryRunAction(() =>
            {
                service.LogInformation("Handling exit notification...", $"ExitCode: {(state.IsCleanExit ? 0 : 1)}");
                if (state.IsCleanExit)
                {
                    Environment.Exit(0);
                }
                else
                {
                    Environment.Exit(1);
                }
            }, nameof(ExitHandler));

            return await Task.FromResult(Unit.Value);
        }
    }
}