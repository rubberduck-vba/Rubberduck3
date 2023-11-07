using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nerdbank.Streams;
using Newtonsoft.Json.Linq;
using NLog.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.Editor.RPC;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.Main.RPC.EditorServer.Handlers;
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
    public class EditorClientService
    {
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

        private static EditorClientOptions ConfigureEditorClient(EditorClientOptions options, Assembly clientAssembly, long clientProcessId, RubberduckSettings settings, string path)
        {
            var info = clientAssembly.ToClientInfo();

            var workspace = new DirectoryInfo(path).ToWorkspaceFolder();
            var clientCapabilities = GetClientCapabilities();

            options.EnableDynamicRegistration();
            options.EnableProgressTokens();
            options.EnableWorkspaceFolders();

            options.WithSerializer(new OmniSharp.Extensions.LanguageServer.Protocol.Serialization.LspSerializer(ClientVersion.Lsp3));
            options.ConfigureLogging(ConfigureClientLogging);

            var initializationOptions = new InitializationOptions
            {
                Timestamp = DateTime.Now,
                Locale = settings.GeneralSettings.Locale,
                //LibraryReferences = TODO[]
            };

            options
                .WithClientInfo(info)
                .WithClientCapabilities(clientCapabilities)
                .WithTrace(settings.LanguageServerSettings.TraceLevel.ToInitializeTrace())
                .WithInitializationOptions(JsonSerializer.Serialize(initializationOptions))
                .WithWorkspaceFolder(workspace)
                .WithContentModifiedSupport(true)

                .OnInitialize((client, request, cancellationToken) =>
                {
                    using var scope = client.CreateScope();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<EditorClientService>>();
                    logger.LogDebug("OnInitialize: sending InitializeParams...");

                    request.ConfigureInitialization(clientProcessId, settings.GeneralSettings.Locale);

                    return Task.CompletedTask;
                })

                .OnInitialized((client, request, response, cancellationToken) =>
                {
                    using var scope = client.CreateScope();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<EditorClientService>>();
                    logger.LogDebug("OnInitialized: received Initialized notification.");

                    return Task.CompletedTask;
                })

                // TODO

                //.WithHandler<WorkspaceFoldersHandler>()
                //.WithHandler<WorkDoneProgressCreateHandler>()
                //.WithHandler<WorkDoneProgressCancelHandler>()
                //.WithHandler<WorkDoneProgressHandler>()

                .WithHandler<ApplyWorkspaceEditHandler>()
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
