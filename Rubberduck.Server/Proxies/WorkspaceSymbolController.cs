using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Workspace.Language.Model;
using StreamJsonRpc;
using System.Threading.Tasks;

namespace Rubberduck.Server.Controllers
{
    public interface IWorkspaceSymbolProxy
    {
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.Workspace.Language.ResolveSymbol)]
        Task<WorkspaceSymbol> ResolveAsync(WorkspaceSymbol parameters);
    }
}
