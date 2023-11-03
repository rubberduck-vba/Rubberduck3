using Rubberduck.InternalApi.WebApi;
using Rubberduck.ServerPlatform.Model.Telemetry;
using System.Collections.Generic;

namespace Rubberduck.TelemetryServer
{
    public class TelemetryTransmitter : ITelemetryTransmitter
    {
        private readonly IPublicApiClient _api;

        public TelemetryTransmitter(IPublicApiClient api)
        {
            _api = api;
        }

        public void Transmit(IEnumerable<TelemetryEventPayload> payload) => _api.TransmitTelemetryAsync(payload).ConfigureAwait(false).GetAwaiter().GetResult();
    }
}
