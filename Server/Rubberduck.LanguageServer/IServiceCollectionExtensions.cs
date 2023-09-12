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

            IObserver<WorkDoneProgressReport>? workDone = null!;

            lsp.WithInput(pipeStream)
               .WithOutput(pipeStream)
               .WithServerInfo(info)
               .OnInitialize(async (server, request, token) => 
               {
                   var manager = server.WorkDoneManager.For(
                       request, new WorkDoneProgressBegin
                       {
                           Title = "Server is starting...",
                           Percentage = 10,
                       }
                   );
                   workDone = manager;

                   manager.OnNext(
                       new WorkDoneProgressReport
                       {
                           Percentage = 20,
                           Message = "Initializing..."
                       }
                   );

                   // keep this info around, it describes everything the server needs to know about the client:

                   /*
                   request.ClientInfo;
                   request.Locale;
                   request.WorkspaceFolders;
                   request.ProcessId;
                   request.Capabilities;
                   */

                   // use manager.OnNext to notify LSP client of progress.

                   manager.OnNext(
                       new WorkDoneProgressReport
                       {
                           Percentage = 50,
                           Message = "Awaiting client initialization..."
                       }
                   );

                   await Task.CompletedTask;
               })

               .OnInitialized(async (server, request, response, token) =>
               {
                   workDone.OnNext(
                        new WorkDoneProgressReport
                        {
                            Percentage = 60,
                            Message = "Initializing workspace..."
                        }
                    );

                   // TODO

                   workDone.OnNext(
                        new WorkDoneProgressReport
                        {
                            Percentage = 100,
                            Message = "Completed."
                        }
                    );

                   workDone.OnCompleted();
                   (workDone as IDisposable)?.Dispose();
                   workDone = null;

                   await Task.CompletedTask;
               })

               .OnStarted((server, token) =>
               {
                   // synchronize workspace and proceed with the initial parse run
                   using (var manager = server.WorkDoneManager.Create(new WorkDoneProgressBegin 
                       { 
                           Title = "",
                           Message = "",
                           Cancellable = true
                       }))
                   {

                   }
                   return Task.CompletedTask;
               })

            // add LSP handlers here
            ;
        }
    }
}