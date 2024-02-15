using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;
using OmniSharp.Extensions.LanguageServer.Server;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.InternalApi.Settings.Model.LanguageClient;
using Rubberduck.InternalApi.Settings.Model.LanguageServer;
using Rubberduck.LanguageServer.Handlers.Lifecycle;
using Rubberduck.LanguageServer.Handlers.Workspace;
using Rubberduck.Parsing._v3.Pipeline;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Parsers;
using Rubberduck.Parsing.PreProcessing;
using Rubberduck.Parsing.TokenStreamProviders;
using Rubberduck.ServerPlatform;
using System;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer
{
    public sealed class ServerApp : RubberduckServerApp<LanguageServerSettings, LanguageServerStartupSettings>
    {
        public ServerApp(ServerStartupOptions options, CancellationTokenSource tokenSource)
            : base(options, tokenSource, requireWorkspaceUri: true)
        {
        }

        public async override Task TestConfigAsync()
        {
            var services = new ServiceCollection();
            services.AddLogging(ConfigureLogging);

            ConfigureServices(StartupOptions, services);

            var provider = services.BuildServiceProvider();
            var logger = provider.GetRequiredService<ILogger<ServerApp>>();
            var app = provider.GetRequiredService<IWorkspaceService>();

            if (!string.IsNullOrWhiteSpace(StartupOptions.WorkspaceRoot))
            {
                var root = new Uri(StartupOptions.WorkspaceRoot);
                await app.OpenProjectWorkspaceAsync(root);
                logger.LogInformation("Workspace was loaded successfully.");

                var service = provider.GetRequiredService<WorkspaceParserPipeline>();
                await service.StartAsync(root, new ParserPipelineState(), TokenSource);
                logger.LogInformation("Workspace was processed successfully.");
            }
        }

        //private TextDocumentSelector GetSelector(SupportedLanguage language)
        //{
        //    var filter = new TextDocumentFilter
        //    {
        //        Language = language.Id,
        //        Pattern = string.Join(";", language.FileTypes.Select(fileType => $"**/{fileType}").ToArray())
        //    };
        //    return new TextDocumentSelector(filter);
        //}

        //private void HandleDidOpenTextDocument(DidOpenTextDocumentParams request, TextSynchronizationCapability capability, CancellationToken token)
        //{
        //    _logger?.LogDebug("Received DidOpenTextDocument notification.");
        //}

        //private TextDocumentOpenRegistrationOptions GetTextDocumentOpenRegistrationOptions(TextSynchronizationCapability capability, ClientCapabilities clientCapabilities)
        //{
        //    return new TextDocumentOpenRegistrationOptions
        //    {
        //        DocumentSelector = GetSelector(new VisualBasicForApplicationsLanguage())
        //    };
        //}

        protected override ServerState<LanguageServerSettings, LanguageServerStartupSettings> GetServerState(IServiceProvider provider) 
            => provider.GetRequiredService<LanguageServerState>();

        protected override void ConfigureServices(ServerStartupOptions options, IServiceCollection services)
        {
            //services.AddSingleton<ILanguageServerFacade>(provider => _languageServer);
            services.AddSingleton<ServerStartupOptions>(provider => options);

            if (options.ClientProcessId > 0)
            {
                services.AddSingleton<Process>(provider => Process.GetProcessById(options.ClientProcessId));
                services.AddSingleton<IHealthCheckService<LanguageServerStartupSettings>, ClientProcessHealthCheckService<LanguageServerStartupSettings>>();
            }

            services.AddSingleton<SupportedLanguage>(provider => SupportedLanguage.VBA);

            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<PerformanceRecordAggregator>();

            services.AddSingleton<IProjectFileService, ProjectFileService>();

            services.AddSingleton<IWorkspaceService, WorkspaceService>();
            services.AddSingleton<IWorkspaceStateManager, WorkspaceStateManager>();

            services.AddSingleton<ParserPipelineProvider>();
            services.AddTransient<WorkspaceParserPipeline>();
            services.AddTransient<WorkspaceFileParserPipeline>();
            services.AddTransient<DocumentMembersPipeline>();
            services.AddTransient<HierarchicalSymbolsPipeline>();

            services.AddSingleton<IParserPipelineFactory<WorkspaceParserPipeline>, ParserPipelineFactory<WorkspaceParserPipeline>>();
            services.AddSingleton<IParserPipelineFactory<WorkspaceFileParserPipeline>, ParserPipelineFactory<WorkspaceFileParserPipeline>>();
            services.AddSingleton<IParserPipelineFactory<DocumentMembersPipeline>, ParserPipelineFactory<DocumentMembersPipeline>>();
            services.AddSingleton<IParserPipelineFactory<HierarchicalSymbolsPipeline>, ParserPipelineFactory<HierarchicalSymbolsPipeline>>();
            services.AddSingleton<PipelineParseTreeSymbolsService>();
            services.AddSingleton<IResolverService, ResolverService>();
            services.AddSingleton<PipelineParserService>();
            services.AddSingleton<IParser<string>, TokenStreamParserAdapterWithPreprocessing<string>>();
            services.AddSingleton<ICommonTokenStreamProvider<string>, StringTokenStreamProvider>();
            services.AddSingleton<ITokenStreamParser, VBATokenStreamParser>();
            services.AddSingleton<ITokenStreamPreprocessor, VBAPreprocessor>();
            services.AddSingleton<VBAPreprocessorParser>();
            services.AddSingleton<ICompilationArgumentsProvider, WorkspaceCompilationArgumentsProvider>();

            services.AddSingleton<ServerPlatformServiceHelper>();
            services.AddSingleton<LanguageServerState>();
            services.AddSingleton<Func<LanguageServerState>>(provider => () => (LanguageServerState)ServerState);
            services.AddSingleton<IServerStateWriter>(provider => provider.GetRequiredService<LanguageServerState>());

            services.AddSingleton<Func<LanguageClientStartupSettings>>(provider => () => provider.GetRequiredService<RubberduckSettingsProvider>().Settings.LanguageClientSettings.StartupSettings);
            services.AddSingleton<Func<LanguageServerStartupSettings>>(provider => () => provider.GetRequiredService<RubberduckSettingsProvider>().Settings.LanguageServerSettings.StartupSettings);
            services.AddSingleton<IDefaultSettingsProvider<RubberduckSettings>>(provider => RubberduckSettings.Default);
            services.AddSingleton<IDefaultSettingsProvider<LanguageServerSettings>>(provider => LanguageServerSettings.Default);
            services.AddSingleton<RubberduckSettingsProvider>();

            services.AddSingleton<DocumentContentStore>();

            services.AddSingleton<IExitHandler, ExitHandler>();

            services.AddSingleton<IWorkDoneProgressStateService, WorkDoneProgressStateService>();
            services.AddSingleton<ISettingsChangedHandler<RubberduckSettings>>(provider => provider.GetRequiredService<RubberduckSettingsProvider>());
            services.AddSingleton<DidChangeConfigurationHandler>();
        }

        protected override void ConfigureHandlers(LanguageServerOptions options)
        {
            options
                .WithHandler<ShutdownHandler>()
                .WithHandler<ExitHandler>()

                //.OnDidOpenTextDocument(HandleDidOpenTextDocument, GetTextDocumentOpenRegistrationOptions)

            /*/ Workspace
                .WithHandler<DidChangeConfigurationHandler>()
                .WithHandler<DidChangeWatchedFileHandler>()
                .WithHandler<DidChangeWorkspaceFoldersHandler>()
                .WithHandler<DidCreateFileHandler>()
                .WithHandler<DidDeleteFileHandler>()
                .WithHandler<DidRenameFileHandler>()
                .WithHandler<ExecuteCommandHandler>()
                .WithHandler<SymbolInformationHandler>()
                .WithHandler<WorkspaceDiagnosticHandler>()
                .WithHandler<WorkspaceSymbolResolveHandler>()
                .WithHandler<WorkspaceSymbolsHandler>()
            */

            /*/ TextDocument
                .WithHandler<CallHierarchyHandler>()
                .WithHandler<CodeActionHandler>()
                .WithHandler<CodeActionResolveHandler>()
                .WithHandler<CodeLensHandler>()
                .WithHandler<ColorPresentationHandler>()
                .WithHandler<CompletionHandler>()
                .WithHandler<DeclarationHandler>()
                .WithHandler<DefinitionHandler>()
                .WithHandler<DocumentColorHandler>()
                .WithHandler<DocumentDiagnosticHandler>()
                .WithHandler<DocumentFormattingHandler>()
                .WithHandler<DocumentHighlightHandler>()
                .WithHandler<DocumentOnTypeFormattingHandler>()
                .WithHandler<DocumentRangeFormattingHandler>()
                .WithHandler<DocumentSymbolHandler>()
                .WithHandler<FoldingRangeHandler>()
                .WithHandler<HoverHandler>()
                .WithHandler<ImplementationHandler>()
                .WithHandler<PrepareRenameHandler>()
                .WithHandler<ReferencesHandler>()
                .WithHandler<RenameHandler>()
                .WithHandler<SelectionRangeHandler>()
                .WithHandler<SemanticTokensHandler>()
                .WithHandler<SemanticTokensFullHandler>()
                .WithHandler<SignatureHelpHandler>()
                .WithHandler<TextDocumentSyncHandler>()
                .WithHandler<TypeDefinitionHandler>()
                .WithHandler<TypeHierarchyHandler>()
            */
            ;
        }

        protected async override Task OnServerStartedAsync(ILanguageServer server, CancellationToken token, IWorkDoneObserver? progress, ServerPlatformServiceHelper service)
        {
            var workspaces = server.GetRequiredService<IWorkspaceService>();

            var state = (ILanguageServerState)ServerState;
            var rootUriString = state.RootUri!.GetFileSystemPath();
            service.LogTrace($"ServerState RootUri: {rootUriString}");
            var rootUri = new Uri(rootUriString);

            if (await workspaces.OpenProjectWorkspaceAsync(rootUri))
            {
                var pipeline = server.GetRequiredService<WorkspaceParserPipeline>();
                var parserState = new ParserPipelineState();
                await pipeline.StartAsync(new WorkspaceFileUri(null!, state.RootUri.ToUri()), parserState, new CancellationTokenSource())
                    .ContinueWith(t =>
                    {
                        progress?.OnCompleted();
                        if (progress != null)
                        {
                            service.LogTrace($"Progress token '{progress.WorkDoneToken}' has been completed.");
                        }
                    }, token, TaskContinuationOptions.None, TaskScheduler.Default);
            }
        }
    }
}