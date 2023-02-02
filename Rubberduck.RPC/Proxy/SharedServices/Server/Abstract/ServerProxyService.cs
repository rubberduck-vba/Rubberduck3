using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Abstract
{
    /// <summary>
    /// A common base class for all server proxy services that expose the server-side RPC targets.
    /// </summary>
    /// <typeparam name="TOptions">The class type of the configuration options for this service.</typeparam>
    /// <typeparam name="TCommands">The class type of the class that exposes the commands for this service.</typeparam>
    public abstract class ServerProxyService<TOptions, TCommands> : IConfigurableServerProxy<TOptions, IServerProxyClient>
        where TOptions : SharedServerCapabilities, new()
        where TCommands : class
    {
        protected ServerProxyService(IServerLogger logger, IServerStateService<TOptions> serverStateService)
        {
            Logger = logger;
            ServerStateService = serverStateService;
            // TODO handle ClientProxy events
        }

        public TOptions Configuration => ServerStateService.Configuration;
        public abstract TCommands Commands { get; }
        public virtual IServerLogger Logger { get; }

        protected IServerStateService<TOptions> ServerStateService { get; }

        public IServerProxyClient ClientProxy { get; }
    }
}
