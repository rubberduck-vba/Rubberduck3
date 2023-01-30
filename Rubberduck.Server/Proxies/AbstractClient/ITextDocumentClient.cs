using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Commands.Parameters;
using StreamJsonRpc;
using System.Threading.Tasks;

namespace Rubberduck.Server.LSP.Controllers.AbstractClient
{
    /// <summary>
    /// Document-level requests sent from a server to a client.
    /// </summary>
    public interface ITextDocumentClient
    {
        [JsonRpcMethod(JsonRpcMethods.ClientProxyRequests.LSP.PublishDocumentDiagnostics)]
        Task PublishDiagnosticsAsync(PublishDiagnosticsParams parameters);
    }
}
