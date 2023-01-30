using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.SharedServices.WorkDoneProgress.Configuration;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Configuration.Options
{
    public class WorkspaceSymbolOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// <c>true</c> if the server provides support to resolve additional information for workspace symbols.
        /// </summary>
        [JsonPropertyName("resolveProvider"), LspCompliant]
        public bool IsResolveProvider { get; set; }
    }
}
