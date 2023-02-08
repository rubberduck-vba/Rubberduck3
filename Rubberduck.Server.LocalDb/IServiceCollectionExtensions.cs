using Microsoft.Extensions.DependencyInjection;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.LocalDbServer;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Telemetry;
using Rubberduck.Server.LocalDb.Internal;
using Rubberduck.Server.LocalDb.Properties;
using Rubberduck.Server.LocalDb.Services;
using Rubberduck.Server.Storage;
using System;
using System.IO.Pipes;
using System.Linq;
using System.Threading;

namespace Rubberduck.Server.LocalDb
{
    internal static class IServiceCollectionExtensions
    {
        internal static IServiceCollection ConfigureRubberduckServerApp(this IServiceCollection services, LocalDbServerCapabilities config, CancellationTokenSource cts)
        {
            return services
                .AddRubberduckServerServices(config, cts)
                .AddJsonRpcTargets()
                .AddServerProxyServices()
                .AddConsoleProxyServices()
            ;
        }

        /// <summary>
        /// Registers <c>IJsonRpcTarget</c> implementations (RPC endpoints).
        /// </summary>
        private static IServiceCollection AddJsonRpcTargets(this IServiceCollection services)
        {
            return services
                .AddScoped<IJsonRpcTarget, ServerConsoleProxyService<LocalDbServerCapabilities>>()
                .AddScoped<IJsonRpcTarget, LocalDbServerProxyService>()
                .AddScoped<IJsonRpcTarget, DeclarationsService>()

            ;
        }

        /// <summary>
        /// Registers server proxy services.
        /// </summary>
        private static IServiceCollection AddServerProxyServices(this IServiceCollection services)
        {
            return services
                .AddScoped<IServerLogger>(provider =>
                {
                    var service = provider.GetService<IServerConsoleProxy<LocalDbServerCapabilities>>();
                    return new ServerLogger<LocalDbServerCapabilities>(service);
                })
            ;
        }

        /// <summary>
        /// Registers console proxy services.
        /// </summary>
        private static IServiceCollection AddConsoleProxyServices(this IServiceCollection services)
        {
            return services
            ;
        }

        /// <summary>
        /// Registers server-level common services as singletons.
        /// </summary>
        private static IServiceCollection AddRubberduckServerServices(this IServiceCollection services, LocalDbServerCapabilities config, CancellationTokenSource cts)
        {
            var pipeName = Settings.Default.JsonRpcPipeName;
            var maxConcurrentRequests = Settings.Default.MaxConcurrentRequests;

            var clientProxyTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.Contains("Rubberduck."))
                .SelectMany(a => a.GetTypes().Where(t => t.IsInterface && t.GetInterfaces().Contains(typeof(IJsonRpcSource))))
                .ToArray();

            return services
                // ServerApp inherits RubberduckServerApp and runs as a BackgroundService.
                .AddSingleton<ServerApp>()
                .AddSingleton<CancellationTokenSource>(provider => cts)
                // ServerApp(IJsonRpcServer,IEnumerable<IJsonRpcTarget>,IServerStateService<LocalDbServerCapabilities>)
                .AddSingleton<IJsonRpcServer>(provider => new LocalDbServer(provider, clientProxyTypes))
                //...

                // ServerConsoleProxyService(IServerStateService<TOptions>,IServerConsoleProxyClient>)

                .AddSingleton<IRpcStreamFactory<NamedPipeServerStream>>(provider => new NamedPipeServerStreamFactory(pipeName, maxConcurrentRequests))

                .AddSingleton<IServerStateService<LocalDbServerCapabilities>>(provider => new ServerStateService<LocalDbServerCapabilities>(config))

                .AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>()
                .AddSingleton<IDbConnectionProvider, SqliteDbConnectionProvider>()

            ;
        }
    }
}
