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
    public class DocumentOnTypeFormattingHandler : DocumentOnTypeFormattingHandlerBase
    {
        private readonly ILanguageServerFacade _server;
        private readonly SupportedLanguage _language;
        private readonly DocumentContentStore _contentStore;

        private readonly TextDocumentSelector _selector;

        public DocumentOnTypeFormattingHandler(ILanguageServerFacade server, SupportedLanguage language, DocumentContentStore contentStore)
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

        public async override Task<TextEditContainer?> Handle(DocumentOnTypeFormattingParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var items = new List<TextEdit>();
            // TODO

            var result = new TextEditContainer(items);
            return await Task.FromResult(result);
        }

        protected override DocumentOnTypeFormattingRegistrationOptions CreateRegistrationOptions(DocumentOnTypeFormattingCapability capability, ClientCapabilities clientCapabilities)
        {
            return new DocumentOnTypeFormattingRegistrationOptions
            {
                DocumentSelector = _selector,
                WorkDoneProgress = true,
                FirstTriggerCharacter = "\n"
            };
        }
    }
}