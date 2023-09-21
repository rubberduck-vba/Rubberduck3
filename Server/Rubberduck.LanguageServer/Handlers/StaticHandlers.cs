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
            token.ThrowIfCancellationRequested();
            server.GetRequiredService<ILogger<ServerApp>>().LogInformation("Initialize request received");
            // TODO?
            await Task.CompletedTask;
        }

        public static async Task HandleAsync(ILanguageServer server, InitializeParams request, InitializeResult response, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            server.GetRequiredService<ILogger<ServerApp>>().LogInformation("Client confirmed reception of InitializeResult");
            // TODO
            await Task.CompletedTask;
        }

        public static async Task HandleAsync(ILanguageServer server, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            server.GetRequiredService<ILogger<ServerApp>>().LogInformation("Language server started");
            // TODO
            await Task.CompletedTask;
        }
    }
}