using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.LanguageServer.Model;
using Rubberduck.LanguageServer.Services;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers
{
    public class DocumentDiagnosticHandler : DocumentDiagnosticHandlerBase
    {
        private readonly ILanguageServerFacade _server;
        private readonly SupportedLanguage _language;
        private readonly DocumentContentStore _contentStore;

        private readonly TextDocumentSelector _selector;

        public DocumentDiagnosticHandler(ILanguageServerFacade server, SupportedLanguage language, DocumentContentStore contentStore)
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

        public async override Task<RelatedDocumentDiagnosticReport> Handle(DocumentDiagnosticParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            //TODO
            var items = new List<Diagnostic>();

            var result = new RelatedFullDocumentDiagnosticReport
            {
                ResultId = "TODO",
                RelatedDocuments = new Dictionary<DocumentUri, DocumentDiagnosticReport>
                {
                    // TODO
                }.ToImmutableDictionary(),
                Items = new Container<Diagnostic>(items)
            };
            return await Task.FromResult(result);
        }

        protected override DiagnosticsRegistrationOptions CreateRegistrationOptions(DiagnosticClientCapabilities capability, ClientCapabilities clientCapabilities)
        {
            return new DiagnosticsRegistrationOptions
            {
                DocumentSelector = _selector,
                Identifier = "TODO",
                InterFileDependencies = true,
                WorkDoneProgress = true,
                WorkspaceDiagnostics = true
            };
        }
    }
}