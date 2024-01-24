using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;
using OmniSharp.Extensions.LanguageServer.Server;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.InternalApi.Settings.Model.LanguageClient;
using Rubberduck.InternalApi.Settings.Model.LanguageServer;
using Rubberduck.LanguageServer.Handlers.Lifecycle;
using Rubberduck.LanguageServer.Handlers.Workspace;
using Rubberduck.ServerPlatform;
using System;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Text.Json;
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
            services.AddSingleton<Process>(provider => Process.GetProcessById(options.ClientProcessId));
            
            services.AddSingleton<SupportedLanguage>(provider => SupportedLanguage.VBA);

            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<PerformanceRecordAggregator>();

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
            var store = server.Services.GetRequiredService<DocumentContentStore>();

            var rootUriString = ((ILanguageServerState)ServerState).RootUri!.GetFileSystemPath();
            service.LogTrace($"ServerState RootUri: {rootUriString}");
            var rootUri = new Uri(rootUriString);

            var projectFilePath = System.IO.Path.Combine(rootUriString, ProjectFile.FileName);
            var projectFileContent = System.IO.File.ReadAllText(projectFilePath);
            var projectFile = JsonSerializer.Deserialize<ProjectFile>(projectFileContent) ?? throw new InvalidOperationException("Project file could not be deserialized.");

            var srcRootPath = System.IO.Path.Combine(rootUriString, ProjectFile.SourceRoot);
            service.LogTrace($"Workspace source root: {srcRootPath}");

            progress?.OnNext(message: "Loading source files...", percentage: 10, cancellable: false);
            foreach (var item in projectFile.VBProject.Modules)
            {
                service.TryRunAction(() =>
                {
                    progress?.OnNext($"Loading file: {item.Name}", 10, false);

                    var path = System.IO.Path.Combine(srcRootPath, item.Uri);
                    var uri = new WorkspaceFileUri(item.Uri, rootUri);
                    var document = new DocumentState(uri, System.IO.File.ReadAllText(path))
                    {
                        IsOpened = item.IsAutoOpen
                    };

                    if (service.TraceLevel == TraceLevel.Verbose)
                    {
                        service.LogTrace($"Loaded: {uri}");
                    }

                    store.AddOrUpdate(uri, document);
                }, "LoadWorkspaceFile");
            }

            progress?.OnNext($"Loading project references...", percentage: 20, cancellable: false);
            foreach (var item in projectFile.VBProject.References)
            {
                progress?.OnNext($"Loading library: {item.Name}", 20, false);
                service.LogTrace($"TODO: load definitions from '{item.Uri}'.");
            }

            progress?.OnNext(message: "Parsing...", percentage: 25, cancellable: false);
            // todo: parse all source files in workspace, make code foldings immediately available for open documents

            progress?.OnNext(message: "Resolving...", percentage: 50, cancellable: false);
            // todo: make semantic highlighting immediately available for open documents

            progress?.OnNext(message: "Analyzing...", percentage: 75, cancellable: false);
            // todo: make diagnostics immediately available for open documents

            progress?.OnCompleted();
            if (progress != null)
            {
                service.LogTrace($"Progress token '{progress.WorkDoneToken}' has been completed.");
            }
        }
    }
}