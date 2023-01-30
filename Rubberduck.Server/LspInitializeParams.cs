using Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using System.Text.Json.Serialization;

namespace Rubberduck.Server.LSP
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
