using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Rubberduck.LanguageServer.Model;
using Rubberduck.LanguageServer.Services;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers.Workspace
{
    public class WorkspaceDiagnosticHandler : WorkspaceDiagnosticHandlerBase
    {
        private readonly ILanguageServerFacade _server;
        private readonly SupportedLanguage _language;
        private readonly DocumentContentStore _contentStore;

        private readonly TextDocumentSelector _selector;

        public WorkspaceDiagnosticHandler(ILanguageServerFacade server, SupportedLanguage language, DocumentContentStore contentStore)
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

        public async override Task<WorkspaceDiagnosticReport> Handle(WorkspaceDiagnosticParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // TODO
            //var result = WorkspaceDiagnosticReport.From(...);

#pragma warning disable CS8603 // Possible null reference return. // TODO remove
            return await Task.FromResult(default(WorkspaceDiagnosticReport));
#pragma warning restore CS8603 // Possible null reference return.
        }

        protected override DiagnosticsRegistrationOptions CreateRegistrationOptions(DiagnosticWorkspaceClientCapabilities capability, ClientCapabilities clientCapabilities)
        {
            return new DiagnosticsRegistrationOptions
            {
                DocumentSelector = _selector,
                InterFileDependencies = true,
                WorkDoneProgress = true,
                WorkspaceDiagnostics = true
            };
        }
    }
}