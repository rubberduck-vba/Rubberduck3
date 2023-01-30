using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.File.Commands.Parameters;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using StreamJsonRpc;
using System;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.File.Abstract
{
    public interface ITextDocumentFileProxy
    {
        /// <summary>
        /// The document <strong>DidChange</strong> notification is sent from the client to the server to signal changes to a text document.
        /// </summary>
        /// <remarks>
        /// Before a client can change a text document it must claim ownership of its content using the <c>textDocument/didOpen</c> notification.
        /// </remarks>
        [LspCompliant(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.File.DidChange)]
        event EventHandler<DidChangeTextDocumentParams> DidChangeTextDocument;

        /// <summary>
        /// The document <strong>DidOpen</strong> notification is sent from the client to the server to signal newly opened text documents.
        /// </summary>
        /// <remarks>
        ///  The document’s content is now managed by the client and the server must not try to read the document’s content using the document’s Uri. Open in this sense means it is managed by the client.
        /// </remarks>
        [LspCompliant(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.File.DidOpen)]
        event EventHandler<DidOpenTextDocumentParams> DidOpenTextDocument;

        /// <summary>
        /// The document <strong>DidClose</strong> notification is sent from the client to the server when the document got closed in the client. 
        /// The document’s master now exists where the document’s Uri points to (e.g. if the document’s Uri is a file Uri the master now exists on disk). 
        /// </summary>
        /// <remarks>
        /// As with the open notification, the close notification is about managing the document’s content. 
        /// Receiving a close notification doesn’t mean that the document was open in an editor before. 
        /// A close notification requires a previous open notification to be sent.
        /// </remarks>
        [LspCompliant(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.File.DidClose)]
        event EventHandler<DidCloseTextDocumentParams> DidCloseTextDocument;

        /// <summary>
        /// The document <strong>DidSave</strong> notification is sent from the client to the server when the document was saved in the client.
        /// </summary>
        [LspCompliant(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.File.DidSave)]
        event EventHandler<DidSaveTextDocumentParams> DidSaveTextDocument;

        /// <summary>
        /// The <strong>WillSaveWaitUntil</strong> request gets an array of TextEdits that will be applied to the document before it is saved.
        /// </summary>
        /// <param name="parameters">The document that will be saved.</param>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.File.WillSaveUntil), LspCompliant]
        Task<TextEdit[]> WillSaveWaitUntilAsync(WillSaveTextDocumentParams parameters);
    }
}
