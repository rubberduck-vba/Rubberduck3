using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.InternalApi.Settings.Model.General;
using System;
using System.IO;
using System.Reflection;

namespace Rubberduck.ServerPlatform
{
    public abstract class ServerPlatformClientService : ServerPlatformServiceBase, ILanguageClientService
    {
        public event EventHandler Connecting = delegate { };
        public event EventHandler Connected = delegate { };
        public event EventHandler Disconnected = delegate { };

        protected ServerPlatformClientService(ILogger<ServerPlatformServiceHelper> logger, RubberduckSettingsProvider settings, IWorkDoneProgressStateService workdone, PerformanceRecordAggregator performance)
            : base(logger, settings, workdone, performance)
        {
        }

        public LanguageClientOptions ConfigureLanguageClient(LanguageClientOptions options, Assembly clientAssembly, long clientProcessId, RubberduckSettings settings, string workspaceRoot)
        {
            var capabilities = GetClientCapabilities();

            options.WithSerializer(new LspSerializer(ClientVersion.Lsp3))
                .ConfigureLogging(ConfigureClientLogging)
                .EnableDynamicRegistration()
                .WithClientCapabilities(capabilities)
                .WithClientInfo(clientAssembly.ToClientInfo())
                .WithTrace(settings.LanguageServerSettings.TraceLevel.ToInitializeTrace())
            ;

            ProgressToken? initializationProgressToken = null;
            if (capabilities.Window?.WorkDoneProgress.IsSupported ?? false)
            {
                initializationProgressToken = new ProgressToken(Guid.NewGuid().ToString());
                options.EnableProgressTokens();
            }

            if (capabilities.Workspace?.WorkspaceFolders.IsSupported ?? false)
            {
                options.EnableWorkspaceFolders(); // supports multiple workspaces/projects
                if (workspaceRoot != null)
                {
                    var workspace = new DirectoryInfo(workspaceRoot).ToWorkspaceFolder();
                    options.WithWorkspaceFolder(workspace);
                }
            }

            if (workspaceRoot != null)
            {
                options.WithRootUri(workspaceRoot);
            }

            var initializationOptions = GetInitializationOptions(settings);
            if (initializationOptions != null)
            {
                options.WithInitializationOptions(System.Text.Json.JsonSerializer.Serialize(initializationOptions));
            }

            options
                .OnInitialize(async (client, request, cancellationToken) =>
                 {
                     // OnLanguageClientInitializeDelegate
                     // Gives your class or handler an opportunity to interact with the InitializeParams
                     // before it is sent to the server.

                     LogDebug("OnInitialize: sending InitializeParams...");

                     Connecting.Invoke(this, EventArgs.Empty);
                     request.ConfigureInitialization(clientProcessId, settings.GeneralSettings.Locale, initializationProgressToken);

                     LogDebug("OnInitialize completed (sending request).");
                 })

                .OnInitialized(async (client, request, response, cancellationToken) =>
                {
                    // OnLanguageClientInitializedDelegate
                    // Gives your class or handler an opportunity to interact with the InitializeParams and InitializeResult
                    // before it is processed by the client.

                    LogDebug("OnInitialized: received Initialized notification.");

                    Connected.Invoke(this, EventArgs.Empty);

                    LogDebug("OnInitialized completed.");
                })

                .OnWorkDoneProgressCreate((request) =>
                {
                    LogDebug("OnWorkDoneProgressCreate: received WorkDoneProgressCreate notification.");
                    var token = request.Token;
                    if (token != null)
                    {
                        LogTrace("ProgressToken: {0}", token.String);
                        OnProgress(token);
                    }
                })

                .OnProgress((request) =>
                {
                    LogDebug("OnProgress: received WorkDoneProgress notification.");
                    var token = request.Token;
                    if (token != null)
                    {
                        var stringReport = request.Value.ToString(Formatting.None);
                        LogTrace(stringReport);

                        var progress = JsonConvert.DeserializeObject<WorkDoneProgressReport>(stringReport);
                        OnProgress(token, progress);
                    }
                })
            ;

            return ConfigureLanguageClient(options);
        }

        protected virtual LanguageClientOptions ConfigureLanguageClient(LanguageClientOptions options) => options;

        /// <summary>
        /// Gets an optional object to send along with the LSP Initialize request.
        /// </summary>
        /// <returns>Unless overridden, returns an <c>InitializationOptions</c> object containing the current timestamp and configured locale.</returns>
        protected virtual object? GetInitializationOptions(RubberduckSettings settings) =>
            new InitializationOptions
            {
                Timestamp = DateTime.Now,
                Locale = settings.GeneralSettings.Locale ?? LocaleSetting.DefaultSettingValue,
            };

        protected virtual void ConfigureClientLogging(ILoggingBuilder builder)
        {
            builder.AddNLog("NLog-client.config");
        }

        protected static Supports<bool> Supported { get; } = new(true);
        protected static Supports<bool> NotSupported { get; } = new(false);

        protected abstract ClientCapabilities GetClientCapabilities();
    }
}
