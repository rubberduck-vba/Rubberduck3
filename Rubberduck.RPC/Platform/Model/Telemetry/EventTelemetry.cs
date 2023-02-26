using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Platform.Model.Telemetry
{
    /// <summary>
    /// <strong>EventTelemetry</strong> represents an event that occurred in the application. 
    /// Typically it is a user interaction such as a button click or order checkout.
    /// It can also be an application life cycle event like initialization or configuration update.
    /// </summary>
    /// <remarks>
    /// Semantically, events may or may not be correlated to requests. 
    /// However if used properly, event telemetry is more important than requests or traces.
    /// Events represent <em>business telemetry</em> and should be subject to separate, less aggressive sampling.
    /// </remarks>
    public class EventTelemetry : TelemetryEvent
    {
        public EventTelemetry() : base(TelemetryEventName.Event) { }

        /// <summary>
        /// The name of the event.
        /// </summary>
        /// <remarks>
        /// Maximum length: 512
        /// </remarks>
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
