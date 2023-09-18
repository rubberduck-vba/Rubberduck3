using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using Rubberduck.ServerPlatform;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using Rubberduck.LanguageServer.Handlers;
using Microsoft.Extensions.Logging;
using Rubberduck.LanguageServer.Services;
using Rubberduck.LanguageServer.Model;
using OmniSharpLanguageServer = OmniSharp.Extensions.LanguageServer.Server.LanguageServer;
using System.IO.Pipelines;
using System.Diagnostics;
using System.IO.Pipes;
using Nerdbank.Streams;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Rubberduck.LanguageServer
{

    public static class Program
    {
        private static TransportOptions _options = ServerArgs.Default;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static async Task<int> Main(string[] args)
        {
            await SetServerOptionsAsync(args);
            await StartAsync();
            return 0;
        }

        private static async Task SetServerOptionsAsync(string[] args)
        {
            _options = await ServerArgs.ParseAsync(args);
        }

        private static async Task StartAsync()
        {
            var tokenSource = new CancellationTokenSource();
            var services = new ServiceCollection();
            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);

            var server = OmniSharpLanguageServer.PreInit(options => ConfigureLanguageServer(options, tokenSource));
            await server.WaitForExit;
        }

        private static void ConfigureLanguageServer(LanguageServerOptions options, CancellationTokenSource tokenSource)
        {
            // example LSP server: https://github.com/OmniSharp/csharp-language-server-protocol/blob/master/sample/SampleServer/Program.cs

            var assembly = typeof(Program).Assembly.GetName();
            var name = assembly.Name ?? throw new InvalidOperationException();
            var version = assembly.Version?.ToString(3);

            var info = new ServerInfo
            {
                Name = name,
                Version = version
            };

            options.WithLoggerFactory(new LoggerFactory())
                   .AddDefaultLoggingProvider()
                   .WithServerInfo(info)
                   .WithServices(ConfigureServices)

                   .OnInitialize(async (server, request, token) =>
                   {
                       token.ThrowIfCancellationRequested();
                       // TODO
                       await Task.CompletedTask;
                   })
                   .OnInitialized(async (server, request, response, token) =>
                   {
                       token.ThrowIfCancellationRequested();
                       // TODO
                       await Task.CompletedTask;
                   })
                   .OnStarted(async (languageServer, token) =>
                   {
                       token.ThrowIfCancellationRequested();
                       // TODO
                       await Task.CompletedTask;
                   })

                   // General
                   .WithHandler<ShutdownHandler>()
                   .WithHandler<ExitHandler>()

                   // Server
                   .WithHandler<SetTraceHandler>()

                   // Workspace
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

                   // TextDocument
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


            ;

            switch (_options.TransportType)
            {
                case TransportType.StdIO:
                    
                    options.WithInput(Console.OpenStandardInput())
                           .WithOutput(Console.OpenStandardOutput());
                    break;

                case TransportType.Pipe:
                    var pipeOptions = (PipeTransportOptions)_options;
                    var pipe = new NamedPipeServerStream(pipeOptions.PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte);
                    options.WithInput(pipe.UsePipeReader())
                           .WithOutput(pipe.UsePipeWriter());
                    pipe.WaitForConnection();
                    break;

                default:
                    throw new NotSupportedException($"Transport type '{_options.TransportType}' is not supported.");
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SupportedLanguage, VisualBasicForApplicationsLanguage>();
            services.AddSingleton<DocumentContentStore>();
            services.AddSingleton<TextDocumentSyncHandler>();
        }
    }
}