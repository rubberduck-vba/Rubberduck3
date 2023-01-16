using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Response;
using Rubberduck.RPC.Parameters;
using System.Threading.Tasks;

namespace Rubberduck.Server.Controllers
{
    public interface IServerController
    {
        Task Exit();
        Task<InitializeResult> Initialize(LspInitializeParams parameters);
        Task Initialized(InitializedParams parameters);
        Task LogTrace(LogTraceParams parameters);
        Task SetTrace(SetTraceParams parameters);
        Task Shutdown();
    }
}