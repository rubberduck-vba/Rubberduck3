using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Server;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Properties;
using Rubberduck.Server.RPC;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace Rubberduck.Server.LSP.Configuration
{
    internal static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRubberduckServerServices(this IServiceCollection services, ServerCapabilities config, CancellationTokenSource tokenSource) => 
            services
                //.AddLanguageServer(ConfigureLSP)
                .AddJsonRpcServer(Settings.Default.DatabaseServerPipeName, ConfigureDbClient)
                .ConfigureDbClientServices()
                //.AddOtherServicesHere()
            ;

        private static void ConfigureLSP(LanguageServerOptions lsp)
        {
            var (input, output) = WithAsyncServerNamedPipeTransport(Settings.Default.LanguageServerPipeName);

            var assembly = typeof(Program).Assembly.GetName();

            var name = assembly.Name;
            var version = assembly.Version.ToString(3);

            lsp.WithInput(input)
               .WithOutput(output)

               //add LSP handlers here

               .OnStarted(async (server, token) =>
               {
                   var info = new RpcClientInfo 
                   {
                       Name = name,
                       Version = version,
                       ProcessId = 0
                   };
                   var db = server.GetService<IDatabaseServerServiceProxy>();
                   await db.ConnectAsync(info);
               });
            ;
        }

        private static void ConfigureDbClient(JsonRpcServerOptions rpc)
        {
            var (input, output) = WithAsyncClientNamedPipeTransport(Settings.Default.DatabaseServerPipeName);
            var assemblies = new[]
            {
                typeof(Rubberduck.Server.Program).Assembly,
                typeof(Rubberduck.RPC.Platform.JsonRpcMethods).Assembly,
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
                .AddSingleton<IDatabaseServerServiceProxy, DatabaseServerService>()
            ;
        }

        private static (Stream input, Stream output) WithAsyncServerNamedPipeTransport(string name)
        {
            var input = new NamedPipeServerStream(name, PipeDirection.InOut, 16, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            var output = new NamedPipeClientStream(".", name, PipeDirection.InOut, PipeOptions.Asynchronous);
            return (input, output);
        }

        private static (Stream input, Stream output) WithAsyncClientNamedPipeTransport(string name)
        {
            var input = new NamedPipeClientStream(".", name, PipeDirection.InOut, PipeOptions.Asynchronous);
            var output = new NamedPipeServerStream(name, PipeDirection.InOut, 16, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            return (input, output);
        }
    }
}
