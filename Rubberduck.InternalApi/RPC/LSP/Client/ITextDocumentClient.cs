using Rubberduck.InternalApi.RPC.LSP.Parameters;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Rubberduck.InternalApi.RPC.LSP.Client
{
    /// <summary>
    /// Document-level requests sent from a server to a client.
    /// </summary>
    [ServiceContract]
    public interface ITextDocumentClient
    {
        [OperationContract(Name = "textDocument/publishDiagnostics")]
        Task PublishDiagnostics(PublishDiagnosticsParams parameters);
    }
}
