using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using System.Threading.Tasks;
using MediatR;
using System.Threading;

namespace Rubberduck.Client.Handlers
{
    public class ShowMessageHandler : ShowMessageHandlerBase
    {
        public override async Task<Unit> Handle(ShowMessageParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // _messageBox.Show(request.Message, ~request.MessageType~)

            return await Task.FromResult(Unit.Value);
        }
    }
}
