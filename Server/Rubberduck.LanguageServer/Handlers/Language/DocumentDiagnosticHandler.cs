using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers.Language;

public class DocumentDiagnosticHandler : DocumentDiagnosticHandlerBase
{
    private readonly IAppWorkspacesService _workspaces;
    private readonly TextDocumentSelector _selector;

    public DocumentDiagnosticHandler(IAppWorkspacesService workspaces, SupportedLanguage language)
    {
        _workspaces = workspaces;
        _selector = language.ToTextDocumentSelector();
    }

    public async override Task<RelatedDocumentDiagnosticReport> Handle(DocumentDiagnosticParams request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var workspace = _workspaces.Workspaces.ActiveWorkspace ?? throw new InvalidOperationException("No workspace is currently active.");
        var uri = workspace.WorkspaceRoot!.FileUriFromAbsolute(request.TextDocument.Uri.ToUri().LocalPath);

        if (workspace.TryGetWorkspaceFile(uri, out var state) && state is CodeDocumentState document)
        {
            return new RelatedFullDocumentDiagnosticReport
            {
                Items = new Container<Diagnostic>(document.Diagnostics)
            };
        }

        throw new InvalidOperationException($"Could not find workspace document at uri '{uri}'.");
    }

    protected override DiagnosticsRegistrationOptions CreateRegistrationOptions(DiagnosticClientCapabilities capability, ClientCapabilities clientCapabilities)
    {
        return new DiagnosticsRegistrationOptions
        {
            DocumentSelector = _selector,
            Identifier = "RDE",
            InterFileDependencies = true,
            WorkDoneProgress = false,
            WorkspaceDiagnostics = false
        };
    }
}