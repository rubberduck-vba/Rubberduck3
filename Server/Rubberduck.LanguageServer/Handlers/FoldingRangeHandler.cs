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
    public class FoldingRangeHandler : FoldingRangeHandlerBase
    {
        private readonly ILanguageServerFacade _server;
        private readonly SupportedLanguage _language;
        private readonly DocumentContentStore _contentStore;

        private readonly TextDocumentSelector _selector;

        public FoldingRangeHandler(ILanguageServerFacade server, SupportedLanguage language, DocumentContentStore contentStore)
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

        public async override Task<Container<FoldingRange>?> Handle(FoldingRangeRequestParam request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var items = new List<FoldingRange>();
            // TODO

            var result = new Container<FoldingRange>(items);
            return await Task.FromResult(result);
        }

        protected override FoldingRangeRegistrationOptions CreateRegistrationOptions(FoldingRangeCapability capability, ClientCapabilities clientCapabilities)
        {
            return new FoldingRangeRegistrationOptions
            {
                DocumentSelector = _selector,
                WorkDoneProgress = true
            };
        }
    }
}