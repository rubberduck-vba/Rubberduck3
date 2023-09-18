using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.LanguageServer.Model;
using Rubberduck.LanguageServer.Services;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers
{
    public class PrepareRenameHandler : PrepareRenameHandlerBase
    {
        private readonly ILanguageServerFacade _server;
        private readonly SupportedLanguage _language;
        private readonly DocumentContentStore _contentStore;

        private readonly TextDocumentSelector _selector;

        public PrepareRenameHandler(ILanguageServerFacade server, SupportedLanguage language, DocumentContentStore contentStore)
        {
            _language = language;
            _server = server;
            _contentStore = contentStore;

            var filter = new TextDocumentFilter
            {
                Language = language.Id,
                Pattern = string.Join(';', language.FileTypes.Select(fileType => $"**/{fileType}").ToArray())
            };
            _selector = new TextDocumentSelector(filter);
        }

        public async override Task<RangeOrPlaceholderRange?> Handle(PrepareRenameParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = new RangeOrPlaceholderRange(new RenameDefaultBehavior()); // TODO
            return await Task.FromResult(result);
        }

        protected override RenameRegistrationOptions CreateRegistrationOptions(RenameCapability capability, ClientCapabilities clientCapabilities)
        {
            return new RenameRegistrationOptions
            {
                DocumentSelector = _selector,
                PrepareProvider = true,
                WorkDoneProgress = true
            };
        }
    }
}