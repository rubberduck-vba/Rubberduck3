using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Client;
using System;
using System.Threading.Tasks;

namespace Rubberduck.Server.Controllers
{
    public class TextDocumentClientController : ITextDocumentClient
    {
        public Task PublishDiagnostics(PublishDiagnosticsParams parameters)
        {
            throw new NotImplementedException();
        }
    }
}
