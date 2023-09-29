using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using System.Threading.Tasks;
using System.Threading;

namespace Rubberduck.Client.Handlers
{
    public class ShowMessageRequestHandler : ShowMessageRequestHandlerBase
    {
        public override async Task<MessageActionItem> Handle(ShowMessageRequestParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            MessageActionItem response = default!;
            // var result = _messageBox.Show(request.Message, ~request.MessageType~)
            // TODO map result to action item

            cancellationToken.ThrowIfCancellationRequested();
            return await Task.FromResult(response);
        }
    }
}
