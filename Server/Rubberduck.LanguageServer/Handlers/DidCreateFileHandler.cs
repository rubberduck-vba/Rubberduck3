using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.LanguageServer.Model;
using Rubberduck.LanguageServer.Services;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using System.Configuration;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers
{
    public class DidCreateFileHandler : DidCreateFileHandlerBase
    {
        private readonly ILogger _logger;
        private readonly ILanguageServerFacade _server;
        private readonly SupportedLanguage _language;
        private readonly DocumentContentStore _contentStore;
        private readonly IFileSystem _fileSystem;
        private readonly ISettingsProvider<LanguageServerSettings> _settings;

        public DidCreateFileHandler(ILogger<DidCreateFileHandler> logger, ILanguageServerFacade server, 
            SupportedLanguage language, 
            DocumentContentStore contentStore,
            IFileSystem fileSystem,
            ISettingsProvider<LanguageServerSettings> settings)
        {
            _logger = logger;
            _server = server;
            _language = language;
            _contentStore = contentStore;
            _fileSystem = fileSystem;
            _settings = settings;
        }

        public async override Task<Unit> Handle(DidCreateFileParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var elapsed = await TimedAction.RunAsync(
                Task.WhenAll(request.Files.Select(file =>
                    Task.Run(() =>
                    {
                        var content = _fileSystem.File.ReadAllText(file.Uri.ToString());
                        _contentStore.AddOrUpdate(file.Uri, new DocumentContent(content));
                        _server.Window.LogInfo($"File '{file.Uri}' was added to content store.");
                    })
                ))
            );

            var traceLevel = _settings.Value.Settings.TraceLevel.ToTraceLevel();
            _logger.LogPerformance("Finished reading content from created files.", elapsed, traceLevel);

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
                            Glob = string.Join(";", _language.FileTypes.Select(fileType => $"**/{fileType}").ToArray()),
                            Matches = FileOperationPatternKind.File,
                            Options = new FileOperationPatternOptions { IgnoreCase = true }
                        }
                    })
            };
        }
    }
}