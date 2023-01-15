using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class ShowMessageRequestParams : ShowMessageParams
    {
        [JsonPropertyName("actions")]
        public MessageActionItem[] Actions { get; set; }
    }
}
