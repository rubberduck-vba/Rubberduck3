using Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.File.Commands.Parameters
{
    public class DidChangeWatchedFilesParams
    {
        [JsonPropertyName("changes")]
        public FileEvent[] Changes { get; set; }
    }
}
