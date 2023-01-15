using AustinHarris.JsonRpc;
using Rubberduck.InternalApi.RPC.LSP;
using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Response;
using System.Threading.Tasks;

namespace Rubberduck.Server.LSP.Controllers.AbstractClient
{
    /// <summary>
    /// Window-level requests sent from a server to a client.
    /// </summary>
    public interface IWindowClient
    {
        /// <summary>
        /// Prompts a client to display a message to the user.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ShowMessage)]
        Task ShowMessage(ShowMessageParams parameters);

        /// <summary>
        /// Requests a <c>MessageActionItem</c> from the client; prompts the client to display a message to the user.
        /// </summary>
        /// <returns>The message action item selected by the user.</returns>
        [JsonRpcMethod(JsonRpcMethods.ShowMessageRequest)]
        Task<MessageActionItem> ShowMessageRequest(ShowMessageRequestParams parameters);

        /// <summary>
        /// Prompts a client to display a particular document in the user interface.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ShowDocument)]
        Task<ShowDocumentResult> ShowDocument(ShowDocumentParams parameters);

        /// <summary>
        /// Prompts a client to log a particular message.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.LogMessage)]
        Task LogMessage(LogMessageParams parameters);

        /// <summary>
        /// Prompts a client to create the UI elements to show work done progress for the supplied token.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.CreateWorkDoneProgress)]
        Task CreateWorkDoneProgress(WorkDoneProgressCreateParams parameters);

        /// <summary>
        /// A notification that cancels work that initiated on the server side with a 'window/workDoneProgress/create' request.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.CancelWorkDoneProgress)]
        Task CancelWorkDoneProgress(WorkDoneProgressCancelParams parameters);
    }
}
