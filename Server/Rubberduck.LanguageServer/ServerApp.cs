using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nerdbank.Streams;
using NLog.Extensions.Logging;
using System;
using System.IO.Pipes;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using Rubberduck.LanguageServer.Handlers;
using Rubberduck.LanguageServer.Services;
using Rubberduck.LanguageServer.Model;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.ServerPlatform;
using OmniSharpLanguageServer = OmniSharp.Extensions.LanguageServer.Server.LanguageServer;

namespace Rubberduck.LanguageServer
{
    public sealed class ServerApp : IDisposable
    {
        private readonly TransportOptions _options;
        private readonly CancellationTokenSource _tokenSource;

        private ILogger<ServerApp>? _logger;
        private NamedPipeServerStream? _pipe;

        private OmniSharpLanguageServer? _languageServer;

        public ServerApp(TransportOptions options, CancellationTokenSource tokenSource)
        {
            _options = options;
            _tokenSource = tokenSource;
        }

        public async Task RunAsync()
        {
            var services = new ServiceCollection();
            services.AddLogging(ConfigureLogging);

            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);
            using (var scope = provider.CreateScope())
            {
                _logger = scope.ServiceProvider.GetRequiredService<ILogger<ServerApp>>();
                _logger.LogInformation("Rubberduck 3.0 Language Server v{0}", Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString(3) ?? "0.x");
                _logger.LogInformation("Starting language server...");
                _logger.LogDebug($"Startup configuration:: TraceLevel='{_options.TraceLevel}' TransportType='{_options.TransportType}'");

                _languageServer = await OmniSharpLanguageServer.From(options => ConfigureLanguageServer(options), provider, _tokenSource.Token);

                _logger.LogInformation("Awaiting client connection...");
                await _languageServer.WaitForExit;
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<SupportedLanguage, VisualBasicForApplicationsLanguage>();
            services.AddScoped<DocumentContentStore>();
        }

        private void ConfigureLogging(ILoggingBuilder builder)
        {
            builder.AddNLog("NLog.config"); // mind: release would deploy all executables to a single folder, so NLog.config is shared!
        }

        private void ConfigureLanguageServer(LanguageServerOptions options)
        {
            // example LSP server: https://github.com/OmniSharp/csharp-language-server-protocol/blob/master/sample/SampleServer/Program.cs

            ConfigureTransport(options);

            var assembly = GetType().Assembly.GetName();
            var name = assembly.Name ?? throw new InvalidOperationException();
            var version = assembly.Version?.ToString(3);

            var info = new ServerInfo
            {
                Name = name,
                Version = version
            };

            options
                .WithServerInfo(info)
                .WithServices(ConfigureServices)

            //.OnInitialize(StaticHandlers.HandleAsync)
            //.OnInitialized(StaticHandlers.HandleAsync)
            //.OnStarted(StaticHandlers.HandleAsync)


                .WithHandler<InitializedHandler>()
                .WithHandler<ShutdownHandler>()
                .WithHandler<ExitHandler>()
            /*/ Server
                .WithHandler<SetTraceHandler>()
            */

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
            _logger?.LogInformation($"Configuring language server transport to use standard input/output streams...");

            options.WithInput(Console.OpenStandardInput());
            options.WithOutput(Console.OpenStandardOutput());
        }

        private void ConfigurePipe(LanguageServerOptions options)
        {
            var ioOptions = (PipeTransportOptions)_options;
            _logger?.LogInformation($"Configuring language server transport to use a named pipe stream (mode: {ioOptions.Mode})...");

            _pipe = new NamedPipeServerStream(ioOptions.PipeName, PipeDirection.InOut, 1, ioOptions.Mode, PipeOptions.Asynchronous);

            options.WithInput(_pipe.UsePipeReader());
            options.WithOutput(_pipe.UsePipeWriter());
        }

        public void Dispose()
        {
            if (_pipe is object)
            {
                _logger?.LogDebug("Disposing NamedPipeServerStream...");
                _pipe.Dispose();
                _pipe = null;
            }

            if (_languageServer is object)
            {
                _logger?.LogDebug("Disposing LanguageServer...");
                _languageServer.Dispose();
                _languageServer = null;
            }
        }

        public class UnsupportedTransportTypeException : NotSupportedException
        {
            public UnsupportedTransportTypeException(TransportType transportType)
                : base($"Transport type '{transportType}' is not supported.") { }

            public TransportType UnsupportedTransportType { get; }
        }
    }
}