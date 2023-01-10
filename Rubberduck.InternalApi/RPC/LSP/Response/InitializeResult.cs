using Rubberduck.InternalApi.RPC.LSP.Capabilities;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class InitializeResult
    {
        /// <summary>
        /// The capabilities of the language server.
        /// </summary>
        [JsonPropertyName("capabilities")]
        public ServerCapabilities Capabilities { get; set; }

        [JsonPropertyName("serverInfo")]
        public ServerInformation ServerInfo { get; set; }

        public class ServerInformation
        {
            /// <summary>
            /// The name of the server.
            /// </summary>
            [JsonPropertyName("name")]
            public string Name { get; set; }

            /// <summary>
            /// The version of the server.
            /// </summary>
            [JsonPropertyName("version")]
            public string Version { get; set; }
        }
    }
}
