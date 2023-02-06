using Microsoft.Extensions.DependencyInjection;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.LocalDbServer;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Telemetry;
using Rubberduck.Server.LocalDb.Internal;
using Rubberduck.Server.LocalDb.Properties;
using Rubberduck.Server.LocalDb.Services;
using Rubberduck.Server.Storage;
using System;
using System.IO.Pipes;
using System.Linq;

namespace Rubberduck.Server.LocalDb
{
    internal static class IServiceCollectionExtensions
    {
        internal static IServiceCollection AddRubberduckServerServices(this IServiceCollection services, LocalDbServerCapabilities config)
        {
            var pipeName = Settings.Default.JsonRpcPipeName;
            var maxConcurrentRequests = Settings.Default.MaxConcurrentRequests;

            var clientProxyTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.Contains("Rubberduck."))
                .SelectMany(a => a.GetTypes().Where(t => t.IsInterface && t.GetInterfaces().Contains(typeof(IJsonRpcSource))))
                .ToArray();

            services
                .AddSingleton<ServerApp>()
                .AddSingleton<IJsonRpcServer>(provider => new LocalDbServer(provider, clientProxyTypes))
                .AddSingleton<IRpcStreamFactory<NamedPipeServerStream>>(provider => new NamedPipeServerStreamFactory(pipeName, maxConcurrentRequests))
                .AddSingleton<IServerProxy<LocalDbServerCapabilities>>()
                .AddSingleton<ITelemetryClientService, TelemetryClientService>()
                .AddSingleton<IServerStateService<LocalDbServerCapabilities>>(provider => new ServerStateService<LocalDbServerCapabilities>(config))
                .AddSingleton<IServerConsoleProxy<LocalDbServerCapabilities>>(provider => new ServerConsoleProxyService<LocalDbServerCapabilities>(provider.GetService<IServerStateService<LocalDbServerCapabilities>>(), /* cannot inject the client proxy here */ null))
                .AddSingleton<IServerLogger>(provider =>
                {
                    var service = provider.GetService<IServerConsoleProxy<LocalDbServerCapabilities>>();
                    return new ServerLogger<LocalDbServerCapabilities>(service);
                })
                .AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>()
                .AddScoped<IDbConnectionProvider, SqliteDbConnectionProvider>()

                .AddScoped<IJsonRpcTarget, ServerConsoleProxyService<LocalDbServerCapabilities>>()
                .AddScoped<IJsonRpcTarget, LocalDbServerProxyService>()
                .AddScoped<IJsonRpcTarget, DeclarationsService>()
            ;

            return services;
        }
    }
}
