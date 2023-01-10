using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Client;
using Rubberduck.InternalApi.RPC.LSP.Response;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Rubberduck.Server.Controllers
{
    [ServiceContract]
    public class WorkspaceClientController : IWorkspaceClient
    {
        public Task<ApplyWorkspaceEditResult> ApplyEdit(ApplyWorkspaceEditParams parameters)
        {
            throw new NotImplementedException();
        }

        public Task RefreshCodeLens()
        {
            throw new NotImplementedException();
        }

        public Task RefreshDiagnostics()
        {
            throw new NotImplementedException();
        }

        public Task RefreshInlayHints()
        {
            throw new NotImplementedException();
        }

        public Task RefreshSemanticTokens()
        {
            throw new NotImplementedException();
        }
    }
}
