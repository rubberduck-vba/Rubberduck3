using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.SharedServices.Telemetry.Model
{
    /// <summary>
    /// <strong>TraceTelemetry</strong> represents trace logs that can later be searched.
    /// </summary>
    /// <remarks>
    /// NLog.ILogger entries translate into this type of telemetry event.
    /// </remarks>
    public class TraceTelemetry : TelemetryEvent
    {
        public TraceTelemetry() : base(TelemetryEventName.Trace) { }

        /// <summary>
        /// The trace message.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// The trace severity level.
        /// </summary>
        [JsonPropertyName("severityLevel"), JsonConverter(typeof(JsonStringEnumConverter))]
        public TelemetryEventSeverityLevel SeverityLevel { get; set; } = TelemetryEventSeverityLevel.Verbose;
    }
}
