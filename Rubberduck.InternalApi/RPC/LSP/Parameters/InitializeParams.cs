using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    /// <summary>
    /// An <c>Initialize</c> request is sent as the first request from a client to the server.
    /// If the server receives a request or notification before the <c>Initialize</c> request, it should:
    /// <list type="bullet">
    /// <item>For a <em>request</em>, respond with error code <c>-32002</c>.</item>
    /// <item>For a <em>notification</em> that isn't the <c>Exit</c> notification, the request should be dropped.</item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// Implementation should allow server to <c>Exit</c> without receiving an <c>Initialize</c> request.
    /// Server may send <c>window/showMessage</c>, <c>window/showMessageRequest</c>, <c>window/logMessage</c>, 
    /// and <c>telemetry/event</c> requests to the client while the <c>Initialize</c> request is processing.
    /// </remarks>
    /// <typeparam name="TOptions"></typeparam>
    public class InitializeParams<TOptions> : WorkDoneProgressParams
        where TOptions : class, new()
    {
        /// <summary>
        /// The process ID of the parent process that started the server. <c>null</c> if the process has not been started by another process.
        /// </summary>
        /// <remarks>
        /// If the parent process is not alive, the server should exit its process.
        /// </remarks>
        [JsonPropertyName("processId")]
        public int? ProcessId { get; set; }

        /// <summary>
        /// Information about the client.
        /// </summary>
        [JsonPropertyName("clientInfo")]
        public ClientInfo ClientInfo { get; set; }

        /// <summary>
        /// The locale the client is currently showing the user interface in.
        /// </summary>
        [JsonPropertyName("locale")]
        public string Locale { get; set; }

        /// <summary>
        /// The provided initialization options.
        /// </summary>
        [JsonPropertyName("initializationOptions")]
        public TOptions InitializationOptions { get; set; }

        /// <summary>
        /// The initial trace setting. Disabled if unspecified.
        /// </summary>
        [JsonPropertyName("trace")]
        public string Trace { get; set; } = Constants.TraceValue.Off;
    }
}
