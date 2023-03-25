using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using Rubberduck.DatabaseServer.Internal.Abstract;
using Rubberduck.DatabaseServer.Internal.HealthChecks;
using Rubberduck.DatabaseServer.Internal.Storage;
using Rubberduck.DatabaseServer.RPC;
using Rubberduck.ServerPlatform.Services;
using System.IO.Pipes;

namespace Rubberduck.DatabaseServer.Configuration
{
    internal static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRubberduckServerServices(this IServiceCollection services, LocalDbServerCapabilities config, NamedPipeServerStream pipe, CancellationTokenSource cts)
        {
            return ConfigureHealthChecks(services)
                .AddJsonRpcServer(ServerPlatform.Settings.DatabaseServerName, options =>
                {
                    options.Services = services;
                    ConfigureRPC(options, pipe);
                })
                .AddSingleton<Application>()
                .AddSingleton<IServerStateService, ServerStateService>()

                .AddSingleton<IDbConnectionProvider, SqliteDbConnectionProvider>()
                .AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>()
            ;
        }

        private static IServiceCollection ConfigureHealthChecks(IServiceCollection services)
        {
            return services
                .AddSingleton<IHealthCheckService, HealthCheckService>()
                .AddSingleton<HealthCheck, CheckProjects>()
            ;
        }

        private static void ConfigureRPC(JsonRpcServerOptions rpc, NamedPipeServerStream pipe)
        {            
            rpc.Concurrency = 8;

            rpc.WithInput(pipe)
               .WithOutput(pipe)
               .WithHandler<ConnectHandler>()
            ;
        }
    }
}
