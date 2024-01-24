using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.InternalApi.Settings.Model.LanguageServer;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers.Workspace
{
    public class DidCreateFileHandler : DidCreateFileHandlerBase
    {
        private readonly ILogger _logger;
        private readonly ILanguageServerFacade _server;
        private readonly SupportedLanguage _language;
        private readonly DocumentContentStore _contentStore;
        private readonly IFileSystem _fileSystem;
        private readonly ISettingsProvider<LanguageServerSettings> _settings;
        private readonly IWorkspaceState _workspace;

        public DidCreateFileHandler(ILogger<DidCreateFileHandler> logger, ILanguageServerFacade server,
            SupportedLanguage language,
            DocumentContentStore contentStore,
            IFileSystem fileSystem,
            IWorkspaceState workspace,
            ISettingsProvider<LanguageServerSettings> settings)
        {
            _logger = logger;
            _server = server;
            _language = language;
            _contentStore = contentStore;
            _fileSystem = fileSystem;
            _settings = settings;
            _workspace = workspace;
        }

        public override Task<Unit> Handle(DidCreateFileParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var elapsed = TimedAction.Run(() =>
                Task.WhenAll(request.Files.Select(file =>
                    Task.Run(() =>
                    {
                        var content = _fileSystem.File.ReadAllText(file.Uri.ToString());
                        var root = _workspace.WorkspaceRoot!;
                        var uri = new WorkspaceFileUri(file.Uri.OriginalString, root);

                        var state = new DocumentState(uri, content);
                        _contentStore.AddOrUpdate(file.Uri, state);
                        _server.Window.LogInfo($"File '{file.Uri}' was added to content store.");
                    })
                )).Wait()
            );

            var traceLevel = _settings.Settings.TraceLevel.ToTraceLevel();
            _logger.LogPerformance(traceLevel, "Finished reading content from created files.", elapsed);

            return Task.FromResult(Unit.Value);
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