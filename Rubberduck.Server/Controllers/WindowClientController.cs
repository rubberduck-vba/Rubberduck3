using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Client;
using Rubberduck.InternalApi.RPC.LSP.Response;
using System;
using System.Threading.Tasks;

namespace Rubberduck.Server.Controllers
{
    public class WindowClientController : IWindowClient
    {
        public Task CancelWorkDoneProgress(WorkDoneProgressCancelParams parameters)
        {
            throw new NotImplementedException();
        }

        public Task CreateWorkDoneProgress(WorkDoneProgressCreateParams parameters)
        {
            throw new NotImplementedException();
        }

        public Task LogMessage(LogMessageParams parameters)
        {
            throw new NotImplementedException();
        }

        public Task<ShowDocumentResult> ShowDocument(ShowDocumentParams parameters)
        {
            throw new NotImplementedException();
        }

        public Task ShowMessage(ShowMessageParams parameters)
        {
            throw new NotImplementedException();
        }

        public Task<MessageActionItem> ShowMessageRequest(ShowMessageRequestParams parameters)
        {
            throw new NotImplementedException();
        }
    }
}
