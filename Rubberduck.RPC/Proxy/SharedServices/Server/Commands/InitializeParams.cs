using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Commands
{
    public class InitializeParams<TOptions> : WorkDoneProgressParams
        where TOptions : new()
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
        /// The initial trace setting. Logs <c>Messages</c> if unspecified.
        /// </summary>
        [JsonPropertyName("trace")]
        public Constants.TraceValue.AsStringEnum Trace { get; set; } = Constants.TraceValue.AsStringEnum.Messages;
    }
}
