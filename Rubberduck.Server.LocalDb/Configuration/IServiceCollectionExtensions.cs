using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using Rubberduck.RPC.Platform.Model.LocalDb;
using Rubberduck.Server.LocalDb.Properties;
using Rubberduck.Server.LocalDb.RPC.Connect;
using Rubberduck.Server.LocalDb.RPC.Disconnect;
using Rubberduck.Server.LocalDb.RPC.Info;
using Rubberduck.Server.LocalDb.RPC.Query;
using Rubberduck.Server.LocalDb.RPC.Save;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.Configuration
{
    internal static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRubberduckServerServices(this IServiceCollection services, LocalDbServerCapabilities config, CancellationTokenSource cts) => 
            services.AddJsonRpcServer(Settings.Default.JsonRpcServerName, ConfigureRPC)
                    //.AddOtherServicesHere()
            ;

        private static void ConfigureRPC(JsonRpcServerOptions rpc)
        {
            var (input, output) = WithAsyncNamedPipeTransport(Settings.Default.JsonRpcPipeName);            
            rpc.Concurrency = Settings.Default.MaxConcurrentRequests;

            rpc.WithRequestProcessIdentifier(new ParallelRequestProcessIdentifier())
               .WithMaximumRequestTimeout(TimeSpan.FromSeconds(10))

               .WithInput(input)
               .WithOutput(output)

               .AddHandler<InfoHandler>()
               .AddHandler<ConnectHandler>()
               .AddHandler<DisconnectHandler>()
               
               .AddHandler<SaveHandler<IdentifierReference>>()
               .AddHandler<SaveHandler<Local>>()
               .AddHandler<SaveHandler<Member>>()
               .AddHandler<SaveHandler<Module>>()
               .AddHandler<SaveHandler<Parameter>>()
               .AddHandler<SaveHandler<Project>>()
               .AddHandler<SaveHandler<DeclarationAnnotation>>()
               .AddHandler<SaveHandler<DeclarationAttribute>>()

               .AddHandler<SelectQueryHandler<ProjectInfo, ProjectInfoRequestOptions>>()
               .AddHandler<SelectQueryHandler<ModuleInfo, ModuleInfoRequestOptions>>()
               ;
        }

        private static (Stream input, Stream output) WithAsyncNamedPipeTransport(string name)
        {
            var input = new NamedPipeServerStream(name, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            var output = new NamedPipeClientStream(".", name, PipeDirection.InOut, PipeOptions.Asynchronous);
            return (input, output);
        }
    }
}
