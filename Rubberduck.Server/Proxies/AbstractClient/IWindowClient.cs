using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters;
using Rubberduck.RPC.Proxy.LspServer.Window.Commands.Parameters;
using Rubberduck.RPC.Proxy.LspServer.Window.Model;
using Rubberduck.RPC.Proxy.LspServer.Workspace.Language.Model;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters;
using StreamJsonRpc;
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
        [JsonRpcMethod(JsonRpcMethods.ClientProxyRequests.LSP.ShowMessage)]
        Task ShowMessageAsync(ShowMessageParams parameters);

        /// <summary>
        /// Requests a <c>MessageActionItem</c> from the client; prompts the client to display a message to the user.
        /// </summary>
        /// <returns>The message action item selected by the user.</returns>
        [JsonRpcMethod(JsonRpcMethods.ClientProxyRequests.LSP.ShowMessageRequest)]
        Task<MessageActionItem> ShowMessageRequestAsync(ShowMessageRequestParams parameters);

        /// <summary>
        /// Prompts a client to display a particular document in the user interface.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ClientProxyRequests.LSP.ShowDocument)]
        Task<ShowDocumentResult> ShowDocumentAsync(ShowDocumentParams parameters);

        /// <summary>
        /// Prompts a client to log a particular message.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ClientProxyRequests.LSP.LogMessage)]
        Task LogMessageAsync(LogMessageParams parameters);

        /// <summary>
        /// Prompts a client to create the UI elements to show work done progress for the supplied token.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ClientProxyRequests.LSP.CreateWorkDoneProgress)]
        Task CreateWorkDoneProgressAsync(WorkDoneProgressCreateParams parameters);

        /// <summary>
        /// A notification that cancels work that initiated on the server side with a 'window/workDoneProgress/create' request.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ClientProxyRequests.LSP.CancelWorkDoneProgress)]
        Task CancelWorkDoneProgressAsync(WorkDoneProgressCancelParams parameters);
    }
}
