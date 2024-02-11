using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers.Workspace
{
    public class DidRenameFileHandler : DidRenameFileHandlerBase
    {
        private readonly ILanguageServerFacade _server;
        private readonly SupportedLanguage _language;
        private readonly DocumentContentStore _contentStore;

        public DidRenameFileHandler(ILanguageServerFacade server, SupportedLanguage language, DocumentContentStore contentStore)
        {
            _language = language;
            _server = server;
            _contentStore = contentStore;
        }
        public async override Task<Unit> Handle(DidRenameFileParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // type FileRename is not LSP-compliant... missing old/new URI... now what?

            //var tasks = request.Files.Select(file =>
            //{
            //    if (!_contentStore.TryRemove(file.Uri))
            //    {
            //        _server.Window.LogWarning($"File '{file.Uri}' could not be removed from content store.");
            //    }
            //    return Task.CompletedTask;
            //});

            //await Task.WhenAll(tasks);
            return await Task.FromResult(Unit.Value);
        }

        protected override DidRenameFileRegistrationOptions CreateRegistrationOptions(FileOperationsWorkspaceClientCapabilities capability, ClientCapabilities clientCapabilities)
        {
            return new DidRenameFileRegistrationOptions
            {
                Filters = new Container<FileOperationFilter>(
                    new FileOperationFilter
                    {
                        Pattern = new FileOperationPattern
                        {
                            Glob = string.Join(";", _language.FileTypes.Select(fileType => $"**/{fileType}").ToArray()),
                            Matches = FileOperationPatternKind.File,
                            Options = new FileOperationPatternOptions { IgnoreCase = true }
                        }
                    })
            };
        }
    }
}