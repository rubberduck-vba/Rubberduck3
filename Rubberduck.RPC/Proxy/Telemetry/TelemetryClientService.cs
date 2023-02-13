using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Client;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Telemetry.Model;
using StreamJsonRpc;
using System;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices.Telemetry
{
    [JsonRpcSource]
    public class TelemetryClientService : /*JsonRpcClientSideServerProxyService<ITelemetryServerProxy>,*/ IConfigurableProxy<TelemetryOptions>, ITelemetryProxyClient
    {
        public TelemetryClientService(IServerStateService<SharedServerCapabilities> serverStateService)
        {
            _serverStateService = serverStateService;
        }

        private readonly IServerStateService<SharedServerCapabilities> _serverStateService;

        [JsonRpcIgnore]
        public async Task<TelemetryOptions> GetServerOptionsAsync() => await Task.FromResult(_serverStateService.Configuration.TelemetryOptions);

        public event EventHandler<object> TelemetryEvent;

        private void OnTelemetryEvent<TEvent>(TEvent item) where TEvent : TelemetryEvent
            => TelemetryEvent?.Invoke(this, item);

        public void OnDependencyTelemetry(DependencyTelemetry item) => OnTelemetryEvent(item);

        public void OnEventTelemetry(EventTelemetry item) => OnTelemetryEvent(item);

        public void OnExceptionTelemetry(ExceptionTelemetry item) => OnTelemetryEvent(item);

        public void OnMetricTelemetry(MetricTelemetry item) => OnTelemetryEvent(item);

        public void OnPageViewTelemetry(PageViewTelemetry item) => OnTelemetryEvent(item);

        public void OnRequestTelemetry(RequestTelemetry item) => OnTelemetryEvent(item);

        public void OnTraceTelemetry(TraceTelemetry item) => OnTelemetryEvent(item);
    }
}
