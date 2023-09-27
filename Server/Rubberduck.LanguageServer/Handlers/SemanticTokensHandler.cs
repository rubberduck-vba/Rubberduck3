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
    public class SemanticTokensHandler : SemanticTokensHandlerBase
    {
        private readonly ILanguageServerFacade _server;
        private readonly SupportedLanguage _language;
        private readonly DocumentContentStore _contentStore;

        private readonly TextDocumentSelector _selector;

        public SemanticTokensHandler(ILanguageServerFacade server, SupportedLanguage language, DocumentContentStore contentStore)
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

        protected override SemanticTokensRegistrationOptions CreateRegistrationOptions(SemanticTokensCapability capability, ClientCapabilities clientCapabilities)
        {
            return new SemanticTokensRegistrationOptions
            {
                DocumentSelector = _selector,
                Full = true,
                WorkDoneProgress = true,
                Legend = new SemanticTokensLegend
                {
                    TokenModifiers = capability.TokenModifiers,
                    TokenTypes = capability.TokenTypes
                }
            };
        }

        protected async override Task<SemanticTokensDocument> GetSemanticTokensDocument(ITextDocumentIdentifierParams request, CancellationToken cancellationToken)
        {
            var result = new SemanticTokensDocument(CreateRegistrationOptions(Capability, ClientCapabilities));
            return await Task.FromResult(result);
        }

        protected async override Task Tokenize(SemanticTokensBuilder builder, ITextDocumentIdentifierParams identifier, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // TODO
            await Task.CompletedTask;
        }
    }
}