using Rubberduck.InternalApi.RPC;
using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.RPC.Parameters;
using System.Threading.Tasks;

namespace Rubberduck.RPC
{
    public interface IServerController<TServerCapabilities> 
        where TServerCapabilities : class, new()
    {
        Task Exit();
        Task<InitializeResult<TServerCapabilities>> Initialize(LspInitializeParams parameters);
        Task Initialized(InternalApi.RPC.LSP.Parameters.InitializedParams parameters);
        Task LogTrace(LogTraceParams parameters);
        Task SetTrace(SetTraceParams parameters);
        Task Shutdown();
    }
}