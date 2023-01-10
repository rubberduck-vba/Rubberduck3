using Rubberduck.InternalApi.RPC.LSP.Response;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Rubberduck.Server.Controllers
{
    [ServiceContract]
    public class CodeActionController
    {
        /// <summary>
        /// Resolves the edit and/or command associated to the specified code action.
        /// </summary>
        [OperationContract(Name = "codeAction/resolve")]
        public async Task<CodeAction> Resolve(CodeAction parameters)
        {
            return null;
        }
    }
}
