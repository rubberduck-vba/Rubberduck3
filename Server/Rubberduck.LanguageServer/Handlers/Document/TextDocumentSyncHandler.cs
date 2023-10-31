using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using Rubberduck.LanguageServer.Model;
using Rubberduck.LanguageServer.Services;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers.Document
{

    public class TextDocumentSyncHandler : TextDocumentSyncHandlerBase
    {
        private readonly ILanguageServerFacade _server;
        private readonly SupportedLanguage _language;
        private readonly DocumentContentStore _contentStore;

        private readonly TextDocumentSelector _selector;

        public TextDocumentSyncHandler(ILanguageServerFacade server, SupportedLanguage language, DocumentContentStore contentStore)
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

        public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
        {
            return new TextDocumentAttributes(uri, _language.Id);
        }

        public async override Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            var uri = request.TextDocument.Uri.ToUri();
            var text = request.TextDocument.Text;
            var content = new DocumentContent(text);

            cancellationToken.ThrowIfCancellationRequested();

            _contentStore.AddOrUpdate(uri, content);
            _server.Window.LogInfo($"DidOpenTextDocument: Updated content for document '{uri}'.");

            return await Task.FromResult(Unit.Value);
        }

        public async override Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
        {
            var uri = request.TextDocument.Uri.ToUri();
            var text = request.ContentChanges.LastOrDefault()?.Text ?? string.Empty;
            var content = new DocumentContent(text);

            cancellationToken.ThrowIfCancellationRequested();

            _contentStore.AddOrUpdate(uri, content);
            _server.Window.LogInfo($"DidChangeTextDocument: Updated content for document '{uri}'.");

            return await Task.FromResult(Unit.Value);
        }

        public async override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
        {
            var uri = request.TextDocument.Uri.ToUri();

            cancellationToken.ThrowIfCancellationRequested();

            // client is notifying about a saved document; does/should server care?
            // TODO trigger here whatever should be triggered on save...
            _server.Window.LogInfo($"DidSaveTextDocument: ACK document '{uri}' was saved.");

            return await Task.FromResult(Unit.Value);
        }

        public async override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
        {
            var uri = request.TextDocument.Uri.ToUri();

            cancellationToken.ThrowIfCancellationRequested();

            // client is notifying about closing a document in the editor; does/should server care?
            // NOTE document should remain in server cache; client may re-open the document any time.
            _server.Window.LogInfo($"DidCloseTextDocument: ACK document '{uri}' was closed.");

            return await Task.FromResult(Unit.Value);
        }

        protected override TextDocumentSyncRegistrationOptions CreateRegistrationOptions(TextSynchronizationCapability capability, ClientCapabilities clientCapabilities)
        {
            return new TextDocumentSyncRegistrationOptions
            {
                DocumentSelector = _selector,
                Save = capability.DidSave,
                Change = TextDocumentSyncKind.Full
            };
        }
    }
}