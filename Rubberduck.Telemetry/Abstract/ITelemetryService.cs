using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using Rubberduck.RPC.Telemetry;
using System;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices.Telemetry.Abstract
{
    /// <summary>
    /// Client-to-server telemetry RPC requests.
    /// </summary>
    /// <remarks>
    /// Methods are implemented on the server side.
    /// </remarks>
    public interface ITelemetryService : IConfigurableProxy<TelemetryOptions>
    {
        /// <summary>
        /// Saves the provided telemetry item locally.
        /// </summary>
        /// <typeparam name="TEvent">The class type of the telemetry event/item.</typeparam>
        /// <param name="item">The telemetry event/item data.</param>
        /// <remarks>LSP supports telemetry, but does not define its model.</remarks>
        /// <returns>
        /// <c>true</c> if the event was successfully persisted.
        /// </returns>
        [RubberduckSP("telemetry/event")]
        Task<bool> OnEventAsync<TEvent>(TEvent item) where TEvent : TelemetryEvent;

        /// <summary>
        /// Transmits untransmitted telemetry items to api.rubberduckvba.com over https.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the telemetry data was successfully transmitted.
        /// </returns>
        [RubberduckSP("telemetry/transmit")]
        Task<bool> TransmitAsync();

        /// <summary>
        /// Deletes all telemetry items stored locally.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the local telemetry data was successfully deleted.
        /// </returns>
        [RubberduckSP("telemetry/clear")]
        Task<bool> OnClearAsync();
    }

    /// <summary>
    /// Client-to-server telemetry notifications.
    /// </summary>
    /// <remarks>
    /// Events are raised on the client side.
    /// </remarks>
    public interface ITelemetryClientService
    {
        event EventHandler<TelemetryEvent> TelemetryEvent;
    }
}
