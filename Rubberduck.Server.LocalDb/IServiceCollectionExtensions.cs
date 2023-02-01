using Microsoft.Extensions.DependencyInjection;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Abstract;
using Rubberduck.Server.LocalDb.Services;

namespace Rubberduck.Server.LocalDb
{
    internal static class IServiceCollectionExtensions
    {
        internal static IServiceCollection AddRubberduckServerServices(this IServiceCollection services, ServerCapabilities config)
        {
            services.AddSingleton<LocalDbServer>()
                    .AddSingleton<NamedPipeStreamFactory>()
                    .AddSingleton<IServerProxyService<ServerCapabilities, IServerProxyClient>, LocalDbServerService>()
                    .AddSingleton<IServerConsoleService<ServerConsoleOptions>, ServerConsoleService<ServerConsoleOptions>>();
            

            return services;
        }
    }
}
