using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Rubberduck.LanguageServer.Model;
using Rubberduck.LanguageServer.Services;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers
{
    public class DidCreateFileHandler : DidCreateFileHandlerBase
    {
        private readonly ILanguageServerFacade _server;
        private readonly SupportedLanguage _language;
        private readonly DocumentContentStore _contentStore;

        public DidCreateFileHandler(ILanguageServerFacade server, SupportedLanguage language, DocumentContentStore contentStore)
        {
            _language = language;
            _server = server;
            _contentStore = contentStore;
        }

        public async override Task<Unit> Handle(DidCreateFileParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var tasks = request.Files.Select(async file =>
            {
                var content = await File.ReadAllTextAsync(file.Uri.ToString(), cancellationToken);
                _contentStore.AddOrUpdate(file.Uri, new DocumentContent(content));
                _server.Window.LogInfo($"File '{file.Uri}' was added to content store.");
            });

            await Task.WhenAll(tasks);
            return Unit.Value;
        }

        protected override DidCreateFileRegistrationOptions CreateRegistrationOptions(FileOperationsWorkspaceClientCapabilities capability, ClientCapabilities clientCapabilities)
        {
            return new DidCreateFileRegistrationOptions
            {
                Filters = new Container<FileOperationFilter>(
                    new FileOperationFilter
                    {
                        Pattern = new FileOperationPattern
                        {
                            Glob = string.Join(';', _language.FileTypes.Select(fileType => $"**/{fileType}").ToArray()),
                            Matches = FileOperationPatternKind.File,
                            Options = new FileOperationPatternOptions { IgnoreCase = true }
                        }
                    })
            };
        }
    }
}