using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.Parsing._v3.Pipeline;
using Rubberduck.ServerPlatform;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers.Document
{
    public class DidChangeTextDocumentHandler : DidChangeTextDocumentHandlerBase
    {
        private readonly ServerPlatformServiceHelper _service;
        private readonly IAppWorkspacesStateManager _workspaces;
        private readonly TextDocumentSelector _selector;

        private readonly DocumentPipeline _pipeline;
        private readonly WorkspacePipeline _workspacePipeline;

        public DidChangeTextDocumentHandler(ServerPlatformServiceHelper service, 
            IAppWorkspacesStateManager workspaces, 
            TextDocumentSelector selector, 
            DocumentPipeline pipeline, 
            WorkspacePipeline workspacePipeline)
        {
            _service = service;
            _workspaces = workspaces;
            _selector = selector;

            _pipeline = pipeline;
            _workspacePipeline = workspacePipeline;
        }

        public async override Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
        {
            var server = _workspacePipeline.Server ?? throw new InvalidOperationException("Could not get ILanguageServer reference.");

            var text = request.ContentChanges.First().Text;
            var version = request.TextDocument.Version ?? throw new InvalidOperationException("Document version was not supplied.");

            var workspace = _workspaces.ActiveWorkspace;
            var uri = new WorkspaceFileUri(request.TextDocument.Uri.ToUri().AbsoluteUri, workspace!.WorkspaceRoot!.WorkspaceRoot);

            if (workspace.TryGetWorkspaceFile(uri, out var document) && document is not null)
            {
                if (document.IsOpened)
                {
                    if (version > document.Version)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        workspace.LoadDocumentState(document with { Text = text, Version = version });
                        _service.LogInformation($"DidChangeTextDocument: Updated content for document '{uri}'.");
                        
                        await _pipeline.StartAsync(server, uri, new CancellationTokenSource());
                    }
                    else
                    {
                        _service.LogWarning($"Server already has a newer version than the notification is for. Request is ignored.", $"Uri: {uri} Version: {document.Version} (server) / {version} (request)");
                    }
                }
                else
                {
                    _service.LogWarning("Document was not opened.", $"Uri: {uri}");
                }
            }

            return Unit.Value;
        }

        protected override TextDocumentChangeRegistrationOptions CreateRegistrationOptions(TextSynchronizationCapability capability, ClientCapabilities clientCapabilities) =>
            new() { DocumentSelector = _selector };
    }
}