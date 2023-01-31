using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Telemetry.Model;
using System;

namespace Rubberduck.RPC.Proxy.SharedServices.Telemetry
{
    public class TelemetryClientService : ITelemetryClientService
    {
        public TelemetryOptions Configuration { get; set; }

        public event EventHandler<object> TelemetryEvent;

        public void OnTelemetryEvent<TEvent>(TEvent item) where TEvent : TelemetryEvent 
            => TelemetryEvent?.Invoke(this, item);
    }
}
