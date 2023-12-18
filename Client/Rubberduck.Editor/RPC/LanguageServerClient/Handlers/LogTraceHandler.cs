using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.ServerPlatform;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Editor.RPC.LanguageServerClient.Handlers
{
    public class LogTraceHandler : LogTraceHandlerBase
    {
        private readonly ServerPlatformServiceHelper _service;

        public LogTraceHandler(ServerPlatformServiceHelper service)
        {
            _service = service;
        }

        public override async Task<Unit> Handle(LogTraceParams request, CancellationToken cancellationToken)
        {
            var service = _service;
            service.LogTrace("Received LogTrace request.");

            cancellationToken.ThrowIfCancellationRequested();

            var message = request.Message;
            var verbose = request.Verbose;

            _service.TryRunAction(() =>
            {
                service.LogTrace(message, verbose);
            }, nameof(LogTraceHandler));

            return await Task.FromResult(Unit.Value);
        }
    }
}
