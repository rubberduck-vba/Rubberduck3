using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Response;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Rubberduck.InternalApi.RPC.LSP.Client
{
    /// <summary>
    /// Window-level requests sent from a server to a client.
    /// </summary>
    [ServiceContract]
    public interface IWindowClient
    {
        /// <summary>
        /// Prompts a client to display a message to the user.
        /// </summary>
        [OperationContract(Name = "window/showMessage")]
        Task ShowMessage(ShowMessageParams parameters);

        /// <summary>
        /// Requests a <c>MessageActionItem</c> from the client; prompts the client to display a message to the user.
        /// </summary>
        /// <returns>The message action item selected by the user.</returns>
        [OperationContract(Name = "window/showMessageRequest")]
        Task<MessageActionItem> ShowMessageRequest(ShowMessageRequestParams parameters);

        /// <summary>
        /// Prompts a client to display a particular document in the user interface.
        /// </summary>
        [OperationContract(Name = "window/showDocument")]
        Task<ShowDocumentResult> ShowDocument(ShowDocumentParams parameters);

        /// <summary>
        /// Prompts a client to log a particular message.
        /// </summary>
        [OperationContract(Name = "window/logMessage")]
        Task LogMessage(LogMessageParams parameters);

        /// <summary>
        /// Prompts a client to create the UI elements to show work done progress for the supplied token.
        /// </summary>
        [OperationContract(Name = "window/workDoneProgress/create")]
        Task CreateWorkDoneProgress(WorkDoneProgressCreateParams parameters);

        /// <summary>
        /// A notification that cancels work that initiated on the server side with a 'window/workDoneProgress/create' request.
        /// </summary>
        [OperationContract(Name = "window/workDoneProgress/cancel")]
        Task CancelWorkDoneProgress(WorkDoneProgressCancelParams parameters);
    }
}
