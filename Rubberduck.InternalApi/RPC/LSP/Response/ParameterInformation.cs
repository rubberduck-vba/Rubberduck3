using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class ParameterInformation
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("documentation")]
        public string Documentation { get; set; }
    }
}
