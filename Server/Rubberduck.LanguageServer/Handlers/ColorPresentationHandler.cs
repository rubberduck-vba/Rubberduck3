using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers
{
    public class ColorPresentationHandler : ColorPresentationHandlerBase
    {
        public async override Task<Container<ColorPresentation>> Handle(ColorPresentationParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var presentation = new ColorPresentation
            {
                // TODO
            };
            var container = new Container<ColorPresentation>(presentation);
            return await Task.FromResult(container);
        }
    }
}