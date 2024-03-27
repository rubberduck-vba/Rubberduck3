using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.ServerPlatform;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers.Document
{
    public class DidCloseTextDocumentHandler : DidCloseTextDocumentHandlerBase
    {
        private readonly ServerPlatformServiceHelper _service;
        private readonly IAppWorkspacesStateManager _workspaces;
        private readonly TextDocumentSelector _selector;

        public DidCloseTextDocumentHandler(ServerPlatformServiceHelper service, IAppWorkspacesStateManager workspaces, TextDocumentSelector selector)
        {
            _service = service;
            _workspaces = workspaces;
            _selector = selector;
        }

        public override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
        {
            var workspace = _workspaces.ActiveWorkspace;
            var uri = new WorkspaceFileUri(request.TextDocument.Uri.ToUri().AbsoluteUri, workspace!.WorkspaceRoot!.WorkspaceRoot);

            if (workspace.TryGetWorkspaceFile(uri, out var document) && document is not null)
            {
                if (document.IsOpened)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    workspace.LoadDocumentState(document with { IsOpened = false });

                    _service.LogInformation($"DidCloseTextDocument: Updated content for document '{uri}'.");
                }
                else
                {
                    _service.LogWarning("Document was already closed.", $"Uri: {uri}");
                }
            }

            return Task.FromResult(Unit.Value);
        }

        protected override TextDocumentCloseRegistrationOptions CreateRegistrationOptions(TextSynchronizationCapability capability, ClientCapabilities clientCapabilities) =>
            new() { DocumentSelector = _selector };
    }
}