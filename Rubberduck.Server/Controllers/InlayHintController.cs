using Rubberduck.InternalApi.RPC.LSP.Response;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Rubberduck.Server.Controllers
{
    [ServiceContract]
    public class InlayHintController
    {
        [OperationContract(Name = "inlayHint/resolve")]
        public async Task<InlayHint> Resolve(InlayHint parameters)
        {
            return null;
        }
    }
}
