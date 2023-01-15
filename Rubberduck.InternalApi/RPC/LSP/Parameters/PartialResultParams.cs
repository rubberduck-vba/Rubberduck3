using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{

    public interface IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [JsonPropertyName("partialResultToken")]
        string PartialResultToken { get; set; }
    }

    public class PartialResultParams : IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [JsonPropertyName("partialResultToken")]
        public string PartialResultToken { get; set; }
    }
}
