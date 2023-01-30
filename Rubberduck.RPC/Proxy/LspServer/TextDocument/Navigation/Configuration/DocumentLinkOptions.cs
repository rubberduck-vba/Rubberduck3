using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.SharedServices.WorkDoneProgress.Configuration;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Navigation.Configuration
{
    public class DocumentLinkOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// <c>true</c> if the server provides support to resolve additional information for document links.
        /// </summary>
        [JsonPropertyName("resolveProvider"), LspCompliant]
        public bool IsResolveProvider { get; set; }
    }
}
