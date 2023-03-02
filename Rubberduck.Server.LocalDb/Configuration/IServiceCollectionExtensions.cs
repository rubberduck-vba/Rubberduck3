using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Shared;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Model.Database;
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
    }
}
