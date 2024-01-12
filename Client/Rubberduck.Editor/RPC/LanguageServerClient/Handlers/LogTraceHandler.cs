using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.ServerPlatform;
using Rubberduck.UI.Shell.Tools.ServerTrace;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Editor.RPC.LanguageServerClient.Handlers
{
    public class LogTraceHandler : LogTraceHandlerBase
    {
        private readonly ServerPlatformServiceHelper _service;
        private readonly ILanguageServerTraceViewModel _traceToolwindow;

        public LogTraceHandler(ServerPlatformServiceHelper service, ILanguageServerTraceViewModel traceToolWindow)
        {
            _service = service;
            _traceToolwindow = traceToolWindow;
        }

        public override async Task<Unit> Handle(LogTraceParams request, CancellationToken cancellationToken)
        {
            var service = _service;
            service.LogTrace("Received LogTrace request.");

            cancellationToken.ThrowIfCancellationRequested();

            var message = request.Message;
            var verbose = request.Verbose;

            _traceToolwindow.OnServerTrace(message, verbose);

            _service.TryRunAction(() =>
            {
                service.LogTrace(message, verbose);
            }, nameof(LogTraceHandler));

            return await Task.FromResult(Unit.Value);
        }
    }
}
