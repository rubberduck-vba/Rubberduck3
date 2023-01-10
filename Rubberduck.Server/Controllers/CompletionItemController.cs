using Rubberduck.InternalApi.RPC.LSP.Response;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Rubberduck.Server.Controllers
{
    [ServiceContract]
    public class CompletionItemController
    {
        [OperationContract(Name = "completionItem/resolve")]
        public async Task<CompletionItem> Resolve(CompletionItem parameters)
        {
            return null;
        }
    }
}
