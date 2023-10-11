using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.InternalApi.ServerPlatform.TelemetryServer;
using System.Text.Json.Serialization;

namespace Rubberduck.ServerPlatform.Model.Telemetry
{
    /// <summary>
    /// <strong>ExceptionTelemetry</strong> represents any exception that occurred during execution of the monitored application.
    /// </summary>
    public record ExceptionTelemetry : TelemetryEvent
    {
        public ExceptionTelemetry(string problemId, TelemetryEventSeverityLevel level, string details, TelemetryEventParams request, TelemetryContext context) 
            : base(TelemetryEventName.Exception, request, context) 
        {
            ProblemId = problemId;
            SeverityLevel = level.ToString();
            ExceptionDetails = details;
        }

        /// <summary>
        /// Identifies where in the code an exception was thrown.
        /// </summary>
        /// <remarks>
        /// Typically a combination of exception type and a function from the call stack.
        /// </remarks>
        [JsonPropertyName("problemId")]
        public string ProblemId { get; init; }

        /// <summary>
        /// Trace severity level. Value can be `Verbose`, `Information`, `Warning`, `Error`, or `Critical`.
        /// </summary>
        [JsonPropertyName("severityLevel")]
        public string SeverityLevel { get; init; } = TelemetryEventSeverityLevel.Verbose.ToString();

        /// <summary>
        /// Stack trace information.
        /// </summary>
        [JsonPropertyName("exceptionDetails")]
        public string ExceptionDetails { get; init; }
    }
}
