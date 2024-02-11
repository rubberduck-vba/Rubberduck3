using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers
{
    public class DidChangeWorkspaceFoldersHandler : DidChangeWorkspaceFoldersHandlerBase
    {
        private readonly ILanguageServerFacade _server;
        private readonly SupportedLanguage _language;
        private readonly DocumentContentStore _contentStore;

        public DidChangeWorkspaceFoldersHandler(ILanguageServerFacade server, SupportedLanguage language, DocumentContentStore contentStore)
        {
            _language = language;
            _server = server;
            _contentStore = contentStore;
        }

        public async override Task<Unit> Handle(DidChangeWorkspaceFoldersParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // TODO update workspace folders accordingly

            return await Task.FromResult(Unit.Value);
        }

        protected override DidChangeWorkspaceFolderRegistrationOptions CreateRegistrationOptions(ClientCapabilities clientCapabilities)
        {
            return new DidChangeWorkspaceFolderRegistrationOptions
            {
                Supported = true,
                ChangeNotifications = true
            };
        }
    }
}