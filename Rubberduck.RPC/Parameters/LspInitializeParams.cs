using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Response;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Parameters
{
    public class LspInitializeOptions
    {
        /* TODO */
    }

    public class LspInitializeParams : InitializeParams<LspInitializeOptions>
    {
        /// <summary>
        /// The workspace folders configured in the client when the server starts. <c>null</c> if no folders are configured.
        /// </summary>
        [JsonPropertyName("workspaceFolders")]
        public WorkspaceFolder[] WorkspaceFolders { get; set; }
    }
}
