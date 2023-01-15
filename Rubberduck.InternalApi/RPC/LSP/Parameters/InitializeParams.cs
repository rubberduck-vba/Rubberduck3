using Rubberduck.InternalApi.RPC.LSP.Response;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class InitializeParams : WorkDoneProgressParams
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
        public LSPAny InitializationOptions { get; set; }

        /// <summary>
        /// The initial trace setting. Disabled if unspecified.
        /// </summary>
        [JsonPropertyName("trace")]
        public string Trace { get; set; } = Constants.TraceValue.Off;

        /// <summary>
        /// The workspace folders configured in the client when the server starts. <c>null</c> if no folders are configured.
        /// </summary>
        [JsonPropertyName("workspaceFolders")]
        public WorkspaceFolder[] WorkspaceFolders { get; set; }
    }
}
