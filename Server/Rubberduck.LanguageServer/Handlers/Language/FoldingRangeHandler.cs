using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Workspace;
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
    public class FoldingRangeHandler : FoldingRangeHandlerBase
    {
        private readonly ServerPlatformServiceHelper _service;
        private readonly IAppWorkspacesStateManager _workspaces;
        private readonly TextDocumentSelector _selector;

        public FoldingRangeHandler(ServerPlatformServiceHelper service, IAppWorkspacesStateManager workspaces, SupportedLanguage language)
        {
            _service = service;
            _workspaces = workspaces;
            _selector = language.ToTextDocumentSelector();
        }

        public async override Task<Container<FoldingRange>?> Handle(FoldingRangeRequestParam request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var items = new List<FoldingRange>();

            if (!_service.TryRunAction(() =>
            {
                var documentUri = request.TextDocument.Uri;
                var workspace = _workspaces.ActiveWorkspace
                    ?? throw new InvalidOperationException("Invalid WorkspaceStateManager state: there is no active workspace.");


                var relativeUri = documentUri.ToUri().OriginalString;
                var marker = $"/{WorkspaceUri.SourceRootName}";
                var srcRootIndex = relativeUri.IndexOf(marker);
                if (srcRootIndex > 0)
                {
                    var fullPath = relativeUri.Substring(srcRootIndex + marker.Length);
                    relativeUri = fullPath;
                }

                var uri = new WorkspaceFileUri(relativeUri, workspace.WorkspaceRoot!.WorkspaceRoot);
                if (workspace.TryGetSourceFile(uri, out var document) && document != null)
                {
                    items.AddRange(document.Foldings);
                    _service.LogInformation($"Found {document.Foldings.Count} foldings for document at uri '{uri}'.");
                }
                else
                {
                    _service.LogWarning($"Could not find workspace file at uri '{uri}'. An empty collection will be returned.");
                }
            }, out var exception, nameof(FoldingRangeHandler)) && exception != null)
            {
                // in case of failure, we throw here to return an error response:
                throw exception;
            }

            var result = new Container<FoldingRange>(items);
            return await Task.FromResult(result);
        }

        protected override FoldingRangeRegistrationOptions CreateRegistrationOptions(FoldingRangeCapability capability, ClientCapabilities clientCapabilities)
        {
            return new FoldingRangeRegistrationOptions
            {
                DocumentSelector = _selector,
                WorkDoneProgress = true
            };
        }
    }
}