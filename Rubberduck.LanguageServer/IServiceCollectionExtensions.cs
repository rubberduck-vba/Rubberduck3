﻿using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Server;
using Rubberduck.ServerPlatform.Model;
using Rubberduck.ServerPlatform.RPC;
using Rubberduck.ServerPlatform.RPC.DatabaseServer;
using System.IO.Pipes;

namespace Rubberduck.LanguageServer.Configuration
{
    internal static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRubberduckServerServices(this IServiceCollection services, ServerCapabilities config, CancellationTokenSource tokenSource) => 
            services
                .AddLanguageServer(ConfigureLSP)
                .AddJsonRpcServer(ServerPlatform.Settings.DatabaseServerPipeName, ConfigureDbClient)
                .ConfigureDbClientServices()
                //.AddOtherServicesHere()
            ;

        private static void ConfigureLSP(LanguageServerOptions lsp)
        {
            var (input, output) = WithAsyncServerNamedPipeTransport(ServerPlatform.Settings.LanguageServerPipeName);

            var assembly = typeof(Program).Assembly.GetName();

            var name = assembly.Name;
            var version = assembly.Version.ToString(3);

            lsp.WithInput(input)
               .WithOutput(output)

               //add LSP handlers here?

               .OnStarted(async (server, token) =>
               {
                   var request = new ConnectRequest
                   {
                       ClientInfo = new ClientProcessInfo
                       {
                           Name = name,
                           Version = version,
                           ProcessId = 0
                       }
                   };
                   var db = server.GetRequiredService<IJsonRpcServer>();
                   await db.SendRequest(JsonRpcMethods.DatabaseServer.Connect, request).Returning<ConnectResult>(token);
               });
            ;
        }

        private static void ConfigureDbClient(JsonRpcServerOptions rpc)
        {
            var (input, output) = WithAsyncClientNamedPipeTransport(ServerPlatform.Settings.DatabaseServerPipeName);
            var assemblies = new[]
            {
                typeof(Program).Assembly,
            };
            
            rpc.UseAssemblyAttributeScanning = true;
            rpc.WithInput(input)
               .WithOutput(output)
               .WithAssemblies(assemblies)
            ;
        }

        private static IServiceCollection ConfigureDbClientServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<Application>()
                //.AddSingleton<IDatabaseServerServiceProxy, DatabaseServerService>()
            ;
        }

        private static (Stream input, Stream output) WithAsyncServerNamedPipeTransport(string name)
        {
            var input = new NamedPipeServerStream(name, PipeDirection.InOut);
            //var output = new NamedPipeClientStream(".", name);
            return (input, input);
        }

        private static (Stream input, Stream output) WithAsyncClientNamedPipeTransport(string name)
        {
            var input = new NamedPipeClientStream(".", name);
            //var output = new NamedPipeServerStream(name);
            return (input, input);
        }
    }
}
