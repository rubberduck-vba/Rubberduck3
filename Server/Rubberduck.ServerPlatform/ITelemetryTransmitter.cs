using Rubberduck.ServerPlatform.Model.Telemetry;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rubberduck.TelemetryServer
{
    public interface ITelemetryTransmitter
    {
        void Transmit(IEnumerable<TelemetryEventPayload> payload);
    }
}
