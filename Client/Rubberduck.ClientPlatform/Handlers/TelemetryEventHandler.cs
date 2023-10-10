using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using System.Threading.Tasks;
using MediatR;
using System.Threading;

namespace Rubberduck.Client.Handlers
{
    public class TelemetryEventHandler : TelemetryEventHandlerBase
    {
        public override async Task<Unit> Handle(TelemetryEventParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // _telemetryClient.OnTelemetryEvent(request, cancellationToken);

            return await Task.FromResult(Unit.Value);
        }
    }
}
