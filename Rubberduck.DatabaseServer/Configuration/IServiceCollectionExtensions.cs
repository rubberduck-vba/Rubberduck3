﻿using Microsoft.Extensions.DependencyInjection;
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
        public static IServiceCollection AddRubberduckServerServices(this IServiceCollection services, LocalDbServerCapabilities config, CancellationTokenSource cts)
        {
            return ConfigureHealthChecks(services)
                .AddJsonRpcServer(ServerPlatform.Settings.DatabaseServerName, options =>
                {
                    options.Services = services;
                    ConfigureRPC(options);
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

        private static void ConfigureRPC(JsonRpcServerOptions rpc)
        {
            var (input, output) = WithAsyncNamedPipeTransport(ServerPlatform.Settings.DatabaseServerPipeName);
            rpc.Concurrency = 8;

            rpc.WithInput(input)
               .WithOutput(output)
               .WithAssemblyAttributeScanning(true)
               .WithAssemblies(typeof(Program).Assembly)
               .WithHandler<ConnectHandler>()
            ;
        }

        private static (Stream input, Stream output) WithAsyncNamedPipeTransport(string name)
        {
            var input = new NamedPipeServerStream(name);
            var output = new NamedPipeClientStream(".", name);
            return (input, output);
        }
    }
}
