using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Telemetry.Model;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices.Telemetry
{
    /// <summary>
    /// Client-to-server telemetry RPC requests.
    /// </summary>
    /// <remarks>
    /// Methods are implemented on the server side.
    /// </remarks>
    /// <typeparam name="TEvent">A common base class type for telemetry event items.</typeparam>
    public interface ITelemetryService : IConfigurableServerProxy<TelemetryOptions, ITelemetryClientService>
    {
        /// <summary>
        /// Saves the provided telemetry item locally.
        /// </summary>
        /// <typeparam name="T">The class type of the telemetry event/item.</typeparam>
        /// <param name="item">The telemetry event/item data.</param>
        /// <remarks>LSP supports telemetry, but does not define its model.</remarks>
        /// <returns>
        /// <c>true</c> if the event was successfully persisted.
        /// </returns>
        [RubberduckSP("telemetry/event")]
        Task<bool> OnEventAsync<T>(T item) where T : TelemetryEvent;

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
}
