using Rubberduck.InternalApi.RPC.LSP.Response;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Rubberduck.Server.Controllers
{
    [ServiceContract]
    public class CodeLensController
    {
        [OperationContract(Name = "codeLens/resolve")]
        public async Task<CodeLens> Resolve(CodeLens parameters)
        {
            return null;
        }
    }
}
