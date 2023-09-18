using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.LanguageServer.Model;
using Rubberduck.LanguageServer.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers
{
    public class DocumentSymbolHandler : DocumentSymbolHandlerBase
    {
        private readonly ILanguageServerFacade _server;
        private readonly SupportedLanguage _language;
        private readonly DocumentContentStore _contentStore;

        private readonly TextDocumentSelector _selector;

        public DocumentSymbolHandler(ILanguageServerFacade server, SupportedLanguage language, DocumentContentStore contentStore)
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

        public async override Task<SymbolInformationOrDocumentSymbolContainer?> Handle(DocumentSymbolParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var items = new List<SymbolInformationOrDocumentSymbol>();
            // TODO

            var result = new SymbolInformationOrDocumentSymbolContainer(items);
            return await Task.FromResult(result);
        }

        protected override DocumentSymbolRegistrationOptions CreateRegistrationOptions(DocumentSymbolCapability capability, ClientCapabilities clientCapabilities)
        {
            return new DocumentSymbolRegistrationOptions
            {
                DocumentSelector = _selector,
                WorkDoneProgress = true
            };
        }
    }
}