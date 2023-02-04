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
    public interface ITelemetryClientService : ITelemetryClientProxy, IJsonRpcSource
    {
        /// <summary>
        /// Sents a <strong>TelemetryEvent</strong> notification to the Rubberduck.Telemetry server.
        /// </summary>
        void OnTelemetryEvent<TEvent>(TEvent item) where TEvent : TelemetryEvent;
    }

    public interface ITelemetryClientProxy : IConfigurableProxy<TelemetryOptions>
    {
        /// <summary>
        /// The <strong>TelemetryEvent</strong> notification is sent from the LSP server to the IDE client to ask the client to log a telemetry event.
        /// </summary>
        [LspCompliant(JsonRpcMethods.ClientProxyRequests.Telemetry.TelemetryEvent)]
        event EventHandler<object> TelemetryEvent;
    }
}
