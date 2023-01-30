using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.WorkDoneProgress.Configuration;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Configuration
{
    /// <summary>
    /// Configures telemetry capabilities.
    /// </summary>
    public class TelemetryOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// Whether telemetry is enabled at all.
        /// </summary>
        /// <remarks>
        /// This parameter takes precedence over any other telemetry configuration when <c>false</c>.
        /// </remarks>
        [JsonPropertyName("enabled")]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Whether <em>Dependency</em> telemetry is enabled.
        /// </summary>
        /// <remarks>
        /// <strong>DependencyTelemetry</strong> represents an interaction of the monitored component with a remote component suhc as a SQL, HTTP, (or RPC) endpoint.
        /// </remarks>>
        [JsonPropertyName("dependency")]
        public bool DependencyTelemetry { get; set; }

        /// <summary>
        /// Configures <c>EventTelemetry</c> capabilities; <c>null</c> if event telemetry is disabled.
        /// </summary>
        /// <remarks>
        /// <strong>EventTelemetry</strong> represents an event that occurred in the application.
        /// </remarks>
        [JsonPropertyName("eventTelemetryProvider")]
        public EventTelemetryOptions EventTelemetryProvider { get; set; }

        /// <summary>
        /// Whether <c>Exception</c> telemetry is enabled.
        /// </summary>
        /// <remarks>
        /// <strong>ExceptionTelemetry</strong> represents any exception that occurred during execution of the monitored application.
        /// </remarks>
        [JsonPropertyName("exception")]
        public bool ExceptionTelemetry { get; set; }

        /// <summary>
        /// Configures <c>MetricTelemetry</c> capabilities; <c>null</c> if metric telemetry is disabled.
        /// </summary>
        /// <remarks>
        /// <strong>MetricTelemetry</strong> represent various aggregatable measures.
        /// </remarks>
        [JsonPropertyName("metricTelemetryProvider")]
        public MetricTelemetryOptions MetricTelemetryProvider { get; set; }

        /// <summary>
        /// Whether <c>PageView</c> telemetry is enabled.
        /// </summary>
        /// <remarks>
        /// <strong>PageViewTelemetry</strong> is logged when the user opens a new <em>page</em> (/tab/screen) of a monitored application.
        /// </remarks>
        [JsonPropertyName("pageView")]
        public bool PageViewTelemetry { get; set; }

        /// <summary>
        /// Whether <c>Request</c> telemetry is enabled.
        /// </summary>
        /// <remarks>
        ///  <strong>RequestTelemetry</strong> represents the logical sequence of execution triggered by an external request to your application.
        /// </remarks>
        [JsonPropertyName("request")]
        public bool RequestTelemetry { get; set; }

        /// <summary>
        /// Configures <c>TraceTelemetry</c> capabilities; <c>null</c> if trace telemetry is disabled.
        /// </summary>
        /// <remarks>
        /// <strong>TraceTelemetry</strong> represents trace logs that can later be searched.
        /// </remarks>
        [JsonPropertyName("trace")]
        public TraceTelemetryOptions TraceTelemetryProvider { get; set; }

        public class EventTelemetryOptions
        {
            /// <summary>
            /// A dictionary keyed with <c>EventTelemetryName</c> identifiers. If the associated value is <c>true</c>, this telemetry event is enabled.
            /// </summary>
            [JsonPropertyName("events")]
            public Dictionary<string, bool> Events { get; set; }
        }

        public class MetricTelemetryOptions
        {
            /* meh */
        }

        public class TelemetryContextOptions
        {
            /// <summary>
            /// Whether <c>TelemetryContext</c> may include an anonymous user ID.
            /// </summary>
            [JsonPropertyName("anonymousUserId")]
            public bool AnonymousUserId { get; set; }

            /// <summary>
            /// Whether <c>TelemetryContext</c> may include an <em>authenticated</em> user ID.
            /// </summary>
            [JsonPropertyName("authenticatedUserId")]
            public bool AuthenticatedUserId { get; set; }

            /// <summary>
            /// Whether <c>TelemetryContext</c> may include any of these custom properties.
            /// </summary>
            [JsonPropertyName("customProperties")]
            public Dictionary<string, bool> CustomProperties { get; set; }
        }

        public class TraceTelemetryOptions
        {
            /// <summary>
            /// The minimum severity level that can generate a <c>TraceTelemetry</c> item.
            /// </summary>
            /// <remarks>
            /// Corresponds to <c>NLog.LogLevel</c>.
            /// </remarks>
            [JsonPropertyName("minLevel")]
            public ServerLogLevel MinimumSeverity { get; set; }
        }
    }
}
