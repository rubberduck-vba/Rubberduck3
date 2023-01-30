using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.Language.Configuration.Client
{
    public class InlayHintWorkspaceClientCapabilities
    {
        /// <summary>
        /// Whether the client supports a refresh request sent from the server to the client.
        /// </summary>
        /// <remarks>
        /// This event is global and will force the client to refresh all inlay hints currently shown. Use carefully.
        /// </remarks>
        [JsonPropertyName("refreshSupport")]
        public bool SupportsRefresh { get; set; }
    }
}
