using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Server;
using Rubberduck.InternalApi;
using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Configuration
{
    internal static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRubberduckServerServices(this IServiceCollection services, ServerCapabilities config, CancellationTokenSource tokenSource) =>
            services
                .AddLanguageServer(ConfigureLSP)
            //.AddOtherServicesHere()
            ;

        private static void ConfigureLSP(LanguageServerOptions lsp)
        {
            var pipeStream = new NamedPipeServerStream(ServerPlatformSettings.LanguageServerPipeName, PipeDirection.InOut);

            var assembly = typeof(Program).Assembly.GetName();
            var name = assembly.Name ?? throw new InvalidOperationException();
            var version = assembly.Version?.ToString(3);

            var info = new ServerInfo
            {
                Name = name,
                Version = version
            };

            lsp.WithInput(pipeStream)
               .WithOutput(pipeStream)
               .WithServerInfo(info)
               .OnInitialize((server, request, token) => 
               {
                   // keep this info around, it describes everything the server needs to know about the client.

                   /*
                   request.ClientInfo;
                   request.Locale;
                   request.WorkspaceFolders;
                   request.ProcessId;
                   request.Capabilities;
                   */
                   return Task.CompletedTask;
               })
               //add LSP handlers here?

               .OnStarted((server, token) =>
               {
                   // synchronize workspace and proceed with the initial parse run

                   return Task.CompletedTask;
               })
            ;
        }
    }
}