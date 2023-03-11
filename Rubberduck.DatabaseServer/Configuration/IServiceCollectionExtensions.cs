using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using Rubberduck.DatabaseServer.Internal.Abstract;
using Rubberduck.DatabaseServer.Internal.HealthChecks;
using Rubberduck.DatabaseServer.Internal.Storage;
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
                .AddSingleton<IServerStateService, Application>()

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
