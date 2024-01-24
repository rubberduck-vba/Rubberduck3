using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers.Language
{
    public class DocumentColorHandler : DocumentColorHandlerBase
    {
        private readonly ILanguageServerFacade _server;
        private readonly SupportedLanguage _language;
        private readonly DocumentContentStore _contentStore;

        private readonly TextDocumentSelector _selector;

        public DocumentColorHandler(ILanguageServerFacade server, SupportedLanguage language, DocumentContentStore contentStore)
        {
            _language = language;
            _server = server;
            _contentStore = contentStore;

            var filter = new TextDocumentFilter
            {
                Language = language.Id,
                Pattern = string.Join(";", language.FileTypes.Select(fileType => $"**/{fileType}").ToArray())
            };
            _selector = new TextDocumentSelector(filter);
        }

        public async override Task<Container<ColorInformation>?> Handle(DocumentColorParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var items = new List<ColorInformation>();
            //TODO
            var result = new Container<ColorInformation>(items);
            return await Task.FromResult(result);
        }

        protected override DocumentColorRegistrationOptions CreateRegistrationOptions(ColorProviderCapability capability, ClientCapabilities clientCapabilities)
        {
            return new DocumentColorRegistrationOptions
            {
                DocumentSelector = _selector,
                WorkDoneProgress = true,
                Id = "TODO"
            };
        }
    }
}