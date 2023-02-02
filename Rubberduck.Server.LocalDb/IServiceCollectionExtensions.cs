using Microsoft.Extensions.DependencyInjection;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.LocalDbServer;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Telemetry;
using Rubberduck.Server.LocalDb.Internal;
using Rubberduck.Server.LocalDb.Properties;
using Rubberduck.Server.LocalDb.Services;
using Rubberduck.Server.Storage;
using System.IO.Pipes;

namespace Rubberduck.Server.LocalDb
{
    internal static class IServiceCollectionExtensions
    {
        internal static IServiceCollection AddRubberduckServerServices(this IServiceCollection services, ServerCapabilities config)
        {
            var pipeName = Settings.Default.JsonRpcPipeName;
            var maxConcurrentRequests = Settings.Default.MaxConcurrentRequests;

            services
                .AddSingleton<ServerApp>()
                .AddSingleton<IJsonRpcServer>(provider => new LocalDbServer(provider))
                .AddSingleton<IRpcStreamFactory<NamedPipeServerStream>>(provider => new NamedPipeStreamFactory(pipeName, maxConcurrentRequests))
                .AddSingleton<IServerProxyService<ServerCapabilities, IServerProxyClient>, LocalDbServerService>()
                .AddSingleton<ITelemetryClientService, TelemetryClientService>()
                .AddSingleton<IServerStateService<ServerCapabilities>>(provider => new ServerStateService<ServerCapabilities>(config))
                .AddSingleton<IServerConsoleService<ServerCapabilities>>(provider => new ServerConsoleService<ServerCapabilities>(provider.GetService<IServerStateService<ServerCapabilities>>()))
                .AddSingleton<IServerLogger>(provider =>
                {
                    var console = provider.GetService<IServerConsoleService<ServerCapabilities>>();
                    return new ServerLogger(console.Log, console.Log);
                })
                .AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>()
                .AddScoped<IDbConnectionProvider, SqliteDbConnectionProvider>()

                .AddScoped<IJsonRpcTarget, ServerConsoleService<ServerCapabilities>>()
                .AddScoped<IJsonRpcTarget, LocalDbServerService>()
                .AddScoped<IJsonRpcTarget, DeclarationsService>()
                    ;

            return services;
        }
    }
}
