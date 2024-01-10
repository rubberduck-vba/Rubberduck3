using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;
using OmniSharp.Extensions.LanguageServer.Server;
using Rubberduck.Editor.RPC.EditorServer;
using Rubberduck.Editor.RPC.EditorServer.Handlers.Lifecycle;
using Rubberduck.Editor.RPC.EditorServer.Handlers.Workspace;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.LanguageServer.Model;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.SettingsProvider.Model.LanguageServer;
using System;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Rubberduck.Editor
{
    /// <summary>
    /// Configures LSP for editor <--> addin communications.
    /// </summary>
    public sealed class EditorServerApp : RubberduckServerApp<LanguageClientSettings, LanguageClientStartupSettings>
    {
        public EditorServerApp(ServerStartupOptions options, CancellationTokenSource tokenSource)
            : base(options, tokenSource, requireWorkspaceUri: false)
        {
        }

        protected override ServerState<LanguageClientSettings, LanguageClientStartupSettings> GetServerState(IServiceProvider provider)
            => provider.GetRequiredService<EditorServerState>();

        protected override void ConfigureServices(ServerStartupOptions options, IServiceCollection services)
        {
            //services.AddSingleton<ILanguageServerFacade>(provider => _languageServer);
            services.AddSingleton<ServerStartupOptions>(provider => options);
            //services.AddSingleton<Process>(provider => Process.GetProcessById(options.ClientProcessId));

            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<PerformanceRecordAggregator>();

            services.AddSingleton<ServerPlatformServiceHelper>();
            services.AddSingleton<EditorServerState>();
            services.AddSingleton<Func<EditorServerState>>(provider => () => (EditorServerState)ServerState);
            services.AddSingleton<IServerStateWriter>(provider => provider.GetRequiredService<EditorServerState>());

            services.AddSingleton<Func<LanguageClientStartupSettings>>(provider => () => provider.GetRequiredService<RubberduckSettingsProvider>().Settings.LanguageClientSettings.StartupSettings);
            services.AddSingleton<Func<LanguageServerStartupSettings>>(provider => () => provider.GetRequiredService<RubberduckSettingsProvider>().Settings.LanguageServerSettings.StartupSettings);
            services.AddSingleton<IDefaultSettingsProvider<RubberduckSettings>>(provider => RubberduckSettings.Default);
            services.AddSingleton<IDefaultSettingsProvider<LanguageServerSettings>>(provider => LanguageServerSettings.Default);
            services.AddSingleton<RubberduckSettingsProvider>();

            services.AddSingleton<SupportedLanguage, VisualBasicForApplicationsLanguage>();
            //services.AddSingleton<DocumentContentStore>();

            services.AddSingleton<IExitHandler, ExitHandler>();
            services.AddSingleton<IHealthCheckService<LanguageServerStartupSettings>, ClientProcessHealthCheckService<LanguageServerStartupSettings>>();

            services.AddSingleton<IWorkDoneProgressStateService, WorkDoneProgressStateService>();
            services.AddSingleton<ISettingsChangedHandler<RubberduckSettings>>(provider => provider.GetRequiredService<RubberduckSettingsProvider>());
            services.AddSingleton<DidChangeConfigurationHandler>();
        }

        protected override void ConfigureHandlers(LanguageServerOptions options)
        {
            options
                .WithHandler<ShutdownHandler>()
                .WithHandler<ExitHandler>()
                .WithHandler<DidChangeConfigurationHandler>()
            ;
        }

        protected async override Task OnServerStartedAsync(ILanguageServer server, CancellationToken token, IWorkDoneObserver? progress, ServerPlatformServiceHelper service)
        {
            service.LogTrace("Server app started.");

            // TODO
            
            progress?.OnCompleted();
            if (progress != null)
            {
                service.LogTrace($"Progress token '{progress.WorkDoneToken}' has been completed.");
            }
        }
    }
}
