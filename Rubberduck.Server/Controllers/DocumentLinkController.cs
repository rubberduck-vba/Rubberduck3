using Rubberduck.InternalApi.RPC.LSP.Response;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Rubberduck.Server.Controllers
{
    [ServiceContract]
    public class DocumentLinkController
    {
        [OperationContract(Name = "documentLink/resolve")]
        public async Task<DocumentLink> Resolve(DocumentLink parameters)
        {
            return null;
        }
    }
}
