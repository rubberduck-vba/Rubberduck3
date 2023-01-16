using AustinHarris.JsonRpc;
using Rubberduck.InternalApi.RPC;
using Rubberduck.InternalApi.RPC.LSP.Parameters;
using System.Threading.Tasks;

namespace Rubberduck.Server.LSP.Controllers.AbstractClient
{
    /// <summary>
    /// Document-level requests sent from a server to a client.
    /// </summary>
    public interface ITextDocumentClient
    {
        [JsonRpcMethod(JsonRpcMethods.PublishTextDocumentDiagnostics)]
        Task PublishDiagnostics(PublishDiagnosticsParams parameters);
    }
}
