using System.Text.Json.Serialization;
namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class SignatureHelpParams : TextDocumentPositionParams, IWorkDoneProgressParams
    {
        /// <summary>
        /// A token that the server can use to report work done progress.
        /// </summary>
        [JsonPropertyName("workDoneToken")]
        public string WorkDoneToken { get; set; }

        [JsonPropertyName("context")]
        public SignatureHelpContext Context { get; set; }
    }
}
