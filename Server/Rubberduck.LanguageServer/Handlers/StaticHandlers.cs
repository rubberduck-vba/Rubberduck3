using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers
{
    public static class StaticHandlers
    {
        public static async Task HandleAsync(ILanguageServer server, InitializeParams request, CancellationToken token)
        {
            var logger = server.GetRequiredService<ILogger<ServerApp>>();
            logger.LogInformation("Initialize request received!");

            token.ThrowIfCancellationRequested();
            // TODO store request.ClientInfo in server state.
            // TODO store request.Capabilities in server state (NOTE: may change with dynamic registration (also TODO! :D)).
            // TODO store request.WorkspaceFolders in server state (NOTE: replaces obsolete RootPath/RootUri properties)
            // TODO process any request.InitializationOptions object
            // TODO validate that the request.ProcessId matches the client process ID passed via command-line, if supplied (required for pipe transport).            
            await Task.CompletedTask;
        }

        public static async Task HandleAsync(ILanguageServer server, InitializeParams request, InitializeResult response, CancellationToken token)
        {
            var logger = server.GetRequiredService<ILogger<ServerApp>>();
            logger.LogInformation("Client confirmed reception of InitializeResult");

            token.ThrowIfCancellationRequested();
            // TODO populate response?
            await Task.CompletedTask;
        }

        public static async Task HandleAsync(ILanguageServer server, CancellationToken token)
        {
            var logger = server.GetRequiredService<ILogger<ServerApp>>();
            logger.LogInformation("Language server is started.");

            token.ThrowIfCancellationRequested();
            // TODO start processing server state asynchronously, notify client accordingly.
            await Task.CompletedTask;
        }
    }
}