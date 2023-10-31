using Rubberduck.ServerPlatform.Model.Telemetry;
using System.Collections.Generic;

namespace Rubberduck.TelemetryServer
{
    public interface ITelemetryTransmitter
    {
        void Transmit(IEnumerable<TelemetryEventPayload> payload);
    }
}
