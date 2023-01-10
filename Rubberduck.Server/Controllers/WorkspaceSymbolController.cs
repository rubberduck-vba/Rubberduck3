using Rubberduck.InternalApi.RPC.LSP.Response;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Rubberduck.Server.Controllers
{
    [ServiceContract]
    public class WorkspaceSymbolController
    {
        [OperationContract(Name = "workspaceSymbol/resolve")]
        public async Task<WorkspaceSymbol> Resolve(WorkspaceSymbol parameters)
        {
            return null;
        }
    }
}
