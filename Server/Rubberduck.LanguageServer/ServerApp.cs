using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;
using Rubberduck.ServerPlatform;
using System;
using System.Threading.Tasks;
using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using Rubberduck.LanguageServer.Handlers;
using Microsoft.Extensions.Logging;
using Rubberduck.LanguageServer.Services;
using Rubberduck.LanguageServer.Model;
using System.IO.Pipes;
using Nerdbank.Streams;
using OmniSharpLanguageServer = OmniSharp.Extensions.LanguageServer.Server.LanguageServer;

namespace Rubberduck.LanguageServer
{
    public sealed class ServerApp : IDisposable
    {
        private readonly TransportOptions _options;
        private readonly CancellationTokenSource _tokenSource = new();

        private ILogger<ServerApp>? _logger;

        public ServerApp(TransportOptions options)
        {
            _options = options;
        }

        public async Task RunAsync()
        {
            var services = new ServiceCollection();
            services.AddLogging(ConfigureLogging);

            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);
            using (var scope = provider.CreateScope())
            {
                _logger = scope.ServiceProvider.GetRequiredService<ILogger<ServerApp>>();
                _logger.LogInformation("Starting language server...");
                _logger.LogDebug($"Startup configuration:: TraceLevel='{_options.TraceLevel}' TransportType='{_options.TransportType}'");

                var server = OmniSharpLanguageServer.Create(options => ConfigureLanguageServer(options, _tokenSource), provider);
                
                _logger.LogInformation("Language server is ready. Awaiting client connection...");
                await server.WaitForExit;
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SupportedLanguage, VisualBasicForApplicationsLanguage>();
            services.AddSingleton<DocumentContentStore>();
        }

        private void ConfigureLogging(ILoggingBuilder builder)
        {
            builder.AddNLog("NLog.config");
        }

        private void ConfigureLanguageServer(LanguageServerOptions options, CancellationTokenSource tokenSource)
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

            options.WithServerInfo(info).WithServices(ConfigureServices);
            ConfigureTransport(options);

            options.OnInitialize(StaticHandlers.HandleAsync)
                   .OnInitialized(StaticHandlers.HandleAsync)
                   .OnStarted(StaticHandlers.HandleAsync)


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
        }

        private void ConfigureTransport(LanguageServerOptions options)
        {
            switch (_options.TransportType)
            {
                case TransportType.StdIO:
                    ConfigureStdIO(options);
                    break;

                case TransportType.Pipe:
                    ConfigurePipe(options);
                    break;

                default:
                    _logger?.LogWarning($"An unsupported transport type was specified.");
                    throw new UnsupportedTransportTypeException(_options.TransportType);
            }
        }

        private void ConfigureStdIO(LanguageServerOptions options)
        {
            var ioOptions = (StandardInOutTransportOptions)_options;
            _logger?.LogInformation($"Configuring language server transport to use standard input/output streams...");

            options.WithInput(Console.OpenStandardInput())
                   .WithOutput(Console.OpenStandardOutput());
        }

        private void ConfigurePipe(LanguageServerOptions options)
        {
            var ioOptions = (PipeTransportOptions)_options;

            _logger?.LogInformation($"Configuring language server transport to use a named pipe stream (mode: {ioOptions.Mode})...");
            var pipe = new NamedPipeServerStream(ioOptions.PipeName, PipeDirection.InOut, 1, ioOptions.Mode, PipeOptions.Asynchronous);
            options.WithInput(pipe.UsePipeReader())
                   .WithOutput(pipe.UsePipeWriter());
        }

        public void Dispose()
        {
            if (!_tokenSource.IsCancellationRequested)
            {
                _tokenSource.Cancel();
                _logger?.LogWarning("CancellationTokenSource was not cancelled; token may be disposed before ongoing operations ");
            }
            _tokenSource.Dispose();
        }

        public class UnsupportedTransportTypeException : NotSupportedException
        {
            public UnsupportedTransportTypeException(TransportType transportType)
                : base($"Transport type '{transportType}' is not supported.") { }

            public TransportType UnsupportedTransportType { get; }
        }
    }
}