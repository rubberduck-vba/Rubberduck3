using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.ServerPlatform.TelemetryServer;
using System.Text.Json.Serialization;

namespace Rubberduck.ServerPlatform.Model.Telemetry
{
    /// <summary>
    /// <strong>TraceTelemetry</strong> represents trace logs that can later be searched.
    /// </summary>
    /// <remarks>
    /// ILogger entries translate into this type of telemetry event.
    /// </remarks>
    public record TraceTelemetry : TelemetryEvent
    {
        public TraceTelemetry(string message, TelemetryEventSeverityLevel level, TelemetryEventParams request, TelemetryContext context) 
            : base(TelemetryEventName.Trace, request, context)
        {
            Message = message;
            SeverityLevel = level;
        }

        /// <summary>
        /// The trace message.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; init; }

        /// <summary>
        /// The trace severity level.
        /// </summary>
        [JsonPropertyName("severityLevel"), JsonConverter(typeof(JsonStringEnumConverter))]
        public TelemetryEventSeverityLevel SeverityLevel { get; init; } = TelemetryEventSeverityLevel.Verbose;
    }
}
