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
    public class ImplementationHandler : ImplementationHandlerBase
    {
        private readonly ILanguageServerFacade _server;
        private readonly SupportedLanguage _language;
        private readonly DocumentContentStore _contentStore;

        private readonly TextDocumentSelector _selector;

        public ImplementationHandler(ILanguageServerFacade server, SupportedLanguage language, DocumentContentStore contentStore)
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

        public async override Task<LocationOrLocationLinks?> Handle(ImplementationParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var items = new List<LocationOrLocationLink>();
            // TODO

            var result = new LocationOrLocationLinks(items);
            return await Task.FromResult(result);
        }

        protected override ImplementationRegistrationOptions CreateRegistrationOptions(ImplementationCapability capability, ClientCapabilities clientCapabilities)
        {
            return new ImplementationRegistrationOptions
            {
                DocumentSelector = _selector,
                WorkDoneProgress = true
            };
        }
    }
}