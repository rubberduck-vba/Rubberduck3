using Rubberduck.RPC.Platform;
using Rubberduck.InternalApi.RPC;
using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Response;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Rubberduck.Server.Controllers
{
    public class WindowClientController : JsonRpcClient
    {
        public WindowClientController(WebSocket socket) : base(socket)
        {
        }

        public async Task CancelWorkDoneProgress(WorkDoneProgressCancelParams parameters)
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.CancelWorkDoneProgress, parameters);
                Notify(request);
            });
        }

        public async Task CreateWorkDoneProgress(WorkDoneProgressCreateParams parameters)
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.CreateWorkDoneProgress, parameters);
                Notify(request);
            });
        }

        public async Task LogMessage(LogMessageParams parameters)
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.LogMessage, parameters);
                Notify(request);
            });
        }

        public async Task<ShowDocumentResult> ShowDocument(ShowDocumentParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.ShowDocument, parameters);
                var response = Request<ShowDocumentResult>(request);

                return response;
            });
        }

        public async Task ShowMessage(ShowMessageParams parameters)
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.ShowMessage, parameters);
                Notify(request);
            });
        }

        public async Task<MessageActionItem> ShowMessageRequest(ShowMessageRequestParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.ShowMessageRequest, parameters);
                var response = Request<MessageActionItem>(request);

                return response;
            });
        }
    }
}
