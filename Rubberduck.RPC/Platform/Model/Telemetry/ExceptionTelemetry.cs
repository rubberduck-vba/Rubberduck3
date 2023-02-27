using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Platform.Model.Telemetry
{
    /// <summary>
    /// <strong>ExceptionTelemetry</strong> represents any exception that occurred during execution of the monitored application.
    /// </summary>
    public class ExceptionTelemetry : TelemetryEvent
    {
        public ExceptionTelemetry() : base(TelemetryEventName.Exception) { }

        /// <summary>
        /// Identifies where in the code an exception was thrown.
        /// </summary>
        /// <remarks>
        /// Typically a combination of exception type and a function from the call stack.
        /// </remarks>
        [JsonPropertyName("problemId")]
        public string ProblemId { get; set; }

        /// <summary>
        /// Trace severity level. Value can be `Verbose`, `Information`, `Warning`, `Error`, or `Critical`.
        /// </summary>
        [JsonPropertyName("severityLevel")]
        public string SeverityLevel { get; set; } = TelemetryEventSeverityLevel.Verbose.ToString();

        /// <summary>
        /// Stack trace information.
        /// </summary>
        [JsonPropertyName("exceptionDetails")]
        public string ExceptionDetails { get; set; }
    }
}
