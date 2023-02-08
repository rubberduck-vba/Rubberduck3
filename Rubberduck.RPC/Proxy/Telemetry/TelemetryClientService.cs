using Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Telemetry.Model;
using System;

namespace Rubberduck.RPC.Proxy.SharedServices.Telemetry
{
    public class TelemetryClientService : ITelemetryProxyClient
    {
        public TelemetryOptions ServerOptions { get; set; }

        public event EventHandler<object> TelemetryEvent;
        public event EventHandler RequestExit;
        public event EventHandler<InitializedParams> Initialized;
        public event EventHandler<SetTraceParams> SetTrace;

        public void OnTelemetryEvent<TEvent>(TEvent item) where TEvent : TelemetryEvent 
            => TelemetryEvent?.Invoke(this, item);
    }
}
