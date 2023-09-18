using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers
{
    public class CodeActionResolveHandler : CodeActionResolveHandlerBase
    {
        public async override Task<CodeAction> Handle(CodeAction request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // TODO
            return await Task.FromResult(request);
        }
    }
}