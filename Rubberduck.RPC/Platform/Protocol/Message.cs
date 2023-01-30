using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Platform.Protocol
{
    public abstract class Message
    {
        /// <summary>
        /// A string specifying the version of the JSON-RPC protocol. MUST be exactly "2.0".
        /// </summary>
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; } = "2.0";
    }
}
