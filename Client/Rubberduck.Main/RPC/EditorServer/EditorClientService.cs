using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nerdbank.Streams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.Main.RPC.EditorServer.Handlers;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using EditorClientOptions = OmniSharp.Extensions.LanguageServer.Client.LanguageClientOptions;

namespace Rubberduck.Main.RPC.EditorServer
{
    public class EditorClientService : ILanguageClientService
    {
        public event EventHandler Connecting = delegate { };
        public event EventHandler Connected = delegate { };
        public event EventHandler Disconnected = delegate { };

        private readonly ILogger _logger;
        private readonly ServerPlatformServiceHelper _service;

        public EditorClientService(ILogger<EditorClientService> logger, ServerPlatformServiceHelper serverPlatformServiceHelper)
        {
            _logger = logger;
            _service = serverPlatformServiceHelper;
        }

        public static EditorClientOptions ConfigureEditorClient(Assembly clientAssembly, NamedPipeClientStream pipe, long clientProcessId, RubberduckSettings settings, string path)
        {
            var options = new EditorClientOptions();
            options.WithInput(pipe.UsePipeReader());
            options.WithOutput(pipe.UsePipeWriter());

            ConfigureEditorClient(options, clientAssembly, clientProcessId, settings, path);
            return options;
        }

        public static EditorClientOptions ConfigureEditorClient(Assembly clientAssembly, Process serverProcess, long clientProcessId, RubberduckSettings settings, string path)
        {
            var options = new EditorClientOptions();
            options.WithInput(serverProcess.StandardOutput.BaseStream);
            options.WithOutput(serverProcess.StandardInput.BaseStream);

            ConfigureEditorClient(options, clientAssembly, clientProcessId, settings, path);
            return options;
        }

        private static void ConfigureClientLogging(ILoggingBuilder builder)
        {
            builder.AddNLog("NLog-editor.config");
        }

        public EditorClientOptions ConfigureLanguageClient(EditorClientOptions options, Assembly clientAssembly, long clientProcessId, RubberduckSettings settings, string workspaceRoot)
        {
            var info = clientAssembly.ToClientInfo();
            var initializationProgressToken = new ProgressToken(Guid.NewGuid().ToString());

            var workspace = new DirectoryInfo(workspaceRoot).ToWorkspaceFolder();
            var clientCapabilities = GetClientCapabilities();

            options.EnableDynamicRegistration();
            options.EnableProgressTokens(); // to support WorkDoneProgress requests/notifications
            
            // TODO disable when hosted in single-project application?
            options.EnableWorkspaceFolders(); // to support multipole workspaces/projects

            options.WithSerializer(new OmniSharp.Extensions.LanguageServer.Protocol.Serialization.LspSerializer(ClientVersion.Lsp3));
            options.ConfigureLogging(ConfigureClientLogging);

            var initializationOptions = new InitializationOptions
            {
                Timestamp = DateTime.Now,
                Locale = settings.GeneralSettings.Locale,
                HostApplication = Path.GetFileName(System.Windows.Forms.Application.ExecutablePath).ToUpper()
            };

            options
                .WithClientInfo(info)
                .WithClientCapabilities(clientCapabilities)
                // NOTE: using same trace level as language server here just to keep configurations "simple"
                .WithTrace(settings.LanguageServerSettings.TraceLevel.ToInitializeTrace())
                .WithInitializationOptions(JsonSerializer.Serialize(initializationOptions))
                .WithWorkspaceFolder(workspace)
                .WithRootUri(workspaceRoot)
                .WithContentModifiedSupport(true)

                // TODO

                //.WithHandler<WorkspaceFoldersHandler>()
                //.WithHandler<WorkDoneProgressCreateHandler>()
                //.WithHandler<WorkDoneProgressCancelHandler>()
                //.WithHandler<WorkDoneProgressHandler>()

                .WithHandler<ApplyWorkspaceEditHandler>()

                .OnInitialize(async (client, request, cancellationToken) =>
                {
                    // OnLanguageClientInitializeDelegate
                    // Gives your class or handler an opportunity to interact with the InitializeParams
                    // before it is sent to the server.

                    _logger.LogDebug("OnInitialize: sending InitializeParams...");
                    
                    Connecting.Invoke(this, EventArgs.Empty);
                    request.ConfigureInitialization(clientProcessId, settings.GeneralSettings.Locale);

                    _logger.LogDebug("OnInitialize completed (InitializeParams will be sent to LSP server)");
                })

                .OnInitialized(async (client, request, response, cancellationToken) =>
                {
                    // OnLanguageClientInitializedDelegate
                    // Gives your class or handler an opportunity to interact with the InitializeParams and InitializeResult
                    // before it is processed by the client.

                    _logger.LogDebug("OnInitialized: received Initialized notification.");

                    Connected.Invoke(this, EventArgs.Empty);

                    _logger.LogDebug("OnInitialized completed (further processing deferred to LSP client)");
                })

                .OnWorkDoneProgressCreate((request) =>
                {
                    _logger.LogDebug("OnWorkDoneProgressCreate: received WorkDoneProgressCreate notification.");
                    var token = request.Token;
                    if (token != null)
                    {
                        _logger.LogTrace("ProgressToken: {0}", token.String);
                        _service.OnProgress(token);
                    }
                })

                .OnProgress((request) =>
                {
                    _logger.LogDebug("OnProgress: received WorkDoneProgress notification.");
                    var token = request.Token;
                    if (token != null)
                    {
                        var stringReport = request.Value.ToString(Formatting.None);
                        _logger.LogTrace(stringReport);

                        var progress = JsonConvert.DeserializeObject<WorkDoneProgressReport>(stringReport);
                        _service.OnProgress(token, progress);
                    }
                })
            ;

            return options;
        }

        private static ClientCapabilities GetClientCapabilities()
        {
            var supported = new Supports<bool> { Value = true };

            var capabilities = new ClientCapabilities
            {
                Workspace = new WorkspaceClientCapabilities
                {
                    Configuration = supported,
                    ApplyEdit = supported,
                    WorkspaceFolders = supported,

                    DidChangeConfiguration = new()
                    {
                        Value = new()
                    },
                    ExecuteCommand = new()
                    {
                        Value = new()
                    },
                    FileOperations = new()
                    {
                        Value = new()
                        {
                            DidCreate = supported,
                            DidDelete = supported,
                            DidRename = supported, // really?
                        }
                    },
                    WorkspaceEdit = new()
                    {
                        Value = new()
                        {
                            DocumentChanges = supported, // NOTE: TextEdit is unhandled
                            FailureHandling = FailureHandlingKind.Abort,
                            NormalizesLineEndings = supported,
                            //ResourceOperations = supported
                        }
                    },
                    ExtensionData = new Dictionary<string, JToken>()
                },
                Window = null,
                TextDocument = null,

                //Experimental = new Dictionary<string, JToken>(),
                //ExtensionData = new Dictionary<string, JToken>()
            };

            return capabilities;
        }
    }
}
