using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.ServerPlatform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers.Language
{
    public class DocumentSymbolHandler : DocumentSymbolHandlerBase
    {
        private readonly ServerPlatformServiceHelper _service;
        private readonly IWorkspaceStateManager _workspaces;
        private readonly TextDocumentSelector _selector;

        public DocumentSymbolHandler(ServerPlatformServiceHelper service, IWorkspaceStateManager workspaces, SupportedLanguage language)
        {
            _service = service;
            _workspaces = workspaces;
            _selector = language.ToTextDocumentSelector();
        }

        public async override Task<SymbolInformationOrDocumentSymbolContainer?> Handle(DocumentSymbolParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var items = new List<SymbolInformationOrDocumentSymbol>();
            
            if (!_service.TryRunAction(() =>
            {
                var documentUri = request.TextDocument.Uri;
                var workspace = _workspaces.ActiveWorkspace
                    ?? throw new InvalidOperationException("Invalid WorkspaceStateManager state: there is no active workspace.");

                var uri = new WorkspaceFileUri(documentUri.ToUri().OriginalString, workspace.WorkspaceRoot!.WorkspaceRoot);
                if (workspace.TryGetWorkspaceFile(uri, out var document) && document?.Symbol != null)
                {
                    items.Add(new SymbolInformationOrDocumentSymbol(document.Symbol));
                    _service.LogInformation($"Found symbol ({document.Symbol.GetType().Name}: {document.Symbol.Children!.Count()} children) for document at uri '{uri}'.");
                }
                else
                {
                    _service.LogWarning($"Could not find workspace file at uri '{uri}'. An empty collection will be returned.");
                }
            }, out var exception, nameof(DocumentSymbolHandler)) && exception != null)
            {
                // in case of failure, we throw here to return an error response:
                throw exception;
            }

            var result = new SymbolInformationOrDocumentSymbolContainer(items);
            return await Task.FromResult(result);
        }

        protected override DocumentSymbolRegistrationOptions CreateRegistrationOptions(DocumentSymbolCapability capability, ClientCapabilities clientCapabilities)
        {
            return new DocumentSymbolRegistrationOptions
            {
                DocumentSelector = _selector,
                WorkDoneProgress = true
            };
        }
    }
}