using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using Rubberduck.DatabaseServer.RPC;
using Rubberduck.DatabaseServer.RPC.Query;
using Rubberduck.ServerPlatform.Model.Entities;
using Rubberduck.ServerPlatform.RPC;
using System.IO.Pipes;

namespace Rubberduck.DatabaseServer.Configuration
{
    internal static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRubberduckServerServices(this IServiceCollection services, LocalDbServerCapabilities config, CancellationTokenSource cts)
        {
            //var server = JsonRpcServer.Create(ConfigureRPC);
            
            return services
                        .AddJsonRpcServer(ServerPlatform.Settings.DatabaseServerName, ConfigureRPC)
                    //.AddOtherServicesHere()
                    ;
        }

        private static void ConfigureRPC(JsonRpcServerOptions rpc)
        {
            var (input, output) = WithAsyncNamedPipeTransport(ServerPlatform.Settings.DatabaseServerPipeName);
            rpc.Concurrency = 8;
            rpc.UseAssemblyAttributeScanning = true;

            rpc.WithInput(input)
               .WithOutput(output)
               .WithActivityTracingStrategy(new CorrelationManagerTracingStrategy())

               //.AddHandler<InfoHandler>()
               //.AddHandler<ConnectHandler>()
               //.AddHandler<DisconnectHandler>()
               
               //.AddHandler<SaveNotificationHandler<IdentifierReference>>(JsonRpcMethods.Database.SaveIdentifierReference)
               //.AddHandler<SaveNotificationHandler<Local>>(JsonRpcMethods.Database.SaveLocal)
               //.AddHandler<SaveNotificationHandler<Member>>(JsonRpcMethods.Database.SaveMember)
               //.AddHandler<SaveNotificationHandler<Module>>(JsonRpcMethods.Database.SaveModule)
               //.AddHandler<SaveNotificationHandler<Parameter>>(JsonRpcMethods.Database.SaveParameter)
               //.AddHandler<SaveNotificationHandler<Project>>(JsonRpcMethods.Database.SaveProject)
               //.AddHandler<SaveNotificationHandler<DeclarationAnnotation>>(JsonRpcMethods.Database.QueryAnnotations)
               //.AddHandler<SaveNotificationHandler<DeclarationAttribute>>(JsonRpcMethods.Database.QueryAttributes)
               //.AddHandler<SelectQueryHandler<MemberInfo, MemberInfoRequestOptions>>(JsonRpcMethods.DatabaseServer.QueryMemberInfo)
               //.AddHandler<SelectQueryHandler<ModuleInfo, ModuleInfoRequestOptions>>(JsonRpcMethods.DatabaseServer.QueryModuleInfo)
               //.AddHandler<SelectQueryHandler<ProjectInfo, ProjectInfoRequestOptions>>(JsonRpcMethods.DatabaseServer.QueryProjectInfo)
            ;
        }

        private static (Stream input, Stream output) WithAsyncNamedPipeTransport(string name)
        {
            const int maxServerInstances = 1;
            var input = new NamedPipeServerStream(name, PipeDirection.InOut, maxServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            var output = new NamedPipeClientStream(".", name, PipeDirection.InOut, PipeOptions.Asynchronous);
            return (input, output);
        }
    }
}
