using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Responses
{
    public class ConsoleStatusResult
    {
        /// <summary>
        /// <c>true</c> if the socket server is still listening to its assigned port.
        /// </summary>
        [JsonPropertyName("isAlive")]
        public bool IsAlive { get; set; }

        /// <summary>
        /// The number of open client connections on this server instance.
        /// </summary>
        [JsonPropertyName("clientConnections")]
        public int ClientConnections { get; set; }

        /// <summary>
        /// The current console configuration options for this server instance.
        /// </summary>
        [JsonPropertyName("options")]
        public ServerConsoleOptions Options { get; set; }
    }

}