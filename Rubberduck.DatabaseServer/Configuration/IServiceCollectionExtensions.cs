using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Model.Database;
using Rubberduck.RPC.Properties;
using System.IO.Pipes;

namespace Rubberduck.DatabaseServer.Configuration
{
    internal static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRubberduckServerServices(this IServiceCollection services, LocalDbServerCapabilities config, CancellationTokenSource cts) 
            => services
                .AddJsonRpcServer(Settings.Default.DatabaseServerPipeName, ConfigureRPC)
                    //.AddOtherServicesHere()
            ;

        private static void ConfigureRPC(JsonRpcServerOptions rpc)
        {
            var (input, output) = WithAsyncNamedPipeTransport(Settings.Default.DatabaseServerPipeName);
            rpc.Concurrency = 8;

            rpc.WithRequestProcessIdentifier(new ParallelRequestProcessIdentifier())
               .WithMaximumRequestTimeout(TimeSpan.FromSeconds(10))

               .WithInput(input)
               .WithOutput(output)

               .AddHandler<InfoHandler>()
               .AddHandler<ConnectHandler>()
               .AddHandler<DisconnectHandler>()
               
               .AddHandler<SaveNotificationHandler<IdentifierReference>>(JsonRpcMethods.Database.SaveIdentifierReference)
               .AddHandler<SaveNotificationHandler<Local>>(JsonRpcMethods.Database.SaveLocal)
               .AddHandler<SaveNotificationHandler<Member>>(JsonRpcMethods.Database.SaveMember)
               .AddHandler<SaveNotificationHandler<Module>>(JsonRpcMethods.Database.SaveModule)
               .AddHandler<SaveNotificationHandler<Parameter>>(JsonRpcMethods.Database.SaveParameter)
               .AddHandler<SaveNotificationHandler<Project>>(JsonRpcMethods.Database.SaveProject)
               .AddHandler<SaveNotificationHandler<DeclarationAnnotation>>(JsonRpcMethods.Database.QueryAnnotations)
               .AddHandler<SaveNotificationHandler<DeclarationAttribute>>(JsonRpcMethods.Database.QueryAttributes)

               .AddHandler<SelectQueryHandler<MemberInfo, MemberInfoRequestOptions>>(JsonRpcMethods.Database.QueryMemberInfo)
               .AddHandler<SelectQueryHandler<ModuleInfo, ModuleInfoRequestOptions>>(JsonRpcMethods.Database.QueryModuleInfo)
               .AddHandler<SelectQueryHandler<ProjectInfo, ProjectInfoRequestOptions>>(JsonRpcMethods.Database.QueryProjectInfo)
            ;
        }

        private static (Stream input, Stream output) WithAsyncNamedPipeTransport(string name)
        {
            const int maxServerInstances = 1;
            var input = new NamedPipeServerStream(name, PipeDirection.InOut, maxServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            var output = new NamedPipeClientStream(".", name, PipeDirection.InOut, PipeOptions.Asynchronous);
            return (input, output);
        }

        private static void ConfigureMediatR(MediatRServiceConfiguration settings)
        {
            settings.Lifetime = ServiceLifetime.Singleton;
        }
    }
}
