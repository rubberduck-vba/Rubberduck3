using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters
{
    public class ProgressParams
    {
        /// <summary>
        /// The progress token provided by the client or server.
        /// </summary>
        [JsonPropertyName("token"), LspCompliant]
        public string Token { get; set; }

        /// <summary>
        /// The progress data.
        /// </summary>
        [JsonPropertyName("value"), LspCompliant]
        public int Value { get; set; }
    }
}
