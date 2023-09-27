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
    public class SignatureHelpHandler : SignatureHelpHandlerBase
    {
        private readonly ILanguageServerFacade _server;
        private readonly SupportedLanguage _language;
        private readonly DocumentContentStore _contentStore;

        private readonly TextDocumentSelector _selector;

        public SignatureHelpHandler(ILanguageServerFacade server, SupportedLanguage language, DocumentContentStore contentStore)
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

        public async override Task<SignatureHelp?> Handle(SignatureHelpParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = new SignatureHelp
            {
                // TODO
            };

            return await Task.FromResult(result);
        }

        protected override SignatureHelpRegistrationOptions CreateRegistrationOptions(SignatureHelpCapability capability, ClientCapabilities clientCapabilities)
        {
            return new SignatureHelpRegistrationOptions
            {
                DocumentSelector = _selector,
                WorkDoneProgress = true,
                TriggerCharacters = new Container<string>("("),
                RetriggerCharacters = new Container<string>(",")
            };
        }
    }
}