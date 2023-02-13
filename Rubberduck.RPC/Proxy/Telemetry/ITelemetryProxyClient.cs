using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Telemetry.Model;
using System;

namespace Rubberduck.RPC.Proxy.SharedServices.Telemetry
{
    /// <summary>
    /// Client-to-server telemetry notifications.
    /// </summary>
    /// <remarks>
    /// Events are raised on the client side.
    /// </remarks>
    [JsonRpcSource]
    public interface ITelemetryProxyClient : IJsonRpcSource
    {
        /// <summary>
        /// The <strong>TelemetryEvent</strong> notification is sent from the LSP server to the IDE client to ask the client to log a telemetry event.
        /// </summary>
        [LspCompliant(JsonRpcMethods.ClientProxyRequests.Telemetry.TelemetryEvent)]
        event EventHandler<object> TelemetryEvent;

        /// <summary>
        /// Sents a <strong>TelemetryEvent</strong> notification to the Rubberduck.Telemetry server.
        /// </summary>
        void OnExceptionTelemetry(ExceptionTelemetry item);

        /// <summary>
        /// Sents a <strong>TelemetryEvent</strong> notification to the Rubberduck.Telemetry server.
        /// </summary>
        void OnEventTelemetry(EventTelemetry item);

        /// <summary>
        /// Sents a <strong>TelemetryEvent</strong> notification to the Rubberduck.Telemetry server.
        /// </summary>
        void OnTraceTelemetry(TraceTelemetry item);

        /// <summary>
        /// Sents a <strong>TelemetryEvent</strong> notification to the Rubberduck.Telemetry server.
        /// </summary>
        void OnRequestTelemetry(RequestTelemetry item);

        /// <summary>
        /// Sents a <strong>TelemetryEvent</strong> notification to the Rubberduck.Telemetry server.
        /// </summary>
        void OnMetricTelemetry(MetricTelemetry item);

        /// <summary>
        /// Sents a <strong>TelemetryEvent</strong> notification to the Rubberduck.Telemetry server.
        /// </summary>
        void OnPageViewTelemetry(PageViewTelemetry item);

        /// <summary>
        /// Sents a <strong>TelemetryEvent</strong> notification to the Rubberduck.Telemetry server.
        /// </summary>
        void OnDependencyTelemetry(DependencyTelemetry item);
    }
}
