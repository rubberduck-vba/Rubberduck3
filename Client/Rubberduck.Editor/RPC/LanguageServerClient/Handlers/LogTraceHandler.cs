using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.UI.Services;
using Rubberduck.UI.Shell.Tools.ServerTrace;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rubberduck.Editor.RPC.LanguageServerClient.Handlers
{
    public class LogTraceHandler : LogTraceHandlerBase
    {
        private readonly UIServiceHelper _service;
        private readonly ILanguageServerTraceViewModel _traceToolwindow;

        public LogTraceHandler(UIServiceHelper service, ILanguageServerTraceViewModel traceToolWindow)
        {
            _service = service;
            _traceToolwindow = traceToolWindow;
        }

        public override async Task<Unit> Handle(LogTraceParams request, CancellationToken cancellationToken)
        {
            var service = _service;
            service.LogTrace("Received LogTrace request.");

            cancellationToken.ThrowIfCancellationRequested();

            var payload = JsonSerializer.Deserialize<LogMessagePayload>(request.Message)
                ?? throw new FormatException("Message payload was not in the expected format.");
            _traceToolwindow.OnServerMessage(payload);

            service.LogTrace(request.Message, request.Verbose);
            return await Task.FromResult(Unit.Value);
        }
    }
}
