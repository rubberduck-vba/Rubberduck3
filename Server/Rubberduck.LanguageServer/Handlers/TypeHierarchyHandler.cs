using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.LanguageServer.Model;
using Rubberduck.LanguageServer.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers
{
    public class TypeHierarchyHandler : TypeHierarchyHandlerBase
    {
        private readonly ILanguageServerFacade _server;
        private readonly SupportedLanguage _language;
        private readonly DocumentContentStore _contentStore;

        private readonly TextDocumentSelector _selector;

        public TypeHierarchyHandler(ILanguageServerFacade server, SupportedLanguage language, DocumentContentStore contentStore)
        {
            _language = language;
            _server = server;
            _contentStore = contentStore;

            var filter = new TextDocumentFilter
            {
                Language = language.Id,
                Pattern = string.Join(';', language.FileTypes.Select(fileType => $"**/{fileType}").ToArray())
            };
            _selector = new TextDocumentSelector(filter);
        }

        public async override Task<Container<TypeHierarchyItem>?> Handle(TypeHierarchyPrepareParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var items = new List<TypeHierarchyItem>();
            // TODO

            var result = new Container<TypeHierarchyItem>(items);
            return await Task.FromResult(result);
        }

        public async override Task<Container<TypeHierarchyItem>?> Handle(TypeHierarchySupertypesParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var items = new List<TypeHierarchyItem>();
            // TODO

            var result = new Container<TypeHierarchyItem>(items);
            return await Task.FromResult(result);
        }

        public async override Task<Container<TypeHierarchyItem>?> Handle(TypeHierarchySubtypesParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var items = new List<TypeHierarchyItem>();
            // TODO

            var result = new Container<TypeHierarchyItem>(items);
            return await Task.FromResult(result);
        }

        protected override TypeHierarchyRegistrationOptions CreateRegistrationOptions(TypeHierarchyCapability capability, ClientCapabilities clientCapabilities)
        {
            return new TypeHierarchyRegistrationOptions
            {
                DocumentSelector = _selector,
                WorkDoneProgress = true
            };
        }
    }
}