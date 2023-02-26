using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Platform.Model.Telemetry
{
    /// <summary>
    /// <strong>DependencyTelemetry</strong> represents an interaction of the monitored component with a remote component suhc as a SQL, HTTP, (or RPC) endpoint.
    /// </summary>
    public class DependencyTelemetry : TelemetryEvent
    {
        public DependencyTelemetry() : base(TelemetryEventName.Dependency) { }

        /// <summary>
        /// The name of the command initiated with this dependency call.
        /// </summary>
        /// <remarks>
        /// Low cardinality value, e.g. stored procedure name, URL path template.
        /// </remarks>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Identifies an instance of a dependency call; correlates between this item and other telemetry items.
        /// </summary>
        /// <remarks>
        /// Value should be globally unique. Maximum length: 128
        /// </remarks>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Command initiated by this dependency call, e.g. SQL statement and HTTP URL with all query parameters.
        /// </summary>
        [JsonPropertyName("data")]
        public string Data { get; set; }

        /// <summary>
        /// Dependency type name. Low cardinality value for logical grouping of dependencies and interpretation of other fields like <c>commandName</c> and <c>resultCode</c>,
        /// e.g. SQL, Azure table, HTTP, ...or in this case WS.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Target site of a dependency call, e.g. server name, host address, etc.
        /// </summary>
        [JsonPropertyName("target")]
        /// <value>The name and process ID of the server, along with its port and path.</value>
        public string Target { get; set; }

        /// <summary>
        /// Request duration in <c>DD.HH:MM:SS.MMMMMM</c> format. Must be less than 1,000 days.
        /// </summary>
        [JsonPropertyName("duration")]
        public string Duration { get; set; }

        /// <summary>
        /// Result code of a dependency call, e.g. SQL error code, HTTP status code, etc.
        /// </summary>
        [JsonPropertyName("resultCode")]
        public string ResultCode { get; set; }

        /// <summary>
        /// Indicates whether the call was successful or not.
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}
