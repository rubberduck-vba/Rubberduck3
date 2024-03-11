using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers.Language;

public class DocumentDiagnosticHandler : DocumentDiagnosticHandlerBase
{
    private readonly IWorkspaceService _workspaces;
    private readonly TextDocumentSelector _selector;

    public DocumentDiagnosticHandler(IWorkspaceService workspaces, SupportedLanguage language)
    {
        _workspaces = workspaces;
        _selector = language.ToTextDocumentSelector();
    }

    public async override Task<RelatedDocumentDiagnosticReport> Handle(DocumentDiagnosticParams request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var workspace = _workspaces.State.ActiveWorkspace ?? throw new InvalidOperationException("No workspace is currently active.");
        var uri = workspace.WorkspaceRoot!.FileUriFromAbsolute(request.TextDocument.Uri.ToUri().LocalPath);
        //var uri = new WorkspaceFileUri(request.TextDocument.Uri.ToUri().OriginalString, workspace.WorkspaceRoot!);

        if (workspace.TryGetWorkspaceFile(uri, out var state) && state != null)
        {
            return new RelatedFullDocumentDiagnosticReport
            {
                Items = new Container<Diagnostic>(state.Diagnostics)
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