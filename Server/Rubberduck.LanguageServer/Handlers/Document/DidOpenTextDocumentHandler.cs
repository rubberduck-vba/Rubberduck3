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
    public class DidOpenTextDocumentHandler : DidOpenTextDocumentHandlerBase
    {
        private readonly ServerPlatformServiceHelper _service;
        private readonly IWorkspaceStateManager _workspaces;
        private readonly TextDocumentSelector _selector;

        public DidOpenTextDocumentHandler(ServerPlatformServiceHelper service, IWorkspaceStateManager workspaces, TextDocumentSelector selector)
        {
            _service = service;
            _workspaces = workspaces;
            _selector = selector;
        }

        public override Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            var workspace = _workspaces.ActiveWorkspace;
            var uri = new WorkspaceFileUri(request.TextDocument.Uri.ToUri().AbsoluteUri, workspace!.WorkspaceRoot!.WorkspaceRoot);

            if (workspace.TryGetWorkspaceFile(uri, out var document) && document is not null)
            {
                if (document.IsOpened)
                {
                    _service.LogWarning("Document was already opened.", $"Uri: {uri}");
                }

                cancellationToken.ThrowIfCancellationRequested();
                workspace.LoadDocumentState(document with { IsOpened = true });

                _service.LogInformation($"DidOpenTextDocument: Updated content for document '{uri}'.");
            }

            return Task.FromResult(Unit.Value);
        }

        protected override TextDocumentOpenRegistrationOptions CreateRegistrationOptions(TextSynchronizationCapability capability, ClientCapabilities clientCapabilities) => 
            new() { DocumentSelector = _selector };
    }
}